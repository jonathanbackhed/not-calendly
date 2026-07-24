"use client";

import { X } from "lucide-react";
import { useRouter } from "next/navigation";
import React from "react";

interface Props {
  children: React.ReactNode;
}

export default function ModalSkeleton({ children }: Props) {
  const router = useRouter();

  const handleClose = () => {
    const authRoutes = ["/login", "/register", "/forgot-password"];
    const prevUrl = document.referrer;

    if (authRoutes.some((route) => prevUrl.includes(route))) {
      router.push("/");
    } else {
      router.back();
    }
  };

  return (
    <div className="absolute top-0 right-0 bottom-0 left-0 z-40 flex h-screen w-screen items-center justify-center bg-black/50">
      <div className="bg-background rounded-lg p-4">
        <div className="flex justify-end">
          <button
            onClick={handleClose}
            className="rounded-full bg-white p-1 shadow-xs hover:cursor-pointer hover:opacity-80"
          >
            <X size={24} color="black" />
          </button>
        </div>
        {children}
      </div>
    </div>
  );
}
