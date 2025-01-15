using System;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;

namespace FrameworkUtils.Utils
{
    public static class TrackedObjectHelperConnex
    {
        /*
        public static void AfterConstruction(ITrackedObject obj, string createdbyproperty)
        {
            Guard.ArgumentNotNull(obj, nameof(obj));
            Guard.ArgumentNotNullOrEmpty(createdbyproperty, nameof(createdbyproperty));

            obj.CreatedOn = DateTime.Now;
            SetUserProperty(obj, createdbyproperty);
        }

        public static void OnSaving(ITrackedObject obj, string modifiedbyproperty)
        {
            Guard.ArgumentNotNull(obj, nameof(obj));
            Guard.ArgumentNotNullOrEmpty(modifiedbyproperty, nameof(modifiedbyproperty));

            obj.ModifiedOn = DateTime.Now;
            SetUserProperty(obj, modifiedbyproperty);
        }

        public static void OnDeleting(ITrackedObject obj, string deletedbyproperty)
        {
            Guard.ArgumentNotNull(obj, nameof(obj));
            Guard.ArgumentNotNullOrEmpty(deletedbyproperty, nameof(deletedbyproperty));

            obj.DeletedOn = DateTime.Now;
            SetUserProperty(obj, deletedbyproperty);
        }
        
        private static void SetUserProperty(ITrackedObject obj, string property)
        {
            if (string.IsNullOrEmpty(property))
                return;

            IMemberInfo member = XafTypesInfo.Instance.FindTypeInfo(obj.GetType()).FindMember(property);
            if (member == null)
                throw new InvalidPropertyPathException($"The {property} property does not exist within the {obj.GetType().Name} type.");

            Type userType = SecurityStrategyService.SecurityStrategy.UserType;
            object userId = SecurityStrategyService.CurrentUserId;

            if (userType != null && userId != null)
                member.SetValue(obj, obj.Session.GetObjectByKey(userType, userId));
        }
        */
    }
}
