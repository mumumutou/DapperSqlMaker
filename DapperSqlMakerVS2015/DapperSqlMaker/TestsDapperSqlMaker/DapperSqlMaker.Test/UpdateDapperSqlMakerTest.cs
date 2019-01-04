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
                .Selec().Column().From().Where(p => p.Name == "测试bool修改2 xxxxxx").ExcuteSelect<LockPers>().FirstOrDefault();

            model.Content = "棉花棉花棉花棉花棉花";
            model.ContentOld = "忽略Write(false)标记字段";
            model.Prompt = "xxxxxxxxxxx";

            var issucs = LockDapperUtilsqlite<LockPers>.Cud.Update(model);
            Console.WriteLine(issucs);
             
        }

        #endregion

        #region MS 修改数据测试

        #endregion


        

    }
}
