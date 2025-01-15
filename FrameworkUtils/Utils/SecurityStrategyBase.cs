using System;
using DevExpress.ExpressApp;

namespace FrameworkUtils.Utils
{
    public interface ISecurityStrategy
    {
        Type UserType { get; }
    }

    public abstract class SecurityStrategyBase : ISecurityStrategy
    {
        protected virtual Type UserType
        {
            get
            {
                // default, legacy behavior
                // SecuritySystem.CurrentUser is sometimes null (e.g. in updater)
                if (SecuritySystem.CurrentUser != null)
                    return SecuritySystem.CurrentUser.GetType();

                // commented out. throws NotImplementedException.
                //return SecuritySystem.UserType; 

                return null;
            }
        }

        #region ISecurityStrategy members

        Type ISecurityStrategy.UserType => this.UserType;

        #endregion
    }

    public class DefaultSecurityStrategy : SecurityStrategyBase
    {
    }
}
