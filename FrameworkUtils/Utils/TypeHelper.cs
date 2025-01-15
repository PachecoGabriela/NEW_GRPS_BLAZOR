using System;

namespace FrameworkUtils.Utils
{
    public static class TypeHelper
    {
        #region PrimitiveTypes

        private static Type[] _primitiveTypes;

        /// <summary>
        /// Array of primitive types like string, int, DateTime, nullable int.
        /// NOTE: enums are not included.
        /// </summary>
        public static Type[] PrimitiveTypes
        {
            get
            {
                if (_primitiveTypes == null)
                    _primitiveTypes = new[] {
                        typeof(string),
                        typeof(DateTime),
                        typeof(decimal),
                        typeof(int),
                        typeof(bool),
                        typeof(long),
                        typeof(short),
                        typeof(byte),
                        typeof(float),
                        typeof(double),
                        typeof(char),

                        typeof(decimal?),
                        typeof(int?),
                        typeof(bool?),
                        typeof(long?),
                        typeof(short?),
                        typeof(byte?),
                        typeof(float?),
                        typeof(double?),
                        typeof(char?),
                    };
                return _primitiveTypes;
            }
        }

        #endregion
    }
}
