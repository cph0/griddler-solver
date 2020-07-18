using Xunit;
using Griddlers.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class LogicTest15 : LogicTest
    {
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

    }
}
