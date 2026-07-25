import { cn } from "@/lib/utils";
import React from "react";

interface Props extends React.InputHTMLAttributes<HTMLInputElement> {
  text: string;
  customStyles?: string;
}

export default function SubmitButton({ text, customStyles, ...rest }: Props) {
  return (
    <input
      type="submit"
      value={text}
      className={cn(
        "w-full rounded-full bg-white px-5 py-3 text-sm shadow-sm hover:cursor-pointer hover:opacity-80 focus:ring-0",
        customStyles,
      )}
      {...rest}
    />
  );
}
