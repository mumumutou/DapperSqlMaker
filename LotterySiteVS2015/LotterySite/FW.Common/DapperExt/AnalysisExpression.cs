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
        public static bool Sql(string str)
        {
            return true;
        }

        public static bool WhereStartIgnore() => true;


    }


    //表达式解析
    /// <summary>
    /// 注意： 可空类型(DateTime)字段 与 非可空类型 比较时 右边的非可空类型会转换为可空类型再比较。所以传入变量也要声明为可空类型
    /// 
    /// </summary>
    public static class AnalysisExpression
    {


        // sql, pars
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
        //public static Expression<Func<T, bool>> True<T>() { return f => true; }
        //public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)  
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first  
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression   
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }
    }


}
