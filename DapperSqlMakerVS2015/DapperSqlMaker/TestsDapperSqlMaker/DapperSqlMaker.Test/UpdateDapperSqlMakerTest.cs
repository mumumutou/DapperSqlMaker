using Dapper;
using DapperSqlMaker;
using DapperSqlMaker.DapperExt;
using Esy.Base.Application.Model;
using FW.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace TestsDapperSqlMaker.DapperExt
{
    [TestFixture()]
    public class UpdateDapperSqlMakerTest
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
        #region 链式解析 更新数据

        [Test]
        public void 更新部分字段_含子查询_测试lt()
        {
            var model = new Users() { CreateTime = DateTime.Now };
            string colm = "img", val = "(select value from skin limit 1 offset 1)"; DateTime cdate = DateTime.Now;
            var update = DBSqlite<Users>.Update().EditColumn(p => new bool[] {
               p.UserName =="几十行代码几十个错 调一步改一步....", p.Password == "bug制造者"
               , p.CreateTime == model.CreateTime,  SM.Sql(p.Remark,"(select '奥德赛 终于改好了')")
            }).Where(p => p.Id == 6 && SM.SQL("IsDel == 0"));

            Console.WriteLine(update.RawSqlParams().Item1);
            var efrow = update.ExecuteUpdate();
            Console.WriteLine(efrow);
        }
        /// <summary>
        /// update 拼sql
        /// </summary>
        [Test]
        public void EditColumnSqlTest() {
            string colm = "img", val = "(select value from skin limit 1 offset 1)"; DateTime cdate = DateTime.Now;
            var update = DBSqlite<Users>.Update().EditColumn(p => new bool[] {
                 SM.Sql(colm,val), SM.Sql(p.Remark,"(select '奥德赛 终于改好了')")
            }).Where(p => p.Id == 6 && SM.SQL("IsDel == 0"));
            WriteSqlParams(update.RawSqlParams());
        }

        [Test]
        public void 修改字段_多次拼接_20_6_5() { 

            string colm = "img", val = "(select value from skin limit 1 offset 1)"; DateTime cdate = DateTime.Now;
            var update = DBSqlite<Users>.Update()
                .EditColumn(p => new bool[] { p.UserName == "123" })
                .EditColumn(p => new bool[] { SM.Sql(colm, val) })
                .EditColumn(p => new bool[] { SM.Sql(p.Remark, "(select '奥德赛 终于改好了')") })
                .EditColumn(p => new bool[] { p.numx == 123 })
                //.EditColumnEnd()
                .Where(p => p.Id == 6 && SM.SQL("IsDel == 0"));
            WriteSqlParams(update.RawSqlParams());

            /*
测试名称:	动态拼修改的字段_20_6_5
测试结果:	已通过
结果 的标准输出:	
update Users set  UserName=@UserName1_  , img=(select value from skin limit 1 offset 1) , Remark=(select '奥德赛 终于改好了') ,  numx=@numx1_  where  Id = @Id0  and IsDel == 0
"UserName1_ -- \"123\""
"numx1_ -- 123.0"
"Id0 -- 6"
             */
        }

        [Test]
        public void 根据条件_动态拼修改的字段() {
            DateTime cdate = DateTime.Now; ;
            Expression<Func<Users, bool[]>> xxexp = p => new bool[] {
               p.UserName =="几十行....", p.Password == "bug制造者"
               , p.CreateTime == cdate,  SM.Sql(p.Remark,"(select '奥德赛 终于改好了')")
            };

            Expression<Func<Users, bool>>[] xxp =
                {
                p => p.UserName == p.UserName,
                p => p.Remark == p.Remark,
            };
                


            var lambdap = Expression.Parameter(typeof(Users), "p");
            //var lambdamod = Expression.MakeMemberAccess(Expression.Constant("测试12",typeof(string)));
            var leftmb= Expression.Property(lambdap, "UserName");
            var rightmb = Expression.Property(Expression.Property(Expression.Constant(DateTime.Now), typeof(Users).GetProperty("UserName")), typeof(Users).GetProperty("UserName"));
            var eqexp = Expression.Equal(leftmb,rightmb);

            var lambdamod2 = Expression.MakeMemberAccess(Expression.Constant(DateTime.Now), typeof(Users).GetMember("CreateTime")[0]);
            var leftmb2 = Expression.MakeMemberAccess(lambdap, typeof(Users).GetMember("CreateTime")[0]);
            var rightmb2 = Expression.MakeMemberAccess(lambdamod2, typeof(Users).GetMember("CreateTime")[0]);
            var eqexp2 = Expression.Equal(leftmb2, rightmb2);
            var exparrss = NewArrayExpression.NewArrayInit(typeof(bool), eqexp, eqexp2);

            LambdaExpression lambdaExpr = Expression.Lambda(exparrss, new List<ParameterExpression>() { lambdap });
            Console.WriteLine(lambdaExpr);// arg => (arg +1)

            var func = lambdaExpr.Compile() as Func<Users,bool[]>;
            var resu = func(new Users() { UserName = "1" }); //, new Users());

            // ################
            ////BinaryExpression.
            //var x = Expression.Equal(Expression.MakeMemberAccess(Expression.Variable(typeof(Users), "UserName"), typeof(Users).GetMember("UserName")[0])
            //    , Expression.MakeMemberAccess(Expression.Variable(typeof(Users), "UserName"), typeof(Users).GetMember("UserName")[0]));

            //LambdaExpression lambdaExpr = Expression.Lambda(Expression.Add(paramExpr, Expression.Constant(1)), new List<ParameterExpression>() { paramExpr });
            //Console.WriteLine(lambdaExpr);// arg => (arg +1)


            //List <bool> edcolms = new List<bool>();
            ////Expression.Constant(" "); // 常量表达式

            //DateTime cdatec = DateTime.Now;
            //Users model = new Users() { UserName="数组表达式构建", CreateTime = cdate };

            ////BinaryExpression.New()

            ////NewArrayExpression.New(typeof(Func<Users,bool>), )

            //List<Expression> exparrxx = new List<Expression>();
            ////exparrxx.Add(, ));

            //List<Expression<Func<Users, bool>>> exparr = new List<Expression<Func<Users, bool>>>();
            //exparr.Add( p => p.UserName == model.UserName );
            //exparr.Add( p => p.CreateTime == model.CreateTime );

            //var arrexp =  NewArrayExpression.NewArrayInit(typeof(bool), exparr);


            //var exparrs = Expression.NewArrayInit(typeof(bool), exparr);

            //LockDapperUtilsqlite<Users>.Update().EditColumn( )

            // ###########
            //把editcolumn改为 where == 形式才能 随意拼接

        }

        [Test]
        public void Update_Output子句测试() {

            var edit = EsyDb<EsySysUser>.Update().EditColum(x => new bool[] { x.F_Account == "123" }).SqlClause("output Inserted.F_Name where");
            var query = EsyDb<EsySysUser>.Insert().AddColumn(x=>new bool[] { x.F_Account == "123" })
                .EditClause(ClauseType.AddColumn, "values", " output Inserted.F_Name values ");
            //query.ExecuteQuery();
            WriteSqlParams(query.RawSqlParams());

          EsyDb<EsySysUser>.Delete().EditClause(ClauseType.Delete, "values", " output Inserted.F_Name values ");//.ExecuteDelete(); 

        }


        [Test]
        public void 事务更新测试ms()
        {
            var str = DateTime.Now.ToString();
            Console.WriteLine(str);
            var update = DBMSSql<Users>.Update().EditColumn(p => new bool[] { p.UserName == "事务修改 第一条语句", p.Password == str })
                .Where(p => p.Id == 4 && SM.SQL(" IsDel = 'false' "));
            var update2 = DBMSSql<Users>.Update().EditColumn(p => new bool[] { p.UserName == "xxxxx 2 sql事务修改", p.Password == str })
                .Where(p => p.Id == 6 && SM.SQL(" IsDel = 'false' "));

            Console.WriteLine(update.RawSqlParams().Item1);
            Console.WriteLine(update2.RawSqlParams().Item1);


            using (var conn = update.GetConn())
            {
                var trans = conn.BeginTransaction();
                try
                {
                    var efrow = conn.Execute(update.RawSqlParams().Item1.ToString(), update.RawSqlParams().Item2, trans);
                    Console.WriteLine(efrow + "第一执行了");

                    //throw new Exception("机房爆炸了");
                    var efrow2 = conn.Execute(update2.RawSqlParams().Item1.ToString(), update2.RawSqlParams().Item2, trans);
                    Console.WriteLine(efrow2 + "第2执行了");

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine(ex.Message);
                }

            }


            //var efrow = update.ExecuteUpdate();

            var rows = DBMSSql<Users>.Select().Column().From().Where(p => p.Id == 4 || p.Id == 6).ExecuteQuery();
            WriteJson(rows);

        }
        /// <summary>
        /// <para> 
        /// short值bool运算时会转为int类型 
        /// 表达式为 Convert(value) == Convert(value) 形式
        /// 所以所有short字段都用int表示
        /// 或者直接sql写入  SM.Sql(p.Gen," 1 ")
        /// </para>
        /// </summary>
        [Test]
        public void EditShortTest() {
            short stnum = 1;
            var edit = DBSqlite<EsySysUser_>.Update().EditColumn(p => new bool[] { p.Gen == stnum,
                 SM.Sql(p.Gen," 1 "), p.F_UserId == "001" }).Where(p => p.F_UserId == "001");

            var rawsp = edit.RawSqlParams();
            WriteSqlParams(rawsp);
            
            //.ExecuteUpdate();

        }

        #endregion

        #region  一些 Dapper.Contrib改编的方法

        #region SQLite 修改数据测试
        [Test]
        public void 更新部分字段测试lt()
        {
            
            var issucs = DBSqliteFuncs.New.Update<LockPers>(
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
            
            LockPers set = new LockPers() {_IsWriteFiled = true, Content = "方法外部赋值修改字段实体" };
            set.Name = "测试bool修改2";
            set.IsDel = true;
            set.ContentOld = "忽略Write(false)标记字段";

            var issucs = DBSqliteFuncs.New.Update<LockPers>(
                set,
                w => w.Name == "测试bool修改2" && w.IsDel == true
                );
            Console.WriteLine(issucs);
        }
        [Test]
        public void 根据主键ID更新整个实体lt()
        {
            
            var model = DBSqlite<LockPers>
                .Select().Column().From().Where(p => p.Name == "测试bool修改2 xxxxxx").ExecuteQuery<LockPers>().FirstOrDefault();

            model.Content = "棉花棉花棉花棉花棉花";
            model.ContentOld = "忽略Write(false)标记字段";
            model.Prompt = "xxxxxxxxxxx";

            var issucs = DBSqliteFuncs.New.Updat<LockPers>(model);
            Console.WriteLine(issucs);

        }

        [Test]
        public void 先查再修改指定字段()
        {
            LockPers p = new LockPers() { Id = "028e7910-6431-4e95-a50f-b9190801933b" };

            var query = DBSqlite<LockPers>
                        .Select().Column(c => new { c.Content, c.EditCount }).From().Where(m => m.Id == p.Id);

            var old = query.ExecuteQuery<LockPers>().FirstOrDefault();

            old._IsWriteFiled = true; // 标记开始记录赋值字段 注意上面查询LockPers 要再默认构造函数里把 标识改为false 查出的数据不要记录赋值字段 
            old.Name = "蛋蛋蛋蛋H$E22222";
            old.Prompt = "好多多读都多大";
            old.UpdateTime = DateTime.Now;

            var id = old.Id;
            
            var t = DBSqliteFuncs.New.Update<LockPers>(old, w => w.Id == p.Id);
        }



        #endregion
        // MS 修改数据测试
        #endregion






    }
}
