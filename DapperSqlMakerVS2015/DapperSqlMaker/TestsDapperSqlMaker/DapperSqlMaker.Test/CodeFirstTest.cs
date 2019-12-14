using Dapper;
using Dapper.Contrib.Extensions;
using DapperSqlMaker.DapperExt;
using FW.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestsDapperSqlMaker.DapperSqlMaker.Test
{
    [Table("MK_Skin")]
    public class MK_Skin_
    {
        [Key]
        public string Id { get; set; }
        public int? IsDel { get; set; }
    }

    [Table("MK_User")]
    public class MK_User_
    {
        [Key]
        public string Id { get; set; }
        public DateTime CDate { get; set; } 
    }

    [Table("MK_Skin")]
    public class MK_Skin
    {
        [Key]
        [StringLength(50, false)]
        [OldField("Id")]
        public string MKSId { get; set; }
        [StringLength(50, Nullable = true, NVarchar = true)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Bgurl { get; set; }
        
        public DateTime? CDate { get; set; } 
    }
    [Table("MK_User")]
    public class MK_User
    {
        [Key]
        [StringLength(50, false)]
        [OldField("Id")]
        public string MKUId { get; set; }
        [StringLength(100, false, true)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Code { get; set; } 
        public int? IsDel { get; set; }
    }

    [TestFixture()]
    public class CodeFirstTest
    {
        [Test]
        public void Code_First_粗暴模式_先删再键表_() {
           var ef = EshineCloudBase.New()
                .CodeFirstInitgg(false,typeof(MK_Skin_), typeof(MK_User_));
            Console.WriteLine($"表更新数量:{ef}");
        }
        /*生成sql:
        CREATE TABLE MK_Skin(  Id varchar(100)  Primary Key  not null , IsDel int   null  )
        CREATE TABLE MK_User(  Id varchar(100)  Primary Key  not null , CDate datetime   not null  )
        */
        [Test]
        public void Code_First_友好模式_修改表() {
            var ef = EshineCloudBase.New()
                .CodeFirstInit(true,typeof(MK_Skin),typeof(MK_User));
            Console.WriteLine($"表更新数量:{ef}");
        }
        /*生成sql:
        EXEC sp_rename 'MK_Skin.[Id]', 'MKSId' , 'COLUMN'
        alter table MK_Skin add Name  nvarchar(50)  null
        ; alter table MK_Skin add Bgurl  varchar(200)  null
        ; alter table MK_Skin add CDate  datetime  null
        ; alter table MK_Skin drop column IsDel
        EXEC sp_rename 'MK_User.[Id]', 'MKUId' , 'COLUMN'
        alter table MK_User add Name  nvarchar(100)  not null
        ; alter table MK_User add Code  varchar(50)  null
        ; alter table MK_User add IsDel  int  null
        ; alter table MK_User drop column CDate
*/
        [Test]
        public void Code_First_删除备份表() {
            var droplsitsql =  
@"SELECT 'drop table ' + TABLE_NAME + ';' FROM  INFORMATION_SCHEMA.TABLES
WHERE(TABLE_TYPE = 'BASE TABLE' OR TABLE_TYPE = 'VIEW') and TABLE_NAME like 'bak_%'  ";
            var list = EshineCloudBase.New().Query<string>(droplsitsql, null).ToList();
            Console.WriteLine(list.Count());
            var dropsql = string.Join("",list);
            var ef = EshineCloudBase.New().Query(dropsql,null);
        }

        [Test]
        public void Dapper_存储过程2种方式()
        {
            using (var conn = EshineCloudBase.New().GetConn())
            {
                var trans = conn.BeginTransaction();
                try
                {
                    // 1
                    conn.Query("EXEC sp_rename 'model_xxx.[Pie666]', 'Pie333' , 'COLUMN'", transaction: trans);

                    // 2
                    //EXEC sp_rename 'model_xxx.[Pie2]', 'Pie666' , 'COLUMN'
                    //@objname,@newname,@objtype
                    var x = conn.Query("sp_rename", new { objname = "model_xxx.[Pie2]", newname = "Pie666", objtype = "COLUMN" }, trans, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch { }
            }
        }

        //[Test]
        //public void Code_First_粗暴模式_先删再键表()
        //{
        //    // 库中表结构
        //    var tabColumns = EshineCloudBase.New().Query<ColumnInfo>(CodeFirstCommon.COLUMN_SQL, null).ToList<ColumnInfo>();
        //    var groups = tabColumns.GroupBy(p => p.Table_Name).ToList();
        //    var tabs = groups.Select(p => new TableInfo(p.FirstOrDefault().Table_Name, p.ToList())).ToList();


        //    // 当前CodeFirst实体
        //    var mtype = typeof(modelxxx_ms_);
        //    var attrTab = mtype.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
        //    var TabBak = DateTime.Now.ToString("bak_yyyyMMdd_HHmmss_") + attrTab.Name; ///备份表名
        //    var TabOld = attrTab.Name.ToLower(); // 原表名

        //    // CodeFirst列数据
        //    var mTypeColumns = mtype.GetProperties().Where(DsmSqlMapperExtensions.IsWriteable) // 过滤Write false字段
        //             .Select<PropertyInfo, ColumnInfo>(p => new ColumnInfo(p, TabOld)).ToList<ColumnInfo>();
        //    // 当前表(库)结构
        //    var tab = tabs.FirstOrDefault(p => p.Table_Name.ToLower() == TabOld);

        //    if (tab != null) // 已有表 备份并删除
        //    {
        //        // 备份表
        //        var bakef = EshineCloudBase.New().Query(CodeFirstCommon.TABLE_BUK_SQL.Replace("@TabBak", TabBak).Replace("@TabOld", TabOld), null);

        //        if (bakef.Count() == 0)
        //        {
        //            // 删除旧表
        //            var delef = EshineCloudBase.New().Query($"drop table {TabOld} ", null);
        //        }
        //    }


        //    var addTab = new TableInfo(TabOld, mTypeColumns);
        //    var createtab = addTab.GetCreateTable();
        //    Console.WriteLine(createtab);
        //    // 创建表
        //    var ef = EshineCloudBase.New().Query(createtab, null);

        //}

        //[Test]
        //public void Code_First_友好模式_修改表()
        //{
        //    // 默认约束更新不支持 需要先删在改

        //    // 当前CodeFirst实体
        //    var mtype = typeof(modelxxx_ms_);
        //    var attrTab = mtype.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
        //    var TabBak = DateTime.Now.ToString("bak_yyyyMMdd_HHmmss_") + attrTab.Name;  // 备份表名
        //    var TabOld = attrTab.Name.ToLower(); // 原表名


        //    // 库中表结构
        //    var tabColumns = EshineCloudBase.New().Query<ColumnInfo>(CodeFirstCommon.COLUMN_SQL, null).ToList<ColumnInfo>();
        //    var groups = tabColumns.GroupBy(p => p.Table_Name).ToList();
        //    var tabs = groups.Select(p => new TableInfo(p.FirstOrDefault().Table_Name, p.ToList())).ToList();



        //    // CodeFirst列数据
        //    var mTypeColumns = mtype.GetProperties().Where(DsmSqlMapperExtensions.IsWriteable) // 过滤Write false字段
        //             .Select<PropertyInfo, ColumnInfo>(p => new ColumnInfo(p, TabOld)).ToList<ColumnInfo>();

        //    // 当前表(库)结构
        //    var tab = tabs.FirstOrDefault(p => p.Table_Name.ToLower() == TabOld);
        //    // 判断库中是否有该表
        //    if (tab == null) // 不存在直接新建表
        //    {
        //        var addTab = new TableInfo(TabOld, mTypeColumns);
        //        var createtab = addTab.GetCreateTable();
        //        Console.WriteLine(createtab);
        //        // 创建表
        //        var ef = EshineCloudBase.New().Query(createtab, null);
        //        return;
        //    }

        //    // 有就备份该表
        //    var bakef = EshineCloudBase.New().Query(CodeFirstCommon.TABLE_BUK_SQL.Replace("@TabBak", TabBak).Replace("@TabOld", TabOld), null);
        //    if (bakef.Count() != 0) throw new Exception($"{TabOld}备份失败");// 备份失败

        //    // 备份成功 开始对比 库中表 和CodeFirst表结构
        //    var alterTab = new TableInfo(TabOld, mTypeColumns);
        //    var alterTab_ret = alterTab.GetAlterTable(tab);
        //    Console.WriteLine(alterTab_ret.Item1);

        //    //EshineCloudBase.New().Query
        //    using (var conn = EshineCloudBase.New().GetConn())
        //    {
        //        var trans = conn.BeginTransaction();
        //        try
        //        {
        //            // 执行所有改列名列存储过程
        //            alterTab_ret.Item2.ForEach(p => conn.Query(p, transaction: trans));

        //            // 修改/新增/删除的列
        //            conn.Query(alterTab_ret.Item1, transaction: trans);

        //            trans.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            trans.Rollback();
        //            Console.WriteLine(ex.Message);
        //        }

        //    }



        //}
    }

}
