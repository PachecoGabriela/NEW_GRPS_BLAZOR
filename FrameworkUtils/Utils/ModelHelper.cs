using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;

namespace FrameworkUtils.Utils
{
    public static class ModelHelper
    {
        #region GetMembers

        /// <summary>
        /// Gets all members of the specified class and its base classes.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="targetObjectType"></param>
        /// <param name="ownMembersOnly">If set, returns only the members of the specified class, excluding members from the base classes.</param>
        /// <returns></returns>
        public static List<IModelMember> GetMembers(XafApplication application, Type targetObjectType, bool ownMembersOnly = false)
        {
            Guard.ArgumentNotNull(application, nameof(application));
            Guard.ArgumentNotNull(targetObjectType, nameof(targetObjectType));

            IModelClass modelClass = application.Model.BOModel.GetClass(targetObjectType);
            if (modelClass == null)
                return new List<IModelMember>();
            return GetMembers(modelClass, ownMembersOnly);
        }

        /// <summary>
        /// Gets all members of the specified class and its base classes.
        /// </summary>
        /// <param name="modelClass"></param>
        /// <param name="ownMembersOnly">If set, returns only the members of the specified class, excluding members from the base classes.</param>
        public static List<IModelMember> GetMembers(IModelClass modelClass, bool ownMembersOnly = false)
        {
            Guard.ArgumentNotNull(modelClass, nameof(modelClass));

            List<IModelMember> result = new List<IModelMember>();

            if (ownMembersOnly)
            {
                result.AddRange(modelClass.OwnMembers);
            }
            else
            {
                IModelClass currentModelClass = modelClass;
                while (currentModelClass != null)
                {
                    result.AddRange(currentModelClass.OwnMembers);
                    currentModelClass = currentModelClass.BaseClass;
                }
            }

            return result;
        }

        #endregion
    }
}
