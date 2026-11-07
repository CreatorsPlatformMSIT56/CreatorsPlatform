"use client";
export default function ErrorPage({ reset }: { reset: () => void }) {
  return <section className="empty"><h1>頁面暫時無法顯示</h1><button onClick={reset}>重新載入</button></section>;
}
