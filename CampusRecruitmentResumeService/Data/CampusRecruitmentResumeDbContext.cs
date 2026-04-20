using CampusRecruitmentResumeService.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusRecruitmentResumeService.Data
{
    public class CampusRecruitmentResumeDbContext : DbContext
    {
        public CampusRecruitmentResumeDbContext(DbContextOptions<CampusRecruitmentResumeDbContext> options) : base(options)
        {
        }

        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobApplicationEducation> JobApplicationEducations { get; set; }
        public DbSet<JobApplicationCertificate> JobApplicationCertificates { get; set; }
        public DbSet<JobApplicationExperience> JobApplicationExperiences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobApplication>().ToTable("JOB_APPLICATION");
            modelBuilder.Entity<JobApplicationEducation>().ToTable("JOB_APPLICATION_EDUCATION");
            modelBuilder.Entity<JobApplicationCertificate>().ToTable("JOB_APPLICATION_CERTIFICATE");
            modelBuilder.Entity<JobApplicationExperience>().ToTable("JOB_APPLICATION_EXPERIENCE");

            // 映射欄位名稱（C# 屬性名稱與資料庫欄位名稱不同）
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.LanguageSkillsEnglish)
                .HasColumnName("LanguageSkills_English");

            modelBuilder.Entity<JobApplication>()
                .Property(j => j.LanguageSkillsJapanese)
                .HasColumnName("LanguageSkills_Japanese");

            modelBuilder.Entity<JobApplication>()
                .HasMany(j => j.Educations)
                .WithOne()
                .HasForeignKey(e => e.JobApplicationId);

            modelBuilder.Entity<JobApplication>()
                .HasMany(j => j.Certificates)
                .WithOne()
                .HasForeignKey(c => c.JobApplicationId);

            modelBuilder.Entity<JobApplication>()
                .HasMany(j => j.Experiences)
                .WithOne()
                .HasForeignKey(e => e.JobApplicationId);
        }
    }
}