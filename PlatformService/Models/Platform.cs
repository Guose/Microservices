using System.ComponentModel.DataAnnotations;

namespace PlatformService.Models
{
    public class Platform
    {
        [Key]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Publisher is required")]
        public string Publisher { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cost is required")]
        public string Cost { get; set; } = string.Empty;
    }
}