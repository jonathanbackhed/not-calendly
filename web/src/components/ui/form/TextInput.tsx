import { cn } from "@/lib/utils";
import React from "react";

interface Props extends React.InputHTMLAttributes<HTMLInputElement> {
  customStyles?: string;
}

export default function TextInput({ customStyles, ...rest }: Props) {
  return <input className={cn("rounded-lg border-0 px-5 py-3 shadow-xs focus:ring-0", customStyles)} {...rest} />;
}
