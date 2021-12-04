namespace Common
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpFrom { get; set; }
    }
}
