import Link from "next/link";
import type { Metadata } from "next";
import { serverApi } from "@/lib/server-api";
import { first, pageNumber, type SearchParams } from "@/lib/catalog-query";
import type { Lookups, PageData } from "@/lib/types";
import { Cards } from "@/components/cards";
import { SearchFilters } from "@/components/search-filters";

type Props = { searchParams: Promise<SearchParams> };
export async function generateMetadata({ searchParams }: Props): Promise<Metadata> {
  const q = first((await searchParams).q);
  return { title: q ? `搜尋「${q}」` : "探索創作", description: "探索作品與創作委託，尋找喜愛的創作者。" };
}
export default async function ExplorePage({ searchParams }: Props) {
  const params = await searchParams, kind = first(params.kind) === "commissions" ? "commissions" : "works";
  const page = pageNumber(params.page), q = first(params.q), subtitle = first(params.subtitle), tag = first(params.tag);
  const sortValue = first(params.sort), sort = (kind === "works" ? ["new", "popular", "old"] : ["new", "price"]).includes(sortValue) ? sortValue : "new";
  const query = new URLSearchParams({ q, sort, page: String(page) });
  for (const [key, value] of [["subtitle", subtitle], ["tag", tag]]) if (/^[1-9]\d*$/.test(value)) query.set(key, value);
  const [result, lookups] = await Promise.all([serverApi<PageData>(`catalog/${kind}?${query}`), serverApi<Lookups>("catalog/lookups")]);
  function href(next: number) { const p = new URLSearchParams(query); p.set("kind", kind); p.set("page", String(next)); return `?${p}`; }
  return <section><p className="eyebrow">FIND YOUR INSPIRATION</p><h1>探索創作</h1>
    <div className="tabs"><Link aria-current={kind === "works" ? "page" : undefined} href="/explore">作品</Link><Link aria-current={kind === "commissions" ? "page" : undefined} href="/explore?kind=commissions">委託</Link></div>
    <SearchFilters path="/explore" kind={kind} q={q} subtitle={subtitle} sort={sort} tag={tag} lookups={lookups} />
    <p className="muted">共 {result.total} 筆創作</p><Cards kind={kind} items={result.items} />
    <nav className="pagination" aria-label="分頁">{page > 1 && <Link className="button" href={href(page - 1)}>上一頁</Link>}<span>第 {page} / {Math.max(1, Math.ceil(result.total / result.pageSize))} 頁</span>{page * result.pageSize < result.total && <Link className="button" href={href(page + 1)}>下一頁</Link>}</nav></section>;
}
