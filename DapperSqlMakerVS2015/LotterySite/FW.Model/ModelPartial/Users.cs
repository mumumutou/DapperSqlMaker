
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the Users table.
    /// </summary>
	[Table("Users")]
	public partial class Users
	{  
	   /*  Id  UserName  Password  CreateTime  Remark  IsDel  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public Users(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_Id = "Id"; 
		public static readonly string  Field_UserName = "UserName"; 
		public static readonly string  Field_Password = "Password"; 
		public static readonly string  Field_CreateTime = "CreateTime"; 
		public static readonly string  Field_Remark = "Remark"; 
		public static readonly string  Field_IsDel = "IsDel"; 
		#endregion

        #region Field
		private int _Id ; 
		private string _UserName ; 
		private string _Password ; 
		private DateTime _CreateTime ; 
		private string _Remark ; 
		private bool _IsDel ; 
        #endregion

		[Key]
		public virtual int Id { 
			set { _Id = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Id) ); }
			get { return _Id; }
		}
		public virtual string UserName { 
			set { _UserName = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_UserName) ); }
			get { return _UserName; }
		}
		public virtual string Password { 
			set { _Password = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Password) ); }
			get { return _Password; }
		}
		public virtual DateTime CreateTime { 
			set { _CreateTime = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_CreateTime) ); }
			get { return _CreateTime; }
		}
		public virtual string Remark { 
			set { _Remark = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Remark) ); }
			get { return _Remark; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_IsDel) ); }
			get { return _IsDel; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  Users_ table.
    /// </summary>
	[Table("Users")]
	public partial class Users_
	{  
	   /*  Id  UserName  Password  CreateTime  Remark  IsDel  */ 

	      
        #region Field
		private int _Id ;
		private string _UserName ;
		private string _Password ;
		private DateTime _CreateTime ;
		private string _Remark ;
		private bool _IsDel ;
        #endregion

		[Key]
		public virtual int Id { 
			set { _Id = value; }
			get { return _Id; }
		}
		public virtual string UserName { 
			set { _UserName = value; }
			get { return _UserName; }
		}
		public virtual string Password { 
			set { _Password = value; }
			get { return _Password; }
		}
		public virtual DateTime CreateTime { 
			set { _CreateTime = value; }
			get { return _CreateTime; }
		}
		public virtual string Remark { 
			set { _Remark = value; }
			get { return _Remark; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; }
			get { return _IsDel; }
		}

	}


} // namespace
