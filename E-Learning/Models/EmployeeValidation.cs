using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class EmployeeValidation
    {
        public int ID { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        public int IDPhongBan { get; set; }
        public string PhongBan { get; set; }
        public string TenQuyen { get; set; }
        public bool IsGV { get; set; } 

    }

    public class JqueryDatatableParam
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
    }

}