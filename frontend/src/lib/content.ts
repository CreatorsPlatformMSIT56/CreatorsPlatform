import sanitizeHtml from "sanitize-html";
import { QuillDeltaToHtmlConverter } from "quill-delta-to-html";

/** 解析儲存的編輯器 Delta 資料。 */
export function parseDelta(value: string) {
  try {
    const data = JSON.parse(value);
    return data && Array.isArray(data.ops) ? data : null;
  } catch {
    return null;
  }
}
/** 清理 HTML，限制允許的標籤與屬性。 */
export function cleanHtml(value: string) {
  return sanitizeHtml(value, {
    allowedTags: [
      "p",
      "br",
      "strong",
      "b",
      "em",
      "i",
      "u",
      "s",
      "h1",
      "h2",
      "h3",
      "h4",
      "h5",
      "h6",
      "ul",
      "ol",
      "li",
      "blockquote",
      "a",
      "img",
      "span",
      "pre",
      "code",
      "sub",
      "sup",
      "iframe",
    ],
    allowedAttributes: {
      a: ["href", "title"],
      img: ["src", "alt"],
      iframe: ["src", "title", "allowfullscreen", "loading", "referrerpolicy"],
      "*": ["style"],
    },
    allowedStyles: {
      "*": {
        color: [/^#[a-f0-9]{3,8}$/i, /^rgb\([\d,\s]+\)$/i, /^[a-z]+$/i],
        "background-color": [
          /^#[a-f0-9]{3,8}$/i,
          /^rgb\([\d,\s]+\)$/i,
          /^[a-z]+$/i,
        ],
        "text-align": [/^(left|right|center|justify)$/],
        "font-size": [/^(0\.75|1\.5|2\.5)em$/],
        "font-family": [/^(serif|monospace)$/],
        "padding-left": [/^\d{1,2}em$/],
        direction: [/^(rtl|ltr)$/],
        "padding-right": [/^\d{1,2}em$/],
      },
    },
    allowedSchemes: ["https", "http", "mailto"],
    allowedSchemesByTag: { img: ["https", "http", "data"], iframe: ["https"] },
    allowedIframeHostnames: [
      "www.youtube.com",
      "www.youtube-nocookie.com",
      "player.vimeo.com",
    ],
    allowIframeRelativeUrls: false,
    transformTags: {
      img: (tagName, attribs) => ({
        tagName,
        attribs: {
          ...attribs,
          ...(attribs.src?.startsWith("data:") &&
          !/^data:image\/(png|jpeg|gif|webp);base64,/i.test(attribs.src)
            ? { src: "" }
            : {}),
        },
      }),
      iframe: (tagName, attribs) => ({
        tagName,
        attribs: {
          ...attribs,
          title: "嵌入影片",
          loading: "lazy",
          referrerpolicy: "strict-origin-when-cross-origin",
        },
      }),
    },
  });
}
/** 將儲存的內容轉換為可呈現的 HTML。 */
export function renderContent(value: string | null | undefined) {
  const source = value ?? "",
    delta = parseDelta(source);
  return cleanHtml(
    delta
      ? new QuillDeltaToHtmlConverter(delta.ops, {
          inlineStyles: {
            font: {
              serif: "font-family:serif",
              monospace: "font-family:monospace",
            },
          },
        }).convert()
      : source,
  );
}
/** 從活動樣式取得背景與文字顏色。 */
export function eventColors(
  style: string | null | undefined,
): [string, string] {
  try {
    const values = JSON.parse(style ?? "[]");
    return [values[0], values[1]].map((v, i) =>
      /^#[a-f0-9]{6}$/i.test(v) ? v : i === 0 ? "#f5d547" : "#ffffff",
    ) as [string, string];
  } catch {
    return ["#f5d547", "#ffffff"];
  }
}
