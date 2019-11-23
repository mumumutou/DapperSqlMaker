using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DapperSqlMaker.DapperExt
{
    // SQLMethods
    public static partial class SM
    {
        // in
        /// <summary>
        /// In 条件  
        /// 表达式内构建的数组连同值转sql语句 A in (1,2)
        /// 传入的数组值走参数查询  A in @A
        /// DateTime类型格式化成string传入 new string { "2004-05-07", "2004-05-07" }
        /// oracle中带转换函数只能用Sql方法
        /// </summary>
        public static bool In<T>(string name, T[] arr) => true;
        /// <summary>
        /// In 条件  
        /// 表达式内构建的数组连同值转sql语句 A in (1,2);
        /// 传入的数组值走参数查询  A in @A;
        /// DateTime类型格式化成string传入 new string { "2004-05-07", "2004-05-07" }
        /// oracle中带转换函数只能用Sql方法
        /// </summary>
        public static bool In<T>(int name, T[] arr) => true;
        public static bool In<T>(int? name, T[] arr) => true;
        public static bool In<T>(DateTime? name, T[] arr) => true;
        public static bool In<T>(int name, List<T> list) => true;
        public static bool In<T>(int? name, List<T> list) => true;
        public static bool In<T>(string name, List<T> list) => true;
        public static bool In<T,Y>(Y name, T[] arr) => true;
        public static bool In<T,Y>(Y name, List<T> list) => true;

        //
        //public static DateTime DateStr(DateTime date) => date;

        public static Boolean NotIn<T>(List<T> list) // not in
        {
            return true;
        }
        public static int Length()  // len();
        {
            return 0;
        }
        //public static bool Like(object name, string str) // like
        //{
        //    return true;
        //}
        //public static bool NotLike(string str) // not like 
        //{
        //    return true;
        //}

        /// <summary>
        /// AddColumn()/EditColumn() 子查询sql拼接  (只给 增加/修改列 子句中使用) 
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="sql">子查询</param>
        /// <returns></returns>
        public static bool Sql(string name, string sql) => true;
        public static bool Sql(int name, string sql) => true;
        public static bool Sql(int? name, string sql) => true;
        public static bool Sql(DateTime name, string sql) => true;
        public static bool Sql(DateTime? name, string sql) => true;
        public static bool Sql(bool name, string sql) => true;
        public static bool Sql(bool? name, string sql) => true;
        public static bool Sql<T>(T name, string sql) => true;
        public static bool Sql<T>(T? name, string sql) where T :struct   => true; 

        public static bool AppendSql2(string str) => false;

        /// <summary>
        /// Where()直接拼接字符串sql  (只给查询 条件 子句中使用) 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool SQL(string str) => true;
        /// <summary>
        /// SM.SQL方法名称：Where()
        /// </summary>
        public static string _SQL_Name = "SQL";

        /// <summary>
        /// Column()直接拼接字符串sql (只给查询 列 子句中使用) 
        /// </summary> 
        public static string Sql(string str) => null;
        /// <summary>
        ///  SM.Sql方法名称 ：Column()/AddColumn()/EditColumn()
        /// </summary>
        public static string _Sql_Name = "Sql";

        #region 标记方法

        /// <summary>
        /// 倒序标记方法 Order By xxx desc
        /// </summary>
        public static string OrderDesc<T>(T field) => null;
        public static string _OrderDesc_Name = "OrderDesc";
        public static string OrderDesc_Sql = " desc ";



        #endregion

        #region 特定sql值和标记
        /// <summary>
        /// select语句开头标记 MSSql分页拼接 开头占位符
        /// </summary>
        public static string PageStartms = " select x.* from (  ";
        /// <summary>
        /// select语句结束标记 MSSql分页拼接 结束占位符
        /// </summary>
        public static string PageEndms = " ) x  where rownum between (@pageIndex - 1) * @pageSize + 1 and @pageIndex * @pageSize ";
        /// <summary>
        /// (内部调用)分页 
        /// </summary>
        public static string ColumnAll = " * ";
        /// <summary>
        /// SQLite分页总记录 特定sql值 (SQLite分页方法内部调用)
        /// </summary>
        public static string LimitSelectCount = " select count(1) counts ";
        /// <summary>
        /// MSSql分页总记录 特定sql值和标记 (MSQL分页查询时 Column()方法中显示标记)
        /// </summary>
        public static string LimitCount = " count(1) over() as counts ";
        /// <summary>
        /// (内部调用)
        /// </summary>
        public static string _limitcount_Name = "SM.LimitCount";
        /// <summary>
        /// (内部调用)MSSql分页
        /// </summary>
        public static string LimitRowNumber_Sql = " row_number() over(order by {0}) as rownum, ";

        #endregion

        #region 参数化 标识
        /// <summary>
        /// "@" MSSql,Sqlite,MYSql
        /// </summary>
        public static string ParamSymbolMSSql = "@";
        /// <summary>
        /// ":" Oracle 
        /// </summary>
        public static string ParamsSymbolOracle = ":";

        #endregion

        public static bool WhereStartIgnore() => true;


    }


    //表达式解析
    /// <summary>
    /// 注意： 可空类型(DateTime)字段 与 非可空类型 比较时 右边的非可空类型会转换为可空类型再比较。所以传入变量也要声明为可空类型
    /// 
    /// </summary>
    public static class AnalysisExpression
    {


        // sql, pars  单表解析
        /// <summary>
        /// 单表没表别名 where条件解析
        /// ????? 可以废除了  JoinExpression 可以解析单表不生成表别名的 where条件了
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sb"></param>
        /// <param name="spars"></param>
        public static void VisitExpression(Expression expression, ref StringBuilder sb, ref DynamicParameters spars)
        {
            if (sb == null)
            {
                sb = new StringBuilder();
                // level = 0;
                spars = new DynamicParameters();
            }
            // Console.WriteLine(sb); // 查看执行顺序是解开
            var num = spars.ParameterNames.Count();
            switch (expression.NodeType)
            {
                case ExpressionType.Call://执行方法
                    MethodCallExpression method = expression as MethodCallExpression;
                    if (method.Method.Name == "WhereStartIgnore") break;
                    // where拼接条件开始 忽略解析该方法  
                    else if (method.Method.Name == "Contains")
                    {
                        MemberExpression Member = method.Object as MemberExpression;
                        ConstantExpression constant = method.Arguments.FirstOrDefault() as ConstantExpression;

                        object ctvalue;
                        if (method.Arguments.FirstOrDefault() is ConstantExpression)
                        { //
                            ctvalue = (method.Arguments.FirstOrDefault() as ConstantExpression).Value;
                        }
                        else if (method.Arguments.FirstOrDefault() is MemberExpression)
                        { // 值 传入的时 变量 
                            ctvalue = GetMemberValue(method.Arguments.FirstOrDefault() as MemberExpression);
                        }
                        else throw new Exception("Contains未解析");

                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        sb.AppendFormat(" {0} like @{0}{1} ", Member.Member.Name, num);
                    }
                    else if (method.Method.Name == "Like")
                    { // 自定义方法 like
                        MemberExpression Member = method.Arguments[0] as MemberExpression;
                        ConstantExpression constant = method.Arguments[1] as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        sb.AppendFormat(" {0} like @{0}{1} ", Member.Member.Name, num);
                    }
                    else if (method.Method.Name == "In")
                    { // 自定义方法 in
                        MemberExpression Member = method.Arguments[0] as MemberExpression; // 字段名

                        MemberExpression member2 = method.Arguments[1] as MemberExpression; //第1种 数组传入表达式
                        if (member2 != null)
                        {
                            //右边变量名 member2.Name 
                            ConstantExpression constMember2 = member2.Expression as ConstantExpression; //右边变量所在的类
                            var constValue = constMember2.Value.GetType().GetField(member2.Member.Name).GetValue(constMember2.Value);

                            //var arr = (member2.Expression as ConstantExpression).Value;
                            spars.Add(Member.Member.Name + num, constValue);//arr);
                            sb.AppendFormat(" {0} in @{0}", Member.Member.Name + num);
                            break;
                        }
                        NewArrayExpression constant = method.Arguments[1] as NewArrayExpression;  //第2种 表达式内构建数组
                        if (constant != null)
                        {
                            var cstarr = constant.Expressions.Select(p =>
                            {
                                var cp = p as ConstantExpression;
                                if (cp.Type == typeof(String)
                                || cp.Type == typeof(DateTime))
                                    return string.Format("'{0}'", cp.Value);
                                return cp.Value;
                            }).ToList();
                            sb.AppendFormat(" {0} in ({1})", Member.Member.Name, string.Join(",", cstarr));
                            break;
                        }


                        // oracle参数化In可传入数组
                        //spars.Add(Member.Member.Name + num, constant.Value.ToString()); 
                        //sb.AppendFormat(" {0} in @{0}{1} ", Member.Member.Name, num);
                    }
                    else if (method.Method.Name == "Convert")
                    {
                        MemberExpression Member = method.Object as MemberExpression;
                        ConstantExpression constant = method.Arguments.FirstOrDefault() as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        sb.AppendFormat(" {0} = @{0}{1} ", Member.Member.Name, num);
                    }
                    // Console.WriteLine(sb);
                    break;
                case ExpressionType.Lambda://lambda表达式
                    LambdaExpression lambda = expression as LambdaExpression;

                    //Console.WriteLine(lambda+ "");  // 打印lambda表达式

                    VisitExpression(lambda.Body, ref sb, ref spars);

                    // BinaryExpression  // 二元表达式
                    // UnaryExpression   // 一元表达式
                    // BlockExpression   // 块

                    break;
                //case ExpressionType.Equal://相等比较 
                case ExpressionType.AndAlso://and条件运算
                case ExpressionType.OrElse: // 或运算
                    BinaryExpression binary = expression as BinaryExpression;
                    //if (binary.Left.NodeType == ExpressionType.Constant)
                    //{
                    //    MemberExpression Member = binary.Right as MemberExpression;
                    //    ConstantExpression constant = binary.Left as ConstantExpression;
                    //    spars.Add( Member.Member.Name + num, constant.Value.ToString()  );
                    //    sb.AppendFormat(" {0} = @{0}{1} ", Member.Member.Name, +num );
                    //}
                    //else if (binary.Left.NodeType == ExpressionType.MemberAccess && binary.Right is ConstantExpression)
                    //{
                    //    MemberExpression Member = binary.Left as MemberExpression;
                    //    ConstantExpression constant = binary.Right as ConstantExpression;
                    //    spars.Add( Member.Member.Name + num, constant.Value.ToString() );
                    //    sb.AppendFormat(" {0} = @{0}{1} ", Member.Member.Name, num );
                    //}
                    //else if (binary.Left.NodeType == ExpressionType.MemberAccess &&  binary.Right.NodeType == ExpressionType.Convert)
                    //{
                    //    MemberExpression Member = binary.Left as MemberExpression;
                    //    ConstantExpression constant = (binary.Right as UnaryExpression).Operand as ConstantExpression;
                    //    spars.Add( Member.Member.Name + num, constant.Value.ToString() );
                    //    sb.AppendFormat(" {0} = @{0}{1} ", Member.Member.Name, num );
                    //}
                    //else
                    //{
                    // sql串联or会补上内嵌括号 不影响  调试时c#串联or的表达式也会自动补上内嵌括号

                    //where拼接条件开始 忽略解析该方法  
                    if (binary.Left.NodeType == ExpressionType.Call)
                    {
                        MethodCallExpression startmethod = binary.Left as MethodCallExpression;
                        if (startmethod.Method.Name == "WhereStartIgnore")
                        {
                            VisitExpression(binary.Right, ref sb, ref spars);
                            return;
                        }// like in 其他方法继续
                    }


                    if (binary.Left.NodeType == ExpressionType.OrElse
                            && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ( "); }

                    VisitExpression(binary.Left, ref sb, ref spars);

                    if (binary.Left.NodeType == ExpressionType.OrElse
                        && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ) "); }

                    if (spars.ParameterNames.Count() > 0 || sb.Length > 0)
                    { //  where拼接条件开始 判断忽略解析WhereStartIgnore方法 
                        sb.Append(expression.NodeType == ExpressionType.OrElse ? " or " : " and ");
                    }

                    if (binary.Right.NodeType == ExpressionType.OrElse
                            && (binary.Left.NodeType != ExpressionType.OrElse || binary.Left.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ( "); }
                    VisitExpression(binary.Right, ref sb, ref spars);
                    if (binary.Right.NodeType == ExpressionType.OrElse
                            && (binary.Left.NodeType != ExpressionType.OrElse || binary.Left.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ) "); }

                    //}

                    // Console.WriteLine(sb);
                    break;
                case ExpressionType.Equal://相等比较
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    // 大于 小于 成员在左边还是在右边的 转成sql的位置也不同???
                    var exgl = expression.NodeType == ExpressionType.Equal ? "="
                            : expression.NodeType == ExpressionType.NotEqual ? "!="
                            : expression.NodeType == ExpressionType.GreaterThan ? ">"
                            : expression.NodeType == ExpressionType.GreaterThanOrEqual ? ">="
                            : expression.NodeType == ExpressionType.LessThan ? "<"
                            : expression.NodeType == ExpressionType.LessThanOrEqual ? "<="
                            : null;
                    if (exgl == null) throw new Exception("未知的比较符号");
                    BinaryExprssRowSqlParms(expression, sb, spars, num, exgl
                        , (name, paramsName, exglstr) => string.Format(" {0} {2} @{1} ", name, paramsName, exglstr) );
                    //sb.AppendFormat(sqlFormart, Member.Member.Name, +num, exgl);  // A > @A0
                    // Console.WriteLine(sb);
                    break;
                default:
                    break;



            }
        }

        /// <summary>
        /// 一元表达式解析 a.Name == "名称" 
        /// </summary>
        /// <param name="expression">一元表达式</param>
        /// <param name="sb">生成的sql</param>
        /// <param name="spars">生成参数</param>
        /// <param name="num">参数区分序号</param>
        /// <param name="exgl">运算符</param> 
        /// <param name="formartFunc">生成指定格式sql (arg1:字段名,arg2:参数名,arg3:表达式) 参数名为 字段名+区分序号 </param>
        public static void BinaryExprssRowSqlParms(Expression expression, StringBuilder sb, DynamicParameters spars, int num, string exgl,Func<string,string,string,string> formartFunc)
        {
            BinaryExpression binaryg = expression as BinaryExpression;
            if (binaryg.Left.NodeType == ExpressionType.Constant)
            { // 左边为常量值  右边为字段名
                MemberExpression Member = binaryg.Right as MemberExpression;
                ConstantExpression constant = binaryg.Left as ConstantExpression;
                string parmName = Member.Member.Name + num + "_"; // 加后缀区分where解析参数
                spars.Add(parmName, constant.Value); //.ToString()  );
                sb.Append( formartFunc(Member.Member.Name, parmName, exgl) );// 
                //sb.AppendFormat(sqlFormart, Member.Member.Name, num, exgl);  // A > @A0  
            }
            else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right is ConstantExpression)
            { // 左边为字段名 右边为常量值
                MemberExpression Member = binaryg.Left as MemberExpression;
                ConstantExpression constant = binaryg.Right as ConstantExpression;
                string parmName = Member.Member.Name + num + "_"; // 加后缀区分where解析参数
                spars.Add(parmName, constant.Value); // .ToString());
                sb.Append( formartFunc(Member.Member.Name, parmName, exgl) );// 
                //sb.AppendFormat(sqlFormart, Member.Member.Name, num, exgl);
            }
            // 时间格式化处理
            //else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right.NodeType == ExpressionType.Convert)
            //{
            //    MemberExpression Member = binaryg.Left as MemberExpression;
            //    ConstantExpression constant = (binaryg.Right as UnaryExpression).Operand as ConstantExpression;

            //    spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
            //    sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, num, exgl);
            //}

            else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right.NodeType == ExpressionType.MemberAccess)
            {  // 左边为字段名 右边为传入的外部变量 w => w.Name == varName
                MemberExpression Member = binaryg.Left as MemberExpression;

                MemberExpression constMember = binaryg.Right as MemberExpression; //右边变量名 constMember.Member.Name 
                                                                                  //ConstantExpression constant = constMember.Expression as ConstantExpression; //右边变量所在的类 
                                                                                  //var constValue = constant.Value.GetType().GetField(constMember.Member.Name).GetValue(constant.Value);

                object constValue = GetMemberValue(constMember);
                string parmName = Member.Member.Name + num;
                spars.Add(parmName, constValue); //.ToString());
                sb.Append( formartFunc(Member.Member.Name, parmName, exgl) );// 
                //sb.AppendFormat(sqlFormart, Member.Member.Name, num, exgl);
            }
        }


        /// <summary>
        /// sql, pars 多表别名解析  表别名按a,b,c来只要读取Lambda表达式参数顺序即可
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sb"></param>
        /// <param name="spars"></param>
        /// <param name="paramsdic">表达式参数字典 参数-参数序号</param>
        /// <param name="isAliasName">是否加表别名</param>
        public static void JoinExpression(Expression expression, ref StringBuilder sb, ref DynamicParameters spars
            , Dictionary<string, int> paramsdic = null, bool isAliasName = true, string sqlParamSymbol = "@")
        {
            if (sb == null)
            {
                sb = new StringBuilder();
                // level = 0;
                spars = new DynamicParameters();
            }
            // Console.WriteLine(sb); // 查看执行顺序是解开
            var num = spars.ParameterNames.Count();
            switch (expression.NodeType)
            {
                case ExpressionType.Call://执行方法
                    MethodCallExpression method = expression as MethodCallExpression;
                    if (method.Method.Name == "WhereStartIgnore") break;
                    // where拼接条件开始 忽略解析该方法  
                    else
                    if (method.Method.Name == "Contains")
                    {
                        MemberExpression Member = method.Object as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        //var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        //var mberName = (isAliasName ? (tabAliasName[tkey] + "." ) : string.Empty) + Member.Member.Name;  //Parmexr.Name 表.字段名  
                        // 表示参数 ---> 表别名 (char)(1+96) <-->  (int)'a'
                        var mberName = (isAliasName ? (((char)(paramsdic[Parmexr.Name] + 96)) + ".") : string.Empty) + Member.Member.Name;  //Parmexr.Name 表.字段名 
                        object ctvalue;
                        if (method.Arguments.FirstOrDefault() is ConstantExpression)
                        { //
                            ctvalue = (method.Arguments.FirstOrDefault() as ConstantExpression).Value;
                        }
                        else if (method.Arguments.FirstOrDefault() is MemberExpression)
                        { // 值 传入的时 变量 
                            ctvalue = GetMemberValue(method.Arguments.FirstOrDefault() as MemberExpression);
                        }
                        else throw new Exception("Contains未解析");

                        spars.Add(Member.Member.Name + num, ctvalue); //constant.Value //.ToString());
                        //sb.AppendFormat(" {0} like @{1}{2} ", mberName, Member.Member.Name, num);
                        sb.AppendFormat(" {0} like {3}{1}{2} ", mberName, Member.Member.Name, num, sqlParamSymbol);  // " {0} like @{1}{2} "
                    }
                    else if (method.Method.Name == "Like")
                    { // 自定义方法 like
                        MemberExpression Member = method.Arguments[0] as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        //var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name]; //tabAliasName[tkey]
                        var mberName = (isAliasName ? (((char)(paramsdic[Parmexr.Name] + 96)) + ".") : string.Empty)
                            + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = method.Arguments[1] as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        //sb.AppendFormat(" {0} like @{1}{2} ", mberName, Member.Member.Name, num);
                        sb.AppendFormat(" {0} like {3}{1}{2} ", mberName, Member.Member.Name, num, sqlParamSymbol); //" {0} like @{1}{2} "
                    }
                    else if (method.Method.Name == "In")
                    { // 自定义方法 in
                        MemberExpression Member = method.Arguments[0] as MemberExpression; // 字段名
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        //var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        //var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        var mberName = (isAliasName ? (((char)(paramsdic[Parmexr.Name] + 96)) + ".") : string.Empty)
                            + Member.Member.Name;  //Parmexr.Name 表.字段名

                        MemberExpression member2 = method.Arguments[1] as MemberExpression; //第1种 数组传入表达式
                        if (member2 != null)
                        {
                            //右边变量名 member2.Name 
                            ConstantExpression constMember2 = member2.Expression as ConstantExpression; //右边变量所在的类
                            var constValue = constMember2.Value.GetType().GetField(member2.Member.Name).GetValue(constMember2.Value);

                            //var arr = (member2.Expression as ConstantExpression).Value;
                            spars.Add(Member.Member.Name + num, constValue);//arr);
                            //sb.AppendFormat(" {0} in @{1}", mberName, Member.Member.Name + num);
                            sb.AppendFormat(" {0} in {2}{1}", mberName, Member.Member.Name + num, sqlParamSymbol); //" {0} in @{1}"
                            break;
                        }
                        NewArrayExpression constant = method.Arguments[1] as NewArrayExpression;  //第2种 表达式内构建数组
                        if (constant != null)
                        {
                            var cstarr = constant.Expressions.Select(p =>
                            {
                                var cp = p as ConstantExpression;
                                if (cp.Type == typeof(String)
                                || cp.Type == typeof(DateTime))
                                    return string.Format("'{0}'", cp.Value);
                                return cp.Value;
                            }).ToList();
                            sb.AppendFormat(" {0} in ({1})", mberName, string.Join(",", cstarr));
                            break;
                        }


                        // oracle参数化In可传入数组
                        //spars.Add(Member.Member.Name + num, constant.Value.ToString()); 
                        //sb.AppendFormat(" {0} in @{0}{1} ", Member.Member.Name, num);
                    }
                    else if (method.Method.Name == SM._SQL_Name)
                    { //拼接任意sql
                        if (method.Arguments[0] is ConstantExpression)
                        { // sql直接赋值传入
                            var constValue = (method.Arguments[0] as ConstantExpression).Value;
                            sb.Append(constValue);
                        }
                        else if (method.Arguments[0] is MemberExpression)
                        {
                            var constValue = AnalysisExpression.GetMemberValue(method.Arguments[0] as MemberExpression);
                            sb.Append(constValue);
                        }

                    }
                    else if (method.Method.Name == "Convert")
                    {
                        MemberExpression Member = method.Object as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        //var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        //var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        var mberName = (isAliasName ? (((char)(paramsdic[Parmexr.Name] + 96)) + ".") : string.Empty)
                            + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = method.Arguments.FirstOrDefault() as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        //sb.AppendFormat(" {0} = @{1}{2} ", mberName, Member.Member.Name, num);
                        sb.AppendFormat(" {0} = {3}{1}{2} ", mberName, Member.Member.Name, num, sqlParamSymbol); // " {0} = @{1}{2} "
                    }
                    else throw new Exception(method.Method.Name + " 暂未做解析的方法 " + expression);
                    // Console.WriteLine(sb);
                    break;
                case ExpressionType.Lambda://lambda表达式
                    LambdaExpression lambda = expression as LambdaExpression;

                    //Console.WriteLine(lambda+ "");  // 打印lambda表达式
                    // 表别名按a,b,c,d来
                    if (paramsdic == null)
                    {// FromJoin外面动态赋值传入
                        //Dictionary<string, int> pdic
                        paramsdic = new Dictionary<string, int>();
                        int i = 1;
                        foreach (var p in lambda.Parameters)
                        {
                            paramsdic.Add(p.Name, i++);
                        }

                    }

                    JoinExpression(lambda.Body, ref sb, ref spars, paramsdic,isAliasName: isAliasName, sqlParamSymbol: sqlParamSymbol);

                    // BinaryExpression  // 二元表达式
                    // UnaryExpression   // 一元表达式
                    // BlockExpression   // 块

                    break;
                //case ExpressionType.Equal://相等比较 
                case ExpressionType.AndAlso://and条件运算
                case ExpressionType.OrElse: // 或运算
                    BinaryExpression binary = expression as BinaryExpression;
                    //if (binary.Left.NodeType == ExpressionType.Constant)
                    //{
                    //    MemberExpression Member = binary.Right as MemberExpression;
                    //    ConstantExpression constant = binary.Left as ConstantExpression;
                    //    spars.Add( Member.Member.Name + num, constant.Value.ToString()  );
                    //    sb.AppendFormat(" {0} = @{0}{1} ", Member.Member.Name, +num );
                    //}
                    //else if (binary.Left.NodeType == ExpressionType.MemberAccess && binary.Right is ConstantExpression)
                    //{
                    //    MemberExpression Member = binary.Left as MemberExpression;
                    //    ConstantExpression constant = binary.Right as ConstantExpression;
                    //    spars.Add( Member.Member.Name + num, constant.Value.ToString() );
                    //    sb.AppendFormat(" {0} = @{0}{1} ", Member.Member.Name, num );
                    //}
                    //else if (binary.Left.NodeType == ExpressionType.MemberAccess &&  binary.Right.NodeType == ExpressionType.Convert)
                    //{
                    //    MemberExpression Member = binary.Left as MemberExpression;
                    //    ConstantExpression constant = (binary.Right as UnaryExpression).Operand as ConstantExpression;
                    //    spars.Add( Member.Member.Name + num, constant.Value.ToString() );
                    //    sb.AppendFormat(" {0} = @{0}{1} ", Member.Member.Name, num );
                    //}
                    //else
                    //{
                    // sql串联or会补上内嵌括号 不影响  调试时c#串联or的表达式也会自动补上内嵌括号

                    //where拼接条件开始 忽略解析该方法  
                    if (binary.Left.NodeType == ExpressionType.Call) {
                        MethodCallExpression startmethod = binary.Left as MethodCallExpression;
                        if (startmethod.Method.Name == "WhereStartIgnore") {
                            JoinExpression(binary.Right, ref sb, ref spars, paramsdic, isAliasName: isAliasName, sqlParamSymbol :sqlParamSymbol);
                            return;
                        }// like in 其他方法继续
                    }

                    if (binary.Left.NodeType == ExpressionType.OrElse
                            && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ( "); }

                    JoinExpression(binary.Left, ref sb, ref spars, paramsdic, isAliasName: isAliasName, sqlParamSymbol: sqlParamSymbol);

                    if (binary.Left.NodeType == ExpressionType.OrElse
                        && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ) "); }


                    //LambdaExpression lambda = expression as LambdaExpression;
                    //MethodCallExpression method = lambda.Body. as MethodCallExpression;
                    //if (method.Method.Name == "WhereStartIgnore") break;

                    if (spars.ParameterNames.Count() > 0 || sb.Length > 0)
                    { //  where拼接条件开始 判断忽略解析WhereStartIgnore方法 
                        sb.Append(expression.NodeType == ExpressionType.OrElse ? " or " : " and ");
                    }

                    if (binary.Right.NodeType == ExpressionType.OrElse
                            && (binary.Left.NodeType != ExpressionType.OrElse || binary.Left.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ( "); }
                    JoinExpression(binary.Right, ref sb, ref spars, paramsdic, isAliasName: isAliasName, sqlParamSymbol: sqlParamSymbol);
                    if (binary.Right.NodeType == ExpressionType.OrElse
                            && (binary.Left.NodeType != ExpressionType.OrElse || binary.Left.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ) "); }

                    //}

                    // Console.WriteLine(sb);
                    break;
                case ExpressionType.Equal://相等比较
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    // 大于 小于 成员在左边还是在右边的 转成sql的位置也不同???
                    var exgl = expression.NodeType == ExpressionType.Equal ? "="
                            : expression.NodeType == ExpressionType.NotEqual ? "!="
                            : expression.NodeType == ExpressionType.GreaterThan ? ">"
                            : expression.NodeType == ExpressionType.GreaterThanOrEqual ? ">="
                            : expression.NodeType == ExpressionType.LessThan ? "<"
                            : expression.NodeType == ExpressionType.LessThanOrEqual ? "<="
                            : null;
                    if (exgl == null) throw new Exception("未知的比较符号");

                    BinaryExpression binaryg = expression as BinaryExpression;
                    if (binaryg.Left.NodeType == ExpressionType.Constant)
                    { // 左边为常量值  右边为字段名
                        MemberExpression Member = binaryg.Right as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        //var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        //var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        var mberName = (isAliasName? (((char)(paramsdic[Parmexr.Name] + 96)) + "."): string.Empty)
                            + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = binaryg.Left as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString()  );
                        //sb.AppendFormat(" {0} {1} @{2}{3} ", mberName, exgl, Member.Member.Name, num);  // A > @A0
                        sb.AppendFormat(" {0} {1} {4}{2}{3} ", mberName, exgl, Member.Member.Name, num, sqlParamSymbol);  //" {0} {1} @{2}{3} "  // A > @A0
                    }
                    else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right is ConstantExpression)
                    { // 左边为字段名 右边为常量值
                        MemberExpression Member = binaryg.Left as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        //var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        //var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        var mberName = (isAliasName? (((char)(paramsdic[Parmexr.Name] + 96)) + "."):string.Empty) 
                            + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = binaryg.Right as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); // .ToString());
                        //sb.AppendFormat(" {0} {1} @{2}{3} ", mberName, exgl, Member.Member.Name, num);
                        sb.AppendFormat(" {0} {1} {4}{2}{3} ", mberName, exgl, Member.Member.Name, num, sqlParamSymbol); // " {0} {1} @{2}{3} "
                    }
                    // 时间格式化处理
                    //else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right.NodeType == ExpressionType.Convert)
                    //{
                    //    MemberExpression Member = binaryg.Left as MemberExpression;
                    //    ConstantExpression constant = (binaryg.Right as UnaryExpression).Operand as ConstantExpression;

                    //    spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                    //    sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, num, exgl);
                    //}

                    else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right.NodeType == ExpressionType.MemberAccess)
                    {  // 左边为字段名 右边为传入的外部变量 w => w.Name == varName
                        MemberExpression Member = binaryg.Left as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        //var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        //var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        var mberName = (isAliasName ? (((char)(paramsdic[Parmexr.Name] + 96)) + "."):string.Empty)
                        + Member.Member.Name;  //Parmexr.Name 表.字段名
                        MemberExpression Member2 = binaryg.Right as MemberExpression;
                        if (Member2.Expression is ParameterExpression) 
                        { // 联表条件 左右都是表字段条件  on tab1.Id = tab2.Id
                            ParameterExpression pexp = Member2.Expression as ParameterExpression;

                            ParameterExpression Parmexr2 = Member2.Expression as ParameterExpression;
                            //var tkey2 = Parmexr2.Type.FullName + paramsdic[Parmexr2.Name];
                            //var mberName2 = tabAliasName[tkey2] + "." + Member2.Member.Name;  //Parmexr.Name 表.字段名
                            // ################### 注意是 Parmexr2 #############################
                            var mberName2 = (isAliasName ? ( ((char)(paramsdic[Parmexr2.Name] + 96)) + ".") :string.Empty) + Member2.Member.Name;  //Parmexr.Name 表.字段名
                            sb.AppendFormat(" {0} {1} {2}", mberName, exgl, mberName2);
                            break;
                        }
                        

                        object constValue = GetMemberValue(Member2);
                        //MemberExpression constMember = binaryg.Right as MemberExpression; //右边变量名 constMember.Member.Name 
                        //ConstantExpression constant = constMember.Expression as ConstantExpression; //右边变量所在的类
                        //var constValue = constant.Value.GetType().GetField(constMember.Member.Name).GetValue(constant.Value);

                        spars.Add(Member.Member.Name + num, constValue); //.ToString());
                        //sb.AppendFormat(" {0} {1} @{2}{3} ", mberName, exgl, Member.Member.Name, num);
                        sb.AppendFormat(" {0} {1} {4}{2}{3} ", mberName, exgl, Member.Member.Name, num, sqlParamSymbol); // " {0} {1} @{2}{3} "

                    }
                    // Console.WriteLine(sb);
                    break;
                default:
                    break;



            }
        }

        /// <summary>
        /// 获取 成员值
        /// </summary> 
        public static object GetMemberValue(MemberExpression ctmber,string mberName = null) //)MethodCallExpression method)
        {
            // ctmber.Member.Name
            if (mberName == null) mberName = ctmber.Member.Name;
            //  w => w.字段 == 变量
            //  w => w.字段 == 变量.属性
            //  w => w.字段 == 变量.属性.属性

            if (ctmber.Expression is MemberExpression) return GetMemberValue(ctmber.Expression as MemberExpression,mberName);

            object ctvalue; 
            ConstantExpression ctconst = ctmber.Expression as ConstantExpression;
            ctvalue = ctconst.Value.GetType().GetField(ctmber.Member.Name).GetValue(ctconst.Value); //ctmber.Member.Name
            if (ctmber.Member.Name != mberName)
                ctvalue = ctvalue.GetType().GetProperty(mberName).GetValue(ctvalue,null);
            return ctvalue;
        }
    }


    //public static class DynamicLinqExpressions
    //{

    //    public static Expression<Func<T, bool>> True<T>() { return f => true; }
    //    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    //    public static Expression<Func<T, bool>> Or1<T>(this Expression<Func<T, bool>> expr1,
    //                                                        Expression<Func<T, bool>> expr2) 
    //    {
    //        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
    //        return Expression.Lambda<Func<T, bool>>
    //              (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
    //    }

    //    public static Expression<Func<T, bool>> And1<T>(this Expression<Func<T, bool>> expr1,
    //                                                         Expression<Func<T, bool>> expr2)
    //    {
    //        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
    //        return Expression.Lambda<Func<T, bool>>
    //              (Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
    //    }

    //}


    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }


    public static class PredicateBuilder
    {
        /// <summary>
        /// where条件拼接初始化条件 解析时忽略返回值方法名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> WhereStart<T>() { return f => SM.WhereStartIgnore(); }
        public static Expression<Func<T,Y, bool>> WhereStart<T,Y>() { return (f,y) => SM.WhereStartIgnore(); }
        public static Expression<Func<T, Y, Z, bool>> WhereStart<T, Y, Z>() { return (f, y, z) => SM.WhereStartIgnore(); }
        public static Expression<Func<T, Y, Z, O, bool>> WhereStart<T, Y, Z, O>() { return (f, y, z, o) => SM.WhereStartIgnore(); }
        public static Expression<Func<T, Y, Z, O, P, bool>> WhereStart<T, Y, Z, O, P>() { return (f, y, z, o, p) => SM.WhereStartIgnore(); }
        public static Expression<Func<T, Y, Z, O, P, Q, bool>> WhereStart<T, Y, Z, O, P, Q>() { return (f, y, z, o, p, q) => SM.WhereStartIgnore(); }


        public static Expression<Func<T, Y, Z, O, P, Q, bool>> And<T, Y, Z, O, P, Q>(this Expression<Func<T, Y, Z, O, P, Q, bool>> first, Expression<Func<T, Y, Z, O, P, Q, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, Z, O, P, Q, bool>> Or<T, Y, Z, O, P, Q>(this Expression<Func<T, Y, Z, O, P, Q, bool>> first, Expression<Func<T, Y, Z, O, P, Q, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }


        public static Expression<Func<T, Y, Z, O, P, bool>> And<T, Y, Z, O, P>(this Expression<Func<T, Y, Z, O, P, bool>> first, Expression<Func<T, Y, Z, O, P, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, Z, O, P, bool>> Or<T, Y, Z, O, P>(this Expression<Func<T, Y, Z, O, P, bool>> first, Expression<Func<T, Y, Z, O, P, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        //public static Expression<Func<T, bool>> True<T>() { return f => true; }
        //public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<Func<T, Y, Z, O, bool>> And<T, Y, Z, O>(this Expression<Func<T, Y, Z, O, bool>> first, Expression<Func<T, Y, Z, O, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, Z, O, bool>> Or<T, Y, Z, O>(this Expression<Func<T, Y, Z, O, bool>> first, Expression<Func<T, Y, Z, O, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }


        public static Expression<Func<T, Y, Z, bool>> And<T, Y, Z>(this Expression<Func<T, Y, Z, bool>> first, Expression<Func<T, Y, Z, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, Z, bool>> Or<T, Y, Z>(this Expression<Func<T, Y, Z, bool>> first, Expression<Func<T, Y, Z, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<T,Y, bool>> And<T,Y>(this Expression<Func<T,Y, bool>> first, Expression<Func<T,Y, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, bool>> Or<T, Y>(this Expression<Func<T, Y, bool>> first, Expression<Func<T, Y, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }



        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)  
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first  
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression   
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
      }


}
