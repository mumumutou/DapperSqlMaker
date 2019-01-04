**DapperSqlMaker 链式查询扩展** 

[Gihub地址](https://github.com/mumumutou/DapperSqlMaker)

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

###### 1.查询-联表查询,分页

```csharp
[Test]
public void 三表联表分页测试()
{
    LockPers lpmodel = new LockPers() { Name = "%蛋蛋%", IsDel = false};
    Users umodel = new Users() { UserName = "jiaojiao" };
    SynNote snmodel = new SynNote() { Name = "%木头%" };
    Expression<Func<LockPers, Users, SynNote, bool>> where = PredicateBuilder.WhereStart<LockPers, Users, SynNote>();
    where = where.And((lpw, uw, sn) => lpw.Name.Contains(lpmodel.Name));
    where = where.And((lpw, uw, sn) => lpw.IsDel == lpmodel.IsDel);
    where = where.And((lpw, uw, sn) => uw.UserName == umodel.UserName);
    where = where.And((lpw, uw, sn) => sn.Name.Contains(snmodel.Name));

    DapperSqlMaker<LockPers, Users, SynNote> query = LockDapperUtilsqlite<LockPers, Users, SynNote>
        .Selec()
        .Column((lp, u, s) =>		// null) //查询所有字段
            new { lp.Id, lp.InsertTime, lp.EditCount, lp.IsDel, u.UserName, s.Content, s.Name })
        .FromJoin(JoinType.Left, (lpp, uu, snn) => uu.Id == lpp.UserId
                , JoinType.Inner, (lpp, uu, snn) => uu.Id == snn.UserId)
        .Where(where)
        .Order((lp, w, sn) => new { lp.EditCount, lp.Name, sn.Content });

    var result = query.ExcuteSelect(); //1. 执行查询
    WriteJson(result); //  打印查询结果

    Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
    WriteSqlParams(resultsqlparams);  // 打印生成sql和参数 

    int page = 2, rows = 3, records;
    var result2 = query.LoadPagelt(page, rows, out records); //2. 分页查询
    WriteJson(result2); //  查询结果
}
```
##### 2.更新-更新部分字段

```csharp
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
```



> //########################################  
> 
> 注意：
>1. svn提交到github时 不要再解决方案内复制文件 直接当作新文件添加进来 
> 
> 
>2.调试打印dapper查询sql
> 方法: Dapper.SqlMapper.QueryImpl
> 取消注释: // Console.WriteLine(cmd.CommandText);  
> 
>3.where条件
> 可变参数 比较时 先转成值类型
> 
>4.实体表明后缀不要是数字
>
>5.七联表以上待扩展 只需copy已实现的 修改3个文件
>  DapperSqlMaker 
>  Template_DapperSqlMaker 上下文类
>  PredicateBuilder        条件拼接类
> 
>6.[实体生成T4模板使用方法点我](https://www.cnblogs.com/cl-blogs/p/7205954.html)
>
