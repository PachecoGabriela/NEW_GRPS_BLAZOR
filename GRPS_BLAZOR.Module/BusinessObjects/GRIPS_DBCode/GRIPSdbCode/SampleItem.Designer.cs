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

    public partial class SampleItem : XPObject
    {
        Product fProduct;
        [Association(@"SampleItemReferencesProduct")]
        public Product Product
        {
            get { return fProduct; }
            set { SetPropertyValue<Product>(nameof(Product), ref fProduct, value); }
        }
        decimal fSampleQuantity;
        public decimal SampleQuantity
        {
            get { return fSampleQuantity; }
            set { SetPropertyValue<decimal>(nameof(SampleQuantity), ref fSampleQuantity, value); }
        }
        decimal fSampleValue;
        public decimal SampleValue
        {
            get { return fSampleValue; }
            set { SetPropertyValue<decimal>(nameof(SampleValue), ref fSampleValue, value); }
        }
        SampleCategory fCategory;
        [Association(@"SampleItemReferencesSampleCategory")]
        public SampleCategory Category
        {
            get { return fCategory; }
            set { SetPropertyValue<SampleCategory>(nameof(Category), ref fCategory, value); }
        }
    }

}
