
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the LuckDraw table.
    /// </summary>
	[Table("[LuckDraw]")]
	public partial class LuckDraw
	{
	   /*  LdID  LdPrizeName  LdName  LdMobile  LdOpenID  LdIfDraw  LdCreateTime  ExhID  */ 

		[Key]
		public virtual int LdID { get; set; }
		public virtual string LdPrizeName { get; set; }
		public virtual string LdName { get; set; }
		public virtual string LdMobile { get; set; }
		public virtual string LdOpenID { get; set; }
		public virtual bool? LdIfDraw { get; set; }
		public virtual DateTime? LdCreateTime { get; set; }
		public virtual int? ExhID { get; set; }
	}


} // namespace
