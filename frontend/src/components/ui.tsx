"use client";
import { useState, type FormEvent, type ReactNode } from "react";

/** 呈現載入或錯誤狀態。 */
export function Status({
  loading,
  error,
  retry,
}: {
  loading?: boolean;
  error?: Error;
  retry?: () => void;
}) {
  if (loading)
    return (
      <p role="status" className="notice">
        載入中…
      </p>
    );
  if (error)
    return (
      <div role="alert" className="notice error">
        {error.message} {retry && <button onClick={retry}>重試</button>}
      </div>
    );
  return null;
}
/** 處理非同步提交並呈現表單狀態。 */
export function AsyncForm({
  action,
  children,
  label = "儲存",
  className = "form",
  onSuccess,
}: {
  action: (data: FormData) => Promise<void>;
  children?: ReactNode;
  label?: string;
  className?: string;
  onSuccess?: () => void;
}) {
  const [busy, setBusy] = useState(false),
    [message, setMessage] = useState(""),
    [error, setError] = useState(false);
  /** 提交表單並處理結果。 */
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (busy) return;
    const data = new FormData(event.currentTarget);
    setBusy(true);
    setMessage("");
    setError(false);
    try {
      await action(data);
      setMessage("已完成。");
      onSuccess?.();
    } catch (err) {
      setMessage(err instanceof Error ? err.message : "操作失敗。");
      setError(true);
    } finally {
      setBusy(false);
    }
  }
  return (
    <form className={className} onSubmit={submit}>
      <fieldset disabled={busy}>
        {children}
        <button type="submit" className="primary">
          {busy ? "處理中…" : label}
        </button>
      </fieldset>
      {message && (
        <p
          role={error ? "alert" : "status"}
          className={error ? "error" : "success"}
        >
          {message}
        </p>
      )}
    </form>
  );
}
/** 呈現帶標籤的表單欄位。 */
export function Field({
  label,
  name,
  type = "text",
  value,
  required = true,
  min,
  maxLength,
}: {
  label: string;
  name: string;
  type?: string;
  value?: string | number;
  required?: boolean;
  min?: number;
  maxLength?: number;
}) {
  return (
    <label>
      {label}
      {type === "textarea" ? (
        <textarea
          name={name}
          defaultValue={value}
          required={required}
          maxLength={maxLength}
          rows={5}
        />
      ) : (
        <input
          name={name}
          type={type}
          defaultValue={value}
          required={required}
          min={min}
          maxLength={maxLength}
        />
      )}
    </label>
  );
}
/** 呈現圖片或缺圖提示。 */
export function Picture({
  src,
  alt,
  className = "",
}: {
  src?: string | null;
  alt: string;
  className?: string;
}) {
  const [failed, setFailed] = useState<string | null>(null);
  return src && src !== failed ? (
    <img
      src={src}
      alt={alt}
      className={className}
      loading="lazy"
      onError={() => setFailed(src)}
    />
  ) : (
    <div className={`placeholder ${className}`} role="img" aria-label={alt}>
      ✧
    </div>
  );
}
/** 從表單資料取得文字值。 */
export const text = (data: FormData, key: string) =>
  String(data.get(key) ?? "");
/** 從表單資料取得數值。 */
export const number = (data: FormData, key: string) => Number(data.get(key));
