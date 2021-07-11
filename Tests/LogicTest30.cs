using Xunit;
using Griddlers.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public partial class LogicTest : LogicTestBase
    {
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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Elephant30x30));

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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Lovers30x30));

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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Woman30x30));

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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Pumpkin30x30));

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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Chaplin30x30));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Chaplin30x30), 30, 30);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            //await Save(nameof(Chaplin30x30), Rows, Cols, PtsOut);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483882933622.png
        public async Task Cobra30x30()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Cobra30x30));

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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Pinapple30x30));

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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Joker30x30));

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
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(KingKong30x30));

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
                && !Points.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Colour))
                .Except(PtsOut.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Colour))).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        //[Fact]
        //hanjie super - issue 27
        private async Task Busker30x30()
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

            await Save(nameof(Poodles30x35), Rows, Cols, PtsOut);

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
        private async Task Ship35x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Ship35x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(Ship35x35), 35, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(Ship35x35), Rows, Cols, Points);

            bool Same = (Points.Count == PtsOut.Count && !Points.Keys.Except(PtsOut.Keys).Any()
                && Dots.Count == DtsOut.Count && !Dots.Keys.Except(DtsOut.Keys).Any());

            Assert.True(Same);
        }

        [Fact]
        //hanjie super - issue 27
        public async Task PirateShip35x35()
        {
            //inputs
            (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(PirateShip35x35));

            //outputs
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();

            (Points, Dots) = await Library.GetOutputData(nameof(PirateShip35x35), 35, 35);

            (Dictionary<(int, int), Point> PtsOut, Dictionary<(int, int), Point> DtsOut)
                = Logic.Run(Rows, Cols);

            await Save(nameof(PirateShip35x35), Rows, Cols, Points);

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
                && !Points.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Colour))
                .Except(PtsOut.Select(s => (s.Key.Item1, s.Key.Item2, s.Value.Colour))).Any()
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
    }
}
