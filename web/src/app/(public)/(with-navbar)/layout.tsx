import Navbar from "@/components/layout/Navbar";
import React from "react";

export default function PublicNavbarLayout({ children, modal }: { children: React.ReactNode; modal: React.ReactNode }) {
  return (
    <>
      <Navbar />
      <div className="flex flex-1 flex-col pt-4">
        {children}
        {modal}
      </div>
    </>
  );
}
