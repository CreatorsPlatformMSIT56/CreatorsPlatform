# IMAGINK CreatorPlatform

Next.js App Router / React / TypeScript 提供介面，ASP.NET Core 10 / EF Core 提供 JSON API，沿用既有 SQL Server 資料庫。Razor Views 與舊 MVC 路由已移除。

## 本機啟動

需要 .NET 10 SDK、Node.js 22.18 以上（測試使用 Node 內建 TypeScript 支援）與既有 IMAGINK SQL Server 資料庫。

1. 使用 `CreatorsPlatform/appsettings.Development.json` 或環境變數 `ConnectionStrings__ImaginkConnstring` 設定資料庫連線。不要將密碼提交到 Git。
2. 終端一啟動後端：

   ```powershell
   dotnet run --project CreatorsPlatform --launch-profile http
   ```

3. 終端二啟動前端：

   ```powershell
   cd frontend
   npm ci
   npm run dev
   ```

4. 開啟 <http://localhost:3000>。後端預設在 `http://localhost:5251`，`/health` 可確認程序狀態。

若後端網址不同，複製 `frontend/.env.example` 為 `.env.local`，修改 `BACKEND_URL`，再重啟前端。Next.js 透過 rewrites 代理 `/api`、`/images`、`/img`，瀏覽器只與前端同源通訊並使用 HttpOnly Session Cookie。不要將資料庫或 SMTP 設定放入前端環境變數。

## 功能與資料

- `/explore`：作品與委託搜尋、分類、排序、分頁。
- `/creators/:id`、`/works/:id`、`/commissions/:id`：創作者、作品與委託。
- `/feed`：追蹤動態。
- `/events`：活動、建立、編輯、投稿。
- `/account`：會員、創作者設定、作品、委託、方案、訂閱與訂單管理。
- `/login`、`/register`、`/forgot-password`：帳號功能。

描述內容沿用 Quill Delta JSON，透過 Quill 顯示及編輯。一般文字與 HTML 介紹會經過清理後顯示。

訂閱維持原專案的示範性質：建立一個月的訂閱紀錄，不處理真實金流，也不蒐集信用卡資料。密碼儲存沿用既有資料庫設計。部署到正式環境前，需另行完成密碼雜湊與真實金流設計。

忘記密碼需要後端 `Mail:Username`、`Mail:Password` 設定（環境變數為 `Mail__Username`、`Mail__Password`），使用 Gmail SMTP。驗證碼由伺服器產生，有效十分鐘；驗證碼與 Session 目前儲存在單一程序記憶體，重啟會清除。

## 建置與執行

```powershell
dotnet build CreatorsPlatform.sln
cd frontend
npm run build
npm run start
```

正式環境需同時執行 ASP.NET Core API 與 Next.js Node.js 服務，對外入口為 Next.js；部署前設定 `BACKEND_URL` 並重新建置。靜態圖片由後端 `wwwroot/img` 提供。請使用 HTTPS。

Next.js 架構與代理設定依官方文件：[App Router 安裝](https://nextjs.org/docs/app/getting-started/installation)、[rewrites](https://nextjs.org/docs/app/api-reference/config/next-config-js/rewrites)。

## 驗證

```powershell
dotnet test CreatorsPlatform.sln
cd frontend
npm test
npm run typecheck
npm run build
```

後端整合測試使用每個測試獨立的 SQLite 記憶體資料庫與真實 HTTP pipeline，覆蓋 Session、JSON 寫入限制、權限、作品上下架、訂閱、訂單狀態及活動編輯。測試不連線、寫入或清除正式 SQL Server。SQLite 不涵蓋所有 SQL Server 查詢翻譯；探索、詳情與媒體查詢另以既有 SQL Server 執行唯讀驗證。

前端測試涵蓋既有 Quill Delta 辨識、HTML 清理與活動配色解析。瀏覽器人工檢查涵蓋實際資料呈現與響應式頁面。SMTP 寄送及真實金流未列入自動測試；平台目前沒有真實金流整合。
