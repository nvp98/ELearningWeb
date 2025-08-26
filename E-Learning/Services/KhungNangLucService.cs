using DocumentFormat.OpenXml.Wordprocessing;
using E_Learning.Models;
using E_Learning.ModelsDTTH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Learning.Services
{
    public class KhungNangLucService
    {
        private readonly ELEARNINGEntities _context;

        public KhungNangLucService(ELEARNINGEntities context)
        {
            _context = context;
        }

        public void InsertKetQuaDG(List<FValueValidation> ListKQ)
        {
            
        }

    }
}