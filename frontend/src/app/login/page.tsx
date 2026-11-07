"use client";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { useSession } from "@/components/session";
import { AsyncForm, Field, text } from "@/components/ui";
/** 呈現使用者登入表單。 */
export default function Login() {
  const router = useRouter(),
    { refresh } = useSession();
  return (
    <section className="auth">
      <p className="eyebrow">WELCOME BACK</p>
      <h1>歡迎回來</h1>
      <p className="muted">繼續探索你的創作宇宙。</p>
      <AsyncForm
        label="登入"
        action={async (data) => {
          await api("account/login", "POST", {
            email: text(data, "email"),
            password: text(data, "password"),
          });
          await refresh();
          router.push("/feed");
        }}
      >
        <Field name="email" type="email" label="電子郵件" />
        <Field name="password" type="password" label="密碼" />
      </AsyncForm>
      <div className="auth-links">
        <Link href="/forgot-password">忘記密碼？</Link>
        <Link href="/register">建立帳號 →</Link>
      </div>
    </section>
  );
}
