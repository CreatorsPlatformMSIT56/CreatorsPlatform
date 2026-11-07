"use client";
import Link from "next/link";
import { api } from "@/lib/api";
import { useSession } from "./session";
import { AsyncForm, Field, number, text } from "./ui";

export function CommissionOrder({ id, price }: { id: number; price: number }) {
  const { member, loading } = useSession();
  if (loading) return <p role="status">確認登入狀態…</p>;
  return member ? <AsyncForm label="送出委託" action={async data => {
    await api("studio/orders", "POST", { commissionId: id, price: number(data, "price"), description: text(data, "description") });
  }}><Field name="price" label="預算（NT$）" type="number" min={0} value={price} /><Field name="description" label="委託內容與需求" type="textarea" /></AsyncForm>
    : <p className="notice">請先<Link href="/login">登入</Link>再提出委託。</p>;
}
