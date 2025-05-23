﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystemMVC.Models;

[PrimaryKey("OptNum", "QID")]
public partial class Questions_Option
{
    [Key]
    public int OptNum { get; set; }

    [Key]
    public int QID { get; set; }

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string OptText { get; set; }

    [ForeignKey("QID")]
    [InverseProperty("Questions_Options")]
    public virtual Question QIDNavigation { get; set; }
}