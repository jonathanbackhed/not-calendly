"use client";

import api from "@/lib/api";
import { LoginRequest, LoginSchema } from "@/lib/schemas/auth";
import { useAuthStore } from "@/lib/store";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import TextInput from "../ui/form/TextInput";
import Link from "next/link";
import SubmitButton from "../ui/form/SubmitButton";

export default function LoginForm() {
  const router = useRouter();
  const setAccessToken = useAuthStore((s) => s.setAccessToken);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginRequest>({
    resolver: zodResolver(LoginSchema),
  });

  const onSubmit = async (data: LoginRequest) => {
    try {
      const response = await api.post("/api/auth/login", data);
      setAccessToken(response.data.accessToken);
      router.push("/dashboard");
    } catch {
      toast("Invalid email or password.");
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="flex flex-col">
        <label htmlFor="email" className="">
          Email
        </label>
        <TextInput id="email" type="email" {...register("email")} />
        {errors.email && <p className="text-sm text-red-500">{errors.email.message}</p>}
      </div>
      <div className="flex flex-col">
        <label htmlFor="password" className="">
          Password
        </label>
        <TextInput id="password" type="password" {...register("password")} />
        {errors.password && <p className="text-sm text-red-500">{errors.password.message}</p>}
      </div>
      <div className="flex flex-col space-y-2">
        <Link href={"/forgot-password"} className="text-sm text-blue-600">
          Forgot password?
        </Link>
        <span className="text-sm">
          No account?{" "}
          <Link href={"/register"} className="text-blue-600">
            Create an account
          </Link>
        </span>
      </div>
      <div className="flex justify-center">
        <SubmitButton text="Log in" />
      </div>
    </form>
  );
}
