using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace FrameworkUtils.Utils
{
    public static class XpoHelper
    {

        #region Session

        /// <summary>
        /// Extension method that provides access to Session.
        /// We assume that XPO is used as an ORM  (not EF, etc).
        /// </summary>
        public static Session Session(this IObjectSpace objectSpace)
        {
            Session result = null;

            if (objectSpace != null)
            {
                if (objectSpace is XPObjectSpace)
                    result = ((XPObjectSpace)objectSpace).Session;
                else if (objectSpace is NonPersistentObjectSpace)
                {
                    // We should not require Session from within Non Persistent Object Space. Every time it happens we need to check in the call stack why it happened 
                    // and if some scenario is done correctly.
                    //System.Diagnostics.Debugger.Break(); //commented out in order to return nested session
                    //return null;

                    //Getting session from nonpersistent additional objectspace
                    var nonPersistentObjectSpace = objectSpace as NonPersistentObjectSpace;
                    var xpoOS = nonPersistentObjectSpace.AdditionalObjectSpaces.FirstOrDefault() as XPObjectSpace;
                    result = xpoOS?.Session;
                }
            }

            return result;
        }

        #endregion

        #region GetNestedMemberValue

        /// <summary>
        /// Returns the value of 'placeHolder', even if it is a nested property path (e.g. "Customer.Address.Name"). 
        /// Returns null for [Browsable(false)] or [MemberDesignTimeVisibility(false)]. 
        /// Works with XPBaseObject descendants.
        /// </summary>
        /// <param name="obj">this. (It is an extension method)</param>
        /// <param name="propertyNamePath">e.g. "Customer.Address.Name"</param>
        /// <returns></returns>
        public static object GetNestedMemberValue(this XPBaseObject obj, string propertyNamePath, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(obj, nameof(obj));
                Guard.ArgumentNotNullOrEmpty(propertyNamePath, nameof(propertyNamePath));
            }
            else
            {
                if (obj == null || string.IsNullOrEmpty(propertyNamePath))
                    return null;
            }

            try
            {
                string baseProperty = propertyNamePath;
                int colonIndex = propertyNamePath.IndexOf(':');
                string propertyNamePathWithoutFormat = colonIndex == -1 ? propertyNamePath : propertyNamePath.Substring(0, colonIndex);

                if (propertyNamePathWithoutFormat.Contains("."))
                {
                    int index = propertyNamePath.IndexOf('.');
                    baseProperty = propertyNamePath.Substring(0, index);
                    object basePropertyValue = GetMemberValue(obj, baseProperty, throwErrors);
                    if (basePropertyValue == null || !(basePropertyValue is XPBaseObject))
                        return null;
                    string subProperty = propertyNamePath.Substring(index + 1, propertyNamePath.Length - index - 1);
                    return (basePropertyValue as XPBaseObject).GetNestedMemberValue(subProperty, throwErrors);
                }
                else
                    return GetMemberValue(obj as XPBaseObject, baseProperty, throwErrors);
            }
            catch (DevExpress.Xpo.Exceptions.PropertyMissingException ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning("PropertyMissingException: " + ex.Message);
            }
            catch (DevExpress.Data.Filtering.Exceptions.InvalidPropertyPathException ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning("InvalidPropertyPathException: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning(ex.Message);
            }

            return null;
        }

        #endregion

        #region SetNestedMemberValue

        /// <summary>
        /// Sets a value to the nested property path.
        /// E.g. for "Customer.Address.Name" assigns a value to the Name property of the Address object.
        /// Also tries to handle null value for primitive types. And cast boxed values to the correct type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyNamePath"></param>
        /// <param name="value"></param>
        /// <param name="throwErrors"></param>
        public static void SetNestedMemberValue(this XPBaseObject obj, string propertyNamePath, object value, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(obj, nameof(obj));
                Guard.ArgumentNotNullOrEmpty(propertyNamePath, nameof(propertyNamePath));
            }
            else
            {
                if (obj == null || string.IsNullOrEmpty(propertyNamePath))
                    return;
            }

            if (propertyNamePath.Contains("."))
            {
                int lastDotIndex = propertyNamePath.LastIndexOf(".");
                string referenceObjectPath = propertyNamePath.Substring(0, lastDotIndex);
                string endPropertyName = propertyNamePath.Substring(lastDotIndex + 1);

                object referenceObject = obj.GetNestedMemberValue(referenceObjectPath, throwErrors);
                XPBaseObject referenceXpBaseObject = referenceObject as XPBaseObject;
                if (referenceXpBaseObject != null)
                {
                    referenceXpBaseObject.SetMemberValue(endPropertyName, value, throwErrors);
                }
            }
            else
            {
                SetMemberValue(obj, propertyNamePath, value, throwErrors);
            }
        }

        #endregion

        #region GetMemberValue

        /// <summary>
        /// Returns the property value, if it was not marked as [Browsable(false)] or [MemberDesignTimeVisibility(false)].
        /// If the tag contained ':', formats the value. Otherwise simply returns the value.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static object GetMemberValue(XPBaseObject obj, string propertyName, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(obj, nameof(obj));
                Guard.ArgumentNotNullOrEmpty(propertyName, nameof(propertyName));
            }
            else
            {
                if (obj == null || string.IsNullOrEmpty(propertyName))
                    return null;
            }

            string propertyFormat = null;
            if (propertyName.Contains(":"))
            {
                string[] propertyNamePortions = propertyName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (propertyNamePortions.Length == 2)
                {
                    propertyName = propertyNamePortions[0];
                    propertyFormat = propertyNamePortions[1];
                }
                else
                {
                    if (throwErrors)
                        throw new Exception($"Incorrect property name format: {propertyName}.");
                    else
                        return null;
                }
            }

            XPMemberInfo memberInfo = GetMember(obj.ClassInfo, propertyName, throwErrors);
            if (memberInfo == null)
                return null;

            object memberValue = GetFormattedValue(memberInfo, obj, propertyFormat, throwErrors);
            return memberValue;
        }

        #endregion

        #region SetMemberValue

        /// <summary>
        /// Sets a value to the specified propertyName.
        /// Also tries to handle null value for primitive types. And cast boxed values to the correct type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="throwErrors"></param>
        public static void SetMemberValue(this XPBaseObject obj, string propertyName, object value, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(obj, nameof(obj));
                Guard.ArgumentNotNullOrEmpty(propertyName, nameof(propertyName));
            }
            else
            {
                if (obj == null || string.IsNullOrEmpty(propertyName))
                    return;
            }

            try
            {
                XPMemberInfo memberInfo = XpoHelper.GetMember(obj.ClassInfo, propertyName, throwErrors);
                if (memberInfo != null)
                {
                    // handle null values
                    if (value == null && memberInfo.MemberType.IsValueType)
                        value = Activator.CreateInstance(memberInfo.MemberType);
                    // handle type mismatch
                    value = Convert.ChangeType(value, memberInfo.MemberType);

                    memberInfo.SetValue(obj, value);
                }
            }
            catch (Exception ex)
            {
                if (throwErrors)
                    throw new Exception($"An error occurred when assigning a value '{value}' to property name '{propertyName}' on object '{obj}'", ex);
                else
                    Tracing.Tracer.LogWarning(ex.Message);
            }
        }

        #endregion

        #region GetFormattedValue

        /// <summary>
        /// Gets the value of the memberInfo from obj record. If propertyFormat is specified, formats the value and casts it to string.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="obj"></param>
        /// <param name="propertyFormat"></param>
        /// <param name="throwErrors"></param>
        /// <returns></returns>
        private static object GetFormattedValue(XPMemberInfo memberInfo, XPBaseObject obj, string propertyFormat, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(memberInfo, nameof(memberInfo));
                Guard.ArgumentNotNull(obj, nameof(obj));
            }
            else
            {
                if (memberInfo == null || obj == null)
                    return null;
            }

            // get the value
            object memberValue = memberInfo.GetValue(obj); //(obj as XPBaseObject).GetMemberValue(propertyName);

            if (!string.IsNullOrEmpty(propertyFormat))
            {
                try
                {
                    // format the value (and make it string be the way)
                    memberValue = string.Format("{0:" + propertyFormat + "}", memberValue);
                }
                catch (Exception ex)
                {
                    if (throwErrors)
                        throw new Exception($"Property format for property '{memberInfo.DisplayName}' was incorrect: '{propertyFormat}'", ex);
                    else
                        Tracing.Tracer.LogWarning(ex.Message);
                }
            }

            return memberValue;
        }

        #endregion

        #region GetMember

        /// <summary>
        /// Gets the IMemberInfo of property by given ITypeInfo.
        /// </summary>
        public static IMemberInfo GetMember(ITypeInfo typesInfo, string propertyName, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(typesInfo, nameof(typesInfo));
                Guard.ArgumentNotNullOrEmpty(propertyName, nameof(propertyName));
            }
            else
            {
                if (typesInfo == null || string.IsNullOrEmpty(propertyName))
                    return null;
            }

            // ignore format data defined after ':'
            if (propertyName.Contains(":"))
            {
                string[] propertyNamePortions = propertyName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (propertyNamePortions.Length == 2)
                {
                    propertyName = propertyNamePortions[0];
                }
                else
                {
                    if (throwErrors)
                        throw new Exception($"Incorrect property name format: {propertyName}.");
                    else
                        return null;
                }
            }

            IMemberInfo result = null;
            try
            {
                result = typesInfo.FindMember(propertyName);
            }
            catch (Exception)
            {
                if (throwErrors)
                    throw;
            }
            if (result == null)
                return null;

            // return null if [Browsable(false)] or [MemberDesignTimeVisibility(false)]
            bool isBrowsable = result.Attributes.OfType<BrowsableAttribute>().FirstOrDefault()?.Browsable ?? true;
            bool isMemberDesignTimeVisibility = result.Attributes.OfType<MemberDesignTimeVisibilityAttribute>().FirstOrDefault()?.IsVisible ?? true;
            if (!isBrowsable || !isMemberDesignTimeVisibility)
                return null;

            return result;
        }

        /// <summary>
        /// Gets the XPMemberInfo of property by given XPClassInfo.
        /// </summary>
        public static XPMemberInfo GetMember(XPClassInfo xpClassInfo, string propertyName, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(xpClassInfo, nameof(xpClassInfo));
                Guard.ArgumentNotNullOrEmpty(propertyName, nameof(propertyName));
            }
            else
            {
                if (xpClassInfo == null || string.IsNullOrEmpty(propertyName))
                    return null;
            }

            // ignore format data defined after ':'
            if (propertyName.Contains(":"))
            {
                string[] propertyNamePortions = propertyName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (propertyNamePortions.Length == 2)
                {
                    propertyName = propertyNamePortions[0];
                }
                else
                {
                    if (throwErrors)
                        throw new Exception($"Incorrect property name format: {propertyName}.");
                    else
                        return null;
                }
            }

            XPMemberInfo result = null;
            try
            {
                result = xpClassInfo.GetMember(propertyName);
            }
            catch (Exception)
            {
                if (throwErrors)
                    throw;
            }
            if (result == null)
                return null;

            // return null if [Browsable(false)] or [MemberDesignTimeVisibility(false)]
            bool isBrowsable = result.Attributes.OfType<BrowsableAttribute>().FirstOrDefault()?.Browsable ?? true;
            bool isMemberDesignTimeVisibility = result.Attributes.OfType<MemberDesignTimeVisibilityAttribute>().FirstOrDefault()?.IsVisible ?? true;
            if (!isBrowsable || !isMemberDesignTimeVisibility)
                return null;

            return result;
        }

        #endregion

        #region GetNestedMember

        /// <summary>
        /// Gets the IMemberInfo of nested property by given ITypeInfo and propertyNamePath. i.e. for propertyNamePath = 'Vendor.Name' the IMemberInfo of 'Name' is returned.
        /// </summary>
        public static IMemberInfo GetNestedMember(ITypeInfo typeInfo, string propertyNamePath, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(typeInfo, nameof(typeInfo));
                Guard.ArgumentNotNullOrEmpty(propertyNamePath, nameof(propertyNamePath));
            }
            else
            {
                if (typeInfo == null | string.IsNullOrEmpty(propertyNamePath))
                    return null;
            }

            try
            {
                string baseProperty = propertyNamePath;

                if (propertyNamePath.Contains("."))
                {
                    int index = propertyNamePath.IndexOf('.');
                    baseProperty = propertyNamePath.Substring(0, index);

                    IMemberInfo baseMemberInfo = GetMember(typeInfo, baseProperty, throwErrors);
                    if (baseMemberInfo == null)
                        return null;
                    string subProperty = propertyNamePath.Substring(index + 1, propertyNamePath.Length - index - 1);

                    return GetNestedMember(baseMemberInfo.MemberTypeInfo, subProperty, throwErrors);
                }
                else
                    return GetMember(typeInfo, baseProperty, throwErrors);
            }
            catch (DevExpress.Xpo.Exceptions.PropertyMissingException ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning("PropertyMissingException: " + ex.Message);
            }
            catch (DevExpress.Data.Filtering.Exceptions.InvalidPropertyPathException ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning("InvalidPropertyPathException: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning(ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets the XPMemberInfo of nested property by given XPClassInfo and propertyNamePath. i.e. for propertyNamePath = 'Vendor.Name' the XPMemberInfo of 'Name' is returned.
        /// </summary>
        public static XPMemberInfo GetNestedMember(XPClassInfo xpClassInfo, string propertyNamePath, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(xpClassInfo, nameof(xpClassInfo));
                Guard.ArgumentNotNullOrEmpty(propertyNamePath, nameof(propertyNamePath));
            }
            else
            {
                if (xpClassInfo == null | string.IsNullOrEmpty(propertyNamePath))
                    return null;
            }

            try
            {
                string baseProperty = propertyNamePath;

                if (propertyNamePath.Contains("."))
                {
                    int index = propertyNamePath.IndexOf('.');
                    baseProperty = propertyNamePath.Substring(0, index);

                    XPMemberInfo baseMemberInfo = GetMember(xpClassInfo, baseProperty, throwErrors);
                    if (baseMemberInfo == null)
                        return null;
                    string subProperty = propertyNamePath.Substring(index + 1, propertyNamePath.Length - index - 1);

                    return GetNestedMember(baseMemberInfo.ReferenceType, subProperty, throwErrors);
                }
                else
                    return GetMember(xpClassInfo, baseProperty, throwErrors);
            }
            catch (DevExpress.Xpo.Exceptions.PropertyMissingException ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning("PropertyMissingException: " + ex.Message);
            }
            catch (DevExpress.Data.Filtering.Exceptions.InvalidPropertyPathException ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning("InvalidPropertyPathException: " + ex.Message);
            }
            catch (System.Exception ex)
            {
                if (throwErrors)
                    throw;
                else
                    Tracing.Tracer.LogWarning(ex.Message);
            }

            return null;
        }

        #endregion

        #region GetClassInfo

        /// <summary>
        /// Simple proxy of session.GetClassInfo() method. But with throwErrors flag.
        /// </summary>
        public static XPClassInfo GetClassInfo(Session session, Type typeToCheck, bool throwErrors = true)
        {
            if (throwErrors)
            {
                Guard.ArgumentNotNull(session, nameof(session));
                Guard.ArgumentNotNull(typeToCheck, nameof(typeToCheck));
            }
            else
            {
                if (session == null || typeToCheck == null)
                    return null;
            }

            XPClassInfo result = null;
            try
            {
                result = session.GetClassInfo(typeToCheck);
            }
            catch (Exception)
            {
                if (throwErrors)
                    throw;
            }

            return result;
        }

        #endregion
    }
}
