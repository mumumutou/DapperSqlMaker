using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FW.Common.DapperExt
{ 
    public interface ISqlMaker 
    {
    }
    public class QueryMaker<T>
    {

         
        public static QueryMaker<T> Select()
        { 
            return new QueryMaker<T>(); 
        }



    }

    //public interface ISqlFirst
    //{
    //    ISqlMakerSelect SELECT(string columns = null);
    //    ISqlMakerSelect SelectDistinct(string columns = null);
    //    ISqlMakerInsert INSERT(string tableName);
    //    ISqlMakerUpdate UPDATE(string tableName);
    //    ISqlMakerDelete DELETE(string tableName);
    //}

}
