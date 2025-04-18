﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystemMVC.Models;

[PrimaryKey("ExamID", "StdID", "QID")]
public partial class Exam_Student_Question
{
    [Key]
    public int ExamID { get; set; }

    [Key]
    public int StdID { get; set; }

    [Key]
    public int QID { get; set; }

    [StringLength(10)]
    public string SelectedOpt { get; set; }

    [ForeignKey("ExamID")]
    [InverseProperty("Exam_Student_Questions")]
    public virtual Exam Exam { get; set; }

    [ForeignKey("QID")]
    [InverseProperty("Exam_Student_Questions")]
    public virtual Question QIDNavigation { get; set; }

    [ForeignKey("StdID")]
    [InverseProperty("Exam_Student_Questions")]
    public virtual Student Std { get; set; }
}