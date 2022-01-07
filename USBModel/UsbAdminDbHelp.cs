﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USBCommon;
using LoginUserManager;

namespace USBModel
{
    public class UsbAdminDbHelp
    {
        private readonly ISqlSugarClient _db;

        public UsbAdminDbHelp(string connString)
        {
            _db = GetSqlClient(connString);
        }

        // Db

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
            try
            {
                if (_db.DbMaintenance.CreateDatabase())
                {
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_PerUsbHistory>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UsbRegistered>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_PerComputer>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_AgentSetting>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UsbRegRequest>();

                    _db.CodeFirst.InitTables<LoginUser>();
                    _db.CodeFirst.InitTables<LoginErrorCountLimit>();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        // AgentSetting

        #region + public async Task<t_AgentSetting> Get_AgentSetting()
        public async Task<IAgentSetting> Get_AgentSetting()
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

        // UsbFilerDb

        #region + public async Task<string> Get_UsbWhitelist()
        public async Task<string> Get_UsbWhitelist()
        {
            StringBuilder filterDb = new StringBuilder();
            var query = await _db.Queryable<Tbl_UsbRegistered>().ToListAsync();
            if (query != null && query.Count > 0)
            {
                // UsbIdentity encode to Base64                   
                foreach (var u in query)
                {
                    filterDb.AppendLine(Base64CodeHelp.Base64Encode(u.UsbIdentity));
                }
            }
            return filterDb.ToString();
        }
        #endregion

        // UsbRegistry

        #region + public async Task Insert_UsbRegistered(UsbInfo usb)
        public async Task Insert_UsbRegistered(Tbl_UsbRegistered usb)
        {
            try
            {
                var usbInDb = await _db.Queryable<Tbl_UsbRegistered>()
                                     .Where(u => u.UsbIdentity == usb.UsbIdentity)
                                     .FirstAsync();

                if (usbInDb == null)
                {
                    var succeed = await _db.Insertable(usb).ExecuteCommandIdentityIntoEntityAsync();
                    if (!succeed)
                    {
                        throw new Exception("UsbRegistered insert fail.");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<int> Get_RegisteredUsbCount()
        public async Task<int> Get_RegisteredUsbCount()
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbRegistered>().CountAsync();

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int total, List<Tbl_UsbRegistered> list)> Get_UsbRegisteredList(int pageIndex, int pageSize)
        public async Task<(int total, List<Tbl_UsbRegistered> list)> Get_UsbRegisteredList(int pageIndex, int pageSize)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_UsbRegistered>()
                                        .OrderBy(u => u.Id, OrderByType.Desc)
                                        .ToPageListAsync(pageIndex, pageSize, total);

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

        // UsbRegRequest

        #region + public async Task<Tbl_UsbRegRequest> Insert_UsbRegRequest(Tbl_UsbRegRequest usb)
        public async Task<Tbl_UsbRegRequest> Insert_UsbRegRequest(Tbl_UsbRegRequest usb)
        {
            try
            {
                var exist = await _db.Queryable<Tbl_UsbRegRequest>().FirstAsync(u => u.UsbIdentity == usb.UsbIdentity);
                if (exist != null)
                {
                    return exist;
                }

                var usbR = await _db.Insertable(usb).ExecuteReturnEntityAsync();
                return usbR;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public async Task<(int total, List<Tbl_UsbRegRequest> list)> Get_UsbRegRequestList(int pageIdnex, int pageSize)
        public async Task<(int total, List<Tbl_UsbRegRequest> list)> Get_UsbRegRequestList(int pageIdnex, int pageSize)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_UsbRegRequest>()
                                        .OrderBy(u => u.RequestTime, OrderByType.Desc)
                                        .ToPageListAsync(pageIdnex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbRegRequest in Database.");
                }

                return (total, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<Tbl_UsbRegRequest> Get_UsbRegRequestById(int id)
        public async Task<Tbl_UsbRegRequest> Get_UsbRegRequestById(int id)
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbRegRequest>().InSingleAsync(id);
                if (query == null)
                {
                    throw new Exception("Tbl_UsbRegRequest cannot find, Id: " + id);
                }

                return query;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public async Task UsbRegRequestToRegistered(Tbl_UsbRegRequest usbRegRequest)
        public async Task UsbRegRequestToRegistered(Tbl_UsbRegRequest usbRegRequest)
        {
            try
            {
                if (usbRegRequest == null)
                {
                    throw new Exception("Tbl_UsbRegRequest is Null.");
                }

                var usb = new Tbl_UsbRegistered
                {
                    DeviceDescription = usbRegRequest.DeviceDescription,
                    Manufacturer = usbRegRequest.Manufacturer,
                    Pid = usbRegRequest.Pid,
                    Vid = usbRegRequest.Vid,
                    Product = usbRegRequest.Product,
                    SerialNumber = usbRegRequest.SerialNumber,
                    UsbIdentity = usbRegRequest.UsbIdentity
                };

                await Insert_UsbRegistered(usb);

                // 刪除 Tbl_UsbRegRequest
                //await _db.Deleteable(usbRegRequest).ExecuteCommandAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        // UsbHistory

        #region + public async Task Insert_UsbHistory(UsbHistory usb)
        public async Task Insert_UsbHistory(Tbl_PerUsbHistory usb)
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

        #region + public async Task<(int totalCount, List<UsbHistoryDetail> list)> Get_UsbHistoryVMList(int pageIndex, int pageSize)
        public async Task<(int totalCount, List<PerUsbHistoryVM> list)> Get_UsbHistoryVMList(int pageIndex, int pageSize)
        {
            try
            {
                var total = new RefAsync<int>();

                var query = await _db.Queryable<Tbl_PerUsbHistory>()
                                        .LeftJoin<Tbl_PerComputer>((h,c)=>h.ComputerIdentity == c.ComputerIdentity)
                                        .OrderBy(h => h.PluginTime, OrderByType.Desc)
                                        .Select((h, c) => new { his = h, com = c })
                                        .ToPageListAsync(pageIndex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbHistory or UserUsb in database.");
                }

                //var pageList = query.OrderByDescending(o=>o.his.PluginTime)
                //                    .Skip((pageIndex - 1) * pageSize)
                //                    .Take(pageSize)
                //                    .ToList();

                var usbList = new List<PerUsbHistoryVM>();
                foreach (var q in query)
                {
                    usbList.Add(new PerUsbHistoryVM(q.his,q.com));
                }
                return (total.Value, usbList);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int totalCount, List<Tbl_UserUsbHistory> list)> Get_UsbHistoryListByComputerIdentity(string computerIdentity,int pageIndex, int pageSize)
        public async Task<(int totalCount, List<Tbl_PerUsbHistory> list)> Get_UsbHistoryListByComputerIdentity(string computerIdentity, int pageIndex, int pageSize)
        {
            try
            {
                var total = new RefAsync<int>();

                var query = await _db.Queryable<Tbl_PerUsbHistory>()
                                        .Where(h => h.ComputerIdentity == computerIdentity)
                                        .OrderBy(h => h.PluginTime, OrderByType.Desc)
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


        // PerComputer

        #region +  public async Task<Tbl_PerComputer> Get_PerComputerById(int id)
        public async Task<Tbl_PerComputer> Get_PerComputerById(int id)
        {
            try
            {
                var query = await _db.Queryable<Tbl_PerComputer>().InSingleAsync(id);
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

        #region + public async Task<(int totalCount,List<Tbl_PerComputer> list)> Get_PerComputerList(int index, int size)
        public async Task<(int totalCount, List<Tbl_PerComputer> list)> Get_PerComputerList(int index, int size)
        {
            try
            {
                var total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_PerComputer>().ToPageListAsync(index, size, total);

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

        #region + public async Task<List<Tbl_PerComputer>> Get_PerComputerListByIdentity(string computerIdentity)
        public async Task<List<Tbl_PerComputer>> Get_PerComputerListByIdentity(string computerIdentity)
        {
            try
            {
                var query = await _db.Queryable<Tbl_PerComputer>().Where(c => c.ComputerIdentity == computerIdentity).ToListAsync();
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

        #region + public async Task Update_PerComputer(Tbl_PerComputer com)
        public async Task Update_PerComputer(Tbl_PerComputer com)
        {
            try
            {
                var query = await _db.Queryable<Tbl_PerComputer>()
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

    }
}