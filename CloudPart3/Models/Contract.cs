using System.ComponentModel.DataAnnotations;

namespace CloudPart3.Models
{
    public class Contract
    {
        [Key]
        public int ContractID { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileData { get; set; } //for storing binary content
        public DateTime UploadedAt { get; set; }
    }
}
