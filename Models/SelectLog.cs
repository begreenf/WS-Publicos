namespace MyWebApp.Models
{
    public class SelectLog
    {
        public DateTime EventTime { get; set; }
        public string DatabaseName { get; set; }
        public string ObjectName { get; set; }
        public string Statement { get; set; }
    }
}
