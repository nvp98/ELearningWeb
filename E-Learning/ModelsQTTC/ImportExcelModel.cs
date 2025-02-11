using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.ModelsQTTC
{
    public class ImportExcelModel
    {
        public HttpPostedFileBase FileExcel { get; set; }
    }
}