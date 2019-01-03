using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FW.Common.DapperExt
{
    // SQLMethods
    public static class SM
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

        //  直接传入sql


        public static bool AppendSql2(string str) => false;

        /// <summary>
        /// 直接拼接字符串sql
        /// </summary> 
        public static string Sql(string str) => null;
        public static string _Sql_Name = "Sql";
       

        /// <summary>
        /// Order By xxx desc
        /// </summary>
        public static string OrderDesc<T>(T field) => null;
        public static string _OrderDesc_Name = "OrderDesc";
        public static string OrderDesc_Sql = " desc ";

        public static string ColumnAll = " * ";
        public static string LimitSelectCount = " select count(1) counts ";
        public static string LimitCount = " count(1) over() as counts ";
        public static string _limitcount_Name = "SM.LimitCount";
        public static string LimitRowNumber_Sql = " row_number() over(order by {0}) as rownum, ";
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
                            spars.Add(Member.Member.Name, constValue);//arr);
                            sb.AppendFormat(" {0} in @{0}", Member.Member.Name);
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
                    if (binary.Left.NodeType == ExpressionType.OrElse
                            && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ( "); }

                    VisitExpression(binary.Left, ref sb, ref spars);

                    if (binary.Left.NodeType == ExpressionType.OrElse
                        && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ) "); }

                    if (spars.ParameterNames.Count() > 0)
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

                    BinaryExpression binaryg = expression as BinaryExpression;
                    if (binaryg.Left.NodeType == ExpressionType.Constant)
                    { // 左边为常量值  右边为字段名
                        MemberExpression Member = binaryg.Right as MemberExpression;
                        ConstantExpression constant = binaryg.Left as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString()  );
                        sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, +num, exgl);  // A > @A0
                    }
                    else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right is ConstantExpression)
                    { // 左边为字段名 右边为常量值
                        MemberExpression Member = binaryg.Left as MemberExpression;
                        ConstantExpression constant = binaryg.Right as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); // .ToString());
                        sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, num, exgl);
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
                        ConstantExpression constant = constMember.Expression as ConstantExpression; //右边变量所在的类
                        var constValue = constant.Value.GetType().GetField(constMember.Member.Name).GetValue(constant.Value);

                        spars.Add(Member.Member.Name + num, constValue); //.ToString());
                        sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, num, exgl);
                    }
                    // Console.WriteLine(sb);
                    break;
                default:
                    break;



            }
        }



        /// <summary>
        /// sql, pars 多表别名解析
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="tabAliasName">别名字典</param>
        /// <param name="sb"></param>
        /// <param name="spars"></param>
        /// <param name="paramsdic">表达式参数字典 参数-参数序号</param>
        public static void JoinExpression(Expression expression, Dictionary<string, string> tabAliasName
            , ref StringBuilder sb, ref DynamicParameters spars, Dictionary<string, int> paramsdic = null)
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
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名 
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
                        sb.AppendFormat(" {0} like @{1}{2} ", mberName, Member.Member.Name, num);
                    }
                    else if (method.Method.Name == "Like")
                    { // 自定义方法 like
                        MemberExpression Member = method.Arguments[0] as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = method.Arguments[1] as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        sb.AppendFormat(" {0} like @{1}{2} ", mberName, Member.Member.Name, num);
                    }
                    else if (method.Method.Name == "In")
                    { // 自定义方法 in
                        MemberExpression Member = method.Arguments[0] as MemberExpression; // 字段名
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名

                        MemberExpression member2 = method.Arguments[1] as MemberExpression; //第1种 数组传入表达式
                        if (member2 != null)
                        {
                            //右边变量名 member2.Name 
                            ConstantExpression constMember2 = member2.Expression as ConstantExpression; //右边变量所在的类
                            var constValue = constMember2.Value.GetType().GetField(member2.Member.Name).GetValue(constMember2.Value);

                            //var arr = (member2.Expression as ConstantExpression).Value;
                            spars.Add(Member.Member.Name, constValue);//arr);
                            sb.AppendFormat(" {0} in @{1}", mberName, Member.Member.Name);
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
                    else if (method.Method.Name == "Convert")
                    {
                        MemberExpression Member = method.Object as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = method.Arguments.FirstOrDefault() as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        sb.AppendFormat(" {0} = @{1}{2} ", mberName, Member.Member.Name, num);
                    }
                    // Console.WriteLine(sb);
                    break;
                case ExpressionType.Lambda://lambda表达式
                    LambdaExpression lambda = expression as LambdaExpression;

                    //Console.WriteLine(lambda+ "");  // 打印lambda表达式
                    Dictionary<string, int> pdic = new Dictionary<string, int>();
                    int i = 1;
                    foreach (var p in lambda.Parameters)
                    {
                        pdic.Add(p.Name,i++);
                    }

                    JoinExpression(lambda.Body, tabAliasName, ref sb, ref spars, pdic);

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
                    if (binary.Left.NodeType == ExpressionType.OrElse
                            && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso))
                    { sb.Append(" ( "); }

                    JoinExpression(binary.Left, tabAliasName, ref sb, ref spars, paramsdic);

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
                    JoinExpression(binary.Right, tabAliasName, ref sb, ref spars, paramsdic);
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
                        var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = binaryg.Left as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString()  );
                        sb.AppendFormat(" {0} {1} @{2}{3} ", mberName, exgl, Member.Member.Name, num);  // A > @A0
                    }
                    else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right is ConstantExpression)
                    { // 左边为字段名 右边为常量值
                        MemberExpression Member = binaryg.Left as MemberExpression;
                        ParameterExpression Parmexr = Member.Expression as ParameterExpression;
                        var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        ConstantExpression constant = binaryg.Right as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); // .ToString());
                        sb.AppendFormat(" {0} {1} @{2}{3} ", mberName, exgl, Member.Member.Name, num);
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
                        var tkey = Parmexr.Type.FullName + paramsdic[Parmexr.Name];
                        var mberName = tabAliasName[tkey] + "." + Member.Member.Name;  //Parmexr.Name 表.字段名
                        MemberExpression Member2 = binaryg.Right as MemberExpression;
                        if (Member2.Expression is ParameterExpression) 
                        { // 联表条件 左右都是表字段条件  on tab1.Id = tab2.Id
                            ParameterExpression pexp = Member2.Expression as ParameterExpression;

                            ParameterExpression Parmexr2 = Member2.Expression as ParameterExpression;
                            var tkey2 = Parmexr2.Type.FullName + paramsdic[Parmexr2.Name];
                            var mberName2 = tabAliasName[tkey2] + "." + Member2.Member.Name;  //Parmexr.Name 表.字段名
                            sb.AppendFormat(" {0} {1} {2}", mberName, exgl, mberName2);
                            break;
                        }
                        

                        object constValue = GetMemberValue(Member2);
                        //MemberExpression constMember = binaryg.Right as MemberExpression; //右边变量名 constMember.Member.Name 
                        //ConstantExpression constant = constMember.Expression as ConstantExpression; //右边变量所在的类
                        //var constValue = constant.Value.GetType().GetField(constMember.Member.Name).GetValue(constant.Value);

                        spars.Add(Member.Member.Name + num, constValue); //.ToString());
                        sb.AppendFormat(" {0} {1} @{2}{3} ", mberName, exgl, Member.Member.Name, num);
                            
                    }
                    // Console.WriteLine(sb);
                    break;
                default:
                    break;



            }
        }

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
        //public static Expression<Func<T, bool>> True<T>() { return f => true; }
        //public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<Func<T, Y, Z, O, bool>> And<T, Y, Z, O>(this Expression<Func<T, Y, Z, O, bool>> first, Expression<Func<T, Y, Z, O, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, Z, O, bool>> Or<T, Y, Z, O>(this Expression<Func<T, Y, Z, O, bool>> first, Expression<Func<T, Y, Z, O, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }


        public static Expression<Func<T, Y, Z, bool>> And<T, Y, Z>(this Expression<Func<T, Y, Z, bool>> first, Expression<Func<T, Y, Z, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, Z, bool>> Or<T, Y, Z>(this Expression<Func<T, Y, Z, bool>> first, Expression<Func<T, Y, Z, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T,Y, bool>> And<T,Y>(this Expression<Func<T,Y, bool>> first, Expression<Func<T,Y, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }
        public static Expression<Func<T, Y, bool>> Or<T, Y>(this Expression<Func<T, Y, bool>> first, Expression<Func<T, Y, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
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
