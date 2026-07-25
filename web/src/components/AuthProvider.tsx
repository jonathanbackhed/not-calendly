"use client";

import api from "@/lib/api";
import { useAuthStore } from "@/lib/store";
import axios from "axios";
import React, { useEffect } from "react";

export default function AuthProvider({ children }: { children: React.ReactNode }) {
  const setAccessToken = useAuthStore((s) => s.setAccessToken);
  const setLoading = useAuthStore((s) => s.setLoading);

  useEffect(() => {
    axios
      .post(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/refresh`, {}, { withCredentials: true })
      .then(({ data }) => setAccessToken(data.accessToken))
      .catch(() => setLoading(false));
  }, []);

  return children;
}
