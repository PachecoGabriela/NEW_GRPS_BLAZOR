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

    public partial class CanadaReturnLine : XPObject
    {
        string fReference;
        public string Reference
        {
            get { return fReference; }
            set { SetPropertyValue<string>(nameof(Reference), ref fReference, value); }
        }
        string fDescription;
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue<string>(nameof(Description), ref fDescription, value); }
        }
        CanadaReturnLine fParentLine;
        [Association(@"CanadaReturnLineReferencesCanadaReturnLine")]
        public CanadaReturnLine ParentLine
        {
            get { return fParentLine; }
            set { SetPropertyValue<CanadaReturnLine>(nameof(ParentLine), ref fParentLine, value); }
        }
        CanadaReturn fParentReturn;
        [Association(@"CanadaReturnLineReferencesCanadaReturn")]
        public CanadaReturn ParentReturn
        {
            get { return fParentReturn; }
            set { SetPropertyValue<CanadaReturn>(nameof(ParentReturn), ref fParentReturn, value); }
        }
        int fLineID;
        public int LineID
        {
            get { return fLineID; }
            set { SetPropertyValue<int>(nameof(LineID), ref fLineID, value); }
        }
        int fRowID;
        public int RowID
        {
            get { return fRowID; }
            set { SetPropertyValue<int>(nameof(RowID), ref fRowID, value); }
        }
        int fLineType;
        public int LineType
        {
            get { return fLineType; }
            set { SetPropertyValue<int>(nameof(LineType), ref fLineType, value); }
        }
        string fShortName;
        public string ShortName
        {
            get { return fShortName; }
            set { SetPropertyValue<string>(nameof(ShortName), ref fShortName, value); }
        }
        [Association(@"CanadaReturnCellReferencesCanadaReturnLine")]
        public XPCollection<CanadaReturnCell> CanadaReturnCells { get { return GetCollection<CanadaReturnCell>(nameof(CanadaReturnCells)); } }
        [Association(@"CanadaReturnLineReferencesCanadaReturnLine")]
        public XPCollection<CanadaReturnLine> CanadaReturnLineCollection { get { return GetCollection<CanadaReturnLine>(nameof(CanadaReturnLineCollection)); } }
    }

}
