import React from "react";

interface Props {
  text: string;
}

export default function PulsatingText({ text }: Props) {
  return <div className="animate-pulse text-center text-xl font-bold">{text}</div>;
}
