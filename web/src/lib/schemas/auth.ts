import { z } from "zod";

export const LoginSchema = z.object({
  email: z.email("Invalid email address"),
  password: z.string().min(1, "Password is required"),
});
export type LoginRequest = z.infer<typeof LoginSchema>;

export const RegisterSchema = z
  .object({
    email: z.email("Invalid email address"),
    password: z
      .string()
      .min(10, "Password must be at least 10 characters long")
      .regex(
        /^(?=.*[A-Z])(?=.*\d)(?=.*[!?/\@#$%^&*]).+$/,
        "Password must contain at least one uppercase letter, one number, and one special character",
      ),
    confirmPassword: z.string(),
    slug: z
      .string()
      .min(1, "Slug is required")
      .max(50, "Slug cannot be longer than 50 characters")
      .regex(/^[a-z0-9-]+$/, "Slug may only contain lowercase letters, numbers and hyphens"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });
export type RegisterRequest = z.infer<typeof RegisterSchema>;
