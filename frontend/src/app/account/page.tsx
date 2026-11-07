"use client";
import Link from "next/link";
import { useState } from "react";
import { useSession } from "@/components/session";
import { api, readImage } from "@/lib/api";
import { useResource } from "@/lib/use-resource";
import type { Dashboard, Order } from "@/lib/types";
import {
  AsyncForm,
  Field,
  Picture,
  Status,
  number,
  text,
} from "@/components/ui";
import { CreationForms } from "@/components/creation-forms";

const tabs = [
  ["profile", "一般設定"],
  ["creator", "創作者設定"],
  ["plans", "訂閱方案"],
  ["work", "發布作品"],
  ["commission", "發布委託"],
  ["manage", "內容管理"],
  ["subscriptions", "訂閱紀錄"],
  ["orders", "委託訂單"],
];
/** 呈現帳戶與管理功能。 */
export default function Account() {
  const { member, loading, refresh } = useSession();
  const [tab, setTab] = useState("profile");
  const dashboard = useResource<Dashboard>(member ? "dashboard" : null);
  if (loading) return <Status loading />;
  if (!member)
    return (
      <p className="notice">
        請先<Link href="/login">登入</Link>，開啟會員中心。
      </p>
    );
  const d = dashboard.data;
  return (
    <section>
      <p className="eyebrow">YOUR CREATIVE SPACE</p>
      <h1>會員中心</h1>
      <div className="account-layout">
        <aside className="sidebar">
          <Picture
            className="avatar large"
            src={member.avatar}
            alt={member.name}
          />
          <h3>{member.name}</h3>
          {tabs.map(([key, name]) => (
            <button
              aria-pressed={tab === key}
              key={key}
              onClick={() => setTab(key)}
            >
              {name}
            </button>
          ))}
          <Link className="button" href="/events/new">
            建立活動 ↗
          </Link>
        </aside>
        <div className="panel">
          <h2>{tabs.find((t) => t[0] === tab)?.[1]}</h2>
          <Status {...dashboard} retry={dashboard.reload} />
          {d && (
            <>
              {tab === "profile" && (
                <>
                  <AsyncForm
                    action={async (data) => {
                      await api("account", "PUT", {
                        name: text(data, "name"),
                        email: text(data, "email"),
                      });
                      await refresh();
                    }}
                  >
                    <Field
                      name="name"
                      label="顯示名稱"
                      value={member.name}
                      maxLength={40}
                    />
                    <Field
                      name="email"
                      type="email"
                      label="電子郵件"
                      value={member.email}
                    />
                  </AsyncForm>
                  <h3>更新頭像</h3>
                  <AsyncForm
                    label="上傳頭像"
                    action={async (data) => {
                      await api("account/avatar", "PUT", {
                        data: await readImage(data.get("avatar") as File, true),
                      });
                      await refresh();
                    }}
                  >
                    <label>
                      頭像圖片
                      <input
                        name="avatar"
                        type="file"
                        required
                        accept="image/png,image/jpeg,image/gif,image/webp"
                      />
                    </label>
                  </AsyncForm>
                  <h3>變更密碼</h3>
                  <AsyncForm
                    action={async (data) => {
                      await api("account/password", "PUT", {
                        currentPassword: text(data, "current"),
                        password: text(data, "password"),
                      });
                    }}
                  >
                    <Field name="current" label="目前密碼" type="password" />
                    <Field
                      name="password"
                      label="新密碼（至少 8 個字元）"
                      type="password"
                      maxLength={100}
                    />
                  </AsyncForm>
                </>
              )}
              {["creator", "plans", "work", "commission"].includes(tab) && (
                <CreationForms
                  kind={tab}
                  dashboard={d}
                  done={() => {
                    dashboard.reload();
                    refresh().catch(() => {});
                  }}
                />
              )}
              {tab === "manage" && (
                <>
                  <h3>作品</h3>
                  {d.works.map((w) => (
                    <div className="manage-row" key={w.id}>
                      <Link href={`/works/${w.id}`}>{w.title}</Link>
                      <DeleteButton
                        kind="works"
                        id={w.id}
                        done={dashboard.reload}
                      />
                    </div>
                  ))}
                  <h3>委託</h3>
                  {d.commissions.map((c) => (
                    <p key={c.id}>
                      <Link href={`/commissions/${c.id}`}>{c.title}</Link>
                    </p>
                  ))}
                  <h3>活動</h3>
                  {d.events.map((e) => (
                    <div className="manage-row" key={e.id}>
                      <Link href={`/events/${e.id}`}>{e.name}</Link>
                      <Link href={`/events/${e.id}/edit`}>編輯</Link>
                      <DeleteButton
                        kind="events"
                        id={e.id}
                        done={dashboard.reload}
                      />
                    </div>
                  ))}
                  <h3>方案</h3>
                  {d.plans.map((p) => (
                    <div className="manage-row" key={p.id}>
                      <span>{p.name}</span>
                      <DeleteButton
                        kind="plans"
                        id={p.id}
                        done={dashboard.reload}
                      />
                    </div>
                  ))}
                </>
              )}
              {tab === "subscriptions" &&
                (d.subscriptions.length ? (
                  <div className="table-scroll">
                    <table>
                      <thead>
                        <tr>
                          <th>方案</th>
                          <th>開始</th>
                          <th>結束</th>
                        </tr>
                      </thead>
                      <tbody>
                        {d.subscriptions.map((s) => (
                          <tr key={s.id}>
                            <td>
                              <Link href={`/creators/${s.creatorId}`}>
                                {s.name}
                              </Link>
                            </td>
                            <td>{s.startDate}</td>
                            <td>{s.endDate}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                ) : (
                  <p className="notice">目前沒有訂閱紀錄。</p>
                ))}
              {tab === "orders" &&
                (d.orders.length ? (
                  d.orders.map((o) => (
                    <OrderCard key={o.id} order={o} done={dashboard.reload} />
                  ))
                ) : (
                  <p className="notice">目前沒有委託訂單。</p>
                ))}
            </>
          )}
        </div>
      </div>
    </section>
  );
}

/** 呈現刪除項目的操作按鈕。 */
function DeleteButton({
  kind,
  id,
  done,
}: {
  kind: string;
  id: number;
  done: () => void;
}) {
  return (
    <AsyncForm
      className="inline-form"
      label="移除"
      action={async () => {
        if (!window.confirm("確定移除這筆內容？")) return;
        await api(`studio/${kind}/${id}`, "DELETE");
        done();
      }}
    />
  );
}
/** 呈現委託訂單及其操作。 */
function OrderCard({ order: o, done }: { order: Order; done: () => void }) {
  const canReply = o.owner
    ? ["待確認中", "價格確認", "製作中"].includes(o.status)
    : o.status === "價格回覆";
  return (
    <article className="order">
      <div className="section-heading">
        <h3>{o.title}</h3>
        <span className="badge">{o.status}</span>
      </div>
      <p>
        {o.owner ? "收到的委託" : "我的委託"} · {o.orderDate}
      </p>
      <p>{o.description}</p>
      <p>NT$ {o.price.toLocaleString()}</p>
      {canReply && (
        <AsyncForm
          label="送出回覆"
          action={async (data) => {
            await api(`studio/orders/${o.id}`, "PUT", {
              action: text(data, "action"),
              price:
                o.owner && o.status === "待確認中"
                  ? number(data, "price")
                  : null,
            });
            done();
          }}
        >
          {o.owner && o.status === "待確認中" && (
            <Field
              name="price"
              label="報價（NT$）"
              type="number"
              min={0}
              value={o.price}
            />
          )}
          <label>
            回覆
            <select name="action">
              <option value="accept">
                {o.owner
                  ? o.status === "待確認中"
                    ? "提供報價"
                    : o.status === "價格確認"
                      ? "開始製作"
                      : "完成委託"
                  : "確認報價"}
              </option>
              <option value="reject">拒絕</option>
            </select>
          </label>
        </AsyncForm>
      )}
    </article>
  );
}
