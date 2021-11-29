using Griddlers.Library;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests;

public class ImageTest
{
    //[Fact]
    //private async Task Pinapple30x30()
    //{
    //    //inputs
    //    (int?[][] rows, int?[][] cols) = await Library.Pinapple30x30Source();

    //    //outputs
    //    (Dictionary<(int, int), int> rowsOut, Dictionary<(int, int), int> colsOut)
    //        = ImageLogic.LoadFile(nameof(Pinapple30x30));

    //    //same
    //    bool same = true;

    //    for (int i = 0; i < rows.GetLength(0) - 1; i++)
    //    {
    //        for (int c = 0; c < rows[i].Length - 1; c++)
    //        {
    //            if (rowsOut.TryGetValue((i, c), out int value) && value != rows[i][c].Value)
    //            {
    //                same = false;
    //                break;
    //            }
    //        }
    //    }

    //    if (same)
    //    {
    //        for (int i = 0; i < cols.GetLength(0) - 1; i++)
    //        {
    //            for (int c = 0; c < cols[i].Length - 1; c++)
    //            {
    //                if (colsOut.TryGetValue((i, c), out int value) && value != cols[i][c].Value)
    //                {
    //                    same = false;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    Assert.True(same);
    //}
}
