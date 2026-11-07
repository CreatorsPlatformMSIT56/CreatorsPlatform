import { after, before, test } from "node:test";
import assert from "node:assert/strict";
import { createServer } from "node:http";
import { spawn } from "node:child_process";
import { once } from "node:events";

let backend, frontend, origin;
const requests = [];
let likes = 3;
before(async () => {
  backend = createServer((req, res) => {
    requests.push({ path: req.url, cookie: req.headers.cookie ?? "" });
    res.setHeader("Content-Type", "application/json");
    if (req.url === "/api/catalog/works/404") { res.writeHead(404).end("{}"); return; }
    if (req.url === "/api/catalog/works/500") { res.writeHead(503).end("{}"); return; }
    if (req.url === "/api/catalog/works/77") {
      res.end(JSON.stringify({ id: 77, title: "SSR 測試作品", creatorId: 1, creatorName: "測試作者", image: "/images/content/77", likes,
        description: JSON.stringify({ ops: [{ insert: "不執行 JavaScript 也能閱讀的介紹", attributes: { bold: true } }, { insert: "\n" }] }),
        tags: [{ id: 1, name: "原創" }], comments: [{ id: 1, name: "讀者", text: "伺服器留言", avatar: null }] }));
      return;
    }
    res.writeHead(404).end("{}");
  });
  backend.listen(0, "127.0.0.1"); await once(backend, "listening");
  const reservation = createServer(); reservation.listen(0, "127.0.0.1"); await once(reservation, "listening");
  const port = reservation.address().port;
  await new Promise(resolve => reservation.close(resolve));
  origin = `http://127.0.0.1:${port}`;
  frontend = spawn(process.execPath, ["node_modules/next/dist/bin/next", "start", "--hostname", "127.0.0.1", "--port", String(port)], {
    env: { ...process.env, BACKEND_URL: `http://127.0.0.1:${backend.address().port}`, NEXT_TELEMETRY_DISABLED: "1" },
    stdio: ["ignore", "pipe", "pipe"],
  });
  let output = "";
  frontend.stdout.on("data", chunk => { output += chunk; });
  frontend.stderr.on("data", chunk => { output += chunk; });
  for (let attempt = 0; attempt < 100; attempt++) {
    try { if ((await fetch(`${origin}/privacy`)).ok) return; } catch { /* startup */ }
    if (frontend.exitCode !== null) throw new Error(output);
    await new Promise(resolve => setTimeout(resolve, 100));
  }
  throw new Error(`Next.js failed to start. Run npm run build before this suite. ${output}`);
}, { timeout: 20000 });

after(async () => {
  if (frontend && frontend.exitCode === null) { frontend.kill(); await once(frontend, "exit"); }
  if (backend) { backend.closeAllConnections(); await new Promise(resolve => backend.close(resolve)); }
});

test("work HTML contains its actual content and metadata without executing JavaScript", async () => {
  const response = await fetch(`${origin}/works/77`);
  assert.equal(response.status, 200);
  const html = await response.text();
  // Ignore the RSC/flight payload: assertions must match visible HTML elements.
  const document = html.replace(/<script\b[^>]*>[\s\S]*?<\/script>/gi, "");
  assert.match(document, /<h1>SSR 測試作品<\/h1>/);
  assert.match(document, /<strong>不執行 JavaScript 也能閱讀的介紹<\/strong>/);
  assert.match(document, /<p>伺服器留言<\/p>/);
  assert.match(document, /<title>SSR 測試作品 \| IMAGINK<\/title>/);
  assert.match(document, /<meta name="description" content="測試作者/);
});

test("SSR forwards only this request's session and does not share cached data", async () => {
  requests.length = 0;
  for (const value of ["alice", "bob"]) {
    const response = await fetch(`${origin}/works/77`, { headers: { Cookie: `Imagink.Session=${value}; unrelated=private` } });
    assert.equal(response.status, 200); await response.text();
  }
  const calls = requests.filter(r => r.path === "/api/catalog/works/77");
  assert.deepEqual(calls.map(c => c.cookie), ["Imagink.Session=alice", "Imagink.Session=bob"]);
  likes = 4;
  const html = await (await fetch(`${origin}/works/77`)).text();
  assert.match(html, /喜歡（4）/);
});

test("missing work returns HTTP 404 instead of an empty success page", async () => {
  assert.equal((await fetch(`${origin}/works/404`)).status, 404);
});
test("backend errors do not masquerade as missing content", async () => {
  assert.equal((await fetch(`${origin}/works/500`)).status, 500);
});
