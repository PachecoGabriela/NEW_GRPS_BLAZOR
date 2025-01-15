using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;

namespace FrameworkUtils.Utils
{
    public static class SecurityStrategyService
    {
        private static ISecurityStrategy _securityStrategy;
        public static ISecurityStrategy SecurityStrategy
        {
            get { return _securityStrategy ?? DefaultStrategy; }
            set { _securityStrategy = value; }
        }

        private static ISecurityStrategy _defaultStrategy;
        private static ISecurityStrategy DefaultStrategy
        {
            get
            {
                if (_defaultStrategy == null)
                    _defaultStrategy = new DefaultSecurityStrategy();
                return _defaultStrategy;
            }
        }

        [Obsolete("Use SecurityStrategyService.FindCurrentUser instead. " +
            "SecuritySystem.CurrentUserId does not work in non-XAF applications.", true)]
        public static object CurrentUserId
        {
            get
            {
                object result = SecuritySystem.CurrentUserId;

                if (result is string)
                {
                    string idStr = (string)result;
                    if (Guid.TryParse(idStr, out Guid resultGuid))
                        result = resultGuid;
                    else if (int.TryParse(idStr, out int resultInt))
                        result = resultInt;
                }

                return result;
            }
        }

        /// <summary>
        /// Finds current user based on the criteria operator function 'CurrentUserId()'.
        /// This is a replacement for SecuritySystem.CurrentUserId which does not work in non-XAF applications.
        /// </summary>
        public static object FindCurrentUser(Session session, Type userType)
        {
            Guard.ArgumentNotNull(session, nameof(session));
            Guard.ArgumentNotNull(userType, nameof(userType));

            return session.FindObject(userType, CriteriaOperator.Parse("Oid = CurrentUserId()"));
        }
    }
}
