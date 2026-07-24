"use client";

import { useAuthStore } from "@/lib/store";
import Link from "next/link";

export default function Navbar() {
  const accessToken = useAuthStore((s) => s.accessToken);

  return (
    <nav className="bg-background sticky top-4 z-20 mx-auto -mt-6 flex h-12 max-w-3xl items-center justify-between rounded-full border-2 border-white px-4 shadow-xs">
      <div className="flex items-center gap-4">
        <span className="text-lg font-bold">Not-calendly</span>
        <div className="flex gap-4 text-sm">
          <a href="#" className="opacity-80 hover:opacity-60">
            Home
          </a>
          <a href="#" className="opacity-80 hover:opacity-60">
            Pricing
          </a>
          <a href="#" className="opacity-80 hover:opacity-60">
            About
          </a>
        </div>
      </div>
      {accessToken ? (
        <Link
          href={"/dashboard"}
          className="rounded-full bg-white px-3 py-1 text-sm shadow-sm hover:cursor-pointer hover:opacity-80 focus:ring-0"
        >
          Dashboard
        </Link>
      ) : (
        <Link
          href={"/login"}
          className="rounded-full bg-white px-3 py-1 text-sm shadow-sm hover:cursor-pointer hover:opacity-80 focus:ring-0"
        >
          Log in
        </Link>
      )}
    </nav>
  );
}
