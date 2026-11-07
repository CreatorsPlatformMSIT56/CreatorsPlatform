"use client";
import { useEffect, useRef } from "react";
import { cleanHtml, parseDelta } from "@/lib/content";

export function RichText({ html }: { html: string | null | undefined }) {
  const ref = useRef<HTMLDivElement>(null);
  const value = html ?? "", delta = parseDelta(value);
  useEffect(() => {
    const node = ref.current;
    if (!node || !parseDelta(value)) return;
    let active = true;
    import("quill").then(({ default: Quill }) => {
      if (!active) return;
      const viewer = new Quill(node, { readOnly: true, modules: { toolbar: false } });
      viewer.setContents(parseDelta(value));
    });
    return () => { active = false; node.replaceChildren(); };
  }, [value]);
  return delta ? <div className="rich-text" ref={ref} /> : <div className="rich-text" dangerouslySetInnerHTML={{ __html: cleanHtml(value) }} />;
}
