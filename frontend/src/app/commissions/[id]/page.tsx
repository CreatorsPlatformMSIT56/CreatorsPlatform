import { cache } from "react";
import type { Metadata } from "next";
import Link from "next/link";
import { serverApi } from "@/lib/server-api";
import type { Commission } from "@/lib/types";
import { Picture } from "@/components/ui";
import { RichText } from "@/components/rich-text";
import { CommissionOrder } from "@/components/commission-order";

const getCommission = cache((id: string) =>
  serverApi<Commission>(`catalog/commissions/${encodeURIComponent(id)}`),
);
type Props = { params: Promise<{ id: string }> };
/** 依目前頁面資料產生中繼資料。 */
export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const c = await getCommission((await params).id);
  return {
    title: c.title,
    description: `${c.creatorName} 的委託「${c.title}」，查看範例、價格與委託說明。`,
  };
}
/** 呈現委託項目詳細資料。 */
export default async function CommissionPage({ params }: Props) {
  const c = await getCommission((await params).id);
  return (
    <section className="detail">
      <Link href={`/creators/${c.creatorId}`}>← {c.creatorName}</Link>
      <h1>{c.title}</h1>
      <div className="gallery">
        {c.images.map((src, i) => (
          <Picture key={i} src={src} alt={`${c.title} 範例 ${i + 1}`} />
        ))}
      </div>
      <p className="price">
        NT$ {c.priceMin?.toLocaleString()}{" "}
        {c.priceMax ? `– ${c.priceMax.toLocaleString()}` : "起"}
      </p>
      <RichText html={c.description} />
      <p>截止日期：{c.overDate ?? "長期開放"}</p>
      <h2>提出委託需求</h2>
      <CommissionOrder id={c.id} price={c.priceMin ?? 0} />
    </section>
  );
}
