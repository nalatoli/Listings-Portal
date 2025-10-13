import { z } from "zod/v4";
import { HoaSchema } from "./Hoa";
import { RealtorSchema } from "./Realtor";

export const ListingSchema = z.object({
  id: z.number(),
  guid: z.string(),
  addressLine1: z.string(),
  addressLine2: z.string().nullable(),
  city: z.string(),
  state: z.string(),
  zipCode: z.string(),
  county: z.string(),
  latitude: z.number(),
  longitude: z.number(),
  propertyType: z.string(),
  bedrooms: z.number(),
  bathrooms: z.number(),
  squareFootage: z.number(),
  lotSize: z.number(),
  yearBuilt: z.number(),
  hoa: HoaSchema.nullable(),
  price: z.number(),
  listingType: z.string(),
  listedDate: z.iso.datetime(),
  mlsName: z.string().nullable(),
  mlsNumber: z.string().nullable(),
  listingAgent: RealtorSchema.nullable(),
  listingOffice: RealtorSchema.nullable(),
});

export type Listing = z.infer<typeof ListingSchema>;
