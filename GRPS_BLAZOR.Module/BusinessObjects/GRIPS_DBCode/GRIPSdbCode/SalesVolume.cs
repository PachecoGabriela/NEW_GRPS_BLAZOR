﻿using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ExcelImport.Interfaces;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema
{

    public partial class SalesVolume : IImportFromExcel
    {
        public SalesVolume(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }

        public bool HasBOM
        {
            get => BOM != null;
        }
    }

}
