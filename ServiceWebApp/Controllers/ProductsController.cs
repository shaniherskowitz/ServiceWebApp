using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceWebApp.Models;

namespace ServiceWebApp.Controllers
{
    /// <summary>
    /// Creates the messageTypeEnum
    /// </summary>
    public enum MessageTypeEnum : int
    {
        INFO,
        WARNING,
        FAIL
    }

    /// <summary>
    /// Creates the commadnInfo
    /// </summary>
    public class CommandInfo
    {
        public string ID { get; set; }
        public string Details { get; set; }

        public CommandInfo(string iD, string details)
        {
            ID = iD;
            Details = details;
        }
    }

    [RoutePrefix("Products")]
    public class ProductsController : Controller
    {
        Model m = new Model();
        string Output = "Output Directory: ";
        string Source = "Source Name: ";
        string LogName = "Log Name: ";
        string ThumbName = "Thumbnail Name: ";
        IList<string> ListPaths = new List<string>();
        private object lockObj = new object();
        public IList<CommandInfo> ListCommands = new List<CommandInfo>();
        public IList<string> images = new List<string>();
        // GET: Products
        public ActionResult Index()
        {
           
            return View();
        }

        public ActionResult Config()
        {
            SetConfig();
            ViewData["outputDir"] = Output;
            ViewData["source"] = Source;
            ViewData["logName"] = LogName;
            ViewData["ThumbName"] = ThumbName;
            ViewData["ListPaths"] = ListPaths;
           
            return View();
        }

        public ActionResult Logs()
        {
            SetLogs();
            ViewData["ListLogs"] = ListCommands;
            return View();
        }

        public ActionResult Images()
        {
            setImage();
            ViewData["ImageList"] = images;

            return View();
        }


        public void SetConfig()
        {
            IList<string> eachPath = m.GetConfig();
            if (eachPath.Count == 5)
            {
                Output = Output + eachPath[4];
                Source = Source + eachPath[3];
                LogName = LogName + eachPath[2];
                ThumbName = ThumbName + eachPath[1];
                string lPaths = eachPath[0];

                IList<string> each = lPaths.Split(';').Reverse().ToList<string>();
                each.ToList().ForEach(ListPaths.Add);
            }
        }

        public void setImage()
        {
            images = m.GetImage();
        }

        public void SetLogs()
        {
            string info = m.GetLogs();

            IList<string> eachPath = info.Split('*').Reverse().ToList<string>();

            for (int i = 0; i < eachPath.Count; i++)
            {
                IList<string> each = eachPath[i].Split(',').Reverse().ToList<string>();
                if (each.Count == 2)
                {
                    lock (lockObj)
                    {
                        Int32.TryParse(each[1], out int x);
                        if (x == (int)MessageTypeEnum.INFO)
                            ListCommands.Add(new CommandInfo(MessageTypeEnum.INFO.ToString(), each[0]));
                        if (x == (int)MessageTypeEnum.FAIL)
                            ListCommands.Add(new CommandInfo(MessageTypeEnum.FAIL.ToString(), each[0]));
                        if (x == (int)MessageTypeEnum.WARNING)
                            ListCommands.Add(new CommandInfo(MessageTypeEnum.WARNING.ToString(), each[0]));
                    }
                }
            }
        }
        
        public ActionResult RemoveHandler(string path)
        {
            
            bool res = m.RemoveHandler(path);
            var obj = new
            {
                valid = res
            };
            return View();
        }
      
    }
}
