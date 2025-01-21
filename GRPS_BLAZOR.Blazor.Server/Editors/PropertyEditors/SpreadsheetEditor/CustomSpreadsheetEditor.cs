using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.SpreadsheetEditor
{
    [PropertyEditor(typeof(object), "SpreadSheetCustomEditor", false)]
    public class CustomSpreadsheetEditor : BlazorPropertyEditorBase
    {
        public CustomSpreadsheetEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        public override SpreadsheetModel ComponentModel => (SpreadsheetModel)base.ComponentModel;
        protected override IComponentModel CreateComponentModel()
        {
            var model = new SpreadsheetModel();
            model.ValueExpression = () => model.Value;
            model.ValueChanged = EventCallback.Factory.Create<object>(this, value => {
                model.Value = value;
                OnControlValueChanged();
                WriteValue();
            });
            model.ValueSaved = EventCallback.Factory.Create<object>(this, value => {
                model.Value = value;
                WriteValue();
            });
            return model;
        }
        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            ComponentModel.Value = (object)PropertyValue;
        }
        protected override object GetControlValueCore() => ComponentModel.Value;
        protected override void ApplyReadOnly()
        {
            base.ApplyReadOnly();
            ComponentModel?.SetAttribute("readonly", !AllowEdit);
        }
    }
}