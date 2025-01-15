using System.Collections.Generic;

namespace FrameworkCore.Utils
{
    public interface IUpdateScriptProvider
    {
        /// <summary>
        /// Return a list of IUpdateScripts that should be run during the Updater.cs UpdateDatabaseBeforeUpdateSchema
        /// </summary>
        IList<IUpdateScript> GetPreUpdateScripts();
        /// <summary>
        /// Return a list of IUpdateScripts that should be run during the Updater.cs UpdateDatabaseAfterUpdateSchema
        /// </summary>
        IList<IUpdateScript> GetPostUpdateScripts();
    }
}
