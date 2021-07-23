using Griddlers.Library;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class LogicTestBase
    {
        private readonly string FileRoot = $"{Library.TryGetSolutionDirectoryInfo().FullName}\\GriddlersDataContext\\Data\\";

        private static string GetColourForFile(string colour)
        {
            return colour switch
            {
                "lightgreen" => "1",
                _ => ""
            };
        }

        protected async Task Save(string fileName,
                            Item[][] rows, 
                            Item[][] cols, 
                            Dictionary<(int, int), Point> points)
        {
            //string file = $"{FileRoot}{fileName}.txt";
            //using (StreamWriter S = new StreamWriter(File.Create(file)))
            //{
            //    List<decimal[]> Rows = new List<decimal[]>(rows.GetLength(0));
            //    foreach (Item[] Row in rows)
            //    {
            //        decimal[] RowS = Row.Select(s => decimal.Parse($"{s.Value}{GetColourForFile(s.Colour)}")).ToArray();
            //        Rows.Add(RowS);
            //    }
            //    List<decimal[]> Columns = new List<decimal[]>(rows.GetLength(0));
            //    foreach (Item[] Col in cols)
            //    {
            //        decimal[] ColS = Col.Select(s => decimal.Parse($"{s.Value}{(GetColourForFile(s.Colour))}")).ToArray();
            //        Columns.Add(ColS);
            //    }

            //    S.WriteLine(JsonConvert.SerializeObject(Rows.ToArray()));
            //    S.WriteLine(JsonConvert.SerializeObject(Columns.ToArray()));
            //    Point[] ptsArray = points.Values
            //        .Select(k => new Point() { Xpos = k.Xpos, Ypos = k.Ypos, Colour = k.Colour })
            //        .ToArray();
            //    S.WriteLine(JsonConvert.SerializeObject(ptsArray));
            //}
        }                        
    }
}
