using DapperSqlMaker;
using DapperSqlMaker.DapperExt;
using FW.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TestsDapperSqlMaker.DapperExt
{
    [TestFixture()]
    public class DeleteDapperSqlMakerTest
    {
        #region 链式解析 删除
        [Test]
        public void 删除数据_含子查询_测试lt() {
            var sql = " UserId = ( select Id from users  where UserName = '木头人1' )";
            var delete = LockDapperUtilsqlite<LockPers>.Delet().Where(p => 
                    p.Name == "H$E22222" && SM.SQL(sql) && SM.SQL(" IsDel = '1' "));
            var efrow = delete.ExecuteDelete();
            Console.WriteLine(efrow);
            Console.WriteLine(delete.RawSqlParams().Item1);
        }
        #endregion

        #region SQLite 修改数据测试

        [Test]
        public void 删除测试_lt()
        {
            DapperFuncs.CurtConn = LockDapperUtilsqlite.New().GetConn();
            var Name = "mssqlmmmmmmmx1";
            var issucs = DapperFuncs .Delete<LockPers>(w => w.Name == Name && w.IsDel == true);
            Console.WriteLine(issucs);
        }


        [Test]
        public void 拼接条件删除数据_lt()
        {

            var Name = "测试修改 生成sql回调格式化";
            var Name2 = "mssqlmmmmmmmx2222222222";

            DapperFuncs.CurtConn = LockDapperUtilsqlite.New().GetConn();
            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();
            where = where.Or(p => p.Name == Name);
            where = where.Or(p => p.Name == Name2);
            where = where.And(p => p.UserId == 3);

            var issucs = DapperFuncs .Deleters<LockPers>(where);
            Console.WriteLine(issucs);
        }

        #endregion


        #region MSSQL 修改数据测试

        [Test]
        public void 拼接条件删除数据_ms()
        {

            var Name = "mssqlmmmmmmmx1";
            var Name2 = "mssqlmmmmmmmx2222222222";

            DapperFuncs.CurtConn = LockDapperUtilsqlite.New().GetConn();
            Expression<Func<LockPers_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms>();
            where = where.Or(p => p.Name == Name);
            where = where.Or(p => p.Name == Name2);
            where = where.And(p => p.UserId == 3);

            var issucs = DapperFuncs.Delete<LockPers_ms>(where);
            Console.WriteLine(issucs);
        }

        #endregion




    }
}
