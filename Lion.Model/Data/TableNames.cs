using System;

namespace Lion.Model
{
    public enum TableNames { __efmigrationshistory , sys_companies , sys_databases , sys_departs , sys_menus , sys_rolemenus , sys_roles , sys_userdeparts , sys_users }

    #region Ã¶¾Ù 
    public enum __efmigrationshistory { MigrationId, ProductVersion }
    public enum sys_companies { Guid, Account, ComName, SortName, Address, Person, Phone, Tell, Email, Url, IsActive, Reamrk }
    public enum sys_databases { Guid, DataName, ConnonStr, Reamrk }
    public enum sys_departs { Guid, Comid, Name, Pid, Sort, Remark, Ts }
    public enum sys_menus { Guid, Comid, Name, Url, Icon, Pid, Sort, Remark, Ts }
    public enum sys_rolemenus { Guid, Roleid, Menuid, Ts }
    public enum sys_roles { Guid, Comid, Name, Remark, Ts }
    public enum sys_userdeparts { Guid, Userid, Departid, Ts }
    public enum sys_users { Guid, Comid, Logincode, Pwd, Username, Roleid, Departid, State, Phone, Remark, Ts }
    #endregion
}