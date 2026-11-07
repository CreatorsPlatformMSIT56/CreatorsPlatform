import { EventForm } from "@/components/event-form";
/** 呈現此路由的頁面內容。 */
export default async function Page({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  return <EventForm id={id} />;
}
