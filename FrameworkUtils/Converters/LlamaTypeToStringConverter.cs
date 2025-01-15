using System;
using DevExpress.ExpressApp.Utils;

namespace FrameworkUtils.Converters
{
    /// <summary>
    /// The same as the built-in TypeToStringConverter, but also displays primitive types.
    /// </summary>
    public class TypeToStringConverter : DevExpress.ExpressApp.Utils.TypeToStringConverter
    {
        public override object ConvertFromStorageType(object stringObjectType)
        {
            if (stringObjectType == null)
                return null;
            object result = Type.GetType(stringObjectType as string);
            if (result == null)
                result = base.ConvertFromStorageType(stringObjectType);
            return result;
        }
    }
}
