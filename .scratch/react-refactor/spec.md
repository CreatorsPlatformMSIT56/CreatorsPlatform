# CreatorPlatform React / Next.js 重構

來源：使用者要求「使用 React 和 Next.js 重構 CreatorPlatform，取代 Razor Page」。
追加要求：使用 `refactor/react` 分支，每階段 code-review 後 commit。

## 範圍

- 保留 ASP.NET Core、EF Core 與現有 SQL Server 結構作為後端，改為 JSON API。
- Next.js App Router 與 React / TypeScript 取代全部 MVC Razor Views 和 jQuery 頁面程式。
- 搬移探索、搜尋/分類/排序/分頁、創作者資料/追蹤/方案、作品/按讚/留言、委託/下單/訂單狀態、訂閱紀錄、活動/建立/編輯/投稿，以及會員註冊/登入/登出/資料/頭像/密碼重設。
- 維持現有訂閱示範流程；沒有第三方支付整合，不蒐集信用卡資料。
- 保留資料庫與靜態圖片，不增加舊路由相容層。
- 使用同源代理傳送 Session Cookie；API 僅公開需要的資料，寫入驗證登入與資料擁有者。
- 錯誤、載入、空資料、未登入狀態需可見且可處理。
- 本次在既有 repository 本機重構，不建立外部託管網站。

## 階段與審查基準

1. 可執行的前後端重構：以起始 HEAD `6738ceaa2aa5cb97094395baeaba455839624498` 審查 `CreatorsPlatform/`、`frontend/`，同時提供替代前端並移除舊介面。
2. 整合驗證：以前階段 commit 審查測試、執行說明與整合修正。

第一輪 Standards 審查指出後端單獨提交會讓頁面無法使用，因此合併原訂的後端與前端階段，保持每個提交皆有可執行產品。

每階段都先執行適當檢查，再以 Standards 與 Spec 兩軸審查，修正實質問題後 commit。尚未提交的變更以 `git diff <階段起點> -- <範圍>` 審查，包含指定範圍內新增檔案。
