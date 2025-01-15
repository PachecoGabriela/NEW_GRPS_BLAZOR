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

    public partial class Sample : XPObject
    {
        Period fPeriod;
        [Association(@"SampleReferencesPeriod")]
        public Period Period
        {
            get { return fPeriod; }
            set { SetPropertyValue<Period>(nameof(Period), ref fPeriod, value); }
        }
        int fSampleType;
        public int SampleType
        {
            get { return fSampleType; }
            set { SetPropertyValue<int>(nameof(SampleType), ref fSampleType, value); }
        }
        int fSampleSize;
        public int SampleSize
        {
            get { return fSampleSize; }
            set { SetPropertyValue<int>(nameof(SampleSize), ref fSampleSize, value); }
        }
        double fTotal;
        public double Total
        {
            get { return fTotal; }
            set { SetPropertyValue<double>(nameof(Total), ref fTotal, value); }
        }
        [Association(@"SampleCategoryReferencesSample")]
        public XPCollection<SampleCategory> SampleCategories { get { return GetCollection<SampleCategory>(nameof(SampleCategories)); } }
    }

}
