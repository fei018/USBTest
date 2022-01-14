﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USBCommon;
using LoginUserManager;

namespace USBModel
{
    public class USBAdminDatabaseHelp
    {
        private readonly ISqlSugarClient _db;

        public USBAdminDatabaseHelp(string connString)
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
                    //_db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UsbRegistered>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_PerComputer>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_AgentSetting>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_UsbRequest>();
                    _db.CodeFirst.SetStringDefaultLength(100).InitTables<Tbl_EmailSetting>();

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

        // UsbWhitelist

        #region + public async Task<string> UsbWhitelist_Get()
        public async Task<string> UsbWhitelist_Get()
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbRequest>()
                                    .Where(u => u.RequestState == UsbRequestStateType.Approve)
                                    .ToListAsync();

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("USB Whitelist is empty in database.");
                }

                StringBuilder whitelist = new StringBuilder();

                // UsbIdentity encode to Base64                   
                foreach (var u in query)
                {
                    whitelist.AppendLine(Base64CodeHelp.Base64Encode(u.UsbIdentity));
                }
                return whitelist.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        // UsbRegistry

        #region UsbRegistry
        //#region + public async Task Insert_UsbRegistered(UsbInfo usb)
        //public async Task Insert_UsbRegistered(Tbl_UsbRegistered usb)
        //{
        //    try
        //    {
        //        var usbInDb = await _db.Queryable<Tbl_UsbRegistered>()
        //                             .Where(u => u.UsbIdentity == usb.UsbIdentity)
        //                             .FirstAsync();

        //        if (usbInDb == null)
        //        {
        //            var succeed = await _db.Insertable(usb).ExecuteCommandIdentityIntoEntityAsync();
        //            if (!succeed)
        //            {
        //                throw new Exception("UsbRegistered insert fail.");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //#endregion

        //#region + public async Task<int> Get_RegisteredUsbCount()
        //public async Task<int> Get_RegisteredUsbCount()
        //{
        //    try
        //    {
        //        var query = await _db.Queryable<Tbl_UsbRegistered>().CountAsync();

        //        return query;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //#endregion

        //#region + public async Task<(int total, List<Tbl_UsbRegistered> list)> Get_UsbRegisteredList(int pageIndex, int pageSize)
        //public async Task<(int total, List<Tbl_UsbRegistered> list)> Get_UsbRegisteredList(int pageIndex, int pageSize)
        //{
        //    try
        //    {
        //        RefAsync<int> total = new RefAsync<int>();
        //        var query = await _db.Queryable<Tbl_UsbRegistered>()
        //                                .OrderBy(u => u.Id, OrderByType.Desc)
        //                                .ToPageListAsync(pageIndex, pageSize, total);

        //        if (query == null || query.Count <= 0)
        //        {
        //            throw new Exception("Nothing UsbRegistered in Database.");
        //        }

        //        return (total.Value, query);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //#endregion
        #endregion

        // UsbRequest

        #region + public async Task<int> UsbRequest_TotalCount()
        public async Task<int> UsbRequest_TotalCount()
        {
            try
            {
                var total = await _db.Queryable<Tbl_UsbRequest>().CountAsync();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public async Task<int> UsbRequest_TotalCount_ByState(string state)
        public async Task<int> UsbRequest_TotalCount_ByState(string state)
        {
            try
            {
                var total = await _db.Queryable<Tbl_UsbRequest>()
                                        .Where(u => u.RequestState == state)
                                        .CountAsync();
                return total;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public async Task<Tbl_UsbRegRequest> UsbRequest_Insert(Tbl_UsbRegRequest usb)
        public async Task<Tbl_UsbRequest> UsbRequest_Insert(Tbl_UsbRequest usb)
        {
            try
            {
                var exist = await _db.Queryable<Tbl_UsbRequest>().FirstAsync(u => u.UsbIdentity == usb.UsbIdentity);
                if (exist != null)
                {
                    return exist;
                }

                usb.RequestState = UsbRequestStateType.UnderReview;
                usb.RequestStateChangeTime = DateTime.Now;
                var usbR = await _db.Insertable(usb).ExecuteReturnEntityAsync();
                return usbR;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public async Task<(int total, List<Tbl_UsbRegRequest> list)> UsbRequest_Get_All(int pageIdnex, int pageSize)
        public async Task<(int total, List<Tbl_UsbRequest> list)> UsbRequest_Get_All(int pageIdnex, int pageSize)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_UsbRequest>()
                                        .OrderBy(u => u.RequestStateChangeTime, OrderByType.Desc)
                                        .ToPageListAsync(pageIdnex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Nothing UsbRegRequest in Database.");
                }

                return (total.Value, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int total, List<Tbl_UsbRequest> list)> UsbRequest_Get_ByStateType(int pageIdnex, int pageSize, string stateType)
        public async Task<(int total, List<Tbl_UsbRequest> list)> UsbRequest_Get_ByStateType(int pageIdnex, int pageSize, string stateType)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var query = await _db.Queryable<Tbl_UsbRequest>()
                                        .Where(u=> u.RequestState == stateType)
                                        .OrderBy(u => u.RequestStateChangeTime, OrderByType.Desc)
                                        .ToPageListAsync(pageIdnex, pageSize, total);

                if (query == null || query.Count <= 0)
                {
                    throw new Exception("Cannot find any Tbl_UsbRequest.");
                }

                return (total.Value, query);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<Tbl_UsbRegRequest> UsbRequest_Get_ById(int id)
        public async Task<Tbl_UsbRequest> UsbRequest_Get_ById(int id)
        {
            try
            {
                var query = await _db.Queryable<Tbl_UsbRequest>().InSingleAsync(id);
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

        #region + public async Task<Tbl_UsbRequest> UsbRequest_ToApprove_ById(int id)
        public async Task<Tbl_UsbRequest> UsbRequest_ToApprove_ById(int id)
        {
            try
            {
                var usbRegRequest = await UsbRequest_Get_ById(id);

                // set Tbl_UsbRegRequest state is Approve
                usbRegRequest.RequestState = UsbRequestStateType.Approve;
                usbRegRequest.RequestStateChangeTime = DateTime.Now;
                await _db.Updateable(usbRegRequest).ExecuteCommandAsync();

                return usbRegRequest;

                //// approve 的 USB save to Tbl_UsbRegistered
                //var usb = new Tbl_UsbRegistered
                //{
                //    DeviceDescription = usbRegRequest.DeviceDescription,
                //    Manufacturer = usbRegRequest.Manufacturer,
                //    Pid = usbRegRequest.Pid,
                //    Vid = usbRegRequest.Vid,
                //    Product = usbRegRequest.Product,
                //    SerialNumber = usbRegRequest.SerialNumber,
                //    UsbIdentity = usbRegRequest.UsbIdentity
                //};
                //await Insert_UsbRegistered(usb);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<Tbl_UsbRequest> UsbRequest_ToReject_ById(int id)
        public async Task<Tbl_UsbRequest> UsbRequest_ToReject_ById(int id)
        {
            try
            {
                var query = await UsbRequest_Get_ById(id);

                query.RequestState = UsbRequestStateType.Reject;
                query.RequestStateChangeTime = DateTime.Now;

                await _db.Updateable(query).ExecuteCommandAsync();

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region MyRegion
        public async Task UsbRequest_Delete_ById(int id)
        {
            try
            {
                await _db.Deleteable<Tbl_UsbRequest>().In(u => u.Id, id).ExecuteCommandAsync();

                return;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region + public async Task<UsbRequestVM> UsbRequestVM_Get_ById(int id)
        public async Task<UsbRequestVM> UsbRequestVM_Get_ById(int id)
        {
            try
            {
                var usb = await _db.Queryable<Tbl_UsbRequest>().InSingleAsync(id);
                if (usb == null)
                {
                    throw new Exception("Tbl_UsbRegRequest cannot find, Id: " + id);
                }

                var com = await PerComputer_Get_ByIdentity(usb.RequestComputerIdentity);

                var vm = new UsbRequestVM(usb,com);

                return vm;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region + public async Task<(int total, List<UsbRequestVM> list)> UsbRequestVM_Get_ByStateType(int pageIdnex, int pageSize, string stateType)
        public async Task<(int total, List<UsbRequestVM> list)> UsbRequestVM_Get_ByStateType(int pageIdnex, int pageSize, string stateType)
        {
            try
            {
                RefAsync<int> total = new RefAsync<int>();
                var usbList = await _db.Queryable<Tbl_UsbRequest>()
                                        .LeftJoin<Tbl_PerComputer>((u, c) => u.RequestComputerIdentity == c.ComputerIdentity)
                                        .Where(u => u.RequestState == stateType)
                                        .OrderBy(u => u.RequestStateChangeTime, OrderByType.Desc)
                                        .Select((u, c) => new { usb = u, com = c })
                                        .ToPageListAsync(pageIdnex, pageSize, total);

                if (usbList == null || usbList.Count <= 0)
                {
                    throw new Exception("Cannot find any Tbl_UsbRequest.");
                }

                var vmlist = new List<UsbRequestVM>();
                foreach (var list in usbList)
                {
                    vmlist.Add(new UsbRequestVM(list.usb, list.com));
                }

                return (total.Value, vmlist);
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

        #region +  public async Task<Tbl_PerComputer> PerComputer_Get_ById(int id)
        public async Task<Tbl_PerComputer> PerComputer_Get_ById(int id)
        {
            try
            {
                var query = await _db.Queryable<Tbl_PerComputer>().InSingleAsync(id);
                if (query == null)
                {
                    throw new Exception("Cannot find Tbl_PerComputer, Id: " + id);
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<(int totalCount,List<Tbl_PerComputer> list)> PerComputer_Get_All(int index, int size)
        public async Task<(int totalCount, List<Tbl_PerComputer> list)> PerComputer_Get_All(int index, int size)
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

        #region + public async Task<List<Tbl_PerComputer>> PerComputer_Get_ByIdentity(string computerIdentity)
        public async Task<Tbl_PerComputer> PerComputer_Get_ByIdentity(string computerIdentity)
        {
            try
            {
                var query = await _db.Queryable<Tbl_PerComputer>().Where(c => c.ComputerIdentity == computerIdentity).FirstAsync();
                if (query == null)
                {
                    throw new Exception("Cannot find Tbl_PerComputer, Identity: " + computerIdentity);
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task PerComputer_InsertOrUpdate(Tbl_PerComputer com)
        public async Task PerComputer_InsertOrUpdate(Tbl_PerComputer com)
        {
            try
            {
                com.LastSeen = DateTime.Now;

                var queryCom = await _db.Queryable<Tbl_PerComputer>()
                                     .Where(c => c.ComputerIdentity == com.ComputerIdentity)
                                     .FirstAsync();
                
                if (queryCom == null)
                {
                    
                    await _db.Insertable(com).ExecuteCommandAsync();
                }
                else
                {
                    com.Id = queryCom.Id;

                    await _db.Updateable(com).ExecuteCommandAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        // EmailSetting

        #region + public async Task<Tbl_EmailSetting> EmailSetting_Get()
        public async Task<Tbl_EmailSetting> EmailSetting_Get()
        {
            try
            {
                var query = await _db.Queryable<Tbl_EmailSetting>().FirstAsync();

                if (query == null)
                {
                    throw new Exception("Cannot find Tbl_EmailSetting.");
                }

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region + public async Task<Tbl_EmailSetting> EmailSetting_Update(Tbl_EmailSetting email)
        public async Task<Tbl_EmailSetting> EmailSetting_Update(Tbl_EmailSetting email)
        {
            try
            {
                var isUpdate = await _db.Updateable(email).ExecuteCommandHasChangeAsync();

                if (!isUpdate)
                {
                    throw new Exception("Tbl_EmailSetting update fail.");
                }

                return email;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
