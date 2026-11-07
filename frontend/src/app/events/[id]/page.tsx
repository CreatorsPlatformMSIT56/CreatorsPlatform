import { cache } from "react";
import type { Metadata } from "next";
import Link from "next/link";
import Form from "next/form";
import { serverApi } from "@/lib/server-api";
import { first, type SearchParams } from "@/lib/catalog-query";
import { eventColors } from "@/lib/content";
import type { EventDetail } from "@/lib/types";
import { Picture } from "@/components/ui";
import { RichText } from "@/components/rich-text";
import { EventLike, EventParticipation } from "@/components/event-actions";

const getEvent = cache((id: string, sort: string) =>
  serverApi<EventDetail>(`events/${encodeURIComponent(id)}?sort=${sort}`),
);
type Props = {
  params: Promise<{ id: string }>;
  searchParams: Promise<SearchParams>;
};
/** 取得活動投稿的有效排序方式。 */
function eventSort(query: SearchParams) {
  const value = first(query.sort);
  return value === "popular" || value === "old" ? value : "new";
}
/** 依目前頁面資料產生中繼資料。 */
export async function generateMetadata({
  params,
  searchParams,
}: Props): Promise<Metadata> {
  const [{ id }, query] = await Promise.all([params, searchParams]);
  const { item } = await getEvent(id, eventSort(query));
  return {
    title: item.name,
    description: `${item.creatorName} 主辦的「${item.name}」，瀏覽活動介紹與參加者投稿。`,
  };
}
/** 呈現活動詳情與投稿。 */
export default async function EventPage({ params, searchParams }: Props) {
  const [{ id }, query] = await Promise.all([params, searchParams]),
    sort = eventSort(query);
  const { item: e, images } = await getEvent(id, sort);
  const [titleColor, introColor] = eventColors(e.eventStyle);
  return (
    <section>
      <Picture src={e.banner} alt={e.name} className="event-banner" />
      <div className="event-title" style={{ backgroundColor: titleColor }}>
        <h1>{e.name}</h1>
        <Link href={`/creators/${e.creatorId}`}>{e.creatorName}</Link>
        <p>
          {e.startDate.replace("T", " ")} — {e.endDate.replace("T", " ")}
        </p>
      </div>
      <div className="panel" style={{ backgroundColor: introColor }}>
        <RichText html={e.description} />
      </div>
      <h2>活動範例</h2>
      <div className="gallery">
        {images
          .filter((i) => i.sample)
          .map((i) => (
            <Picture key={i.id} src={i.image} alt={i.title || "活動範例"} />
          ))}
      </div>
      <div className="section-heading">
        <h2>參加者投稿</h2>
        <Form className="filters" action={`/events/${e.id}`}>
          <select
            name="sort"
            aria-label="投稿排序"
            defaultValue={sort}
            key={sort}
          >
            <option value="new">最新投稿</option>
            <option value="popular">最多喜歡</option>
            <option value="old">最早投稿</option>
          </select>
          <button>套用排序</button>
        </Form>
      </div>
      <div className="grid">
        {images
          .filter((i) => !i.sample)
          .map((i) => (
            <article className="card" key={i.id}>
              <Picture src={i.image} alt={i.title} />
              <h3>{i.title}</h3>
              <Link href={`/creators/${i.creatorId}`}>{i.creatorName}</Link>
              <RichText html={i.description} />
              <EventLike id={i.id} likes={i.likes ?? 0} />
            </article>
          ))}
      </div>
      <EventParticipation id={e.id} creatorId={e.creatorId} />
    </section>
  );
}
