
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.MSSQL.Model
{

    /// <summary>
    /// A class which represents the SynNote table.
    /// </summary>
	[Table("[SynNote]")]
	public partial class SynNote
	{
	   /*  Id  Content  NoteDate  Name  UserId  IsDel  */ 

		[Key]
		public virtual int Id { get; set; }
		public virtual string Content { get; set; }
		public virtual DateTime? NoteDate { get; set; }
		public virtual string Name { get; set; }
		public virtual int UserId { get; set; }
		public virtual bool? IsDel { get; set; }
	}

} // namespace
