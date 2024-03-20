using System.ComponentModel.DataAnnotations;

namespace ChampionshipApp.Models
{
    public class TeamType
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        [Required]
        public string? CreatedBy { get; set; }
        [Required]
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
