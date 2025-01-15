﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using GRPS_BLAZOR.Module.BusinessObjects.Base;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema
{

    public partial class DimPartType : CustomBaseClass
    {
        Period fActiveFrom;
        [Association(@"DimPartTypeReferencesPeriod")]
        public Period ActiveFrom
        {
            get { return fActiveFrom; }
            set { SetPropertyValue<Period>(nameof(ActiveFrom), ref fActiveFrom, value); }
        }
        Period fActiveTo;
        [Association(@"DimPartTypeReferencesPeriod1")]
        public Period ActiveTo
        {
            get { return fActiveTo; }
            set { SetPropertyValue<Period>(nameof(ActiveTo), ref fActiveTo, value); }
        }
        Company fCompany;
        [Association(@"DimPartTypeReferencesCompany")]
        public Company Company
        {
            get { return fCompany; }
            set { SetPropertyValue<Company>(nameof(Company), ref fCompany, value); }
        }
        string fPartTypeName;
        public string PartTypeName
        {
            get { return fPartTypeName; }
            set { SetPropertyValue<string>(nameof(PartTypeName), ref fPartTypeName, value); }
        }
        string fPart_Group_Level1;
        public string Part_Group_Level1
        {
            get { return fPart_Group_Level1; }
            set { SetPropertyValue<string>(nameof(Part_Group_Level1), ref fPart_Group_Level1, value); }
        }
        string fPart_Group_Level2;
        public string Part_Group_Level2
        {
            get { return fPart_Group_Level2; }
            set { SetPropertyValue<string>(nameof(Part_Group_Level2), ref fPart_Group_Level2, value); }
        }
        string fPart_Group_Level3;
        public string Part_Group_Level3
        {
            get { return fPart_Group_Level3; }
            set { SetPropertyValue<string>(nameof(Part_Group_Level3), ref fPart_Group_Level3, value); }
        }
        int fBaseOid;
        public int BaseOid
        {
            get { return fBaseOid; }
            set { SetPropertyValue<int>(nameof(BaseOid), ref fBaseOid, value); }
        }
    }

}
