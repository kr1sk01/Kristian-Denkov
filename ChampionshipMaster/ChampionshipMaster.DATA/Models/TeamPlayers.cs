﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChampionshipMaster.DATA.Models
{
    public class TeamPlayers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? PlayerId { get; set; }
        public virtual Player? Player { get; set; }

        public int? TeamId { get; set; }
        public virtual Team? Team { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
