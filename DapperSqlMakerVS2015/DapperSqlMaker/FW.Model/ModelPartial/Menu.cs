
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the Menu table.
    /// </summary>
	[Table("Menu")]
	public partial class Menu
	{  
	   /*  ID  ParentId  Title  img  href  seq  Target  IsDel  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public Menu() {
            this._IsWriteFiled = false;
        }
        public Menu(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_ID = "ID"; 
		public static readonly string  Field_ParentId = "ParentId"; 
		public static readonly string  Field_Title = "Title"; 
		public static readonly string  Field_img = "img"; 
		public static readonly string  Field_href = "href"; 
		public static readonly string  Field_seq = "seq"; 
		public static readonly string  Field_Target = "Target"; 
		public static readonly string  Field_IsDel = "IsDel"; 
		#endregion

        #region Field
		private int _ID ; 
		private int _ParentId ; 
		private string _Title ; 
		private string _img ; 
		private string _href ; 
		private int? _seq ; 
		private int _Target ; 
		private string _IsDel ; 
        #endregion

		[Key]
		public virtual int ID { 
			set { _ID = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_ID) ); }
			get { return _ID; }
		}
		public virtual int ParentId { 
			set { _ParentId = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_ParentId) ); }
			get { return _ParentId; }
		}
		public virtual string Title { 
			set { _Title = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Title) ); }
			get { return _Title; }
		}
		public virtual string img { 
			set { _img = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_img) ); }
			get { return _img; }
		}
		public virtual string href { 
			set { _href = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_href) ); }
			get { return _href; }
		}
		public virtual int? seq { 
			set { _seq = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_seq) ); }
			get { return _seq; }
		}
		public virtual int Target { 
			set { _Target = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Target) ); }
			get { return _Target; }
		}
		public virtual string IsDel { 
			set { _IsDel = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_IsDel) ); }
			get { return _IsDel; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  Menu_ table.
    /// </summary>
	[Table("Menu")]
	public partial class Menu_
	{  
	   /*  ID  ParentId  Title  img  href  seq  Target  IsDel  */ 

	      
        #region Field
		private int _ID ;
		private int _ParentId ;
		private string _Title ;
		private string _img ;
		private string _href ;
		private int? _seq ;
		private int _Target ;
		private string _IsDel ;
        #endregion

		[Key]
		public virtual int ID { 
			set { _ID = value; }
			get { return _ID; }
		}
		public virtual int ParentId { 
			set { _ParentId = value; }
			get { return _ParentId; }
		}
		public virtual string Title { 
			set { _Title = value; }
			get { return _Title; }
		}
		public virtual string img { 
			set { _img = value; }
			get { return _img; }
		}
		public virtual string href { 
			set { _href = value; }
			get { return _href; }
		}
		public virtual int? seq { 
			set { _seq = value; }
			get { return _seq; }
		}
		public virtual int Target { 
			set { _Target = value; }
			get { return _Target; }
		}
		public virtual string IsDel { 
			set { _IsDel = value; }
			get { return _IsDel; }
		}

	}


} // namespace
