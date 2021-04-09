
using System;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Esy.Base.Application.Model
{

    /// <summary>
    /// 用户信息表 Esy_Sys_User table.
    /// </summary>
	[Table("Esy_Sys_User")]
	public partial class EsySysUser
	{  
	   /* 
  F_UserId
  F_EnCode
  F_Account
  F_Password
  F_Secretkey
  F_RealName
  F_NickName
  F_HeadIcon
  F_QuickQuery
  F_SimpleSpelling
  F_Gender
  F_Birthday
  F_Mobile
  F_Telephone
  F_Email
  F_OICQ
  F_WeChat
  F_CompanyId
  F_DepartmentId
  F_SecurityLevel
  F_OpenId
  F_Question
  F_AnswerQuestion
  F_CheckOnLine
  F_AllowStartTime
  F_AllowEndTime
  F_LockStartDate
  F_LockEndDate
  F_SortCode
  F_DeleteMark
  F_EnabledMark
  F_Description
  F_CreateDate
  F_CreateUserId
  F_CreateUserName
  F_ModifyDate
  F_ModifyUserId
  F_ModifyUserName
  F_UserType
  F_UniqueId
  F_CpieUseState
  F_Balance
  F_RegType
  F_HistoryOrder
  F_TotalAmount
  F_TotalConsumption
  F_Address
  F_CarNum
  */ 

	    
        #region 待写入字段集合 可抽象出来
		private bool _IsWriteFiled = true; // 默认记录赋值过的字段
        [WriteFiled]
        public System.Collections.Generic.List<System.Reflection.PropertyInfo> _wf 
						= new System.Collections.Generic.List<System.Reflection.PropertyInfo>();
						
        public void SetWriteFiled(bool ib = true) => this._IsWriteFiled = ib;
		#endregion
		 
        public EsySysUser() {
            this._IsWriteFiled = false;
        }
        public EsySysUser(bool isWrite) {
            this._IsWriteFiled = isWrite;
        }

        #region FieldName
		public static readonly string  Field_F_UserId = "F_UserId"; 
		public static readonly string  Field_F_EnCode = "F_EnCode"; 
		public static readonly string  Field_F_Account = "F_Account"; 
		public static readonly string  Field_F_Password = "F_Password"; 
		public static readonly string  Field_F_Secretkey = "F_Secretkey"; 
		public static readonly string  Field_F_RealName = "F_RealName"; 
		public static readonly string  Field_F_NickName = "F_NickName"; 
		public static readonly string  Field_F_HeadIcon = "F_HeadIcon"; 
		public static readonly string  Field_F_QuickQuery = "F_QuickQuery"; 
		public static readonly string  Field_F_SimpleSpelling = "F_SimpleSpelling"; 
		public static readonly string  Field_F_Gender = "F_Gender"; 
		public static readonly string  Field_F_Birthday = "F_Birthday"; 
		public static readonly string  Field_F_Mobile = "F_Mobile"; 
		public static readonly string  Field_F_Telephone = "F_Telephone"; 
		public static readonly string  Field_F_Email = "F_Email"; 
		public static readonly string  Field_F_OICQ = "F_OICQ"; 
		public static readonly string  Field_F_WeChat = "F_WeChat"; 
		public static readonly string  Field_F_CompanyId = "F_CompanyId"; 
		public static readonly string  Field_F_DepartmentId = "F_DepartmentId"; 
		public static readonly string  Field_F_SecurityLevel = "F_SecurityLevel"; 
		public static readonly string  Field_F_OpenId = "F_OpenId"; 
		public static readonly string  Field_F_Question = "F_Question"; 
		public static readonly string  Field_F_AnswerQuestion = "F_AnswerQuestion"; 
		public static readonly string  Field_F_CheckOnLine = "F_CheckOnLine"; 
		public static readonly string  Field_F_AllowStartTime = "F_AllowStartTime"; 
		public static readonly string  Field_F_AllowEndTime = "F_AllowEndTime"; 
		public static readonly string  Field_F_LockStartDate = "F_LockStartDate"; 
		public static readonly string  Field_F_LockEndDate = "F_LockEndDate"; 
		public static readonly string  Field_F_SortCode = "F_SortCode"; 
		public static readonly string  Field_F_DeleteMark = "F_DeleteMark"; 
		public static readonly string  Field_F_EnabledMark = "F_EnabledMark"; 
		public static readonly string  Field_F_Description = "F_Description"; 
		public static readonly string  Field_F_CreateDate = "F_CreateDate"; 
		public static readonly string  Field_F_CreateUserId = "F_CreateUserId"; 
		public static readonly string  Field_F_CreateUserName = "F_CreateUserName"; 
		public static readonly string  Field_F_ModifyDate = "F_ModifyDate"; 
		public static readonly string  Field_F_ModifyUserId = "F_ModifyUserId"; 
		public static readonly string  Field_F_ModifyUserName = "F_ModifyUserName"; 
		public static readonly string  Field_F_UserType = "F_UserType"; 
		public static readonly string  Field_F_UniqueId = "F_UniqueId"; 
		public static readonly string  Field_F_CpieUseState = "F_CpieUseState"; 
		public static readonly string  Field_F_Balance = "F_Balance"; 
		public static readonly string  Field_F_RegType = "F_RegType"; 
		public static readonly string  Field_F_HistoryOrder = "F_HistoryOrder"; 
		public static readonly string  Field_F_TotalAmount = "F_TotalAmount"; 
		public static readonly string  Field_F_TotalConsumption = "F_TotalConsumption"; 
		public static readonly string  Field_F_Address = "F_Address"; 
		public static readonly string  Field_F_CarNum = "F_CarNum"; 
		#endregion

        #region Field
		private string _F_UserId ; 
		private string _F_EnCode ; 
		private string _F_Account ; 
		private string _F_Password ; 
		private string _F_Secretkey ; 
		private string _F_RealName ; 
		private string _F_NickName ; 
		private string _F_HeadIcon ; 
		private string _F_QuickQuery ; 
		private string _F_SimpleSpelling ; 
		private int _F_Gender ; 
		private DateTime? _F_Birthday ; 
		private string _F_Mobile ; 
		private string _F_Telephone ; 
		private string _F_Email ; 
		private string _F_OICQ ; 
		private string _F_WeChat ; 
		private string _F_CompanyId ; 
		private string _F_DepartmentId ; 
		private int? _F_SecurityLevel ; 
		private int? _F_OpenId ; 
		private string _F_Question ; 
		private string _F_AnswerQuestion ; 
		private int? _F_CheckOnLine ; 
		private DateTime? _F_AllowStartTime ; 
		private DateTime? _F_AllowEndTime ; 
		private DateTime? _F_LockStartDate ; 
		private DateTime? _F_LockEndDate ; 
		private int _F_SortCode ; 
		private int _F_DeleteMark ; 
		private int _F_EnabledMark ; 
		private string _F_Description ; 
		private DateTime? _F_CreateDate ; 
		private string _F_CreateUserId ; 
		private string _F_CreateUserName ; 
		private DateTime? _F_ModifyDate ; 
		private string _F_ModifyUserId ; 
		private string _F_ModifyUserName ; 
		private int _F_UserType ; 
		private int _F_UniqueId ; 
		private int _F_CpieUseState ; 
		private decimal _F_Balance ; 
		private string _F_RegType ; 
		private int _F_HistoryOrder ; 
		private decimal _F_TotalAmount ; 
		private decimal _F_TotalConsumption ; 
		private string _F_Address ; 
		private string _F_CarNum ; 
        #endregion

        /// <summary>
        /// 用户主键
        /// </summary>
		[ExplicitKey] // 手动插入(主)键
		public virtual string F_UserId { 
			set { _F_UserId = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_UserId) ); }
			get { return _F_UserId; }
		}
        /// <summary>
        /// 工号
        /// </summary>
		public virtual string F_EnCode { 
			set { _F_EnCode = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_EnCode) ); }
			get { return _F_EnCode; }
		}
        /// <summary>
        /// 登录账户
        /// </summary>
		public virtual string F_Account { 
			set { _F_Account = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Account) ); }
			get { return _F_Account; }
		}
        /// <summary>
        /// 登录密码
        /// </summary>
		public virtual string F_Password { 
			set { _F_Password = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Password) ); }
			get { return _F_Password; }
		}
        /// <summary>
        /// 密码秘钥
        /// </summary>
		public virtual string F_Secretkey { 
			set { _F_Secretkey = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Secretkey) ); }
			get { return _F_Secretkey; }
		}
        /// <summary>
        /// 真实姓名
        /// </summary>
		public virtual string F_RealName { 
			set { _F_RealName = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_RealName) ); }
			get { return _F_RealName; }
		}
        /// <summary>
        /// 呢称
        /// </summary>
		public virtual string F_NickName { 
			set { _F_NickName = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_NickName) ); }
			get { return _F_NickName; }
		}
        /// <summary>
        /// 头像
        /// </summary>
		public virtual string F_HeadIcon { 
			set { _F_HeadIcon = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_HeadIcon) ); }
			get { return _F_HeadIcon; }
		}
        /// <summary>
        /// 快速查询
        /// </summary>
		public virtual string F_QuickQuery { 
			set { _F_QuickQuery = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_QuickQuery) ); }
			get { return _F_QuickQuery; }
		}
        /// <summary>
        /// 简拼
        /// </summary>
		public virtual string F_SimpleSpelling { 
			set { _F_SimpleSpelling = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_SimpleSpelling) ); }
			get { return _F_SimpleSpelling; }
		}
        /// <summary>
        /// 性别 1男 2女
        /// </summary>
		public virtual int F_Gender { 
			set { _F_Gender = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Gender) ); }
			get { return _F_Gender; }
		}
        /// <summary>
        /// 生日
        /// </summary>
		public virtual DateTime? F_Birthday { 
			set { _F_Birthday = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Birthday) ); }
			get { return _F_Birthday; }
		}
        /// <summary>
        /// 手机
        /// </summary>
		public virtual string F_Mobile { 
			set { _F_Mobile = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Mobile) ); }
			get { return _F_Mobile; }
		}
        /// <summary>
        /// 电话
        /// </summary>
		public virtual string F_Telephone { 
			set { _F_Telephone = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Telephone) ); }
			get { return _F_Telephone; }
		}
        /// <summary>
        /// 电子邮件
        /// </summary>
		public virtual string F_Email { 
			set { _F_Email = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Email) ); }
			get { return _F_Email; }
		}
        /// <summary>
        /// QQ号
        /// </summary>
		public virtual string F_OICQ { 
			set { _F_OICQ = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_OICQ) ); }
			get { return _F_OICQ; }
		}
        /// <summary>
        /// 微信号
        /// </summary>
		public virtual string F_WeChat { 
			set { _F_WeChat = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_WeChat) ); }
			get { return _F_WeChat; }
		}
        /// <summary>
        /// 机构主键
        /// </summary>
		public virtual string F_CompanyId { 
			set { _F_CompanyId = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_CompanyId) ); }
			get { return _F_CompanyId; }
		}
        /// <summary>
        /// 部门主键
        /// </summary>
		public virtual string F_DepartmentId { 
			set { _F_DepartmentId = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_DepartmentId) ); }
			get { return _F_DepartmentId; }
		}
        /// <summary>
        /// 安全级别
        /// </summary>
		public virtual int? F_SecurityLevel { 
			set { _F_SecurityLevel = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_SecurityLevel) ); }
			get { return _F_SecurityLevel; }
		}
        /// <summary>
        /// 单点登录标识
        /// </summary>
		public virtual int? F_OpenId { 
			set { _F_OpenId = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_OpenId) ); }
			get { return _F_OpenId; }
		}
        /// <summary>
        /// 密码提示问题
        /// </summary>
		public virtual string F_Question { 
			set { _F_Question = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Question) ); }
			get { return _F_Question; }
		}
        /// <summary>
        /// 密码提示答案
        /// </summary>
		public virtual string F_AnswerQuestion { 
			set { _F_AnswerQuestion = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_AnswerQuestion) ); }
			get { return _F_AnswerQuestion; }
		}
        /// <summary>
        /// 允许多用户同时登录
        /// </summary>
		public virtual int? F_CheckOnLine { 
			set { _F_CheckOnLine = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_CheckOnLine) ); }
			get { return _F_CheckOnLine; }
		}
        /// <summary>
        /// 允许登录时间开始
        /// </summary>
		public virtual DateTime? F_AllowStartTime { 
			set { _F_AllowStartTime = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_AllowStartTime) ); }
			get { return _F_AllowStartTime; }
		}
        /// <summary>
        /// 允许登录时间结束
        /// </summary>
		public virtual DateTime? F_AllowEndTime { 
			set { _F_AllowEndTime = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_AllowEndTime) ); }
			get { return _F_AllowEndTime; }
		}
        /// <summary>
        /// 暂停用户开始日期
        /// </summary>
		public virtual DateTime? F_LockStartDate { 
			set { _F_LockStartDate = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_LockStartDate) ); }
			get { return _F_LockStartDate; }
		}
        /// <summary>
        /// 暂停用户结束日期
        /// </summary>
		public virtual DateTime? F_LockEndDate { 
			set { _F_LockEndDate = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_LockEndDate) ); }
			get { return _F_LockEndDate; }
		}
        /// <summary>
        /// 排序码
        /// </summary>
		public virtual int F_SortCode { 
			set { _F_SortCode = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_SortCode) ); }
			get { return _F_SortCode; }
		}
        /// <summary>
        /// 删除标记 0 正常 1 删除
        /// </summary>
		public virtual int F_DeleteMark { 
			set { _F_DeleteMark = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_DeleteMark) ); }
			get { return _F_DeleteMark; }
		}
        /// <summary>
        /// 有效标志 1 有效 0 无效
        /// </summary>
		public virtual int F_EnabledMark { 
			set { _F_EnabledMark = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_EnabledMark) ); }
			get { return _F_EnabledMark; }
		}
        /// <summary>
        /// 备注
        /// </summary>
		public virtual string F_Description { 
			set { _F_Description = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Description) ); }
			get { return _F_Description; }
		}
        /// <summary>
        /// 创建日期
        /// </summary>
		public virtual DateTime? F_CreateDate { 
			set { _F_CreateDate = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_CreateDate) ); }
			get { return _F_CreateDate; }
		}
        /// <summary>
        /// 创建用户主键
        /// </summary>
		public virtual string F_CreateUserId { 
			set { _F_CreateUserId = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_CreateUserId) ); }
			get { return _F_CreateUserId; }
		}
        /// <summary>
        /// 创建用户
        /// </summary>
		public virtual string F_CreateUserName { 
			set { _F_CreateUserName = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_CreateUserName) ); }
			get { return _F_CreateUserName; }
		}
        /// <summary>
        /// 修改日期
        /// </summary>
		public virtual DateTime? F_ModifyDate { 
			set { _F_ModifyDate = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_ModifyDate) ); }
			get { return _F_ModifyDate; }
		}
        /// <summary>
        /// 修改用户主键
        /// </summary>
		public virtual string F_ModifyUserId { 
			set { _F_ModifyUserId = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_ModifyUserId) ); }
			get { return _F_ModifyUserId; }
		}
        /// <summary>
        /// 修改用户
        /// </summary>
		public virtual string F_ModifyUserName { 
			set { _F_ModifyUserName = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_ModifyUserName) ); }
			get { return _F_ModifyUserName; }
		}
        /// <summary>
        /// 1 系统平台用户  2 租户管理员 3 普通租户 4 充电桩用户
        /// </summary>
		public virtual int F_UserType { 
			set { _F_UserType = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_UserType) ); }
			get { return _F_UserType; }
		}
        /// <summary>
        /// 微信unionid   平台则为系统生成用5位唯一数字
        /// </summary>
		public virtual int F_UniqueId { 
			set { _F_UniqueId = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_UniqueId) ); }
			get { return _F_UniqueId; }
		}
        /// <summary>
        /// 电桩用户状态  默认0空闲中 1正准备开始充电 2充电进行中 3充电结束 4启动失败 5预约状态    
        /// </summary>
		public virtual int F_CpieUseState { 
			set { _F_CpieUseState = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_CpieUseState) ); }
			get { return _F_CpieUseState; }
		}
        /// <summary>
        /// 用户余额
        /// </summary>
		public virtual decimal F_Balance { 
			set { _F_Balance = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Balance) ); }
			get { return _F_Balance; }
		}
        /// <summary>
        /// 注册方式  微信 
        /// </summary>
		public virtual string F_RegType { 
			set { _F_RegType = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_RegType) ); }
			get { return _F_RegType; }
		}
        /// <summary>
        /// 历史订单数量
        /// </summary>
		public virtual int F_HistoryOrder { 
			set { _F_HistoryOrder = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_HistoryOrder) ); }
			get { return _F_HistoryOrder; }
		}
        /// <summary>
        /// 总充值金额
        /// </summary>
		public virtual decimal F_TotalAmount { 
			set { _F_TotalAmount = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_TotalAmount) ); }
			get { return _F_TotalAmount; }
		}
        /// <summary>
        /// 总消费金额
        /// </summary>
		public virtual decimal F_TotalConsumption { 
			set { _F_TotalConsumption = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_TotalConsumption) ); }
			get { return _F_TotalConsumption; }
		}
        /// <summary>
        /// 地址
        /// </summary>
		public virtual string F_Address { 
			set { _F_Address = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_Address) ); }
			get { return _F_Address; }
		}
        /// <summary>
        /// 车牌号
        /// </summary>
		public virtual string F_CarNum { 
			set { _F_CarNum = value; 
					if(_IsWriteFiled) _wf.Add(this.GetType().GetProperty(Field_F_CarNum) ); }
			get { return _F_CarNum; }
		}

	}

//	################
    /// <summary>
    /// 查询用 只存数据的实体 用户信息表  Esy_Sys_User_ table.
    /// </summary>
	[Table("Esy_Sys_User")]
	public partial class EsySysUser_
	{  
	   /*  F_UserId  F_EnCode  F_Account  F_Password  F_Secretkey  F_RealName  F_NickName  F_HeadIcon  F_QuickQuery  F_SimpleSpelling  F_Gender  F_Birthday  F_Mobile  F_Telephone  F_Email  F_OICQ  F_WeChat  F_CompanyId  F_DepartmentId  F_SecurityLevel  F_OpenId  F_Question  F_AnswerQuestion  F_CheckOnLine  F_AllowStartTime  F_AllowEndTime  F_LockStartDate  F_LockEndDate  F_SortCode  F_DeleteMark  F_EnabledMark  F_Description  F_CreateDate  F_CreateUserId  F_CreateUserName  F_ModifyDate  F_ModifyUserId  F_ModifyUserName  F_UserType  F_UniqueId  F_CpieUseState  F_Balance  F_RegType  F_HistoryOrder  F_TotalAmount  F_TotalConsumption  F_Address  F_CarNum  */ 

	      
        #region Field
		private string _F_UserId ;
		private string _F_EnCode ;
		private string _F_Account ;
		private string _F_Password ;
		private string _F_Secretkey ;
		private string _F_RealName ;
		private string _F_NickName ;
		private string _F_HeadIcon ;
		private string _F_QuickQuery ;
		private string _F_SimpleSpelling ;
		private int _F_Gender ;
		private DateTime? _F_Birthday ;
		private string _F_Mobile ;
		private string _F_Telephone ;
		private string _F_Email ;
		private string _F_OICQ ;
		private string _F_WeChat ;
		private string _F_CompanyId ;
		private string _F_DepartmentId ;
		private int? _F_SecurityLevel ;
		private int? _F_OpenId ;
		private string _F_Question ;
		private string _F_AnswerQuestion ;
		private int? _F_CheckOnLine ;
		private DateTime? _F_AllowStartTime ;
		private DateTime? _F_AllowEndTime ;
		private DateTime? _F_LockStartDate ;
		private DateTime? _F_LockEndDate ;
		private int _F_SortCode ;
		private int _F_DeleteMark ;
		private int _F_EnabledMark ;
		private string _F_Description ;
		private DateTime? _F_CreateDate ;
		private string _F_CreateUserId ;
		private string _F_CreateUserName ;
		private DateTime? _F_ModifyDate ;
		private string _F_ModifyUserId ;
		private string _F_ModifyUserName ;
		private int _F_UserType ;
		private int _F_UniqueId ;
		private int _F_CpieUseState ;
		private decimal _F_Balance ;
		private string _F_RegType ;
		private int _F_HistoryOrder ;
		private decimal _F_TotalAmount ;
		private decimal _F_TotalConsumption ;
		private string _F_Address ;
		private string _F_CarNum ;
        #endregion

        public short Gen { get; set; }

        /// <summary>
        /// 用户主键
        /// </summary>
		[ExplicitKey] // 手动插入(主)键
		public virtual string F_UserId { 
			set { _F_UserId = value; }
			get { return _F_UserId; }
		}
        /// <summary>
        /// 工号
        /// </summary>
		public virtual string F_EnCode { 
			set { _F_EnCode = value; }
			get { return _F_EnCode; }
		}
        /// <summary>
        /// 登录账户
        /// </summary>
		public virtual string F_Account { 
			set { _F_Account = value; }
			get { return _F_Account; }
		}
        /// <summary>
        /// 登录密码
        /// </summary>
		public virtual string F_Password { 
			set { _F_Password = value; }
			get { return _F_Password; }
		}
        /// <summary>
        /// 密码秘钥
        /// </summary>
		public virtual string F_Secretkey { 
			set { _F_Secretkey = value; }
			get { return _F_Secretkey; }
		}
        /// <summary>
        /// 真实姓名
        /// </summary>
		public virtual string F_RealName { 
			set { _F_RealName = value; }
			get { return _F_RealName; }
		}
        /// <summary>
        /// 呢称
        /// </summary>
		public virtual string F_NickName { 
			set { _F_NickName = value; }
			get { return _F_NickName; }
		}
        /// <summary>
        /// 头像
        /// </summary>
		public virtual string F_HeadIcon { 
			set { _F_HeadIcon = value; }
			get { return _F_HeadIcon; }
		}
        /// <summary>
        /// 快速查询
        /// </summary>
		public virtual string F_QuickQuery { 
			set { _F_QuickQuery = value; }
			get { return _F_QuickQuery; }
		}
        /// <summary>
        /// 简拼
        /// </summary>
		public virtual string F_SimpleSpelling { 
			set { _F_SimpleSpelling = value; }
			get { return _F_SimpleSpelling; }
		}
        /// <summary>
        /// 性别 1男 2女
        /// </summary>
		public virtual int F_Gender { 
			set { _F_Gender = value; }
			get { return _F_Gender; }
		}
        /// <summary>
        /// 生日
        /// </summary>
		public virtual DateTime? F_Birthday { 
			set { _F_Birthday = value; }
			get { return _F_Birthday; }
		}
        /// <summary>
        /// 手机
        /// </summary>
		public virtual string F_Mobile { 
			set { _F_Mobile = value; }
			get { return _F_Mobile; }
		}
        /// <summary>
        /// 电话
        /// </summary>
		public virtual string F_Telephone { 
			set { _F_Telephone = value; }
			get { return _F_Telephone; }
		}
        /// <summary>
        /// 电子邮件
        /// </summary>
		public virtual string F_Email { 
			set { _F_Email = value; }
			get { return _F_Email; }
		}
        /// <summary>
        /// QQ号
        /// </summary>
		public virtual string F_OICQ { 
			set { _F_OICQ = value; }
			get { return _F_OICQ; }
		}
        /// <summary>
        /// 微信号
        /// </summary>
		public virtual string F_WeChat { 
			set { _F_WeChat = value; }
			get { return _F_WeChat; }
		}
        /// <summary>
        /// 机构主键
        /// </summary>
		public virtual string F_CompanyId { 
			set { _F_CompanyId = value; }
			get { return _F_CompanyId; }
		}
        /// <summary>
        /// 部门主键
        /// </summary>
		public virtual string F_DepartmentId { 
			set { _F_DepartmentId = value; }
			get { return _F_DepartmentId; }
		}
        /// <summary>
        /// 安全级别
        /// </summary>
		public virtual int? F_SecurityLevel { 
			set { _F_SecurityLevel = value; }
			get { return _F_SecurityLevel; }
		}
        /// <summary>
        /// 单点登录标识
        /// </summary>
		public virtual int? F_OpenId { 
			set { _F_OpenId = value; }
			get { return _F_OpenId; }
		}
        /// <summary>
        /// 密码提示问题
        /// </summary>
		public virtual string F_Question { 
			set { _F_Question = value; }
			get { return _F_Question; }
		}
        /// <summary>
        /// 密码提示答案
        /// </summary>
		public virtual string F_AnswerQuestion { 
			set { _F_AnswerQuestion = value; }
			get { return _F_AnswerQuestion; }
		}
        /// <summary>
        /// 允许多用户同时登录
        /// </summary>
		public virtual int? F_CheckOnLine { 
			set { _F_CheckOnLine = value; }
			get { return _F_CheckOnLine; }
		}
        /// <summary>
        /// 允许登录时间开始
        /// </summary>
		public virtual DateTime? F_AllowStartTime { 
			set { _F_AllowStartTime = value; }
			get { return _F_AllowStartTime; }
		}
        /// <summary>
        /// 允许登录时间结束
        /// </summary>
		public virtual DateTime? F_AllowEndTime { 
			set { _F_AllowEndTime = value; }
			get { return _F_AllowEndTime; }
		}
        /// <summary>
        /// 暂停用户开始日期
        /// </summary>
		public virtual DateTime? F_LockStartDate { 
			set { _F_LockStartDate = value; }
			get { return _F_LockStartDate; }
		}
        /// <summary>
        /// 暂停用户结束日期
        /// </summary>
		public virtual DateTime? F_LockEndDate { 
			set { _F_LockEndDate = value; }
			get { return _F_LockEndDate; }
		}
        /// <summary>
        /// 排序码
        /// </summary>
		public virtual int F_SortCode { 
			set { _F_SortCode = value; }
			get { return _F_SortCode; }
		}
        /// <summary>
        /// 删除标记 0 正常 1 删除
        /// </summary>
		public virtual int F_DeleteMark { 
			set { _F_DeleteMark = value; }
			get { return _F_DeleteMark; }
		}
        /// <summary>
        /// 有效标志 1 有效 0 无效
        /// </summary>
		public virtual int F_EnabledMark { 
			set { _F_EnabledMark = value; }
			get { return _F_EnabledMark; }
		}
        /// <summary>
        /// 备注
        /// </summary>
		public virtual string F_Description { 
			set { _F_Description = value; }
			get { return _F_Description; }
		}
        /// <summary>
        /// 创建日期
        /// </summary>
		public virtual DateTime? F_CreateDate { 
			set { _F_CreateDate = value; }
			get { return _F_CreateDate; }
		}
        /// <summary>
        /// 创建用户主键
        /// </summary>
		public virtual string F_CreateUserId { 
			set { _F_CreateUserId = value; }
			get { return _F_CreateUserId; }
		}
        /// <summary>
        /// 创建用户
        /// </summary>
		public virtual string F_CreateUserName { 
			set { _F_CreateUserName = value; }
			get { return _F_CreateUserName; }
		}
        /// <summary>
        /// 修改日期
        /// </summary>
		public virtual DateTime? F_ModifyDate { 
			set { _F_ModifyDate = value; }
			get { return _F_ModifyDate; }
		}
        /// <summary>
        /// 修改用户主键
        /// </summary>
		public virtual string F_ModifyUserId { 
			set { _F_ModifyUserId = value; }
			get { return _F_ModifyUserId; }
		}
        /// <summary>
        /// 修改用户
        /// </summary>
		public virtual string F_ModifyUserName { 
			set { _F_ModifyUserName = value; }
			get { return _F_ModifyUserName; }
		}
        /// <summary>
        /// 1 系统平台用户  2 租户管理员 3 普通租户 4 充电桩用户
        /// </summary>
		public virtual int F_UserType { 
			set { _F_UserType = value; }
			get { return _F_UserType; }
		}
        /// <summary>
        /// 微信unionid   平台则为系统生成用5位唯一数字
        /// </summary>
		public virtual int F_UniqueId { 
			set { _F_UniqueId = value; }
			get { return _F_UniqueId; }
		}
        /// <summary>
        /// 电桩用户状态  默认0空闲中 1正准备开始充电 2充电进行中 3充电结束 4启动失败 5预约状态    
        /// </summary>
		public virtual int F_CpieUseState { 
			set { _F_CpieUseState = value; }
			get { return _F_CpieUseState; }
		}
        /// <summary>
        /// 用户余额
        /// </summary>
		public virtual decimal F_Balance { 
			set { _F_Balance = value; }
			get { return _F_Balance; }
		}
        /// <summary>
        /// 注册方式  微信 
        /// </summary>
		public virtual string F_RegType { 
			set { _F_RegType = value; }
			get { return _F_RegType; }
		}
        /// <summary>
        /// 历史订单数量
        /// </summary>
		public virtual int F_HistoryOrder { 
			set { _F_HistoryOrder = value; }
			get { return _F_HistoryOrder; }
		}
        /// <summary>
        /// 总充值金额
        /// </summary>
		public virtual decimal F_TotalAmount { 
			set { _F_TotalAmount = value; }
			get { return _F_TotalAmount; }
		}
        /// <summary>
        /// 总消费金额
        /// </summary>
		public virtual decimal F_TotalConsumption { 
			set { _F_TotalConsumption = value; }
			get { return _F_TotalConsumption; }
		}
        /// <summary>
        /// 地址
        /// </summary>
		public virtual string F_Address { 
			set { _F_Address = value; }
			get { return _F_Address; }
		}
        /// <summary>
        /// 车牌号
        /// </summary>
		public virtual string F_CarNum { 
			set { _F_CarNum = value; }
			get { return _F_CarNum; }
		}

	}


} // namespace
