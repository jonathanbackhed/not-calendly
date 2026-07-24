export default function PublicLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="mx-auto min-h-screen w-full max-w-7xl border-x border-neutral-200">
      <div className="h-10 border-b border-neutral-200" />
      {children}
      <div className="absolute bottom-0 h-10 border-t border-neutral-200" />
    </div>
  );
}
