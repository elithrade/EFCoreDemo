using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new SchoolDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                new SchoolDbInitializer().Seed(context);

                var students = context.Students.ToList();

                // Eager loading
                var studentsEagerLoading = context.Students.Include(s => s.StudentInfo).Include(s => s.Enrollments);
                // Explicit loading
                var randomStudent = context.Students.Single(s => s.StudentID == 1);
                context.Entry(randomStudent).Collection(s => s.Enrollments).Load();
                context.Entry(randomStudent).Reference(s => s.StudentInfo).Load();

                foreach (var student in students)
                {
                    Console.WriteLine($"{student.FirstMidName} enrolled course " +
                        $"{string.Join(", ", student.Enrollments.Select(e => e.Course.Title))} on " +
                        $"{student.EnrollmentDate}");
                }

                List<Enrollment> enrollments = students
                    .SelectMany(x => x.Enrollments)
                    .Where(e => e.Grade == Grade.A).ToList();

                Console.WriteLine();

                foreach (var enrollment in enrollments)
                {
                    Console.WriteLine($"{enrollment.Student.FirstMidName}" +
                        $"received grade {enrollment.Grade} in " +
                        $"course {enrollment.Course.Title}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
