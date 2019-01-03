
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the SynNote table.
    /// </summary>
	[Table("SynNote")]
	public partial class SynNote
	{  
	   /*  Id  Content  NoteDate  Name  UserId  IsDel  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public SynNote(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_Id = "Id"; 
		public static readonly string  Field_Content = "Content"; 
		public static readonly string  Field_NoteDate = "NoteDate"; 
		public static readonly string  Field_Name = "Name"; 
		public static readonly string  Field_UserId = "UserId"; 
		public static readonly string  Field_IsDel = "IsDel"; 
		#endregion

        #region Field
		private int _Id ; 
		private string _Content ; 
		private DateTime? _NoteDate ; 
		private string _Name ; 
		private int _UserId ; 
		private bool _IsDel ; 
        #endregion

		[Key]
		public virtual int Id { 
			set { _Id = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Id) ); }
			get { return _Id; }
		}
		public virtual string Content { 
			set { _Content = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Content) ); }
			get { return _Content; }
		}
		public virtual DateTime? NoteDate { 
			set { _NoteDate = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_NoteDate) ); }
			get { return _NoteDate; }
		}
		public virtual string Name { 
			set { _Name = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Name) ); }
			get { return _Name; }
		}
		public virtual int UserId { 
			set { _UserId = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_UserId) ); }
			get { return _UserId; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_IsDel) ); }
			get { return _IsDel; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  SynNote_ table.
    /// </summary>
	[Table("SynNote")]
	public partial class SynNote_
	{  
	   /*  Id  Content  NoteDate  Name  UserId  IsDel  */ 

	      
        #region Field
		private int _Id ;
		private string _Content ;
		private DateTime? _NoteDate ;
		private string _Name ;
		private int _UserId ;
		private bool _IsDel ;
        #endregion

		[Key]
		public virtual int Id { 
			set { _Id = value; }
			get { return _Id; }
		}
		public virtual string Content { 
			set { _Content = value; }
			get { return _Content; }
		}
		public virtual DateTime? NoteDate { 
			set { _NoteDate = value; }
			get { return _NoteDate; }
		}
		public virtual string Name { 
			set { _Name = value; }
			get { return _Name; }
		}
		public virtual int UserId { 
			set { _UserId = value; }
			get { return _UserId; }
		}
		public virtual bool IsDel { 
			set { _IsDel = value; }
			get { return _IsDel; }
		}

	}






} // namespace
