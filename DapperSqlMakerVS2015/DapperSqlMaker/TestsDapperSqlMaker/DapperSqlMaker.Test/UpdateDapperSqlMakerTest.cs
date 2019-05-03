using DapperSqlMaker;
using DapperSqlMaker.DapperExt;
using FW.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestsDapperSqlMaker.DapperExt
{
    [TestFixture()]
    public class UpdateDapperSqlMakerTest
    {
        #region 链式解析 更新数据

        [Test]
        public void 更新部分字段_含子查询_测试lt()
        {
            string colm = "img", val = "(select value from skin limit 1 offset 1)"; DateTime cdate = DateTime.Now;
            var update = LockDapperUtilsqlite<Users>.Updat().EditColumn(p => new bool[] {
               p.UserName =="几十行代码几十个错 调一步改一步....", p.Password == "bug制造者"
               , p.CreateTime == cdate,  SM.Sql(p.Remark,"(select '奥德赛 终于改好了')")
            }).Where(p => p.Id == 6 && SM.SQL("IsDel == 0"));

            Console.WriteLine(update.RawSqlParams().Item1);
            var efrow = update.ExecuteUpdate();
            Console.WriteLine(efrow);
        }

        #endregion

        #region  一些 Dapper.Contrib改编的方法

        #region SQLite 修改数据测试
        [Test]
        public void 更新部分字段测试lt()
        {
            DapperFuncs.CurtConn = LockDapperUtilsqlite.New().GetConn();
            var issucs = DapperFuncs.Update<LockPers>(
                s =>
                {
                    s._IsWriteFiled = true;
                    s.Name = "测试修改 生成sql回调格式化";
                    s.Content = "66666666";
                    s.IsDel = true;
                },
                w => w.IsDel == true  //w.Name == "测试bool修改1" && 
                );
            Console.WriteLine(issucs);
        }

        [Test]
        public void 更新部分字段2测试lt()
        {
            DapperFuncs.CurtConn = LockDapperUtilsqlite.New().GetConn();
            LockPers set = new LockPers() { Content = "方法外部赋值修改字段实体" };
            set.Name = "测试bool修改2";
            set.IsDel = true;
            set.ContentOld = "忽略Write(false)标记字段";

            var issucs = DapperFuncs.Update<LockPers>(
                set,
                w => w.Name == "测试bool修改2" && w.IsDel == true
                );
            Console.WriteLine(issucs);
        }
        [Test]
        public void 根据主键ID更新整个实体lt()
        {
            DapperFuncs.CurtConn = LockDapperUtilsqlite.New().GetConn();
            var model = LockDapperUtilsqlite<LockPers>
                .Selec().Column().From().Where(p => p.Name == "测试bool修改2 xxxxxx").ExecuteQuery<LockPers>().FirstOrDefault();

            model.Content = "棉花棉花棉花棉花棉花";
            model.ContentOld = "忽略Write(false)标记字段";
            model.Prompt = "xxxxxxxxxxx";

            var issucs = DapperFuncs.Updat<LockPers>(model);
            Console.WriteLine(issucs);

        }

        [Test]
        public void 先查再修改指定字段()
        {
            LockPers p = new LockPers() { Id = "028e7910-6431-4e95-a50f-b9190801933b" };

            var query = LockDapperUtilsqlite<LockPers>
                        .Selec().Column(c => new { c.Content, c.EditCount }).From().Where(m => m.Id == p.Id);

            var old = query.ExecuteQuery<LockPers>().FirstOrDefault();

            old._IsWriteFiled = true; // 标记开始记录赋值字段 注意上面查询LockPers 要再默认构造函数里把 标识改为false 查出的数据不要记录赋值字段 
            old.Name = "蛋蛋蛋蛋H$E22222";
            old.Prompt = "好多多读都多大";
            old.UpdateTime = DateTime.Now;

            var id = old.Id;
            DapperFuncs.CurtConn = LockDapperUtilsqlite.New().GetConn();
            var t = DapperFuncs.Update<LockPers>(old, w => w.Id == p.Id);
        }



        #endregion
        // MS 修改数据测试
        #endregion






    }
}
