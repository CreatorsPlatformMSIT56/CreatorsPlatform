import type { Metadata } from "next";
import { SessionProvider } from "@/components/session";
import { Header } from "@/components/header";
import "./globals.css";
import "quill/dist/quill.snow.css";

export const metadata: Metadata = { title: { default: "IMAGINK 創作者平台", template: "%s | IMAGINK" }, description: "探索創作、支持喜愛的創作者，讓想像成真。" };
export default function Layout({ children }: Readonly<{ children: React.ReactNode }>) {
  return <html lang="zh-Hant"><body><SessionProvider><Header /><main id="main">{children}</main>
    <footer>IMAGINK · 每一份創作，都值得被看見。<LinkPrivacy /></footer></SessionProvider></body></html>;
}
function LinkPrivacy() { return <a href="/privacy">隱私權政策</a>; }
