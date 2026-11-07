import { EventForm } from "@/components/event-form";
export default async function Page({ params }: { params: Promise<{ id: string }> }) { const { id } = await params; return <EventForm id={id} />; }
