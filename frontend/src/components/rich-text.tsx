import { renderContent } from "@/lib/content";
import "./rich-text.css";

export function RichText({ html }: { html: string | null | undefined }) {
  return <div className="rich-text" dangerouslySetInnerHTML={{ __html: renderContent(html) }} />;
}
