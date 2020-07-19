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
