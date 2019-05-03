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
        public void DapperContrib添加方法()
        {
            Skin additem = new Skin() { InsertDate = DateTime.Now.ToString(), Name = "奥的阿三", Remake = "背景", Type = "bg", Value = "www.baidu.com" };
            int efrow = LockDapperUtilsqlite<Skin>.Cud.Inser(additem);
            Console.WriteLine("影响行数-" + efrow);
        }

        [Test]
        public void 添加部分字段_返回影响行_测试lt()
        {
            var efrow = LockDapperUtilsqlite<LockPers>.Cud.Insert(p =>
            {
                p._IsWriteFiled = true;
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
        public void 添加部分字段_返回插入ID_测试lt()
        {

            return;
            var efrow = LockDapperUtilsqlite<LockPers>.Cud.InsertGetId(p =>
            {
                p._IsWriteFiled = true;
                p.Id = Guid.NewGuid().ToString();
                p.Name = "测试bool添加";
                p.Content = p.Name;
                p.InsertTime = DateTime.Now;
                p.IsDel = false;
            });
            Console.WriteLine("影响行数-" + efrow);

        }

        [Test]
        public void 添加部分字段和子查询_测试lt()
        {
            string colm1 = "remake", val1 = " (select '子查询和注入的sql语句') ";
            var datestr = DateTime.Now.ToString("yyMMdd HHmmss");
            var query = LockDapperUtilsqlite<Skin>.Inser().AddColumn(p => new bool[] {
                SM.Sql(colm1, val1), SM.Sql(p.Name, " (select '宽叶飙车9facebook改写修改的了') "),
                p.UserId == 1
                , p.Value == "www.baidu.com", p.Type == "bg", p.InsertDate == datestr
                 });
            var sqlparms = query.RawSqlParams();
            Console.WriteLine(sqlparms.Item1);
            var ew = query.ExecuteInsert();
            Console.WriteLine(ew);
            return;

        }

        [Test]
        public void 添加部分字段和子查询_测试lt2() {
            string colm = "img", val = "(select value from skin limit 1 offset 1)"; DateTime cdate = DateTime.Now;
            var insert = LockDapperUtilsqlite<Users>.Inser().AddColumn(p => new bool[] {
                p.UserName =="木头人1", p.Password == "666", p.CreateTime == cdate
                , SM.Sql(colm,val), SM.Sql(p.Remark,"(select '荒野高尔夫')")
            }); 
            var efrow = insert.ExecuteInsert();
            Console.WriteLine(efrow + " " + insert.RawSqlParams().Item1);
        }
        #endregion


        #region MS 添加数据测试

        [Test]
        public void DapperContrib添加方法MS()
        {
            //return;
            LockPers_ms item = new LockPers_ms(true)
            {
                Id = Guid.NewGuid().ToString(),
                Name = "mssqlmmmmmmmx2222222222",
                Content = "这是棉花好多好多",
                InsertTime = DateTime.Now,
                IsDel = false,
                UserId = 3
            };
            var efrow = LockDapperUtilmssql<LockPers_ms>.Cud.Inser(item
                );
            Console.WriteLine("影响行数-" + efrow);
        }
        [Test]
        public void 添加数据返回影响行数_测试MS()
        {
            return;
            var efrow = LockDapperUtilmssql<LockPers_ms>.Cud.Insert(p =>
            {
                p._IsWriteFiled = true;
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
                p._IsWriteFiled = true;
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
            var efrow = LockDapperUtilmssql<LockPers_ms_>.Cud.InserList(list3new);
            Console.WriteLine("影响行数-" + efrow);

            return;

            var list2old = LockDapperUtilsqlite<SynNote>.Cud.GetAll();
            foreach (var item in list2old)
            {
                var model = CopyModelHelper.Mapper<SynNote_ms, SynNote>(item);
                LockDapperUtilmssql<SynNote_ms>.Cud.Insert(model);
                //newlist.Add ( );
            }

            var list1old = LockDapperUtilsqlite<Users>.Cud.GetAll();
            foreach (var item in list1old)
            {
                var model = CopyModelHelper.Mapper<Users_ms, Users>(item);
                LockDapperUtilmssql<Users_ms>.Cud.Insert(model);
            }
        }

    }
}
