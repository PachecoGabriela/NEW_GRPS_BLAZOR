using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.DC;
using Microsoft.AspNetCore.Components;
using XafCustomComponents;
using XafCustomComponents.Utilities.Reflection;

namespace GRPS_BLAZOR.Blazor.Server.Editors.PropertyEditors.IdentifyRecordByPropertyEditor
{
    public partial class IdentifyRecordByRenderer : ComponentBase
    {
        [Parameter] public IdentifyRecordByModel ComponentModel { get; set; }

        IList<DataItem<string>> GetData()
        {
            IList<DataItem<string>> dataItems = new List<DataItem<string>>();
            if (ComponentModel.ObjectType is not null)
            {
                IEnumerable<IMemberInfo> visibleMembers = TypeInfoUtil.GetVisibleMembersInfo(ComponentModel.ObjectType);
                IEnumerable<IMemberInfo> nonListisibleMembers = TypeInfoUtil.GetNonListMembersInfo(visibleMembers);
                IEnumerable<IMemberInfo> nonPersistentObjectMembers = TypeInfoUtil.GetNonPersistentAssociatedObjectsMemberInfo(nonListisibleMembers);

                foreach (IMemberInfo member in nonPersistentObjectMembers)
                {
                    string text = member.DisplayName;
                    if (string.IsNullOrEmpty(text))
                        text = member.Name;
                    dataItems.Add(new DataItem<string>($"[{member.Name}] = ?", text));
                }
            }

            return dataItems;
        }
    }
}
