﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService.Models
{
    public class Master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? ForecastDate { get; set; }
        public DateTime Date { get; set; }
        public CompletionStatus Status { get; set; }
        public ICollection<Detail> Details { get; set; } = new HashSet<Detail>();
    }

    public enum CompletionStatus
    {
        Success,
        Partial,
        Failed
    }
}
