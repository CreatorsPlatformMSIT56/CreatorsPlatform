import Form from "next/form";
import type { Lookups } from "@/lib/types";

/** 呈現搜尋及篩選條件。 */
export function SearchFilters({
  path,
  kind,
  q,
  subtitle,
  sort,
  tag,
  lookups,
}: {
  path: "/explore" | "/feed";
  kind: "works" | "commissions";
  q: string;
  subtitle: string;
  sort: string;
  tag?: string;
  lookups?: Lookups;
}) {
  return (
    <Form className="filters" action={path}>
      <input type="hidden" name="kind" value={kind} />
      {tag && <input type="hidden" name="tag" value={tag} />}
      <input
        name="q"
        aria-label="搜尋作品或創作者"
        placeholder="搜尋作品或創作者…"
        defaultValue={q}
        key={q}
      />
      <select
        name="subtitle"
        aria-label="創作分類"
        defaultValue={subtitle}
        key={`sub-${subtitle}`}
      >
        <option value="">所有分類</option>
        {lookups?.subtitles.map((s) => (
          <option value={s.id} key={s.id}>
            {s.name}
          </option>
        ))}
      </select>
      <select
        name="sort"
        aria-label="排序方式"
        defaultValue={sort}
        key={`sort-${sort}`}
      >
        <option value="new">最新發布</option>
        {kind === "works" ? (
          <>
            <option value="popular">人氣優先</option>
            <option value="old">最早發布</option>
          </>
        ) : (
          <option value="price">價格由低到高</option>
        )}
      </select>
      <button className="primary">搜尋</button>
    </Form>
  );
}
