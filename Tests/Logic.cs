using Xunit;
using Griddlers.Library;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;

namespace Tests
{
    public class LogicTest
    {
        private readonly string FileRoot = $"{Library.TryGetSolutionDirectoryInfo().FullName}\\Griddlers\\Data\\";

        [Fact]
        //https://www.google.co.uk/search?biw=1366&bih=651&tbm=isch&sa=1&ei=49eDW6qPJcadgAagq6eoBw&q=griddler+puzzle+bird+10x10&oq=griddler+puzzle+bird+10x10&gs_l=img.3...5270.7092.0.7246.8.7.1.0.0.0.101.469.6j1.7.0....0...1c.1.64.img..0.0.0....0.aNqK1hEfN-g#imgdii=t0h858VvE2gcqM:&imgrc=PNMm0vS0934YyM:
        public async Task Bird10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Bird10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();
            
            (Points, Dots) = await Library.GetOutputData(nameof(Bird10x10), 10, 10);

            await Save(nameof(Bird10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);
            
            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());
            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?biw=1366&bih=651&tbm=isch&sa=1&ei=49eDW6qPJcadgAagq6eoBw&q=griddler+puzzle+bird+10x10&oq=griddler+puzzle+bird+10x10&gs_l=img.3...5270.7092.0.7246.8.7.1.0.0.0.101.469.6j1.7.0....0...1c.1.64.img..0.0.0....0.aNqK1hEfN-g#imgrc=PNMm0vS0934YyM:
        public async Task Man10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Man10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Man10x10), 10, 10);

            await Save(nameof(Man10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?biw=1366&bih=651&tbm=isch&sa=1&ei=49eDW6qPJcadgAagq6eoBw&q=griddler+puzzle+bird+10x10&oq=griddler+puzzle+bird+10x10&gs_l=img.3...5270.7092.0.7246.8.7.1.0.0.0.101.469.6j1.7.0....0...1c.1.64.img..0.0.0....0.aNqK1hEfN-g#imgdii=qC3B63j5uriqhM:&imgrc=PNMm0vS0934YyM:
        public async Task Rabbit10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Rabbit10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Rabbit10x10), 10, 10);

            await Save(nameof(Rabbit10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        private async Task Save(string fileName,
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

        [Fact]
        //https://www.google.co.uk/search?biw=1366&bih=651&tbm=isch&sa=1&ei=49eDW6qPJcadgAagq6eoBw&q=griddler+puzzle+bird+10x10&oq=griddler+puzzle+bird+10x10&gs_l=img.3...5270.7092.0.7246.8.7.1.0.0.0.101.469.6j1.7.0....0...1c.1.64.img..0.0.0....0.aNqK1hEfN-g#imgdii=qC3B63j5uriqhM:&imgrc=PNMm0vS0934YyM:
        public async Task Bug10x9()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Bug10x9));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Bug10x9), 10, 9);

            await Save(nameof(Bug10x9), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=THi3s9ixtk6UNM:
        public async Task Snail10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Snail10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Snail10x10), 10, 10);

            await Save(nameof(Snail10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=MSnBjI8o1iuopM:
        public async Task Leaf10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Leaf10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Leaf10x10), 10, 10);

            await Save(nameof(Leaf10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=S5ko5B-eEh9v_M:
        public async Task Notes10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Notes10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Notes10x10), 10, 10);

            await Save(nameof(Notes10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=PeFfVG8VKLEkEM:
        public async Task Tree10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Tree10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Tree10x10), 10, 10);

            await Save(nameof(Tree10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }
        
        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=DfNZfo_-hbVfxM:
        public async Task TV10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(TV10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(TV10x10), 10, 10);

            await Save(nameof(TV10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=zdALuB1t2uoA7M:
        public async Task Heart10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Heart10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Heart10x10), 10, 10);

            await Save(nameof(Heart10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=Sw4rBRw33nejSM:
        public async Task Mouse10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Mouse10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Mouse10x10), 10, 10);

            await Save(nameof(Mouse10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=v5wYBQ7dY-TLoM:
        public async Task Face10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Face10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Face10x10), 10, 10);

            await Save(nameof(Face10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgdii=1qbET0KKAY9Q7M:&imgrc=PeFfVG8VKLEkEM:
        public async Task Coffee10x10()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Coffee10x10));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Coffee10x10), 10, 10);

            await Save(nameof(Coffee10x10), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        //not solvable!
        //[Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-46-17.png?w=840
        private async Task IceCream11x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(IceCream11x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(IceCream11x20), 11, 20);

            await Save(nameof(IceCream11x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-44-41.png?w=840
        public async Task ChessKnight13x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(ChessKnight13x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(ChessKnight13x20), 13, 20);

            await Save(nameof(ChessKnight13x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-44-33.png?w=840
        public async Task Matryoshka13x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Matryoshka13x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Matryoshka13x20), 13, 20);

            await Save(nameof(Matryoshka13x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-46-04.png
        public async Task StripedFish20x14()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(StripedFish20x14));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(StripedFish20x14), 20, 14);

            await Save(nameof(StripedFish20x14), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-45-52.png?w=840
        public async Task Necklace14x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Necklace14x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Necklace14x20), 14, 20);

            await Save(nameof(Necklace14x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-45-44.png?w=840
        public async Task Pineapple14x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Pineapple14x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Pineapple14x20), 14, 20);

            await Save(nameof(Pineapple14x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-45-30.png?w=840
        public async Task Cheburashka20x14()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Cheburashka20x14));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Cheburashka20x14), 20, 14);

            await Save(nameof(Cheburashka20x14), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-43-31.png?w=840
        public async Task Apple16x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Apple16x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Apple16x15), 16, 15);

            await Save(nameof(Apple16x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-45-22.png?w=840
        public async Task Jar17x16()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Jar17x16));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Jar17x16), 17, 16);

            await Save(nameof(Jar17x16), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-44-23.png?w=840
        public async Task Sakura17x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Sakura17x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Sakura17x15), 17, 15);

            await Save(nameof(Sakura17x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-45-09.png?w=840
        public async Task Goose18x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Goose18x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Goose18x15), 18, 15);

            await Save(nameof(Goose18x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);
            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-45-02.png?w=840
        public async Task PolarBear19x14()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(PolarBear19x14));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(PolarBear19x14), 19, 14);

            await Save(nameof(PolarBear19x14), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2ifNsDkP_1I49Erl1vs7RT9lCs30DYJNnPbSBRfBx1bcIyoSCagtwet4-bq4ER98vJwAYfkzKhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCZ82wOQ_18jj0EdZA21ULtYdZKhIJSuXW-ztFP2URmFwIBdXoadEqEgkKzfQNgk2c9hEGAQ6MsGHi6ioSCdIFF8HHVtwjEQYBDoywYeLq&tbo=u&sa=X&ved=2ahUKEwjDyMHT55LdAhXSasAKHdZuC-UQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=Bsrqt2F5zQNBBM:
        public async Task Flower15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Flower15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Flower15x15), 15, 15);

            await Save(nameof(Flower15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2hK5db7O0U_1ZQrN9A2CTZz20gUXwcdW3COfNsDkP_1I49CoSCagtwet4-bq4ESOLE6wyQ1x7KhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCUrl1vs7RT9lETchgBFjNoweKhIJCs30DYJNnPYRBgEOjLBh4uoqEgnSBRfBx1bcIxEGAQ6MsGHi6ioSCZ82wOQ_18jj0EdZA21ULtYdZ&tbo=u&sa=X&ved=2ahUKEwi708rZoJzdAhUqLcAKHdH-AbMQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=vkANFee5wiCXzM:
        public async Task Yoga15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Yoga15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Yoga15x15), 15, 15);

            await Save(nameof(Yoga15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2hK5db7O0U_1ZQrN9A2CTZz20gUXwcdW3COfNsDkP_1I49CoSCagtwet4-bq4ESOLE6wyQ1x7KhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCUrl1vs7RT9lETchgBFjNoweKhIJCs30DYJNnPYRBgEOjLBh4uoqEgnSBRfBx1bcIxEGAQ6MsGHi6ioSCZ82wOQ_18jj0EdZA21ULtYdZ&tbo=u&sa=X&ved=2ahUKEwidgcLi1qrdAhWIKsAKHYeFAOcQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgdii=Mbu83FmRKdb8IM:&imgrc=k8l9RPnI60_9CM:
        public async Task Swan15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Swan15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Swan15x15), 15, 15);

            await Save(nameof(Swan15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CdkavEZy7mqiIjg8qCg6dM5oD7pU-Sld3um-ufhmmAOOCemAHlOPokdN5rlt3UAmHeismkiqWA0ao5PCNihCzKB0ICoSCTyoKDp0zmgPEQwCjUnBG2-QKhIJulT5KV3e6b4R0ifhUhWtHlAqEgm5-GaYA44J6RE9aHrFcDoIMioSCYAeU4-iR03mEfsaS-N10WSHKhIJuW3dQCYd6KwRkSWC2I98st0qEgmaSKpYDRqjkxGVEkYQaWDE1CoSCcI2KELMoHQgET435ZmjY_10Q&tbo=u&sa=X&ved=2ahUKEwjTr6bu36rdAhVlIMAKHWFuDgwQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=2Rq8RnLuaqJ8EM:
        public async Task Turtles15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Turtles15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Turtles15x15), 15, 15);

            await Save(nameof(Turtles15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CdkavEZy7mqiIjg8qCg6dM5oD7pU-Sld3um-ufhmmAOOCemAHlOPokdN5rlt3UAmHeismkiqWA0ao5PCNihCzKB0ICoSCTyoKDp0zmgPEQwCjUnBG2-QKhIJulT5KV3e6b4R0ifhUhWtHlAqEgm5-GaYA44J6RE9aHrFcDoIMioSCYAeU4-iR03mEfsaS-N10WSHKhIJuW3dQCYd6KwRkSWC2I98st0qEgmaSKpYDRqjkxGVEkYQaWDE1CoSCcI2KELMoHQgET435ZmjY_10Q&tbo=u&sa=X&ved=2ahUKEwjTr6bu36rdAhVlIMAKHWFuDgwQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=t0AjfUAzU_WG-M:
        public async Task Clock15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Clock15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Clock15x15), 15, 15);

            await Save(nameof(Clock15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CdkavEZy7mqiIjg8qCg6dM5oD7pU-Sld3um-ufhmmAOOCemAHlOPokdN5rlt3UAmHeismkiqWA0ao5PCNihCzKB0ICoSCTyoKDp0zmgPEQwCjUnBG2-QKhIJulT5KV3e6b4R0ifhUhWtHlAqEgm5-GaYA44J6RE9aHrFcDoIMioSCYAeU4-iR03mEfsaS-N10WSHKhIJuW3dQCYd6KwRkSWC2I98st0qEgmaSKpYDRqjkxGVEkYQaWDE1CoSCcI2KELMoHQgET435ZmjY_10Q&tbo=u&sa=X&ved=2ahUKEwjTr6bu36rdAhVlIMAKHWFuDgwQ9C96BAgBEBg&biw=1366&bih=651&dpr=1#imgrc=e8y48fCRUdRjeM:
        public async Task Moose15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Moose15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Moose15x15), 15, 15);

            await Save(nameof(Moose15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2hK5db7O0U_1ZQrN9A2CTZz20gUXwcdW3COfNsDkP_1I49CoSCagtwet4-bq4ESOLE6wyQ1x7KhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCUrl1vs7RT9lETchgBFjNoweKhIJCs30DYJNnPYRBgEOjLBh4uoqEgnSBRfBx1bcIxEGAQ6MsGHi6ioSCZ82wOQ_18jj0EdZA21ULtYdZ&tbo=u&sa=X&ved=2ahUKEwjj4r_Ttq7dAhXHA8AKHUySAN0Q9C96BAgBEBg&biw=1438&bih=685&dpr=0.95#imgrc=m2hYMGZmI6KHnM:
        public async Task Pelican15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Pelican15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Pelican15x15), 15, 15);

            await Save(nameof(Pelican15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://www.google.co.uk/search?q=griddler+puzzle+bird+10x10&tbm=isch&tbs=rimg:CTzTJtL0tPd-IjioLcHrePm6uGA0HhMCApVVt0h858VvE2hK5db7O0U_1ZQrN9A2CTZz20gUXwcdW3COfNsDkP_1I49CoSCagtwet4-bq4ESOLE6wyQ1x7KhIJYDQeEwIClVURiTpzPlBEB98qEgm3SHznxW8TaBGsmrPUwlbWSSoSCUrl1vs7RT9lETchgBFjNoweKhIJCs30DYJNnPYRBgEOjLBh4uoqEgnSBRfBx1bcIxEGAQ6MsGHi6ioSCZ82wOQ_18jj0EdZA21ULtYdZ&tbo=u&sa=X&ved=2ahUKEwjj4r_Ttq7dAhXHA8AKHUySAN0Q9C96BAgBEBg&biw=1438&bih=685&dpr=0.95#imgdii=gMnU_DD9ipPvWM:&imgrc=m2hYMGZmI6KHnM:
        public async Task Girafe15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Girafe15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Girafe15x15), 15, 15); ;

            await Save(nameof(Girafe15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Ski15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Ski15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Ski15x15), 15, 15);

            await Save(nameof(Ski15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Shared15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Shared15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Shared15x15), 15, 15);

            await Save(nameof(Shared15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Amphora15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Amphora15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Amphora15x15), 15, 15);

            await Save(nameof(Amphora15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Itzy15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Itzy15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Itzy15x15), 15, 15);

            await Save(nameof(Itzy15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Ostrich15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Ostrich15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Ostrich15x15), 15, 15);

            await Save(nameof(Ostrich15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanji Issue 65
        public async Task Bye15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Bye15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Bye15x15), 15, 15);

            await Save(nameof(Bye15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Sparrow15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Sparrow15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Sparrow15x15), 15, 15);

            await Save(nameof(Sparrow15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task InTheGym15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(InTheGym15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(InTheGym15x15), 15, 15);

            await Save(nameof(InTheGym15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Cook15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Cook15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Cook15x15), 15, 15);

            await Save(nameof(Cook15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 65
        public async Task Bulb15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Bulb15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Bulb15x15), 15, 15);

            await Save(nameof(Bulb15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Celebration15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Celebration15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Celebration15x15), 15, 15);

            await Save(nameof(Celebration15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Spider15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Spider15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Spider15x15), 15, 15);

            await Save(nameof(Spider15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Wet15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Wet15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Wet15x15), 15, 15);

            await Save(nameof(Wet15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Wicked15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Wicked15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Wicked15x15), 15, 15);

            await Save(nameof(Wicked15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Tropical15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Tropical15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Tropical15x15), 15, 15);

            await Save(nameof(Tropical15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanji Issue 113
        public async Task Creature15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Creature15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Creature15x15), 15, 15);

            await Save(nameof(Creature15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Little15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Little15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Little15x15), 15, 15);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Little15x15), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Liar15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Liar15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Liar15x15), 15, 15);

            await Save(nameof(Liar15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Yawn15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Yawn15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Yawn15x15), 15, 15);

            await Save(nameof(Yawn15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanji Issue 113
        public async Task Bird15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Bird15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Bird15x15), 15, 15);

            await Save(nameof(Bird15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-43-13.png?w=840
        public async Task Beer15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Beer15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Beer15x15), 15, 15);

            await Save(nameof(Beer15x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-42-55.png?w=768
        public async Task Chicken15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Chicken15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Chicken15x15), 15, 15);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Chicken15x15), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-42-49.png?w=768
        public async Task Butterfly15x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Butterfly15x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Butterfly15x15), 15, 15);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Butterfly15x15), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-42-33.png?w=768
        public async Task Umbrella14x16()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Umbrella14x16));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Umbrella14x16), 14, 16);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Umbrella14x16), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-42-22.png?w=768
        public async Task Shoe16x14()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Shoe16x14));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Shoe16x14), 16, 14);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Shoe16x14), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-42-13.png?w=768
        public async Task Lamp11x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Lamp11x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Lamp11x20), 11, 20);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Lamp11x20), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-42-06.png?w=768
        public async Task SumoWrestler14x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(SumoWrestler14x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(SumoWrestler14x15), 14, 15);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(SumoWrestler14x15), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-44-08.png?w=840
        public async Task Sail15x17()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Sail15x17));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Sail15x17), 15, 17);

            await Save(nameof(Sail15x17), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-43-47.png?w=840
        public async Task GirlInSunglasses15x17()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(GirlInSunglasses15x17));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(GirlInSunglasses15x17), 15, 17);

            await Save(nameof(GirlInSunglasses15x17), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_224252.png
        public async Task JollyRoger20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(JollyRoger20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(JollyRoger20x15), 20, 15);

            await Save(nameof(JollyRoger20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }
        
        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_224220.png
        public async Task Gun20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Gun20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Gun20x15), 20, 15);

            await Save(nameof(Gun20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_224136.png
        public async Task Glasses20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Glasses20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Glasses20x15), 20, 15);

            await Save(nameof(Glasses20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_224054.png
        public async Task Doggie20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Doggie20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Doggie20x15), 20, 15);

            await Save(nameof(Doggie20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_224004.png
        public async Task Boot20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Boot20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Boot20x15), 20, 15);

            await Save(nameof(Boot20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_223928.png
        public async Task Jeep20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Jeep20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Jeep20x15), 20, 15);

            await Save(nameof(Jeep20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_223811.png
        public async Task Goldfish20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Goldfish20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Goldfish20x15), 20, 15);

            await Save(nameof(Goldfish20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_223709.png
        public async Task Fencer20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Fencer20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Fencer20x15), 20, 15);

            await Save(nameof(Fencer20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161207_223532.png
        public async Task Diplodocus20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Diplodocus20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Diplodocus20x15), 20, 15);

            await Save(nameof(Diplodocus20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanjie issue 65
        public async Task HereDaisy20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(HereDaisy20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(HereDaisy20x15), 20, 15);

            await Save(nameof(HereDaisy20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task CountryLodge20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(CountryLodge20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(CountryLodge20x15), 20, 15);

            await Save(nameof(CountryLodge20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Pig20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Pig20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Pig20x15), 20, 15);

            await Save(nameof(Pig20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Pumpkin20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Pumpkin20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Pumpkin20x15), 20, 15);

            await Save(nameof(Pumpkin20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task NightLight20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(NightLight20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(NightLight20x15), 20, 15);

            await Save(nameof(NightLight20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Vampire20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Vampire20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Vampire20x15), 20, 15);

            await Save(nameof(Vampire20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Bonfire20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Bonfire20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Bonfire20x15), 20, 15);

            await Save(nameof(Bonfire20x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Poodle20x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Poodle20x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Poodle20x15), 20, 15);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);
            
            await Save(nameof(Poodle20x15), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanjie issue 65
        public async Task Landfall15x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Landfall15x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Landfall15x20), 15, 20);

            await Save(nameof(Landfall15x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Centaur15x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Centaur15x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Centaur15x20), 15, 20);

            await Save(nameof(Centaur15x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-43-29.png?w=840
        public async Task Teapot17x14()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Teapot17x14));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Teapot17x14), 17, 14);

            await Save(nameof(Teapot17x14), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-44-17.png?w=840
        public async Task Fishes17x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Fishes17x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Fishes17x15), 17, 15);

            await Save(nameof(Fishes17x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-43-55.png?w=840
        public async Task Mortarboard17x15()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Mortarboard17x15));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Mortarboard17x15), 17, 15);

            await Save(nameof(Mortarboard17x15), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-43-37.png?w=840
        public async Task Sushi19x13()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Sushi19x13));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Sushi19x13), 19, 13);

            await Save(nameof(Sushi19x13), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }



        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/12/screenshot_2017-12-06-15-44-50.png?w=840
        public async Task Ninja22x12()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Ninja22x12));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Ninja22x12), 22, 12);

            await Save(nameof(Ninja22x12), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }



        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161208_151846.png
        public async Task TomaHawk20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(TomaHawk20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(TomaHawk20x20), 20, 20);

            await Save(nameof(TomaHawk20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161208_152713.png
        public async Task Dino20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Dino20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Dino20x20), 20, 20);

            await Save(nameof(Dino20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161208_152121.png
        public async Task Butterfly20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Butterfly20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Butterfly20x20), 20, 20);

            await Save(nameof(Butterfly20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161208_151731.png
        public async Task Teapot20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Teapot20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Teapot20x20), 20, 20);            

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);
            
            await Save(nameof(Teapot20x20), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161208_152601.png
        public async Task Kitty20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Kitty20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Kitty20x20), 20, 20);

            await Save(nameof(Kitty20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161208_152039.png
        public async Task Bus20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Bus20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Bus20x20), 20, 20);

            await Save(nameof(Bus20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2016/12/20161208_151546.png
        public async Task Peacock20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Peacock20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Peacock20x20), 20, 20);

            await Save(nameof(Peacock20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Phone20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Phone20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Phone20x20), 20, 20);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Phone20x20), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Flower20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Flower20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Flower20x20), 20, 20);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);
            
            await Save(nameof(Flower20x20), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Portable20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Portable20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Portable20x20), 20, 20);

            await Save(nameof(Portable20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }
        
        [Fact]
        //hanjie issue 65
        public async Task Dragon20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Dragon20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Dragon20x20), 20, 20);

            await Save(nameof(Dragon20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Worzel20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Worzel20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Worzel20x20), 20, 20);

            await Save(nameof(Worzel20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }
        
        [Fact]
        //hanjie issue 65
        public async Task Mask20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Mask20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Mask20x20), 20, 20);

            await Save(nameof(Mask20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Highlights20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Highlights20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Highlights20x20), 20, 20);

            await Save(nameof(Highlights20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanjie issue 65
        public async Task FastFood20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(FastFood20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(FastFood20x20), 20, 20);

            await Save(nameof(FastFood20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Beg20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Beg20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Beg20x20), 20, 20);

            await Save(nameof(Beg20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task WhatsThat20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(WhatsThat20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(WhatsThat20x20), 20, 20);

            await Save(nameof(WhatsThat20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanjie issue 65
        public async Task Volley20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Volley20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Volley20x20), 20, 20);

            await Save(nameof(Volley20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Budgie20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Budgie20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Budgie20x20), 20, 20);

            await Save(nameof(Budgie20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Lion20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Lion20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Lion20x20), 20, 20);

            await Save(nameof(Lion20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanjie issue 65
        public async Task Swing20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Swing20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Swing20x20), 20, 20);

            await Save(nameof(Swing20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Crash20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Crash20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Crash20x20), 20, 20);

            await Save(nameof(Crash20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task House20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(House20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(House20x20), 20, 20);

            await Save(nameof(House20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Knight20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Knight20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Knight20x20), 20, 20);

            await Save(nameof(Knight20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Ninja20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Ninja20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Ninja20x20), 20, 20);

            await Save(nameof(Ninja20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanjie issue 113
        public async Task Edible20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Edible20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Edible20x20), 20, 20);

            await Save(nameof(Edible20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Little20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Little20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Little20x20), 20, 20);

            await Save(nameof(Little20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Large20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Large20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Large20x20), 20, 20);

            await Save(nameof(Large20x20), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Man20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Man20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Man20x20), 20, 20);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Man20x20), Rows, Cols, PtsOut);
            
            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 113
        public async Task Girl20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Girl20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Girl20x20), 20, 20);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Girl20x20), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        public async Task Chick20x20()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Chick20x20));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Chick20x20), 20, 20);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);
            
            await Save(nameof(Chick20x20), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Agile20x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Agile20x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Agile20x30), 20, 30);

            await Save(nameof(Agile20x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        private async Task Balanced20x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Balanced20x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Balanced20x30), 20, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Balanced20x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //hanjie issue 65
        public async Task Victory20x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Victory20x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Victory20x30), 20, 30);

            await Save(nameof(Victory20x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }


        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483456723865.png
        public async Task Lion25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Lion25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Lion25x25), 25, 25);

            await Save(nameof(Lion25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483458193986.png
        public async Task Train25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Train25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Train25x25), 25, 25);

            await Save(nameof(Train25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483457646822.png
        public async Task Raccoon25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Raccoon25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Raccoon25x25), 25, 25);

            await Save(nameof(Raccoon25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483458411037.png
        public async Task UFO25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(UFO25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(UFO25x25), 25, 25);

            await Save(nameof(UFO25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-14834577551211.png
        public async Task Santa25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Santa25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Santa25x25), 25, 25);            

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Santa25x25), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483457506480.png
        public async Task Pegasus25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Pegasus25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Pegasus25x25), 25, 25);

            await Save(nameof(Pegasus25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483457506480.png
        public async Task Tea25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Tea25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Tea25x25), 25, 25);

            await Save(nameof(Tea25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        //not solvable!
        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483457175860.png
        public async Task Crab25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Crab25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Crab25x25), 25, 25);            

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Crab25x25), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483458897387.png
        public async Task Zen25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Zen25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Zen25x25), 25, 25);

            await Save(nameof(Zen25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483456906772.png
        public async Task Cock25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Cock25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Cock25x25), 25, 25);

            await Save(nameof(Cock25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483458723500.png
        public async Task Goblin25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Goblin25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Goblin25x25), 25, 25);

            await Save(nameof(Goblin25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483457314992.png
        public async Task Koala25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Koala25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Koala25x25), 25, 25);

            await Save(nameof(Koala25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483458586518.png
        public async Task Acorns25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Acorns25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Acorns25x25), 25, 25);

            await Save(nameof(Acorns25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task LittleDevil25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(LittleDevil25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(LittleDevil25x25), 25, 25);

            await Save(nameof(LittleDevil25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Girl25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Girl25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Girl25x25), 25, 25);

            await Save(nameof(Girl25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Boy25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Boy25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Boy25x25), 25, 25);

            await Save(nameof(Boy25x25), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task OneToday25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(OneToday25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(OneToday25x25), 25, 25);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(OneToday25x25), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Penguins25x25()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Penguins25x25));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Penguins25x25), 25, 25);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Penguins25x25), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Getaway25x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Getaway25x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Getaway25x30), 25, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Getaway25x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        //[Fact]
        //hanjie super - issue 27
        private async Task FourCallingBirds25x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(FourCallingBirds25x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(FourCallingBirds25x35), 25, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(FourCallingBirds25x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483882808999.png
        public async Task Elephant30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Elephant30x30));
            
            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Elephant30x30), 30, 30);

            await Save(nameof(Elephant30x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883389207.png
        public async Task Lovers30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Lovers30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Lovers30x30), 30, 30);

            await Save(nameof(Lovers30x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883599061.png
        public async Task Woman30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Woman30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Woman30x30), 30, 30);

            await Save(nameof(Woman30x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883309075.png
        public async Task Pumpkin30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Pumpkin30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Pumpkin30x30), 30, 30);
            
            await Save(nameof(Pumpkin30x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883705360.png
        public async Task Chaplin30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Chaplin30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();
            
            (Points, Dots) = await Library.GetOutputData(nameof(Chaplin30x30), 30, 30);
            
            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Chaplin30x30), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }
        
        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483882933622.png
        public async Task Cobra30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Cobra30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Cobra30x30), 30, 30);

            await Save(nameof(Cobra30x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883874393.png
        public async Task Pinapple30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Pinapple30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Pinapple30x30), 30, 30);            

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Pinapple30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883065667.png
        public async Task Joker30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(Joker30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Joker30x30), 30, 30);

            await Save(nameof(Joker30x30), Rows, Cols, Points);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883191274.png
        public async Task KingKong30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await  Library.GetSourceData(nameof(KingKong30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(KingKong30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(KingKong30x30), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483883524880.png
        public async Task Spartan30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Spartan30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Spartan30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Spartan30x30), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Posing30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Posing30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Posing30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Posing30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task NoBite30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(NoBite30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(NoBite30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(NoBite30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Draw30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Draw30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Draw30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Draw30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task FancyDress30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(FancyDress30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(FancyDress30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(FancyDress30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Castle30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Castle30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Castle30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Castle30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Deer30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Deer30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Deer30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Deer30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie issue 65
        public async Task Landscape30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Landscape30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Landscape30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Landscape30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie super - issue 27
        public async Task KissHerYuk30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(KissHerYuk30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(KissHerYuk30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(KissHerYuk30x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count
                && !Points.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Green)).Except(PtsOut.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Green))).Any()
                && DtsOut.Count == 304);

            Assert.True(Same);
        }

        [Fact]
        //hanjie super - issue 27
        public async Task Busker30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Busker30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            //(Points, Dots) = await Library.GetOutputData(nameof(Busker30x30), 30, 30);

            //(Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
            //    = Logic.Run(Rows, Cols);

            await Save(nameof(Busker30x30), Rows, Cols, Points);

            //bool Same = (Points.Count == PtsOut.Count
            //    && !Points.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Green)).Except(PtsOut.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Green))).Any()
            //    && DtsOut.Count == 304);

            Assert.True(false);
        }

        [Fact]
        //telegraph newspaper - No 1536
        public async Task Elephant30x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Elephant30x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Elephant30x35), 30, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Elephant30x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //telegraph newspaper - No 1538
        public async Task Daffodils30x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Daffodils30x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Daffodils30x35), 30, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Daffodils30x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //telegraph newspaper - No 1491
        public async Task MiniToMaxi30x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(MiniToMaxi30x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(MiniToMaxi30x35), 30, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(MiniToMaxi30x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //telegraph newspaper - No 1533
        public async Task Poodles30x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Poodles30x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Poodles30x35), 30, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Poodles30x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //telegraph newspaper - No 1511
        public async Task Gorilla30x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Gorilla30x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Gorilla30x35), 30, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Gorilla30x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483958164120.png
        public async Task Sparrow35x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Sparrow35x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Sparrow35x30), 35, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Sparrow35x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483958164120.png
        public async Task Lotus35x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Lotus35x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Lotus35x30), 35, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Lotus35x30), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        //[Fact]
        //hanjie super - issue 27
        private async Task Jester35x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Jester35x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Jester35x35), 35, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Jester35x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count
                && !Points.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Green)).Except(PtsOut.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Green))).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        //[Fact]
        //hanjie super - issue 27
        private async Task RoundTheTree35x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(RoundTheTree35x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(RoundTheTree35x35), 35, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(RoundTheTree35x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie super - issue 27
        public async Task SevenSwansSwimming45x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(SevenSwansSwimming45x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(SevenSwansSwimming45x35), 45, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(SevenSwansSwimming45x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie super - issue 27
        public async Task TwoTurtleDoves50x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(TwoTurtleDoves50x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(TwoTurtleDoves50x35), 50, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(TwoTurtleDoves50x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }
    }
}
