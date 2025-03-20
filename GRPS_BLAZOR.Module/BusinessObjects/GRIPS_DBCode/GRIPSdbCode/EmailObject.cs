﻿using System;
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


        string suppliersSelectedOids;
        string attachments;
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
        [ImmediatePostData]
        public string FilesUploaded
        {
            get
            {
                return string.Join("<br/>", Files.Select(file =>
            $"<span id='file-{file.Oid}'>" +
            $"<a href='#' onclick='downloadFile(event, \"/api/downloadfile/download/{file.Oid}\", \"{file.FileName}\")'>{file.FileName}</a> " +
            $"<button onclick='deleteFile(\"{file.Oid}\")' style='background:none;border:none;color:#15a362;cursor:pointer;'>❌</button>" +
            "</span>")) +
            "<script>" +
            "async function deleteFile(fileOid) { " +
            "    try { " +
            "        let response = await fetch(`/api/downloadfile/delete/${fileOid}`, { method: 'DELETE' });" +
            "        if (response.ok) { " +
            "            document.getElementById('file-' + fileOid).remove(); " +
            "        } else { " +
            "            alert('Error when deleting document.'); " +
            "        }" +
            "    } catch (error) { alert('Error de conexión.'); }" +
            "}" +
            "function downloadFile(event, url, filename) { " +
            "    event.preventDefault(); " +
            "    var a = document.createElement('a');" +
            "    a.href = url;" +
            "    a.setAttribute('download', filename);" +
            "    document.body.appendChild(a);" +
            "    a.click();" +
            "    document.body.removeChild(a);" +
            "}" +
            "</script>";
            }
        }

        [NonPersistent]
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Attachments
        {
            get
            {
                return string.Join("<br/>", Files.Select(file =>
            $"<a href='#' onclick='downloadFile(\"/api/downloadfile/download/{file.Oid}\", \"{file.FileName}\")'>{file.FileName}</a>")) +
            "<script>" +
            "function downloadFile(url, filename) { " +
            "    event.preventDefault(); " +
            "    var a = document.createElement('a');" +
            "    a.href = url;" +
            "    a.setAttribute('download', filename);" +
            "    document.body.appendChild(a);" +
            "    a.click();" +
            "    document.body.removeChild(a);" +
            "}" +
            "</script>";
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string SuppliersSelectedOids
        {
            get => suppliersSelectedOids;
            set => SetPropertyValue(nameof(SuppliersSelectedOids), ref suppliersSelectedOids, value);
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "UploadFile" && newValue is FileDataEmail uploadedFile)
            {
                if (this.UploadFile is not null)
                {
                    var fileDataEmail = new FileDataEmail(Session);

                    using (var stream = uploadedFile.OpenReadStream())
                    {
                        fileDataEmail.LoadFromStream(uploadedFile.FileName, stream);
                    }

                    Files.Add(fileDataEmail);

                }
            }
        }
    }
}