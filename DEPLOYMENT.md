# CampusRecruitmentResumeService 部署說明（SQLite 版本）

## 專案概述
此服務為校園徵才履歷收件系統，使用 SQLite 作為資料庫，適合對外公開部署。

## 系統需求
- .NET 8.0 Runtime
- IIS 10.0 或更高版本
- ASP.NET Core Hosting Bundle

## 部署步驟

### 1. 編譯與打包
```powershell
cd D:\newGitea\CampusRecruitmentResumeService\project\CampusRecruitmentResumeService
dotnet restore
dotnet build -c Release
dotnet publish -c Release -o D:\Publish\CampusRecruitmentResumeService
```

### 2. 設定 IIS

#### 2.1 建立應用程式池
1. 開啟 IIS 管理員
2. 點擊「應用程式集區」→「新增應用程式集區」
3. 設定：
   - 名稱：`CampusRecruitmentResumeServiceAppPool`
   - .NET CLR 版本：`無受控程式碼`
   - 受控管線模式：`整合式`
4. 進階設定：
   - 啟用 32 位元應用程式：`False`
   - 身分識別：`ApplicationPoolIdentity`

#### 2.2 建立網站或應用程式
1. 在「Default Web Site」上點擊右鍵
2. 選擇「新增應用程式」
3. 設定：
   - 別名：`CampusRecruitmentResumeService`
   - 應用程式集區：選擇剛建立的 `CampusRecruitmentResumeServiceAppPool`
   - 實體路徑：`D:\Publish\CampusRecruitmentResumeService`

### 3. 設定權限
為應用程式池身分授予資料庫檔案的讀寫權限：
```powershell
$path = "D:\Publish\CampusRecruitmentResumeService"
$acl = Get-Acl $path
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS AppPool\CampusRecruitmentResumeServiceAppPool","FullControl","ContainerInherit,ObjectInherit","None","Allow")
$acl.SetAccessRule($rule)
Set-Acl $path $acl
```

### 4. 初始化資料庫
資料庫會在首次啟動時自動建立（使用 EnsureCreated）。
資料庫檔案位置：`D:\Publish\CampusRecruitmentResumeService\CampusRecruitmentResume.db`

### 5. 測試 API
- Health Check: `https://www.jochu.com/CampusRecruitmentResumeService/api/health`
- POST API: `https://www.jochu.com/CampusRecruitmentResumeService/api/job-applications`

## API 規格

### POST /api/job-applications

#### 必填欄位
- name (string)
- appliedPosition (string)
- mobilePhone (string)

#### 可選欄位
- birthDate (date, nullable)
- email (string, nullable)
- contactAddress (string, nullable)
- educations (array)
- certificates (array)
- experiences (array)
- languageSkillsEnglish (string: Advanced/Normal/Basic, nullable)
- languageSkillsJapanese (string: Advanced/Normal/Basic, nullable)
- remark (string, nullable)

#### 範例 JSON
```json
{
  "name": "張三",
  "appliedPosition": "軟體工程師",
  "birthDate": "1995-06-15",
  "mobilePhone": "0912345678",
  "email": "test@example.com",
  "contactAddress": "台北市信義區",
  "educations": [
    {
      "schoolName": "台灣大學",
      "degree": "學士",
      "fieldOfStudy": "資訊工程",
      "startYear": 2013,
      "endYear": 2017
    }
  ],
  "certificates": [],
  "experiences": [],
  "languageSkillsEnglish": "Advanced",
  "languageSkillsJapanese": "Normal",
  "remark": "期待加入貴公司"
}
```

## 防護機制
- 同一 IP 或 User-Agent 在一天內最多提交 3 次不同姓名的申請
- 驗證必填欄位
- Email 格式驗證
- 出生日期不可大於今天
- 年份範圍：1900 到當前年份
- 月份範圍：1-12
- 語言等級限制：Advanced/Normal/Basic

## 備份建議
定期備份 SQLite 資料庫檔案：
```powershell
Copy-Item "D:\Publish\CampusRecruitmentResumeService\CampusRecruitmentResume.db" `
          "D:\Backup\CampusRecruitmentResume_$(Get-Date -Format 'yyyyMMdd_HHmmss').db"
```

## 疑難排解
1. 如果無法寫入資料庫，檢查應用程式池身分的資料夾權限
2. 查看 IIS 日誌：`C:\inetpub\logs\LogFiles`
3. 查看應用程式日誌：Windows 事件檢視器 → 應用程式日誌

## 注意事項
- SQLite 是檔案型資料庫，適合中小型應用
- 如需高併發或大量資料，建議改用 SQL Server 或 PostgreSQL
- 定期備份資料庫檔案
- 確保資料庫檔案所在目錄有適當的權限設定
