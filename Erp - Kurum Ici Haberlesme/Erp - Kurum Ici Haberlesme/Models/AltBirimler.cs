using System.ComponentModel.DataAnnotations;

namespace Erp___Kurum_Ici_Haberlesme.Models
{
    public class AltBirimler
    {
        [Key]
        public int AltBirimId { get; set; }
        public string? AltBirimAdı { get; set; }
        public int BirimId { get; set; }
        public virtual Birimler Birim { get; set; }
    }
}
