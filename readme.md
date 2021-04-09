
**DapperSqlMaker 链式查询扩展 ** 

###### Gihub地址：
>[https://github.com/mumumutou/DapperSqlMaker](https://github.com/mumumutou/DapperSqlMaker)   
######  
	Nuget安装:> Install-Package DapperSqlMaker -Version 0.2.2
	
	Dapper-1.50.2\Dapper
	Dapper-1.50.2\Dapper.Contrib 
###### ：
> 1. ExcuteSelect 方法名更改为 => ExecuteQuery 
> 2. s._IsWriteFiled 在实体类默认构造函数中默认为false   赋值时需修改为true
> 3. 非链式解析的curd方法全放到 DapperFuncs类中了
> 4. 新增 .SqlClaus(withsql) 任意段拼接方法(公用表表达式 带完善)
> 5. 新增 事务执行栗子 在修改demo中
	
###### (依赖Dapper源码版)   
	Nuget安装:>  Install-Package DapperSqlMaker -Version 0.1.9 
    依赖项:
	Dapper (>= 1.50.0)
	Dapper.Contrib (>= 1.50.0)

###### Demo:
> 查询 [TestsDapperSqlMaker\DapperSqlMaker.Test\...SelectDapperSqlMakerTest.cs](https://github.com/mumumutou/DapperSqlMaker/blob/master/DapperSqlMakerVS2015/DapperSqlMaker/TestsDapperSqlMaker/DapperSqlMaker.Test/SelectDapperSqlMakerTest.cs)

> 添加 [TestsDapperSqlMaker\DapperSqlMaker.Test\...InsertDapperSqlMakerTest.cs](https://github.com/mumumutou/DapperSqlMaker/blob/master/DapperSqlMakerVS2015/DapperSqlMaker/TestsDapperSqlMaker/DapperSqlMaker.Test/InsertDapperSqlMakerTest.cs) 

> 更新 [TestsDapperSqlMaker\DapperSqlMaker.Test\...UpdateDapperSqlMakerTest.cs](https://github.com/mumumutou/DapperSqlMaker/blob/master/DapperSqlMakerVS2015/DapperSqlMaker/TestsDapperSqlMaker/DapperSqlMaker.Test/UpdateDapperSqlMakerTest.cs)

> 删除 [TestsDapperSqlMaker\DapperSqlMaker.Test\...DeleteDapperSqlMakerTest.cs](https://github.com/mumumutou/DapperSqlMaker/blob/master/DapperSqlMakerVS2015/DapperSqlMaker/TestsDapperSqlMaker/DapperSqlMaker.Test/DeleteDapperSqlMakerTest.cs)

> 一些方法 [DapperSqlMaker\DapperExt\DapperFuncsBase.cs](https://github.com/mumumutou/DapperSqlMaker/blob/master/DapperSqlMakerVS2015/DapperSqlMaker/DapperSqlMaker/DapperExt/DapperFuncsBase.cs)

> CodeFirst [TestsDapperSqlMaker\DapperSqlMaker.Test\...CodeFirstTest.cs](https://github.com/mumumutou/DapperSqlMaker/blob/master/DapperSqlMakerVS2015/DapperSqlMaker/TestsDapperSqlMaker/DapperSqlMaker.Test/CodeFirstTest.cs)

> 上下文类 TestsDapperSqlMaker\DbDapperSqlMaker\     LockDapperUtilsqlite.cs



	
##### 简单栗子：
##### 1.单表查询
```csharp
 public ActionResult Load(int page, int rows ,string serh)
{
	serh = $"%{serh}%";
	int records;
	var where = PredicateBuilder.WhereStart<LockPers>();
	where = where.And( m => m.IsDel != true );
	if (!string.IsNullOrWhiteSpace(serh)) {
		where = where.And( m => (m.Name.Contains(serh) || m.Prompt.Contains(serh) ) );
	}
	var query = LockDapperUtilsqlite<LockPers>
		.Selec().Column(p => new { t = "datetime(a.InsertTime) as InsertTimestr"
			, p.Id, p.Name, p.Content, p.Prompt, p.EditCount, p.CheckCount})
		.From().Where(where).Order(m => new { m.Name });
	var list = query.LoadPagelt(page, rows, out records);
	var resultjson = JsonConvert.SerializeObject(new { data = list, records = records});
	return Content(resultjson);
}
```

##### 2.查询-联表查询,分页
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
##### 3.添加-添加部分字段

```csharp 
public void 添加部分字段和子查询_测试lt2() 
{
    string colm = "img", val = "(select value from skin limit 1 offset 1)"; 
	DateTime cdate = DateTime.Now;
    var insert = LockDapperUtilsqlite<Users>.Inser().AddColumn(p => new bool[] {
         p.UserName =="木头人1", p.Password == "666", p.CreateTime == cdate
        , SM.Sql(colm,val), SM.Sql(p.Remark,"(select '荒野高尔夫')")
    }); 
    var efrow = insert.ExecuteInsert();
    Console.WriteLine(efrow + " " + insert.RawSqlParams().Item1);
}
```

##### 4.更新-更新部分字段

```csharp
public void 更新部分字段_含子查询_测试lt()
{
    string colm = "img", val = "(select value from skin limit 1 offset 1)"; 
	DateTime cdate = DateTime.Now;
    var update = LockDapperUtilsqlite<Users>.Updat().EditColumn(p => new bool[] {
        p.UserName =="几十行代码几十个错 调一步改一步....", p.Password == "bug制造者"
        , p.CreateTime == cdate,  SM.Sql(p.Remark,"(select '奥德赛 终于改好了')")
    }).Where(p => p.Id == 6 && SM.SQL("IsDel == 0"));
	
    var efrow = update.ExecuteUpdate();
    Console.WriteLine(efrow + update.RawSqlParams().Item1);
}
```

##### 5.删除-
```csharp
public void 删除数据_含子查询_测试lt() {
    var sql = " UserId = ( select Id from users  where UserName = '木头人1' )";
    var delete = LockDapperUtilsqlite<LockPers>.Delet().Where(p => 
            p.Name == "H$E22222" && SM.SQL(sql) && SM.SQL(" IsDel = '1' "));
    var efrow = delete.ExecuteDelete();
    Console.WriteLine(efrow + " " + delete.RawSqlParams().Item1);
}
```
 
##### 6.事务示例
```csharp


``` 
##### 7.事务示例
```csharp
 using (var conn = DBSqliteFuncs.New.GetConn()) // var = GetConn())
    {
        System.Data.IDbTransaction transaction = null;
        int? commandTimeout = null;
        var entity = new SynNote();
        // using Dapper.Contrib.Extensions;
        var t = conn.Inser(entity, false, transaction, commandTimeout);
        return (int)t;
    }

```


##### 8.条件方法参数传入规范示例  Column/Where/AddColumn/EditColumn
> 1. 直接where()方法中赋值       s.IsDel = 1;
> 2. 声明变量 接收参数 再传入    int Id = int.Parse( Request.Form["Id"]);  ---->   w.Id == Id_
> 3. Action参数装载器的参数不能直接传入  int Id_ = Id;  ---->   w.Id == Id_
> 4. 时间不能直接赋值 需要赋值给外部变量传入  p.Date == DateTime.Now  ------>  var date = DateTime.Now; // 再传入
```csharp
...
```


 
 注意：
> 1. svn提交到github时 不要再解决方案内复制文件 直接当作新文件添加进来 
> 2. 调试打印dapper查询sql  
   方法: Dapper.SqlMapper.QueryImpl  
    取消注释: // Console.WriteLine(cmd.CommandText);  
> 3. where条件
     可空类型参数 比较时 先转成值类型
> 4. 实体表明后缀不要是数字
> 5. 表别名按 a,b,c... 顺序类推
> 6. 七联表以上待扩展       
    只需copy已实现的 修改3个文件            
    DapperSqlMaker              
    Template_DapperSqlMaker 上下文类         
    PredicateBuilder        条件拼接类
> 7. [实体生成T4模板使用方法点我](https://www.cnblogs.com/cl-blogs/p/7205954.html)

.NetCore版：
> 1. DapperSqlMakerVS2017Core 文件下为 .NetCore2.0版本的待完善 表达式解析直接可以用
> 2. SqlMapperExtensions.cs扩展类中 991-994行(#if COREFX 附近)源码编译失败 已暂时修改为return null 待解决
> 3. 有时间把Dapper1.50.2源码和修改过的做分离开
> 4. t4未加入

> 待测问题：
> 1. SM这几个方法Like In Convert 值参数为 A.B.C.val VisitExpression和JoinExpression中
	 Like方法用
> 2.可以解析还有待继续测试 // actin自动装载的参数 传入 不能解析的问题
    ActionResult AddSkin(string name, string url)
      LockSqlite<Skin>.Inser().AddColumn( ...

VS2015测试工具NUnit安装:
```csharp

1.工具 -> 扩展和更新 ->联机
NUnit第3版，那么搜索NUnit 3 Test Adapter

2.先把这个东西复制上 -》》》   download.visualstudio.microsoft.com
搜索站长工具    -》http://tool.chinaz.com/dns/?type=1&host=download.visualstudio.microsoft.com&ip=

3.然后打开cmd
输入ipconfig /flushdns

```
