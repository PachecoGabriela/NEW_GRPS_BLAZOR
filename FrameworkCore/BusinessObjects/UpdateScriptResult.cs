using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FrameworkCore.BusinessObjects
{
    public sealed class UpdateScriptResult : BaseObject
    {
        public UpdateScriptResult(Session session)
            : base(session)
        {
        }

        private Guid _UpdateID;
        public Guid UpdateID
        {
            get { return _UpdateID; }
            set { SetPropertyValue<Guid>(nameof(UpdateID), ref _UpdateID, value); }
        }


        private string _UpdateDescription;
        [Size(1024)]
        public string UpdateDescription
        {
            get { return _UpdateDescription; }
            set { SetPropertyValue<string>(nameof(UpdateDescription), ref _UpdateDescription, value); }
        }


        private DateTime _CreatedDate;
        public DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { SetPropertyValue<DateTime>(nameof(CreatedDate), ref _CreatedDate, value); }
        }


        private DateTime _RunOn;
        public DateTime RunOn
        {
            get { return _RunOn; }
            set { SetPropertyValue<DateTime>(nameof(RunOn), ref _RunOn, value); }
        }


        private string _Result;
        [Size(SizeAttribute.Unlimited)]
        public string Result
        {
            get { return _Result; }
            set { SetPropertyValue<string>(nameof(Result), ref _Result, value); }
        }
    }
}