using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class Blog:BaseEntity
    {
        [Required(ErrorMessage = "Don't be empty ! Please Add Header text")]
        public string Header { get; set; }

        [Required(ErrorMessage = "Don't be empty ! Please Add Description text")]
        public string Description { get; set; }
        
        public string Image { get; set; }

        [NotMapped, Required(ErrorMessage = "Don't be empty ! Please Add Image")]
        public IFormFile Photo { get; set; }

        [Required(ErrorMessage = "Don't be empty ! Please Add Date")]
        public DateTime Date { get; set; }
    }
}
