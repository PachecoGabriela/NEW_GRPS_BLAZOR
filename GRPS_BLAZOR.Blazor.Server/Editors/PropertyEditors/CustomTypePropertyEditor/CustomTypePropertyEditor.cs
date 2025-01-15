using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using ExcelImport.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.CustomTypePropertyEditor
{
    [PropertyEditor(typeof(Type), "CustomTypePropertyEditor", false)]
    public class CustomTypePropertyEditor : TypePropertyEditor
    {
        public CustomTypePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
        }

        protected override bool IsSuitableType(Type type)
        {
            if (type != null && type.GetInterface(nameof(IImportFromExcel)) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
