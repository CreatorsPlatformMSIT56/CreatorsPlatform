"use client";
import { use, useState } from "react";
import Link from "next/link";
import { api } from "@/lib/api";
import { useResource } from "@/lib/use-resource";
import type { Creator, PageData } from "@/lib/types";
import { AsyncForm, Cards, Picture, Status } from "@/components/ui";
import { RichText } from "@/components/rich-text";

export default function CreatorPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params), [kind, setKind] = useState<"works" | "commissions">("works"), [page, setPage] = useState(1);
  const profile = useResource<Creator>(`catalog/creators/${id}`);
  const works = useResource<PageData>(`catalog/${kind}?creator=${id}&page=${page}`);
  const c = profile.data;
  return <section><Status {...profile} retry={profile.reload} />{c && <>{c.bannerUrl && <Picture src={c.bannerUrl} alt={`${c.name} 封面`} className="event-banner" />}<div className="profile-head"><Picture src={c.avatar} alt={c.name} className="avatar large" />
    <div><p className="eyebrow">CREATOR</p><h1>{c.name}</h1><p>{c.followers} 人追蹤</p></div><AsyncForm className="inline-form" label={c.following ? "取消追蹤" : "＋ 追蹤創作者"} action={async () => { await api(`studio/creators/${id}/follow`, "POST"); profile.reload(); }} /></div>
    <RichText html={c.description} /><aside className="notice"><strong>創作者公告</strong><RichText html={c.notice} /></aside>
    <h2>支持創作者</h2><div className="grid plans">{c.plans.map(p => <article className="panel" key={p.id}><h3>{p.name}</h3><p className="price">NT$ {p.price.toLocaleString()} <small>/ 月</small></p><RichText html={p.description} /><Link className="button primary" href={`/subscribe/${p.id}`}>選擇方案</Link></article>)}</div>
    <div className="tabs"><button aria-pressed={kind === "works"} onClick={() => { setKind("works"); setPage(1); }}>作品</button><button aria-pressed={kind === "commissions"} onClick={() => { setKind("commissions"); setPage(1); }}>委託</button></div>
    <Status {...works} retry={works.reload} />{works.data && <><Cards items={works.data.items} kind={kind} /><div className="pagination"><button disabled={page <= 1} onClick={() => setPage(p => p - 1)}>上一頁</button><span>第 {page} 頁</span><button disabled={page * 12 >= works.data.total} onClick={() => setPage(p => p + 1)}>下一頁</button></div></>}</>}</section>;
}
