using Griddlers.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Griddlers.Library
{
    public class Logic
    {
        private static IEnumerator<Point>? Stream;
        private static bool Streaming = false;
        private static List<Point> StreamCache = new List<Point>();
        private static int Current = 0;

        private static bool Staging = false;
        private static int TreeLevel = 0;
        private static Dictionary<(int, int), Point> TreeStage = new Dictionary<(int, int), Point>();
        private static readonly List<TreeNode> Excludes = new List<TreeNode>();

        private readonly HashSet<byte> RemovedActions = new HashSet<byte>();

        public Dictionary<(int, int), Point> dots = new Dictionary<(int, int), Point>();
        public Dictionary<(int, int), Point> points = new Dictionary<(int, int), Point>();
        private IReadOnlyDictionary<int, Line> Rows = new Dictionary<int, Line>();
        private IReadOnlyDictionary<int, Line> Cols = new Dictionary<int, Line>();
        private readonly HashSet<(int, int)> Trials = new HashSet<(int, int)>();
        private readonly HashSet<(int, int)> IncorrectTrials = new HashSet<(int, int)>();

        private readonly IDictionary<string, int> MethodCounts = new Dictionary<string, int>();

        public void AddMethodCount(string key)
        {
            if (MethodCounts.ContainsKey(key))
                MethodCounts[key]++;
            else
                MethodCounts[key] = 1;
        }

        public static (Dictionary<(int, int), Point>, Dictionary<(int, int), Point>) Run(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            return L.RunNonStatic(rows, columns);
        }

        private (Dictionary<(int, int), Point>, Dictionary<(int, int), Point>) RunNonStatic(Item[][] rows, Item[][] columns)
        {
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();
            Staging = false;
            IEnumerable<Point> All = RunI(rows, columns);

            foreach (Point Point in All)
            {
                if (Point.IsDot)
                    Dots.TryAdd((Point.Xpos, Point.Ypos), Point);
                else
                    Points.TryAdd((Point.Xpos, Point.Ypos), Point);
            }

            return (Points, Dots);
        }

        /// <summary>
        /// Starts solving a griddler as a stream.  
        /// If no action is passed then the stream will be paused.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <param name="action">The method to call for each point/dot solved.</param>
        public static void RunAsStream(Item[][] rows, Item[][] columns, Action<Point>? action = null)
        {
            Logic L = new Logic();
            Stream = L.RunI(rows, columns).GetEnumerator();
            Staging = false;
            Current = 0;
            StreamCache = new List<Point>();

            if (action != null)
                Play(action);
        }
        /// <summary>
        /// Starts/Restarts a stream.  
        /// Must call <see cref="RunAsStream(Item[][], Item[][], Action{Point}?)"/> first.
        /// </summary>
        /// <param name="action">The method to call for each point/dot solved.</param>
        public static void Play(Action<Point> action)
        {
            Streaming = true;
            Task.Run(() =>
            {
                while (Streaming)
                {
                    Point? Pt = Next();
                    if (Pt != (object?)null)
                        action(Pt);
                    else
                        Streaming = false;

                    Thread.Sleep(100);
                }
            });
        }
        /// <summary>
        /// Stops a stream.
        /// </summary>
        public static void Stop()
        {
            Streaming = false;
        }
        /// <summary>
        /// Gets the next point to be solved.
        /// </summary>
        /// <returns>The next point in the stream.</returns>
        public static Point? Next()
        {
            Point? Pt = null;
            if (Stream != null)
            {
                if (Current < StreamCache.Count - 1)
                    Pt = StreamCache[Current + 1];
                else if (Stream.MoveNext())
                {
                    Pt = Stream.Current;
                    StreamCache.Add(Pt);
                }

                //if (StreamCache.Count < points.Count + dots.Count)
                Current++;
            }

            return Pt;
        }
        /// <summary>
        /// Gets the previous point that was solved.
        /// </summary>
        /// <returns>The previous point in the stream.</returns>
        public static Point? Previous()
        {
            Point? Pt = null;

            if (StreamCache.Count > 0 && Current > 0 && Current <= StreamCache.Count)
            {
                Pt = StreamCache[Current - 1];
                Current--;
            }

            return Pt;
        }

        public static Tree CreateTree(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            Staging = true;
            Point.Group = 0;

            IEnumerable<Point> Stage = L.RunI(rows, columns);

            Tree Tree = new Tree(Stage);

            L.AddChildren(rows, columns, Tree.Root);

            return Tree;
        }
        private void AddChildren(Item[][] rows, Item[][] columns, TreeNode parent)
        {
            foreach (TreeNode Node in parent.Children)
            {
                foreach (Point Point in Node.Points.Values)
                {
                    if (Point.IsDot)
                        dots.TryAdd((Point.Xpos, Point.Ypos), Point);
                    else
                        points.TryAdd((Point.Xpos, Point.Ypos), Point);
                }

                IEnumerable<Point> Stage = RunI(rows, columns);
                //Excludes.AddRange(parent.Children);
                Node.SetNodes(Stage, Excludes);
                AddChildren(rows, columns, Node);
            }

            //if (TreeLevel < 10)
            //{
            //foreach (Point Point in parent.Points.Values)
            //{
            //    if (Point.IsDot)
            //        dots.Remove((Point.Xpos, Point.Ypos));
            //    else
            //        points.Remove((Point.Xpos, Point.Ypos));
            //}
            //}

            //TreeLevel++;
        }
        public static Tree CreateTree2(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            Staging = true;
            Point.Group = 0;

            bool Complete = false;
            Tree Tree = new Tree();
            TreeNode Current = Tree.Root;

            while (!Complete)
            {
                IEnumerable<Point> Stage = L.RunI(rows, columns);
                Current.SetNodes(Stage);

                if (Current.Children.Count > 0 && Current.CurrentNodePos < Current.Children.Count)
                {
                    TreeNode Child = Current.Children[Current.CurrentNodePos];
                    Child.Parent = Current;
                    Current = Child;

                    foreach (Point Point in Current.Points.Values)
                    {
                        if (Point.IsDot)
                            L.dots.TryAdd((Point.Xpos, Point.Ypos), Point);
                        else
                            L.points.TryAdd((Point.Xpos, Point.Ypos), Point);
                    }
                }
                else if (Current.Parent != null)
                {
                    foreach (Point Point in Current.Points.Values)
                    {
                        if (Point.IsDot)
                            L.dots.Remove((Point.Xpos, Point.Ypos));
                        else
                            L.points.Remove((Point.Xpos, Point.Ypos));
                    }

                    Current = Current.Parent;
                    Current.CurrentNodePos++;
                }
                else
                    Complete = true;
            }

            return Tree;
        }

        /// <summary>
        /// Determines if a griddler cannot solve without a particular action.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <param name="points">The points</param>
        /// <param name="dots">The dots</param>
        /// <param name="action">The action to test</param>
        /// <returns><see cref="true"/> if the action is required.</returns>
        public static bool IsActionRequired(Item[][] rows,
                                            Item[][] columns,
                                            IReadOnlyDictionary<(int, int), Point> points,
                                            IReadOnlyDictionary<(int, int), Point> dots,
                                            string action)
        {
            bool RetVal = false;
            Logic L = new Logic();
            L.RemovedActions.Add((byte)Enum.Parse(typeof(GriddlerPath.Action), action));

            var Output = L.RunNonStatic(rows, columns);

            if (Output.Item1.Count != points.Count || Output.Item1.Keys.Except(points.Keys).Any()
                && Output.Item2.Count != dots.Count || Output.Item2.Keys.Except(dots.Keys).Any())
                RetVal = true;

            return RetVal;
        }
        /// <summary>
        /// Determines what actions a griddler needs to solve.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <param name="points">The points</param>
        /// <param name="dots">The dots</param>
        /// <returns>A list of required actions.</returns>
        public static IReadOnlyList<string> GetRequiredActions(Item[][] rows,
                                                               Item[][] columns,
                                                               IReadOnlyDictionary<(int, int), Point> points,
                                                               IReadOnlyDictionary<(int, int), Point> dots)
        {
            string[] AllActions = Enum.GetNames(typeof(GriddlerPath.Action));
            List<string> Actions = new List<string>(AllActions.Length);

            Parallel.ForEach(AllActions, (Action) =>
            {
                if (IsActionRequired(rows, columns, points, dots, Action))
                    Actions.Add(Action);
            });

            return Actions;
        }
        /// <summary>
        /// Determines what actions are used by a griddler.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>A list of used actions.</returns>
        public static IReadOnlyList<string> GetUsedActions(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            IEnumerable<Point> Points = L.RunI(rows, columns);
            IEnumerable<string> Actions = (from p in Points
                                           select Enum.GetName(typeof(GriddlerPath.Action), p.Action));
            return Actions.Distinct().ToList();
        }

        /// <summary>
        /// Determines if a griddler satisfies the following
        /// <list type="number">
        /// <item>No zero lines</item>
        /// <item>No trivial lines (full lines)</item>
        /// <item>Size is a mulitple of 5</item>
        /// <item>No symmetry (TEST)</item>
        /// <item>No trial and error (TODO)</item>
        /// </list>
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns><see cref="true"/> if the griddler is a true griddler</returns>
        public static bool IsTrueGriddler(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            int Width = columns.Length;
            int Height = rows.Length;
            IEnumerable<Line> RowLines = rows.Select((s, si) => new Line(si, true, Width, s, L));
            IEnumerable<Line> ColumnLines = columns.Select((s, si) => new Line(si, false, Height, s, L));

            IEnumerable<Point> Rows = L.FullLine(RowLines);
            IEnumerable<Point> Columns = L.FullLine(ColumnLines);

            bool AnyZeroLines = rows.Any(a => a[0].Value == 0) || columns.Any(a => a[0].Value == 0);
            bool AnyFullLines = Rows.Any() || Columns.Any();
            bool MultipleOfFive = rows.Length % 5 == 0 && columns.Length % 5 == 0;
            bool Symmetry = true;
            bool TrialAndError = true;

            //symmetry
            for (int index = 0; index < Height; index++)
            {
                Item[] First = rows[index];
                Item[] Last = rows[Height - index - 1];

                for (int c = 0; c < Width; c++)
                {
                    if (First[c] != Last[c])
                    {
                        Symmetry = false;
                        break;
                    }
                }

                if (!Symmetry)
                    break;
            }

            if (Symmetry)
            {
                for (int index = 0; index < Width; index++)
                {
                    Item[] First = rows[index];
                    Item[] Last = rows[Width - index - 1];

                    for (int c = 0; c < Height; c++)
                    {
                        if (First[c] != Last[c])
                        {
                            Symmetry = false;
                            break;
                        }
                    }

                    if (!Symmetry)
                        break;
                }
            }

            return !AnyZeroLines && !AnyFullLines && MultipleOfFive && !Symmetry && !TrialAndError;
        }

        /// <summary>
        /// Determines the number of trivial lines (full lines).
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of full lines.</returns>
        public static int FullLineCount(Item[][] rows, Item[][] columns)
        {
            (int, int) Counts = FullLineCounts(rows, columns);
            return Counts.Item1 + Counts.Item2;
        }
        /// <summary>
        /// Determines the number of trivial lines (full lines).
        /// Split into rows/columns.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of full rows and columns.</returns>
        public static (int, int) FullLineCounts(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            int Width = columns.Length;
            int Height = rows.Length;
            L.Rows = rows.Select((s, si) => new Line(si, true, Width, s, L)).ToDictionary(k => k.LineIndex);
            L.Cols = columns.Select((s, si) => new Line(si, false, Height, s, L)).ToDictionary(k => k.LineIndex);

            IEnumerable<Point> Rows = L.FullLine(L.Rows.Values);
            IEnumerable<Point> Columns = L.FullLine(L.Cols.Values);

            return (Rows.GroupBy(g => g.Ypos).Count(), Columns.GroupBy(g => g.Xpos).Count());
        }

        /// <summary>
        /// Determines the number of overlap lines.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of overlap liens.</returns>
        public static int OverlapLineCount(Item[][] rows, Item[][] columns)
        {
            (int, int) Counts = OverlapLineCounts(rows, columns);
            return Counts.Item1 + Counts.Item2;
        }
        /// <summary>
        /// Determines the number of overlap lines.
        /// Split into rows/columns.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of overlap rows and columns.</returns>
        public static (int, int) OverlapLineCounts(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            int Width = columns.Length;
            int Height = rows.Length;
            L.Rows = rows.Select((s, si) => new Line(si, true, Width, s, L)).ToDictionary(k => k.LineIndex);
            L.Cols = columns.Select((s, si) => new Line(si, false, Height, s, L)).ToDictionary(k => k.LineIndex);

            IEnumerable<Point> Rows = L.OverlapLine(L.Rows.Values);
            IEnumerable<Point> Columns = L.OverlapLine(L.Cols.Values);

            return (Rows.GroupBy(g => g.Ypos).Count(), Columns.GroupBy(g => g.Xpos).Count());
        }


        private IEnumerable<Point> RunI(Item[][] rows, Item[][] columns)
        {
            int Width = columns.Length;
            int Height = rows.Length;

            if (!Staging)
            {
                points = new Dictionary<(int, int), Point>(Width * Height);
                dots = new Dictionary<(int, int), Point>(Width * Height);
                Point.Group = 0;
            }

            int Count = 0;
            int LoopCount = -1;

            Rows = rows.Select((s, si) => new Line(si, true, Width, s, this)).ToDictionary(k => k.LineIndex);
            Cols = columns.Select((s, si) => new Line(si, false, Height, s, this)).ToDictionary(k => k.LineIndex);

            foreach (Line Row in Rows.Values)
                Row.SetPairLines(Cols.Values);

            foreach (Line Col in Cols.Values)
                Col.SetPairLines(Rows.Values);

            //full rows <5ms
            foreach (Point Point in FullLine(Rows.Values))
                yield return Point;

            //full columns <5ms
            foreach (Point Point in FullLine(Cols.Values))
                yield return Point;

            //overlapping rows <20ms
            foreach (Point Point in OverlapLine(Rows.Values))
                yield return Point;

            //overlapping columns <20ms
            foreach (Point Point in OverlapLine(Cols.Values))
                yield return Point;

            //multi colour cross reference rows
            foreach (Point Point in MultiColourCrossReference(Rows.Values))
                yield return Point;

            //multi colour cross reference columns
            foreach (Point Point in MultiColourCrossReference(Cols.Values))
                yield return Point;

            foreach (Point Point in Run())
                yield return Point;

            //trial and error - EXPERIMENTAL
            if (!Staging && false && points.Count + dots.Count < Width * Height
                && points.Count + dots.Count > (Width * Height * 3) / 4)
            {
                int TrialCount = 1;
                List<(int, int)> TestPointKeys = new List<(int, int)>(50);
                List<(int, int)> TestDotKeys = new List<(int, int)>(50);

                while (!IsComplete(Rows.Values))
                {
                    foreach (Point Point in TrialAndError(Rows.Values))
                    {
                        if (Point.IsDot)
                            TestDotKeys.Add((Point.Xpos, Point.Ypos));
                        else
                            TestPointKeys.Add((Point.Xpos, Point.Ypos));
                    }

                    //foreach (Point Point in TrialAndError(Cols.Values))
                    //    yield return Point;

                    foreach (Point Point in Run())
                    {
                        if (Point.IsDot)
                            TestDotKeys.Add((Point.Xpos, Point.Ypos));
                        else
                            TestPointKeys.Add((Point.Xpos, Point.Ypos));
                    }

                    //undo
                    if (points.Keys.Any(a => dots.ContainsKey(a)) //|| !Validate(Cols.Values)
                        )
                    {
                        foreach ((int, int) PointKey in TestPointKeys)
                        {
                            if (PointKey.Item1 < Cols.Count)
                                Rows[PointKey.Item1].IsComplete = false;

                            if (PointKey.Item2 < Rows.Count)
                                Cols[PointKey.Item2].IsComplete = false;

                            points.Remove(PointKey);
                        }

                        foreach ((int, int) DotKey in TestDotKeys)
                        {
                            if (DotKey.Item1 < Cols.Count)
                                Rows[DotKey.Item1].IsComplete = false;

                            if (DotKey.Item2 < Rows.Count)
                                Cols[DotKey.Item2].IsComplete = false;

                            dots.Remove(DotKey);
                        }

                        TestPointKeys.Clear();
                        TestDotKeys.Clear();

                        IncorrectTrials.UnionWith(Trials);
                        Trials.Clear();
                    }

                    if (TrialCount == 10)
                        break;

                    TrialCount++;
                }

                foreach ((int, int) PointKey in TestPointKeys)
                    yield return points[PointKey];

                foreach ((int, int) DotKey in TestDotKeys)
                    yield return dots[DotKey];
            }

            IEnumerable<Point> Run()
            {
                while (!IsComplete(Rows.Values))
                {
                    int PointsChange = points.Count;
                    int DotsChange = dots.Count;
                    bool StrayDog = false;

                    if (Count == LoopCount)
                        break;

                    //row full line dots <5ms
                    foreach (Point Point in FullLineDots(Rows.Values))
                        yield return Point;

                    //column full line dots <5ms
                    foreach (Point Point in FullLineDots(Cols.Values))
                        yield return Point;

                    //row line dots <300ms
                    foreach (Point Point in CompleteItem(Rows.Values))
                        yield return Point;

                    //column line dots <300ms
                    foreach (Point Point in CompleteItem(Cols.Values))
                        yield return Point;

                    //row line dots <300ms
                    foreach (Point Point in LineBlocks(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column line dots <300ms
                    foreach (Point Point in LineBlocks(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    //row line gaps <10ms
                    foreach (Point Point in LineGaps(Rows.Values))
                        yield return Point;

                    //column line gaps < 10ms
                    foreach (Point Point in LineGaps(Cols.Values))
                        yield return Point;

                    foreach ((int, int) Item in points.Keys.Where(w => dots.ContainsKey(w)))
                    {
                        Console.WriteLine($"stray dog x:{Item.Item1}y:{Item.Item2}");
                        StrayDog = true;
                    }

                    //busker30x30 - does not solve
                    //points.TryAdd((3, 13), new Point(false, 3, 13, false));
                    //points.TryAdd((0, 15), new Point(false, 0, 15, false));
                    //points.TryAdd((7, 23), new Point(false, 7, 23, true));
                    //points.TryAdd((7, 25), new Point(false, 7, 25, false)); //bug

                    //FourCallingBirds25x35 - does not solve
                    //points.TryAdd((6, 13), new Point(false, 6, 13, false));
                    //points.TryAdd((6, 14), new Point(false, 6, 14, false));
                    //points.TryAdd((6, 15), new Point(false, 6, 15, false));

                    //FiveGoldRing25x35 - does not solve
                    //dots.TryAdd((9, 11), new Point(true, 9, 11, false));
                    //dots.TryAdd((9, 13), new Point(true, 9, 13, false));

                    if (PointsChange == points.Count && DotsChange == dots.Count)
                        break;

                    if (StrayDog)
                        break;

                    Count++;
                }

            }

            Console.WriteLine($"Loop Count: {Count}");
            //foreach (KeyValuePair<string, int> Method in MethodCounts)
            //{
            //    Console.WriteLine($"{Method.Key}: {Method.Value}");
            //}
        }

        private bool IsComplete(IEnumerable<Line> lines)
        {
            bool Complete = true;

            foreach (Line Line in ForEachLine(lines))
            {
                if (Line.GetLinePointsValue(true) != Line.LineLength)
                {
                    Complete = false;
                    break;
                }
                else
                    Line.IsComplete = true;
            }

            return Complete;
        }

        private bool Validate(IEnumerable<Line> lines)
        {
            bool Valid = true;

            foreach (Line Line in ForEachLine(lines))
            {
                int CurrentItem = 0;

                foreach ((int Pos, (int, int) Xy, Block Block, _, _, _) in ForEachLinePos(Line, (nS, wS, c, xy, gS) => nS && wS))
                {
                    if (Line.All(a => Block != a))
                    {
                        Valid = false;
                        break;
                    }

                    //if (Block != Line[CurrentItem])
                    //{
                    //    Valid = false;
                    //    break;
                    //}

                    CurrentItem++;
                }

                if (!Valid)
                    break;
            }

            return Valid;
        }


        public IEnumerable<Point> AddPoints(Line line, int start, bool green, GriddlerPath.Action action, int? end = null, bool dot = false)
        {
            for (int Pos = start; Pos <= end.GetValueOrDefault(start); Pos++)
            {
                var Xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);

                Point NewPoint = new Point(dot, Xy, green, action);
                bool New = (dot && !dots.ContainsKey(Xy)) || (!dot && !points.ContainsKey(Xy));

                if (!Staging && !RemovedActions.Contains((byte)action))
                {
                    if (dot)
                    {
                        dots.TryAdd(Xy, NewPoint);
                        line.AddDot(Pos, action);
                    }
                    else
                    {
                        points.TryAdd(Xy, NewPoint);
                        line.AddPoint(Pos, green ? "green" : "black", action);
                    }
                }

                if (New && !RemovedActions.Contains((byte)action))
                    yield return NewPoint;

                if (line.IsRow && Pos >= 0 && Pos < Cols.Count)
                {
                    Rows[line.LineIndex].ClearCaches(start);
                    Cols[Pos].ClearCaches(line.LineIndex);
                }
                else if (!line.IsRow && Pos >= 0 && Pos < Rows.Count)
                {
                    Cols[line.LineIndex].ClearCaches(start);
                    Rows[Pos].ClearCaches(line.LineIndex);
                }
            }
        }

        private IEnumerable<Line> ForEachLine(IEnumerable<Line> lines, Func<Line, bool>? run = null, int minLineValue = 1)
        {
            foreach (Line L in lines)
            {
                if (L.LineValue >= minLineValue && !L.IsComplete && (run == null || run(L)))
                    yield return L;
            }
        }

        public IEnumerable<(int, (int, int), Block, bool, int, int)> ForEachLinePos(Line line, Func<bool, bool, int, (int, int), int, bool>? run = null, Predicate<int>? stop = null, bool b = false, int start = 0, Func<int>? advance = null)
        {
            bool WasSolid = false;
            int SolidBlockCount = 0, GapSize = 0, SolidCount = 0;
            Block Block = new Block(0, false);
            for (int Pos = b ? line.LineLength - 1 : start; b ? Pos >= start : Pos < line.LineLength; Pos += (b ? -1 : 1))
            {
                var Xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);
                bool ChangedColour = false;

                if (stop != null && stop(Pos))
                    break;

                if (points.TryGetValue(Xy, out Point? Pt) && Block.Green != Pt.Green)
                    ChangedColour = true;

                if (run == null || run(Pt == (object?)null || ChangedColour, WasSolid, Pos, Xy, GapSize))
                    yield return (Pos, Xy, Block, WasSolid, GapSize, SolidCount);
                //Pos += (b ? -1 : 1) * logic(Pos, Xy, Block, WasSolid, GapSize, SolidCount);

                if (advance != null)
                    Pos += (b ? -1 : 1) * advance();

                if (!dots.ContainsKey(Xy))
                    GapSize++;
                else
                    GapSize = 0;

                if (points.TryGetValue(Xy, out Pt))
                {
                    if (Block.Green != Pt.Green)
                    {
                        if (Block.SolidCount > 0)
                            SolidBlockCount++;

                        Block = new Block(SolidBlockCount, Pt.Green);
                    }

                    if (!WasSolid || ChangedColour)
                        Block.StartIndex = Pos;

                    Block.EndIndex = Pos;
                    Block.SolidCount++;
                    SolidCount++;

                    WasSolid = true;
                }
                else
                {
                    if (WasSolid)
                    {
                        SolidBlockCount++;
                        Block = new Block(SolidBlockCount, false);
                        SolidCount = 0;
                    }

                    WasSolid = false;
                }
            }
        }

        private IEnumerable<Point> FullPart(Line line, int linePosition, int startItem, int endItem, GriddlerPath.Action action, bool complete = true)
        {
            Point.Group++;

            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                bool Green = Item.Green;
                foreach (Point Point in AddPoints(line, linePosition, Green, action, linePosition + Item.Value - 1))
                    yield return Point;
                if (complete)
                    line.AddBlock(Item, true, linePosition, linePosition + Item.Value - 1);
                linePosition += Item.Value;
                if (linePosition < line.LineLength - 1 && line.GetDotCount(Item.Index) == 1)
                {
                    foreach (Point Pt in AddPoints(line, linePosition, Green, action, dot: true))
                        yield return Pt;

                    linePosition++;
                }
            }
        }

        private IEnumerable<Point> OverlapPart(Line line, int position, int lineEnd, int startItem, int endItem, GriddlerPath.Action action)
        {
            int LinePosition = position;
            bool[,] LineFlagsForward = new bool[lineEnd + 1, endItem + 1];
            bool[,] LineFlagsBackward = new bool[lineEnd + 1, endItem + 1];
            Point.Group++;

            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                for (int Pos = 0; Pos < Item.Value; Pos++)
                {
                    LineFlagsForward[LinePosition, Item.Index] = true;
                    LinePosition++;
                }
                LinePosition += line.GetDotCount(Item.Index);
            }
            LinePosition = 0;
            foreach (Item Item in line.Reverse(endItem).Take(endItem - startItem + 1))
            {
                for (int Pos = 0; Pos < Item.Value; Pos++)
                {
                    LineFlagsBackward[lineEnd - LinePosition, Item.Index] = true;
                    LinePosition += 1;
                }
                LinePosition += line.GetDotCountB(Item.Index);
            }
            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                bool Any = false;
                for (int Pos = position; Pos <= lineEnd; Pos++)
                {
                    if (LineFlagsForward[Pos, Item.Index] && LineFlagsBackward[Pos, Item.Index])
                    {
                        foreach (Point Pt in AddPoints(line, Pos, Item.Green, action))
                            yield return Pt;

                        Any = true;
                    }
                }

                if (Any)
                    line.AddBlock(Item);
            }
        }

        /// <summary>
        /// For each line add up the items and if the sum equals the line length fill them in
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> FullLine(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines, (line) => line.LineValue == line.LineLength))
            {
                Line.IsComplete = true;
                foreach (Point Pt in FullPart(Line, 0, 0, Line.LineItems - 1, GriddlerPath.Action.FullLine))
                    yield return Pt;
            }
        }

        /// <summary>
        /// For each line add up the items and if the sum is greater than half the line length do an overlap
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> OverlapLine(IEnumerable<Line> lines)
        {
            static bool Run(Line line) =>
                line.LineValue < line.LineLength && line.LineValue > line.LineLength / 2;
            foreach (Line Line in ForEachLine(lines, Run))
            {
                foreach (Point Pt in OverlapPart(Line, 0, Line.LineLength - 1, 0, Line.LineItems - 1, GriddlerPath.Action.OverlapLine))
                    yield return Pt;
            }
        }

        /// <summary>
        /// For each line add up the points and if the sum equals the sum of the items fill the gaps with dots
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> FullLineDots(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines, (line) => line.LineValue - line.GetDotCount() == line.GetLinePointsValue(), 0))
            {
                Point.Group++;
                Line.IsComplete = true;

                for (int Pos = 0; Pos < Line.LineLength; Pos++)
                    if (!points.ContainsKey(Line.IsRow ? (Pos, Line.LineIndex) : (Line.LineIndex, Pos)))
                        foreach (Point Pt in AddPoints(Line, Pos, false, GriddlerPath.Action.FullLineDots, dot: true))
                            yield return Pt;
            }
        }

        /// <summary>
        /// For each line do the following:        
        /// <list type="number">
        /// <item>After each gap calculate the possible items in the gap</item>
        /// <item>
        ///     The gap is filled with dots if any of the following are true
        ///     <para>
        ///         <list type="bullet">
        ///             <item>All the items are bigger than the gap size</item>
        ///             <item></item>
        ///         </list> 
        ///     </para>
        /// </item>
        /// <item>
        ///     The gap is filled with points if any of the following are true
        ///     <para>
        ///         <list type="bullet">
        ///             <item>The sum of the items equals the gap size</item>
        ///             <item></item>
        ///         </list> 
        ///     </para>
        /// </item>
        /// <item>
        ///     The gap is partially filled with points if any of the following are true
        ///     <para>
        ///         <list type="bullet">
        ///             <item>The sum of the items is greater than half the gap size</item>
        ///             <item></item>
        ///         </list> 
        ///     </para>
        /// </item>
        /// </list>
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> LineGaps(IEnumerable<Line> lines)
        {
            static bool NoItemForGap(Gap g, bool e, Item? i)
                => e && (!i.HasValue || g.Size < i.Value.Value);
            static bool ItemFillsGap(Gap g, bool e, Item? i)
                => e && g.HasPoints && i.HasValue && g == i.Value;
            static bool ItemAtEdgeOfGap(bool a, bool e, Line l, Item? i)
                => a && i != null && (e || l.ItemsOneValue);

            foreach (Line Line in ForEachLine(lines))
            {
                Gap? LastGap = null;

                for (int i = 1; i < Line.MinItem; i++)
                {
                    foreach (Gap Gap in Line.GetGapsBySize(i))
                    {
                        foreach (Point Pt in Line.AddDots(Gap.Start, Gap.End, GriddlerPath.Action.GapDotsMinItem))
                            yield return Pt;
                    }
                }

                foreach (var (Gap, Ls, Skip) in Line.GetGaps(true))
                {
                    if (Gap.IsFull)
                    {
                        LastGap = Gap;
                        continue;
                    }

                    var (Item, Equality, Index, _, _) = Ls;
                    if (NoItemForGap(Gap, Equality, Item))
                    {
                        foreach (Point Pt in Line.AddDots(Gap.Start, Gap.End, GriddlerPath.Action.GapDotsTooBig))
                            yield return Pt;
                    }
                    else if (ItemFillsGap(Gap, Equality, Item) && Item != null)
                    {
                        foreach (Point Pt in Line.AddPoints(Gap.Start, Gap.End, Item.Value.Colour, GriddlerPath.Action.GapFull))
                            yield return Pt;
                    }
                    else
                    {
                        LineSegment LsEnd = Line.GetItemAtPosB(Gap);
                        var (ItemE, EqualityE, IndexE, _, _) = LsEnd;
                        ItemRange Range = Ls.With(LsEnd, true);
                        int Sum = Range.Sum();

                        bool CombinedEqualityNoItem()
                        {
                            if (EqualityE && LastGap != null && LastGap.IsFull
                                && Index > IndexE)
                            {
                                int ItemShift = Line.SumWhile(IndexE, Gap, null, false);
                                if (!Line.Where(IndexE - ItemShift, IndexE - 1)
                                    .Any(a => LastGap == a))
                                    return true;
                            }

                            if (Equality && Index > IndexE)
                            {
                                Gap? NextGap = Line.FindGapAtPos(Gap.End + 1);
                                if (NextGap != null)
                                {
                                    int ItemShift = Line.SumWhile(IndexE, Gap);
                                    if (!Line.Where(Index + 1, Index + ItemShift)
                                        .Any(a => NextGap == a))
                                        return true;
                                }
                            }

                            return false;
                        }

                        if (NoItemForGap(Gap, EqualityE, ItemE)
                            || (Equality && EqualityE && Index > IndexE)
                            || (Equality && EqualityE && Sum > Gap.Size)
                            || CombinedEqualityNoItem())
                        {
                            foreach (Point Pt in Line.AddDots(Gap.Start, Gap.End, GriddlerPath.Action.GapDotsSum))
                                yield return Pt;
                        }
                        else if (ItemFillsGap(Gap, EqualityE, ItemE) && ItemE != null)
                        {
                            foreach (Point Pt in Line.AddPoints(Gap.Start, Gap.End, ItemE.Value.Colour, GriddlerPath.Action.GapFull, ItemE.Value.Index))
                                yield return Pt;
                        }
                        else if (Item.HasValue && ItemE.HasValue && Sum == Gap.Size)
                        {
                            foreach (Point Pt in FullPart(Line, Gap.Start, Item.Value.Index, ItemE.Value.Index, GriddlerPath.Action.GapFull))
                                yield return Pt;
                        }
                        else if (Item.HasValue && ItemE.HasValue && (Index == IndexE
                            || Equality || EqualityE) && Sum < Gap.Size && Sum > Gap.Size / 2)
                        {
                            foreach (Point Pt in OverlapPart(Line, Gap.Start, Gap.End, Item.Value.Index, ItemE.Value.Index, GriddlerPath.Action.GapOverlap))
                                yield return Pt;
                        }
                        else if (ItemAtEdgeOfGap(Gap.HasFirstPoint, Equality, Line, Item) && Item.HasValue)
                        {
                            foreach (Point Pt in Line.AddPoints(Gap.Start + 1, Gap.Start + Item.Value.Value - 1, Item.Value.Colour, GriddlerPath.Action.CompleteItem, Item.Value.Index))
                                yield return Pt;

                            if (Line.ShouldAddDots(Item.Value.Index).Item2)
                            {
                                Point? Dot = Line.AddDot(Gap.Start + Item.Value.Value, GriddlerPath.Action.CompleteItem);

                                if (Dot != null)
                                    yield return Dot;
                            }
                        }
                        else if (ItemAtEdgeOfGap(Gap.HasLastPoint, EqualityE, Line, ItemE) && ItemE.HasValue)
                        {
                            if (Gap.End - ItemE.Value.Value == Gap.Start)
                                Skip.Index = Gap.End;
                            foreach (Point Pt in Line.AddPoints(Gap.End - ItemE.Value.Value + 1, Gap.End - 1, ItemE.Value.Colour, GriddlerPath.Action.CompleteItem, ItemE.Value.Index))
                                yield return Pt;

                            if (Line.ShouldAddDots(ItemE.Value.Index).Item1)
                            {
                                Point? Dot = Line.AddDot(Gap.End - ItemE.Value.Value, GriddlerPath.Action.CompleteItem);
                                if (Dot != null)
                                    yield return Dot;
                            }
                        }
                        //else if (Gap.HasLastPoint && ItemE.HasValue)
                        //{
                        //    if (Gap.End - ItemE.Value.Value == Gap.Start)
                        //        Skip.Index = Gap.End;
                        //    Line.AddPoints(Gap.End - ItemE.Value.Value + 1, Gap.End - 1, ItemE.Value.Colour, GriddlerPath.Action.CompleteItem, ItemE.Value.Index);

                        //    if (Line.ShouldAddDots(ItemE.Value.Index).Item1)
                        //        Line.AddDot(Gap.End - ItemE.Value.Value, GriddlerPath.Action.CompleteItem);
                        //}
                    }
                }

                //bool FirstSolid = false;

                //foreach ((int Pos, (int, int) Xy, _, _, int GapSize, _) in ForEachLinePos(Line))
                //{
                //    int GapSizeCopy = GapSize;
                //    if (dots.ContainsKey(Xy) || Pos == Line.LineLength - 1)
                //    {
                //        int IsEnd = (!dots.ContainsKey(Xy) && Pos == Line.LineLength - 1) ? 1 : 0;

                //        if (IsEnd == 1)
                //            GapSizeCopy++;

                //        if (GapSizeCopy > 0)
                //        {
                //            if (Line.MinItem > GapSizeCopy)
                //            {
                //                foreach (Point Point in AddPoints(Line, Pos - GapSizeCopy + IsEnd, false, GriddlerPath.Action.GapDotsMinItem, Pos + IsEnd - 1, true))
                //                    yield return Point;
                //            }
                //            else
                //            {
                //                LineSegment Ls = Line.GetItemAtPos(Pos - GapSizeCopy + IsEnd);
                //                LineSegment LsEnd = Line.GetItemAtPosB(Pos - 1 + IsEnd);

                //                //multiple solids
                //                ItemRange Range = Ls.With(LsEnd);
                //                int Sum = Range.Sum();

                //                bool NextColourSumTooBig = false;
                //                Item? FirstColourItem = LsEnd.After.LastOrDefault(l => l.Index < LsEnd.Index &&
                //                                                l.Green == Ls.RightBefore.First().Green);
                //                if (LsEnd.Valid && LsEnd.Eq && FirstColourItem.HasValue
                //                    && Ls.RightBefore.First().EndIndex == Pos - GapSizeCopy + IsEnd - 2)
                //                {
                //                    int ColourSum = new ItemRange(LsEnd.After.Where(w => w.Index > FirstColourItem.Value.Index)).Sum();
                //                    if (ColourSum > GapSizeCopy)
                //                        NextColourSumTooBig = false;
                //                }

                //                if (Range.All(a => a.Value > GapSizeCopy)
                //                    || (Ls.Eq && LsEnd.Eq && Sum > GapSizeCopy)
                //                    || (Ls.Eq && Ls.Valid && Ls.Item?.Value > GapSizeCopy)
                //                    || (LsEnd.Eq && LsEnd.Valid && LsEnd.Item?.Value > GapSizeCopy)
                //                    || (Ls.Eq && Ls.Index > Line.LineItems - 1)
                //                    || (LsEnd.Eq && LsEnd.Index < 0)
                //                    || (Ls.Eq && LsEnd.Eq && Ls.Index > LsEnd.Index)
                //                    || (LsEnd.Valid && Ls.Eq && LsEnd.LastItemAtEquality < Ls.Index)
                //                    || (Ls.Valid && LsEnd.Eq && Ls.LastItemAtEquality > LsEnd.Index)
                //                    || (NextColourSumTooBig)
                //                    || (Ls.Eq && LsEnd.ScV && Ls.Index < Line.LineItems - 1 && Line[Ls.Index + 1].Value > LsEnd.Sc
                //                        && Line[Ls.Index + 1] + Line[Ls.Index] > GapSizeCopy)
                //                    || (LsEnd.Eq && Ls.ScV && LsEnd.Index > 0 && Line[LsEnd.Index - 1].Value > Ls.Sc
                //                        && Line[LsEnd.Index - 1] + Line[LsEnd.Index] > GapSizeCopy)
                //                    )
                //                {
                //                    GriddlerPath.Action Action = GriddlerPath.Action.GapDotsTooBig;
                //                    //if (Eq && EqEnd && Sum > GapSizeCopy)
                //                    //    Action = GriddlerPath.Action.GapDotsSum;
                //                    //else if (Valid && Eq && Line[Item].Value > GapSizeCopy)
                //                    //    Action = GriddlerPath.Action.GapDotsSumF;
                //                    //else if (ValidEnd && EqEnd && Line[ItemEnd].Value > GapSizeCopy)
                //                    //    Action = GriddlerPath.Action.GapDotsSumB;
                //                    //else if (Eq && Item > Line.LineItems - 1)
                //                    //    Action = GriddlerPath.Action.GapDotsNoMoreItemsF;
                //                    //else if (EqEnd && ItemEnd < 0)
                //                    //    Action = GriddlerPath.Action.GapDotsNoMoreItemsB;

                //                    Point.Group++;
                //                    foreach (Point Point in AddPoints(Line, Pos - GapSizeCopy + IsEnd, false, Action, Pos + IsEnd - 1, true))
                //                        yield return Point;
                //                }
                //                else if (Ls.Valid && (LsEnd.Valid
                //                    || (Ls.Eq && Ls.Index > 0 && LsEnd.ScV && Line[Ls.Index].Value != LsEnd.Sc)
                //                    ) && Sum == GapSizeCopy)
                //                {
                //                    foreach (Point Point in FullPart(Line, Pos - GapSizeCopy + IsEnd, Ls.Index, LsEnd.Index, GriddlerPath.Action.GapFull))
                //                        yield return Point;
                //                }
                //                else if (Ls.Valid && FirstSolid && Ls.Index <= LsEnd.Index && Ls.ItemsOneValue)
                //                {
                //                    foreach (Point Point in FullPart(Line, Pos - GapSizeCopy + IsEnd, Ls.Index, Ls.Index, GriddlerPath.Action.GapFull, false))
                //                        yield return Point;
                //                }
                //                else if (Ls.Valid && LsEnd.Valid && (Ls.Index == LsEnd.Index || Ls.Eq || LsEnd.Eq)
                //                    && Sum < GapSizeCopy && Sum > GapSizeCopy / 2)
                //                {
                //                    GriddlerPath.Action Action = GriddlerPath.Action.GapOverlapSameItem;
                //                    if (Ls.Eq || LsEnd.Eq)
                //                        Action = GriddlerPath.Action.GapOverlap;

                //                    foreach (Point Point in OverlapPart(Line, Pos - GapSizeCopy + IsEnd, Pos - 1 + IsEnd, Ls.Index, LsEnd.Index, Action))
                //                        yield return Point;
                //                }
                //            }
                //            FirstSolid = false;
                //        }
                //    }
                //    else
                //    {
                //        if (GapSizeCopy == 0 && points.ContainsKey(Xy))
                //            FirstSolid = true;
                //    }
                //}
            }
        }

        private IEnumerable<Point> CompleteItem(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                (bool IsLineIsolated, _, _) = Line.IsLineIsolated();
                int BlockCount = 0;

                foreach (var (Block, Gap, Ls, Skip) in Line.GetBlocks(true))
                {
                    bool CompleteItem(out bool S, out bool E, out int? ItmIdx)
                    {
                        (S, E) = (Line.ItemsOneColour, Line.ItemsOneColour);
                        ItmIdx = null;

                        if (Line.MaxItem == Block.Size)
                            return true;
                        else if (IsLineIsolated && BlockCount < Line.LineItems
                            && Line[BlockCount].Value == Block.Size)
                        {
                            (S, E) = Line.ShouldAddDots(BlockCount);
                            return true;
                        }
                        else if (Ls.All(Block.IsOrCantBe))
                            return true;

                        Block? LastBlock = Gap.GetLastBlock(Block.Start - 1);
                        if (LastBlock != null)
                        {
                            Item? LastItem = Ls.FirstOrDefault(LastBlock.CanBe);

                            if (LastItem.HasValue && Ls.All(a => Line.IsolatedPart(a, LastBlock, Block))
                                && Line.Where(LastItem.Value.Index + 1, Line.LineItems - 1)
                                .All(Block.IsOrCantBe))
                                return true;
                        }

                        LineSegment LsEnd = Line.GetItemAtPosB(Gap, Block);
                        var (ItemE, EqualityE, IndexE, _, _) = LsEnd;

                        //if (LastBlock != null)
                        //{
                        //    ;
                        //}

                        return Ls.With(LsEnd).All(Block.IsOrCantBe);
                    }

                    if (CompleteItem(out bool S, out bool E, out int? ItmIdx))
                    {
                        Point.Group++;

                        if (S)
                        {
                            if (Block.Start - 1 > Gap.Start)
                                BlockCount--;
                            else
                                Skip.Index = Block.End;

                            Point? Dot = Line.AddDot(Block.Start - 1, GriddlerPath.Action.CompleteItem);
                            if (Dot != null)
                                yield return Dot;
                        }

                        if (E)
                        {
                            Point? Dot = Line.AddDot(Block.End + 1, GriddlerPath.Action.CompleteItem);
                            if (Dot != null)
                                yield return Dot;
                        }
                    }

                    BlockCount++;
                }
            }
        }

        private IEnumerable<IEnumerable<Point>> LineBlocks(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                var (LineIsolated, _, _) = Line.IsLineIsolated();
                Block? LastBlock = null;
                int BlockCount = 0;

                foreach (var (Block, Gap, Ls, Skip) in Line.GetBlocks(true))
                {
                    if (Gap.IsFull)
                    {
                        BlockCount++;
                        continue;
                    }

                    var (Item, Equality, Index, EqualityIndex, IndexAtBlock) = Ls;
                    LineSegment LsEnd = Line.GetItemAtPosB(Gap, Block);
                    var (ItemE, EqualityE, IndexE, EqualityIndexE, IndexAtBlockE) = LsEnd;

                    //no join
                    if (!Line.Dots.Contains(Block.End + 1)
                        && Line.Points.ContainsKey(Block.End + 2))
                    {
                        Block? NextBlock = Gap.GetBlockAtStart(Block.End + 2);
                        if (NextBlock != null && Block.Colour == NextBlock.Colour
                            && Line.All(e => (e.Value > 1 || e.Colour == Block.Colour)
                                && e.Value < NextBlock.End - Block.Start + 1))
                        {
                            yield return new[] { Line.AddDot(Block.End + 1, GriddlerPath.Action.NoJoin) };
                            BlockCount++;
                            continue;
                        }
                    }

                    //must join
                    if (!Line.Dots.Contains(Block.End + 1)
                        && !Line.Points.ContainsKey(Block.End + 1)
                        && Line.Points.ContainsKey(Block.End + 2)
                        && !Line.Pair().Any(s => Block.CanBe(s.Item1)
                        && s.Item1.Value <= Block.End - Gap.Start + 1
                        && s.Item2.Value <= Gap.End
                        - (Block.End + 1 + Line.GetDotCount(s.Item1.Index)) + 1))
                    {
                        Point? Pt = Line.AddPoint(Block.End + 1, Block.Colour, GriddlerPath.Action.MustJoin);

                        if (Pt != null)
                            yield return new[] { Pt };
                        continue;
                    }
                    else if (Equality && EqualityE && Index + 1 == IndexE)
                    {
                        Block? NextBlock = Gap.GetNextBlock(Block.End + 1);

                        if (LastBlock != null && NextBlock != null && Item.HasValue)
                        {
                            bool Isolated = Line.IsolatedPart(Item.Value, Block, LastBlock);

                            if (Isolated)
                            {
                                yield return Line.AddPoints(Block.End + 1,
                                                            NextBlock.Start - 1,
                                                            Block.Colour,
                                                            GriddlerPath.Action.MustJoin);
                                continue;
                            }
                        }
                    }

                    //min item backwards
                    bool MinItemBackwards(out int m)
                    {
                        m = 0;

                        if (EqualityE && ItemE.HasValue)
                        {
                            Block? NextBlock = Gap.GetNextBlock(Block.Start + 1);
                            if (NextBlock != null)
                            {
                                int ItemShift = Line.SumWhile(IndexE, Gap, Block, false);
                                bool Isolated = Line.IsolatedPart(ItemE.Value, Block, NextBlock, false);

                                if (ItemShift == 1 && Isolated)
                                {
                                    m = new ItemRange(Line.Where(IndexE - 1, IndexE)).Sum();
                                    return true;
                                }
                            }
                        }

                        if (Index == 0 && Block.End == Gap.End && BlockCount == 1)
                        {
                            int ItemShift = Line.SumWhile(0, Gap);
                            if (ItemShift == 2 && LastBlock != null
                                && Line.IsolatedPart(Line[ItemShift - 2], Block, LastBlock))
                            {
                                m = Line[ItemShift - 1].Value;
                                return true;
                            }
                        }

                        //m = Line.min(Ls.With(LsEnd), Block);
                        return true;
                    };
                    if (MinItemBackwards(out int mB) && false
                        && Gap.End - mB + 1 < Block.Start)
                    {
                        yield return Line.AddPoints(Gap.End - mB + 1, Block.Start - 1, Block.Colour, GriddlerPath.Action.MinItem);
                        BlockCount++;
                        continue;
                    }

                    //min item forwards
                    bool MinItemForwards(out int m)
                    {
                        m = 0;

                        if (Line.UniqueCount(Block, out Item Itm))
                        {
                            m = Itm.Value;
                            return true;
                        }

                        if (Equality && Item.HasValue)
                        {
                            Block? LastBlock = Gap.GetLastBlock(Block.Start - 1);
                            if (LastBlock != null)
                            {
                                int ItemShift = Line.SumWhile(Index, Gap, Block);
                                bool Isolated = Line.IsolatedPart(Item.Value, Block, LastBlock);

                                if (ItemShift == 1 && Isolated)
                                {
                                    m = new ItemRange(Line.Where(Index, Index + 1)).Sum();
                                    return true;
                                }
                            }
                        }
                        //m = Line.Min(Ls.With(LsEnd), Block);
                        return true;
                    };
                    if (MinItemForwards(out int m) && false
                        && Gap.Start + m - 1 > Block.End)
                    {
                        yield return Line.AddPoints(Block.End + 1,
                                                    Gap.Start + m - 1,
                                                    Block.Colour,
                                                    GriddlerPath.Action.MinItem);
                        BlockCount++;
                        continue;
                    }

                    //single item Start
                    bool SingleItemStart(out int singleItem)
                    {
                        singleItem = 0;

                        if (LineIsolated && (Gap.NumberOfBlocks == 1 || BlockCount == 0
                            || Gap.GetLastBlock(Block.Start - 1) == null)
                            && Block.End - Line[BlockCount].Value >= Gap.Start)
                        {
                            singleItem = Line[BlockCount].Value;
                            return true;
                        }
                        //else if (IsolatedItem && (Gap.NumberOfBlocks == 1 || BlockCount == 0
                        //    || Gap.GetLastBlock(Block.Start - 1) == null)
                        //    && Block.End - Line[IsolatedItem].Value >= Gap.Start)
                        //{
                        //    singleItem = Line[IsolatedItem].Value;
                        //}
                        else if (Equality && EqualityE && Index == IndexE
                            && Gap.NumberOfBlocks == 1)
                        {
                            singleItem = Line.Where(Index, IndexE).FirstOrDefault().Value;
                            return true;
                        }
                        else if (Ls.UniqueCount(Block, out Item Item)
                                && Item.Index == EqualityIndex)
                        {
                            singleItem = Item.Value;
                            return true;
                        }

                        return false;
                    }
                    if (SingleItemStart(out int Sis) && false
                        && Block.End - Sis >= Gap.Start)
                    {
                        yield return Line.AddDots(Gap.Start,
                                                  Block.End - Sis,
                                                  GriddlerPath.Action.ItemBackwardReach);
                        Skip.Index = Block.End;
                    }

                    //single item End
                    bool SingleItemEnd(out int singleItem)
                    {
                        singleItem = 0;

                        if (LineIsolated && (Gap.NumberOfBlocks == 1
                            || BlockCount == Line.LineItems - 1
                            || Gap.GetNextBlock(Block.End + 1) == null))
                        {
                            singleItem = Line[BlockCount].Value;
                            return true;
                        }
                        //breaks sevenswansswimming45x35, daffodills30x35, +1 more
                        //else if (isolatedItem && (Gap.NumberOfBlocks == 1
                        //    || isolatedItem == Line.LineItems - 1 || !Gap.GetNextBlock(Block.End + 1))) {
                        //    singleItem = Line[isolatedItem].Value;
                        //}
                        else if (Equality && IndexAtBlock == Index && Gap.NumberOfBlocks == 1
                            && (Index + 1 > Line.LineItems - 1
                            || !Line.FitsInSpace(Block, Gap, Line[Index + 1])))
                        {
                            singleItem = Line[Index].Value;
                            return true;
                        }
                        else if (LsEnd.UniqueCount(Block, out Item item)
                                && item.Index == EqualityIndexE)
                        {
                            singleItem = item.Value;
                            return true;
                        }

                        return false;
                    }
                    if (SingleItemEnd(out int Sie) && false && Block.Start + Sie <= Gap.End)
                        Line.AddDots(Block.Start + Sie, Gap.End, GriddlerPath.Action.ItemForwardReach);

                    //sum dot forward
                    if (false && Item.HasValue && IndexAtBlock - 1 >= 0 && IndexAtBlock - 1 <= Line.LineItems - 1
                        && Line.Where(EqualityIndex, IndexAtBlock - 1).All(e => Block.Is(e))
                        && Gap.Start + new ItemRange(Line.Where(Index, IndexAtBlock - 1)).Sum() - 1
                        == Block.Start - 1 - Line.GetDotCount(IndexAtBlock - 1))
                    {
                        int GapStart = Gap.Start;
                        Line.AddDot(Block.Start - 1, GriddlerPath.Action.SumDotForward);

                        if (Block.Start - 1 > GapStart)
                        {
                            BlockCount--;
                            continue;
                        }
                        else
                            Skip.Index = Block.End;
                    }

                    //sum dot backward
                    if (ItemE.HasValue && IndexAtBlockE >= 0 && false
                        && IndexAtBlockE + 1 <= Line.LineItems - 1
                        && Line.Where(IndexAtBlockE + 1, EqualityIndexE).All(e => Block.Is(e)
                        && Gap.End - new ItemRange(Line.Where(IndexAtBlockE + 1, IndexE)).Sum() + 1
                        == Block.End + 1 + Line.GetDotCount(IndexAtBlockE)))
                    {
                        Line.AddDot(Block.End + 1, GriddlerPath.Action.SumDotBackward);
                    }

                    //half Gap overlap backwards
                    bool HalfGapOverlapBackwards(out ItemRange r)
                    {
                        if (Ls.With(LsEnd).UniqueCount(Block, out Item item)
                            && item.Index > Index)
                        {
                            r = new ItemRange(Line, Index, item.Index - 1);
                            return true;
                        }
                        else if (ItemE.HasValue && Equality && IndexAtBlockE - 1 >= 0)
                        {
                            r = new ItemRange(Line, EqualityIndex, IndexAtBlockE - 1);
                            return true;
                        }

                        r = new ItemRange();
                        return false;
                    };
                    if (HalfGapOverlapBackwards(out ItemRange r) && false)
                    {
                        int Sum = r.Sum();
                        var Space = Line.SpaceBetween(Gap, Block, Line[r.End]);
                        if (Sum < Space.Item1 && Sum > Space.Item1 / 2)
                            yield return OverlapPart(Line, Gap.Start, Block.Start - 1 - Space.Item3, r.Start, r.End, GriddlerPath.Action.HalfGapOverlap);
                    }

                    //half Gap overlap forwards
                    //const halfGapOverlapForwards = () => {
                    //    if (Valid && EqualityE && indexAtBlock + 1 <= Line.LineItems - 1)
                    //        return [indexAtBlock, equalityIndexE];
                    //}
                    //const hGapOvFor = halfGapOverlapForwards();
                    //if (hGapOvFor)
                    //{
                    //    const sum = Line.sum(true, Line.filterItems(hGapOvFor[0] + 1, hGapOvFor[1]));
                    //    const space = Line.spaceBetween(Block, Gap, Line[hGapOvFor[0]]);
                    //    if (sum < space[0] && sum > space[0] / 2)
                    //        overlapPart(Line, Block.End + 1 + space[1], Gap.End, hGapOvFor[0] + 1, hGapOvFor[1], GriddlerPath.Action.HalfGapOverlap);
                    //}

                    //half Gap full part forwards
                    //if (hGapOvFor)
                    //{
                    //    const sum = Line.sum(true, Line.filterItems(hGapOvFor[0] + 1, hGapOvFor[1]));
                    //    const space = Line.spaceBetween(Block, Gap, Line[hGapOvFor[0]]);
                    //    if (sum == space[0])
                    //        fullPart(Line, Block.End + 1 + space[1], hGapOvFor[0] + 1, hGapOvFor[1], GriddlerPath.Action.HalfGapFullPart);
                    //}

                    //half Gap full part backwards
                    //if (hGapOvBck)
                    //{
                    //    int Sum = Line.sum(true, Line.filterItems(hGapOvBck[0], hGapOvBck[1] - 1));
                    //    int Space = Line.SpaceBetween(Gap, Block, Line[hGapOvBck[1] - 1]);
                    //    if (Sum == Space[0])
                    //        OverlapPart(Line, Gap.Start, Block.Start - 1 - space[2], hGapOvBck[0], hGapOvBck[1] - 1, GriddlerPath.Action.HalfGapOverlap);
                    //}

                    //Isolated items reach
                    if (LineIsolated && Gap.NumberOfBlocks > 1
                        && BlockCount < Line.LineItems - 1)
                    {
                        Item NextItem = Line[BlockCount + 1];
                        int Start = Block.Start + Line[BlockCount].Value;
                        Block? NextBlock = Gap.GetNextBlock(Block.End + 1);

                        if (NextBlock != null)
                            Line.AddDots(Start, NextBlock.End - NextItem.Value, GriddlerPath.Action.IsolatedItemsReach);
                    }
                    //else if (isolatedItem && isolatedItem !== isolatedItems.get(BlockCount + 1)
                    //    && Gap.NumberOfBlocks > 1 && isolatedItem < Line.LineItems - 1)
                    //{
                    //    Item NextItem = Line[IsolatedItem + 1];
                    //    int Start = Block.Start + Line[IsolatedItem].Value;
                    //    Block? NextBlock = Gap.GetNextBlock(Block.End + 1);

                    //    if (NextBlock != null)
                    //        Line.AddDots(Start, NextBlock.End - NextItem.Value, GriddlerPath.Action.IsolatedItemsReach);
                    //}

                    BlockCount++;
                    LastBlock = Block;
                }
            }
        }

        /// <summary>
        /// Restrict where items of a certain colour can go by looking at 
        /// the perpendicular lines and their appropriate items. 
        /// Probably only for the edges.
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> MultiColourCrossReference(IEnumerable<Line> lines)
        {
            int MaxLines = lines.First().IsRow ? Rows.Count : Cols.Count;

            foreach (Line Line in ForEachLine(lines))
            {
                if (Line.LineIndex == 0 || Line.LineIndex == MaxLines - 1)
                {
                    int ColourBlockIndex = 0;
                    List<Block> ColourCounts = new List<Block>(10) { new Block(false, 0) };
                    bool PrevColour = false;

                    foreach ((int Pos, (int, int) Xy, _, _, _, _) in ForEachLinePos(Line))
                    {
                        bool Colour = false;

                        if (Line.IsRow)
                        {
                            int ItemIndex = Line.LineIndex == 0 ? 0 : Cols[Pos].LineItems - 1;
                            Colour = Cols[Pos][ItemIndex].Green;
                        }
                        else
                        {
                            int ItemIndex = Line.LineIndex == 0 ? 0 : Rows[Pos].LineItems - 1;
                            Colour = Rows[Pos][ItemIndex].Green;
                        }

                        if (Colour == PrevColour)
                        {
                            ColourCounts[ColourBlockIndex].EndIndex = Pos;
                            ColourCounts[ColourBlockIndex].SolidCount++;
                        }
                        else
                        {
                            PrevColour = Colour;
                            ColourBlockIndex++;
                            ColourCounts.Add(new Block(Pos, Pos, Colour));
                        }
                    }

                    if (Line.Count(c => c.Green) == ColourCounts.Count(c => c.Green))
                    {
                        foreach ((Block, Item) Item in ColourCounts.Where(w => w.Green).Zip(Line))
                        {
                            if (Item.Item2.Value == Item.Item1.SolidCount)
                            {
                                foreach (Point Point in FullPart(Line, Item.Item1.StartIndex, Item.Item2.Index, Item.Item2.Index, GriddlerPath.Action.GapFull))
                                    yield return Point;
                            }
                            else if (Item.Item2.Value < Item.Item1.SolidCount
                                    && Item.Item2.Value >= Item.Item1.SolidCount / 2)
                            {
                                foreach (Point Point in OverlapPart(Line, Item.Item1.StartIndex, Item.Item1.EndIndex, Item.Item2.Index, Item.Item2.Index, GriddlerPath.Action.GapFull))
                                    yield return Point;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Add one point at a time
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> TrialAndError(IEnumerable<Line> lines)
        {
            bool Flag = false;
            foreach (Line Line in ForEachLine(lines))
            {
                foreach ((int Pos, (int, int) Xy, _, _, _, _) in ForEachLinePos(Line))
                {
                    if (!points.ContainsKey(Xy) && !dots.ContainsKey(Xy)
                        && !IncorrectTrials.Contains(Xy))
                    {
                        LineSegment Ls = Line.GetItemAtPos(Pos);

                        if (Ls.Valid && Ls.Eq && Ls.Item.HasValue)
                        {
                            //add one point
                            foreach (Point Point in AddPoints(Line, Pos, Ls.Item.Value.Green, GriddlerPath.Action.TrialAndError, Pos))
                            {
                                Trials.Add((Point.Xpos, Point.Ypos));
                                yield return Point;
                            }

                            Flag = true;
                            break;
                        }
                    }
                }

                if (Flag)
                    break;
            }
        }

    }
}