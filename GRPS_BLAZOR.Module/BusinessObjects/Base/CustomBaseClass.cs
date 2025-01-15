using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Module.BusinessObjects.Base
{
    [NonPersistent]
    public abstract class CustomBaseClass : XPObject
    {
        public CustomBaseClass(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            //CreatedBy = GetCurrentUser();
            //CreatedOn = DateTime.Now;
        }

        //PermissionPolicyUser createdBy;
        //[NonCloneable]
        //[Browsable(false)]
        //public PermissionPolicyUser CreatedBy
        //{
        //    get => createdBy;
        //    set => SetPropertyValue(nameof(CreatedBy), ref createdBy, value);
        //}

        //PermissionPolicyUser lastChangedBy;
        //[NonCloneable]
        //[Browsable(false)]
        //public PermissionPolicyUser LastChangedBy
        //{
        //    get => lastChangedBy;
        //    set => SetPropertyValue(nameof(LastChangedBy), ref lastChangedBy, value);
        //}

        //PermissionPolicyUser deletedBy;
        //[NonCloneable]
        //[Browsable(false)]
        //public PermissionPolicyUser DeletedBy
        //{
        //    get => deletedBy;
        //    set => SetPropertyValue(nameof(DeletedBy), ref deletedBy, value);
        //}

        //PermissionPolicyUser GetCurrentUser()
        //{
        //    return Session.GetObjectByKey<PermissionPolicyUser>(SecuritySystem.CurrentUserId);
        //}

        //DateTime createdOn;
        //[NonCloneable]
        //[Browsable(false)]
        //public DateTime CreatedOn
        //{
        //    get => createdOn;
        //    set => SetPropertyValue(nameof(CreatedOn), ref createdOn, value);
        //}

        //DateTime lastChangedOn;
        //[NonCloneable]
        //[Browsable(false)]
        //public DateTime LastChangedOn
        //{
        //    get => lastChangedOn;
        //    set => SetPropertyValue(nameof(LastChangedOn), ref lastChangedOn, value);
        //}

        //DateTime deletionDate;
        //[NonCloneable]
        //[Browsable(false)]
        //public DateTime DeletionDate
        //{
        //    get => deletionDate;
        //    set => SetPropertyValue(nameof(DeletionDate), ref deletionDate, value);
        //}

        //protected override void OnSaving()
        //{
        //    LastChangedBy = GetCurrentUser();
        //    LastChangedOn = DateTime.Now;
        //    base.OnSaving();
        //}
        //protected override void OnDeleting()
        //{
        //    base.OnDeleting();
        //    DeletedBy = GetCurrentUser();
        //    DeletionDate = DateTime.Now;
        //}
    }
}
