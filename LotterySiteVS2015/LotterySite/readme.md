**DapperSqlMaker 链式查询扩展** 
###### 基于:
	Dapper-1.50.2\Dapper
	Dapper-1.50.2\Dapper.Contrib
###### Demo:
	查询 TestsFW.Common\DapperExt\SelectDapperSqlMakerTest.cs
	添加 TestsFW.Common\DapperExt\InsertDapperSqlMakerTest.cs
	更新 TestsFW.Common\DapperExt\UpdateDapperSqlMakerTest.cs
	删除 TestsFW.Common\DapperExt\DeleteDapperSqlMakerTest.cs
	上下文类      FW.Common\DapperExt\LockDapperUtilsqlite.cs
	
##### 简单栗子：

###### 1.查询-联表查询,分页

```
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
    WriteJson(result); //  查询结果

    Tuple<StringBuilder, DynamicParameters> resultsqlparams = query.RawSqlParams();
    WriteSqlParams(resultsqlparams);

    int page = 2, rows = 3, records;
    var result2 = query.LoadPagelt(page, rows, out records); //2. 分页查询
    WriteJson(result2); //  查询结果
}
```
##### 2.更新-更新部分字段

```
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
> 抽奖程序 Demo
> sqlite 库
> 
> 注意：
> 1. svn提交到github时 不要再解决方案内复制文件 直接当作新文件添加进来 
> 
> 
> 打印查询sql
> 方法: Dapper.SqlMapper.QueryImpl
> 取消注释: // Console.WriteLine(cmd.CommandText);  
> 
> where条件
> 可变参数 比较时 先转成值类型
> 
> 实体表明后缀不要是数字