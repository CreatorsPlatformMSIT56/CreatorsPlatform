"use client";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { AsyncForm, Field, text } from "./ui";

export function WorkActions({ id, likes }: { id: number; likes: number }) {
  const router = useRouter();
  return <><AsyncForm className="inline-form" label={`♡ 喜歡（${likes}）`} action={async () => {
    await api(`studio/works/${id}/like`, "POST"); router.refresh();
  }} /><h2>留言</h2><AsyncForm label="送出留言" action={async data => {
    await api(`studio/works/${id}/comments`, "POST", { text: text(data, "text") }); router.refresh();
  }}><Field label="分享你的想法" name="text" type="textarea" maxLength={500} /></AsyncForm></>;
}
