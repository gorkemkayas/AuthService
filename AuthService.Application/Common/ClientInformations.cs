namespace AuthService.Application.Common
{
    public class ClientInformations
    {
        public string IpAddress { get; private set; } = null!;
        public string UserAgent { get; private set; } = null!;
        public string DeviceName { get; private set; } = null!;

        public ClientInformations(string ipAddress, string userAgent, string deviceName)
        {
            IpAddress = ipAddress;
            UserAgent = userAgent;
            DeviceName = deviceName;
        }
    }
}
