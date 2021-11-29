using Xunit;
using Griddlers.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests;

public partial class LogicTest : LogicTestBase
{

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
    //https://nonogramskatana.files.wordpress.com/2016/12/20161207_224252.png
    public async Task JollyRoger20x15()
    {
        //inputs
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(JollyRoger20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Gun20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Glasses20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Doggie20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Boot20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Jeep20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Goldfish20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Fencer20x15));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(TomaHawk20x20));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Dino20x20));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Butterfly20x20));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Teapot20x20));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Kitty20x20));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Bus20x20));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Peacock20x20));

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
    //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483456723865.png
    public async Task Lion25x25()
    {
        //inputs
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Lion25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Train25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Raccoon25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(UFO25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Santa25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Pegasus25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Tea25x25));

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

    [Fact]
    //https://nonogramskatana.files.wordpress.com/2017/01/wp-1483457175860.png
    public async Task Crab25x25()
    {
        //inputs
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Crab25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Zen25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Cock25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Goblin25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Koala25x25));

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
        (Item[][] Rows, Item[][] Cols) = await Library.GetSourceData(nameof(Acorns25x25));

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

}
