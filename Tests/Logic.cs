using Griddlers.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class LogicTestBase
    {
        private readonly string FileRoot = $"{Library.TryGetSolutionDirectoryInfo().FullName}\\GriddlersDataContext\\Data\\";
        protected async Task Save(string fileName,
                            Item[][] rows, 
                            Item[][] cols, 
                            Dictionary<(int, int), Point> points)
        {
            //string file = $"{FileRoot}{fileName}.json";
            //using (StreamWriter S = new StreamWriter(File.Create(file)))
            //{
            //    List<decimal[]> Rows = new List<decimal[]>(rows.GetLength(0));
            //    foreach (Item[] Row in rows)
            //    {
            //        decimal[] RowS = Row.Select(s => decimal.Parse($"{s.Value}{(s.Green ? ".1" : "")}")).ToArray();
            //        Rows.Add(RowS);
            //    }
            //    List<decimal[]> Columns = new List<decimal[]>(rows.GetLength(0));
            //    foreach (Item[] Col in cols)
            //    {
            //        decimal[] ColS = Col.Select(s => decimal.Parse($"{s.Value}{(s.Green ? ".1" : "")}")).ToArray();
            //        Columns.Add(ColS);
            //    }

            //    var ptsArray = points.Values
            //        .Select(k => new { xPos = k.Xpos, yPos = k.Ypos, colour = k.Green ? "green" : "black" })
            //        .ToArray();

            //    int RowDepth = Rows.Select(s => s.Length).Max();
            //    int ColDepth = Columns.Select(s => s.Length).Max();

            //    var data = new
            //    {
            //        name = fileName,
            //        width = Columns.Count,
            //        height = Rows.Count,
            //        rowDepth = RowDepth,
            //        colDepth = ColDepth,
            //        depth = Math.Max(RowDepth, ColDepth),
            //        rows = Rows.ToArray(),
            //        cols = Columns.ToArray(),
            //        points = ptsArray
            //    };

            //    S.WriteLine(JsonConvert.SerializeObject(data));
            //}

            //string file = $"{FileRoot}{fileName}.txt";
            //using (StreamWriter S = new StreamWriter(File.Create(file)))
            //{
            //    List<decimal[]> Rows = new List<decimal[]>(rows.GetLength(0));
            //    foreach (Item[] Row in rows)
            //    {
            //        decimal[] RowS = Row.Select(s => decimal.Parse($"{s.Value}{(s.Green ? ".1" : "")}")).ToArray();
            //        Rows.Add(RowS);
            //    }
            //    List<decimal[]> Columns = new List<decimal[]>(rows.GetLength(0));
            //    foreach (Item[] Col in cols)
            //    {
            //        decimal[] ColS = Col.Select(s => decimal.Parse($"{s.Value}{(s.Green ? ".1" : "")}")).ToArray();
            //        Columns.Add(ColS);
            //    }

            //    S.WriteLine(JsonConvert.SerializeObject(Rows.ToArray()));
            //    S.WriteLine(JsonConvert.SerializeObject(Columns.ToArray()));
            //    Point[] ptsArray = points.Values
            //        .Select(k => new Point() { Xpos = k.Xpos, Ypos = k.Ypos, Green = k.Green })
            //        .ToArray();
            //    S.WriteLine(JsonConvert.SerializeObject(ptsArray));
            //}
        }                        
    }
}
