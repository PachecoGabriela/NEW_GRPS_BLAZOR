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

    public partial class Part : IGroupNameWrapper, IPartGroupFilter, IPartTypeFilter, IImportFromExcel
    {
        public Part(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
