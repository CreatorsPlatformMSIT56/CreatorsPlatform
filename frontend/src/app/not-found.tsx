import Link from "next/link";
/** 呈現找不到頁面時的提示。 */
export default function NotFound() {
  return (
    <section className="empty">
      <h1>找不到這個頁面</h1>
      <Link href="/">回到首頁</Link>
    </section>
  );
}
