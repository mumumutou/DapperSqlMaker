
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the Roles table.
    /// </summary>
	[Table("Roles")]
	public partial class Roles
	{  
	   /*  Id  Name  Remake  IsDel  MenusID  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public Roles() {
            this._IsWriteFiled = false;
        }
        public Roles(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_Id = "Id"; 
		public static readonly string  Field_Name = "Name"; 
		public static readonly string  Field_Remake = "Remake"; 
		public static readonly string  Field_IsDel = "IsDel"; 
		public static readonly string  Field_MenusID = "MenusID"; 
		#endregion

        #region Field
		private int _Id ; 
		private string _Name ; 
		private string _Remake ; 
		private bool _IsDel ; 
		private int _MenusID ; 
        #endregion

		[Key]
		public virtual int Id { 
			set { _Id = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Id) ); }
			get { return _Id; }
		}
		public virtual string Name { 
			set { _Name = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Name) ); }
			get { return _Name; }
		}
		public virtual string Remake { 
			set { _Remake = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Remake) ); }
			get { return _Remake; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_IsDel) ); }
			get { return _IsDel; }
		}
		public virtual int MenusID { 
			set { _MenusID = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_MenusID) ); }
			get { return _MenusID; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  Roles_ table.
    /// </summary>
	[Table("Roles")]
	public partial class Roles_
	{  
	   /*  Id  Name  Remake  IsDel  MenusID  */ 

	      
        #region Field
		private int _Id ;
		private string _Name ;
		private string _Remake ;
		private bool _IsDel ;
		private int _MenusID ;
        #endregion

		[Key]
		public virtual int Id { 
			set { _Id = value; }
			get { return _Id; }
		}
		public virtual string Name { 
			set { _Name = value; }
			get { return _Name; }
		}
		public virtual string Remake { 
			set { _Remake = value; }
			get { return _Remake; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; }
			get { return _IsDel; }
		}
		public virtual int MenusID { 
			set { _MenusID = value; }
			get { return _MenusID; }
		}

	}


} // namespace
