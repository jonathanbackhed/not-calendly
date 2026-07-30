"use client";

import PulsatingText from "@/components/ui/PulsatingText";
import { useEventTypes } from "@/hooks/usePublic";
import { EventTypeMinimalResponse } from "@/lib/types/eventTypes";
import { cn } from "@/lib/utils";
import { ArrowRight, CircleQuestionMark, Clock, MapPin, Phone, Video } from "lucide-react";
import Link from "next/link";

interface Props {
  userSlug: string;
}

export default function EventTypeList({ userSlug }: Props) {
  const { data, isLoading, error } = useEventTypes(userSlug);

  if (isLoading)
    return (
      <div className="flex flex-1 items-center justify-center">
        <PulsatingText text="Loading..." />
      </div>
    );

  if (error)
    return (
      <div className="flex flex-1 items-center justify-center">
        <PulsatingText text={`Error: ${error.message}`} />
      </div>
    );

  return (
    <div className="flex flex-col">
      <h3 className="text-center text-2xl font-bold">{userSlug}</h3>
      <h2 className="mb-4 text-center text-2xl">Pick a meeting type to get started</h2>
      <div className="grid grid-cols-3 gap-2 px-4">
        {data.map((e: EventTypeMinimalResponse) => {
          const locationType = e.locationType.toLowerCase();

          return (
            <Link key={e.id} href={`${userSlug}/${e.slug}`}>
              <div
                className={cn(
                  "flex flex-row items-center gap-2 rounded-lg border border-transparent bg-white p-4 shadow-xs hover:shadow-sm",
                  locationType === "online"
                    ? "hover:bg-blue-100"
                    : locationType === "phone"
                      ? "hover:bg-orange-100"
                      : locationType === "physical"
                        ? "hover:bg-green-100"
                        : "hover:bg-neutral-100",
                )}
              >
                {locationType === "online" ? (
                  <div className="h-fit w-fit rounded-lg bg-blue-900 p-2.5">
                    <Video size={14} className="text-blue-300" />
                  </div>
                ) : locationType === "phone" ? (
                  <div className="h-fit w-fit rounded-lg bg-orange-900 p-2.5">
                    <Phone size={14} className="text-orange-300" />
                  </div>
                ) : locationType === "physical" ? (
                  <div className="h-fit w-fit rounded-lg bg-green-900 p-2.5">
                    <MapPin size={15} className="text-green-300" />
                  </div>
                ) : (
                  <div className="h-fit w-fit rounded-lg bg-neutral-900 p-2.5">
                    <CircleQuestionMark size={14} className="text-neutral-300" />
                  </div>
                )}
                <div>
                  <h3 className="text-xl font-semibold">{e.title}</h3>
                  <span className="flex items-center gap-1">
                    <Clock size={16} className="inline text-neutral-600" />
                    {e.durationMinutes} min &middot; {e.locationValue}
                  </span>
                </div>
                <ArrowRight size={24} className="ml-auto" />
              </div>
            </Link>
          );
        })}
      </div>
    </div>
  );
}
