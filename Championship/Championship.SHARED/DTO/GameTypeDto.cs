using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.SHARED.DTO
{
    public class GameTypeDto
    {
        
        public int Id { get; set; }
        //
        
        public string? Name { get; set; }

        public int? MaxPoints { get; set; }
        
        public string? TeamTypeName { get; set; }
    }
}
