using System;
using SqlSugar;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using USBCommon;
using System.Collections.Generic;

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
                _db.CodeFirst.InitTables<UsbHistory>();
                _db.CodeFirst.InitTables<UserUsb>();
                _db.CodeFirst.InitTables<UserComputer>();
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

        #region + public async Task<List<RegisteredUsb>> GetRegisteredUsbList()
        public async Task<List<UserUsb>> GetRegisteredUsbList()
        {
            try
            {
                var query = await _db.Queryable<UserUsb>().ToListAsync();
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

        #region + public async Task<List<RegisteredUsb>> GetRegisteredUsbListByComputer(UserComputer com)
        public async Task<List<UserUsb>> GetRegisteredUsbListByComputer(UserComputer com)
        {
            try
            {
                var query = await _db.Queryable<UserUsb>()
                                    .Where(u=>u.RequestComputerId == com.ComputerIdentity)
                                    .ToListAsync();

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Find RegisteredUsb is Null or Empty.");
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
                    usb.RegisteredTime = DateTime.Now;
                    usb.IsRegistered = false;
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
                        usbInDb.RegisteredTime = DateTime.Now;
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
                    usb.RequestComputerId = com?.ComputerIdentity;
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
