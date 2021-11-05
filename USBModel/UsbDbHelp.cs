﻿using System;
using SqlSugar;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using USBCommon;

namespace UsbModel
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

        #region + public void CreateDatabase()
        public void CreateDatabase()
        {
            if (_db.DbMaintenance.CreateDatabase())
            {
                _db.CodeFirst.InitTables<RegisteredUsb>();
                _db.CodeFirst.InitTables<ComputerInfo>();
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
                var usbs = await _db.Queryable<RegisteredUsb>().ToListAsync();
                if (usbs == null || usbs.Count <= 0) return null;

                var sb = new StringBuilder();
                foreach (var u in usbs)
                {
                    sb.AppendLine(Base64Encode(u.ToFilterString()));
                }

                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task UpdateOrInsert_ComputerInfo_by_Json(string comjson)
        public async Task UpdateOrInsert_ComputerInfo_by_Json(string comjson)
        {
            try
            {
                ComputerInfo com = JsonConvert.DeserializeObject(comjson, typeof(IComputerInfo)) as ComputerInfo;

                if (com == null) throw new Exception("Json to ComputerInfo as Null.");

                var query = await _db.Queryable<ComputerInfo>()
                                     .Where(c => c.BiosSerial == com.BiosSerial && c.MacAddress == com.MacAddress)
                                     .FirstAsync();

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

        #region + public async Task Insert_RegisteredUsb(RegisteredUsb usb)
        public async Task Insert_RegisteredUsb(RegisteredUsb usb)
        {
            try
            {
                var query = await _db.Queryable<RegisteredUsb>()
                                     .Where(u=> u.Vid == usb.Vid && u.Pid == usb.Pid && u.SerialNumber == usb.SerialNumber)
                                     .FirstAsync();

                if (query == null)
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
    }
}