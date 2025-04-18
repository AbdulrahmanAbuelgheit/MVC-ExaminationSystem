﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystemMVC.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Courses_Topic> Courses_Topics { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Exam_Student_Question> Exam_Student_Questions { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Questions_Option> Questions_Options { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Student_Course> Student_Courses { get; set; }

    public virtual DbSet<Student_Exam> Student_Exams { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<User_PhoneNumber> User_PhoneNumbers { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasOne(d => d.Manager).WithMany(p => p.Branches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Branches_Instructor");

            entity.HasMany(d => d.Tracks).WithMany(p => p.Branches)
                .UsingEntity<Dictionary<string, object>>(
                    "Branch_Track",
                    r => r.HasOne<Track>().WithMany()
                        .HasForeignKey("TrackID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Branch_Tracks_Tracks"),
                    l => l.HasOne<Branch>().WithMany()
                        .HasForeignKey("BranchID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Branch_Tracks_Branches1"),
                    j =>
                    {
                        j.HasKey("BranchID", "TrackID");
                        j.ToTable("Branch_Tracks");
                    });
        });

        modelBuilder.Entity<Courses_Topic>(entity =>
        {
            entity.HasOne(d => d.Crs).WithMany(p => p.Courses_Topics)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Courses_Topics_Courses");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue("T");

            entity.HasOne(d => d.Crs).WithMany(p => p.Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exams_Courses");

            entity.HasMany(d => d.QIDs).WithMany(p => p.Exams)
                .UsingEntity<Dictionary<string, object>>(
                    "Exam_Question",
                    r => r.HasOne<Question>().WithMany()
                        .HasForeignKey("QID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Exam_Questions_Questions"),
                    l => l.HasOne<Exam>().WithMany()
                        .HasForeignKey("ExamID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Exam_Questions_Exams"),
                    j =>
                    {
                        j.HasKey("ExamID", "QID");
                        j.ToTable("Exam_Questions");
                    });
        });

        modelBuilder.Entity<Exam_Student_Question>(entity =>
        {
            entity.Property(e => e.SelectedOpt).IsFixedLength();

            entity.HasOne(d => d.Exam).WithMany(p => p.Exam_Student_Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Student_Questions_Exams");

            entity.HasOne(d => d.QIDNavigation).WithMany(p => p.Exam_Student_Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Student_Questions_Questions");

            entity.HasOne(d => d.Std).WithMany(p => p.Exam_Student_Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Exam_Student_Questions_Student");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.Property(e => e.InsID).ValueGeneratedNever();

            entity.HasOne(d => d.Ins).WithOne(p => p.Instructor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Instructor_User");

            entity.HasMany(d => d.BranchesNavigation).WithMany(p => p.Ins)
                .UsingEntity<Dictionary<string, object>>(
                    "Instructor_Branch",
                    r => r.HasOne<Branch>().WithMany()
                        .HasForeignKey("BranchID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Instructor_Branches_Branches"),
                    l => l.HasOne<Instructor>().WithMany()
                        .HasForeignKey("InsID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Instructor_Branches_Instructor"),
                    j =>
                    {
                        j.HasKey("InsID", "BranchID");
                        j.ToTable("Instructor_Branches");
                    });

            entity.HasMany(d => d.Crs).WithMany(p => p.Ins)
                .UsingEntity<Dictionary<string, object>>(
                    "Instructor_Course",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CrsID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Instructor_Courses_Courses"),
                    l => l.HasOne<Instructor>().WithMany()
                        .HasForeignKey("InsID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Instructor_Courses_Instructor"),
                    j =>
                    {
                        j.HasKey("InsID", "CrsID");
                        j.ToTable("Instructor_Courses");
                    });

            entity.HasMany(d => d.Tracks).WithMany(p => p.Ins)
                .UsingEntity<Dictionary<string, object>>(
                    "Instructor_Track",
                    r => r.HasOne<Track>().WithMany()
                        .HasForeignKey("TrackID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Instructor_Tracks_Tracks"),
                    l => l.HasOne<Instructor>().WithMany()
                        .HasForeignKey("InsID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Instructor_Tracks_Instructor"),
                    j =>
                    {
                        j.HasKey("InsID", "TrackID");
                        j.ToTable("Instructor_Tracks");
                    });
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasOne(d => d.Crs).WithMany(p => p.Questions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Questions_Courses");
        });

        modelBuilder.Entity<Questions_Option>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("trg_CheckOptionsCount"));

            entity.HasOne(d => d.QIDNavigation).WithMany(p => p.Questions_Options).HasConstraintName("FK_Questions_Options_Questions1");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.StdID).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue("T");

            entity.HasOne(d => d.Branch).WithMany(p => p.Students)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Branches");

            entity.HasOne(d => d.Std).WithOne(p => p.Student)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_User");

            entity.HasOne(d => d.Track).WithMany(p => p.Students)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Tracks");
        });

        modelBuilder.Entity<Student_Course>(entity =>
        {
            entity.HasOne(d => d.Crs).WithMany(p => p.Student_Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Courses_Courses");

            entity.HasOne(d => d.Std).WithMany(p => p.Student_Courses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Courses_Student");
        });

        modelBuilder.Entity<Student_Exam>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("ExamScore"));

            entity.HasOne(d => d.Exam).WithMany(p => p.Student_Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Exams_Exams");

            entity.HasOne(d => d.Std).WithMany(p => p.Student_Exams)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Exams_Student");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasOne(d => d.Supervisor).WithMany(p => p.TracksNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tracks_Instructor");

            entity.HasMany(d => d.Crs).WithMany(p => p.Tracks)
                .UsingEntity<Dictionary<string, object>>(
                    "Tracks_Course",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CrsID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Tracks_Courses_Courses"),
                    l => l.HasOne<Track>().WithMany()
                        .HasForeignKey("TrackID")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Tracks_Courses_Tracks"),
                    j =>
                    {
                        j.HasKey("TrackID", "CrsID");
                        j.ToTable("Tracks_Courses");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID).HasName("PK_User");
        });

        modelBuilder.Entity<User_PhoneNumber>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.User_PhoneNumbers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_PhoneNumbers_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}