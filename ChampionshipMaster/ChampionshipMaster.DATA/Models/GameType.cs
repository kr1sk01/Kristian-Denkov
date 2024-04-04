using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipMaster.DATA.Models
{
    public class GameType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //
        [StringLength(255)]
        public string? Name { get; set; }

        public int? MaxPoints { get; set; }

        public int? TeamTypeId { get; set; }
        [ForeignKey("TeamTypeId")]
        public TeamType? TeamType { get; set; }
    }
}
