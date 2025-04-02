namespace DocumentApi.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public string? PTTTrackingNumber { get; set; }
        public string? PTTCargoStatus { get; set; }
    }
}