using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Data;
using System.Collections;
using xxoo.Common;

namespace LotterySystem.lib
{
    public class CHOUJIANG
    {
        static string exhID = "6"; // 

        public static string getJIANGPIN()
        {
            double d = 1.0d;

            Hashtable ht = new Hashtable();
            ht = GetLuckDrawNumberByPrize();   // 获取当前 获奖数量

            List<KeyValuePair<long, double>> elements = new List<KeyValuePair<long, double>>();

            //elements.Add(new KeyValuePair<long, double>(0, 0.0005));
            //elements.Add(new KeyValuePair<long, double>(1, 0.0));
            //elements.Add(new KeyValuePair<long, double>(2, 0.0));
            //elements.Add(new KeyValuePair<long, double>(3, 0.001));
            //elements.Add(new KeyValuePair<long, double>(4, 0.9985));

            //SSHT                 得奖数  成本小计
            //现金200元              3      1200    0.2%   0.002
            //豪恩礼品价值119元      15             1.5%   0.015
            //现金50元               20     2000      3%   0.03
            //纪念奖（二合一数据线） 300    2400   95.3%   0.953

            List<PrizeQuota> pqList = PrizeQuota.LoadPrizeQuotas();
            // 一等奖
            PrizeQuota firstPrize = PrizeQuota.GetPrizeQuotasByLevel(0, pqList);
            if (firstPrize.count == 0 || ht.ContainsKey("现金200元") && ht["现金200元"].ToInt() >= firstPrize.count)
            {
                elements.Add(new KeyValuePair<long, double>(0, 0.0));
            }
            else
            {
                elements.Add(new KeyValuePair<long, double>(0, firstPrize.odds));
                d = d - firstPrize.odds;
            }

            // 二等奖
            PrizeQuota secondPrize = PrizeQuota.GetPrizeQuotasByLevel(1, pqList);
            if (secondPrize.count == 0 || ht.ContainsKey("豪恩礼品价值119元") && ht["豪恩礼品价值119元"].ToInt() >= secondPrize.count)
            {
                elements.Add(new KeyValuePair<long, double>(1, 0.0));
            }
            else
            {
                elements.Add(new KeyValuePair<long, double>(1, secondPrize.odds));
                d = d - secondPrize.odds;
            }

            // 三等奖
            PrizeQuota thirdPrize = PrizeQuota.GetPrizeQuotasByLevel(2, pqList);
            if (thirdPrize.count == 0 || ht.ContainsKey("现金50元") && ht["现金50元"].ToInt() >= thirdPrize.count)
            {
                elements.Add(new KeyValuePair<long, double>(2, 0.0));
            }
            else
            {
                elements.Add(new KeyValuePair<long, double>(2, thirdPrize.odds));
                d = d - thirdPrize.odds;
            }
            
            // 四等奖 
            PrizeQuota fourthPrize = PrizeQuota.GetPrizeQuotasByLevel(3, pqList);
            if (fourthPrize.count == 0 || ht.ContainsKey("精美礼品") && ht["精美礼品"].ToInt() >= fourthPrize.count)
            {
                elements.Add(new KeyValuePair<long, double>(3, 0.0));
            }
            else
            {
                elements.Add(new KeyValuePair<long, double>(3, fourthPrize.odds)); //4等奖几率为减去其他
                d = d - fourthPrize.odds;
            }
            elements.Add(new KeyValuePair<long, double>(4, d)); // 谢谢惠顾 所有奖项抽完了
            if (d == 1) { // 所有奖项抽完了
                return "谢谢参与";
            }
            //elements.Add(new KeyValuePair<long, double>(1, 0.0));
            //elements.Add(new KeyValuePair<long, double>(2, 0.0));
            //elements.Add(new KeyValuePair<long, double>(5, d));

            Dictionary<long, string> prize = new Dictionary<long, string>();
            prize.Add(0, @"现金200元");
            prize.Add(1, @"豪恩礼品价值119元");
            prize.Add(2, @"现金50元");
            prize.Add(3, @"精美礼品");
            prize.Add(4, @"谢谢参与");

            var max = elements.Where(p => p.Value == 1).ToList();
            if (max.Count != 0) { // 某个奖品概率为1，直接抽取
                return prize[max[0].Key];
            }

            //求出概率基数  
            long basicNumber = 0;

            double[] array = new double[elements.Count];

            int m = 0;

            foreach (KeyValuePair<long, double> item in elements)
            {
                array[m] = item.Value;
                m++;
            }

            basicNumber = ToolMethods.GetBaseNumber(array);

            //判断设置的概率  
            double allRate = 0;

            foreach (var item in elements)
            {
                allRate += item.Value;
            }

            if (allRate != 1)
            {
                return "";
            }

            //抽奖  
            Random random = new Random();

            long selectedElement = 0;
            long diceRoll = ToolMethods.GetRandomNumber(random, 1, basicNumber);
            long cumulative = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                cumulative += (long)(elements[i].Value * basicNumber);
                if (diceRoll <= cumulative)
                {
                    selectedElement = elements[i].Key;
                    break;
                }
            }
            return prize[selectedElement];
        }

        /// <summary>
        /// 获取奖品发放数量
        /// </summary>
        /// <returns></returns>
        public static int InsertLuckDraw(string LdPrizeName, string LdOpenID)
        {
            string sql = string.Format("INSERT INTO LuckDraw(LdPrizeName,LdOpenID,LdCreateTime,exhID) VALUES('{0}','{1}',datetime(),{2})", LdPrizeName, LdOpenID, exhID);
            return SQLitehelper.ExecuteNonQuery(sql);
        }
        
        /// <summary>
        /// 获取奖品发放数量
        /// </summary>
        /// <returns></returns>
        public static string getLdPrizeNameByOpenID(string openid)
        {
            return getLdPrizeNameByOpenID(openid, exhID);
        }
        /// <summary>
        /// 获取奖品发放数量
        /// </summary>
        /// <returns></returns>
        public static string getLdPrizeNameByOpenID(string openid, string exhID)
        {
            string sql = string.Format("SELECT LdPrizeName FROM LuckDraw WHERE LdOpenID='{0}' and exhid='{1}'", openid, exhID);
            return SQLitehelper.ExecuteScalar(sql) + "";
        }

        public static Hashtable GetLuckDrawNumberByPrize()
        {
            Hashtable ht = new Hashtable();
            Dictionary<String, String> PrizeList = new Dictionary<String, String>();

            string sql = string.Format("SELECT LdPrizeName,COUNT(LdID) AS SL FROM LuckDraw where  exhid={0} GROUP BY LdPrizeName", exhID);

            DataTable dt = new DataTable();

            dt = SQLitehelper.ExecuteTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                ht.Add(dr["LdPrizeName"].ToString(), dr["SL"].ToString());
            }
            return ht;
        }
    }
}