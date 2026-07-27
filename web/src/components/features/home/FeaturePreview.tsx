"use client";

import { cn } from "@/lib/utils";
import { useState } from "react";

export default function FeaturePreview() {
  const [index, setIndex] = useState(0);

  return (
    <div className="flex flex-1 flex-col items-center justify-center px-48">
      <div className="mx-48 aspect-square w-full bg-white">{index}</div>
      <div className="flex w-full">
        <button
          onClick={() => setIndex(0)}
          className={cn(
            "flex-1 cursor-pointer border border-neutral-200 p-4 text-sm font-semibold",
            index === 0 && "bg-white",
          )}
        >
          Your booking link, instantly shareable.
        </button>
        <button
          onClick={() => setIndex(1)}
          className={cn(
            "flex-1 cursor-pointer border border-neutral-200 p-4 text-sm font-semibold",
            index === 1 && "bg-white",
          )}
        >
          Set your availability once.
        </button>
        <button
          onClick={() => setIndex(2)}
          className={cn(
            "flex-1 cursor-pointer border border-neutral-200 p-4 text-sm font-semibold",
            index === 2 && "bg-white",
          )}
        >
          Get notified, stay on top.
        </button>
      </div>
    </div>
  );
}
