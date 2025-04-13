﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystemMVC.Models;

public partial class Branch
{
    [Key]
    public int BranchID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string BranchName { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Governate { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string City { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Street { get; set; }

    public int Tel { get; set; }

    public int ManagerID { get; set; }

    [ForeignKey("ManagerID")]
    [InverseProperty("Branches")]
    public virtual Instructor Manager { get; set; }

    [InverseProperty("Branch")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    [ForeignKey("BranchID")]
    [InverseProperty("BranchesNavigation")]
    public virtual ICollection<Instructor> Ins { get; set; } = new List<Instructor>();

    [ForeignKey("BranchID")]
    [InverseProperty("Branches")]
    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}