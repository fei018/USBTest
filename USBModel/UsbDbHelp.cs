using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USBCommon;
using System.Linq;

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
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<UsbHistory>();
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<UserUsb>();
                _db.CodeFirst.SetStringDefaultLength(100).InitTables<UserComputer>();
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

        #region + public async Task<string> GetUsbFilterTable()
        public async Task<string> GetUsbFilterTable()
        {
            try
            {
                var usbs = await _db.Queryable<UserUsb>().ToListAsync();
                if (usbs == null || usbs.Count <= 0) return null;

                var sb = new StringBuilder();
                foreach (var u in usbs)
                {
                    sb.AppendLine(Base64Encode(u.UsbIdentity));
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region +  public async Task<UserComputer> GetUserComputerById(int id)
        public async Task<UserComputer> GetUserComputerById(int id)
        {
            try
            {
                var query = await _db.Queryable<UserComputer>().InSingleAsync(id);
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

        #region + public async Task<List<UserUsb>> GetAllUsb(int pageIndex, int pageSize)
        public async Task<(int total, List<UserUsb> list)> GetAllUsb(int pageIndex, int pageSize)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var query = await _db.Queryable<UserUsb>().ToPageListAsync(pageIndex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing Usb in Database.");
                }

                return (total.Value, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<List<UserUsb>> GetRegisteredUsbList()
        public async Task<List<UserUsb>> GetRegisteredUsbList()
        {
            try
            {
                var query = await _db.Queryable<UserUsb>()
                                    .Where(u => u.IsRegistered == true)
                                    .ToListAsync();

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("RegisteredUsb Db is Null or Empty.");
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<List<UserUsb>> GetRegisteredUsbListByComputerIdentity(string computerIdentity)
        public async Task<List<UserUsb>> GetRegisteredUsbListByComputerIdentity(string computerIdentity)
        {
            try
            {
                var query = await _db.Queryable<UserUsb>()
                                    .Where(u => u.PostComputerId == computerIdentity && u.IsRegistered == true)
                                    .ToListAsync();

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing RegisteredUsb in database.");
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<List<UserUsb>> GetUnregisterUsbList(int pageIndex, int pageSize)
        public async Task<List<UserUsb>> GetUnregisterUsbList(int pageIndex, int pageSize)
        {
            try
            {
                var query = await _db.Queryable<UserUsb>()
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
        public async Task<(int totalCount, List<UsbHistoryDetail> list)> GetUsbHistoryDetailList(int pageIndex, int pageSize)
        {
            try
            {
                var total = new RefAsync<int>();

                var query = await _db.Queryable<UsbHistory, UserUsb, UserComputer>((h, u, c) =>
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

                var usbList = new List<UsbHistoryDetail>();
                foreach (var q in query)
                {
                    usbList.Add(new UsbHistoryDetail(q.his, q.usb, q.com));
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
        public async Task<(int totalCount, List<UsbHistoryDetail> list)> GetUsbHistoryDetailListByComputerIdentity(string computerIdentity, int pageIndex, int pageSize)
        {
            try
            {
                var total = new RefAsync<int>();

                var query = await _db.Queryable<UsbHistory, UserUsb>((h, u) =>
                                        h.ComputerIdentity == computerIdentity && h.UsbIdentity == u.UsbIdentity)
                                        .OrderBy(h=>h.PluginTime)
                                        .Select((h, u) => new { his = h, usb = u})
                                        .ToPageListAsync(pageIndex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbHistory or UserUsb in database.");
                }

                //var pageList = query.OrderByDescending(o=>o.his.PluginTime)
                //                    .Skip((pageIndex - 1) * pageSize)
                //                    .Take(pageSize)
                //                    .ToList();

                var usbList = new List<UsbHistoryDetail>();
                foreach (var q in query)
                {
                    usbList.Add(new UsbHistoryDetail(q.his, q.usb));
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
        public async Task<(int totalCount, List<UserComputer> list)> GetUserComputerList(int index, int size)
        {
            try
            {
                var total = new RefAsync<int>();
                var query = await _db.Queryable<UserComputer>().ToPageListAsync(index, size, total);

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
        public async Task<List<UserComputer>> GetUserComputerListByIdentity(string computerIdentity)
        {
            try
            {
                var query = await _db.Queryable<UserComputer>().Where(c => c.ComputerIdentity == computerIdentity).ToListAsync();
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

        #region + public async Task Register_Usb(UserUsb usb)
        public async Task Register_Usb(UserUsb usb)
        {
            try
            {
                var usbInDb = await _db.Queryable<UserUsb>()
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
        public async Task Update_UserComputer(UserComputer com)
        {
            try
            {
                var query = await _db.Queryable<UserComputer>()
                                     .Where(c => c.ComputerIdentity == com.ComputerIdentity)
                                     .FirstAsync();

                com.UpdateTime = DateTime.Now;
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
        private async Task Insert_UserUsb(UserUsb usb)
        {
            try
            {
                var usbInDb = await _db.Queryable<UserUsb>()
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
        private async Task Insert_UsbHistory(UsbHistory usb)
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
        /// update UserComputer and UserUsb to database
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task Update_PostComputerUsbHistory(PostComUsbHistory post)
        {
            try
            {
                var com = post.ComputerInfo as UserComputer;
                if (com != null)
                {
                    await Update_UserComputer(com);
                }

                var usb = post.UsbInfo as UserUsb;
                if (usb != null)
                {
                    usb.PostComputerId = com?.ComputerIdentity;
                    await Insert_UserUsb(usb);
                }

                var history = post.UsbHistory as UsbHistory;
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
