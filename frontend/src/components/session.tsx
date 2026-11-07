"use client";
import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { api } from "@/lib/api";
import type { Member } from "@/lib/types";

const Session = createContext<{ member: Member | null; loading: boolean; refresh: () => Promise<void> }>({ member: null, loading: true, refresh: async () => {} });
export function SessionProvider({ children }: { children: ReactNode }) {
  const [member, setMember] = useState<Member | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  async function refresh() {
    setError("");
    try { setMember(await api<Member | null>("account")); }
    catch (err) { setError(err instanceof Error ? err.message : "無法讀取登入狀態。"); throw err; }
    finally { setLoading(false); }
  }
  useEffect(() => { refresh().catch(() => {}); }, []);
  return <Session value={{ member, loading, refresh }}>{error && <div className="notice error" role="alert">無法確認登入狀態：{error} <button onClick={() => refresh().catch(() => {})}>重試</button></div>}{children}</Session>;
}
export const useSession = () => useContext(Session);
