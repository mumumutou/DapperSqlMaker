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
    public class InsertDapperSqlMakerTest
    {
        private static void WriteJson(object test2)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(test2);
            Console.WriteLine(str);
        } 

        #region SQLite 添加数据测试

        [Test]
        public void 添加数据返回影响行数测试lt()
        {
            var efrow = LockDapperUtilsqlite<LockPers>.Cud.Insert(p =>
            {
                p.Id = Guid.NewGuid().ToString();
                p.Name = "mssqlmmmmmmmx2222222222";
                p.Content = "这是棉花好多好多";
                p.InsertTime = DateTime.Now;
                p.IsDel = false;
                p.UserId = 3;
            });
            Console.WriteLine("影响行数-" + efrow );
        }

        [Test]
        public void 添加数据返回插入ID测试lt() {

            return;
            var efrow = LockDapperUtilsqlite<LockPers>.Cud.InsertGetId(p =>
            {
                p.Id = Guid.NewGuid().ToString();
                p.Name = "测试bool添加";
                p.Content = p.Name;
                p.InsertTime = DateTime.Now;
                p.IsDel = false;
            });
            Console.WriteLine("影响行数-" + efrow);

        }

        #endregion


        #region MS 添加数据测试

        [Test]
        public void 添加数据返回影响行数_测试MS() {
            return;
            var efrow = LockDapperUtilmssql<LockPers_ms>.Cud.Insert(p =>
            {
                p.Id = Guid.NewGuid().ToString();
                p.Name = "mssqlmmmmmmmx2222222222";
                p.Content = "这是棉花好多好多";
                p.InsertTime = DateTime.Now;
                p.IsDel = false;
                p.UserId = 3;
            });
            Console.WriteLine("影响行数-" + efrow);
        }
        [Test]
        public void 添加数据返回插入ID_测试MS()
        {
            //return;

            var id = LockDapperUtilmssql<LockPers_ms>.Cud.InsertGetId(p =>
            {
                p.Id = Guid.NewGuid().ToString();
                p.Name = "mssqlmmmmmmmx1";
                p.Content = p.Name;
                p.InsertTime = DateTime.Now;
                p.IsDel = false;
            });

            Console.WriteLine("插入数据ID-" + id);

            //var sid =LockDapperUtilmssql<SynNote_ms>.Cud.InsertGetId( p => {
            //    p.Name = "mssqlmmmmmmmx44444444";
            //    p.Content = p.Name;
            //    p.NoteDate = DateTime.Now;
            //    p.IsDel = false;
            //    p.UserId = 3;
            //});

            //var uid = LockDapperUtilmssql<Users_ms>.Cud.InsertGetId( p => {
            //    p.UserName = "mssqlmmmmmmmmmmx111111";
            //    p.Password = p.UserName;
            //    p.CreateTime = DateTime.Now;
            //    p.IsDel = false;
            //});



        }



        #endregion

        [Test]
        public void 批量插入测试()
        {
            //return;

            var list3old = LockDapperUtilsqlite<LockPers_>.Cud.GetAll();
            List<LockPers_ms_> list3new = new List<LockPers_ms_>();
            foreach (var item in list3old)
            {
                //var model = CopyModelHelper.MapperWrite<LockPers_ms, LockPers>(item);

                list3new.Add(CopyModelHelper.Mapper<LockPers_ms_, LockPers_>(item));
            }
            var efrow = LockDapperUtilmssql<LockPers_ms_>.Cud.InsertList(list3new);
            Console.WriteLine("影响行数-" + efrow);

            return;

            var list2old = LockDapperUtilsqlite<SynNote>.Cud.GetAll();
            foreach (var item in list2old)
            {
                var model = CopyModelHelper.MapperWrite<SynNote_ms, SynNote>(item);
                LockDapperUtilmssql<SynNote_ms>.Cud.Insert(model);
                //newlist.Add ( );
            }

            var list1old = LockDapperUtilsqlite<Users>.Cud.GetAll();
            foreach (var item in list1old)
            {
                var model = CopyModelHelper.MapperWrite<Users_ms, Users>(item);
                LockDapperUtilmssql<Users_ms>.Cud.Insert(model);
            }
        }

    }
}
