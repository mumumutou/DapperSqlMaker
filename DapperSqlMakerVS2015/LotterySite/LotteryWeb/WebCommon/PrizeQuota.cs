using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace LotterySystem.lib
{
    public class PrizeQuota
    {
        //level="0" count="3"   total="1200" odds="0.002" 
        
        /// <summary>
        /// 奖品级别 0(一等奖) -> 3(三等奖)
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 奖品数量 
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 总计
        /// </summary>
        public string total { get; set; }

        /// <summary>
        /// 奖品几率
        /// </summary>
        public double odds { get; set; }

        /// <summary>
        /// 加载所有奖品 配额信息
        /// </summary>
        /// <returns></returns>
        public static List<PrizeQuota> LoadPrizeQuotas()
        {
            if (HttpContext.Current.Cache["PrizeQuota_Info"] != null)
            {
                return HttpContext.Current.Cache["PrizeQuota_Info"] as List<PrizeQuota>;
            }

            //读取污染颜色值指标
            List<PrizeQuota> pqList = new List<PrizeQuota>();
            string prizeQuotaPath = HttpContext.Current.Server.MapPath("~/PrizeQuota.xml");
            if (!File.Exists(prizeQuotaPath)) return null;

            XmlDocument xmlDoc = new XmlDocument(); // System.Xml.dll
            xmlDoc.Load(prizeQuotaPath);
            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                if (node.Name != "PrizeQuotas") continue;
                foreach (XmlNode metNode in node.ChildNodes)
                {
                    if (metNode.Name != "PrizeQuota") continue;
                    PrizeQuota pqModel = new PrizeQuota();
                    pqModel.level = int.Parse(metNode.Attributes["level"].Value); // 级别
                    pqModel.count = int.Parse(metNode.Attributes["count"].Value); // 数量
                    //pqModel.total = metNode.Attributes["total"].Value; // 总价
                    pqModel.odds = double.Parse(metNode.Attributes["odds"].Value); // 概率

                    pqList.Add(pqModel);
                }// pqList
            }
            HttpContext.Current.Cache["PrizeQuota_Info"] = pqList;
            return pqList;
        }

        /// <summary>
        /// 获取指定级别奖品 配额信息
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static PrizeQuota GetPrizeQuotasByLevel(int level,List<PrizeQuota> list) 
        {
            return list.Where(p => p.level == level).ToList<PrizeQuota>()[0];
        }

    }
}