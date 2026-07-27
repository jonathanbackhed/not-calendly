export default function PublicLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="mx-auto flex min-h-screen w-full max-w-6xl flex-col border-x border-neutral-200">
      <div className="h-10 border-b border-neutral-200" />
      {children}
      <div className="static bottom-0 left-0 h-10 w-full border-t border-neutral-200" />
    </div>
  );
}
