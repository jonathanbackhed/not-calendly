"use client";

import api from "@/lib/api";
import { useAuthStore } from "@/lib/store";
import React, { useEffect } from "react";

export default function AuthProvider({ children }: { children: React.ReactNode }) {
  const setAccessToken = useAuthStore((s) => s.setAccessToken);
  const setLoading = useAuthStore((s) => s.setLoading);

  useEffect(() => {
    api
      .post("/api/auth/refresh")
      .then(({ data }) => setAccessToken(data.accessToken))
      .catch(() => setLoading(false));
  }, []);

  return children;
}
