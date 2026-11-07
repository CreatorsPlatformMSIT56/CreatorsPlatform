import { renderContent } from "@/lib/content";
import "./rich-text.css";

/** 呈現經過清理的富文字內容。 */
export function RichText({ html }: { html: string | null | undefined }) {
  return (
    <div
      className="rich-text"
      dangerouslySetInnerHTML={{ __html: renderContent(html) }}
    />
  );
}
