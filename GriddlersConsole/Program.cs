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
            while (true)
            {
                Console.WriteLine("Enter griddler name or Y to exit:");

                string Name = Console.ReadLine();

                if (Name == "Y")
                    break;
                else
                {
                    try
                    {
                        await DrawGriddler(Name);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Not Found.");
                    }
                }
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
                    int Value = 0;

                    if (Pos < Column.Length) 
                    {
                        Value = Column[Pos].Value;
                        Cell = Value.ToString();                    
                    }

                    ColumnString = $"{ColumnString} {Cell}{(Value < 10 ? " " : string.Empty)}";
                }

                Console.WriteLine(ColumnString);
            }

            int RowIndex = 0;
            foreach (Item[] Row in Rows)
            {
                string RowString = string.Join(" ", Row.Select(s => s.Value));
                int NumberOfTwoDigits = Row.Count(c => c.Value > 9);
                
                for (int Pos = Row.Length + 1; Pos <= MaxRowDepth + 1; Pos++)
                    RowString = $"{RowString}  ";

                if(NumberOfTwoDigits > 0)
                    RowString = RowString.Remove(RowString.Length - NumberOfTwoDigits);

                Console.ResetColor();
                Console.Write(RowString);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;

                for (int Pos = 0; Pos < Columns.Length; Pos++)
                {
                    string Cell = "   ";                    

                    if (Points.TryGetValue((Pos, RowIndex), out Point Pt))
                    { 
                        Cell = "███";

                        if(Pt.Green)
                            Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (Dots.ContainsKey((Pos, RowIndex)))
                        Cell = " . ";

                    Console.Write(Cell);
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.ResetColor();
                Console.WriteLine();
                RowIndex++;
            }
        }
    }
}
