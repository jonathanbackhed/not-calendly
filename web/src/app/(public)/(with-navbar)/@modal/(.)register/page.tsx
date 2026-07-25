import RegisterForm from "@/components/forms/RegisterForm";
import ModalSkeleton from "@/components/ui/ModalSkeleton";

export default function RegisterModal() {
  return (
    <ModalSkeleton>
      <div className="px-8">
        <h3 className="mb-4 text-center text-2xl font-bold">Create account</h3>
        <RegisterForm />
      </div>
    </ModalSkeleton>
  );
}
