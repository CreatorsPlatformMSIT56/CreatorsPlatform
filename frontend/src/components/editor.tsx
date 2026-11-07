"use client";
import { useEffect, useRef, useState } from "react";
import { parseDelta } from "@/lib/content";

/** 呈現富文字編輯器。 */
export function Editor({
  name = "description",
  value = "",
  label = "內容",
}: {
  name?: string;
  value?: string;
  label?: string;
}) {
  const container = useRef<HTMLDivElement>(null);
  const [content, setContent] = useState(value);
  useEffect(() => {
    const node = container.current;
    if (!node) return;
    let active = true;
    import("quill").then(({ default: Quill }) => {
      if (!active) return;
      const host = document.createElement("div");
      node.append(host);
      const editor = new Quill(host, {
        theme: "snow",
        modules: {
          toolbar: [
            [{ header: [2, 3, false] }],
            ["bold", "italic", "underline"],
            [{ list: "ordered" }, { list: "bullet" }],
            ["link", "clean"],
          ],
        },
      });
      editor.root.setAttribute("aria-label", label);
      const delta = parseDelta(value);
      if (delta) editor.setContents(delta);
      else editor.setText(value);
      editor.on("text-change", () =>
        setContent(
          editor.getText().trim() ? JSON.stringify(editor.getContents()) : "",
        ),
      );
    });
    return () => {
      active = false;
      node.replaceChildren();
    };
  }, [value, label]);
  return (
    <div className="editor">
      <p>{label}</p>
      <div ref={container} />
      <input type="hidden" name={name} value={content} />
    </div>
  );
}
