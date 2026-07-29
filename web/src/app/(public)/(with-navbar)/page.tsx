import FeaturePreview from "@/components/features/home/FeaturePreview";
import Link from "next/link";

export default function Home() {
  return (
    <div className="flex flex-col">
      <div className="flex flex-1 flex-col items-center justify-center space-y-6 p-48">
        <h1 className="font-special text-center text-7xl font-light">
          Let clients book you. <br /> You focus on the work.
        </h1>
        <p className="text-center">Share your booking link and let clients schedule time with you.</p>
        <Link href={"/login"} className="rounded-full bg-black px-8 py-4 text-white shadow-xs">
          Get started today
        </Link>
      </div>

      <FeaturePreview />

      <div className="flex flex-1 flex-col items-center justify-center space-y-6 border-y border-neutral-200 px-48 py-32">
        <div className="rounded-full bg-white px-3 py-1 text-xs shadow-xs">Zero friction booking</div>
        <h1 className="text-center text-5xl font-semibold">From &apos;are you free?&apos; to booked - in seconds</h1>
        <p className="text-center">No emails back and forth. No scheduling confusion. Just a link that works.</p>
      </div>

      <div className="flex flex-1 flex-col items-center justify-center space-y-6 px-48 py-32">
        <div></div>
      </div>
    </div>
  );
}
