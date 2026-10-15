using System;

namespace DragonBoy_Web_Host
{
    /// <summary>
    /// Cấu hình địa chỉ kết nối WebSocket-to-TCP Relay cho phiên bản trình duyệt.
    /// </summary>
    public static class WebNetworkConfig
    {
        /// <summary>
        /// Tạo URI kết nối WebSocket Relay tương ứng với địa chỉ máy chủ TCP đích.
        /// </summary>
        public static Uri BuildRelayUri(string targetHost, int targetPort)
        {
            string wsUrl = WebInterop.GetRelayWsUrl(targetHost ?? string.Empty, targetPort);
            return new Uri(wsUrl);
        }
    }
}
