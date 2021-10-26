using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;


namespace USBNetLib
{
    public class BootDiskHelper
    {
        private readonly NotifyDiskHelp _notifyDiskHelp;
        private readonly PolicyTableHelp _policyTableHelp;
        private readonly PolicyRule _policyRule;
        private readonly USBBusController _usbBus;

        public BootDiskHelper()
        {
            _notifyDiskHelp = new NotifyDiskHelp();
            _policyTableHelp = new PolicyTableHelp();
            _usbBus = new USBBusController();
            _policyRule = new PolicyRule();
        }

        private List<MSFT_Disk> Get_USB_Disk_List()
        {
            var scope = new ManagementScope(@"\\.\ROOT\Microsoft\Windows\Storage");
            var query = new ObjectQuery("SELECT * FROM MSFT_Disk where BusType=\"USB\"");
            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                var disks = searcher.Get();

                var list = new List<MSFT_Disk>();

                foreach (ManagementObject disk in disks)
                {
                    var d = Set_MSFT_Disk_Info(disk);

                    list.Add(d);
                }

                return list;
            }
        }

        private MSFT_Disk Get_Disk_By_Path(string diskPath)
        {
            var scope = new ManagementScope(@"\\.\ROOT\Microsoft\Windows\Storage");
            var query = new ObjectQuery($"SELECT * FROM MSFT_Disk where Path=\"{diskPath}\"");
            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                var disks = searcher.Get();

                if (disks.Count <= 0)
                {
                    throw new Exception("Cannot find the disk path is: " + diskPath);
                }
                if (disks.Count > 1)
                {
                    throw new Exception("More than 1 number of disk path is: " + diskPath);
                }

                foreach (ManagementObject d in disks)
                {
                    return Set_MSFT_Disk_Info(d);
                }
                return null;
            }
        }

        private MSFT_Disk Set_MSFT_Disk_Info(ManagementObject disk)
        {
            var d = new MSFT_Disk
            {
                Path = Convert.ToString(disk["Path"]),
                BusType = Convert.ToUInt16(disk["BusType"]),
                IsBoot = Convert.ToBoolean(disk["IsBoot"]),
                IsClustered = Convert.ToBoolean(disk["IsClustered"]),
                IsOffline = Convert.ToBoolean(disk["IsOffline"]),
                IsReadOnly = Convert.ToBoolean(disk["IsReadOnly"]),
                IsSystem = Convert.ToBoolean(disk["IsSystem"]),             
                FriendlyName = Convert.ToString(disk["FriendlyName"]),
                Model = Convert.ToString(disk["Model"]),
                Number = Convert.ToUInt32(disk["Number"]),
                Size = Convert.ToUInt64(disk["Size"]),

                //AllocatedSize = Convert.ToUInt64(disk["AllocatedSize"]),
                //BootFromDisk = Convert.ToBoolean(disk["BootFromDisk"]),                
                //FirmwareVersion = Convert.ToString(disk["FirmwareVersion"]),                
                //Guid = Convert.ToString(disk["Guid"]),
                //HealthStatus = Convert.ToUInt16(disk["HealthStatus"]),            
                //LargestFreeExtent = Convert.ToUInt64(disk["LargestFreeExtent"]),
                //Location = Convert.ToString(disk["Location"]),
                //LogicalSectorSize = Convert.ToUInt32(disk["LogicalSectorSize"]),
                //Manufacturer = Convert.ToString(disk["Manufacturer"]),               
                //NumberOfPartitions = Convert.ToUInt32(disk["NumberOfPartitions"]),
                //OfflineReason = Convert.ToUInt16(disk["OfflineReason"]),
                //OperationalStatus = ((UInt16[])disk["OperationalStatus"])[0],
                //PartitionStyle = Convert.ToUInt16(disk["PartitionStyle"]),                
                //PhysicalSectorSize = Convert.ToUInt32(disk["PhysicalSectorSize"]),
                //ProvisioningType = Convert.ToUInt16(disk["ProvisioningType"]),
                //SerialNumber = Convert.ToString(disk["SerialNumber"]),
                //Signature = Convert.ToUInt32(disk["Signature"]),               
                //UniqueId = Convert.ToString(disk["UniqueId"]),
                //UniqueIdFormat = Convert.ToUInt16(disk["UniqueIdFormat"]),
                //ObjectId = Convert.ToString(disk["ObjectId"]),
                //PassThroughClass = Convert.ToString(disk["PassThroughClass"]),
                //PassThroughIds = Convert.ToString(disk["PassThroughIds"]),
                //PassThroughNamespace = Convert.ToString(disk["PassThroughNamespace"]),
                //PassThroughServer = Convert.ToString(disk["PassThroughServer"])
            };
            return d;
        }

        private void Get_UsbDisk_ParentDeviceId(string diskPath)
        {
            var usb = new NotifyUSB { DiskPath = diskPath };
            if(_notifyDiskHelp.Find_UsbDeviceId_By_DiskPath_SetupDi(ref usb))
            {
                if (_usbBus.Find_VidPidSerial_In_UsbBus(ref usb))
                {
                    if ( !_policyTableHelp.IsMatchPolicyTable(usb) )
                    {
                        _policyRule.NotMatch_In_PolicyTable(usb);
                    }
                }                
            }
        }
        
        // Powershell
        //public void SetDiskIsReadOnly(MSFT_Disk disk, bool isReadOnly)
        //{
        //    if (disk.IsReadOnly)
        //    {
        //        return;
        //    }

        //    using (PowerShell ps = PowerShell.Create())
        //    {
        //        ps.AddCommand("set-disk")
        //            .AddParameter("Path", disk.Path)
        //            .AddParameter("IsReadOnly", isReadOnly);

        //        var result = ps.Invoke();
        //        if (ps.HadErrors)
        //        {
        //            _logger.Error($"{disk.FriendlyName},{disk.Model},{disk.UniqueId},{disk.SerialNumber}");
        //            if (ps.Streams.Error.Count > 0)
        //            {
        //                foreach (var e in ps.Streams.Error)
        //                {
        //                    _logger.Error(e.Exception.Message);
        //                }
        //            }
        //        }
        //    }
        //}


        
    }
}
