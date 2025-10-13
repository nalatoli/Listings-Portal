import { z } from "zod/v4";

export const HoaSchema = z.object({
  fee: z.number(),
});

export type Hoa = z.infer<typeof HoaSchema>;
