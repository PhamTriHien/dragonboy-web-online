using System;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Cấu hình địa chỉ kết nối WebSocket-to-TCP Relay cho phiên bản trình duyệt (hỗ trợ cả kết nối trực tiếp và qua SOCKS5 / HTTP CONNECT Proxy).
    /// </summary>
    public static class WebNetworkConfig
    {
        /// <summary>
        /// Tạo URI kết nối WebSocket Relay tương ứng với địa chỉ máy chủ TCP đích và cấu hình Proxy hiện tại.
        /// </summary>
        public static Uri BuildRelayUri(string targetHost, int targetPort)
        {
            string wsUrl = WebInterop.GetRelayWsUrl(targetHost ?? string.Empty, targetPort);
            if (global::ModProxyServer.HasProxy())
            {
                string sep = wsUrl.Contains('?') ? "&" : "?";
                wsUrl = wsUrl + sep + "proxy=" + Uri.EscapeDataString(global::ModProxyServer.proxyRaw.Trim());
            }
            return new Uri(wsUrl);
        }
    }
}
