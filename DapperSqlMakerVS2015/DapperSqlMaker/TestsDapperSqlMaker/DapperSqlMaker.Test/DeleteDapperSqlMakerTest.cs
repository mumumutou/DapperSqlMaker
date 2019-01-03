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


        #region SQLite 修改数据测试

        [Test]
        public void 删除测试_lt()
        {
            var Name = "mssqlmmmmmmmx1";
            var issucs = LockDapperUtilsqlite<LockPers>.Cud.Delete( w => w.Name == Name && w.IsDel == true  );
            Console.WriteLine(issucs);
        }


        [Test]
        public void 拼接条件删除数据_lt() {

            var Name = "mssqlmmmmmmmx1";
            var Name2 = "mssqlmmmmmmmx2222222222";

            Expression<Func<LockPers, bool>> where = PredicateBuilder.WhereStart<LockPers>();
            where = where.Or( p => p.Name == Name);
            where = where.Or( p => p.Name == Name2);
            where = where.And( p => p.UserId == 3);

            var issucs = LockDapperUtilsqlite<LockPers>.Cud.Delete(where);
            Console.WriteLine(issucs);
        }

        #endregion


        #region MSSQL 修改数据测试

        [Test]
        public void 拼接条件删除数据_ms()
        {

            var Name = "mssqlmmmmmmmx1";
            var Name2 = "mssqlmmmmmmmx2222222222";

            Expression<Func<LockPers_ms, bool>> where = PredicateBuilder.WhereStart<LockPers_ms>();
            where = where.Or(p => p.Name == Name);
            where = where.Or(p => p.Name == Name2);
            where = where.And(p => p.UserId == 3);

            var issucs = LockDapperUtilmssql<LockPers_ms>.Cud.Delete(where);
            Console.WriteLine(issucs);
        }

        #endregion




    }
}
