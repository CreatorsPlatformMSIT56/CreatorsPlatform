"use client";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { useSession } from "@/components/session";
import { AsyncForm, Field, text } from "@/components/ui";
/** 呈現使用者註冊表單。 */
export default function Register() {
  const router = useRouter(),
    { refresh } = useSession();
  return (
    <section className="auth">
      <p className="eyebrow">YOUR NEXT CHAPTER</p>
      <h1>加入 IMAGINK</h1>
      <AsyncForm
        label="建立帳號"
        action={async (data) => {
          const password = text(data, "password");
          if (password.length < 8) throw new Error("密碼至少需要 8 個字元。");
          if (password !== text(data, "confirm"))
            throw new Error("兩次密碼不一致。");
          await api("account/register", "POST", {
            name: text(data, "name"),
            email: text(data, "email"),
            password,
          });
          await refresh();
          router.push("/account");
        }}
      >
        <Field name="name" label="顯示名稱" maxLength={40} />
        <Field name="email" type="email" label="電子郵件" />
        <Field
          name="password"
          type="password"
          label="密碼（至少 8 個字元）"
          maxLength={100}
        />
        <Field
          name="confirm"
          type="password"
          label="再次輸入密碼"
          maxLength={100}
        />
      </AsyncForm>
      <p>
        已經有帳號？<Link href="/login">登入</Link>
      </p>
    </section>
  );
}
