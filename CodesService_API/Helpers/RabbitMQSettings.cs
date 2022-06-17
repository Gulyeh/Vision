namespace CodesService_API.Helpers
{
    public class RabbitMQSettings
    {
        public RabbitMQSettings()
        {
            Hostname = string.Empty;
            Password = string.Empty;
            Username = string.Empty;
        }

        public string Hostname { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public int Port { get; set; }
    }
}