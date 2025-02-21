using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Microsoft.Identity.Client;
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
            IObjectSpace objectSpace = XPObjectSpace.FindObjectSpaceByObject(this);
            if (objectSpace != null)
            {
                this.CreatedBy = objectSpace.GetObject(SecuritySystem.CurrentUser) as ApplicationUser;
            }
        }


        ApplicationUser createdBy;
        string companyName;
        string companyCode;
        string fileName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string FileName
        {
            get => fileName;
            set => SetPropertyValue(nameof(FileName), ref fileName, value);
        }

        [Appearance("Disable CompanyCode - SpreadSheetContainer", Enabled = false)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CompanyCode
        {
            get => companyCode;
            set => SetPropertyValue(nameof(CompanyCode), ref companyCode, value);
        }

        [Appearance("Disable CompanyName - SpreadSheetContainer", Enabled = false)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CompanyName
        {
            get => companyName;
            set => SetPropertyValue(nameof(CompanyName), ref companyName, value);
        }

        byte[] spreadStream;
        [VisibleInListView(false)]
        [EditorAlias("SpreadSheetCustomEditor")]
        public byte[] SpreadsheetFile
        {
            get => spreadStream;
            set => SetPropertyValue(nameof(SpreadsheetFile), ref spreadStream, value);
        }

        [Browsable(false)]
        public ApplicationUser CreatedBy
        {
            get => createdBy;
            set => SetPropertyValue(nameof(CreatedBy), ref createdBy, value);
        }
    }
}