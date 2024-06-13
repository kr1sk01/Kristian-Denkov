using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiceConsoleApp;

public class CsvData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Month { get; set; } = default!;

    [Required]
    public string Day { get; set; } = default!;

    [Required]
    public string Hour { get; set; } = default!;

    [Required]
    public double Parameter { get; set; } = default!;

    public int MasterId { get; set; } = default!;
    public Master Master { get; set; } = default!;
}

