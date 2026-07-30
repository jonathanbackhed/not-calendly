import { getEventTypes } from "@/lib/api/public";
import { useQuery } from "@tanstack/react-query";

export const useEventTypes = (userSlug: string | null) => {
  return useQuery({
    staleTime: 1000 * 60 * 5,
    queryKey: ["eventTypes", userSlug],
    queryFn: () => getEventTypes(userSlug!),
    enabled: !!userSlug,
  });
};
