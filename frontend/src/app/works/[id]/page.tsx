"use client";
import { use } from "react";
import Link from "next/link";
import { api } from "@/lib/api";
import { useResource } from "@/lib/use-resource";
import type { Work } from "@/lib/types";
import { AsyncForm, Field, Picture, Status, text } from "@/components/ui";
import { RichText } from "@/components/rich-text";

export default function WorkPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params), resource = useResource<Work>(`catalog/works/${id}`), w = resource.data;
  return <section className="detail"><Status {...resource} retry={resource.reload} />{w && <><Link href={`/creators/${w.creatorId}`}>← {w.creatorName}</Link><h1>{w.title}</h1>
    <Picture src={w.image} alt={w.title} className="artwork" /><RichText html={w.description} /><div className="tags">{w.tags.map(t => <Link key={t.id} href={`/explore?tag=${t.id}`}>#{t.name}</Link>)}</div>
    <AsyncForm className="inline-form" label={`♡ 喜歡（${w.likes ?? 0}）`} action={async () => { await api(`studio/works/${id}/like`, "POST"); resource.reload(); }} />
    <h2>留言</h2><AsyncForm label="送出留言" action={async data => { await api(`studio/works/${id}/comments`, "POST", { text: text(data, "text") }); resource.reload(); }}><Field label="分享你的想法" name="text" type="textarea" maxLength={500} /></AsyncForm>
    {!w.comments.length && <p className="muted">還沒有留言，來留下第一則吧。</p>}{w.comments.map(c => <article key={c.id} className="comment"><Picture src={c.avatar} alt={c.name} className="avatar" /><div><strong>{c.name}</strong><p>{c.text}</p></div></article>)}</>}</section>;
}
