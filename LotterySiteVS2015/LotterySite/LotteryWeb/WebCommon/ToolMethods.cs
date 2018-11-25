using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LotterySystem.lib
{
    public static class ToolMethods
    {
        /// <summary>  
        /// 获取概率的基数  
        /// </summary>  
        /// <param name="array"></param>  
        /// <returns></returns>  
        public static long GetBaseNumber(double[] array)
        {
            long result = 0;
            try
            {
                if (array == null || array.Length == 0)
                {
                    return result;
                }

                string targetNumber = string.Empty;

                foreach (double item in array)
                {
                    string temp = item.ToString();


                    if (!temp.Contains('.'))
                    {
                        continue;
                    }


                    temp = temp.Substring(temp.IndexOf('.')).Replace(".", "");


                    if (targetNumber.Length < temp.Length)
                    {
                        targetNumber = temp;
                    }
                }


                if (!string.IsNullOrEmpty(targetNumber))
                {
                    int ep = targetNumber.Length;


                    result = (long)Math.Pow(10, ep);
                }
            }
            catch { }


            return result;
        }


        /// <summary>  
        /// 获取随机数  
        /// </summary>  
        /// <param name="random"></param>  
        /// <param name="min"></param>  
        /// <param name="max"></param>  
        /// <returns></returns>  
        public static long GetRandomNumber(this Random random, long min, long max)
        {
            byte[] minArr = BitConverter.GetBytes(min);


            int hMin = BitConverter.ToInt32(minArr, 4);


            int lMin = BitConverter.ToInt32(new byte[] { minArr[0], minArr[1], minArr[2], minArr[3] }, 0);


            byte[] maxArr = BitConverter.GetBytes(max);


            int hMax = BitConverter.ToInt32(maxArr, 4);


            int lMax = BitConverter.ToInt32(new byte[] { maxArr[0], maxArr[1], maxArr[2], maxArr[3] }, 0);


            if (random == null)
            {
                random = new Random();
            }


            int h = random.Next(hMin, hMax);


            int l = 0;


            if (h == hMin)
            {
                l = random.Next(Math.Min(lMin, lMax), Math.Max(lMin, lMax));
            }
            else
            {
                l = random.Next(0, Int32.MaxValue);
            }


            byte[] lArr = BitConverter.GetBytes(l);


            byte[] hArr = BitConverter.GetBytes(h);


            byte[] result = new byte[8];


            for (int i = 0; i < lArr.Length; i++)
            {
                result[i] = lArr[i];
                result[i + 4] = hArr[i];
            }


            return BitConverter.ToInt64(result, 0);
        }
    }
}