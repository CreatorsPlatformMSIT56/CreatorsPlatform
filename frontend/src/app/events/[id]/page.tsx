"use client";
import { use, useState } from "react";
import Link from "next/link";
import { api, readImage } from "@/lib/api";
import { eventColors } from "@/lib/content";
import { useResource } from "@/lib/use-resource";
import type { EventDetail } from "@/lib/types";
import { AsyncForm, Field, Picture, Status, text } from "@/components/ui";
import { RichText } from "@/components/rich-text";
import { useSession } from "@/components/session";
export default function EventPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params), [sort, setSort] = useState("new");
  const resource = useResource<EventDetail>(`events/${id}?sort=${sort}`), e = resource.data?.item;
  const { member } = useSession();
  const [titleColor, introColor] = eventColors(e?.eventStyle);
  return <section><Status {...resource} retry={resource.reload} />{e && <><Picture src={e.banner} alt={e.name} className="event-banner" /><div className="event-title" style={{ backgroundColor: titleColor }}><h1>{e.name}</h1><Link href={`/creators/${e.creatorId}`}>{e.creatorName}</Link><p>{e.startDate.replace("T", " ")} — {e.endDate.replace("T", " ")}</p></div>
    <div className="panel" style={{ backgroundColor: introColor }}><RichText html={e.description} /></div>{member?.creatorId === e.creatorId && <p><Link className="button" href={`/events/${id}/edit`}>編輯活動</Link></p>}
    <h2>活動範例</h2><div className="gallery">{resource.data?.images.filter(i => i.sample).map(i => <Picture key={i.id} src={i.image} alt={i.title || "活動範例"} />)}</div>
    <div className="section-heading"><h2>參加者投稿</h2><select aria-label="投稿排序" value={sort} onChange={e => setSort(e.target.value)}><option value="new">最新投稿</option><option value="popular">最多喜歡</option><option value="old">最早投稿</option></select></div>
    <div className="grid">{resource.data?.images.filter(i => !i.sample).map(i => <article className="card" key={i.id}><Picture src={i.image} alt={i.title} /><h3>{i.title}</h3><Link href={`/creators/${i.creatorId}`}>{i.creatorName}</Link><RichText html={i.description} /><AsyncForm className="inline-form" label={`♡ ${i.likes ?? 0}`} action={async () => { await api(`events/entries/${i.id}/like`, "POST"); resource.reload(); }} /></article>)}</div>
    <h2>投稿作品</h2>{member ? <AsyncForm label="送出投稿" action={async data => { await api(`events/${id}/entries`, "POST", { title: text(data, "title"), description: text(data, "description"), image: await readImage(data.get("image") as File) }); resource.reload(); }}><Field name="title" label="投稿標題" maxLength={100} /><Field name="description" label="作品介紹" type="textarea" /><label>投稿圖片<input name="image" type="file" required accept="image/png,image/jpeg,image/gif,image/webp" /></label></AsyncForm> : <p className="notice">請先<Link href="/login">登入</Link>再投稿。</p>}</>}</section>;
}
