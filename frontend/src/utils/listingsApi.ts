import { useMap } from "@vis.gl/react-google-maps";
import type { Filters } from "../models/Filters";
import { ListingSetSchema, type ListingSet } from "../models/ListingSet";
import { useEffect, useState } from "react";

function getEndpoint(relUrl: string) {
  return `${import.meta.env.VITE_SERVER_URL}/api/${relUrl}`;
}

export default function listingsApi() {
  async function fetchListingsPage(
    filters: Filters,
    page: number,
    pageSize: number,
    signal?: AbortSignal
  ): Promise<ListingSet> {
    const qs = new URLSearchParams({
      page: String(page),
      pageSize: String(pageSize),
    });

    if (filters.minPrice != null) qs.set("minPrice", String(filters.minPrice));
    if (filters.maxPrice != null) qs.set("maxPrice", String(filters.maxPrice));
    if (filters.minBedrooms != null)
      qs.set("bedrooms", String(filters.minBedrooms));
    if (filters.minBathrooms != null)
      qs.set("bathrooms", String(filters.minBathrooms));
    if (filters.maxDaysOld != null)
      qs.set("daysOld", String(filters.maxDaysOld));
    if (filters.counties && filters.counties.length > 0)
      filters.counties.forEach((c) => qs.append("counties", c));

    const resp = await fetch(getEndpoint(`v1/listings/counties?${qs}`), {
      signal,
    });
    if (!resp.ok) throw new Error(`HTTP ${resp.status}: ${await resp.text()}`);
    return ListingSetSchema.parse(await resp.json());
  }

  async function getListings(filters: Filters, signal?: AbortSignal) {
    const items: ListingSet["items"] = [];
    let page = 1;

    while (true) {
      const data = await fetchListingsPage(filters, page, 50, signal);
      if (data.items.length === 0) break;
      items.push(...data.items);
      if (items.length >= data.totalCount) break;
      if (data.items.length < data.pageSize) break;

      page += 1;
    }

    return items;
  }

  return {
    getListings,
  };
}

export function useVisibleListingCount(
  listings: Array<{ latitude: number; longitude: number }>
) {
  const map = useMap();
  const [count, setCount] = useState(0);

  useEffect(() => {
    if (!map) return;

    let timeout: number | null = null;

    const updateCount = () => {
      const bounds = map.getBounds();
      if (!bounds) return;

      const c = listings.reduce((acc, x) => {
        const pos = new google.maps.LatLng(x.latitude, x.longitude);
        return acc + (bounds.contains(pos) ? 1 : 0);
      }, 0);

      setCount(c);
    };

    const handleIdle = () => {
      if (timeout) window.clearTimeout(timeout);
      timeout = window.setTimeout(updateCount, 100);
    };

    handleIdle();
    const idleListener = map.addListener("idle", handleIdle);

    return () => {
      if (timeout) window.clearTimeout(timeout);
      idleListener.remove();
    };
  }, [map, listings]);

  return count;
}
