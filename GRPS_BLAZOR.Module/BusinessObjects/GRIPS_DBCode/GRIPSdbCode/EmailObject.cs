using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPS_schema;
using Syncfusion.EJ2.Inputs;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode
{
    public class EmailObject : BaseObject
    { 
        public EmailObject(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();            
        }


        SpreadsheetContainer originContainer;
        FileDataEmail uploadFile;
        string from;
        string emailAddressSender;
        string emailSentFrom;
        string sentFrom;
        string companyCode;
        bool received;
        string toEmail;
        bool sent;
        string message;
        string to;
        string subject;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Subject
        {
            get => subject;
            set => SetPropertyValue(nameof(Subject), ref subject, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Browsable(false)]
        public string From
        {
            get => from;
            set => SetPropertyValue(nameof(From), ref from, value);
        }

        [Appearance("Disable To - EmailObject", Enabled = false)]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string To
        {
            get => to;
            set => SetPropertyValue(nameof(To), ref to, value);
        }

        [Appearance("Disable ToEmail - EmailObject", Enabled = false)]
        [Size(SizeAttribute.Unlimited)]
        [XafDisplayName("To")]
        public string ToEmail
        {
            get => toEmail;
            set => SetPropertyValue(nameof(ToEmail), ref toEmail, value);
        }

        [VisibleInListView(false)]
        [EditorAlias("FileData")]
        [DevExpress.Xpo.DisplayName("Upload Data")]
        [ImmediatePostData]
        public FileDataEmail UploadFile
        {
            get => uploadFile;
            set => SetPropertyValue(nameof(UploadFile), ref uploadFile, value);
        }


        [Size(SizeAttribute.Unlimited)]
        public string Message
        {
            get => message;
            set => SetPropertyValue(nameof(Message), ref message, value);
        }

        [VisibleInDetailView(false), VisibleInListView(false)]
        public bool Sent
        {
            get => sent;
            set => SetPropertyValue(nameof(Sent), ref sent, value);
        }

        [VisibleInDetailView(false), VisibleInListView(false)]
        public bool Received
        {
            get => received;
            set => SetPropertyValue(nameof(Received), ref received, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Browsable(false)]
        public string CompanyCode
        {
            get => companyCode;
            set => SetPropertyValue(nameof(CompanyCode), ref companyCode, value);
        }

        [Browsable(false)]
        public string SentFrom
        {
            get => sentFrom;
            set => SetPropertyValue(nameof(SentFrom), ref sentFrom, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Browsable(false)]
        public string EmailSentFrom
        {
            get => emailSentFrom;
            set => SetPropertyValue(nameof(EmailSentFrom), ref emailSentFrom, value);
        }

        [NoForeignKey]
        public SpreadsheetContainer OriginContainer
        {
            get => originContainer;
            set => SetPropertyValue(nameof(OriginContainer), ref originContainer, value);
        }

        [Association("EmailObject-Files"), DevExpress.Xpo.Aggregated]
        public XPCollection<FileDataEmail> Files
        {
            get
            {
                return GetCollection<FileDataEmail>(nameof(Files));
            }
        }

        [NonPersistent]
        [Size(SizeAttribute.Unlimited)]
        public string FileLinks
        {
            get
            {
                return string.Join("<br/>", Files.Select(file => $"<a href='/api/file/download/{file.Oid}' download>{file.FileName}</a>"));
            }
        }



        public List<Supplier> suppliersSelected { get; set; } = new List<Supplier>();

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "UploadFile" && newValue is FileDataEmail uploadedFile)
            {
                if (this.UploadFile is not null)
                {
                    // Crea una nueva instancia de FileDataEmail
                    var fileDataEmail = new FileDataEmail(Session);

                    // Abre el flujo del archivo subido
                    using (var stream = uploadedFile.OpenReadStream())
                    {
                        // Carga los datos del archivo en la instancia de FileDataEmail
                        fileDataEmail.LoadFromStream(uploadedFile.FileName, stream);
                    }

                    // Agrega el archivo a la colección Files
                    Files.Add(fileDataEmail);

                }
            }
        }
    }
}