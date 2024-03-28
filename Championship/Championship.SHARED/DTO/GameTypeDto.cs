using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.SHARED.DTO
{
    public class GameTypeDto
    {
        [Key]
        public string Id { get; set; } = default!;
        //
        [StringLength(255)]
        public string? Name { get; set; }

        public int? MaxPoints { get; set; }
        
        public string? TeamTypeId { get; set; }
        [ForeignKey("TeamTypeId")]
        public TeamTypeDto? TeamType { get; set; }
    }
}
