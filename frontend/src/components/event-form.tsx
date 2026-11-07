"use client";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { api, readImage } from "@/lib/api";
import { eventColors } from "@/lib/content";
import { useResource } from "@/lib/use-resource";
import type { EventDetail, Lookups } from "@/lib/types";
import { useSession } from "./session";
import { AsyncForm, Field, Picture, Status, number, text } from "./ui";
import { Editor } from "./editor";

export function EventForm({ id }: { id?: string }) {
  const { member, loading } = useSession(), router = useRouter();
  const lookups = useResource<Lookups>("catalog/lookups");
  const resource = useResource<EventDetail>(id ? `events/${id}` : null);
  if (loading) return <Status loading />;
  if (!member) return <p className="notice">請先<Link href="/login">登入</Link>。</p>;
  if (!member.creatorId) return <p className="notice">請先在<Link href="/account">會員中心</Link>設定創作者資料。</p>;
  if (id && !resource.data) return <Status {...resource} retry={resource.reload} />;
  const e = resource.data?.item;
  if (e && member.creatorId !== e.creatorId) return <p className="notice">只有活動主辦者可以編輯。</p>;
  const [titleColor, introColor] = eventColors(e?.eventStyle);
  return <section className="narrow"><h1>{id ? "編輯活動" : "建立活動"}</h1><Status {...lookups} retry={lookups.reload} /><AsyncForm label="儲存活動" action={async data => {
    const banner = data.get("banner") as File;
    const files = data.getAll("samples").filter((f): f is File => f instanceof File && f.size > 0);
    if (files.length > 10) throw new Error("最多 10 張範例圖片。");
    // Retain current samples unless the owner explicitly removes them.
    const retained = data.getAll("keepSample").map(Number);
    const result = await api<{ id: number }>(id ? `events/${id}` : "events", id ? "PUT" : "POST", {
      name: text(data, "name"), description: text(data, "description"), startDate: text(data, "startDate"), endDate: text(data, "endDate"), categoryId: number(data, "categoryId"),
      titleColor: text(data, "titleColor"), introColor: text(data, "introColor"), banner: banner.size ? await readImage(banner) : null,
      keepSamples: retained, samples: await Promise.all(files.map(f => readImage(f))),
    }); router.push(`/events/${result.id}`);
  }}><Field name="name" label="活動名稱" value={e?.name} maxLength={200} /><div className="row"><Field name="startDate" label="開始時間" type="datetime-local" value={e?.startDate.slice(0, 16)} /><Field name="endDate" label="結束時間" type="datetime-local" value={e?.endDate.slice(0, 16)} /></div>
    <label>活動分類<select name="categoryId" required defaultValue={e?.categoryId}>{lookups.data?.categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}</select></label>
    <div className="row"><Field label="標題背景色" name="titleColor" type="color" value={titleColor} /><Field label="介紹背景色" name="introColor" type="color" value={introColor} /></div>
    <Editor value={e?.description ?? ""} label="活動介紹" /><label>活動橫幅<input name="banner" type="file" required={!id} accept="image/png,image/jpeg,image/gif,image/webp" /></label>
    {resource.data?.images.filter(i => i.sample).map(i => <label className="sample-choice" key={i.id}><input type="checkbox" name="keepSample" value={i.id} defaultChecked /><Picture src={i.image} alt="活動範例" />保留此範例</label>)}
    <label>新增範例圖片（最多 10 張）<input name="samples" type="file" multiple accept="image/png,image/jpeg,image/gif,image/webp" /></label></AsyncForm></section>;
}
