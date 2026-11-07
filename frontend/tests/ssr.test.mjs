import { after, before, test } from "node:test";
import assert from "node:assert/strict";
import { createServer } from "node:http";
import { spawn } from "node:child_process";
import { once } from "node:events";

let backend, frontend, origin;
const requests = [];
let likes = 3;
before(
  async () => {
    backend = createServer((req, res) => {
      requests.push({ path: req.url, cookie: req.headers.cookie ?? "" });
      res.setHeader("Content-Type", "application/json");
      if (req.url === "/api/catalog/works/404") {
        res.writeHead(404).end("{}");
        return;
      }
      if (req.url === "/api/catalog/works/500") {
        res.writeHead(503).end("{}");
        return;
      }
      if (req.url === "/api/catalog/works/77") {
        res.end(
          JSON.stringify({
            id: 77,
            title: "SSR 測試作品",
            creatorId: 1,
            creatorName: "測試作者",
            image: "/images/content/77",
            likes,
            description: JSON.stringify({
              ops: [
                {
                  insert: "不執行 JavaScript 也能閱讀的介紹",
                  attributes: { bold: true },
                },
                { insert: "\n" },
              ],
            }),
            tags: [{ id: 1, name: "原創" }],
            comments: [
              { id: 1, name: "讀者", text: "伺服器留言", avatar: null },
            ],
          }),
        );
        return;
      }
      const url = new URL(req.url, "http://fixture");
      if (url.pathname === "/api/catalog/creators/1") {
        res.end(
          JSON.stringify({
            id: 1,
            name: "SSR 創作者",
            description: '{"ops":[{"insert":"創作者簡介\\n"}]}',
            notice: "創作者公告",
            avatar: null,
            bannerUrl: null,
            followers: 2,
            following: req.headers.cookie === "Imagink.Session=alice",
            plans: [
              {
                id: 1,
                name: "支持方案",
                price: 100,
                level: 0,
                description: "方案內容",
              },
            ],
          }),
        );
        return;
      }
      if (
        url.pathname === "/api/catalog/works" ||
        url.pathname === "/api/catalog/commissions"
      ) {
        const page = Number(url.searchParams.get("page") ?? 1);
        res.end(
          JSON.stringify({
            total: 30,
            page,
            pageSize: 12,
            items: [
              {
                id: 77,
                title: `搜尋結果第${page}頁`,
                creatorId: 1,
                creatorName: "測試作者",
                image: null,
                likes: 3,
                priceMin: 100,
              },
            ],
          }),
        );
        return;
      }
      if (url.pathname === "/api/catalog/lookups") {
        res.end(
          JSON.stringify({
            categories: [{ id: 1, name: "繪畫" }],
            subtitles: [{ id: 1, name: "原創", categoryId: 1 }],
          }),
        );
        return;
      }
      if (url.pathname === "/api/catalog/commissions/9") {
        res.end(
          JSON.stringify({
            id: 9,
            title: "SSR 委託",
            creatorId: 1,
            creatorName: "測試作者",
            description: "委託完整介紹",
            priceMin: 100,
            priceMax: 200,
            overDate: null,
            images: [],
          }),
        );
        return;
      }
      if (url.pathname === "/api/events/1") {
        res.end(
          JSON.stringify({
            item: {
              id: 1,
              name: "SSR 活動",
              creatorId: 1,
              creatorName: "主辦者",
              description: "活動完整介紹",
              startDate: "2026-11-01T00:00:00",
              endDate: "2026-12-01T00:00:00",
              eventStyle: '["#112233","#ffffff"]',
              banner: null,
            },
            images: [
              {
                id: 1,
                title:
                  url.searchParams.get("sort") === "popular"
                    ? "熱門投稿"
                    : "最新投稿",
                description: "投稿完整介紹",
                sample: false,
                image: null,
                likes: 5,
                creatorId: 1,
                creatorName: "投稿者",
              },
            ],
          }),
        );
        return;
      }
      res.writeHead(404).end("{}");
    });
    backend.listen(0, "127.0.0.1");
    await once(backend, "listening");
    const reservation = createServer();
    reservation.listen(0, "127.0.0.1");
    await once(reservation, "listening");
    const port = reservation.address().port;
    await new Promise((resolve) => reservation.close(resolve));
    origin = `http://127.0.0.1:${port}`;
    frontend = spawn(
      process.execPath,
      [
        "node_modules/next/dist/bin/next",
        "start",
        "--hostname",
        "127.0.0.1",
        "--port",
        String(port),
      ],
      {
        env: {
          ...process.env,
          BACKEND_URL: `http://127.0.0.1:${backend.address().port}`,
          NEXT_TELEMETRY_DISABLED: "1",
        },
        stdio: ["ignore", "pipe", "pipe"],
      },
    );
    let output = "";
    frontend.stdout.on("data", (chunk) => {
      output += chunk;
    });
    frontend.stderr.on("data", (chunk) => {
      output += chunk;
    });
    for (let attempt = 0; attempt < 100; attempt++) {
      try {
        if ((await fetch(`${origin}/privacy`)).ok) return;
      } catch {
        /* startup */
      }
      if (frontend.exitCode !== null) throw new Error(output);
      await new Promise((resolve) => setTimeout(resolve, 100));
    }
    throw new Error(
      `Next.js failed to start. Run npm run build before this suite. ${output}`,
    );
  },
  { timeout: 20000 },
);

after(async () => {
  if (frontend && frontend.exitCode === null) {
    frontend.kill();
    await once(frontend, "exit");
  }
  if (backend) {
    backend.closeAllConnections();
    await new Promise((resolve) => backend.close(resolve));
  }
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
    const response = await fetch(`${origin}/works/77`, {
      headers: { Cookie: `Imagink.Session=${value}; unrelated=private` },
    });
    assert.equal(response.status, 200);
    await response.text();
  }
  const calls = requests.filter((r) => r.path === "/api/catalog/works/77");
  assert.deepEqual(
    calls.map((c) => c.cookie),
    ["Imagink.Session=alice", "Imagink.Session=bob"],
  );
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
const visibleHtml = (html) =>
  html.replace(/<script\b[^>]*>[\s\S]*?<\/script>/gi, "");
test("creator profile and initial works are SSR, while follow state remains request-specific", async () => {
  for (const [cookie, label] of [
    ["alice", "取消追蹤"],
    ["bob", "＋ 追蹤創作者"],
    [null, "＋ 追蹤創作者"],
  ]) {
    const response = await fetch(`${origin}/creators/1`, {
      headers: cookie ? { Cookie: `Imagink.Session=${cookie}` } : {},
    });
    assert.equal(response.status, 200);
    const html = visibleHtml(await response.text());
    assert.match(html, /<h1>SSR 創作者<\/h1>/);
    assert.match(html, /創作者簡介/);
    assert.match(html, /<h3>搜尋結果第1頁<\/h3>/);
    assert.match(html, /<h3>支持方案<\/h3>/);
    assert.ok(html.includes(label));
  }
});
test("explore SSR preserves search, category, sort and page in pagination links", async () => {
  requests.length = 0;
  const html = visibleHtml(
    await (
      await fetch(
        `${origin}/explore?q=海邊&subtitle=1&sort=popular&page=2&tag=3`,
      )
    ).text(),
  );
  assert.match(html, /<h3>搜尋結果第2頁<\/h3>/);
  assert.match(html, /<title>搜尋「海邊」 \| IMAGINK<\/title>/);
  const pagination = [...html.matchAll(/href="(\?[^\"]+)"/g)].map(
    (match) => new URL(match[1].replaceAll("&amp;", "&"), origin).searchParams,
  );
  assert.deepEqual(
    pagination.map((p) => p.get("page")),
    ["1", "3"],
  );
  for (const params of pagination) {
    assert.equal(params.get("q"), "海邊");
    assert.equal(params.get("subtitle"), "1");
    assert.equal(params.get("tag"), "3");
    assert.equal(params.get("kind"), "works");
    assert.equal(params.get("sort"), "popular");
  }
  const call = requests.find((r) => r.path.startsWith("/api/catalog/works?"));
  const query = new URL(call.path, "http://fixture").searchParams;
  assert.equal(query.get("q"), "海邊");
  assert.equal(query.get("page"), "2");
  assert.equal(query.get("sort"), "popular");
  assert.equal(query.get("subtitle"), "1");
  assert.equal(query.get("tag"), "3");
});
test("creator commission tab and event sorting affect server-rendered results", async () => {
  requests.length = 0;
  const creator = visibleHtml(
    await (await fetch(`${origin}/creators/1?kind=commissions&page=2`)).text(),
  );
  assert.match(creator, /href="\/commissions\/77"/);
  assert.ok(
    requests.some(
      (r) => r.path === "/api/catalog/commissions?creator=1&page=2",
    ),
  );
  const event = visibleHtml(
    await (await fetch(`${origin}/events/1?sort=popular`)).text(),
  );
  assert.match(event, /<h1>SSR 活動<\/h1>/);
  assert.match(event, /活動完整介紹/);
  assert.match(event, /<h3>熱門投稿<\/h3>/);
  assert.match(event, /background-color:#112233/);
  const commission = visibleHtml(
    await (await fetch(`${origin}/commissions/9`)).text(),
  );
  assert.match(commission, /<h1>SSR 委託<\/h1>/);
  assert.match(commission, /委託完整介紹/);
});
