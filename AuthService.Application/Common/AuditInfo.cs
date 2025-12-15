namespace AuthService.Application.Common
{
    public class AuditInfo
    {
        public string IpAddress { get; private set; } = null!;
        public string UserAgent { get; private set; } = null!;
        public string DeviceName { get; private set; } = null!;

        public AuditInfo(string ipAddress, string userAgent, string deviceName)
        {
            IpAddress = ipAddress;
            UserAgent = userAgent;
            DeviceName = deviceName;
        }
    }
}
