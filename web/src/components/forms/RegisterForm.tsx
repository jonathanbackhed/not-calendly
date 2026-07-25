"use client";

import api from "@/lib/api";
import { RegisterRequest, RegisterSchema } from "@/lib/schemas/auth";
import { useAuthStore } from "@/lib/store";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import TextInput from "../ui/form/TextInput";
import Link from "next/link";
import SubmitButton from "../ui/form/SubmitButton";
import axios from "axios";

export default function RegisterForm() {
  const router = useRouter();
  const setAccessToken = useAuthStore((s) => s.setAccessToken);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterRequest>({
    resolver: zodResolver(RegisterSchema),
  });

  const onSubmit = async (data: RegisterRequest) => {
    try {
      const { confirmPassword, ...requestData } = data;
      const response = await api.post("/api/auth/register", requestData);
      setAccessToken(response.data.accessToken);
      router.push("/dashboard");
    } catch (error) {
      if (axios.isAxiosError(error)) {
        toast.error(error.response?.data?.error ?? "Something went wrong.");
      } else {
        toast.error("Something went wrong.");
      }
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="flex flex-col">
        <label htmlFor="email">Email</label>
        <TextInput id="email" type="email" autoComplete="off" {...register("email")} />
        {errors.email && <p className="text-sm text-red-500">{errors.email.message}</p>}
      </div>
      <div className="flex flex-col">
        <label htmlFor="password">Password</label>
        <TextInput id="password" type="password" autoComplete="off" {...register("password")} />
        {errors.password && <p className="text-sm text-red-500">{errors.password.message}</p>}
      </div>
      <div className="flex flex-col">
        <TextInput type="password" placeholder="confirm password" autoComplete="off" {...register("confirmPassword")} />
        {errors.confirmPassword && <p className="text-sm text-red-500">{errors.confirmPassword.message}</p>}
      </div>
      <div className="flex flex-col">
        <label htmlFor="email">Slug</label>
        <TextInput id="slug" type="text" placeholder="ex. jane-pt-coach" autoComplete="off" {...register("slug")} />
        {errors.slug && <p className="text-sm text-red-500">{errors.slug.message}</p>}
      </div>
      <div className="flex flex-col space-y-2">
        <span className="text-sm">
          Already have an account?{" "}
          <Link href={"/login"} className="text-blue-600">
            Login
          </Link>
        </span>
      </div>
      <div className="flex justify-center">
        <SubmitButton text="Create account" />
      </div>
    </form>
  );
}
