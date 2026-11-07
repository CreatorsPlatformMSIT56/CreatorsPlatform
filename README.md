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

若後端網址不同，複製 `frontend/.env.example` 為 `.env.local`，修改 `BACKEND_URL`，再重啟前端。Next.js 透過 rewrites 代理 `/api`、`/images`、`/img`，瀏覽器只與前端同源通訊並使用 HttpOnly 驗證 Cookie（ASP.NET Core Cookie Authentication）。不要將資料庫或 SMTP 設定放入前端環境變數。

## 功能與資料

- `/explore`：作品與委託搜尋、分類、排序、分頁。
- `/creators/:id`、`/works/:id`、`/commissions/:id`：創作者、作品與委託。
- `/feed`：追蹤動態。
- `/events`：活動、建立、編輯、投稿。
- `/account`：會員、創作者設定、作品、委託、方案、訂閱與訂單管理。
- `/login`、`/register`、`/forgot-password`：帳號功能。

描述內容沿用 Quill Delta JSON，編輯時使用 Quill，閱讀時轉成經過清理的 HTML，不必在瀏覽器啟動 Quill。

作品、創作者、委託與活動詳情，以及探索搜尋結果由 Next.js Server Components 取得 C# API 資料，初始 HTML 即包含主要內容與依資料產生的頁面標題／描述。追蹤、按讚、留言及投稿成功後刷新伺服器內容；會員中心與訂單管理維持瀏覽器端 React。首頁、活動列表與追蹤動態仍由瀏覽器取得資料。

伺服器讀取 API 時只轉送該請求的 `Imagink.Auth` Cookie，採用 `no-store`，不共用個人化資料快取。這次不增加觀看權限或金流規則，相關業務判斷仍由 C# API 處理。

訂閱維持原專案的示範性質：建立一個月的訂閱紀錄，不處理真實金流，也不蒐集信用卡資料。密碼儲存沿用既有資料庫設計。部署到正式環境前，需另行完成密碼雜湊與真實金流設計。

忘記密碼需要後端 `Mail:Username`、`Mail:Password` 設定（環境變數為 `Mail__Username`、`Mail__Password`），使用 Gmail SMTP。驗證碼由伺服器產生，有效十分鐘；驗證碼目前儲存在單一程序記憶體，重啟會清除。登入使用加密驗證 Cookie，有效期限為 30 分鐘並啟用滑動續期；受保護 API 由 `[Authorize]` 檢查，未登入回傳 401。正式多執行個體部署需持久化並共用 ASP.NET Core Data Protection 金鑰。

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
npm run test:ssr
```

後端整合測試使用每個測試獨立的 SQLite 記憶體資料庫與真實 HTTP pipeline，覆蓋 Cookie 驗證、JSON 寫入限制、權限、作品上下架、訂閱、訂單狀態及活動編輯。測試不連線、寫入或清除正式 SQL Server。SQLite 不涵蓋所有 SQL Server 查詢翻譯；探索、詳情與媒體查詢另以既有 SQL Server 執行唯讀驗證。

前端內容測試涵蓋 Quill Delta 轉換、格式保留、HTML 清理與活動配色解析。`npm run test:ssr` 需先執行 `npm run build`，會啟動獨立的 Next.js 正式程序與記憶體測試 API，驗證可見 HTML（排除 script 資料）、metadata、Cookie 隔離、搜尋／分頁／排序與 404／500；不使用現有資料庫。瀏覽器人工檢查另驗證互動刷新。SMTP 寄送及真實金流未列入自動測試；平台目前沒有真實金流整合。

伺服器元件與互動分工依 [Next.js Server and Client Components](https://nextjs.org/docs/app/getting-started/server-and-client-components)，錯誤頁使用目前版本的 [`retry()`](https://nextjs.org/docs/app/api-reference/file-conventions/error#retry) 重新取得伺服器資料。
