"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { api, readImage } from "@/lib/api";
import { useResource } from "@/lib/use-resource";
import type { Dashboard, Lookups, Plan } from "@/lib/types";
import { AsyncForm, Field, Status, text, number } from "./ui";
import { Editor } from "./editor";

export function CreationForms({ kind, dashboard, done }: { kind: string; dashboard: Dashboard; done: () => void }) {
  const lookups = useResource<Lookups>("catalog/lookups");
  const router = useRouter();
  const [category, setCategory] = useState("");
  if (!lookups.data) return <Status {...lookups} retry={lookups.reload} />;
  const options = lookups.data;
  if (!dashboard.creator && kind !== "creator") return <p className="notice">請先在「創作者設定」填寫資料，開啟創作者功能。</p>;
  if (kind === "creator") return <AsyncForm action={async data => {
    await api("studio/creator", "PUT", { description: text(data, "description"), notice: text(data, "notice") }); done();
  }}><Field label="創作者簡介" name="description" type="textarea" value={dashboard.creator?.description ?? ""} /><Field label="創作者公告" name="notice" type="textarea" value={dashboard.creator?.notice ?? ""} /></AsyncForm>;
  if (kind === "plans") return <PlanForm done={done} plans={dashboard.plans} />;
  if (kind === "work" && !dashboard.plans.length) return <p className="notice">請先在「訂閱方案」建立方案，再發布作品。</p>;
  return <AsyncForm key={kind} label={kind === "work" ? "發布作品" : "發布委託"} action={async data => {
    const common = { title: text(data, "title"), description: text(data, "description"), subtitleId: number(data, "subtitleId") };
    if (!common.description) throw new Error("請填寫內容。");
    if (kind === "work") {
      const file = data.get("image") as File;
      const result = await api<{ id: number }>("studio/works", "POST", { ...common, categoryId: number(data, "categoryId"), planId: number(data, "planId"), image: await readImage(file, true) });
      router.push(`/works/${result.id}`);
    } else {
      const files = data.getAll("images").filter((f): f is File => f instanceof File && f.size > 0);
      if (files.length > 10) throw new Error("最多可上傳 10 張範例。");
      const result = await api<{ id: number }>("studio/commissions", "POST", { ...common, priceMin: number(data, "priceMin"), priceMax: text(data, "priceMax") ? number(data, "priceMax") : null, overDate: text(data, "overDate") || null, images: await Promise.all(files.map(f => readImage(f))) });
      router.push(`/commissions/${result.id}`);
    }
  }}><Field name="title" label={kind === "work" ? "作品標題" : "委託標題"} maxLength={kind === "work" ? 200 : 500} />
    {kind === "work" ? <><label>分類<select required name="categoryId" value={category} onChange={e => setCategory(e.target.value)}><option value="">選擇分類</option>{options.categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}</select></label>
      <label>子分類<select required name="subtitleId" key={category}><option value="">選擇子分類</option>{options.subtitles.filter(s => s.categoryId === Number(category)).map(s => <option key={s.id} value={s.id}>{s.name}</option>)}</select></label>
      <label>訂閱方案<select required name="planId">{dashboard.plans.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}</select></label><label>作品圖片<input type="file" name="image" required accept="image/png,image/jpeg,image/gif,image/webp" /></label></>
      : <><label>子分類<select required name="subtitleId">{options.subtitles.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}</select></label><div className="row"><Field name="priceMin" label="最低價格" type="number" min={0} /><Field name="priceMax" label="最高價格（選填）" type="number" min={0} required={false} /></div><Field name="overDate" label="截止日期（選填）" type="date" required={false} /><label>範例圖片（最多 10 張）<input type="file" name="images" multiple accept="image/png,image/jpeg,image/gif,image/webp" /></label></>}
    <Editor /></AsyncForm>;
}

function PlanForm({ plans, done }: { plans: Plan[]; done: () => void }) {
  const [selected, setSelected] = useState("");
  const plan = plans.find(p => p.id === Number(selected));
  return <><label>編輯方案<select value={selected} onChange={e => setSelected(e.target.value)}><option value="">建立新方案</option>{plans.map(p => <option value={p.id} key={p.id}>{p.name}</option>)}</select></label>
    <AsyncForm key={selected} action={async data => { await api("studio/plans", "PUT", { name: text(data, "name"), price: number(data, "price"), level: number(data, "level"), description: text(data, "description") }); done(); }}>
      <Field name="name" label="方案名稱" value={plan?.name} maxLength={100} /><Field name="price" label="每月價格（NT$）" type="number" min={0} value={plan?.price} /><Field name="level" label="方案等級（相同等級會更新原方案）" type="number" min={0} value={plan?.level ?? 0} /><Field name="description" label="方案說明" type="textarea" value={plan?.description} maxLength={300} /></AsyncForm></>;
}
