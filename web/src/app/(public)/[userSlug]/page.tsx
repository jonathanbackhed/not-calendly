import EventTypeList from "@/components/features/booking/EventTypeList";

export default async function UserPage({ params }: { params: Promise<{ userSlug: string }> }) {
  const { userSlug } = await params;

  return (
    <div className="flex flex-1 flex-col py-4">
      <EventTypeList userSlug={userSlug} />
    </div>
  );
}
