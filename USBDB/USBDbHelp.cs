using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace USBDB
{
    public class USBDbHelp
    {
        private readonly ISqlSugarClient _db;

        public USBDbHelp(string connString)
        {
            _db = GetSqlClient(connString);
        }

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

        public void CreateDb()
        {
            if (_db.DbMaintenance.CreateDatabase())
            {
                _db.CodeFirst.InitTables<UsbInfo>();
                _db.CodeFirst.InitTables<ComputerInfo>();
            }           
        }
    }
}
