export class ApiError extends Error {
  /** 建立包含 HTTP 狀態碼與錯誤訊息的 API 錯誤。 */
  constructor(
    public status: number,
    message: string,
  ) {
    super(message);
  }
}

/** 呼叫 API 並處理回應錯誤。 */
export async function api<T>(
  path: string,
  method = "GET",
  body?: unknown,
  signal?: AbortSignal,
): Promise<T> {
  const response = await fetch(`/api/${path}`, {
    method,
    credentials: "same-origin",
    cache: "no-store",
    signal,
    ...(method !== "GET"
      ? {
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(body ?? {}),
        }
      : {}),
  });
  if (!response.ok) {
    const data = await response.json().catch(() => null);
    const message =
      data?.message ||
      (data?.errors ? Object.values(data.errors).flat().join(" ") : null) ||
      ({
        401: "請先登入。",
        403: "請先在會員中心設定創作者資料。",
        404: "找不到這筆資料。",
        409: "資料仍被使用，無法完成操作。",
      }[response.status] ??
        "服務暫時無法使用，請稍後重試。");
    throw new ApiError(response.status, message);
  }
  if (response.status === 204 || response.headers.get("content-length") === "0")
    return undefined as T;
  return response.json();
}

/** 讀取圖片檔案並轉換為資料網址或 Base64。 */
export async function readImage(file: File, raw = false): Promise<string> {
  if (
    !/^image\/(png|jpeg|gif|webp)$/.test(file.type) ||
    file.size > 5 * 1024 * 1024
  )
    throw new Error("請選擇 5 MB 以下的 PNG、JPEG、GIF 或 WebP 圖片。");
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () =>
      resolve(
        raw ? String(reader.result).split(",")[1] : String(reader.result),
      );
    reader.onerror = () => reject(new Error("無法讀取圖片。"));
    reader.readAsDataURL(file);
  });
}
