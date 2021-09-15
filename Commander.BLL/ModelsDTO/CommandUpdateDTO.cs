using System.ComponentModel.DataAnnotations;

namespace Commander.BLL.ModelsDTO
{
    public class CommandUpdateDTO
    {
        [Required]
        [MaxLength(250)]
        public string HowTo { get; set; }
        [Required]
        [MaxLength(100)]
        public string Line { get; set; }
        [Required]
        [MaxLength(50)]
        public string Platform { get; set; }
    }
}