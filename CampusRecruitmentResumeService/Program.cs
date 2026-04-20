using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CampusRecruitmentResumeService.Data;
using CampusRecruitmentResumeService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<CampusRecruitmentResumeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<JobApplicationService>();

var app = builder.Build();

// 確保資料庫已建立
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampusRecruitmentResumeDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage(); // 顯示詳細錯誤訊息

app.UsePathBase("/CampusRecruitmentResumeService");

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();