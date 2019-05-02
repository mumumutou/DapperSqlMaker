
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
	   /*  Id  UserName  Password  CreateTime  Remark  IsDel  img  RolesId  SkinId  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public Users() {
            this._IsWriteFiled = false;
        }
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
		public static readonly string  Field_img = "img"; 
		public static readonly string  Field_RolesId = "RolesId"; 
		public static readonly string  Field_SkinId = "SkinId"; 
		#endregion

        #region Field
		private int _Id ; 
		private string _UserName ; 
		private string _Password ; 
		private DateTime _CreateTime ; 
		private string _Remark ; 
		private bool _IsDel ; 
		private string _img ; 
		private int _RolesId ; 
		private int _SkinId ; 
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
		public virtual string img { 
			set { _img = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_img) ); }
			get { return _img; }
		}
		public virtual int RolesId { 
			set { _RolesId = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_RolesId) ); }
			get { return _RolesId; }
		}
		public virtual int SkinId { 
			set { _SkinId = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_SkinId) ); }
			get { return _SkinId; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  Users_ table.
    /// </summary>
	[Table("Users")]
	public partial class Users_
	{  
	   /*  Id  UserName  Password  CreateTime  Remark  IsDel  img  RolesId  SkinId  */ 

	      
        #region Field
		private int _Id ;
		private string _UserName ;
		private string _Password ;
		private DateTime _CreateTime ;
		private string _Remark ;
		private bool _IsDel ;
		private string _img ;
		private int _RolesId ;
		private int _SkinId ;
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
		public virtual string img { 
			set { _img = value; }
			get { return _img; }
		}
		public virtual int RolesId { 
			set { _RolesId = value; }
			get { return _RolesId; }
		}
		public virtual int SkinId { 
			set { _SkinId = value; }
			get { return _SkinId; }
		}

	}


} // namespace
