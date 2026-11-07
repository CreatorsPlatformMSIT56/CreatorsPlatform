"use client";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { useResource } from "@/lib/use-resource";
import type { Lookups, PageData } from "@/lib/types";
import { Cards, Status } from "./ui";
import { useSession } from "./session";

export function Explore({ following = false }: { following?: boolean }) {
  const params = useSearchParams();
  const { member, loading } = useSession();
  const kind = !following && params.get("kind") === "commissions" ? "commissions" : "works";
  const page = Math.max(1, Number(params.get("page")) || 1);
  const query = new URLSearchParams({ q: params.get("q") ?? "", sort: params.get("sort") ?? "new", page: String(page) });
  for (const key of ["subtitle", "tag"]) if (params.get(key)) query.set(key, params.get(key)!);
  if (following) query.set("following", "true");
  const result = useResource<PageData>(following && !member ? null : `catalog/${kind}?${query}`);
  const lookups = useResource<Lookups>("catalog/lookups");
  function href(next: number) { const p = new URLSearchParams(params); p.set("page", String(next)); return `?${p}`; }
  if (following && !loading && !member) return <p className="notice">請先<Link href="/login">登入</Link>，查看追蹤動態。</p>;
  return <section><p className="eyebrow">FIND YOUR INSPIRATION</p><h1>{following ? "追蹤動態" : "探索創作"}</h1>
    {!following && <div className="tabs"><Link aria-current={kind === "works" ? "page" : undefined} href="/explore">作品</Link><Link aria-current={kind === "commissions" ? "page" : undefined} href="/explore?kind=commissions">委託</Link></div>}
    <form className="filters" action={following ? "/feed" : "/explore"}>
      <input type="hidden" name="kind" value={kind} /><input name="q" aria-label="搜尋作品或創作者" placeholder="搜尋作品或創作者…" defaultValue={params.get("q") ?? ""} key={params.get("q")} />
      <select name="subtitle" aria-label="創作分類" defaultValue={params.get("subtitle") ?? ""} key={`sub-${params.get("subtitle")}`}><option value="">所有分類</option>{lookups.data?.subtitles.map(s => <option value={s.id} key={s.id}>{s.name}</option>)}</select>
      <select name="sort" aria-label="排序方式" defaultValue={params.get("sort") ?? "new"} key={`sort-${params.get("sort")}`}><option value="new">最新發布</option>{kind === "works" ? <><option value="popular">人氣優先</option><option value="old">最早發布</option></> : <option value="price">價格由低到高</option>}</select><button className="primary">搜尋</button>
    </form><Status {...result} retry={result.reload} />{result.data && <><p className="muted">共 {result.data.total} 筆創作</p><Cards kind={kind} items={result.data.items} />
      <nav className="pagination" aria-label="分頁">{page > 1 && <Link className="button" href={href(page - 1)}>上一頁</Link>}<span>第 {page} / {Math.max(1, Math.ceil(result.data.total / result.data.pageSize))} 頁</span>{page * result.data.pageSize < result.data.total && <Link className="button" href={href(page + 1)}>下一頁</Link>}</nav></>}</section>;
}
