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

        #region SQLite 修改数据测试
        [Test]
        public void 更新部分字段测试lt()
        {
            var issucs = LockDapperUtilsqlite<LockPers>.Cud.Update(
                s =>
                {
                    s.Name = "测试bool修改1";
                    s.Content = "update方法内赋值修改字段";
                    s.IsDel = true;
                },
                w => w.Name == "测试bool修改1" && w.IsDel == true
                );
            Console.WriteLine(issucs);
        }

        [Test]
        public void 更新部分字段2测试lt()
        {
            LockPers set = new LockPers() { Content = "方法外部赋值修改字段实体" };
            set.Name = "测试bool修改2";
            set.IsDel = true;
            set.ContentOld = "忽略Write(false)标记字段";

            var issucs = LockDapperUtilsqlite<LockPers>.Cud.Update(
                set,
                w => w.Name == "测试bool修改2" && w.IsDel == true
                );
            Console.WriteLine(issucs);
        }
        [Test]
        public void 根据主键ID更新整个实体lt()
        {

            var model = LockDapperUtilsqlite<LockPers>
                .Selec().Column().From().Where(p => p.Name == "测试bool修改2 xxxxxx").ExcuteQuery<LockPers>().FirstOrDefault();

            model.Content = "棉花棉花棉花棉花棉花";
            model.ContentOld = "忽略Write(false)标记字段";
            model.Prompt = "xxxxxxxxxxx";

            var issucs = LockDapperUtilsqlite<LockPers>.Cud.Updat(model);
            Console.WriteLine(issucs);
             
        }

        [Test]
        public void 先查再修改指定字段()
        {
            LockPers p = new LockPers() {  Id = "028e7910-6431-4e95-a50f-b9190801933b" };

            var query = LockDapperUtilsqlite<LockPers>
                        .Selec().Column(c => new { c.Content, c.EditCount }).From().Where(m => m.Id == p.Id);

            var old = query.ExcuteQuery<LockPers>().FirstOrDefault();

            old._IsWriteFiled = true; // 标记开始记录赋值字段 注意上面查询LockPers 要再默认构造函数里把 标识改为false 查出的数据不要记录赋值字段 
            old.Name = "蛋蛋蛋蛋H$E22222";
            old.Prompt = "好多多读都多大";
            old.UpdateTime = DateTime.Now;

            var id = old.Id;
            var t = LockDapperUtilsqlite<LockPers>.Cud.Update(old, w => w.Id == p.Id);
        }

        [Test]
        public void 更新传入int变量测速() {
            TestIntAction(7);

        }

        public void TestIntAction(int Id)
        {
            int UserIds = 1;
            bool isSuccess = LockDapperUtilsqlite<Skin>.Cud.Update(s => {
                s._IsWriteFiled = true; s.IsDel = 1;
            }, w => w.Id == Id && w.UserId == UserIds);
            Console.WriteLine(isSuccess);
        }
        #endregion

        #region MS 修改数据测试

        #endregion


        

    }
}
