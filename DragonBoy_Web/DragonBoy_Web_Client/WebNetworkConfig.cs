using System;

namespace DragonBoy_Web_Host
{
    public static class WebNetworkConfig
    {
        public static Uri BuildRelayUri(string targetHost, int targetPort)
        {
            string wsUrl = WebInterop.GetRelayWsUrl(targetHost ?? string.Empty, targetPort);
            return new Uri(wsUrl);
        }
    }
}
