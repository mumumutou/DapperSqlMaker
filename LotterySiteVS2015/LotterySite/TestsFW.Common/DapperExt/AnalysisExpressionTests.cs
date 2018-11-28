using NUnit.Framework;
using FW.Common.DapperExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using FW.Model;
using Dapper;

namespace FW.Common.DapperExt.Tests
{
    [TestFixture()]
    public class AnalysisExpressionTests
    {
        private static void WriteJson(object test2)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(test2);
            Console.WriteLine(str);
        }

        #region Select

        #endregion

        #region Add

        #endregion

        #region Update

        #endregion

        #region Delete

        #endregion


        [Test]
        public void VisitExpressionTest1()
        {
            // 1
            Expression<Func<LockPers, bool>> expression = t => SM.In(t.Name, new string[] { "马", "码" })
               && t.Name == "农码一生" && t.Prompt == "男" || t.Name.Contains("11");
            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            Console.WriteLine(sql);

            var example = " Name in ('马','码') and Name = @Name0  and Prompt = @Prompt1  or Name like @Name2 ";
            Console.WriteLine(example);

            var sqlstr = " Name in ('马','码') && Name == '农码一生' && Prompt == '男' || Name like '11' ";
            Console.WriteLine(sqlstr);

        }
        [Test]
        public void VisitExpressionTest()
        {
            var arrEditCount = new int[5] { 22, 2, 3, 5, 1 };  // 
            //2 
            Expression<Func<LockPers, bool>> expression = w =>
                (w.Prompt.Contains("%hou%") || w.Prompt.Contains("%15%") || w.Prompt.Contains("%137%") || w.Prompt.Contains("%138%") || w.Prompt.Contains("%139%"))
            //    && ( w.IsDel == "1" && ( w.IsDel == "1" || w.IsDel != "0"  ) || SM.In(w.IsDel, arrEditCount)  )
            //    && ( w.Name.Contains("%蛋蛋%") || w.Name.Contains("%zfb%") )
            ;
            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sql, ref spars);

            foreach (var name in spars.ParameterNames)
            {
                Console.WriteLine(name);
                WriteJson(spars.Get<object>(name));
            }
            Console.WriteLine(sql);

            var example = " ";
            Console.WriteLine(example);

            // var datearr = new DateTime[2] { new DateTime(18, 11, 28), new DateTime(18, 11, 22) };
            //      && SM.In(w.UpdateTime, datearr)
            //      //and EditCount in ('18/11/28', '18/11/22')
            // Assert.Fail(); //断言

            //var objs = LockDapperUtil<LockPers>.Get(expression);
            //WriteJson(objs);

        }

        /// <summary>
        /// // C#解析表达时 有括号的括号里肯定有or () 并且括号外有一边是 and
        /// // sql串联or会补上内嵌括号 不影响  调试时c#串联or的表达式也会自动补上内嵌括号 
        /// </summary>
        [Test]
        public void 括号优先级()
        { 
            // 1
            Expression<Func<LockPers, bool>> expression = w =>
                (w.Id == "1" && (w.Name == "2" || w.Prompt == "3") && w.Content == "4" || w.Id == "5")
                && ( w.IsDel != false && ( w.IsDel == false || w.IsDel == true) );
            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            Console.WriteLine(sql);
            var example = "( w.Id == '1' && (w.Name == '2' || w.Prompt == '3') && w.Content == '4' || w.Id == '5')  && w.IsDel != '0'";
            Console.WriteLine(example);

            //// 2
            //sql = null;
            //spars = null;
            //expression = w =>
            //    ( (w.Id == "1" || w.Name == "2" ) && (w.Prompt == "3" || w.Content == "4" ) || w.Id == "5")
            //    && w.IsDel != "0";
            //AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            //Console.WriteLine(sql);

            //// 3
            //sql = null;
            //spars = null;

            //expression = w =>
            //    w.IsDel != "0" && w.Id == "1" || w.Name == "2" || w.Prompt == "3" || w.Content == "4" || w.Id == "5";
            //AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            //Console.WriteLine(sql);

            //// 4
            //sql = null;
            //spars = null;
            //expression = w =>
            //    w.IsDel != "0" && w.Id == "1" && w.Name == "2" && (w.Prompt == "3" || w.Id == "5") && w.Content == "4";
            //AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            //Console.WriteLine(sql);

            foreach (var name in spars.ParameterNames)
            {
                Console.WriteLine(name);
                WriteJson(spars.Get<object>(name));
            }



        }

        [Test]
        public void 括号优先级连接数据库测试()
        {

            Expression<Func<LockPers, bool>> expression = w =>
                 w.IsDel != true
            ;
            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            Console.WriteLine(sql);

            var example = "IsDel != 1 and  (name like '%蛋蛋%' or name like '%大疆%' or Prompt like '%_%') and EditCount in (1, 2, 3) and InsertTime > '2017-11-01 00:00:00' ";
            Console.WriteLine(example);

        }

        public void 不同查询条件添加不通的where() {

        }

        public void 子查询()
        {

        }

        //将IsDel=False修改成0


        [Test]
        public void DynamicQuery()
        {

            // 1. dynamic  
            DynamicParameters spars2 = new DynamicParameters();
            spars2.Add("isDel", 1);
            spars2.Add("name", "%蛋蛋%");
            DynamicParameters spars = new DynamicParameters();
            spars.Add("EditCount", new int[4] { 11, 2, 3, 5 });
            spars.AddDynamicParams(spars2);

            object test2 = LockDapperUtil.Query("SELECT * FROM LockPers where isDel != @isDel and name like @name and EditCount in @EditCount  order by Name ", spars);
            WriteJson(test2);


            // 2 fomarrt
            //var name = "xxxo";
            //var key = new { Name = "test" };
            //string sql = $"select * from {name} where {key.Name} = @id";


        }

        

        [Test]
        public void Get()
        {
            // 1. GetId 
            //var old1 = LockDapperUtil.Get<LockPers>("7fc02473-fee2-40da-a048-4398d9b052fd");
            //WriteJson(old1); 
            //var old2 = LockDapperUtil.Get<LockPers_>("7fc02473-fee2-40da-a048-4398d9b052fd");
            //WriteJson(old2);


            // 2. 
            var objs3 = LockDapperUtil<LockPers>.Get(w => w.IsDel == true && w.Name.Contains("%蛋蛋%")
                && SM.In(w.EditCount, new int[4] { 11, 2, 3, 5 })
                );
            WriteJson(objs3);


            //var a = new object[] {  1 , "" , 222  };

            //LockDapperUtil<LockPers>.Get(
            //    f => {
            //           object o = new object[5] { f.Id, f.IsDel, f.Name, f.Prompt, f.UpdateTime };
            //    }

            //    ,w =>   w.IsDel == "1" && w.Name.Contains("%蛋蛋%") 
            //          ///&& SM.In(w.EditCount, new int[4] { 11, 2, 3, 5 })
            //    );

        }

        public void Update() {

            // 1. Update (set和where里不能有相同字段)
            LockPers pset = new LockPers(true);
            pset.Name = "修改95 只修改Name字段";
            LockPers pwhere = new LockPers(true);
            pwhere.Content = "7fa867c5b404547797614abe57341844";
            //var efrwostest2 = LockDapperUtil.Update<LockPers>(pset, pwhere);

            // 2.Update
            var efrowsupdate2 = LockDapperUtil<LockPers>.Update(
            set =>
            {
                set.Name = "修改95 修改Name和Content字段";
                set.Prompt = "BMWWWWWWWWWWWWWWW";
            }
            , where => where.Content == "xxxxxxxoooooooo");


            // 3.Update
            // where 字段参数名 会 和 set 字段参数名重复, set字段名统一加_
            var efrowsupdate3 = LockDapperUtil<LockPers>.Update(
            set =>
            {
                set.Name = "修改95 修改Name和Content字段";
                set.Prompt = "BMWWWWWWWWWWWWWWW";
            }
            , 
            where => SM.In(where.Name, new string[] { "马", "码" })
                     && where.Name == "农码一生" && where.Prompt == "男" || where.Name.Contains("11"));

        }

    }
}