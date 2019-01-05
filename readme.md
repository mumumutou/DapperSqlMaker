
**DapperSqlMaker 链式查询扩展** 

###### Gihub地址：
>[https://github.com/mumumutou/DapperSqlMaker](https://github.com/mumumutou/DapperSqlMaker)   欢迎dalao加入来完善
###### Nuget安装:  
	Install-Package DapperSqlMaker -Version 0.1.1

###### 基于(已引入源码):
	Dapper-1.50.2\Dapper
	Dapper-1.50.2\Dapper.Contrib
###### Demo:
	查询       TestsDapperSqlMaker\DapperSqlMaker.Test\  SelectDapperSqlMakerTest.cs
	添加       TestsDapperSqlMaker\DapperSqlMaker.Test\  InsertDapperSqlMakerTest.cs
	更新       TestsDapperSqlMaker\DapperSqlMaker.Test\  UpdateDapperSqlMakerTest.cs
	删除       TestsDapperSqlMaker\DapperSqlMaker.Test\  DeleteDapperSqlMakerTest.cs
	上下文类   TestsDapperSqlMaker\DbDapperSqlMaker\     LockDapperUtilsqlite.cs
	
##### 简单栗子：

##### 1.查询-联表查询,分页

```csharp 
public void 三表联表分页测试()
{
    var arruser = new int[2] { 1,2 };  // 
    string uall = "b.*", pn1 = "%蛋蛋%", pn2 = "%m%";
    LockPers lpmodel = new LockPers() { IsDel = false};
    Users umodel = new Users() { UserName = "jiaojiao" };
    SynNote snmodel = new SynNote() { Name = "木头" };
    Expression<Func<LockPers, Users, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote>();
    where = where.And((l, u, s) => ( l.Name.Contains(pn1) || l.Name.Contains(pn2) ));
    where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
    where = where.And((l, u, s) => u.UserName == umodel.UserName);
    where = where.And((l, u, s) => s.Name == snmodel.Name );
    where = where.And((l, u, s) => SM.In(u.Id, arruser));

    DapperSqlMaker<LockPers, Users, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote>
        .Selec()
        .Column((lp, u, s) => //null)  //查询所有字段
            new { lp.Name, lpid = lp.Id, x = "LENGTH(a.Prompt) as len", b = SM.Sql(uall), scontent = s.Content, sname = s.Name })
        .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
        .Where(where)
        .Order((lp, w, sn) => new { lp.EditCount, x = SM.OrderDesc(lp.Name), sn.Content });

    var result = query.ExcuteSelect();
    WriteJson(result); //  查询结果

    Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
    WriteSqlParams(resultsqlparams); // 打印sql和参数

    int page = 2, rows = 3, records;
    var result2 = query.LoadPagelt(page, rows, out records);
    WriteJson(result2); //  查询结果
}
```
*生成的sql* :
```sql
select  a.Name as Name, a.Id as lpid
	, LENGTH(a.Prompt) as len, b.*
	, c.Content as scontent, c.Name as sname  
from LockPers a  
	left join  Users b on  b.Id = a.UserId   
	inner join  SynNote c on  b.Id = c.UserId  
where  (  a.Name like @Name0  or  a.Name like @Name1  )  
	and  a.IsDel = @IsDel2  and  b.UserName = @UserName3  
	and  c.Name = @Name4  and  b.Id in @Id 
order by  a.EditCount, a.Name desc , c.Content 
```

##### 2.更新-更新部分字段

```csharp
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
```


 

-----

 
 注意：
> 1. svn提交到github时 不要再解决方案内复制文件 直接当作新文件添加进来 
> 2. 调试打印dapper查询sql  
   方法: Dapper.SqlMapper.QueryImpl  
    取消注释: // Console.WriteLine(cmd.CommandText);  
> 3. where条件
     可变参数 比较时 先转成值类型
> 4. 实体表明后缀不要是数字
> 5. 表别名按 a,b,c... 顺序类推
> 6. 七联表以上待扩展       
    只需copy已实现的 修改3个文件            
    DapperSqlMaker              
    Template_DapperSqlMaker 上下文类         
    PredicateBuilder        条件拼接类
> 7. [实体生成T4模板使用方法点我](https://www.cnblogs.com/cl-blogs/p/7205954.html)
> 

> 待测：
> 1. SM这几个方法Like In Convert 值参数为 A.B.C.val VisitExpression和JoinExpression中