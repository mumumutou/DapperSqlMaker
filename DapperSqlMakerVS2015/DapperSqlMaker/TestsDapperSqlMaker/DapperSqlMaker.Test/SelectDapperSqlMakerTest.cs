using NUnit.Framework;
using System;
using System.Text;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;

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
                WriteJson(name + " -- " + Newtonsoft.Json.JsonConvert.SerializeObject(resultsqlparams.Item2.Get<object>(name))); // 参数 -- 值
            }
        }

        #region SQLite联表分页查询
        //数据库上下文类 LockDapperUtilsqlite

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
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote, SynNote> query = DBSqlite<LockPers, Users, SynNote, SynNote, SynNote, SynNote>
                .Select()
                .Column((lp, u, s, sn, s5, s6) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn, s5, s6) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == s5.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => s5.Id == s6.UserId)
                .Where(where) //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                .Order((lp, w, sn, snn, s5, s6) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExecuteQuery();
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
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote> query = DBSqlite<LockPers, Users, SynNote, SynNote, SynNote>
                .Select()
                .Column((lp, u, s, sn, s5) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn, s5) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == s5.UserId)
                .Where(where) //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                .Order((lp, w, sn, snn, s5) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExecuteQuery();
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
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
            Expression<Func<LockPers, Users, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote>();
            where = where.And((lpw, uw, sn, snn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn, snn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn, snn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn, snn) => sn.Name.Contains(snmodel.Name));
            //  SM.LimitCount,
            DapperSqlMaker<LockPers, Users, SynNote, SynNote> query = DBSqlite<LockPers, Users, SynNote, SynNote>
                .Select()
                .Column((lp, u, s, sn) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => snnn.Id == snn.UserId)
                .Where(where) //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                .Order((lp, w, sn, snn) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExecuteQuery();
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
            var arruser = new int[2] { 1, 2 };  // 
            string uall = "b.*", pn1 = "%蛋蛋%", pn2 = "%m%";
            LockPers lpmodel = new LockPers() { IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "木头" };
            Expression<Func<LockPers, Users, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote>();
            where = where.And((l, u, s) => (l.Name.Contains(pn1) || l.Name.Contains(pn2)));
            where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((l, u, s) => u.UserName == umodel.UserName);
            where = where.And((l, u, s) => s.Name == snmodel.Name);
            where = where.And((l, u, s) => SM.In(u.Id, arruser));

            DapperSqlMaker<LockPers, Users, SynNote> query = DBSqlite<LockPers, Users, SynNote>
                .Select()
                .Column((lp, u, s) => //null)  //查询所有字段
                    new { lp.Name, lpid = lp.Id, x = "LENGTH(a.Prompt) as len", b = SM.Sql(uall), scontent = s.Content, sname = s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
                .Where(where)
                .Order((lp, w, sn) => new { lp.EditCount, x = SM.OrderDesc(lp.Name), sn.Content });

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams); // 打印sql和参数

            var result = query.ExecuteQuery();
            WriteJson(result); //  查询结果

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

            DapperSqlMaker<LockPers, Users> query = DBSqlite<LockPers, Users>
                .Select()
                .Column((lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where)
                            //(lp, u) => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel || u.UserName == umodel.UserName )
                            .Order((lp, w) => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
            var result = query.ExecuteQuery();
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

            DapperSqlMaker<LockPers> query = DBSqlite<LockPers>
                .Select()
                .Column() // lp => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel }) // null查询所有字段
                .From()
                .Where(where) //lp => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel  )
                .Order(lp => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
            var result = query.ExecuteQuery();
            WriteJson(result); //  查询结果

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams);

            int page = 1, rows = 3, records;
            var result2 = query.LoadPagelt(page, rows, out records);
            WriteJson(result2); //  查询结果
        }

        [Test]
        public void 查表简单查询测试()
        {

            var first = DBSqlite<LockPers>.Select()
                .Column()
                .From()
                .Where(a => a.IsDel == true)
                .ExecuteQuery<LockPers>();
            WriteJson(first); //  查询结果
        }

        [Test]
        public void 查询首行首列列测试() {

            var first = DBSqlite<Users, Skin>.Select()
                .Column((a, b) => new { Value = b.Value })
                .FromJoin(JoinType.Inner, (a, b) => a.SkinId == b.Id)
                .Where((a, b) => a.Id == 1 && a.UserName == "cc")
                .ExecuteScalar<string>();
            WriteJson(first); //  查询结果
        }

        #endregion

        #region MSSQL联表分页查询

        //数据库上下文类 LockDapperUtilmssql

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
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote, SynNote> query = DBMSSql<LockPers, Users, SynNote, SynNote, SynNote, SynNote>
                .Select()
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

            var result = query.ExecuteQuery();
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
            DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote> query = DBMSSql<LockPers, Users, SynNote, SynNote, SynNote>
                .Select()
                .RowRumberOrderBy((lp, u, s, sn, s5) => new { lp.Id, a = u.Id })
                .Column((lp, u, s, sn, s5) =>  // )null查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn, s5) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == s5.UserId)
                .Where(where); //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
                               //.Order((lp, w, sn, snn, s5) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExecuteQuery();
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
                DBMSSql<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms>
                .Select()
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

            var result = query.ExecuteQuery();
            WriteJson(result); //  查询结果

            int page = 2, rows = 2, records;
            var result2 = query.LoadPagems(page, rows, out records);
            WriteJson(result2); //  查询结果

        }
        [Test]
        public void 三表联表分页测试MS()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            SynNote snmodel = new SynNote() { Name = "%木头%" };
            Expression<Func<LockPers_ms, Users_ms, SynNote_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms, Users_ms, SynNote_ms>();
            where = where.And((lpw, uw, sn) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw, sn) => uw.UserName == umodel.UserName);
            where = where.And((lpw, uw, sn) => sn.Name.Contains(snmodel.Name));

            DapperSqlMaker<LockPers_ms, Users_ms, SynNote_ms> query = DBMSSql<LockPers_ms, Users_ms, SynNote_ms>
                .Select()
                .RowRumberOrderBy((lp, u, sn) => new { x = SM.OrderDesc(lp.EditCount), lp.Name, sn.Content })
                .Column((lp, u, s) => //null)  //查询所有字段
                    new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
                .Where(where);
            //.Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });

            var result = query.ExecuteQuery();
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
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            DapperSqlMaker<LockPers, Users> query = DBMSSql<LockPers, Users>
                .Select()
                .RowRumberOrderBy((lp, u) => new { lp.EditCount, lp.Name })
                .Column((lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where); //(lp, u) => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel || u.UserName == umodel.UserName )
                               //.Order((lp, w) => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
            var result = query.ExecuteQuery();
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

            DapperSqlMaker<LockPers> query = DBMSSql<LockPers>
                .Select()
                .RowRumberOrderBy(lp => new { lp.EditCount, lp.Name })
                .Column() // lp => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel }) // null查询所有字段
                .From()
                .Where(where); //lp => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel  )
                               //.Order(lp => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
            var result = query.ExecuteQuery();
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
        public void 左连接右表条件测试() {

            var where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => uw.UserName == "jiaojiao");

            var all = DBSqlite<LockPers>.Select().Column().From().ExecuteQueryList();
            Console.WriteLine(all.Count);

            var list = DBSqlite<LockPers, Users>
                .Select()
                .Column((lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where).ExecuteQueryList();
            Console.WriteLine(list.Count);

            var name = "jiaojiao";
            //DynamicParameters jdmp = new DynamicParameters("#");
            //jdmp.Add("UserName_join_0", name);

            var leftquery = DBSqlite<LockPers, Users>
                .Select()
                .Column((lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId && uu.UserName == name);
            //.Where((l,u) => u.UserName == name)
            //.SqlParams(jdmp);
            Console.WriteLine(leftquery.RawSqlParams().Item1);
            var parms = leftquery.RawSqlParams().Item2;

            var left = leftquery.ExecuteQueryList();
            Console.WriteLine(left.Count);

        }
        [Test]
        public void MsSql12新分页语法测试()
        {
            int page = 2;
            int rows = 3;
            int records = 0;
            var name = "jiaojiao";
            var list = DBMSSql<LockPers, Users>
                .Select()
                .Column() //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId && uu.UserName == name)
                .Order((l, u) => new { u.UserName })
                .LoadPageMsSql2List<LockPers>(page, rows, out records, p => p.records);
            WriteJson(list);
        }

        [Test]
        public void jqGrid数据栗子MS()
        {
            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();
            where = where.And((lpw) => lpw.Name.Contains("%0%"));

            DapperSqlMaker<LockPers> query = DBMSSql<LockPers>
                .Select()
                .RowRumberOrderBy(lp => new { lp.EditCount, lp.Name })
                .Column(p => new { SM.LimitCount, a = SM.Sql("*") }) // 
                .From()
                .Where(where); //lp => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel  )
                               //.Order(lp => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
        }

        #endregion

        [Test]
        public void MSSql新分页测试() {
            var x = PredicateBuilder.WhereStart<Esy.Base.Application.Model.EsySysUser_>();
            x = x.And(p => p.F_UserId != "");
            var query = EsyDb<Esy.Base.Application.Model.EsySysUser_>.Select()
                //.RowRumberOrderBy(p => new { p.F_OrdCode })
                //.Column(p => new { SM.LimitCount, x = SM.Sql("*") })
                .Column()
                .From().Where(x)
                .Order(p => new { p.F_UserId })
                ;
            var sql = query.RawSqlParams().Item1.ToString();
            Console.WriteLine("1");
            try
            {
                var list = query.LoadPageMsSql2<Esy.Base.Application.Model.EsySysUser_>(1, 50);
                Console.WriteLine(list);
                // 没问题
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Test]
        public void 空数据查询() {

            try
            { // 首行空数据 T (空数据报错)
                LockPers tnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQueryFirst<LockPers>();
            }
            catch (Exception) { Console.WriteLine("查询首行报错啦"); }
            try
            {   // 首行空数据 dynamic (空数据报错)
                var mnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQueryFirst();
            }
            catch (Exception) { Console.WriteLine("查询首行报错啦"); }
            // 首行空数据 T (空数据返回null)
            LockPers dftnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQueryFirstOrDefault<LockPers>();
            // 首行空数据 T (空数据返回null)
            var dfmnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQueryFirstOrDefault();


            // 首行首列 (空数据返回null)
            var mcolumnnull = DBSqlite<LockPers>.Select().Column(p => new { p.Name }).From().Where(p => p.Name == "null").ExecuteScalar<string>();
            // 首行首列 (空数据返回0)
            var icolumnnull = DBSqlite<LockPers>.Select().Column(p => new { p.Name }).From().Where(p => p.Name == "null").ExecuteScalar<int>();


            // 以下多行空数据 都返回Count=0的集合

            // 多行空数据 dynamic
            var tdsmlistnullist = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQuery();
            // 多行空数据 T
            var tlistnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQuery<LockPers>();
            // 多行空数据 dynamic
            var tdsmlistnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQueryList();
            // 多行空数据 T
            var tlistnullist = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").ExecuteQueryList<LockPers>();

            int records;
            // 多行空数据 T
            var pagesnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").LoadPagelt<LockPers>(1, 10, out records);
            // 多行空数据 T
            var pagesdsmnull = DBSqlite<LockPers>.Select().Column().From().Where(p => p.Name == "null").LoadPagelt(1, 10, out records);



        }

        [Test]
        public void 查询首行()
        {
            var m = DBSqlite<LockPers>.Select().Column().From().ExecuteQueryFirst();

            LockPers t = DBSqlite<LockPers>.Select().Column().From().ExecuteQueryFirst<LockPers>();

            WriteJson(m); //  查询结果
            Console.WriteLine("-------------------");
            WriteJson(t); //  查询结果
            Console.WriteLine("-------------------");

            LockPers t2 = new LockPers();
            t2.Name = m.Name;
            t2.Content = m.Content;
            t2.Prompt = m.Prompt;
            t2.Id = m.Id;
            t2.InsertTime = m.InsertTime;
            t2.IsDel = m.IsDel;
            t2.DelTime = m.DelTime;
            t2.UpdateTime = m.UpdateTime;
            t2.EditCount = Convert.ToInt32(m.EditCount);
            t2.CheckCount = Convert.ToInt32(m.CheckCount);
            t2.UserId = Convert.ToInt32(m.UserId);

            WriteJson(t2);


        }
        [Test]
        public void 查询所有数据() {
            // 4表
            DapperSqlMaker<LockPers, Users, SynNote, SynNote> query4 =
                DBSqlite<LockPers, Users, SynNote, SynNote>
                .Select()
                .Column()  // null查询所有字段 // (lp, u, s, sn) =>  new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
                .FromJoin(JoinType.Left, (lpp, uu, snn, snnn) => uu.Id == lpp.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => uu.Id == snn.UserId && snn.Id == snn.UserId
                        , JoinType.Inner, (lpp, uu, snn, snnn) => snnn.Id == snn.UserId);
            var result4 = query4.ExecuteQuery();
            WriteJson(result4); //  查询结果
            // 3表
            DapperSqlMaker<LockPers, Users, SynNote> query3 =
                DBSqlite<LockPers, Users, SynNote>
               .Select()
               .Column() //null 查询所有字段 // (lp, u, s) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
               .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                       , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId);
            var result3 = query3.ExecuteQuery();
            WriteJson(result3); //  查询结果

            // 2表
            DapperSqlMaker<LockPers, Users> query2 = DBSqlite<LockPers, Users>
               .Select()
               .Column() //null 查询所有字段 //(lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
               .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId);
            var result2 = query2.ExecuteQuery();
            WriteJson(result2); //  查询结果

            // 1 表
            DapperSqlMaker<LockPers> query = DBSqlite<LockPers>
               .Select()
               .Column() // null查询所有字段// lp => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel }) 
               .From();
            var result = query.ExecuteQuery();
            WriteJson(result); //  查询结果
        }
        [Test]
        public void 字段别名和查询字段中直接拼接sql()
        {
            //1. 查询的实体类 字段别名 为匿名类型成员名
            //2. (Column和Order)中直接拼接sql 用SM.Sql 或者直接写入 字符串值
            //3. (2)的字段别名 也要写在字符串里 注意:这里匿名类型成员名只是为了符合语法,不会被解析成别名 
            //4. Order倒序标记方法 SM.OrderDesc()
            string umodelall = "b.*";
            LockPers lpmodel = new LockPers() { IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };

            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            DapperSqlMaker<LockPers, Users> query = DBSqlite<LockPers, Users>
                .Select()
                .Column((lp, u) => new { a = "LENGTH(a.Prompt) as len", b = SM.Sql(umodelall), lpid = lp.Id, lp.Name, lp.Prompt })
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where)
                .Order((lp, w) => new { a = SM.OrderDesc(lp.EditCount), lp.Id });

            var result = query.ExecuteQuery();
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

            DapperSqlMaker<LockPers, Users> query = DBSqlite<LockPers, Users>
                .Select()
                .Column()
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where)
                .Order((lp, w) => new { a = SM.OrderDesc(lp.EditCount), lp.Id });

            var result = query.ExecuteQuery();
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

            DapperSqlMaker<LockPers, Users> query = DBSqlite<LockPers, Users>
                .Select()
                .Column() //(lp, u) => new { SM.LimitCount , lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where((lp, u) => lp.Name.Contains(lpmodel.Name) && lp.IsDel == lpmodel.IsDel && u.UserName == umodel.UserName)
                .Order((lp, w) => new { a = SM.OrderDesc(lp.EditCount), lp.Id });
            //.ExecuteQuery();
            var result = query.ExecuteQuery();
            Console.WriteLine(query.RawSqlParams().Item1);
            WriteJson(result); //  查询结果

        }

        [Test]
        public void Like查询条件() {

            LockPers lpm = new LockPers();
            Users um = new Users();
            um.UserName = "%jiaojiao%";
            lpm.Users = um;
            PredicateBuilder.WhereStart<LockPers, Users>();

            var lpnobj = new { UserName = "%jiaojiao%" };


            var query = DBSqlite<LockPers, Users>
                .Select().Column().FromJoin(JoinType.Left, (p, u) => p.UserId == u.Id)
                .Where((p, u) => u.UserName.Contains(lpnobj.UserName));
            var rawsqlparms = query.RawSqlParams();
            WriteSqlParams(rawsqlparms); //打印sql和参数

            var re = query.ExecuteQuery();

        }


        [Test]
        public void Or开头查询条件()
        {




            LockPers lpmodel = new LockPers();
            lpmodel.IsDel = false;
            Users umodel = new Users();
            umodel.UserName = "jiaojiao";
            string serh = "%%";
            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();
            //where = where.Or((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            //where = where.Or((lpw, uw) => uw.UserName == umodel.UserName);
            where = where.And(m => m.IsDel != true);
            where = where.And(m => (m.Name.Contains(serh) || m.Prompt.Contains(serh)));

            DapperSqlMaker<LockPers> query = DBSqlite<LockPers>
                .Select()
                .Column()
                .From()
                .Where(where)
                .Order((lp) => new { a = SM.OrderDesc(lp.EditCount), lp.Id });
            var rawsqlparms = query.RawSqlParams();
            WriteSqlParams(rawsqlparms); //打印sql和参数

            var result = query.ExecuteQuery();
            WriteJson(result); //  查询结果
        }


        //MSSql: with 表表达式拼接
        [Test]
        public void 共用表表达式() {

            string withsql = " with u as ( select * from Users )";
            string uname = " , (select u.UserName from u where u.Id = a.UserId) UserNamex ";

            // 单表
            LockPers lpmodel = new LockPers();
            lpmodel.Name = "%蛋蛋%";
            lpmodel.IsDel = false;
            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();
            where = where.And((lpw) => lpw.Name.Contains(lpmodel.Name));
            where = where.And((lpw) => lpw.IsDel == lpmodel.IsDel);

            DapperSqlMaker<LockPers> query = DBMSSql<LockPers>
                .SqlClaus(withsql)  // 公用表表达式
                .Select()
                .RowRumberOrderBy(lp => new { lp.EditCount, lp.Name })
                .Column() //p => new { p.Name, p.Prompt }) // lp => new {SM.Sql("*") lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel }) // null查询所有字段
                .SqlClause(uname)   // 查询cte字段
                .From()
                .Where(where); //lp => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel  )
                               //.Order(lp => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
            var result = query.ExecuteQuery();
            WriteJson(result); //  查询结果
            query.PageStartms(1).PageEndms();
            int records;
            var pageresult = query.LoadPagemscte(1, 2, out records); // mssql cte 分页
            Console.WriteLine(records);
            WriteJson(pageresult);


            #region 4表测试
            //// 4表
            //LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            //Users umodel = new Users() { UserName = "jiaojiao" };
            //SynNote snmodel = new SynNote() { Name = "%木头%" };
            //Expression<Func<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms>();
            //where = where.And((lpw, uw, sn, snn) => lpw.Name.Contains(lpmodel.Name));
            //where = where.And((lpw, uw, sn, snn) => lpw.IsDel == lpmodel.IsDel);
            //where = where.And((lpw, uw, sn, snn) => uw.UserName == umodel.UserName);
            //where = where.And((lpw, uw, sn, snn) => sn.Name.Contains(snmodel.Name));
            ////  SM.LimitCount,
            //DapperSqlMaker<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms> query =
            //    LockDapperUtilmssql<LockPers_ms, Users_ms, SynNote_ms, SynNote_ms>
            //    .SqlClaus(withsql)  // 公用表表达式
            //    .Select()
            //    .RowRumberOrderBy((lp, u, s, sn) => new { lp.Id, a = u.Id, xu = SM.OrderDesc(lp.UserId) })
            //    .Column((lp, u, s, sn) =>
            //       new { SM.LimitCount, lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })//null查询所有字段
            //    .SqlClause(uname)
            //    .FromJoin(JoinType.Left, (lpp, uu, snn, snnn) => uu.Id == lpp.UserId
            //        , JoinType.Inner, (lpp, uu, snn, snnn) => uu.Id == snn.UserId && snn.Id == snn.UserId
            //        , JoinType.Inner, (lpp, uu, snn, snnn) => snnn.Id == snn.UserId)
            //    .Where(where); //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
            //                   // .Order((lp, w, sn, snn) => new { lp.EditCount, lp.Name, sn.Content });
            //Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            //WriteSqlParams(resultsqlparams);
            //var result = query.ExecuteQuery();
            //WriteJson(result); //  查询结果
            //int records;
            //var result2 = query.PageStartms(1).PageEndms().LoadPagemscte(1, 2, out records);
            //WriteJson(result2);
            #endregion

            #region 多表测试 2 3 5 6
            //// 2表
            //LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            //Users umodel = new Users() { UserName = "jiaojiao" };
            //Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            //where = where.And((lpw, uw) => lpw.Name.Contains(lpmodel.Name));
            //where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            //where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            //DapperSqlMaker<LockPers, Users> query = LockDapperUtilmssql<LockPers, Users>
            //    .SqlClaus(withsql)  // 公用表表达式
            //    .Select()
            //    .RowRumberOrderBy((lp, u) => new { lp.EditCount, lp.Name })
            //    .Column((lp, u) => new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName }) //null查询所有字段
            //    .SqlClause(uname)   // 查询cte字段
            //    .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
            //    .Where(where); //(lp, u) => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel || u.UserName == umodel.UserName )
            //                   //.Order((lp, w) => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
            //var result = query.ExecuteQuery();
            //WriteJson(result); //  查询结果

            //// 3表
            //LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            //Users umodel = new Users() { UserName = "jiaojiao" };
            //SynNote snmodel = new SynNote() { Name = "%木头%" };
            //Expression<Func<LockPers_ms, Users_ms, SynNote_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms, Users_ms, SynNote_ms>();
            //where = where.And((lpw, uw, sn) => lpw.Name.Contains(lpmodel.Name));
            //where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
            //where = where.And((lpw, uw, sn) => uw.UserName == umodel.UserName);
            //where = where.And((lpw, uw, sn) => sn.Name.Contains(snmodel.Name));

            //DapperSqlMaker<LockPers_ms, Users_ms, SynNote_ms> query = LockDapperUtilmssql<LockPers_ms, Users_ms, SynNote_ms>
            //    .SqlClaus(withsql)  // 公用表表达式
            //    .Select()
            //    .RowRumberOrderBy((lp, u, sn) => new { x = SM.OrderDesc(lp.EditCount), lp.Name, sn.Content })
            //    .Column((lp, u, s) => //null)  //查询所有字段
            //        new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
            //    .SqlClause(uname)
            //    .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
            //            , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
            //    .Where(where);
            ////.Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });
            //var result = query.ExecuteQuery();
            //WriteJson(result); //  查询结果




            ////5 表
            //LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            //Users umodel = new Users() { UserName = "jiaojiao" };
            //SynNote snmodel = new SynNote() { Name = "%木头%" };
            //Expression<Func<LockPers, Users, SynNote, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote, SynNote>();
            //where = where.And((lpw, uw, sn, snn, s5) => lpw.Name.Contains(lpmodel.Name));
            //where = where.And((lpw, uw, sn, snn, s5) => lpw.IsDel == lpmodel.IsDel);
            //where = where.And((lpw, uw, sn, snn, s5) => uw.UserName == umodel.UserName);
            //where = where.And((lpw, uw, sn, snn, s5) => sn.Name.Contains(snmodel.Name));
            ////  SM.LimitCount,
            //DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote> query = LockDapperUtilmssql<LockPers, Users, SynNote, SynNote, SynNote>
            //    .SqlClaus(withsql)  // 公用表表达式
            //    .Select()
            //    .RowRumberOrderBy((lp, u, s, sn, s5) => new { lp.Id, a = u.Id })
            //    .Column((lp, u, s, sn, s5) =>  // )null查询所有字段
            //        new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
            //    .SqlClause(uname)
            //    .FromJoin(JoinType.Left, (lpp, uu, snn, snnn, s5) => uu.Id == lpp.UserId
            //            , JoinType.Inner, (lpp, uu, snn, snnn, s5) => uu.Id == snn.UserId && snn.Id == snn.UserId
            //            , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == snn.UserId
            //            , JoinType.Inner, (lpp, uu, snn, snnn, s5) => snnn.Id == s5.UserId)
            //    .Where(where); //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
            //                   //.Order((lp, w, sn, snn, s5) => new { lp.EditCount, lp.Name, sn.Content });
            //var result = query.ExecuteQuery();
            //WriteJson(result); //  查询结果

            //LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            //Users umodel = new Users() { UserName = "jiaojiao" };
            //SynNote snmodel = new SynNote() { Name = "%木头%" };
            //Expression<Func<LockPers, Users, SynNote, SynNote, SynNote, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote, SynNote, SynNote, SynNote>();
            //where = where.And((lpw, uw, sn, snn, s5, s6) => lpw.Name.Contains(lpmodel.Name));
            //where = where.And((lpw, uw, sn, snn, s5, s6) => lpw.IsDel == lpmodel.IsDel);
            //where = where.And((lpw, uw, sn, snn, s5, s6) => uw.UserName == umodel.UserName);
            //where = where.And((lpw, uw, sn, snn, s5, s6) => sn.Name.Contains(snmodel.Name));
            ////  SM.LimitCount,
            //DapperSqlMaker<LockPers, Users, SynNote, SynNote, SynNote, SynNote> query = LockDapperUtilmssql<LockPers, Users, SynNote, SynNote, SynNote, SynNote>
            //    .SqlClaus(withsql)  // 公用表表达式
            //    .Select()
            //    .RowRumberOrderBy((lp, u, s, sn, s5, s6) => new { lp.Id, b = SM.OrderDesc(lp.EditCount), a = u.Id })
            //    .Column((lp, u, s, sn, s5, s6) =>  // )null查询所有字段
            //        new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
            //    .SqlClause(uname)
            //    .FromJoin(JoinType.Left, (lpp, uu, snn, snnn, s5, s6) => uu.Id == lpp.UserId
            //            , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => uu.Id == snn.UserId && snn.Id == snn.UserId
            //            , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == snn.UserId
            //            , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => snnn.Id == s5.UserId
            //            , JoinType.Inner, (lpp, uu, snn, snnn, s5, s6) => s5.Id == s6.UserId)
            //    .Where(where); //(lpp, uu, snn, snnn) => uu.Id == snn.UserId && snnn.Id == snn.UserId)//)
            //                   //.Order((lp, w, sn, snn, s5, s6) => new { lp.EditCount, lp.Name, sn.Content });

            //var result = query.ExecuteQuery();
            //WriteJson(result); //  查询结果
            #endregion


        }

        [Test]
        public void Base_Select_Test()
        {
            DBMSSql<LockPers, LockPers>.Select().FromJoin(JoinType.Inner, (a, b) => a.UserId == b.UserId);
            DBMSSql<Skin, Skin>.Select().FromJoin(JoinType.Inner, (a, b) => a.Id == b.Id);
            DBMSSql<LockPers, LockPers>.Select().FromJoin(JoinType.Inner, (a, b) => a.UserId == b.UserId);

            //DapperSqlMaker<LockPers> query = LockDapperUtilmssql<LockPers>

            //LockContext._().Lock1<LockPers>().Select_Test();
            int i = 'a';
            //LockContext._().Lock2<LockPers,Skin>().Select_Test();

        }


        [Test]
        public void where_暂未做解析的方法() {

            var efup = DBSqlite<Users>.Update().EditColumn(p => new bool[] { p.Remark == "木头人xx" }).Where(p => p.UserName.Contains("%木头人%")).ExecuteUpdate();
            Console.WriteLine(efup);
            var efde = DBSqlite<Users>.Delete().Where(p => p.UserName == "木头人3名称必须唯一19/11/3 21:17:42").ExecuteDelete();
            Console.WriteLine(efde);

            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();

            Console.WriteLine("=======");
            Console.WriteLine("=======");
            var query = DBMSSql<LockPers>.Select().Column().From().Where(where).ExecuteQuery();
            WriteJson(query);
            Console.WriteLine("=======");
            Console.WriteLine("=======");
            Console.WriteLine("=======");
            var q2 = DBSqlite<Users>.Select().Column().From().Where(p => p.UserName.Contains("%木头人%") // p.UserName == "木头人1 名称必须唯一" // 
            ).ExecuteQuery();
            WriteJson(q2);

        }

        [Test]
        public void DapperFuncMs类直接执行sql() {
            var r = DapperFuncMs.New.Query<LockPers>("select * from LockPers where Name like @Name", new { Name = "%蛋蛋%" });
            WriteJson(r);
        }

        /// <summary>
        /// where.And 直接拼sql测试
        /// <para>where.And((lpw, uw) =>  SM.Sql(字段1,值变量)   未解析  </para>
        /// <para>以上为where字句中拼sql错误写法 </para>
        /// <para>正确写法为 </para>
        /// <para>where = where.And((a,b) => SM.SQL(" a.IsDel = false ") ); </para> 
        /// </summary>
        [Test]
        public void whereAndTest()
        {
            LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false };
            Users umodel = new Users() { UserName = "jiaojiao" };
            Expression<Func<LockPers, Users, bool>> where = PredicateBuilder.WhereStart<LockPers, Users>();
            //where = where.And((lpw, uw) => lpw.Name.Contains(lpmodel.Name));
            //where = where.And((lpw, uw) => lpw.IsDel == lpmodel.IsDel);
            //where = where.And((lpw, uw) => uw.UserName == umodel.UserName);

            // sm.sql test 
            where = where.And((a,b) => SM.SQL(" a.IsDel = false ") );
            //

            DapperSqlMaker<LockPers, Users> query = DBMSSql<LockPers, Users>
                .Select()
                .RowRumberOrderBy((lp, u) => new { lp.EditCount, lp.Name })
                .Column((lp, u) => new { all = SM.Sql("a.IsDel, a.*") }) //null查询所有字段 lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName , 
                .FromJoin(JoinType.Left, (lpp, uu) => uu.Id == lpp.UserId)
                .Where(where); //(lp, u) => lp.Name == lpmodel.Name && lp.IsDel == lpmodel.IsDel || u.UserName == umodel.UserName )
                               //.Order((lp, w) => new { lp.EditCount, lp.Name }); // .ExecuteQuery();
            //var result = query.ExecuteQuery();
            //WriteJson(result); //  查询结果

            Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
            WriteSqlParams(resultsqlparams);

        }


        ////动态sql
        //#region Dapper已有方法
        //[Test]
        //public void 查询首行()
        //{
        //    SynNote_ sn = new SynNote_();
        //    sn.Id = 3;
        //    string updatesql = " select * from SynNote where \"Id\" = @Id";
        //    var ef = DapperFuncs.New.QueryFirst<SynNote_>(updatesql, sn, LockDapperUtilsqlite.New().GetConnSign(false));
        //    WriteJson(ef);

        //}
        //[Test]
        //public void 查询()
        //{
        //    string updatesql = " select * from SynNote ";
        //    var ef = DapperFuncs.New.Query<SynNote_>(updatesql, null, LockDapperUtilsqlite.New().GetConnSign(false));
        //    WriteJson(ef);
        //}
        //[Test]
        //public void 查询首行首列()
        //{
        //    string updatesql = " select Content from SynNote ";
        //    var ef = DapperFuncs.New.ExecuteScalar<string>(updatesql, null, LockDapperUtilsqlite.New().GetConnSign(false));
        //    WriteJson(ef);
        //}
        //[Test]
        //public void 修改()
        //{
        //    SynNote_ sn = new SynNote_();
        //    sn.Content = "备注333";
        //    sn.IsDel = false;
        //    sn.Name = "棉花多读懂多多多多好多好多好多好多";
        //    sn.NoteDate = DateTime.Now;
        //    sn.UserId = 2;
        //    sn.Id = 3;
        //    string updatesql = "update SynNote set \"Content\" = @Content, \"NoteDate\" = @NoteDate, \"Name\" = @Name, \"UserId\" = @UserId, \"IsDel\" = @IsDel where \"Id\" = @Id";
        //    var ef = DapperFuncs.New.Execute(updatesql, sn, LockDapperUtilsqlite.New().GetConnSign(false));
        //    Console.WriteLine(ef);

        //}


        //#endregion

        [Test]
        public void lock下拉刷新分页测试() {
            //m.IsDel != true  m.Name != "xx"
            string name = "%a%";
            var nobj = new { name = name  };
            int records;
            int page = 2, rows = 20;
            string rownm = " (SELECT COUNT(*) FROM LockPers AS t2  WHERE t2.Name < a.Name ) + (SELECT COUNT(*) FROM LockPers AS t3 WHERE t3.Name = a.Name AND t3.rowid < a.rowid  ) +1 AS rowNum";

            var query = DBSqlite<LockPers>
               .Select().Column(p => new { t = "datetime(a.InsertTime) as InsertTimestr", b = SM.Sql(rownm), p.Id, p.Name, p.Content, p.Prompt, p.EditCount })
               .From().Where(m => m.IsDel != true && m.Name.Contains(nobj.name)).Order(m => new { m.Name });
            //Tuple<StringBuilder, Dapper.DynamicParameters> ru = query.RawSqlParams();
            //var list = query.ExecuteQuery<LockPers>();

            Tuple<StringBuilder, DynamicParameters, StringBuilder> ru = query.RawLimitSqlParams();
            var list = query.LoadPagelt(page, rows, out records);

        }
    }
}
