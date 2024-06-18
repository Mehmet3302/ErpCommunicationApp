using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Erp___Kurum_Ici_Haberlesme.ViewModel
{
    public class EditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Personel Adı ve Soyadı gereklidir.")]
        [Display(Name = "Personel Adı ve Soyadı")]
        public string? PersonelAdSoyad { get; set; }

        [Required(ErrorMessage = "TC Kimlik Numarası gereklidir.")]
        [Display(Name = "TC Kimlik Numarası")]
        public string? TcKimlikNo { get; set; }

        [Display(Name = "Profil Resmi")]
        public byte[]? ProfilResmiDosyasi { get; set; }

        [Required(ErrorMessage = "Birim seçiniz.")]
        [Display(Name = "Personelin Bağlı Olduğu Birim")]
        public int BirimId { get; set; }

        [Display(Name = "Personelin Bağlı Olduğu Alt Birim")]
        public int AltBirimId { get; set; }

        public IList<string>? SelectedRoles { get; set; }

        public List<SelectListItem>? Birimler { get; set; }

        public List<SelectListItem>? AltBirimler { get; set; }
    }
}
