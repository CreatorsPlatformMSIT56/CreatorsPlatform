import { cache } from "react";
import type { Metadata } from "next";
import Link from "next/link";
import { serverApi } from "@/lib/server-api";
import type { Work } from "@/lib/types";
import { Picture } from "@/components/ui";
import { RichText } from "@/components/rich-text";
import { WorkActions } from "@/components/work-actions";

const getWork = cache((id: string) =>
  serverApi<Work>(`catalog/works/${encodeURIComponent(id)}`),
);
type Props = { params: Promise<{ id: string }> };
/** 依目前頁面資料產生中繼資料。 */
export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const w = await getWork((await params).id);
  return {
    title: w.title,
    description: `${w.creatorName} 的作品「${w.title}」。瀏覽作品介紹、創作內容與讀者留言。`,
  };
}
/** 呈現作品詳細資料。 */
export default async function WorkPage({ params }: Props) {
  const w = await getWork((await params).id);
  return (
    <section className="detail">
      <Link href={`/creators/${w.creatorId}`}>← {w.creatorName}</Link>
      <h1>{w.title}</h1>
      <Picture src={w.image} alt={w.title} className="artwork" />
      <RichText html={w.description} />
      <div className="tags">
        {w.tags.map((t) => (
          <Link key={t.id} href={`/explore?tag=${t.id}`}>
            #{t.name}
          </Link>
        ))}
      </div>
      <WorkActions id={w.id} likes={w.likes ?? 0} />
      {!w.comments.length && (
        <p className="muted">還沒有留言，來留下第一則吧。</p>
      )}
      {w.comments.map((c) => (
        <article key={c.id} className="comment">
          <Picture src={c.avatar} alt={c.name} className="avatar" />
          <div>
            <strong>{c.name}</strong>
            <p>{c.text}</p>
          </div>
        </article>
      ))}
    </section>
  );
}
