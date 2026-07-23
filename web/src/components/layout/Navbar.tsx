import React from "react";

export default function Navbar() {
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
      <button className="rounded-full bg-white px-3 py-1 text-sm shadow-sm hover:cursor-pointer hover:opacity-80 focus:ring-0">
        Log in
      </button>
    </nav>
  );
}
