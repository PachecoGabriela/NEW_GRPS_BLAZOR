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

    public partial class SampleCoefficient : XPObject
    {
        EnumInstance fSource;
        [Association(@"SampleCoefficientReferencesEnumInstance3")]
        public EnumInstance Source
        {
            get { return fSource; }
            set { SetPropertyValue<EnumInstance>(nameof(Source), ref fSource, value); }
        }
        EnumInstance fMaterial;
        [Association(@"SampleCoefficientReferencesEnumInstance")]
        public EnumInstance Material
        {
            get { return fMaterial; }
            set { SetPropertyValue<EnumInstance>(nameof(Material), ref fMaterial, value); }
        }
        EnumInstance fPackType;
        [Association(@"SampleCoefficientReferencesEnumInstance2")]
        public EnumInstance PackType
        {
            get { return fPackType; }
            set { SetPropertyValue<EnumInstance>(nameof(PackType), ref fPackType, value); }
        }
        double fTonnes;
        public double Tonnes
        {
            get { return fTonnes; }
            set { SetPropertyValue<double>(nameof(Tonnes), ref fTonnes, value); }
        }
        SampleCategory fCategory;
        [Association(@"SampleCoefficientReferencesSampleCategory")]
        public SampleCategory Category
        {
            get { return fCategory; }
            set { SetPropertyValue<SampleCategory>(nameof(Category), ref fCategory, value); }
        }
        EnumInstance fMaterialCategory;
        [Association(@"SampleCoefficientReferencesEnumInstance1")]
        public EnumInstance MaterialCategory
        {
            get { return fMaterialCategory; }
            set { SetPropertyValue<EnumInstance>(nameof(MaterialCategory), ref fMaterialCategory, value); }
        }
        double fAvgTonnes;
        public double AvgTonnes
        {
            get { return fAvgTonnes; }
            set { SetPropertyValue<double>(nameof(AvgTonnes), ref fAvgTonnes, value); }
        }
        double fMOETonnes;
        public double MOETonnes
        {
            get { return fMOETonnes; }
            set { SetPropertyValue<double>(nameof(MOETonnes), ref fMOETonnes, value); }
        }
    }

}
