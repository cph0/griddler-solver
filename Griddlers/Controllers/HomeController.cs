using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Griddlers.Library;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Griddlers.Database;
using Microsoft.AspNetCore.SignalR;
using Griddlers.Hubs;
using System.Text.RegularExpressions;

namespace Griddlers.Controllers
{
    public class HomeController : Controller
    {
        //private GriddlerDbContext db;
        private IHubContext<GriddlerHub, IGriddlerHub> HubContext;

        public HomeController(
            //GriddlerDbContext theDb,
            IHubContext<GriddlerHub,IGriddlerHub> hubContext
            )
        {
            //db = theDb;
            HubContext = hubContext;
        }

        private class Data
        {
            public int?[][] rows { get; set; } = new int?[][] { };
            public int?[][] columns { get; set; } = new int?[][] { };
        }

        private class SG
        {
            public string sG { get; set; } = "";
        }

        private class ImageData
        {
            public IFormFile? file { get; set; }
        }

        public class ClientGriddler
        {
            public int id { get; set; }
            public string name { get; set; }

            public ClientGriddler(string n)
            {
                name = n;
            }

            public ClientGriddler(Griddler g) 
            {
                id = g.griddler_id;
                name = g.griddler_name;
            }
        }

        public class ClientGriddlerPathGroup
        {
            public string Name { get; set; }
            public short Group { get; set; }
            public ClientGriddlerPath[] Items { get; set; }
            public bool Open { get; set; }

            public ClientGriddlerPathGroup(ClientGriddlerPath p, ClientGriddlerPath[] i) 
            {
                Name = p.Name;
                Group = p.Group;
                Items = i;
            }
        }

        public class ClientGriddlerPath
        {
            public string Name { get; set; }
            public short Group { get; set; }

            [JsonProperty("xPos")]
            public byte Xpos { get; set; }

            [JsonProperty("yPos")]
            public byte Ypos { get; set; }

            public ClientGriddlerPath(Point point) 
            {
                Name = string.Empty;

                string? EnumValue = Enum.GetName(typeof(GriddlerPath.Action), point.Action);
                if (EnumValue != null)
                    Name = EnumValue;

                Group = point.Grp;
                Xpos = (byte)point.Xpos;
                Ypos = (byte)point.Ypos;
            }

            public ClientGriddlerPath(GriddlerPath p) 
            {
                Name = p.action_name;
                Group = p.group_num;
                Xpos = p.x_position;
                Ypos = p.y_position;
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadImage(IFormFile file)
        {
            (string jsonRows, string jsonCols) = ImageLogic.UploadFile(file);

            return Json("OK");
        }

        //public JsonResult GetData([FromBody]JToken dt)
        //{
        //    int width, height;
        //    Data data = JsonConvert.DeserializeObject<Data>(dt.ToString(Formatting.None));
        //    width = data.columns.Length;
        //    height = data.rows.Length;

        //    (Dictionary<(int, int), Point> points, Dictionary<(int, int), Point> dots)
        //        = Logic.Run(width, height, data.rows.Select(s => new Logic.Item(s)), data.columns);

        //    var retVal = new
        //    {
        //        pts = points.Values.ToArray(),
        //        dots = dots.Values.ToArray()
        //    };

        //    return Json(retVal);
        //}

        public async Task<JsonResult> GetGriddler([FromBody]JToken dt)
        {
            SG Data = JsonConvert.DeserializeObject<SG>(dt.ToString(Formatting.None));
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();
            (Item[][] R, Item[][] C) = (new Item[][] { }, new Item[][] { });
            
            //inputs
            (R, C) = await Library.Library.GetSourceData(Data.sG);

            //outputs
            (Points, Dots) = Logic.Run(R, C);

            List<Point> Ordered = Points.Values.ToList();
            Ordered.AddRange(Dots.Values);
            ClientGriddlerPathGroup[] Groups = (from p in Ordered
                                                group p by p.Grp into grp
                                                select new
                                                {
                                                    key = new ClientGriddlerPath(grp.First()),
                                                    items = grp.Select(s => new ClientGriddlerPath(s)).ToArray()
                                                }).Select(s => new ClientGriddlerPathGroup(s.key, s.items))
                                                .ToArray();

            Point[] pts = Points.Keys.Select(s => new Point(false, s.Item1, s.Item2, false)).ToArray();

            string json = JsonConvert.SerializeObject(pts);

            var retVal = new
            {
                w = C.Length,
                h = R.Length,
                d = Math.Max(C.Max(m => m.Length), R.Max(m => m.Length)),
                R,
                C,
                pts = Points.Values.ToArray(),
                dots = Dots.Values.ToArray(),
                paths = Groups
            };

            return Json(retVal);
        }

        public async Task<JsonResult> StreamGriddler([FromBody] JToken dt)
        {
            SG Data = JsonConvert.DeserializeObject<SG>(dt.ToString(Formatting.None));
            (Item[][] R, Item[][] C) = (new Item[][] { }, new Item[][] { });

            //inputs
            (R, C) = await Library.Library.GetSourceData(Data.sG);

            //outputs
            //Logic.RunAsStream(R, C);
            Logic.RunAsStream(R, C, async (Point Pt) =>
            {
                //signalr
                await HubContext.Clients.All.SendPoint(Pt);
            });

            var RetVal = new
            {
                w = C.Length,
                h = R.Length,
                d = Math.Max(C.Max(m => m.Length), R.Max(m => m.Length)),
                R,
                C
            };

            return Json(RetVal);
        }

        public JsonResult StreamGriddlerPlay()
        {
            Logic.Play(async (Point Pt) => {
                await HubContext.Clients.All.SendPoint(Pt);
            });
            return Json(true);
        }

        public JsonResult StreamGriddlerStop()
        {
            Logic.Stop();
            return Json(true);
        }

        public JsonResult StreamGriddlerNext()
        {
            Point? Pt = Logic.Next();

            var RetVal = new { 
                pt = Pt
            };

            return Json(RetVal);
        }

        public JsonResult StreamGriddlerPrevious()
        {
            Point? Pt = Logic.Previous();

            var RetVal = new
            {
                pt = Pt
            };

            return Json(RetVal);
        }

        public async Task<JsonResult> CreateTreeTest()
        {
            (Item[][] R, Item[][] C) = (new Item[][] { }, new Item[][] { });

            //inputs
            (R, C) = await Library.Library.GetSourceData("Bird10x10");

            Tree Tree = Logic.CreateTree(R, C);

            return Json(Tree);
        }

        public async Task<JsonResult> GetTreeTest([FromBody] JToken dt)
        {
            SG Data = JsonConvert.DeserializeObject<SG>(dt.ToString(Formatting.None));
            (Item[][] R, Item[][] C) = (new Item[][] { }, new Item[][] { });

            //inputs
            (R, C) = await Library.Library.GetSourceData(Data.sG);

            Tree Tree = Logic.CreateTree2(R, C);

            return Json(Tree.Root);
        }


        public async Task<JsonResult> GetRequiredActionsTest()
        {
            (Item[][] R, Item[][] C) = (new Item[][] { }, new Item[][] { });

            //inputs
            (R, C) = await Library.Library.GetSourceData("Bird10x10");

            Regex Regex = new Regex("([0-9]+)x([0-9]+)");
            Match Match = Regex.Match("Bird10x10");
            int Width = int.Parse(Match.Groups[1].Value);
            int Height = int.Parse(Match.Groups[2].Value);
            var Output = await Library.Library.GetOutputData("Bird10x10", Width, Height);

            IReadOnlyList<string> Actions = Logic.GetRequiredActions(R, C, Output.Item1, Output.Item2);

            return Json(Actions);
        }

        public async Task<JsonResult> IsTrueGriddlerTest(string g)
        {
            (Item[][] R, Item[][] C) = (new Item[][] { }, new Item[][] { });

            //inputs
            (R, C) = await Library.Library.GetSourceData(g);

            bool TrueGriddler = Logic.IsTrueGriddler(R, C);

            return Json(TrueGriddler);
        }


        public class ChartData
        {
            public string Name { get; set; }
            public decimal Used { get; set; }
            public decimal Required { get; set; }

            public ChartData(string name, decimal used = 0, decimal req = 0) 
            {
                Name = name;
                Used = used;
                Required = req;
            }
        }

        public async Task<JsonResult> GetActionsChart([FromBody] JToken dt)
        {
            //SG Data = JsonConvert.DeserializeObject<SG>(dt.ToString(Formatting.None));
            (Item[][] R, Item[][] C) = (new Item[][] { }, new Item[][] { });

            string[] Griddlers = Library.Library.ListGriddlers();
            List<string> UsedActions = new List<string>(Griddlers.Length * 30);
            List<string> RequiredActions = new List<string>(Griddlers.Length * 30);
            
            int Count = 0;
            HashSet<string> Exclude = (new string[] { "Acorns25x25", "Agile20x30", "Balanced20x30","Beg20x20", "Boy25x25"}).ToHashSet();

            foreach (string Griddler in Griddlers.Where(w => !Exclude.Contains(w)))
            {
                (R, C) = await Library.Library.GetSourceData(Griddler);

                if (Count < 8)
                {
                    IReadOnlyList<string> UseActions = Logic.GetUsedActions(R, C);
                    UsedActions.AddRange(UseActions);                
                }

                if (Count < 3)
                {
                    Regex Regex = new Regex("([0-9]+)x([0-9]+)");
                    Match Match = Regex.Match(Griddler);
                    int Width = int.Parse(Match.Groups[1].Value);
                    int Height = int.Parse(Match.Groups[2].Value);
                    var Output = await Library.Library.GetOutputData(Griddler, Width, Height);

                    IReadOnlyList<string> ReqActions = Logic.GetRequiredActions(R, C, Output.Item1, Output.Item2);
                    RequiredActions.AddRange(ReqActions);
                }

                if (Count > 6)
                    break;

                Count++;
            }

            Dictionary<string, ChartData> ChartData = new Dictionary<string, ChartData>();

            IEnumerable<(string, decimal)> A = (from a in UsedActions
                                                group a by a into grp
                                                select (grp.Key, (decimal)grp.Count()));

            ChartData = A.ToDictionary(k => k.Item1, k => new ChartData(k.Item1, k.Item2));

            IEnumerable<(string, decimal)> B = (from a in RequiredActions
                                                group a by a into grp
                                                select (grp.Key, (decimal)grp.Count()));

            foreach ((string, decimal) Item in B)
            {
                if (ChartData.ContainsKey(Item.Item1))
                    ChartData[Item.Item1].Required = Item.Item2;
                else
                    ChartData.Add(Item.Item1, new ChartData(Item.Item1, req: Item.Item2));
            }

            return Json(ChartData.Values.ToArray());
        }

        [HttpGet]
        public JsonResult ListGriddlers()
        {
            string[] Griddlers = Library.Library.ListGriddlers();

            ClientGriddler[] ClientGriddlers = Griddlers.Select(s => new ClientGriddler(s))
                                                    .ToArray();

            return Json(ClientGriddlers);
        }

        //[HttpGet]
        //public async Task<JsonResult> ListGriddlersDb()
        //{
        //    Griddler[] Griddlers = await db.ListGriddlers();

        //    ClientGriddler[] ClientGriddlers = Griddlers.Select(s=>new ClientGriddler(s))
        //                                            .ToArray();

        //    return Json(ClientGriddlers);
        //}

        //[HttpGet]
        //public async Task<JsonResult> GetGriddlerDb(short id)
        //{
        //    Dictionary<(int, int), Point> points = new Dictionary<(int, int), Point>();
        //    Dictionary<(int, int), Point> dots = new Dictionary<(int, int), Point>();
            
        //    Griddler Griddler = await db.GetGriddler(id, "");

        //    (points, dots) = Logic.Run(Griddler.Rows, Griddler.Cols);

        //    List<Point> Ordered = points.Values.ToList();
        //    Ordered.AddRange(dots.Values);

        //    if(Griddler.Paths.Length == 0 && false)
        //        await db.SaveGriddlerPath(id, Ordered.OrderBy(o => o.Time).ToArray());

        //    ClientGriddlerPathGroup[] Groups = (from p in Griddler.Paths
        //                                        group p by p.group_num into grp
        //                                        select new
        //                                        {
        //                                            key = new ClientGriddlerPath(grp.First()),
        //                                            items = grp.Select(s => new ClientGriddlerPath(s)).ToArray()
        //                                        }).Select(s => new ClientGriddlerPathGroup(s.key, s.items))
        //                                        .ToArray();

        //    Point[] Pts = points.Keys.Select(s => new Point(s.Item1, s.Item2, false)).ToArray();

        //    string Jsn = JsonConvert.SerializeObject(Pts);

        //    var RetVal = new
        //    {
        //        w = Griddler.width,
        //        h = Griddler.height,
        //        d = Math.Max(Griddler.Cols.Max(m => m.Length), Griddler.Rows.Max(m => m.Length)),
        //        r = Griddler.Rows,
        //        c = Griddler.Cols,
        //        pts = points.Values.ToArray(),
        //        dots = dots.Values.ToArray(),
        //        paths = Groups
        //    };

        //    return Json(RetVal);
        //}



        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
