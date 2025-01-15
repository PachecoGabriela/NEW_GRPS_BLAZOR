using System;

namespace FrameworkUtils.Helpers
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Sets TLS 1.2 or so to ensure latest security protocol. Used for File Attachment Server.
        /// 
        /// NOTE: Check supported security protocols by this powershell command: [Net.ServicePointManager]::SecurityProtocol
        /// 
        /// https://stackoverflow.com/questions/26742054/the-client-and-server-cannot-communicate-because-they-do-not-possess-a-common-a
        /// </summary>
        public static void SetSecurityProtocol(string securityProtocolConfig)
        {
            System.Net.SecurityProtocolType securityProtocol = System.Net.SecurityProtocolType.Tls12;

            if (!string.IsNullOrEmpty(securityProtocolConfig))
                if (Enum.TryParse(securityProtocolConfig, true, out System.Net.SecurityProtocolType securityProtocolParsed))
                    securityProtocol = securityProtocolParsed;

            System.Net.ServicePointManager.SecurityProtocol = securityProtocol;
        }
    }
}
