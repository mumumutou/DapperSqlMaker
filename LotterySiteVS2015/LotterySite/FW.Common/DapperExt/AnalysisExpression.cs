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



        public static Boolean NotIn<T>(List<T> list) // not in
        {
            return true;
        }
        public static int Length()  // len();
        {
            return 0;
        }
        public static bool Like(object name, string str) // like
        {
            return true;
        }
        public static bool NotLike(string str) // not like 
        {
            return true;
        }

        //  直接传入sql
        public static bool Sql(string str)
        {
            return true;
        }
    }


    //表达式解析
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
                    if (method.Method.Name == "Contains")
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
                            var arr = (member2.Expression as ConstantExpression).Value;
                            spars.Add(Member.Member.Name, arr);
                            sb.AppendFormat(" {0} in @{0}", Member.Member.Name);
                            break;
                        }
                        NewArrayExpression constant = method.Arguments[1] as NewArrayExpression;  //第2种 表达式内构建数组
                        if (constant != null) {
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
                            && (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso ) )
                        { sb.Append(" ( "); }  
                         
                        VisitExpression(binary.Left, ref sb, ref spars);

                        if (binary.Left.NodeType == ExpressionType.OrElse
                            &&  (binary.Right.NodeType != ExpressionType.OrElse || binary.Right.NodeType != ExpressionType.AndAlso ) )
                        { sb.Append(" ) "); }


                        sb.Append(expression.NodeType == ExpressionType.OrElse ? " or " : " and ");

                        if (binary.Right.NodeType == ExpressionType.OrElse
                                && (binary.Left.NodeType != ExpressionType.OrElse || binary.Left.NodeType != ExpressionType.AndAlso))
                        { sb.Append(" ( "); }
                        VisitExpression(binary.Right, ref sb, ref spars);
                        if (binary.Right.NodeType == ExpressionType.OrElse
                                && (binary.Left.NodeType != ExpressionType.OrElse || binary.Left.NodeType != ExpressionType.AndAlso) )
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
                    {
                        MemberExpression Member = binaryg.Right as MemberExpression;
                        ConstantExpression constant = binaryg.Left as ConstantExpression;
                        spars.Add( Member.Member.Name + num, constant.Value); //.ToString()  );
                        sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, +num, exgl);  // A > @A0
                    }
                    else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right is ConstantExpression)
                    {
                        MemberExpression Member = binaryg.Left as MemberExpression;
                        ConstantExpression constant = binaryg.Right as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); // .ToString());
                        sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, num, exgl);
                    }
                    else if (binaryg.Left.NodeType == ExpressionType.MemberAccess && binaryg.Right.NodeType == ExpressionType.Convert)
                    {
                        MemberExpression Member = binaryg.Left as MemberExpression;
                        ConstantExpression constant = (binaryg.Right as UnaryExpression).Operand as ConstantExpression;
                        spars.Add(Member.Member.Name + num, constant.Value); //.ToString());
                        sb.AppendFormat(" {0} {2} @{0}{1} ", Member.Member.Name, num, exgl);
                    }
                    // Console.WriteLine(sb);
                    break;
                default:
                    break;



            }
        } 

    }
}
