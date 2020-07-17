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
using System.Reflection;
using Griddlers.Database;

namespace Griddlers.Controllers
{
    public class HomeController : Controller
    {
        //private GriddlerDbContext db;

        public HomeController(
            //GriddlerDbContext theDb
            )
        {
            //db = theDb;
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
            public byte Xpos { get; set; }
            public byte Ypos { get; set; }
            public byte SortOrder { get; set; }

            public ClientGriddlerPath(GriddlerPath p) 
            {
                Name = p.action_name;
                Group = p.group_num;
                Xpos = p.x_position;
                Ypos = p.y_position;
                SortOrder = p.sort_order;
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

            Point[] pts = Points.Keys.Select(s => new Point(s.Item1, s.Item2, false)).ToArray();

            string json = JsonConvert.SerializeObject(pts);

            var retVal = new
            {
                w = C.Length,
                h = R.Length,
                d = Math.Max(C.Max(m => m.Length), R.Max(m => m.Length)),
                R,
                C,
                pts = Points.Values.ToArray(),
                dots = Dots.Values.ToArray()
            };

            return Json(retVal);
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
