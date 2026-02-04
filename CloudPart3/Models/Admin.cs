using System.ComponentModel.DataAnnotations;

namespace CloudPart3.Models
{
    public class Admin
    {
        [Key]
        public int adminID { get; set; }
        public string adminEmail { get; set; }

        public string adminPassword { get; set; }
    }
}
