using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USBCommon;
using LoginUserManager;

namespace USBModel
{
    public class UsbDbHelp
    {
        private readonly ISqlSugarClient _db;

        public UsbDbHelp(string connString)
        {
            _db = GetSqlClient(connString);
        }

        #region + private ISqlSugarClient GetSqlClient(string conn)
        private ISqlSugarClient GetSqlClient(string conn)
        {
            ISqlSugarClient client = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = conn,
                DbType = DbType.SqlServer,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            return client;
        }
        #endregion

        #region + public void TryCreateDatabase()
        public void TryCreateDatabase()
        {
            if (_db.DbMaintenance.CreateDatabase())
            {
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UserUsbHistory>();
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UsbRegistered>();
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UserComputer>();
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_AgentSetting>();
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UserAgentSetting>();

                _db.CodeFirst.InitTables<LoginUser>();
                _db.CodeFirst.InitTables<LoginErrorCountLimit>();
            }
        }
        #endregion

        #region Base64Encode
        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region + public async Task<UsbFilterDbHttp> GetUsbFilterDb(string comIdentity)
        public async Task<GetUsbFilterDbHttp> GetUsbFilterDb(string comIdentity)
        {
            try
            {
                var com = await _db.Queryable<Tbl_UserComputer>().FirstAsync(c => c.ComputerIdentity == comIdentity);

                if (com == null)
                {
                    throw new Exception("cannot find the UserComputer: " + comIdentity);
                }

                Tbl_UserAgentSetting setting = await _db.Queryable<Tbl_UserAgentSetting>().FirstAsync(a => a.ComputerIdentity == comIdentity);
                bool isFilterEnable = setting?.UsbFilterEnabled ?? true;                

                List<Tbl_UsbRegistered> usbInfos = null;
                StringBuilder filterDb = null;

                if (isFilterEnable)
                {
                    usbInfos = await _db.Queryable<Tbl_UsbRegistered>().ToListAsync();
                    if (usbInfos != null && usbInfos.Count > 0)
                    {
                        // UsbIdentity encode to Base64                   
                        foreach (var u in usbInfos)
                        {
                            filterDb.AppendLine(Base64Encode(u.UsbIdentity));
                        }
                    }                   
                }
                              
                var filter = new GetUsbFilterDbHttp
                {
                    UsbFilterDb = filterDb?.ToString(),
                    UserUsbFilterEnabled = isFilterEnable
                };

                return filter;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<t_AgentSetting> GetAgentSetting()
        public async Task<IAgentSetting> GetAgentSetting()
        {
            try
            {
                var query = await _db.Queryable<Tbl_AgentSetting>().FirstAsync();
                if (query == null)
                {
                    throw new Exception("AgentSetting is null.");
                }

                return query;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region +  public async Task<UserComputer> GetUserComputerById(int id)
        public async Task<Tbl_UserComputer> GetUserComputerById(int id)
        {
            try
            {
                var query = await _db.Queryable<Tbl_UserComputer>().InSingleAsync(id);
                if (query == null)
                {
                    throw new Exception("UserComputer cannot find. Id: " + id);
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int total, List<Tbl_UsbRegistered> list)> GetUsbRegisteredList(int pageIndex, int pageSize)
        public async Task<(int total, List<Tbl_UsbRegistered> list)> GetUsbRegisteredList(int pageIndex, int pageSize)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_UsbRegistered>().ToPageListAsync(pageIndex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbRegistered in Database.");
                }

                return (total.Value, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<int> GetRegisteredUsbCount()
        public async Task<int> GetRegisteredUsbCount()
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbRegistered>().CountAsync();

                if (query <= 0)
                {
                    throw new Exception("Nothing UsbRegistered in database.");
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<List<UsbInfo>> GetUnregisterUsbList(int pageIndex, int pageSize)
        public async Task<List<Tbl_UsbRegistered>> GetUnregisterUsbList(int pageIndex, int pageSize)
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbRegistered>()
                                        .ToPageListAsync(pageIndex, pageSize);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing Unregister Usb in Database.");
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int totalCount, List<UsbHistoryDetail> list)> GetUsbHistoryDetailList(int pageIndex, int pageSize)
        public async Task<(int totalCount, List<UserUsbHistoryDetail> list)> GetUsbHistoryDetailList(int pageIndex, int pageSize)
        {
            try
            {
                var total = new RefAsync<int>();

                var query = await _db.Queryable<Tbl_UserUsbHistory, Tbl_UserComputer>((h,c) => h.ComputerIdentity == c.ComputerIdentity)
                                        .OrderBy(h => h.PluginTime)
                                        .Select((h,c) => new { his = h,com = c })
                                        .ToPageListAsync(pageIndex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbHistory or UserUsb in database.");
                }

                //var pageList = query.OrderByDescending(o=>o.his.PluginTime)
                //                    .Skip((pageIndex - 1) * pageSize)
                //                    .Take(pageSize)
                //                    .ToList();

                var usbList = new List<UserUsbHistoryDetail>();
                foreach (var q in query)
                {
                    usbList.Add(new UserUsbHistoryDetail(q.com));
                }
                return (total.Value, usbList);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int totalCount, List<Tbl_UserUsbHistory> list)> GetUsbHistoryListByComputerIdentity(string computerIdentity,int pageIndex, int pageSize)
        public async Task<(int totalCount, List<Tbl_UserUsbHistory> list)> GetUsbHistoryListByComputerIdentity(string computerIdentity, int pageIndex, int pageSize)
        {
            try
            {
                var total = new RefAsync<int>();

                var query = await _db.Queryable<Tbl_UserUsbHistory>()
                                        .Where(h => h.ComputerIdentity == computerIdentity)
                                        .OrderBy(h => h.PluginTime)
                                        .ToPageListAsync(pageIndex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbHistory in database.");
                }

                return (total.Value, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int totalCount,List<UserComputer> list)> GetUserComputerList(int index, int size)
        public async Task<(int totalCount, List<Tbl_UserComputer> list)> GetUserComputerList(int index, int size)
        {
            try
            {
                var total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_UserComputer>().ToPageListAsync(index, size, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UserComputer in database.");
                }

                return (total.Value, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<List<UserComputer>> GetUserComputerListByIdentity(string computerIdentity)
        public async Task<List<Tbl_UserComputer>> GetUserComputerListByIdentity(string computerIdentity)
        {
            try
            {
                var query = await _db.Queryable<Tbl_UserComputer>().Where(c => c.ComputerIdentity == computerIdentity).ToListAsync();
                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UserComputer in database.");
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task Register_Usb(UsbInfo usb)
        public async Task Register_Usb(Tbl_UsbRegistered usb)
        {
            try
            {
                var usbInDb = await _db.Queryable<Tbl_UsbRegistered>()
                                     .Where(u => u.UsbIdentity == usb.UsbIdentity)
                                     .FirstAsync();

                if (usbInDb == null)
                {
                    await _db.Insertable(usb).ExecuteCommandAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task Update_UserComputer(UserComputer com)
        public async Task Update_UserComputer(Tbl_UserComputer com)
        {
            try
            {
                var query = await _db.Queryable<Tbl_UserComputer>()
                                     .Where(c => c.ComputerIdentity == com.ComputerIdentity)
                                     .FirstAsync();

                com.LastSeen = DateTime.Now;
                if (query == null)
                {
                    await _db.Insertable(com).ExecuteCommandAsync();
                }
                else
                {
                    com.Id = query.Id;

                    await _db.Updateable(com).ExecuteCommandAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task Insert_UsbHistory(UsbHistory usb)
        public async Task Insert_UsbHistory(Tbl_UserUsbHistory usb)
        {
            try
            {
                await _db.Insertable(usb).ExecuteCommandAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

       
    }
}
