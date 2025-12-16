using Dapper;
using Griddlers.Library;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Griddlers.Database;

public class GriddlerDbContext : DbContext
{
    private DbSet<Griddler> Griddlers { get; set; } = null!;
    private DbSet<GriddlerItem> GriddlerItems { get; set; } = null!;
    private DbSet<GriddlerPath> GriddlerPaths { get; set; } = null!;

    public GriddlerDbContext(DbContextOptions<GriddlerDbContext> options) : base(options)
    {

    }


    private T WithConnection<T>(Func<IDbConnection, T> ret)
    {
        string cnnString = Database.GetDbConnection().ConnectionString;
        SqlConnection cnn = new SqlConnection(cnnString);

        try
        {
            cnn.Open();
            return ret(cnn);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Griddler[]> ListGriddlers()
        => await Griddlers.FromSqlRaw("dbo.usp_list_griddlers").ToArrayAsync();

    //public async Task<GriddlerPath[]> ListGriddlerPaths()
    //    => await GriddlerPaths.FromSqlRaw("dbo.usp_list_griddler_paths").ToArrayAsync();

    public async Task<int> SaveGriddlerPath(short id,
                                            Point[] points)
    {
        return await WithConnection(async c =>
        {
            DataTable Data = new DataTable();
            Data.Columns.Add("action_id", typeof(byte));
            Data.Columns.Add("x_position", typeof(byte));
            Data.Columns.Add("y_position", typeof(byte));
            Data.Columns.Add("group_num", typeof(short));
            Data.Columns.Add("sort_order", typeof(short));

            short Count = 0;
            foreach (Point item in points)
                Data.Rows.Add(item.Action, item.X, item.Y, item.Grp, Count++);

            DynamicParameters p = new DynamicParameters();
            p.Add("@id", id);
            p.Add("@data", Data.AsTableValuedParameter());

            await c.ExecuteAsync("dbo.usp_save_griddler_path", p, null, null, CommandType.StoredProcedure);

            return id;
        });
    }


    public async Task<Griddler> GetGriddler(short? griddlerID, string name)
    {
        return await WithConnection(async c =>
        {
            Griddler Griddler = new Griddler();

            DynamicParameters p = new DynamicParameters();
            p.Add("@griddlerID", griddlerID);
            p.Add("@name", name);

            using (var r = await c.QueryMultipleAsync("dbo.usp_get_griddler", p, null, null, CommandType.StoredProcedure))
            {
                Griddler? G = r.Read<Griddler>().SingleOrDefault();

                if (G != null)
                {
                    Griddler = G;
                    Griddler.Items = r.Read<GriddlerItem>().ToArray();
                    Griddler.Solids = r.Read<GriddlerSolid>().ToArray();
                    Griddler.Paths = r.Read<GriddlerPath>().ToArray();
                }
            }

            var (Rows, Cols) = (Array.Empty<Item[]>(), Array.Empty<Item[]>());
            Rows = new Item[Griddler.height][];
            Cols = new Item[Griddler.width][];

            Dictionary<byte, byte> RowDepths = (from i in Griddler.Items
                                                where i.is_row
                                                group i by i.line_number into grp
                                                select new { k = grp.Key, m = grp.Max(m => m.position) })
                                               .ToDictionary(k => k.k, k => k.m);

            Dictionary<byte, byte> ColDepths = (from i in Griddler.Items
                                                where !i.is_row
                                                group i by i.line_number into grp
                                                select new { k = grp.Key, m = grp.Max(m => m.position) })
                                                .ToDictionary(k => k.k, k => k.m);

            foreach (GriddlerItem item in Griddler.Items)
            {
                if (item.is_row)
                {
                    if (Rows[item.line_number] == null)
                        Rows[item.line_number] = new Item[RowDepths[item.line_number] + 1];

                    Rows[item.line_number][item.position] = new Item(item);
                }
                else
                {
                    if (Cols[item.line_number] == null)
                        Cols[item.line_number] = new Item[ColDepths[item.line_number] + 1];

                    Cols[item.line_number][item.position] = new Item(item);
                }
            }

            Dictionary<(int, int), Point> pts = Griddler.Solids
                .ToDictionary(k => ((int)k.x_position, (int)k.y_position), k => new Point(k));

            Griddler.Rows = Rows;
            Griddler.Cols = Cols;
            Griddler.Pts = pts;

            return Griddler;
        });
    }

    //public async Task<GriddlerItem[]> ListGriddlerItems(short griddlerID)
    //    => await Griddlers.FromSqlRaw("dbo.usp_list_griddler_items").ToArrayAsync();

    public async Task<int> SaveGriddler(string name,
                                        byte width,
                                        byte height,
                                        Item[][] rows,
                                        Item[][] cols,
                                        Dictionary<(int, int), Point> points)
    {
        return await WithConnection(async c =>
        {
            DataTable Data = new DataTable();
            Data.Columns.Add("line_number", typeof(byte));
            Data.Columns.Add("is_row", typeof(bool));
            Data.Columns.Add("position", typeof(byte));
            Data.Columns.Add("value", typeof(byte));
            Data.Columns.Add("colour", typeof(string));

            byte LineNumber = 0;
            foreach (Item[] Row in rows)
            {
                byte Position = 0;
                foreach (Item Item in Row)
                {
                    Data.Rows.Add(LineNumber, true, Position, (byte)Item.Value, Item.Colour);
                    Position++;
                }
                LineNumber++;
            }

            LineNumber = 0;
            foreach (Item[] Col in cols)
            {
                byte Position = 0;
                foreach (Item Item in Col)
                {
                    Data.Rows.Add(LineNumber, false, Position, (byte)Item.Value, Item.Colour);
                    Position++;
                }
                LineNumber++;
            }

            DataTable Solids = new DataTable();
            Solids.Columns.Add("x_position", typeof(byte));
            Solids.Columns.Add("y_position", typeof(byte));
            Solids.Columns.Add("colour", typeof(string));

            foreach (KeyValuePair<(int, int), Point> Solid in points)
            {
                Solids.Rows.Add((byte)Solid.Key.Item1, (byte)Solid.Key.Item2, (string)Solid.Value.Colour);
            }

            DynamicParameters P = new DynamicParameters();
            P.Add("@name", name);
            P.Add("@width", width);
            P.Add("@height", height);
            P.Add("@data", Data.AsTableValuedParameter());
            P.Add("@solids", Solids.AsTableValuedParameter());
            P.Add("@RetVal", direction: ParameterDirection.ReturnValue);

            await c.ExecuteAsync("dbo.usp_save_griddler", P, null, null, CommandType.StoredProcedure);

            return P.Get<int>("@RetVal");
        });
    }


    public async Task<int> SaveGriddler(short id,
                                        Dictionary<(int, int), Point> points)
    {
        return await WithConnection(async c =>
        {
            DataTable Solids = new DataTable();
            Solids.Columns.Add("x_position", typeof(byte));
            Solids.Columns.Add("y_position", typeof(byte));
            Solids.Columns.Add("colour", typeof(string));

            foreach (KeyValuePair<(int, int), Point> Solid in points)
            {
                Solids.Rows.Add((byte)Solid.Key.Item1, (byte)Solid.Key.Item2, (string)Solid.Value.Colour);
            }

            DynamicParameters p = new DynamicParameters();
            p.Add("@id", id);
            p.Add("@solids", Solids.AsTableValuedParameter());

            await c.ExecuteAsync("dbo.usp_save_griddler_solids", p, null, null, CommandType.StoredProcedure);

            return id;
        });
    }

}
