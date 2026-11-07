import { test } from "node:test";
import assert from "node:assert/strict";
import {
  cleanHtml,
  eventColors,
  parseDelta,
  renderContent,
} from "../src/lib/content.ts";

test("existing Quill Delta is recognized without mistaking ordinary text for JSON", () => {
  const delta = {
    ops: [{ insert: "創作", attributes: { bold: true } }, { insert: "\n" }],
  };
  assert.deepEqual(parseDelta(JSON.stringify(delta)), delta);
  assert.equal(parseDelta("一般文字"), null);
  assert.equal(parseDelta('{"title":"hello"}'), null);
});
test("rich HTML keeps presentation but removes scripts, event handlers and unsafe links", () => {
  const result = cleanHtml(
    '<p><strong>作品</strong><script>alert(1)</script><img src="x" onerror="alert(1)"><a href="javascript:alert(1)">link</a></p>',
  );
  assert.match(result, /<strong>作品<\/strong>/);
  assert.doesNotMatch(result, /script|onerror|javascript:/i);
});
test("stored event colors accept only hex and tolerate malformed data", () => {
  assert.deepEqual(eventColors('["#112233","#aabbcc"]'), [
    "#112233",
    "#aabbcc",
  ]);
  assert.deepEqual(eventColors('["url(evil)","red"]'), ["#f5d547", "#ffffff"]);
  assert.deepEqual(eventColors("bad JSON"), ["#f5d547", "#ffffff"]);
});
test("Delta renders content and formatting without a DOM or browser JavaScript", () => {
  const html = renderContent(
    JSON.stringify({
      ops: [
        { insert: "作品介紹", attributes: { bold: true } },
        { insert: "\n", attributes: { header: 2 } },
        { insert: "第一項" },
        { insert: "\n", attributes: { list: "bullet" } },
        { insert: { image: "data:image/png;base64,iVBORw0KGgo=" } },
        { insert: "\n" },
      ],
    }),
  );
  assert.match(html, /<h2><strong>作品介紹<\/strong><\/h2>/);
  assert.match(html, /<ul><li>第一項<\/li><\/ul>/);
  assert.match(html, /data:image\/png;base64/);
});
test("Delta text is escaped and dangerous formatting URLs are removed", () => {
  const html = renderContent(
    JSON.stringify({
      ops: [
        {
          insert: "<script>alert(1)</script>",
          attributes: { link: "javascript:alert(1)" },
        },
        { insert: { image: 'data:image/svg+xml,<svg onload="alert(1)"/>' } },
        { insert: "\n" },
      ],
    }),
  );
  assert.doesNotMatch(html, /<script|href="javascript:|src="data:image\/svg/i);
  assert.match(html, /&lt;script&gt;/);
});
test("Delta font, RTL indentation and supported video embeds retain their formatting", () => {
  const html = renderContent(
    JSON.stringify({
      ops: [
        { insert: "文字", attributes: { font: "serif" } },
        { insert: "\n", attributes: { direction: "rtl", indent: 2 } },
        { insert: { video: "https://www.youtube.com/embed/abc" } },
        { insert: { video: "https://untrusted.example/embed/abc" } },
      ],
    }),
  );
  assert.match(html, /font-family:serif/);
  assert.match(html, /padding-right:6em/);
  assert.match(html, /src="https:\/\/www.youtube.com\/embed\/abc"/);
  assert.doesNotMatch(html, /src="https:\/\/untrusted/);
});
