using System;
using System.IO;
using System.Security.AccessControl;

namespace USBNotifyLib
{
    public class AgentManager
    {
        #region + public static void Startup()
        public static void Startup()
        {
            try
            {
                // registry 讀取 UsbFilterEnable 設定
                if (AgentRegistry.UsbFilterEnabled)
                {
                    // 載入 UsbWhitelist cache
                    UsbWhitelistHelp.Reload_UsbWhitelist();

                    // 掃描過濾所有 usb disk
                    new UsbFilter().Filter_Scan_All_USB_Disk();
                }

                // 執行 agent 定時任務
                AgentTimer.ReloadTask();              
            }
            catch (Exception ex)
            {
                AgentLogger.Error(ex.Message);
            }
        }
        #endregion

        #region Stop()
        public static void Stop()
        {
            AgentTimer.ClearTimerTask();
        }
        #endregion

        #region + public static void SetDirACL_AuthenticatedUsers_Modify(string dirPath)
        public static void SetDirACL_AuthenticatedUsers_Modify(string dirPath)
        {
            var dirInfo = new DirectoryInfo(dirPath);
            
            var dirACL = dirInfo.GetAccessControl();

            var rule = new FileSystemAccessRule("Authenticated Users",
                    FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

            dirACL.AddAccessRule(rule);
            dirInfo.SetAccessControl(dirACL);
        }
        #endregion
    }
}
