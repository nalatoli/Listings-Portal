import {
  AdvancedMarker,
  APIProvider,
  Map,
  InfoWindow,
  useMap,
} from "@vis.gl/react-google-maps";
import { MarkerClusterer } from "@googlemaps/markerclusterer";
import type { Marker as GMarker } from "@googlemaps/markerclusterer";
import {
  useCallback,
  useEffect,
  useState,
  useMemo,
  useRef,
  useLayoutEffect,
} from "react";
import listingsApi, { useVisibleListingCount } from "./utils/listingsApi";
import type { Listing } from "./models/Listing";
import "./App.css";
import type { Filters } from "./models/Filters";
import FilterBar from "./components/FilterBar";

function ResultStrip({
  visibleCount,
  count,
  loading,
  top,
}: {
  visibleCount: number;
  count: number;
  loading: boolean;
  top: number;
}) {
  const label = loading
    ? "Loading…"
    : `${visibleCount.toLocaleString()} / ${count.toLocaleString()} listing${
        count === 1 ? "" : "s"
      }`;
  return (
    <div
      className="result-strip"
      role="status"
      aria-live="polite"
      style={{ top: `${top}px` }}
    >
      <span className="result-pill">{label}</span>
    </div>
  );
}

const DEFAULTS: Filters = {
  minPrice: null,
  maxPrice: null,
  minBedrooms: null,
  minBathrooms: null,
  maxDaysOld: 1,
};

function toNum(v: string | null): number | null {
  if (v == null) return null;
  const n = Number(v);
  return Number.isFinite(n) ? n : null;
}

function parseFiltersFromSearch(search: string): Filters {
  const q = new URLSearchParams(search);
  const parseCounties = (): string[] | null => {
    const entries = q.getAll("counties");
    const list = Array.from(
      new Set(
        entries
          .flatMap((v) => (v ?? "").split(","))
          .map((s) => s.trim())
          .filter(Boolean),
      ),
    );
    return list.length ? list : (DEFAULTS.counties ?? null);
  };
  return {
    minPrice: q.has("minPrice") ? toNum(q.get("minPrice")) : DEFAULTS.minPrice,
    maxPrice: q.has("maxPrice") ? toNum(q.get("maxPrice")) : DEFAULTS.maxPrice,
    minBedrooms: q.has("minBedrooms")
      ? toNum(q.get("minBedrooms"))
      : DEFAULTS.minBedrooms,
    minBathrooms: q.has("minBathrooms")
      ? toNum(q.get("minBathrooms"))
      : DEFAULTS.minBathrooms,
    maxDaysOld: q.has("maxDaysOld")
      ? toNum(q.get("maxDaysOld"))
      : DEFAULTS.maxDaysOld,
    counties: parseCounties(),
  };
}

// Light-mode colors are darker so white text stays readable.
// Dark-mode colors are brighter to pop against the dark map.
const COLORS = {
  basic:  { light: "#5a7a00", dark: "#cad411" },
  partial:{ light: "#b45309", dark: "#d49a11" },
  full:   { light: "#b91c1c", dark: "#EA4335" },
  multi:  { light: "#6d28d9", dark: "#7c3aed" },
} as const;

function pin(bg: string, label: string, count = 1) {
  return (
    <div data-count={count} style={{ display: "flex", flexDirection: "column", alignItems: "center", cursor: "pointer" }}>
      <div
        style={{
          background: bg,
          color: "white",
          border: "1.5px solid white",
          borderRadius: 4,
          padding: "2px 6px",
          fontSize: 11,
          fontWeight: 600,
          whiteSpace: "nowrap",
          boxShadow: "0 2px 4px rgba(0,0,0,.4)",
        }}
      >
        {label}
      </div>
      <div
        style={{
          width: 0,
          height: 0,
          borderLeft: "5px solid transparent",
          borderRight: "5px solid transparent",
          borderTop: `6px solid ${bg}`,
        }}
      />
    </div>
  );
}

function ClusteredMarkers({
  listings,
  isDark,
  onSelect,
}: {
  listings: Listing[];
  isDark: boolean;
  onSelect: (group: Listing[]) => void;
}) {
  const map = useMap();
  const clusterer = useRef<MarkerClusterer | null>(null);
  const markerRefs = useRef<Record<string, GMarker>>({});

  // Group listings that share the same address into one marker
  const groups = useMemo(() => {
    const addrMap: Record<string, Listing[]> = {};
    for (const l of listings) {
      const key = `${l.addressLine1}|${l.city}|${l.state}`;
      (addrMap[key] ??= []).push(l);
    }
    return Object.values(addrMap);
  }, [listings]);

  useEffect(() => {
    if (!map) return;
    clusterer.current = new MarkerClusterer({
      map,
      renderer: {
        render({ markers, position }) {
          const total = (markers as google.maps.marker.AdvancedMarkerElement[]).reduce((sum, m) => {
            const root = m.content as HTMLElement | null;
            const el = root?.dataset?.count !== undefined
              ? root
              : root?.querySelector<HTMLElement>("[data-count]");
            return sum + parseInt(el?.dataset?.count ?? "1");
          }, 0);
          const size = total < 10 ? 36 : total < 100 ? 42 : 50;
          const div = document.createElement("div");
          div.style.cssText = `
            width:${size}px;height:${size}px;
            background:#1d4ed8;
            color:white;
            border:2px solid white;
            border-radius:50%;
            display:flex;align-items:center;justify-content:center;
            font-size:${size < 42 ? 12 : 13}px;
            font-weight:700;
            box-shadow:0 2px 6px rgba(0,0,0,.45);
            cursor:pointer;
          `;
          div.textContent = total > 999 ? "999+" : String(total);
          return new google.maps.marker.AdvancedMarkerElement({ position, content: div });
        },
      },
    });
    return () => {
      clusterer.current?.clearMarkers();
      clusterer.current = null;
    };
  }, [map]);

  const setMarkerRef = useCallback((marker: GMarker | null, id: string) => {
    if (marker) {
      markerRefs.current[id] = marker;
    } else {
      delete markerRefs.current[id];
    }
    clusterer.current?.clearMarkers();
    clusterer.current?.addMarkers(Object.values(markerRefs.current));
  }, []);

  return groups.map((group) => {
    const first = group[0];
    const multi = group.length > 1;

    let palette: keyof typeof COLORS = "multi";
    if (!multi) {
      const hasYear = first.yearBuilt !== -1;
      const hasSqft = first.squareFootage !== -1;
      palette = hasYear && hasSqft ? "full" : hasYear || hasSqft ? "partial" : "basic";
    }
    const bg = isDark ? COLORS[palette].dark : COLORS[palette].light;
    const label = multi ? `${group.length} units` : `$${first.price}`;

    return (
      <AdvancedMarker
        key={first.id}
        position={{ lat: first.latitude, lng: first.longitude }}
        ref={(m) => setMarkerRef(m, String(first.id))}
        onClick={() => onSelect(group)}
      >
        {pin(bg, label, group.length)}
      </AdvancedMarker>
    );
  });
}

function ListingsMap() {
  const [isDark, setIsDark] = useState(false);
  const [listings, setListings] = useState<Listing[]>([]);
  const [isLoading, setLoading] = useState(true);
  const [selected, setSelected] = useState<Listing[] | null>(null);
  const [filters, setFilters] = useState<Filters>(() =>
    typeof window === "undefined"
      ? DEFAULTS
      : parseFiltersFromSearch(window.location.search),
  );
  const visibleCount = useVisibleListingCount(listings);

  const listingApi = useMemo(() => listingsApi(), []);
  const lat = parseFloat(import.meta.env.VITE_DEFAULT_LATITUDE);
  const lng = parseFloat(import.meta.env.VITE_DEFAULT_LONGITUDE);
  const refetch = useRef<ReturnType<typeof setTimeout> | null>(null);
  const count = useMemo(() => listings.length, [listings]);
  const [filterBarHeight, setFilterBarHeight] = useState(0);
  const filterBarRef = useRef<HTMLDivElement>(null);

  const fmtInt = (n: number) => Math.round(n).toLocaleString();
  const fmtBaths = (n: number) => (Number.isInteger(n) ? `${n}` : n.toFixed(1));
  const daysOld = (l: Listing) =>
    Math.floor((Date.now() - new Date(l.listedDate).getTime()) / 86_400_000);

  useEffect(() => {
    const onPop = () =>
      setFilters(parseFiltersFromSearch(window.location.search));
    window.addEventListener("popstate", onPop);
    return () => window.removeEventListener("popstate", onPop);
  }, []);

  useLayoutEffect(() => {
    if (filterBarRef.current) {
      setFilterBarHeight(filterBarRef.current.offsetHeight);
    }
  }, [filters]);

  useEffect(() => {
    const mq = window.matchMedia("(prefers-color-scheme: dark)");
    const handler = (evt: MediaQueryListEvent) => setIsDark(evt.matches);

    setIsDark(mq.matches);
    mq.addEventListener("change", handler);
    return () => mq.removeEventListener("change", handler);
  }, []);

  useEffect(() => {
    const ctrl = new AbortController();
    if (refetch.current) clearTimeout(refetch.current);

    refetch.current = setTimeout(async () => {
      try {
        setLoading(true);
        const items = await listingApi.getListings(filters, ctrl.signal);
        setListings(items);
      } catch (e) {
        if (!(e instanceof DOMException && e.name === "AbortError")) {
          console.error("Fetch failed:", e);
          setListings([]);
        }
      } finally {
        setLoading(false);
      }
    }, 250);

    return () => {
      ctrl.abort();
      if (refetch.current) clearTimeout(refetch.current);
    };
  }, [listingApi, lat, lng, filters]);


  return (
    <div>
      <div ref={filterBarRef}>
        <FilterBar value={filters} onChange={setFilters} />
      </div>
      <ResultStrip
        visibleCount={visibleCount}
        count={count}
        loading={isLoading}
        top={filterBarHeight}
      />
      <Map
        style={{ width: "100vw", height: "100vh" }}
        mapId={import.meta.env.VITE_GOOGLE_MAPS_MAP_ID ?? "DEMO_MAP_ID"}
        colorScheme="FOLLOW_SYSTEM"
        defaultCenter={{
          lat: lat,
          lng: lng,
        }}
        defaultZoom={parseInt(import.meta.env.VITE_DEFAULT_ZOOM)}
        gestureHandling="greedy"
        disableDefaultUI
      >
        <ClusteredMarkers
          listings={listings}
          isDark={isDark}
          onSelect={setSelected}
        />
        {selected && (() => {
          const first = selected[0];
          const multi = selected.length > 1;
          return (
            <InfoWindow
              pixelOffset={[0, -28]}
              position={{ lat: first.latitude, lng: first.longitude }}
              onCloseClick={() => setSelected(null)}
            >
              <div style={{ minWidth: 220, lineHeight: 1.25, color: "black" }}>
                {/* Address (always shown) */}
                <div style={{ fontWeight: 700, fontSize: 14, marginBottom: 8 }}>
                  {first.addressLine1}, {first.city}, {first.state}
                </div>

                {multi ? (
                  /* Multi-unit: one row per listing */
                  <div style={{ display: "flex", flexDirection: "column", gap: 10 }}>
                    {selected.map((l) => {
                      const d = daysOld(l);
                      return (
                        <div key={l.id} style={{ borderTop: "1px solid #e5e7eb", paddingTop: 8 }}>
                          <div style={{ fontWeight: 700, fontSize: 15, marginBottom: 3 }}>
                            ${fmtInt(l.price)}
                            {l.addressLine2 && (
                              <span style={{ fontWeight: 400, fontSize: 12, marginLeft: 6, opacity: 0.7 }}>
                                {l.addressLine2}
                              </span>
                            )}
                          </div>
                          <div style={{ fontSize: 12, opacity: 0.85, marginBottom: 5 }}>
                            {l.bedrooms} bd • {fmtBaths(l.bathrooms)} ba
                            {l.squareFootage !== -1 && <> • {fmtInt(l.squareFootage)} sqft</>}
                            {" • "}
                            {d === 0 ? "Today" : d === 1 ? "1 day ago" : `${d} days ago`}
                          </div>
                          <a
                            href={`https://www.google.com/search?q=${encodeURI(l.guid)}`}
                            target="_blank"
                            rel="noreferrer"
                            style={{ fontSize: 12, textDecoration: "underline" }}
                          >
                            Google listing →
                          </a>
                        </div>
                      );
                    })}
                  </div>
                ) : (
                  /* Single listing */
                  (() => {
                    const l = first;
                    const hasYear = l.yearBuilt !== -1;
                    const hasSqft = l.squareFootage !== -1;
                    const d = daysOld(l);
                    return (
                      <>
                        <div style={{ fontWeight: 700, fontSize: 16, marginBottom: 6 }}>
                          ${fmtInt(l.price)}
                        </div>
                        <div style={{ fontSize: 13, opacity: 0.9, marginBottom: 6 }}>
                          {l.bedrooms} bd • {fmtBaths(l.bathrooms)} ba
                        </div>
                        <div style={{ fontSize: 12, opacity: 0.85, marginBottom: 10 }}>
                          {hasYear && <>Built {l.yearBuilt}</>}
                          {hasYear && hasSqft && " • "}
                          {hasSqft && <>{fmtInt(l.squareFootage)} sqft</>}
                          {(hasYear || hasSqft) && " • "}
                          {d === 0 ? "Listed today" : d === 1 ? "Listed 1 day ago" : `Listed ${d} days ago`}
                        </div>
                        <a
                          href={`https://www.google.com/search?q=${encodeURI(l.guid)}`}
                          target="_blank"
                          rel="noreferrer"
                          style={{ textDecoration: "underline" }}
                        >
                          Google listing →
                        </a>
                      </>
                    );
                  })()
                )}
              </div>
            </InfoWindow>
          );
        })()}
      </Map>
    </div>
  );
}

export default function App() {
  return (
    <APIProvider apiKey={import.meta.env.VITE_GOOGLE_MAPS_API_KEY}>
      <ListingsMap />
    </APIProvider>
  );
}
