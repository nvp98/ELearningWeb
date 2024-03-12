using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Models
{
    public class NotificationView
    {
        public int ID { get; set; }
        public string NoiDungTB { get; set; }
        public DateTime NgayTB { get; set; }
        public Nullable<int> TinhTrang { get; set; }
    }
}