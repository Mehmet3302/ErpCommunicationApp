using System.ComponentModel.DataAnnotations;

namespace Erp___Kurum_Ici_Haberlesme.Models
{
    public class Birimler
    {
        [Key]
        public int BirimId { get; set; }
        public string? BirimAd { get; set; }
    }
}
