"use client";
import { use } from "react";
import Link from "next/link";
import { api } from "@/lib/api";
import { useResource } from "@/lib/use-resource";
import type { Commission } from "@/lib/types";
import { AsyncForm, Field, Picture, Status, number, text } from "@/components/ui";
import { RichText } from "@/components/rich-text";
import { useSession } from "@/components/session";

export default function CommissionPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params), resource = useResource<Commission>(`catalog/commissions/${id}`), c = resource.data;
  const { member } = useSession();
  return <section className="detail"><Status {...resource} retry={resource.reload} />{c && <><Link href={`/creators/${c.creatorId}`}>← {c.creatorName}</Link><h1>{c.title}</h1>
    <div className="gallery">{c.images.map((src, i) => <Picture key={i} src={src} alt={`${c.title} 範例 ${i + 1}`} />)}</div><p className="price">NT$ {c.priceMin?.toLocaleString()} {c.priceMax ? `– ${c.priceMax.toLocaleString()}` : "起"}</p>
    <RichText html={c.description} /><p>截止日期：{c.overDate ?? "長期開放"}</p><h2>提出委託需求</h2>
    {member ? <AsyncForm label="送出委託" action={async data => { await api("studio/orders", "POST", { commissionId: Number(id), price: number(data, "price"), description: text(data, "description") }); }}>
      <Field name="price" label="預算（NT$）" type="number" min={0} value={c.priceMin} /><Field name="description" label="委託內容與需求" type="textarea" /></AsyncForm> : <p className="notice">請先<Link href="/login">登入</Link>再提出委託。</p>}</>}</section>;
}
