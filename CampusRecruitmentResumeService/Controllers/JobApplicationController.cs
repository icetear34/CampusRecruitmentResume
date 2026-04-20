using CampusRecruitmentResumeService.Models;
using CampusRecruitmentResumeService.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CampusRecruitmentResumeService.Controllers
{
    [ApiController]
    [Route("api/job-applications")]
    public class JobApplicationController : ControllerBase
    {
        private readonly JobApplicationService _service;

        public JobApplicationController(JobApplicationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobApplication([FromBody] JobApplication jobApplication)
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(jobApplication.Name))
            {
                ModelState.AddModelError("name", "姓名為必填欄位");
            }

            if (string.IsNullOrWhiteSpace(jobApplication.AppliedPosition))
            {
                ModelState.AddModelError("appliedPosition", "應徵職位為必填欄位");
            }

            if (string.IsNullOrWhiteSpace(jobApplication.MobilePhone))
            {
                ModelState.AddModelError("mobilePhone", "手機號碼為必填欄位");
            }

            // 驗證 Email 格式
            if (!string.IsNullOrWhiteSpace(jobApplication.Email) &&
                !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(jobApplication.Email))
            {
                ModelState.AddModelError("email", "Email 格式不正確");
            }

            // 驗證出生日期
            if (jobApplication.BirthDate.HasValue && jobApplication.BirthDate.Value > DateTime.UtcNow)
            {
                ModelState.AddModelError("birthDate", "出生日期不可大於今天");
            }

            // 驗證學歷
            if (jobApplication.Educations != null)
            {
                for (int i = 0; i < jobApplication.Educations.Count; i++)
                {
                    var edu = jobApplication.Educations[i];
                    if (edu.StartYear < 1900 || edu.StartYear > DateTime.UtcNow.Year)
                    {
                        ModelState.AddModelError($"educations[{i}].startYear", "開始年份必須在 1900 到當前年份之間");
                    }
                    if (edu.EndYear.HasValue && (edu.EndYear.Value < 1900 || edu.EndYear.Value > DateTime.UtcNow.Year))
                    {
                        ModelState.AddModelError($"educations[{i}].endYear", "結束年份必須在 1900 到當前年份之間");
                    }
                    if (edu.EndYear.HasValue && edu.EndYear.Value < edu.StartYear)
                    {
                        ModelState.AddModelError($"educations[{i}].endYear", "結束年份不可小於開始年份");
                    }
                }
            }

            // 驗證工作經歷
            if (jobApplication.Experiences != null)
            {
                for (int i = 0; i < jobApplication.Experiences.Count; i++)
                {
                    var exp = jobApplication.Experiences[i];
                    if (exp.StartMonth < 1 || exp.StartMonth > 12)
                    {
                        ModelState.AddModelError($"experiences[{i}].startMonth", "開始月份必須在 1 到 12 之間");
                    }
                    if (exp.EndMonth.HasValue && (exp.EndMonth.Value < 1 || exp.EndMonth.Value > 12))
                    {
                        ModelState.AddModelError($"experiences[{i}].endMonth", "結束月份必須在 1 到 12 之間");
                    }
                }
            }

            // 驗證語言技能
            var validLevels = new[] { "Advanced", "Normal", "Basic" };
            if (!string.IsNullOrWhiteSpace(jobApplication.LanguageSkillsEnglish) &&
                !validLevels.Contains(jobApplication.LanguageSkillsEnglish))
            {
                ModelState.AddModelError("languageSkillsEnglish", "英語等級僅允許 Advanced/Normal/Basic");
            }
            if (!string.IsNullOrWhiteSpace(jobApplication.LanguageSkillsJapanese) &&
                !validLevels.Contains(jobApplication.LanguageSkillsJapanese))
            {
                ModelState.AddModelError("languageSkillsJapanese", "日語等級僅允許 Advanced/Normal/Basic");
            }

            // 如果有驗證錯誤，返回詳細錯誤訊息
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "驗證失敗",
                    errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                });
            }

            // 驗證提交頻率限制
            var createdIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (!await _service.IsSubmissionAllowedAsync(createdIp, userAgent, jobApplication.Name))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "提交次數超過限制",
                    errors = new { general = new[] { "同一 IP 或 User-Agent 在一天內最多提交 3 次不同姓名的申請" } }
                });
            }

            try
            {
                jobApplication.CreatedIp = createdIp;
                jobApplication.UserAgent = userAgent;
                await _service.CreateJobApplicationAsync(jobApplication);
                return Ok(new { success = true, message = "申請已成功提交" });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "系統錯誤",
                    errors = new { general = new[] { "處理您的申請時發生錯誤，請稍後再試" } }
                });
            }
        }
    }
}