using SqlSugar;

namespace USBModel
{
    public class Tbl_PrintTemplate
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        public string SiteName { get; set; }

        public string SubnetAddr { get; set; }

        public string FileName { get; set; }



    }
}
