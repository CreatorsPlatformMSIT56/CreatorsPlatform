import { Suspense } from "react";
import { Feed } from "@/components/feed";
export default function Page() { return <Suspense fallback={<p>載入中…</p>}><Feed /></Suspense>; }
