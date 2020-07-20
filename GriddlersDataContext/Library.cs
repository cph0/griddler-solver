using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Griddlers.Library
{
    public class Library
    {
        private static readonly string FileRoot = $"{TryGetSolutionDirectoryInfo().FullName}\\GriddlersDataContext\\Data\\";

        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }

        public static string[] ListGriddlers() 
        {
            return new DirectoryInfo(FileRoot).GetFiles().Select(s => s.Name.Replace(s.Extension, "")).ToArray();
        }

        public async static Task<(Item[][], Item[][])> GetSourceData(string fileName)
        {
            Item[][] Rows = new Item[][] { };
            Item[][] Cols = new Item[][] { };
            
            string File = $"{FileRoot}{fileName}.txt";
            using (StreamReader S = new StreamReader(File))
            {
                string? RowsString = await S.ReadLineAsync();
                string? ColsString = await S.ReadLineAsync();
                string?[][] Rs = JsonConvert.DeserializeObject<string?[][]>(RowsString);
                string?[][] Cs = JsonConvert.DeserializeObject<string?[][]>(ColsString);
                int Index = 0;
                Rows = new Item[Rs.GetLength(0)][];
                foreach (string?[] Row in Rs)
                {
                    Rows[Index] = Row.Select((s, i) => new Item(i, s)).ToArray();
                    Index++;
                }
                Index = 0;
                Cols = new Item[Cs.GetLength(0)][];
                foreach (string?[] Col in Cs)
                {
                    Cols[Index] = Col.Select((s, i) => new Item(i, s)).ToArray();
                    Index++;
                }
            }

            return (Rows, Cols);
        }

        public async static Task<(Dictionary<(int, int), Point>, Dictionary<(int, int), Point>)> GetOutputData(string fileName, int w, int h)
        {
            Point[] Points;
            Dictionary<(int, int), Point> Pts = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dts = new Dictionary<(int, int), Point>();
            
            string File = $"{FileRoot}{fileName}.txt";
            using (StreamReader S = new StreamReader(File))
            {
                await S.ReadLineAsync();
                await S.ReadLineAsync();
                string? colsString = await S.ReadLineAsync();
                Points = JsonConvert.DeserializeObject<Point[]>(colsString);
            }

            Pts = Points.ToDictionary(k => (k.Xpos, k.Ypos));

            for (int Li = 0; Li < w; Li++)
                for (int Pos = 0; Pos < h; Pos++)
                    if (!Pts.ContainsKey((Li, Pos)))
                        Dts.Add((Li, Pos), new Point());

            return (Pts, Dts);
        }

    }
}
