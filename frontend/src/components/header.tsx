"use client";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { useSession } from "./session";
import { useState } from "react";

export function Header() {
  const { member, refresh } = useSession();
  const router = useRouter();
  const [error, setError] = useState("");
  return <><header className="header"><Link href="/" className="brand">IMAG<span>INK</span><small>創作，讓想像成真</small></Link>
    <nav aria-label="主要導覽"><Link href="/explore">探索作品</Link><Link href="/explore?kind=commissions">找委託</Link><Link href="/events">活動</Link>
      {member ? <><Link href="/feed">追蹤動態</Link><Link href="/account">{member.name}</Link><button onClick={async () => {
        try { await api("account/logout", "POST"); await refresh(); router.push("/"); } catch { setError("登出失敗，請重試。"); }
      }}>登出</button></> : <Link className="button" href="/login">登入 / 註冊</Link>}
    </nav></header>{error && <p role="alert" className="error">{error}</p>}</>;
}
