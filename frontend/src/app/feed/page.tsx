import { Suspense } from "react";
import { Feed } from "@/components/feed";
/** 呈現此路由的頁面內容。 */
export default function Page() {
  return (
    <Suspense fallback={<p>載入中…</p>}>
      <Feed />
    </Suspense>
  );
}
