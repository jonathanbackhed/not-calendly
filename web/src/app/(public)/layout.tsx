import Navbar from "@/components/layout/Navbar";
import React from "react";

export default function PublicLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <div className="relative mx-auto min-h-screen w-full max-w-7xl border-x border-neutral-200">
      <div className="h-10 border-b border-neutral-200" />
      <Navbar />
      <div className="px-4 py-10">{children}</div>
    </div>
  );
}
