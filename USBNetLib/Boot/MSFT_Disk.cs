using System;
using System.Collections.Generic;

namespace USBNetLib
{
    public class MSFT_Disk
    {
        public String Path;
        public String FriendlyName;
        public UInt32 Number;       
        public String Model;      
        public Boolean IsOffline;
        public UInt16 BusType;
        public Boolean IsReadOnly;
        public Boolean IsSystem;
        public Boolean IsClustered;
        public Boolean IsBoot;
        public UInt64 Size;
        public String SerialNumber;


        //public String Location;
        //public String Manufacturer;
        //public new String UniqueId;
        //public UInt16 UniqueIdFormat;        
        //public String FirmwareVersion;               
        //public UInt64 AllocatedSize;
        //public UInt32 LogicalSectorSize;
        //public UInt32 PhysicalSectorSize;
        //public UInt64 LargestFreeExtent;
        //public UInt32 NumberOfPartitions;
        //public UInt16 ProvisioningType;
        //public UInt16 OperationalStatus;
        //public UInt16 HealthStatus;       
        //public UInt16 PartitionStyle;
        //public UInt32 Signature;
        //public String Guid;      
        //public UInt16 OfflineReason;
        //public Boolean BootFromDisk;

        private static Dictionary<ushort, string> BusTypeMap => new Dictionary<ushort, string>()
        {
            {0, "Unknown"},
            {1, "SCSI"},
            {2, "ATAPI"},
            {3, "ATA"},
            {4, "1394"},
            {5, "SSA"},
            {6, "Fibre Channel"},
            {7, "USB"},
            {8, "RAID"},
            {9, "iSCSI"},
            {10, "SAS"},
            {11, "SATA"},
            {12, "SD"},
            {13, "MMC"},
            {14, "Virtual"},
            {15, "File Backed Virtual"},
            {16, "Storage Spaces"},
            {17, "NVMe"}
        };
    }
}
