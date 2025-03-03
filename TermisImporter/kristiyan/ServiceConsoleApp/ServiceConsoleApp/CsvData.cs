﻿using ServiceConsoleApp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public double EarthTemperature { get; set; } = default!;
    public double AirTemperature { get; set; } = default!;
    public int MasterId { get; set; } = default!;
    public Master Master { get; set; } = default!;
}

