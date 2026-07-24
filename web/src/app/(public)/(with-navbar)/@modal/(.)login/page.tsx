import ModalSkeleton from "@/components/ui/ModalSkeleton";
import LoginForm from "@/components/forms/LoginForm";

export default function LoginModal() {
  return (
    <ModalSkeleton>
      <div className="px-8">
        <h3 className="mb-4 text-center text-2xl font-bold">Log in</h3>
        <LoginForm />
      </div>
    </ModalSkeleton>
  );
}
