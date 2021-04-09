using Dapper;
using DapperSqlMaker;
using DapperSqlMaker.DapperExt;
using FW.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

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
        // 打印sql和参数
        private static void WriteSqlParams(Tuple<StringBuilder, DynamicParameters> resultsqlparams)
        {
            Console.WriteLine(resultsqlparams.Item1.ToString()); // sql
            foreach (var name in resultsqlparams.Item2.ParameterNames)
            {
                WriteJson(name + " -- " + Newtonsoft.Json.JsonConvert.SerializeObject(resultsqlparams.Item2.Get<object>(name))); // 参数 -- 值
            }
        }

        #region 链式解析 添加数据

        [Test]
        public void 添加部分字段_含子查询_测试lt()
        {
            var name = "木头人3名称必须唯一" + DateTime.Now.ToString();
            string colm = "img", val = "(select value from skin limit 1 offset 1)"; DateTime cdate = DateTime.Now;
            var insert = DBSqlite<Users>.Insert().AddColumn(p => new bool[] {
                p.UserName == name, p.Password == "666", p.CreateTime == cdate
                , SM.Sql(colm,val), SM.Sql(p.Remark,"(select '荒野高尔夫')")
            });
             
            Console.WriteLine(insert.RawSqlParams().Item1);
            var efrow = insert.ExecuteInsert();
            Console.WriteLine(efrow);
            string guid = Guid.NewGuid().ToString();
            var efrow2 = DBSqlite<LockPers>.Insert().AddColumn(p => new bool[] {
                p.Id            ==   guid,
                p.Name          == "木头人1"  ,
                p.Content       == "这是棉花好多好多"         ,
                p.InsertTime    == DateTime.Now               ,
                p.IsDel         == false                      ,
                p.UserId        == 3

            }).ExecuteInsert();
            Console.WriteLine(efrow2);

        } 
        /// <summary>
        /// 直接拼sql测试
        /// </summary>
        [Test]
        public void AddColumnSqlTest() {
            string colm = "img", val = "(select value from skin limit 1 offset 1)";
            var insert = DBSqlite<Users>.Insert().AddColumn(p => new bool[] {
                 SM.Sql(colm,val), SM.Sql(p.Remark,"(select '荒野高尔夫')"), 
            });

            WriteSqlParams(insert.RawSqlParams());


        }
        [Test]
        public void 动态添加不同字段测试() {
            /*
测试名称:	动态添加不同字段测试
测试结果:	已通过
结果 的标准输出:	insert into Users (img,Remark) values ((select value from skin limit 1 offset 1),(select '荒野高尔夫'))  (age) values ((select 1))

            问题   把多个AddColumn合并到1个
            将AddColumn里的sql和parms添加到 addList集合中 最后AddColumnEnd子句中执行 字段和values的join 
 */

            string colm = "img", val = "(select value from skin limit 1 offset 1)";
            var insert = DBSqlite<Users>.Insert().AddColumn(p => new bool[] {
                 SM.Sql(colm,val), SM.Sql(p.Remark,"(select '荒野高尔夫')"),
            })
            .AddColumn(p => new bool[] { SM.Sql("age", "(select 1)") });

            WriteSqlParams(insert.RawSqlParams());

        }

        #endregion


        #region 一些 Dapper.Contrib改编的方法


        #region SQLite 添加数据测试
        [Test]
        public void DapperContrib添加方法()
        {
            
            Skin additem = new Skin() { InsertDate = DateTime.Now.ToString(), Name = "奥的阿三", Remake = "背景", Type = "bg", Value = "www.baidu.com" };
            int efrow = DBSqliteFuncs.New.Inser<Skin>(additem);
            Console.WriteLine("影响行数-" + efrow);
        }

        [Test]
        public void 添加部分字段_返回影响行_测试lt()
        {
            
            var efrow = DBSqliteFuncs.New.Insert<LockPers>(p =>
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
            var efrow = DBSqliteFuncs.New.InsertGetId<LockPers>(p =>
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
            var efrow = DBSqliteFuncs.New.Inser<LockPers_ms>(item
                );
            Console.WriteLine("影响行数-" + efrow);
        }
        [Test]
        public void 添加数据返回影响行数_测试MS()
        {
            
            return;
            var efrow = DBSqliteFuncs.New.Insert<LockPers_ms>(p =>
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
            

            var id = DBSqliteFuncs.New.InsertGetId<LockPers_ms>(p =>
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
        #endregion

        [Test]
        public void 批量插入测试()
        {   
            //return;  // sqlite 读取 批量插入到 mssql
            

            var list3old = DBSqliteFuncs.New.GetAll<LockPers_>();
            List<LockPers_ms_> list3new = new List<LockPers_ms_>();
            foreach (var item in list3old)
            {
                //var model = CopyModelHelper.MapperWrite<LockPers_ms, LockPers>(item);

                list3new.Add(CopyModelHelper.Mapper<LockPers_ms_, LockPers_>(item));
            }
            var efrow = DapperFuncMs.New.InserList<LockPers_ms_>(list3new);
            Console.WriteLine("影响行数-" + efrow);

            return;

            var list2old = DBSqliteFuncs.New.GetAll<SynNote>();
            foreach (var item in list2old)
            {
                var model = CopyModelHelper.Mapper<SynNote_ms, SynNote>(item);
                DapperFuncMs.New.Insert<SynNote_ms>(model);
                //newlist.Add ( );
            }

            var list1old = DBSqliteFuncs.New.GetAll<Users>();
            foreach (var item in list1old)
            {
                var model = CopyModelHelper.Mapper<Users_ms, Users>(item);
                DapperFuncMs.New.Insert<Users_ms>(model);
            }
        }

        [Test]
        public int Dapper_Contrib插入语句测试示例()
        {
             
            using (var conn = DBSqliteFuncs.New.GetConn()) // var = GetConn())
            {
                System.Data.IDbTransaction transaction = null;
                int? commandTimeout = null;
                var entity = new SynNote();
                // using Dapper.Contrib.Extensions;
                var t = conn.Inser(entity, false, transaction, commandTimeout);
                return (int)t;
            }

        }




    }
}
