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
namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema
{

    public partial class SupplierAddress : XPObject
    {
        string fAddress1;
        public string Address1
        {
            get { return fAddress1; }
            set { SetPropertyValue<string>(nameof(Address1), ref fAddress1, value); }
        }
        string fAddress2;
        public string Address2
        {
            get { return fAddress2; }
            set { SetPropertyValue<string>(nameof(Address2), ref fAddress2, value); }
        }
        string fAddress3;
        public string Address3
        {
            get { return fAddress3; }
            set { SetPropertyValue<string>(nameof(Address3), ref fAddress3, value); }
        }
        string fAddress4;
        public string Address4
        {
            get { return fAddress4; }
            set { SetPropertyValue<string>(nameof(Address4), ref fAddress4, value); }
        }
        string fAddress5;
        public string Address5
        {
            get { return fAddress5; }
            set { SetPropertyValue<string>(nameof(Address5), ref fAddress5, value); }
        }
        string fPostCode;
        public string PostCode
        {
            get { return fPostCode; }
            set { SetPropertyValue<string>(nameof(PostCode), ref fPostCode, value); }
        }
        Supplier fSupplier;
        [Association(@"SupplierAddressReferencesSupplier")]
        public Supplier Supplier
        {
            get { return fSupplier; }
            set { SetPropertyValue<Supplier>(nameof(Supplier), ref fSupplier, value); }
        }
    }

}
