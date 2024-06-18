using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TermisImporterService
{
    public class CsvData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        public string Day { get; set; }

        [Required]
        public string Hour { get; set; }

        [Required]
        public double Parameter { get; set; }

        public int MasterId { get; set; }
        public Master Master { get; set; }
    }


}

