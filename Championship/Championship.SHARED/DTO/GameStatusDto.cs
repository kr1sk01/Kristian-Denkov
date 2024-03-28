using System.ComponentModel.DataAnnotations;

namespace Championship.SHARED.DTO
{
    public class GameStatusDto
    {
        [Key]
        public string Id { get; set; } = default!;
        //
        [StringLength(255)]
        public string? Name { get; set; }
    }
}
