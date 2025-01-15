using DevExpress.ExpressApp.Utils;

namespace FrameworkUtils.Helpers
{
    public static class CaptionHelper
    {
        #region GetLocalizedText

        /// <summary>
        /// Indicates which overload of CaptionHelper.GetLocalizedText should be used by checking if any args are passed to the method and gets the text from Localization.
        /// </summary>
        /// <param name="groupPath">LocalizedText group path, e.g: Controllers\RoesleinConnex.Module.Controllers.Assets.ShipAssetViewController  where "Controllers\" is a main folder of the class and "RoesleinConnex.Module.Controllers.Assets.ShipAssetViewController" is a full type name (namespace + typename).</param>
        /// <param name="itemName">LocalizedText item key, e.g: NoRecordsSelected</param>
        /// <param name="defaultText">Default text value, e.g: 'No records selected'.</param>
        /// <param name="args"></param>
        public static string GetLocalizedText(string groupPath, string itemName, string defaultText, params object[] args)
        {
            string text;

            if (args == null || args.Length == 0)
            {
                text = DevExpress.ExpressApp.Utils.CaptionHelper.GetLocalizedText(groupPath, itemName);

                //If an item in the Model Localization does not exist, return defaultText.
                if (text == itemName || string.IsNullOrEmpty(text))
                    return defaultText;
            }
            else
            {
                text = DevExpress.ExpressApp.Utils.CaptionHelper.GetLocalizedText(groupPath, itemName, args);

                //If an item in the Model Localization does not exist, return defaultText with passed parameters.
                if (text == itemName || string.IsNullOrEmpty(text))
                {
                    return string.Format(defaultText, args);
                }
            }

            return text;
        }

        #endregion
    }
}
