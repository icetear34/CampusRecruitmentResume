using System;
using System.Collections.Generic;

namespace CampusRecruitmentResumeService.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AppliedPosition { get; set; }
        public DateTime? BirthDate { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string ContactAddress { get; set; }
        public List<JobApplicationEducation> Educations { get; set; }
        public List<JobApplicationCertificate> Certificates { get; set; }
        public List<JobApplicationExperience> Experiences { get; set; }
        public string LanguageSkillsEnglish { get; set; }
        public string LanguageSkillsJapanese { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedIp { get; set; }
        public string UserAgent { get; set; }
    }

    public class JobApplicationEducation
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public string SchoolName { get; set; }
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
    }

    public class JobApplicationCertificate
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public string CertificateName { get; set; }
        public DateTime ObtainedDate { get; set; }
    }

    public class JobApplicationExperience
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public int StartMonth { get; set; }
        public int StartYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
    }
}