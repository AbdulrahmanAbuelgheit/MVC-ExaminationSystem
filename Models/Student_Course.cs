﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystemMVC.Models;

[PrimaryKey("StdID", "CrsID")]
public partial class Student_Course
{
    [Key]
    public int StdID { get; set; }

    [Key]
    public int CrsID { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string Grade { get; set; }

    [ForeignKey("CrsID")]
    [InverseProperty("Student_Courses")]
    public virtual Course Crs { get; set; }

    [ForeignKey("StdID")]
    [InverseProperty("Student_Courses")]
    public virtual Student Std { get; set; }
}