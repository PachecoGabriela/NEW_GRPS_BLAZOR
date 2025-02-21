using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace GRPS_BLAZOR.Module.BusinessObjects.GRIPS_DBCode.GRIPSdbCode
{
    public class FileDataEmail : BaseObject, IFileData, IEmptyCheckable
    { 
        public FileDataEmail(Session session)
            : base(session)
        {
        }

        [Association("EmailObject-Files")]
        public EmailObject EmailObject
        {
            get => emailObject;
            set => SetPropertyValue(nameof(EmailObject), ref emailObject, value);
        }

        EmailObject emailObject;
        private string fileName = "";

        [Persistent]
        private int size;

        public int Size => size;

        [Size(260)]
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                SetPropertyValue("FileName", ref fileName, value);
            }
        }

        [Persistent]
        [Delayed(true)]
        [ValueConverter(typeof(CompressionConverter))]
        [MemberDesignTimeVisibility(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] Content
        {
            get
            {
                return GetDelayedPropertyValue<byte[]>("Content");
            }
            set
            {
                int num = size;
                if (value != null)
                {
                    size = value.Length;
                }
                else
                {
                    size = 0;
                }

                SetDelayedPropertyValue("Content", value);
                OnChanged("Size", num, size);
            }
        }

        [MemberDesignTimeVisibility(false)]
        public bool IsEmpty => string.IsNullOrEmpty(FileName);

        public virtual void LoadFromStream(string fileName, Stream stream)
        {
            Guard.ArgumentNotNull(stream, "stream");
            FileName = fileName;
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            Content = array;
        }

        public virtual void SaveToStream(Stream stream)
        {
            if (Content != null)
            {
                stream.Write(Content, 0, Size);
            }

            stream.Flush();
        }

        public void Clear()
        {
            Content = null;
            FileName = string.Empty;
        }

        public override string ToString()
        {
            return FileName;
        }

        public Stream OpenReadStream()
        {
            if (Content == null || Content.Length == 0)
            {
                throw new InvalidOperationException("No hay contenido para leer.");
            }

            // Crea un MemoryStream a partir del contenido del archivo
            return new MemoryStream(Content);
        }
    }
}