using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode
{
    [DefaultClassOptions]
    
    public class SpreadsheetContainer : BaseObject
    { 
        public SpreadsheetContainer(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            
        }


        string fileName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string FileName
        {
            get => fileName;
            set => SetPropertyValue(nameof(FileName), ref fileName, value);
        }

        byte[] spreadStream;
        [VisibleInListView(false)]
        [EditorAlias("SpreadSheetCustomEditor")]
        public byte[] SpreadsheetFile
        {
            get => spreadStream;
            set => SetPropertyValue(nameof(SpreadsheetFile), ref spreadStream, value);
        }
    }
}