using CampusRecruitmentResumeService.Data;
using CampusRecruitmentResumeService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CampusRecruitmentResumeService.Services
{
    public class JobApplicationService
    {
        private readonly CampusRecruitmentResumeDbContext _context;

        public JobApplicationService(CampusRecruitmentResumeDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateJobApplicationAsync(JobApplication jobApplication)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                jobApplication.CreatedAt = DateTime.UtcNow;
                _context.JobApplications.Add(jobApplication);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> IsSubmissionAllowedAsync(string createdIp, string userAgent, string name)
        {
            var oneDayAgo = DateTime.UtcNow.AddDays(-1);
            var recentSubmissions = await _context.JobApplications
                .Where(j => (j.CreatedIp == createdIp || j.UserAgent == userAgent) && j.CreatedAt >= oneDayAgo)
                .ToListAsync();

            return recentSubmissions.Count(j => j.Name != name) < 3;
        }
    }
}