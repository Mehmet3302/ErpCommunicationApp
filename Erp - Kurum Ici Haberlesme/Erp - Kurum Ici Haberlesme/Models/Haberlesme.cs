using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erp___Kurum_Ici_Haberlesme.Models
{
    public class Haberlesme
    {
        public enum Oncelik
        {
            IkiUcHafta,
            BirHafta,
            Hemen  
        }

        public enum TalepDurumu
        {
            Beklemede,
            Onaylandi,
            Red
        }

        public enum SatinAlmaDurumu
        {
            Beklemede,
            FiyatAlindi,
            OdemeBekliyor,
            Onaylandi,
            Red
        }

        [Key]
        public int TalebId { get; set; }


        [Required(ErrorMessage = "Açıklama alanı gereklidir.")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Öncelik alanı gereklidir.")]
        public Oncelik OncelikDurumu { get; set; }

        [Required(ErrorMessage = "Talep durumu alanı gereklidir.")]
        public TalepDurumu Durum { get; set; }

        [Required(ErrorMessage = "Oluşturma tarihi alanı gereklidir.")]
        public DateTime OlusturmaTarihi { get; set; }

        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }

        public string? OnaylayanPersonel { get; set; }

        public byte[]? PdfFile { get; set; }

        [NotMapped]
        [Display(Name = "PDF Dosyası")]
        public IFormFile? PdfFileUpload { get; set; }

        public string? SaticiBilgileri { get; set; }

        public SatinAlmaDurumu? SatinAlmaDurumuu { get; set; }
        public DateTime? FiyatAlmaTarihi { get; set; }
        public decimal? OdemeMiktari { get; set; }
        public DateTime? OdemeTarihi { get; set; }
        public bool? OdemeOnaylandiMi { get; set; }
    }
}
