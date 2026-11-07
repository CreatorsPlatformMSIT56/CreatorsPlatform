"use client";
import { useCallback, useEffect, useState } from "react";
import { api, ApiError } from "./api";

/** 載入指定資源並提供載入狀態與重新載入功能。 */
export function useResource<T>(path: string | null) {
  const [state, setState] = useState<{
    data?: T;
    error?: ApiError;
    loading: boolean;
  }>({ loading: true });
  const [version, setVersion] = useState(0);
  const reload = useCallback(() => setVersion((v) => v + 1), []);
  useEffect(() => {
    if (path === null) {
      setState({ loading: false });
      return;
    }
    const controller = new AbortController();
    setState({ loading: true });
    api<T>(path, "GET", undefined, controller.signal)
      .then((data) => setState({ data, loading: false }))
      .catch((error) => {
        if (!controller.signal.aborted) setState({ error, loading: false });
      });
    return () => controller.abort();
  }, [path, version]);
  return { ...state, reload };
}
