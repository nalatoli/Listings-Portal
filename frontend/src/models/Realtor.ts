import { z } from "zod/v4";

export const RealtorSchema = z.object({
  name: z.string(),
  phone: z.string(),
  email: z.string(),
  website: z.string(),
});

export type Realtor = z.infer<typeof RealtorSchema>;
