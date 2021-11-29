using Xunit;
using Griddlers.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests;

public partial class LogicTest : LogicTestBase
{
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

}
