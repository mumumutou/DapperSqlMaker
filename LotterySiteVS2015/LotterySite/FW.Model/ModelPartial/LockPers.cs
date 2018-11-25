
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the LockPers table.
    /// </summary>
	[Table("LockPers")]
	public partial class LockPers
	{  
	   /*  Name  Content  Prompt  Id  InsertTime  IsDel  DelTime  UpdateTime  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = false;
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion

        #region FieldName
		public readonly string  Field_Name = "Name"; 
		public readonly string  Field_Content = "Content"; 
		public readonly string  Field_Prompt = "Prompt"; 
		public readonly string  Field_Id = "Id"; 
		public readonly string  Field_InsertTime = "InsertTime"; 
		public readonly string  Field_IsDel = "IsDel"; 
		public readonly string  Field_DelTime = "DelTime"; 
		public readonly string  Field_UpdateTime = "UpdateTime"; 
		#endregion

        #region Field
		private string _Name { get; set; }
		private string _Content { get; set; }
		private string _Prompt { get; set; }
		private string _Id { get; set; }
		private DateTime? _InsertTime { get; set; }
		private string _IsDel { get; set; }
		private DateTime? _DelTime { get; set; }
		private DateTime? _UpdateTime { get; set; }
        #endregion

		public virtual string Name { 
			set { _Name = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_Name) ); }
			get { return _Name; }
		}
		public virtual string Content { 
			set { _Content = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_Content) ); }
			get { return _Content; }
		}
		public virtual string Prompt { 
			set { _Prompt = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_Prompt) ); }
			get { return _Prompt; }
		}
        [ExplicitKey] // 手动插入(主)键
        public virtual string Id { 
			set { _Id = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_Id) ); }
			get { return _Id; }
		}
		public virtual DateTime? InsertTime { 
			set { _InsertTime = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_InsertTime) ); }
			get { return _InsertTime; }
		}
		public virtual string IsDel { 
			set { _IsDel = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_IsDel) ); }
			get { return _IsDel; }
		}
		public virtual DateTime? DelTime { 
			set { _DelTime = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_DelTime) ); }
			get { return _DelTime; }
		}
		public virtual DateTime? UpdateTime { 
			set { _UpdateTime = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(this.Field_UpdateTime) ); }
			get { return _UpdateTime; }
		}

	}


} // namespace
