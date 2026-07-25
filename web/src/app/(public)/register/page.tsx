import RegisterForm from "@/components/forms/RegisterForm";

export default function Register() {
  return (
    <div className="flex flex-1 flex-col items-center justify-center">
      <h3 className="mb-4 text-2xl font-bold">Create account</h3>
      <RegisterForm />
    </div>
  );
}
