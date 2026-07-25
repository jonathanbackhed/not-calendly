import { create } from "zustand";

interface AuthState {
  accessToken: string | null;
  setAccessToken: (token: string) => void;
  isLoading: boolean;
  setLoading: (loading: boolean) => void;
  clearAccessToken: () => void;
}

export const useAuthStore = create<AuthState>()((set) => ({
  accessToken: null,
  setAccessToken: (token: string) => set({ accessToken: token, isLoading: false }),
  isLoading: true,
  setLoading: (loading: boolean) => set({ isLoading: loading }),
  clearAccessToken: () => set({ accessToken: null }),
}));
