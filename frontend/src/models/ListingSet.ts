import { z } from "zod/v4";
import { ListingSchema } from "./Listing";

export const ListingSetSchema = z.object({
  page: z.number(),
  pageSize: z.number(),
  totalCount: z.number(),
  items: z.array(ListingSchema),
});

export type ListingSet = z.infer<typeof ListingSetSchema>;
