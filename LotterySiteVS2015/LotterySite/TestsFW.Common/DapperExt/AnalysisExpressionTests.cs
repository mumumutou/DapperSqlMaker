using NUnit.Framework;
using FW.Common.DapperExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using FW.Model;
using Dapper;
using System.Data.SQLite;

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

        #region where条件

        #endregion

        // 字段赋值 和表达式 两套分开
        // 根据不同条件拼接 where

        public void GetPage(int page, int rows) {

            int a = (page - 1) * rows;
            int b = page * rows;

            //      int records; // 总条数
            //      List<GeneralMessage> generalmessageList = generalmessageBll.LoadPageEntities(
            //          where,
            //          p => p.GmgID,
            //          page, rows, out records, true
            //          ).ToList<GeneralMessage>();
            //     
            //      int total = (int)Math.Ceiling(records * 1.0 / rows); // 总页数
            //      var o = new { records = records, total = total, rows = generalmessageList };
            //      string s = JsonHelper.DateSerializeObject(o);
            //      return Content(s);
        }

        [Test]
        public void Get测试() {

            

            var arrEditCount = new int[6] { 22, 2, 3, 5, 1, 11 };  // 
            DateTime? startDate2 = new DateTime(2018, 3, 17);
            DateTime? endDate2 = ((DateTime)startDate2).AddDays(1);
            DateTime? startDate = new DateTime(2018, 10, 17);
            DateTime? endDate = ((DateTime)startDate).AddDays(1);

            var objs = LockDapperUtilTest<LockPers_>.New.Get(w =>
                SM.In(w.EditCount, arrEditCount)
                && (w.InsertTime >= startDate && w.InsertTime < endDate || w.InsertTime >= startDate2 && w.InsertTime < endDate2)
                && w.IsDel == false
            );

            WriteJson(objs);
        }
        [Test]
        public void Get测试2() {

            var arrEditCount = new int[6] { 22, 2, 3, 5, 1, 11 };  // 
            DateTime? startDate2 = new DateTime(2018, 3, 17);
            DateTime? endDate2 = ((DateTime)startDate2).AddDays(1);
            DateTime? startDate = new DateTime(2018, 10, 17);
            DateTime? endDate = ((DateTime)startDate).AddDays(1);

            Expression<Func<LockPers_, bool>> where = PredicateBuilder.WhereStart<LockPers_>();
            where = where.And(w => SM.In(w.EditCount, arrEditCount));
            where = where.And(w => (w.InsertTime >= startDate && w.InsertTime < endDate || w.InsertTime >= startDate2 && w.InsertTime < endDate2));
            where = where.And(w => w.IsDel == false);

            var order = LockPers.Field_InsertTime;

            var objs = LockDapperUtilTest<LockPers_>.New.Get(where, order, true);

            WriteJson(objs);
        }

        [Test]
        public void Add测试()
        {

            var efrows = LockDapperUtilTest<LockPers>.New.Insert(p =>
            {
                p.Id = Guid.NewGuid().ToString();
                p.Name = "测试bool添加";
                p.Content = p.Name;
                p.InsertTime = DateTime.Now;
                p.IsDel = false;
            });

            Console.WriteLine(efrows);

        }
        [Test]
        public void Update测试1()
        {
            var issucs = LockDapperUtilTest<LockPers>.New.Update(
                s =>
                {
                    s.Name = "测试bool修改";
                    s.Content = s.Name;
                    s.IsDel = true;
                },
                w => w.Name == "测试bool添加" && w.IsDel == false
                );
            Console.WriteLine(issucs);
        }
        [Test]
        public void Update测试2()
        {
            LockPers set = new LockPers() { Content = "测试bool修改2" };
            set.Name = "测试bool修改2";
            set.IsDel = true;

            var issucs = LockDapperUtilTest<LockPers>.New.Update(
                set,
                w => w.Name == "测试bool修改" && w.IsDel == true
                );
            Console.WriteLine(issucs);
        }

        [Test]
        public void Delete测试()
        {

            var Name = "测试bool修改2";

            var issucs = LockDapperUtilTest<LockPers>.New.Delete(
                w => w.Name == Name && w.IsDel == true
                );
            Console.WriteLine(issucs);

        }




        [Test]
        public void bool测试()
        {
            Expression<Func<LockPers, bool>> expression = t => t.Name.Contains("%蛋蛋%") && t.IsDel == false;
            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            Console.WriteLine(sql);

            var objs3 = LockDapperUtilTest<LockPers>.New.Get(expression);
            WriteJson(objs3);

        }

        [Test]
        public void In测试1()
        {
            // 1 in 表达式内创建数组(少量) 会直接转成sql 不走参数化
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
        public void In测试2()
        {
            //var Name = "测试bool修改2";
            //2 in  声明数组变量当参数传入 走参数化查询
            var arrEditCount = new int[5] { 22, 2, 3, 5, 1 };  // 
            Expression<Func<LockPers, bool>> expression = w => SM.In(w.EditCount, arrEditCount)
                //&& w.Name == Name
                && (w.Prompt.Contains("%hou%") || w.IsDel == false);
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

            //var objs = LockDapperUtilTest<LockPers>.Get(expression);
            //WriteJson(objs);

        }

        public void NotIn测试() {

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
                && (w.IsDel != false && (w.IsDel == false || w.IsDel == true));
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
        public void 括号测试2() {
            Expression<Func<LockPers, bool>> expression = w =>
             w.Id == "1" && w.Name == "2"
             || (w.IsDel != false && w.IsDel == true);
            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            Console.WriteLine(sql);
            var example = " w.Id == '1' && w.Name == '2' || (w.IsDel != false && w.IsDel == true) ";
            Console.WriteLine(example);

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

        [Test]
        public void 不同查询条件添加不通的where()
        {

            var Name = "测试bool修改2";
            //var Content = "测试bool修改2";
            //var arrEditCount = new int[5] { 22, 2, 3, 5, 1 };  // 
            DateTime? startDate = new DateTime(2018, 10, 17);
            DateTime? endDate = ((DateTime)startDate).AddDays(1);
            var IsDel = false;

            //时间类型现在获取不到
            Expression<Func<LockPers, bool>> expression = w =>
                w.Name == Name
                && w.InsertTime >= startDate
                && w.InsertTime < endDate
                //&& w.InsertTime > // SM.DateStr(InserTime)
                //&& SM.In(w.EditCount, arrEditCount) 
                && w.IsDel == IsDel;

            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sql, ref spars);
            Console.WriteLine(sql);

            // expression = w => w.Content == Content;


            foreach (var name in spars.ParameterNames)
            {
                Console.WriteLine(name);
                WriteJson(spars.Get<object>(name));
            }

        }
        [Test]
        public void 不同查询条件添加不通的where2() {

            //1
            //Expression<Func<LockPers, bool>> expression1 = w => w.Name == "";
            //Expression<Func<LockPers, bool>> expression2 = w => w.IsDel == true;
            //Expression<Func<LockPers, bool>> expression3 = w => w.Content == ""; 
            //Expression<Func<LockPers, bool>> expression4 = expression3.And(expression1.Or(expression2));

            //2
            var arrEditCount = new int[6] { 22, 2, 3, 5, 1, 11 };  // 
            DateTime? startDate = new DateTime(2018, 10, 17);
            DateTime? endDate = ((DateTime)startDate).AddDays(1);

            DateTime? startDate2 = new DateTime(2018, 3, 17);
            DateTime? endDate2 = ((DateTime)startDate).AddDays(1);

            Expression<Func<LockPers_, bool>> where = PredicateBuilder.WhereStart<LockPers_>();
            where = where.And(w => SM.In(w.EditCount, arrEditCount));
            where = where.And(w => w.InsertTime >= startDate && w.InsertTime < endDate || w.InsertTime >= startDate2 && w.InsertTime < endDate2);
            where = where.And(w => w.IsDel == false);
            //where = where.Or(w => ());



            //3 or里面括号问题
            //int? i1 = 1;
            //int? i2 = 2;
            //int? i3 = 3;
            //Expression<Func<LockPers_, bool>> where = PredicateBuilder.WhereStart<LockPers_>();
            //where = where.And( ww => ww.EditCount == i1);
            //where = where.And(ww => ww.EditCount == i2);
            //where = where.Or(ww => (ww.EditCount == i3 && ww.IsDel == true));

            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(where, ref sql, ref spars);
            Console.WriteLine(sql);


            foreach (var name in spars.ParameterNames)
            {
                Console.WriteLine(name);
                WriteJson(spars.Get<object>(name));
            }


        }

        [Test]
        public void 分页查询()
        {

        }
        // 表别名字典重复????
        [Test]
        public void 四次自连表测试()
        {

        }

        [Test]
        public void 四表连接测试(){
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            SynNote snmodel = new SynNote();
            snmodel.Name = "%木头木%";
            Expression<Func<LockPers, Users, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote>();
            where = where.And((lpw, uw, sn, snn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            QueryMaker<LockPers, Users, SynNote, SynNote> query = LockDapperUtilTest<LockPers, Users, SynNote, SynNote>.Init()
                            .Select((lp, u, s, sn) => new {lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name }//null查询所有字段
                                  , JoinType.Left,  (lpp, uu, snn, snnn) => uu.Id == lpp.UserId
                                  , JoinType.Inner, (lpp, uu, snn, snnn) => uu.Id == snn.UserId && snn.Id == snn.UserId
                                  , JoinType.Inner, (lpp, uu, snn, snnn) => snnn.Id == snn.UserId )
                            .Where(where) //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                            .Order((lp, w, sn, snn) => new { lp.EditCount, lp.Name, sn.Content });

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                Console.WriteLine(name);  // 参数名
                WriteJson(resultsqlparams.Item2.Get<object>(name)); // 值
            }

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果

            int page = 2, rows = 2, records;
            var result2 = query.LoadPage(page, rows, out records);
            WriteJson(result2); //  查询结果

        }
        [Test]
        public void 三表连接测试() {

            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            SynNote snmodel = new SynNote();
            snmodel.Name = "%木头%";
            Expression<Func<LockPers, Users, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote>();
            where = where.And((lpw, uw, sn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn) => sn.Name.Contains(snmodel.Name));

            QueryMaker<LockPers, Users, SynNote> query = LockDapperUtilTest<LockPers, Users, SynNote>.Init()
                            .Select((lp, u, s) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name }//null查询所有字段
                                  , JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                                  , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
                            .Where(where)
                            .Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                Console.WriteLine(name);  // 参数名
                WriteJson(resultsqlparams.Item2.Get<object>(name)); // 值
            }

            int page = 2, rows = 3, records;
            var result2 = query.LoadPage(page, rows, out records);
            WriteJson(result2); //  查询结果
        }

        [Test]
        public void 双表连接测试() {

            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            QueryMaker<LockPers, Users> query = LockDapperUtilTest<LockPers, Users>.Init()
                            .Select( (lp, u) =>  new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }//null查询所有字段
                                  , JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                            .Where(  where)
                                   //(lp, u) => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel || u.UserName == umodel.UserName )
                            .Order((lp, w) => new { lp.EditCount, lp.Name }); // .ExcuteSelect();
            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                Console.WriteLine(name);  // 参数名
                WriteJson(resultsqlparams.Item2.Get<object>(name)); // 值
            }

            int page = 2, rows = 3, records;
            var result2 = query.LoadPage(page, rows, out records);
            WriteJson(result2); //  查询结果



            //QueryMaker<LockPers, Users> query = new LockDapperUtilTest<LockPers, Users>();
            //query


            // 第2中where表达式 匿名参数属性传入
            var model = new { Name  = "%蛋蛋%" , IsDel  = false, UserName = "jiaojiao" };
            //Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            //where = where.And((lpw, uw) => lpw.Name.Contains(model.Name));
            //where = where.And((lpw, uw) => lpw.IsDel == model.IsDel);
            //where = where.And((lpw, uw) => uw.UserName == model.UserName);


            // 第3种where表达式
            var Name2 = "%蛋蛋%";
            var IsDel2 = false;
            var UserName = "jiaojiao";
            var Name = model.Name; // "%蛋蛋%";
            var IsDel = model.IsDel; //false;
            var uName = model.UserName; // "jiaojiao";

            //.Where( (lpw, uw) => lpw.Name.Contains("%蛋蛋%") && lpw.Name.Contains(Name2) && lpw.Name.Contains(model.Name)
            //&& lpw.IsDel == IsDel2 && lpw.IsDel == model.IsDel && uw.UserName == model.UserName )  // 


            //foreach (dynamic item in result)
            //{

            //} 

        }

        /*
           LockDapperUtilTest.Queryable<LockPers,Users>((lp, u) => new object[] {
            JoinType.Left,   u.Id = lp.UserId })
           .Select(
               (w,w2) => w.Name == Name  && w.IsDel == IsDel && w2.UserName == uName    
           ).ToList();

           LockDapperUtilTest.Queryable<LockPers, Users>(JoinType.Left,(lp, u) => u.Id == lp.UserId)
           .Queryable<Users, LockPers>(JoinType.Left,(u, lp) => u.Id == lp.UserId)
           .Where()
           .Excute();

           QueryMaker.New()
                           .SELECT<LockPers, Users, SynNote>()
                           .JoinTable(  JoinType.Left, (lp, u) => u.Id == lp.UserId     )
                           .JoinTable<Users, >(JoinType.Left, (u, lp) => u.Id == lp.UserId)
                           .Where<LockPers, Users>( (lp, u) => lp.Name == Name && lp.IsDel == IsDel && u.UserName == uName )
                           .RawSql(); 
            
                            //.JoinTable(JoinType.Left, (lp, u) => u.Id == lp.UserId)
                            // .LeftJoin( (u, lp) => u.Id == lp.UserId)
            */
        [Test]
        public void 表别名() {

            var Name = "测试bool修改2";
            var IsDel = false;
            var uName = "cc";

            Tuple<StringBuilder, DynamicParameters> result = LockDapperUtilTest<LockPers,Users>.Init().Select(
                             (lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }
                                  ,JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                            .Where((lpw, uw) => lpw.Name == Name && lpw.IsDel == IsDel && uw.UserName == uName)
                            .Order((lp, w) => new { lp.EditCount, lp.Name })
                            .RawSqlParams();
                            //.Excute(); 
            Console.WriteLine(result.Item1);
            foreach (var name in result.Item2.ParameterNames)
            {
                Console.WriteLine(name);
                WriteJson(result.Item2.Get<object>(name));
            }

            //var obj = LockDapperUtilTest.Queryable<LockPers, Users>(JoinType.Left,(lp, u) => u.Id == lp.UserId);
            //var obj2 = LockDapperUtilTest.Queryable<Users, LockPers>(JoinType.Left,(u, lp) => u.Id == lp.UserId);
            //var obj =  LockDapperUtilTest.Queryable1<LockPers, Users>((lp, u) => new object[] {
            //  "JoinType.Left",   u.Id == lp.UserId });
            Expression<Func<LockPers, bool>> expression = t => t.Name.Contains("%蛋蛋%") && t.IsDel == false;
            //var list = LockDapperUtilTest<LockPers>.New.Get(expression);
            //WriteJson(list);
        }
        [Test]
        public void 联表sql生成() {
            var Name = "测试bool修改2";
            //var Content = "测试bool修改2";
            //var arrEditCount = new int[5] { 22, 2, 3, 5, 1 };  // 
            DateTime? startDate = new DateTime(2018, 10, 17);
            DateTime? endDate = ((DateTime)startDate).AddDays(1);
            var IsDel = false;
            var uName = "cc";
            //w,w2 作为表别名
            Dictionary<string, string> tabalis = new Dictionary<string, string>();
            tabalis.Add(typeof(LockPers).FullName,"w");
            tabalis.Add(typeof(Users).FullName, "w2");
            //时间类型现在获取不到
            Expression<Func<LockPers,Users, bool>> expression = (w,w2) =>
                w.Name == Name
                && w.InsertTime >= startDate
                && w.InsertTime < endDate
                //&& w.InsertTime > // SM.DateStr(InserTime)
                //&& SM.In(w.EditCount, arrEditCount) 
                && w.IsDel == IsDel
                && w2.UserName == uName
                ;

            StringBuilder sql = null;
            DynamicParameters spars = null;
            AnalysisExpression.JoinExpression(expression, tabalis, ref sql, ref spars);

            Console.WriteLine(sql);
            // expression = w => w.Content == Content;
            foreach (var name in spars.ParameterNames)
            {
                Console.WriteLine(name);
                WriteJson(spars.Get<object>(name));
            }

            Console.WriteLine("++_+++++++");

        }

        [Test]
        public void 联表查询() {

             
            string sql = " select * from Users u left join LockPers lp on u.Id = lp.UserId where u.Id = 3";
            using (SQLiteConnection conn = new SQLiteConnection(DataBaseConfig.LockTestSqlLiteConnectionString)) //GetCurrentConnection() )
            {
                //conn.Query<,,>()
                var obj = conn.Query<Users, LockPers, Users>(sql, 
                    (user, lockPers) => {
                        if (lockPers != null)
                            user.LockPerss.Add(lockPers);
                        return user;
                    }
                    , null
                    ); 


             }
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

            object test2 = LockDapperUtilTest.New.Query("SELECT * FROM LockPers where isDel != @isDel and name like @name and EditCount in @EditCount  order by Name ", spars);
            WriteJson(test2);
            object test3 = LockDapperUtilTest.New.Query("select count(1) as mycount , * from SynNote sn ", null);



            // 2 fomarrt
            //var name = "xxxo";
            //var key = new { Name = "test" };
            //string sql = $"select * from {name} where {key.Name} = @id";


        }



        [Test]
        public void Get查询测试()
        {
            // 1. GetId 
            //var old1 = LockDapperUtilTest.Get<LockPers>("7fc02473-fee2-40da-a048-4398d9b052fd");
            //WriteJson(old1); 
            //var old2 = LockDapperUtilTest.Get<LockPers_>("7fc02473-fee2-40da-a048-4398d9b052fd");
            //WriteJson(old2);


            // 2. 
            var objs3 = LockDapperUtilTest<LockPers>.New.Get(w => w.IsDel == true && w.Name.Contains("%蛋蛋%")
                && SM.In(w.EditCount, new int[4] { 11, 2, 3, 5 })
                );
            WriteJson(objs3);


            //var a = new object[] {  1 , "" , 222  };

            //LockDapperUtilTest<LockPers>.Get(
            //    f => {
            //           object o = new object[5] { f.Id, f.IsDel, f.Name, f.Prompt, f.UpdateTime };
            //    }

            //    ,w =>   w.IsDel == "1" && w.Name.Contains("%蛋蛋%") 
            //          ///&& SM.In(w.EditCount, new int[4] { 11, 2, 3, 5 })
            //    );

        }

        public void Update()
        {

            // 1. Update (set和where里不能有相同字段)
            LockPers pset = new LockPers(true);
            pset.Name = "修改95 只修改Name字段";
            LockPers pwhere = new LockPers(true);
            pwhere.Content = "7fa867c5b404547797614abe57341844";
            //var efrwostest2 = LockDapperUtilTest.Update<LockPers>(pset, pwhere);

            // 2.Update
            var efrowsupdate2 = LockDapperUtilTest<LockPers>.New.Update(
            set =>
            {
                set.Name = "修改95 修改Name和Content字段";
                set.Prompt = "BMWWWWWWWWWWWWWWW";
            }
            , where => where.Content == "xxxxxxxoooooooo");


            // 3.Update
            // where 字段参数名 会 和 set 字段参数名重复, set字段名统一加_
            var efrowsupdate3 = LockDapperUtilTest<LockPers>.New.Update(
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