using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Linq.Expressions;
using Dapper;
using Dapper.Contrib.Extensions;
using DapperSqlMaker.DapperExt;

namespace Dapper
{
    public enum SqlClauseType
    {
        None,
        /// <summary>
        /// MSSql 分页开头语句
        /// </summary>
        PageStartms,
        /// <summary>
        /// MSSql 分页结束语句
        /// </summary>
        PageEndms,
    }

    public abstract class SqlMakerBase<Child> where Child : class
    {

        #region Clause
        protected ClauseType ClauseFirst { get; set; }

        protected enum ClauseType
        {
            ActionSelect,
            //ActionSelectLimitCounts,   
            ActionSelectRowRumberOrderBy,
            ActionSelectColumn,
            ActionSelectFrom,
            //ActionSelectJoin,
            //ActionLeftJoin,
            //ActionRightJoin,
            //ActionInnerJoin,
            ActionSelectWhereOnHaving,
            ActionSelectOrder,
            Table,

            Insert,
            AddColumn,
            Update,
            EditColumn,
            Delete,
            SqlClause,
        }

        protected class Clause
        {
            public static Clause New(ClauseType type, string select = null
                , string rowRumberOrderBy = null  //, string selectCounts = null
                , string selectColumn = null, string fromJoin = null
                , string seletTable = null//, string jointable = null, string aliace = null
                , string condition = null, DynamicParameters conditionParms = null
                , string order = null, string extra = null
                , string insert = null, string addcolumn = null, DynamicParameters insertParms = null
                , string update = null, string editcolumn = null, DynamicParameters updateParms = null
                , string delete = null, string sqlclause = null, DynamicParameters sqlClauseParms = null)
            {
                return new Clause
                {
                    ClauseType = type,
                    Select = select,
                    //SelectCounts = selectCounts,
                    SeletTable = seletTable, // 无用
                    RowRumberOrderBy = rowRumberOrderBy,
                    SelectColumn = selectColumn,
                    FromJoin = fromJoin,
                    //JoinTable = jointable,
                    //Aliace = aliace,
                    Condition = condition,
                    ConditionParms = conditionParms,
                    Order = order,
                    Extra = extra,
                    //添加 ------------
                    Insert = insert,
                    AddColumn = addcolumn,
                    InsertParms = insertParms,
                    //修改 ------------
                    Update = update,
                    EditColumn = editcolumn,
                    UpdateParms = updateParms,
                    Delete = delete,
                    // 任意位置sql
                    SqlClause = sqlclause,
                    SqlClauseParms = sqlClauseParms
                };
            }

            public ClauseType ClauseType { get; private set; }
            public string SeletTable { get; private set; }
            public string Select { get; private set; }
            //public string SelectCounts { get; private set; }
            public string RowRumberOrderBy { get; private set; }
            public string SelectColumn { get; private set; }
            public string FromJoin { get; private set; }
            //public string JoinTable { get; private set; }//
            public string Condition { get; private set; } // where
            public string Order { get; private set; }
            //public string Aliace { get; private set; } 
            public DynamicParameters ConditionParms { get; private set; }
            public string Extra { get; private set; } // 字段 
            public string Insert { get; private set; }
            public string AddColumn { get; private set; }
            public DynamicParameters InsertParms { get; private set; }
            public string Update { get; private set; }
            public string EditColumn { get; private set; }
            public DynamicParameters UpdateParms { get; private set; }
            public string Delete { get; private set; }
            /// <summary>
            /// 任意位置 sql
            /// </summary>
            public string SqlClause { get; private set; }
            /// <summary>
            /// 任意位置 sql 参数
            /// </summary>
            public DynamicParameters SqlClauseParms { get; private set; }
        }

        //protected class TabAliace {
        //    public static Dictionary<string, string> Dic = new Dictionary<string, string>();
        //    //public static TabAliace New(string name, string aliace) {
        //    //    return new TabAliace { Name = name,Aliace = aliace };
        //    //}
        //    //public string Name { get; private set; }
        //    //public string Aliace { get; private set; } 
        //}

        protected List<Clause> _clauses;
        protected List<Clause> Clauses
        {
            get { return _clauses ?? (_clauses = new List<Clause>()); }
        }
        #endregion


        public abstract Child GetChild() ;


        /// <summary>
        /// 任意部分sql拼接
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <returns></returns>
        protected SqlMakerBase<Child> SqlClauseAddClauses(string sqlClause)
        {
            Clauses.Add(Clause.New(ClauseType.SqlClause, sqlclause: sqlClause));
            return this;
        }
        /// <summary>
        /// 特定sql拼接
        /// </summary>
        /// <param name="sqlClauseType">with加分页查询语句首位占位符 默认为空</param>
        /// <returns></returns>
        protected SqlMakerBase<Child> SqlClauseTypeAddClauses(SqlClauseType sqlClauseType = SqlClauseType.None)
        {
            //string sqlClause;
            switch (sqlClauseType)
            {
                case SqlClauseType.PageStartms:
                    Clauses.Add(Clause.New(ClauseType.SqlClause, sqlclause: SM.PageStartms));
                    return this;
                case SqlClauseType.PageEndms:
                    Clauses.Add(Clause.New(ClauseType.SqlClause, sqlclause: SM.PageEndms));
                    return this;
                case SqlClauseType.None:
                default:
                    return this;
            }

            //string sqlclause = base.SqlClauseTypeStr(sqlClauseType);
            ;
            //return sqlClause;
        }


        /// <summary>
        /// 任意部分sql拼接
        /// </summary>
        /// <param name="sqlClause">拼接语句</param>
        /// <returns></returns>
        public Child SqlClause(string sqlClause) => this.SqlClauseAddClauses(sqlClause).GetChild();

    }
    public class SqlMaker<T> : SqlMakerBase<SqlMaker<T>> 
    {
        public override SqlMaker<T> GetChild() => this;
        

    }
    public abstract class SqlMaker<T,Y,Child>
    {

        public void Test() {
            SqlMaker<T> query = new SqlMaker<T>();
            //query.Sql
        }
    }

    public abstract class SqlMaker<T,Y,Z, Child>
    {

    }
}
