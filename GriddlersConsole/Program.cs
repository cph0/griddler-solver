using Griddlers.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GriddlersConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            bool Exit = false;

            while (!Exit)
            {
                Console.WriteLine("Enter griddler name or Y to exit:");

                string Name = Console.ReadLine();

                if (Name == "Y")
                    Exit = true;                
                else
                    await DrawGriddler(Name);                
            }            
        }

        private static async Task DrawGriddler(string name) 
        {
            (Item[][] Rows, Item[][] Columns) = await Library.GetSourceData(name);

            (Dictionary<(int, int), Point> Points, Dictionary<(int, int), Point> Dots)
                = Logic.Run(Rows, Columns);

            int MaxRowDepth = Rows.Max(m => m.Length);
            int MaxColumnDepth = Columns.Max(m => m.Length);

            for (int Pos = 0; Pos <= MaxColumnDepth; Pos++)
            {
                string ColumnString = string.Empty;

                for (int RowPos = 0; RowPos <= MaxRowDepth; RowPos++)
                    ColumnString = $"{ColumnString}  ";

                foreach (Item[] Column in Columns)
                {
                    string Cell = " ";

                    if (Pos < Column.Length)
                        Cell = Column[Pos].Value.ToString();

                    ColumnString = $"{ColumnString} {Cell} ";
                }

                Console.WriteLine(ColumnString);
            }

            int RowIndex = 0;
            foreach (Item[] Row in Rows)
            {
                string RowString = string.Join(" ", Row.Select(s => s.Value));

                for (int Pos = Row.Length + 1; Pos <= MaxRowDepth + 1; Pos++)
                    RowString = $"{RowString}  ";

                for (int Pos = 0; Pos < Columns.Length; Pos++)
                {
                    string Cell = string.Empty;

                    if (Points.TryGetValue((Pos, RowIndex), out Point Pt))
                        Cell = "███";
                    else if (Dots.ContainsKey((Pos, RowIndex)))
                        Cell = " . ";

                    RowString = $"{RowString}{Cell}";
                }

                Console.WriteLine(RowString);
                RowIndex++;
            }
        }
    }
}
