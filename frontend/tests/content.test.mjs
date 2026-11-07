import { test } from "node:test";
import assert from "node:assert/strict";
import { cleanHtml, eventColors, parseDelta } from "../src/lib/content.ts";

test("existing Quill Delta is recognized without mistaking ordinary text for JSON", () => {
  const delta = { ops: [{ insert: "創作", attributes: { bold: true } }, { insert: "\n" }] };
  assert.deepEqual(parseDelta(JSON.stringify(delta)), delta);
  assert.equal(parseDelta("一般文字"), null);
  assert.equal(parseDelta('{"title":"hello"}'), null);
});
test("rich HTML keeps presentation but removes scripts, event handlers and unsafe links", () => {
  const result = cleanHtml('<p><strong>作品</strong><script>alert(1)</script><img src="x" onerror="alert(1)"><a href="javascript:alert(1)">link</a></p>');
  assert.match(result, /<strong>作品<\/strong>/);
  assert.doesNotMatch(result, /script|onerror|javascript:/i);
});
test("stored event colors accept only hex and tolerate malformed data", () => {
  assert.deepEqual(eventColors('["#112233","#aabbcc"]'), ["#112233", "#aabbcc"]);
  assert.deepEqual(eventColors('["url(evil)","red"]'), ["#f5d547", "#ffffff"]);
  assert.deepEqual(eventColors("bad JSON"), ["#f5d547", "#ffffff"]);
});
