using NUnit.Framework;
using System;
using System.Text;
using System.Linq.Expressions;
using Dapper;

using FW.Model;
using FW.Common.DapperExt;
using FW.Common;

namespace TestsFW.Common
{
    [TestFixture()]
    public class SelectDapperSqlMaker
    {
        private static void WriteJson(object test2)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(test2);
            Console.WriteLine(str);
        }

        #region SQLite联表分页查询
        [Test]
        public void 四表联表分页测试()
        {
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            SynNote snmodel = new SynNote();
            snmodel.Name = "%木头%";
            Expression<Func<LockPers, Users, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote>();
            where = where.And((lpw, uw, sn, snn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            DapperSqlMaker<LockPers, Users, SynNote, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote, SynNote>
                .Selec()
                .Column((lp, u, s, sn) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => snnn.Id == snn.UserId)
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
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果

        }
        [Test]
        public void 三表联表分页测试()
        {
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

            DapperSqlMaker<LockPers, Users, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote>
                .Selec()
                .Column((lp, u, s) => //null)  //查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
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
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 双表联表分页测试()
        { 
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            DapperSqlMaker<LockPers, Users> query = LockDapperUtilsqlite<LockPers, Users>
                .Selec()
                .Column((lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where)
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

            int page = 1, rows = 3, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果



            //DapperSqlMaker<LockPers, Users> query = new LockDapperUtilsqlite<LockPers, Users>();
            //query


            // 第2中where表达式 匿名参数属性传入
            var model = new { Name = "%蛋蛋%", IsDel = false, UserName = "jiaojiao" };
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
        [Test]
        public void 单表分页查询测试()
        {
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();
            where = where.And((lpw) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw) => lpw.IsDel == lpmodel.IsDel);

            DapperSqlMaker<LockPers> query = LockDapperUtilsqlite<LockPers>
                .Selec()
                .Column() // lp => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel }) // null查询所有字段
                .From()
                .Where(where) //lp => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel  )
                .Order(lp => new { lp.EditCount, lp.Name }); // .ExcuteSelect();
            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                WriteJson(name + " -- " + resultsqlparams.Item2.Get<object>(name)); // 值
            }

            int page = 1, rows = 3, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        #endregion


        #region MSSQL联表分页查询
        [Test]
        public void 查询批量插入测试MS()
        {
            //var list3old = LockDapperUtilTest<LockPers>.New.GetAll();
            //foreach (var item in list3old)
            //{
            //    var model = CopyModelHelper.MapperWrite<LockPers_ms, LockPers>(item);
            //    LockDapperUtilTest<LockPers_ms>.New.Insert(model);
            //}


            //var list2old = LockDapperUtilTest<SynNote>.New.GetAll();
            //foreach (var item in list2old)
            //{
            //    var model = CopyModelHelper.MapperWrite<SynNote_ms, SynNote>(item);
            //    LockDapperUtilTest<SynNote_ms>.New.Insert(model);
            //    //newlist.Add ( );
            //}

            //var list1old = LockDapperUtilTest<Users>.New.GetAll();
            //foreach (var item in list1old)
            //{
            //    var model = CopyModelHelper.MapperWrite<Users_ms, Users>(item);
            //    LockDapperUtilTest<Users_ms>.New.Insert(model);
            //}
        }

        [Test]
        public void 四表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            SynNote snmodel = new SynNote();
            snmodel.Name = "%木头%";
            Expression<Func<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms>();
            where = where.And((lpw, uw, sn, snn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            DapperSqlMaker<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms> query =
                LockDapperUtilmssql<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms>
                .Selec()
                .RowRumberOrderBy((lp, u, s, sn) => new { lp.Id, a = u.Id })
                .Column((lp, u, s, sn) =>
                   new { SM.LimitCount, lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })//null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn) => uu.Id == lpp.UserId
                    , JoinType.Inner, (lpp, uu, snn, snnn) => uu.Id == snn.UserId && snn.Id == snn.UserId
                    , JoinType.Inner, (lpp, uu, snn, snnn) => snnn.Id == snn.UserId)
                .Where(where); //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                               // .Order((lp, w, sn, snn) => new { lp.EditCount, lp.Name, sn.Content });

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
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果

        }
        [Test]
        public void 三表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            SynNote snmodel = new SynNote();
            snmodel.Name = "%木头%";
            Expression<Func<LockPers_ms, Users_ms, SynNote_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms, Users_ms, SynNote_ms>();
            where = where.And((lpw, uw, sn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn) => sn.Name.Contains(snmodel.Name));

            DapperSqlMaker<LockPers_ms, Users_ms, SynNote_ms> query = LockDapperUtilmssql<LockPers_ms, Users_ms, SynNote_ms>
                .Selec()
                .RowRumberOrderBy((lp, u, sn) => new { lp.EditCount, lp.Name, sn.Content })
                .Column((lp, u, s) => //null)  //查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
                .Where(where);
            //.Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });

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
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 双表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            DapperSqlMaker<LockPers, Users> query = LockDapperUtilmssql<LockPers, Users>
                .Selec()
                .RowRumberOrderBy((lp, u) => new { lp.EditCount, lp.Name })
                .Column((lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where); //(lp, u) => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel || u.UserName == umodel.UserName )
                //.Order((lp, w) => new { lp.EditCount, lp.Name }); // .ExcuteSelect();
            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                WriteJson(name + " -- " + resultsqlparams.Item2.Get<object>(name)); // 参数 -- 值
            }

            int page = 1, rows = 3, records;
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果

        }

        [Test]
        public void 单表分页查询测试MS()
        {
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();
            where = where.And((lpw) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw) => lpw.IsDel == lpmodel.IsDel);

            DapperSqlMaker<LockPers> query = LockDapperUtilmssql<LockPers>
                .Selec()
                .RowRumberOrderBy(lp => new { lp.EditCount, lp.Name })
                .Column() // lp => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel }) // null查询所有字段
                .From()
                .Where(where); //lp => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel  )
                //.Order(lp => new { lp.EditCount, lp.Name }); // .ExcuteSelect();
            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            
            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                WriteJson(name + " -- " + resultsqlparams.Item2.Get<object>(name)); // 参数 -- 值
            }

            int page = 1, rows = 3, records;
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果
        }

        #endregion

    }
}
