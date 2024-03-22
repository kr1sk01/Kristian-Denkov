using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChampionshipApp.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        [ForeignKey("GameType")]
        public int? GameTypeID { get; set; }
        [ForeignKey("GameStatus")]
        public int? GameStatusID { get; set; }
        [ForeignKey("BlueTeam")]
        public int? BlueTeamID { get; set; }
        [ForeignKey("RedTeam")]
        public int? RedTeamID { get; set; }
        public int? BluePoints { get; set; }
        public int? RedPoints { get; set; }
        public DateTime? Date { get; set; }
        [Required]
        public string? CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        // Add other properties as needed

        // Navigation properties
        // These should be virtual to leverage Entity Framework's lazy loading feature
        // You can also include foreign key properties as needed
        [Required]
        public virtual GameType? GameType { get; set; }
        [Required]
        public virtual GameStatus? GameStatus { get; set; }
        [Required]
        public virtual Team BlueTeam { get; set; }
        [Required]
        public virtual Team RedTeam { get; set; }
    }
}
