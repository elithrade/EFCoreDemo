using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo
{
    public class SchoolDbContext : DbContext
    {
        private static string ConnectionString = "Server=localhost;Database=SchoolDb;User Id=sa;Password=Hbmp821329";

        public SchoolDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
            // Lazy loading
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<StudentInfo> StudentInfos { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<TeacherStudent> TeacherStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // HasRequired, HasOptional, or HasMany method to specify the type of relationship this entity participates in.
            // WithRequired, WithOptional, and WithMany method to specify the inverse relationship.

            // Configure one to one relationship between Student and StudentInfo,
            // this meaning that the Student entity object must include the StudentInfo
            // entity object and the StudentInfo entity must include the Student entity. 
            modelBuilder.Entity<Student>()
                .HasOne(student => student.StudentInfo) // EF Core HasOne(s => s.Info)
                .WithOne(info => info.Student) // EF Core WithOne(i => i.Student)
                .HasForeignKey<StudentInfo>(info => info.ID);

            // Configure one to many relationship between Course and Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(enrollment => enrollment.Course) // Enrollment entity has required the Course property
                .WithMany(course => course.Enrollments) // Configure the other end that Course entity includes many Enrollment entities
                .HasForeignKey(enrollment => enrollment.CourseID); // Specify the foreign key

            // Similarly one to many relationship between Student and Enrollment, but configure Student instead of Enrollment
            modelBuilder.Entity<Student>()
                .HasMany(student => student.Enrollments)
                .WithOne(enrollment => enrollment.Student)
                .HasForeignKey(enrollment => enrollment.StudentID);

            // Many-to-many relationships without an entity class to represent the
            // join table are not yet supported. However, you can represent a many-to-many
            // relationship by including an entity class for the join table and mapping
            // two separate one-to-many relationships.
            modelBuilder.Entity<TeacherStudent>()
                .HasKey(ts => new { ts.TeacherId, ts.StudentId });

            modelBuilder.Entity<TeacherStudent>()
                .HasOne(ts => ts.Student)
                .WithMany(student => student.TeacherStudents)
                .HasForeignKey(ts => ts.StudentId);

            modelBuilder.Entity<TeacherStudent>()
                .HasOne(ts => ts.Teacher)
                .WithMany(teacher => teacher.TeacherStudents)
                .HasForeignKey(ts => ts.TeacherId);
        }
    }

    // EF Core
    // To create a Many-to-Many relationship using Fluent API you have to create a Joining Entity.
    // This joining entity will contain the foreign keys (reference navigation property) for both the other entities.
    // These foreign keys will form the composite primary key for this joining entity.
    public class TeacherStudent
    {
        protected TeacherStudent() { }

        public int StudentId { get; set; } // Foreign key property
        public virtual Student Student { get; set; } // Reference navigation property

        public int TeacherId { get; set; } // Foreign key property
        public virtual Teacher Teacher { get; set; } // Reference navigation property
    }

    public class Student
    {
        public int StudentID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual StudentInfo StudentInfo { get; set; }

        // Collection navigation property
        public virtual ICollection<TeacherStudent> TeacherStudents { get; set; }
    }

    public class StudentInfo
    {
        public int ID { get; set; }
        public DateTime DateOfBirth { get; set; }
        // public string Address { get; set; }

        public virtual Student Student { get; set; }
    }

    public class Teacher
    {
        public int TeacherID { get; set; }
        public string Name { get; set; }

        // Collection navigation property
        public virtual ICollection<TeacherStudent> TeacherStudents { get; set; }
    }

    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Grade? Grade { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }

    public class Course
    {
        // The attribute lets you enter the primary key for the course rather than having the database generate it.
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
