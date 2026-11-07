import { Suspense } from "react";
import { Explore } from "@/components/explore";
export default function Page() { return <Suspense fallback={<p>載入中…</p>}><Explore following /></Suspense>; }
