import { UUID } from "crypto";

export interface EventTypeMinimalResponse {
  id: UUID;
  title: string;
  slug: string;
  durationMinutes: number;
  locationType: string;
  locationValue: string;
}
