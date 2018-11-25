using FW.Common.DapperExt;
using FW.Model;
using LotterySystem.lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xxoo.Common;

namespace LotteryWeb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult zhuye()
        {
            return View();
        }

        public ActionResult folio()
        {

            return View();
        }

        public ActionResult Index()
        {
          

            string LdPrizeName = "";
            string Level = "";
            string OpenID = Session.SessionID;


            LdPrizeName = CHOUJIANG.getLdPrizeNameByOpenID(OpenID);
            if (LdPrizeName == null || LdPrizeName == "")
            {
                LdPrizeName = CHOUJIANG.getJIANGPIN();
                //CHOUJIANG.InsertLuckDraw(LdPrizeName, OpenID);

                DapperUtil.Insert<LuckDraw>(new LuckDraw() { LdPrizeName = LdPrizeName, LdOpenID = OpenID, ExhID = 6 });
            }

            //SIBT		            得奖数   成本小计
            //现金200元               3      1200      0.2%    0.002
            //豪恩礼品价值119元       15               1.5%    0.015
            //现金50元                20     2000      3%      0.03
            //纪念奖(二合一数据线)    300    2400      95.3%   0.953
            if (LdPrizeName == "现金200元")
            {
                Level = "特等奖";
                LdPrizeName = "现金<span>200</span>元";
            }
            else if (LdPrizeName == "豪恩礼品价值119元")
            {
                Level = "二等奖";
                LdPrizeName = "豪恩礼品价值119元";
            }
            else if (LdPrizeName == "现金50元")
            {
                Level = "三等奖";
                LdPrizeName = "现金50元";
            }
            else if (LdPrizeName == "精美礼品")
            {
                Level = "四等奖";
                LdPrizeName = "精美礼品";
            }
            else
            {
                Level = "谢谢参与";
            }


            ViewBag.Level = Level;
            ViewBag.LdPrizeName = LdPrizeName;

            return View();
        }

        public ActionResult Hua()
        {
            return View();
        }

        public ActionResult MapPath()
        {

            var a = System.IO.File.Exists("f:/usr/LocalUser/bxw2713720271/db/cater.db");
            var b = System.IO.File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "/Views/Home/Index.cshtml");

            return Content(a + Server.MapPath("~/") + b);
        }


        public ActionResult Reader()
        {
            var msg = "";
            try
            {
                SQLiteConnection conn = new SQLiteConnection("Data Source=f:/usr/LocalUser/bxw2713720271/db/cater.db;");
                SQLiteCommand cmd = new SQLiteCommand("SELECT count(LdPrizeName) FROM LuckDraw", conn);
                conn.Open();
                var r = cmd.ExecuteScalar();
                msg = r.ToString();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            //var a = System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "/Views/Home/Index.cshtml");

            return Content(msg);
        }

        public ActionResult LockReader()
        {

            string sql = string.Format("SELECT * FROM LockPers");
            DataTable dt = LockSQLitehelper.ExecuteTable(sql);
            var str =  Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            return Content(str);
        }
         
    }
}
