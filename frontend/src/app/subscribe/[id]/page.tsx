"use client";
import { use } from "react";
import Link from "next/link";
import { api } from "@/lib/api";
import { useResource } from "@/lib/use-resource";
import type { Plan } from "@/lib/types";
import { AsyncForm, Status } from "@/components/ui";
import { RichText } from "@/components/rich-text";
import { useSession } from "@/components/session";

/** 呈現訂閱方案及訂閱操作。 */
export default function Subscribe({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params),
    resource = useResource<Plan & { creatorName: string; creatorId: number }>(
      `catalog/plans/${id}`,
    ),
    p = resource.data;
  const { member } = useSession();
  return (
    <section className="narrow">
      <Status {...resource} retry={resource.reload} />
      {p && (
        <>
          <p className="eyebrow">SUPPORT THE CREATOR</p>
          <h1>{p.name}</h1>
          <Link href={`/creators/${p.creatorId}`}>{p.creatorName}</Link>
          <p className="price">NT$ {p.price.toLocaleString()} / 月</p>
          <RichText html={p.description} />
          <p className="notice">
            此為訂閱示範功能，確認後會建立一個月的訂閱紀錄，不會實際扣款。
          </p>
          {member ? (
            <AsyncForm
              label="確認訂閱"
              action={async () => {
                await api(`studio/plans/${id}/subscribe`, "POST");
              }}
            />
          ) : (
            <Link className="button primary" href="/login">
              登入以訂閱
            </Link>
          )}
          <p>
            <Link href="/account">前往會員中心查看訂閱紀錄 →</Link>
          </p>
        </>
      )}
    </section>
  );
}
