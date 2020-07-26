using Griddlers.Library;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Griddlers.Hubs
{
    public interface IGriddlerHub
    {
        Task SendPoint(Point pt);
    }

    public class GriddlerHub : Hub<IGriddlerHub>
    {
        
    }
}
