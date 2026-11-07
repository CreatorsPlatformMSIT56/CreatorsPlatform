"use client";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { useResource } from "@/lib/use-resource";
import { pageNumber } from "@/lib/catalog-query";
import type { Lookups, PageData } from "@/lib/types";
import { Status } from "./ui";
import { Cards } from "./cards";
import { SearchFilters } from "./search-filters";
import { useSession } from "./session";

/** 呈現追蹤動態及分頁。 */
export function Feed() {
  const params = useSearchParams(),
    { member, loading } = useSession();
  const page = pageNumber(params.get("page") ?? undefined);
  const q = params.get("q") ?? "",
    subtitle = params.get("subtitle") ?? "",
    sort = params.get("sort") ?? "new";
  const query = new URLSearchParams({
    q,
    sort,
    page: String(page),
    following: "true",
  });
  for (const key of ["subtitle", "tag"])
    if (params.get(key)) query.set(key, params.get(key)!);
  const result = useResource<PageData>(
    member ? `catalog/works?${query}` : null,
  );
  const lookups = useResource<Lookups>("catalog/lookups");
  /** 建立指定頁碼的連結。 */
  function href(next: number) {
    const p = new URLSearchParams(params);
    p.set("page", String(next));
    return `?${p}`;
  }
  if (loading) return <Status loading />;
  if (!member)
    return (
      <p className="notice">
        請先<Link href="/login">登入</Link>，查看追蹤動態。
      </p>
    );
  return (
    <section>
      <p className="eyebrow">FIND YOUR INSPIRATION</p>
      <h1>追蹤動態</h1>
      <SearchFilters
        path="/feed"
        kind="works"
        q={q}
        subtitle={subtitle}
        sort={sort}
        tag={params.get("tag") ?? undefined}
        lookups={lookups.data}
      />
      <Status {...lookups} retry={lookups.reload} />
      <Status {...result} retry={result.reload} />
      {result.data && (
        <>
          <p className="muted">共 {result.data.total} 筆創作</p>
          <Cards items={result.data.items} />
          <nav className="pagination" aria-label="分頁">
            {page > 1 && (
              <Link className="button" href={href(page - 1)}>
                上一頁
              </Link>
            )}
            <span>
              第 {page} /{" "}
              {Math.max(1, Math.ceil(result.data.total / result.data.pageSize))}{" "}
              頁
            </span>
            {page * result.data.pageSize < result.data.total && (
              <Link className="button" href={href(page + 1)}>
                下一頁
              </Link>
            )}
          </nav>
        </>
      )}
    </section>
  );
}
