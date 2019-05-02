
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
	   /*  Name  Content  Prompt  Id  InsertTime  IsDel  DelTime  UpdateTime  EditCount  CheckCount  UserId  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public LockPers() {
            this._IsWriteFiled = false;
        }
        public LockPers(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_Name = "Name"; 
		public static readonly string  Field_Content = "Content"; 
		public static readonly string  Field_Prompt = "Prompt"; 
		public static readonly string  Field_Id = "Id"; 
		public static readonly string  Field_InsertTime = "InsertTime"; 
		public static readonly string  Field_IsDel = "IsDel"; 
		public static readonly string  Field_DelTime = "DelTime"; 
		public static readonly string  Field_UpdateTime = "UpdateTime"; 
		public static readonly string  Field_EditCount = "EditCount"; 
		public static readonly string  Field_CheckCount = "CheckCount"; 
		public static readonly string  Field_UserId = "UserId"; 
		#endregion

        #region Field
		private string _Name ; 
		private string _Content ; 
		private string _Prompt ; 
		private string _Id ; 
		private DateTime? _InsertTime ; 
		private bool _IsDel ; 
		private DateTime? _DelTime ; 
		private DateTime? _UpdateTime ; 
		private int _EditCount ; 
		private int _CheckCount ; 
		private int _UserId ; 
        #endregion

		public virtual string Name { 
			set { _Name = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Name) ); }
			get { return _Name; }
		}
		public virtual string Content { 
			set { _Content = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Content) ); }
			get { return _Content; }
		}
		public virtual string Prompt { 
			set { _Prompt = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Prompt) ); }
			get { return _Prompt; }
		}
		public virtual string Id { 
			set { _Id = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Id) ); }
			get { return _Id; }
		}
		public virtual DateTime? InsertTime { 
			set { _InsertTime = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_InsertTime) ); }
			get { return _InsertTime; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_IsDel) ); }
			get { return _IsDel; }
		}
		public virtual DateTime? DelTime { 
			set { _DelTime = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_DelTime) ); }
			get { return _DelTime; }
		}
		public virtual DateTime? UpdateTime { 
			set { _UpdateTime = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_UpdateTime) ); }
			get { return _UpdateTime; }
		}
		public virtual int EditCount { 
			set { _EditCount = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_EditCount) ); }
			get { return _EditCount; }
		}
		public virtual int CheckCount { 
			set { _CheckCount = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_CheckCount) ); }
			get { return _CheckCount; }
		}
		public virtual int UserId { 
			set { _UserId = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_UserId) ); }
			get { return _UserId; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  LockPers_ table.
    /// </summary>
	[Table("LockPers")]
	public partial class LockPers_
	{  
	   /*  Name  Content  Prompt  Id  InsertTime  IsDel  DelTime  UpdateTime  EditCount  CheckCount  UserId  */ 

	      
        #region Field
		private string _Name ;
		private string _Content ;
		private string _Prompt ;
		private string _Id ;
		private DateTime? _InsertTime ;
		private bool _IsDel ;
		private DateTime? _DelTime ;
		private DateTime? _UpdateTime ;
		private int _EditCount ;
		private int _CheckCount ;
		private int _UserId ;
        #endregion

		public virtual string Name { 
			set { _Name = value; }
			get { return _Name; }
		}
		public virtual string Content { 
			set { _Content = value; }
			get { return _Content; }
		}
		public virtual string Prompt { 
			set { _Prompt = value; }
			get { return _Prompt; }
		}
		public virtual string Id { 
			set { _Id = value; }
			get { return _Id; }
		}
		public virtual DateTime? InsertTime { 
			set { _InsertTime = value; }
			get { return _InsertTime; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; }
			get { return _IsDel; }
		}
		public virtual DateTime? DelTime { 
			set { _DelTime = value; }
			get { return _DelTime; }
		}
		public virtual DateTime? UpdateTime { 
			set { _UpdateTime = value; }
			get { return _UpdateTime; }
		}
		public virtual int EditCount { 
			set { _EditCount = value; }
			get { return _EditCount; }
		}
		public virtual int CheckCount { 
			set { _CheckCount = value; }
			get { return _CheckCount; }
		}
		public virtual int UserId { 
			set { _UserId = value; }
			get { return _UserId; }
		}

	}


} // namespace
