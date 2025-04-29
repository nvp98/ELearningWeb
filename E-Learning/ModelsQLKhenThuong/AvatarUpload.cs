using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsQLKhenThuong
{
    public class AvatarUpload
    {
        public string MaDoiTuong { get; set; }
        public string LoaiDoiTuong { get; set; }

        public string HoTenOrTenDonVi { get; set; }

        public HttpPostedFileBase File { get; set; }

        public string AvatarPreviewUrl { get; set; }
    }
}