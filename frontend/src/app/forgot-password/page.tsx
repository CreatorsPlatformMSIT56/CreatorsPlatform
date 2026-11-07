"use client";
import Link from "next/link";
import { useState } from "react";
import { api } from "@/lib/api";
import { AsyncForm, Field, text } from "@/components/ui";
export default function ForgotPassword() {
  const [email, setEmail] = useState("");
  return <section className="auth"><h1>重設密碼</h1><AsyncForm label="寄送驗證碼" action={async data => { const value = text(data, "email"); await api("account/reset-code", "POST", { email: value }); setEmail(value); }}><Field name="email" type="email" label="帳號電子郵件" /></AsyncForm>
    {email && <><p>若帳號存在，驗證碼已寄出，請在 10 分鐘內完成。</p><AsyncForm label="重設密碼" action={async data => {
      await api("account/reset-password", "POST", { email, code: text(data, "code"), password: text(data, "password") }); setEmail("");
    }}><Field name="code" label="信箱驗證碼" /><Field name="password" type="password" label="新密碼（至少 8 個字元）" maxLength={100} /></AsyncForm></>}
    <p><Link href="/login">返回登入</Link></p></section>;
}
