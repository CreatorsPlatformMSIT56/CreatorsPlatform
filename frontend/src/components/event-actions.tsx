"use client";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { api, readImage } from "@/lib/api";
import { useSession } from "./session";
import { AsyncForm, Field, text } from "./ui";

/** 呈現活動投稿的喜歡操作。 */
export function EventLike({ id, likes }: { id: number; likes: number }) {
  const router = useRouter();
  return (
    <AsyncForm
      className="inline-form"
      label={`♡ ${likes}`}
      action={async () => {
        await api(`events/entries/${id}/like`, "POST");
        router.refresh();
      }}
    />
  );
}
/** 呈現活動投稿功能。 */
export function EventParticipation({
  id,
  creatorId,
}: {
  id: number;
  creatorId: number;
}) {
  const { member, loading } = useSession(),
    router = useRouter();
  return (
    <>
      {member?.creatorId === creatorId && (
        <p>
          <Link className="button" href={`/events/${id}/edit`}>
            編輯活動
          </Link>
        </p>
      )}
      <h2>投稿作品</h2>
      {loading ? (
        <p role="status">確認登入狀態…</p>
      ) : member ? (
        <AsyncForm
          label="送出投稿"
          action={async (data) => {
            await api(`events/${id}/entries`, "POST", {
              title: text(data, "title"),
              description: text(data, "description"),
              image: await readImage(data.get("image") as File),
            });
            router.refresh();
          }}
        >
          <Field name="title" label="投稿標題" maxLength={100} />
          <Field name="description" label="作品介紹" type="textarea" />
          <label>
            投稿圖片
            <input
              name="image"
              type="file"
              required
              accept="image/png,image/jpeg,image/gif,image/webp"
            />
          </label>
        </AsyncForm>
      ) : (
        <p className="notice">
          請先<Link href="/login">登入</Link>再投稿。
        </p>
      )}
    </>
  );
}
