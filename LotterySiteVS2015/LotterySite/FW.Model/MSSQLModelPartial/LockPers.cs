
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.MSSQL.Model
{

    /// <summary>
    /// A class which represents the LockPers table.
    /// </summary>
	[Table("[LockPers]")]
	public partial class LockPer
	{
	   /*  Name  Content  Prompt  Id  InsertTime  IsDel  DelTime  UpdateTime  EditCount  UserId  */ 

		public virtual string Name { get; set; }
		public virtual string Content { get; set; }
		public virtual string Prompt { get; set; }
		public virtual string Id { get; set; }
		public virtual DateTime? InsertTime { get; set; }
		public virtual bool IsDel { get; set; }
		public virtual DateTime? DelTime { get; set; }
		public virtual DateTime? UpdateTime { get; set; }
		public virtual int? EditCount { get; set; }
		public virtual int UserId { get; set; }
	}

} // namespace
