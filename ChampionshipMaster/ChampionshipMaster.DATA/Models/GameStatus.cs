using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipMaster.DATA.Models
{
    public class GameStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //
        [StringLength(255)]
        public string? Name { get; set; }
    }
}
