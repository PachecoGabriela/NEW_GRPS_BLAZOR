using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using GRPS_BLAZOR.Module.Interfaces;
using ExcelImport.Interfaces;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema
{

    public partial class Product : IGroupNameWrapper, IProductGroupFilter, ISupplierFilter, IImportFromExcel
    {
        public Product(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }

        bool hasBOM;
        [NonPersistent]
        public bool HasBOM
        {
            get
            {
                if(fActiveBOM is null)
                {
                    hasBOM = false;
                }
                else
                {
                    hasBOM= true;
                }
                return hasBOM;
            }
        }





    }

}
