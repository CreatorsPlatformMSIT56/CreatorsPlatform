"use client";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { AsyncForm } from "./ui";

/** 呈現追蹤創作者的操作。 */
export function CreatorFollow({
  id,
  following,
}: {
  id: number;
  following: boolean;
}) {
  const router = useRouter();
  return (
    <AsyncForm
      className="inline-form"
      label={following ? "取消追蹤" : "＋ 追蹤創作者"}
      action={async () => {
        await api(`studio/creators/${id}/follow`, "POST");
        router.refresh();
      }}
    />
  );
}
