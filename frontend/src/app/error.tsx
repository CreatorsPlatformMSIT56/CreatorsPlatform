"use client";
export default function ErrorPage({ retry }: { retry: () => void }) {
  return <section className="empty"><h1>頁面暫時無法顯示</h1><button onClick={retry}>重新載入</button></section>;
}
