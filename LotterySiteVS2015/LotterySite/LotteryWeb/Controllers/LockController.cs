using Dapper;
using Dapper.Contrib.Extensions;
using FW.Common;
using FW.Common.DapperExt;
using FW.Model;
using QnCmsData.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using xxoo.Common;
using static Dapper.SqlMapper;

namespace LotteryWeb.Controllers
{
    public class LockController : Controller
    {
        //
        // GET: /Lock/
        // 0 false 1 true 

        public ActionResult Index()
        {

            string sql = string.Format("SELECT * FROM LockPers where isDel != 1  order by Name ");
            DataTable dt = LockSQLitehelper.ExecuteTable(sql);
            ViewData.Model = DataToModelHelper.RefDataTableToList<LockPers>(dt);
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            return View();
        }

        public ActionResult Check(LockPers p)
        {
            string sql = string.Format("SELECT Content FROM LockPers where Id = @Id");
            var content = LockSQLitehelper.ExecuteScalar(sql, new SQLiteParameter("@Id", DbType.String) { Value = p.Id }) + "";

            var encstr = LockEncrypt.StringToMD5(p.Content);

            return Content(Convert.ToInt32(content.Equals(encstr)).ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> -1 旧内容不对</returns>
        public ActionResult AddUpdate(LockPers p)
        {
            if (p.Id == "-1")
            { // 添加

                string sql = "insert into LockPers(Id,Name,Content,Prompt,InsertTime) values(@Id,@Name,@Content,@Prompt,@InsertTime)";
                p.Id = Guid.NewGuid().ToString();
                p.Content = LockEncrypt.StringToMD5(p.Content);
                p.InsertTime = DateTime.Now;
                var efrwos = 0;
                using (IDbConnection conn = DataBaseConfig.GetSqliteConnection(DataBaseConfig.LockSqlLiteConnectionString))
                {
                    efrwos = conn.Execute(sql, p);
                }

                return Content(efrwos.ToString());

            }
            else
            { // 修改
                var old = LockDapperUtil.Get<LockPers>(p.Id);
                if (old.Content != LockEncrypt.StringToMD5(p.ContentOld))
                { // 旧内容不一致
                    return Content("-1");  // 旧内容不一致
                }
                old.Name = p.Name;
                old.Content = LockEncrypt.StringToMD5(p.Content);
                old.Prompt = p.Prompt;
                old.UpdateTime = DateTime.Now;
                var t = LockDapperUtil.Update(old);

                return Content(Convert.ToInt32(t).ToString());
            }
            // 
        }

        public ActionResult Delete(string Id)
        {
            var old = LockDapperUtil.Get<LockPers>(Id);
            old.IsDel = true;
            old.DelTime = DateTime.Now;
            var t = LockDapperUtil.Update(old);
            return Content(Convert.ToInt32(t).ToString());

            ////string sql = string.Format(" delete from LockPers where Id = @Id ");
            //string sql = string.Format(" update LockPers set IsDel = 1 where Id = @Id ");
            //var isSuccess = LockSQLitehelper.ExecuteNonQuery(sql, new SQLiteParameter("@Id", DbType.String) { Value = Id }) > 0;
            //return Content(Convert.ToInt32(isSuccess).ToString());
        }


        public void delinsert()
        {
            

            // 2. Update (跟新部分字段 set和where里不能有相同字段)
            LockPers pset = new LockPers(true);
            pset.Name = "修改95 只修改Name字段";
            LockPers pwhere = new LockPers(true);
            pwhere.Content = "7fa867c5b404547797614abe57341844";
            //var efrwostest2 = LockDapperUtil.Update<LockPers>(pset, pwhere);

            // 4. Delete 
            LockPers pdel = new LockPers(true);
            pdel.Id = "1339a621-09f5-4d44-8653-09354013c3c0";
            pdel.IsDel = true;
            //var efrowsdel = LockDapperUtil.Delete(pdel);

            // 6. Update
            //var efrowsupdate2 = LockDapperUtil<LockPers>.Update(
            //set =>
            //{
            //    set.Name = "修改95 修改Name和Content字段";
            //    set.Prompt = "BMWWWWWWWWWWWWWWW";
            //}
            //, where => where.Content = "xxxxxxxoooooooo");

            // 7. Delete
            //var efrowsdelete = LockDapperUtil<LockPers>.Delete(where =>
            //{
            //    where.Id = "3e478c04-5e5b-41a8-af7a-5185cc141618";
            //    where.IsDel = "1";
            //});

        }

        public void insert()
        {
            // 1. Add
            //LockPers padd = new LockPers(true);
            //padd.Id = Guid.NewGuid().ToString();
            //padd.InsertTime = DateTime.Now;
            //padd.IsDel = "1";
            //padd.Content = "xxxxxxxoooooooo";
            //padd.Name = "913 添加数据";
            //var efrowsadd = LockDapperUtil.Insert<LockPers>(padd);

            // 3. Update Amobject  批量插入/修改/删除 ... 
            // string sqlamobject = "update LockPers set IsDel = @IsDelN where IsDel = @IsDel ";
            // var efrows = LockDapperUtil.Execute(sqlamobject, new { IsDelN = "0", IsDel = "False" });

            // -----------------------------------------------------------------------------
            // 5.Add
            //var efrowsadd2 = LockDapperUtil<LockPers>.Insert(
            //filed =>
            //{ 
            //    filed.Id = Guid.NewGuid().ToString();
            //    filed.InsertTime = DateTime.Now;
            //    filed.IsDel = "1";
            //    filed.Content = "xxxxxxxoooooooo";
            //    filed.Name = "913 添加数据";
            //});

            // 8. In
            //string sql = "SELECT * FROM SomeTable WHERE id IN @ids"
            //var results = conn.Query(sql, new { ids = new[] { 1, 2, 3, 4, 5 });

            // 8. Update  条件使用表达式 Like, In, Or ??  表达式转sql  区分表达式和赋值字段
            //var efrowsupdateex = LockDapperUtil<LockPers>.Update(
            //set =>
            //{
            //    set.Name = "修改913 修改Name和Content字段 模糊匹配xxxx";
            //    set.Prompt = "BMWWWWWWWWWWWWWWW";
            //    set.UpdateTime = DateTime.Now;   
            //    set.IsDel = "1";
            //}
            //, where => SQLMethods.DB_Like(where.Content, "%xxoo%") && where.IsDel == "1");

            // 9. select 表达式 
            var test = LockDapperUtil<LockPers>.New.Get(w => SM.Like(w.Name, "%Steam%") && w.IsDel == true);

            // 10. delete 表达式 
            // var delresult = LockDapperUtil<LockPers>.Delete(w => SQLMethods.DB_Like(w.Content, "%xxoo%") && w.IsDel == "1");

            // 11. dynamic 
            //var test2 = LockDapperUtil.Query("SELECT * FROM LockPers where isDel != 1  order by Name ", null);

            // 12. order by 排序



            // --------------------------------------------------------------------------------------

            //DynamicParameters mapped = new DynamicParameters();
            //// mapped.AddParameters(cmd, identity);
            //mapped.Add("@isDel", 1);
            //var efrows = LockDapperUtil.Execute(sqlSelect, mapped);


            //LockDapperUtil.Get<LockPers>("xxxxxxxoooooooo");


            // --------------------------------------------------------------------------------------


            // --------------------------------------------------------------------------------------
            //var name = "xxxo";
            //var key = new { Name = "test" };
            //string sql = $"select * from {name} where {key.Name} = @id";

            //var customers = new List<LockPers>();

            //// 1
            //IEnumerable<LockPers> customerQuery = from cust in customers
            //                                      where cust.Content == "London"
            //                                      select cust;

            //// 2
            //IEnumerable<LockPers> customerQuery2 = customers.Where(p => p.Content == "London");

            //ExpressionType
            // var a = new System.Linq.Expressions.Expression();


            Expression<Func<LockPers, bool>> expression = t => SM.In(t.Name, new string[] { "马", "码" }) 
               && t.Name == "农码一生" && t.Prompt == "男" || t.Name.Contains("11") ;
            StringBuilder sb = null;
            DynamicParameters spars = null;
            AnalysisExpression.VisitExpression(expression, ref sb, ref spars);


        }
        
    }

}
