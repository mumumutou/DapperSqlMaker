
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the LOLkengbi table.
    /// </summary>
	[Table("LOLkengbi")]
	public partial class LOLkengbi
	{  
	   /*  Id  Name  Hero  Server  IsDel  Remake  UserId  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public LOLkengbi() {
            this._IsWriteFiled = false;
        }
        public LOLkengbi(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_Id = "Id"; 
		public static readonly string  Field_Name = "Name"; 
		public static readonly string  Field_Hero = "Hero"; 
		public static readonly string  Field_Server = "Server"; 
		public static readonly string  Field_IsDel = "IsDel"; 
		public static readonly string  Field_Remake = "Remake"; 
		public static readonly string  Field_UserId = "UserId"; 
		#endregion

        #region Field
		private int _Id ; 
		private string _Name ; 
		private string _Hero ; 
		private string _Server ; 
		private int _IsDel ; 
		private string _Remake ; 
		private int _UserId ; 
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
		public virtual string Hero { 
			set { _Hero = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Hero) ); }
			get { return _Hero; }
		}
		public virtual string Server { 
			set { _Server = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Server) ); }
			get { return _Server; }
		}
		public virtual int IsDel { 
			set { _IsDel = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_IsDel) ); }
			get { return _IsDel; }
		}
		public virtual string Remake { 
			set { _Remake = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Remake) ); }
			get { return _Remake; }
		}
		public virtual int UserId { 
			set { _UserId = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_UserId) ); }
			get { return _UserId; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  LOLkengbi_ table.
    /// </summary>
	[Table("LOLkengbi")]
	public partial class LOLkengbi_
	{  
	   /*  Id  Name  Hero  Server  IsDel  Remake  UserId  */ 

	      
        #region Field
		private int _Id ;
		private string _Name ;
		private string _Hero ;
		private string _Server ;
		private int _IsDel ;
		private string _Remake ;
		private int _UserId ;
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
		public virtual string Hero { 
			set { _Hero = value; }
			get { return _Hero; }
		}
		public virtual string Server { 
			set { _Server = value; }
			get { return _Server; }
		}
		public virtual int IsDel { 
			set { _IsDel = value; }
			get { return _IsDel; }
		}
		public virtual string Remake { 
			set { _Remake = value; }
			get { return _Remake; }
		}
		public virtual int UserId { 
			set { _UserId = value; }
			get { return _UserId; }
		}

	}


} // namespace
