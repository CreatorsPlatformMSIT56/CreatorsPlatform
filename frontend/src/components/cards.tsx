import Link from "next/link";
import type { Card } from "@/lib/types";
import { Picture } from "./ui";

export function Cards({ items, kind = "works" }: { items: Card[]; kind?: "works" | "commissions" }) {
  if (!items.length) return <p className="notice">目前沒有符合條件的內容。</p>;
  return <div className="grid">{items.map(item => <article className="card" key={item.id}>
    <Link href={`/${kind}/${item.id}`}><Picture src={item.image} alt={item.title} /><h3>{item.title}</h3></Link>
    <Link className="muted" href={`/creators/${item.creatorId}`}>{item.creatorName}</Link>
    <p className="meta">{kind === "works" ? `♡ ${item.likes ?? 0}　${item.subtitle ?? ""}` : `NT$ ${item.priceMin?.toLocaleString()}${item.priceMax ? ` – ${item.priceMax.toLocaleString()}` : " 起"}`}</p>
  </article>)}</div>;
}
