using CapstoneQuizzCreationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneQuizzCreationApp.Context
{
    public class QuizzContext:DbContext
    {
        public QuizzContext(DbContextOptions options) : base(options)
        {

        }
        public  DbSet<User> Users { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<UserCredential> Credentials { get; set; }  
        public DbSet<Certificate> Certificates { get; set; } 
        public DbSet<CertificationTest> CertificationTests { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Option> Options { get; set; }  
        public DbSet<Question> Questions { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<SubmissionAnswer> SubmissionAnswers { get; set; }
        public DbSet<TestHistory> TestHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Certificate relationships
            modelBuilder.Entity<Certificate>()
               .HasOne(c => c.Submission)
               .WithMany()
               .HasForeignKey(c => c.SubmissionId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
            modelBuilder.Entity<Favourite>()
             .HasOne(c => c.User)
             .WithMany(u=>u.Favourites)
             .HasForeignKey(c => c.UserId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.CertificationTest)
                .WithMany()
                .HasForeignKey(c => c.TestId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
          

            // Configure Submission relationships
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.CertificationTest)
                .WithMany()
                .HasForeignKey(s => s.TestId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Configure Question-Option relationship
            modelBuilder.Entity<Option>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Question relationships
            modelBuilder.Entity<Question>()
                .HasOne(q => q.CertificationTest)
                .WithMany(c => c.Questions)
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            modelBuilder.Entity<TestHistory>()
            .HasOne(th => th.Certificate)
            .WithMany()
            .HasForeignKey(th => th.CertificateId)
            .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Question>()
            //    .HasOne(q => q.Option)
            //    .WithMany()
            //    .HasForeignKey(q => q.CorrectAnswerId)
            //    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
