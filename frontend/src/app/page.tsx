"use client";
import Link from "next/link";
import { useResource } from "@/lib/use-resource";
import type { Creator, PageData } from "@/lib/types";
import { Picture, Status } from "@/components/ui";
import { Cards } from "@/components/cards";

export default function Home() {
  const works = useResource<PageData>("catalog/works?sort=popular");
  const creators = useResource<Creator[]>("catalog/creators");
  return <><section className="hero"><div><p className="eyebrow">IMAGINATION INTO INK</p><h1>讓你的想像，<br />遇見懂它的人。</h1>
    <p>發現喜愛的作品、支持獨特的創作，<br />在這裡，和創作者一起實現每一個靈感。</p><div className="actions"><Link className="button primary" href="/explore">探索作品 ↗</Link><Link className="button" href="/explore?kind=commissions">尋找創作委託</Link></div></div>
    <div className="hero-art" aria-hidden="true"><span>✦</span><p>CREATE<br /><b>YOUR</b><br />WORLD.</p><i>想像不設限</i></div></section>
    <section><div className="section-heading"><div><p className="eyebrow">MEET THE CREATORS</p><h2>發現創作者</h2></div></div>
      <Status {...creators} retry={creators.reload} /><div className="creators">{creators.data?.map(c => <Link className="creator-tile" href={`/creators/${c.id}`} key={c.id}>
        <Picture src={c.avatar} alt={c.name} className="avatar" /><strong>{c.name}</strong><small>{c.followers} 人追蹤</small></Link>)}</div></section>
    <section><div className="section-heading"><div><p className="eyebrow">MADE TO INSPIRE</p><h2>人氣作品</h2></div><Link href="/explore">瀏覽所有作品 →</Link></div>
      <Status {...works} retry={works.reload} />{works.data && <Cards items={works.data.items} />}</section></>;
}
