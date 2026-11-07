"use client";
import Link from "next/link";
import { useState } from "react";
import { useResource } from "@/lib/use-resource";
import type { EventItem } from "@/lib/types";
import { Picture, Status } from "@/components/ui";
export default function Events() {
  const resource = useResource<EventItem[]>("events"), [ended, setEnded] = useState(false);
  const items = resource.data?.filter(e => (new Date(e.endDate).getTime() < Date.now()) === ended);
  return <section><div className="section-heading"><div><p className="eyebrow">CREATE TOGETHER</p><h1>創作活動</h1></div><Link className="button primary" href="/events/new">＋ 建立活動</Link></div>
    <div className="tabs"><button aria-pressed={!ended} onClick={() => setEnded(false)}>進行中與即將開始</button><button aria-pressed={ended} onClick={() => setEnded(true)}>已結束</button></div>
    <Status {...resource} retry={resource.reload} />{items && !items.length && <p className="notice">目前沒有活動。</p>}
    <div className="grid">{items?.map(e => <article className="card" key={e.id}><Link href={`/events/${e.id}`}><Picture src={e.banner} alt={e.name} /><h3>{e.name}</h3></Link><p>{e.creatorName}</p><p className="muted">{e.startDate.slice(0, 10)} — {e.endDate.slice(0, 10)}</p></article>)}</div></section>;
}
