
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace FW.Model
{

    /// <summary>
    /// A class which represents the Skin table.
    /// </summary>
	[Table("Skin")]
	public partial class Skin
	{  
	   /*  Id  Name  Value  Type  IsDel  Remake  UserId  InsertDate  */ 

	    
        #region 待写入字段集合 可抽象出来
		public bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _WriteFiled 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
		#endregion
		 
        public Skin() {
            this._IsWriteFiled = false;
        }
        public Skin(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_Id = "Id"; 
		public static readonly string  Field_Name = "Name"; 
		public static readonly string  Field_Value = "Value"; 
		public static readonly string  Field_Type = "Type"; 
		public static readonly string  Field_IsDel = "IsDel"; 
		public static readonly string  Field_Remake = "Remake"; 
		public static readonly string  Field_UserId = "UserId"; 
		public static readonly string  Field_InsertDate = "InsertDate"; 
		#endregion

        #region Field
		private int _Id ; 
		private string _Name ; 
		private string _Value ; 
		private string _Type ; 
		private int _IsDel ; 
		private string _Remake ; 
		private int _UserId ; 
		private string _InsertDate ; 
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
		public virtual string Value { 
			set { _Value = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Value) ); }
			get { return _Value; }
		}
		public virtual string Type { 
			set { _Type = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_Type) ); }
			get { return _Type; }
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
		public virtual string InsertDate { 
			set { _InsertDate = value; 
					if(_IsWriteFiled) _WriteFiled.Add(this.GetType().GetProperty(Field_InsertDate) ); }
			get { return _InsertDate; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体  Skin_ table.
    /// </summary>
	[Table("Skin")]
	public partial class Skin_
	{  
	   /*  Id  Name  Value  Type  IsDel  Remake  UserId  InsertDate  */ 

	      
        #region Field
		private int _Id ;
		private string _Name ;
		private string _Value ;
		private string _Type ;
		private int _IsDel ;
		private string _Remake ;
		private int _UserId ;
		private string _InsertDate ;
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
		public virtual string Value { 
			set { _Value = value; }
			get { return _Value; }
		}
		public virtual string Type { 
			set { _Type = value; }
			get { return _Type; }
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
		public virtual string InsertDate { 
			set { _InsertDate = value; }
			get { return _InsertDate; }
		}

	}






} // namespace
