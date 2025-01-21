using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.SpreadsheetEditor
{
    public class SpreadsheetModel : ComponentModelBase
    {
        public object Value
        {
            get => GetPropertyValue<object>();
            set => SetPropertyValue(value);
        }
        public EventCallback<object> ValueChanged
        {
            get => GetPropertyValue<EventCallback<object>>();
            set => SetPropertyValue(value);
        }
        public EventCallback<object> ValueSaved
        {
            get => GetPropertyValue<EventCallback<object>>();
            set => SetPropertyValue(value);
        }
        public Expression<Func<object>> ValueExpression
        {
            get => GetPropertyValue<Expression<Func<object>>>();
            set => SetPropertyValue(value);
        }
        public override Type ComponentType => typeof(SpreadsheetComponent);
    }
}