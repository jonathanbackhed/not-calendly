import Navbar from "@/components/layout/Navbar";
import React from "react";

export default function PublicNavbarLayout({ children, modal }: { children: React.ReactNode; modal: React.ReactNode }) {
  return (
    <>
      <Navbar />
      <div className="px-4 py-10">
        {children}
        {modal}
      </div>
    </>
  );
}
