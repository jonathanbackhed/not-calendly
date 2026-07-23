import api from "@/lib/api";
import { useAuthStore } from "@/lib/store";
import { useRouter } from "next/navigation";
import React, { useEffect } from "react";

export default function AuthProvider({ children }: Readonly<{ children: React.ReactNode }>) {
  const setAccessToken = useAuthStore((s) => s.setAccessToken);
  const router = useRouter();

  useEffect(() => {
    api
      .post("/api/auth/refresh")
      .then(({ data }) => setAccessToken(data.accessToken))
      .catch(() => {
        useAuthStore.getState().setLoading(false);
        router.replace("/login");
      });
  }, []);

  return children;
}
