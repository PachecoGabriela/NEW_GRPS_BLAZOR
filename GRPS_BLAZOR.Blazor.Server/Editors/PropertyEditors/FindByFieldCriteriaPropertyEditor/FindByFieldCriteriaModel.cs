using DevExpress.ExpressApp.Blazor.Components.Models;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.FindByFieldCriteriaPropertyEditor
{
    public class FindByFieldCriteriaModel : ComponentModelBase
    {
        public string Value
        {
            get => GetPropertyValue<string>();
            set => SetPropertyValue(value);
        }
        public bool ReadOnly
        {
            get => GetPropertyValue<bool>();
            set => SetPropertyValue(value);
        }

        public void SetValueFromUI(string value)
        {
            SetPropertyValue(value, notify: false, nameof(Value));
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ValueChanged;
    }
}
