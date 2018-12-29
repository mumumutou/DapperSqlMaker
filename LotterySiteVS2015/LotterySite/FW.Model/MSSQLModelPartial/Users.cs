
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.MSSQL.Model
{

    /// <summary>
    /// A class which represents the Users table.
    /// </summary>
	[Table("[Users]")]
	public partial class User
	{
	   /*  Id  UserName  Password  CreateTime  Remark  IsDel  */ 

		[Key]
		public virtual int Id { get; set; }
		public virtual string UserName { get; set; }
		public virtual string Password { get; set; }
		public virtual DateTime CreateTime { get; set; }
		public virtual string Remark { get; set; }
		public virtual bool IsDel { get; set; }
	}


} // namespace
