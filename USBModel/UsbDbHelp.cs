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
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UsbInfo>();
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UserComputer>();
                _db.CodeFirst.InitTables<Tbl_AgentSetting>();

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
                if (!com.UsbFilterEnabled)
                {
                    throw new Exception("UserUsbFilterEnabled is false.");
                }

                // UsbFilterDb
                var filterDb = await _db.Queryable<Tbl_UsbInfo>().ToListAsync();
                if (filterDb == null || filterDb.Count <= 0) throw new Exception("UsbFilterDb is null or empty in database.");

                var sb = new StringBuilder();
                foreach (var u in filterDb)
                {
                    sb.AppendLine(Base64Encode(u.UsbIdentity));
                }

                var filter = new GetUsbFilterDbHttp
                {
                    UsbFilterDb = sb.ToString(),
                    UserUsbFilterEnabled = com.UsbFilterEnabled,
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
        public async Task<IAgentSettingHttp> GetAgentSetting()
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

        #region + public async Task<(int total, List<UsbInfo> list)> GetAllUsbInfo(int pageIndex, int pageSize)
        public async Task<(int total, List<Tbl_UsbInfo> list)> GetAllUsbInfo(int pageIndex, int pageSize)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_UsbInfo>().ToPageListAsync(pageIndex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbInfo in Database.");
                }

                return (total.Value, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<List<UsbInfo>> GetRegisteredUsbList()
        public async Task<List<Tbl_UsbInfo>> GetRegisteredUsbList()
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbInfo>()
                                    .Where(u => u.IsRegistered == true)
                                    .ToListAsync();

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing registered of UsbInfo in database.");
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
        public async Task<List<Tbl_UsbInfo>> GetUnregisterUsbList(int pageIndex, int pageSize)
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbInfo>()
                                .Where(u => u.IsRegistered == false)
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

                var query = await _db.Queryable<Tbl_UserUsbHistory, Tbl_UsbInfo, Tbl_UserComputer>((h, u, c) =>
                                        h.UsbIdentity == u.UsbIdentity && h.ComputerIdentity == c.ComputerIdentity)
                                        .OrderBy(h => h.PluginTime)
                                        .Select((h, u, c) => new { his = h, usb = u, com = c })
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
                    usbList.Add(new UserUsbHistoryDetail(q.his, q.usb, q.com));
                }
                return (total.Value, usbList);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int totalCount, List<UsbHistoryDetail> list)> GetUsbHistoryDetailList(int pageIndex, int pageSize)
        public async Task<(int totalCount, List<UserUsbHistoryDetail> list)> GetUsbHistoryDetailListByComputerIdentity(string computerIdentity, int pageIndex, int pageSize)
        {
            try
            {
                var total = new RefAsync<int>();

                var query = await _db.Queryable<Tbl_UserUsbHistory, Tbl_UsbInfo>((h, u) =>
                                        h.ComputerIdentity == computerIdentity && h.UsbIdentity == u.UsbIdentity)
                                        .OrderBy(h => h.PluginTime)
                                        .Select((h, u) => new { his = h, usb = u })
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
                    usbList.Add(new UserUsbHistoryDetail(q.his, q.usb));
                }
                return (total.Value, usbList);
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
        public async Task Register_Usb(Tbl_UsbInfo usb)
        {
            try
            {
                var usbInDb = await _db.Queryable<Tbl_UsbInfo>()
                                     .Where(u => u.UsbIdentity == usb.UsbIdentity)
                                     .FirstAsync();

                if (usbInDb == null)
                {
                    usb.IsRegistered = true;
                    await _db.Insertable(usb).ExecuteCommandAsync();
                }
                else
                {
                    if (usbInDb.IsRegistered)
                    {
                        throw new Exception("USB Registered: " + usbInDb.ToString());
                    }
                    else
                    {
                        usbInDb.IsRegistered = true;
                        await _db.Updateable(usbInDb).ExecuteCommandAsync();
                    }
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

        #region + private async Task Insert_UserUsb(UserUsb usb)
        private async Task Insert_UserUsb(Tbl_UsbInfo usb)
        {
            try
            {
                var usbInDb = await _db.Queryable<Tbl_UsbInfo>()
                                    .Where(u => u.UsbIdentity == usb.UsbIdentity)
                                    .FirstAsync();

                if (usbInDb == null)
                {
                    usb.IsRegistered = false;
                    await _db.Insertable(usb).ExecuteCommandAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + private async Task Insert_UsbHistory(UsbHistory usb)
        private async Task Insert_UsbHistory(Tbl_UserUsbHistory usb)
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

        #region + public async Task Update_PostComputerUsbHistory(PostComUsbInfo post)
        /// <summary>
        /// update UserComputer and UsbInfo and UsbHistory to database
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task Update_PostUsbHistory(PostUserUsbHistoryHttp post)
        {
            try
            {
                var com = post.UserComputer as Tbl_UserComputer;
                if (com != null)
                {
                    await Update_UserComputer(com);
                }

                var usb = post.UsbInfo as Tbl_UsbInfo;
                if (usb != null)
                {
                    await Insert_UserUsb(usb);
                }

                var history = post.UserUsbHistory as Tbl_UserUsbHistory;
                if (history != null)
                {
                    await Insert_UsbHistory(history);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
