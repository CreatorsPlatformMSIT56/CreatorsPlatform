import "server-only";
import { cookies } from "next/headers";
import { notFound, redirect } from "next/navigation";

// Fetch directly from C#, never through our own public HTTP entry point.
// No shared data cache: Creator responses include per-user follow state.
/** 在伺服器端帶入驗證 Cookie 呼叫 API。 */
export async function serverApi<T>(path: string): Promise<T> {
  const auth = (await cookies()).get("Imagink.Auth");
  const response = await fetch(
    `${process.env.BACKEND_URL || "http://localhost:5251"}/api/${path}`,
    {
      cache: "no-store",
      headers: auth ? { Cookie: `Imagink.Auth=${auth.value}` } : {},
      signal: AbortSignal.timeout(15000),
    },
  );
  if (response.status === 404) notFound();
  if (response.status === 401) redirect("/login");
  if (!response.ok) throw new Error("服務暫時無法使用，請稍後重試。");
  return response.json();
}
