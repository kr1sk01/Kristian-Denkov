using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipApp.Models
{
    public class Championship
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string? Name { get; set; }
        public byte[]? Logo { get; set; }
        [Required]
        [ForeignKey("ChampionshipType")]
        public int ChampionshipTypeId { get; set; }
        [Required]
        [ForeignKey("ChampionshipStatus")]
        public int ChampionshipStatusId { get; set; }
        [Required]
        [ForeignKey("GameType")]
        public int GameTypeId { get; set; }
        [ForeignKey("Winner")]
        public int? WinnerId { get; set; }
        public DateTime? LotDate { get; set; }
        public DateTime? Date { get; set; }
        [Required]
        public string? CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public ChampionshipType ChampionshipType { get; set; }
        public ChampionshipStatus ChampionshipStatus { get; set; }
        public GameType GameType { get; set; }
        public Team? Winner { get; set; }
    }
}
