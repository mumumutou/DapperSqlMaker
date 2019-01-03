using NUnit.Framework;
using System;
using System.Text;
using System.Linq.Expressions;
using Dapper;

using FW.Model;
using DapperSqlMaker.DapperExt;
using DapperSqlMaker;

namespace TestsDapperSqlMaker
{
    [TestFixture()]
    public class SelectDapperSqlMakerTest
    {
        private static void WriteJson(object test2)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(test2);
            Console.WriteLine(str);
        }

        // 打印sql和参数
        private static void WriteSqlParams(Tuple<StringBuilder, DynamicParameters> resultsqlparams)
        {
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                WriteJson(name + " -- " + resultsqlparams.Item2.Get<object>(name)); // 参数 -- 值
            }
        }

        #region SQLite联表分页查询
        [Test]
        public void 六表联表分页测试()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
            Expression<Func<LockPers, Users, SynNote, SynNote, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote, SynNote, SynNote>();
            where = where.And((lpw, uw, sn, snn, s5, s6) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn, s5, s6) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn, s5, s6) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn, s5, s6) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote, SynNote, SynNote, SynNote>
                .Selec()
                .Column((lp, u, s, sn, s5, s6) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left,  (lpp, uu, snn, snnn, s5, s6) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == s5.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => s5.Id == s6.UserId)
                .Where(where) //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                .Order((lp, w, sn, snn, s5, s6) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); // 打印sql和参数

            int page = 2, rows = 2, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 五表联表分页测试()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
            Expression<Func<LockPers, Users, SynNote, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote, SynNote>();
            where = where.And((lpw, uw, sn, snn, s5) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn, s5) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn, s5) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn, s5) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote, SynNote, SynNote>
                .Selec()
                .Column((lp, u, s, sn, s5) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left,  (lpp, uu, snn, snnn, s5) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == s5.UserId)
                .Where(where) //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                .Order((lp, w, sn, snn, s5) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); // 打印sql和参数

            int page = 2, rows = 2, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 四表联表分页测试()
        {
            LockPers lpmodel = new LockPers() { Name= "%蛋蛋%", IsDel= false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
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

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); // 打印sql和参数

            int page = 2, rows = 2, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 三表联表分页测试()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false};
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
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
            WriteSqlParams(resultsqlparams);

            int page = 2, rows = 3, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 双表联表分页测试()
        { 
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
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
            WriteSqlParams(resultsqlparams);

            int page = 1, rows = 3, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 单表分页查询测试()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
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
            WriteSqlParams(resultsqlparams);

            int page = 1, rows = 3, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }

        [Test]
        public void 查询所有数据() {
            // 4表
            DapperSqlMaker<LockPers, Users, SynNote, SynNote> query4 = 
                LockDapperUtilsqlite<LockPers, Users, SynNote, SynNote>
                .Selec()
                .Column()  // null查询所有字段 // (lp, u, s, sn) =>  new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => snnn.Id == snn.UserId);
            var result4 = query4.ExcuteSelect();
            WriteJson(result4); //  查询结果
            // 3表
            DapperSqlMaker<LockPers, Users, SynNote> query3 = 
                LockDapperUtilsqlite<LockPers, Users, SynNote>
               .Selec()
               .Column() //null 查询所有字段 // (lp, u, s) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
               .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                       , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId);
            var result3 = query3.ExcuteSelect();
            WriteJson(result3); //  查询结果

            // 2表
            DapperSqlMaker<LockPers, Users> query2 = LockDapperUtilsqlite<LockPers, Users>
               .Selec()
               .Column() //null 查询所有字段 //(lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
               .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId);
            var result2 = query2.ExcuteSelect();
            WriteJson(result2); //  查询结果

            // 1 表
            DapperSqlMaker<LockPers> query = LockDapperUtilsqlite<LockPers>
               .Selec()
               .Column() // null查询所有字段// lp => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel }) 
               .From();
            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
        }


        #endregion


        #region MSSQL联表分页查询
        [Test]
        public void 六表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
            Expression<Func<LockPers, Users, SynNote, SynNote, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote, SynNote, SynNote>();
            where = where.And((lpw, uw, sn, snn, s5, s6) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn, s5, s6) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn, s5, s6) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn, s5, s6) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote, SynNote> query = LockDapperUtilmssql<LockPers, Users, SynNote, SynNote, SynNote, SynNote>
                .Selec()
                .RowRumberOrderBy((lp, u, s, sn, s5, s6) => new { lp.Id, b = SM.OrderDesc(lp.EditCount), a = u.Id })
                .Column((lp, u, s, sn, s5, s6) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn, s5, s6) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == s5.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => s5.Id == s6.UserId)
                .Where(where); //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                //.Order((lp, w, sn, snn, s5, s6) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); // 打印sql和参数

            int page = 2, rows = 2, records;
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 五表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
            Expression<Func<LockPers, Users, SynNote, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote, SynNote>();
            where = where.And((lpw, uw, sn, snn, s5) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn, s5) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn, s5) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn, s5) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote> query = LockDapperUtilmssql<LockPers, Users, SynNote, SynNote, SynNote>
                .Selec()
                .RowRumberOrderBy((lp, u, s, sn, s5) => new { lp.Id, a = u.Id })
                .Column((lp, u, s, sn, s5) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn, s5) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == s5.UserId)
                .Where(where); //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                //.Order((lp, w, sn, snn, s5) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); // 打印sql和参数

            int page = 2, rows = 2, records;
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 四表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
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
            WriteSqlParams(resultsqlparams);

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果

            int page = 2, rows = 2, records;
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果

        }
        [Test]
        public void 三表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers() {  Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
            Expression<Func<LockPers_ms, Users_ms, SynNote_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms, Users_ms, SynNote_ms>();
            where = where.And((lpw, uw, sn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn) => sn.Name.Contains(snmodel.Name));

            DapperSqlMaker<LockPers_ms, Users_ms, SynNote_ms> query = LockDapperUtilmssql<LockPers_ms, Users_ms, SynNote_ms>
                .Selec()
                .RowRumberOrderBy((lp, u, sn) => new { x = SM.OrderDesc(lp.EditCount), lp.Name, sn.Content })
                .Column((lp, u, s) => //null)  //查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
                .Where(where);
            //.Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); //打印sql和参数 

            int page = 2, rows = 3, records;
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果
        }
        [Test]
        public void 双表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel=false };
            Users umodel = new Users() { UserName = "jiaojiao" };
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
            WriteSqlParams(resultsqlparams);

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

        [Test]
        public void 字段别名和Column直接拼接sql()
        {
            //1. 字段别名 为 匿名类型成员名
            //2. Column和Order 直接拼接sql  用SM.Sql 或者直接写入 字符串值
            //2. (2)的字段别名 也要写在字符串里 注意:不是匿名类型成员们 
            string umodelall = "b.*";
            LockPers lpmodel = new LockPers() { IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" }; 

            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            DapperSqlMaker<LockPers, Users> query = LockDapperUtilsqlite<LockPers, Users>
                .Selec()
                .Column((lp, u) => new { a="LENGTH(a.Prompt)",b=SM.Sql(umodelall), lpid = lp.Id, lp.Name, lp.Prompt })
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where)
                .Order((lp, w) => new { a = SM.OrderDesc(lp.EditCount), lp.Id });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
            WriteSqlParams(query.RawSqlParams()); //打印sql和参数
        }

        [Test]
        public void 拼接不同查询条件() {

            LockPers lpmodel = new LockPers();
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";

            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            DapperSqlMaker<LockPers, Users> query = LockDapperUtilsqlite<LockPers, Users>
                .Selec()
                .Column() 
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where)
                .Order((lp, w) => new { a = SM.OrderDesc(lp.EditCount), lp.Id });

            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
        }

        [Test]
        public void 多排序字段升序降序() {
            // 倒序方法 SM.OrderDesc(lp.EditCount)

            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";

            DapperSqlMaker<LockPers, Users> query = LockDapperUtilsqlite<LockPers, Users>
                .Selec()
                .Column() //(lp, u) => new { SM.LimitCount , lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where((lp, u) => lp.Name.Contains(lpmodel.Name) && lp.IsDel == lpmodel.IsDel && u.UserName == umodel.UserName)
                .Order((lp, w) => new { a = SM.OrderDesc(lp.EditCount), lp.Id });
            //.ExcuteSelect();
            var result = query.ExcuteSelect();
            WriteJson(result); //  查询结果
             
        }

        //动态sql

    }
}
