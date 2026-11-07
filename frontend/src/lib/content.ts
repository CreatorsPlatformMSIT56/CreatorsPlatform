import sanitizeHtml from "sanitize-html";

export function parseDelta(value: string) {
  try {
    const data = JSON.parse(value);
    return data && Array.isArray(data.ops) ? data : null;
  } catch { return null; }
}
export function cleanHtml(value: string) {
  return sanitizeHtml(value, {
    allowedTags: ["p", "br", "strong", "b", "em", "i", "u", "s", "h1", "h2", "h3", "h4", "ul", "ol", "li", "blockquote", "a", "img"],
    allowedAttributes: { a: ["href", "title"], img: ["src", "alt"] },
    allowedSchemes: ["https", "http", "mailto"],
  });
}
export function eventColors(style: string | null | undefined): [string, string] {
  try {
    const values = JSON.parse(style ?? "[]");
    return [values[0], values[1]].map((v, i) => /^#[a-f0-9]{6}$/i.test(v) ? v : i === 0 ? "#f5d547" : "#ffffff") as [string, string];
  } catch { return ["#f5d547", "#ffffff"]; }
}
