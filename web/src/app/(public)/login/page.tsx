import LoginForm from "@/components/forms/LoginForm";

export default function Login() {
  return (
    <div className="flex flex-1 flex-col items-center justify-center">
      <h3 className="mb-4 text-2xl font-bold">Log in</h3>
      <LoginForm />
    </div>
  );
}
