"use client";

import { LoginRequest, loginSchema } from "@/lib/schemas/auth";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import ModalSkeleton from "@/components/ui/ModalSkeleton";
import TextInput from "@/components/ui/form/TextInput";
import SubmitButton from "@/components/ui/form/SubmitButton";
import api from "@/lib/api";
import { useAuthStore } from "@/lib/store";
import { useRouter } from "next/navigation";
import { toast } from "sonner";

export default function LoginModal() {
  const router = useRouter();
  const setAccessToken = useAuthStore((s) => s.setAccessToken);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginRequest>({
    resolver: zodResolver(loginSchema),
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
    <ModalSkeleton>
      <div className="px-8">
        <h3 className="mb-2 text-center text-2xl font-bold">Log in</h3>
        {/* <p className="text-sm">
            By continuing, you agree to our user agreement and acknowledge that you understand our privacy policy.
          </p> */}
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
      </div>
    </ModalSkeleton>
  );
}
