import FeaturePreview from "@/components/features/home/FeaturePreview";
import Link from "next/link";

export default function Home() {
  return (
    <div className="flex flex-col">
      <div className="flex flex-1 flex-col items-center justify-center space-y-6 bg-red-100 p-48">
        <h1 className="font-special text-center text-7xl font-light">
          Let clients book you. <br /> You focus on the work.
        </h1>
        <p className="text-center">
          Share your booking link and let clients schedule time with you - no back-and-forth, no hassle.
        </p>
        <Link href={"/login"} className="rounded-full bg-black px-8 py-4 text-white shadow-xs">
          Get started today
        </Link>
      </div>

      <FeaturePreview />
    </div>
  );
}
