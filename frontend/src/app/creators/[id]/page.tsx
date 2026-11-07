import { cache } from "react";
import type { Metadata } from "next";
import Link from "next/link";
import { serverApi } from "@/lib/server-api";
import { first, pageNumber, type SearchParams } from "@/lib/catalog-query";
import type { Creator, PageData } from "@/lib/types";
import { Picture } from "@/components/ui";
import { Cards } from "@/components/cards";
import { RichText } from "@/components/rich-text";
import { CreatorFollow } from "@/components/creator-follow";

const getCreator = cache((id: string) =>
  serverApi<Creator>(`catalog/creators/${encodeURIComponent(id)}`),
);
type Props = {
  params: Promise<{ id: string }>;
  searchParams: Promise<SearchParams>;
};
/** 依目前頁面資料產生中繼資料。 */
export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const creator = await getCreator((await params).id);
  return {
    title: creator.name,
    description: `探索 ${creator.name} 的作品、委託與訂閱方案。`,
  };
}
/** 呈現創作者資料與作品。 */
export default async function CreatorPage({ params, searchParams }: Props) {
  const [{ id }, query] = await Promise.all([params, searchParams]);
  const kind = first(query.kind) === "commissions" ? "commissions" : "works",
    page = pageNumber(query.page);
  const [c, works] = await Promise.all([
    getCreator(id),
    serverApi<PageData>(
      `catalog/${kind}?creator=${encodeURIComponent(id)}&page=${page}`,
    ),
  ]);
  return (
    <section>
      {c.bannerUrl && (
        <Picture
          src={c.bannerUrl}
          alt={`${c.name} 封面`}
          className="event-banner"
        />
      )}
      <div className="profile-head">
        <Picture src={c.avatar} alt={c.name} className="avatar large" />
        <div>
          <p className="eyebrow">CREATOR</p>
          <h1>{c.name}</h1>
          <p>{c.followers} 人追蹤</p>
        </div>
        <CreatorFollow id={c.id} following={c.following} />
      </div>
      <RichText html={c.description} />
      <aside className="notice">
        <strong>創作者公告</strong>
        <RichText html={c.notice} />
      </aside>
      <h2>支持創作者</h2>
      <div className="grid plans">
        {c.plans.map((p) => (
          <article className="panel" key={p.id}>
            <h3>{p.name}</h3>
            <p className="price">
              NT$ {p.price.toLocaleString()} <small>/ 月</small>
            </p>
            <RichText html={p.description} />
            <Link className="button primary" href={`/subscribe/${p.id}`}>
              選擇方案
            </Link>
          </article>
        ))}
      </div>
      <div className="tabs">
        <Link
          aria-current={kind === "works" ? "page" : undefined}
          href="?kind=works"
        >
          作品
        </Link>
        <Link
          aria-current={kind === "commissions" ? "page" : undefined}
          href="?kind=commissions"
        >
          委託
        </Link>
      </div>
      <Cards items={works.items} kind={kind} />
      <nav className="pagination" aria-label="創作者內容分頁">
        {page > 1 && (
          <Link className="button" href={`?kind=${kind}&page=${page - 1}`}>
            上一頁
          </Link>
        )}
        <span>第 {page} 頁</span>
        {page * 12 < works.total && (
          <Link className="button" href={`?kind=${kind}&page=${page + 1}`}>
            下一頁
          </Link>
        )}
      </nav>
    </section>
  );
}
