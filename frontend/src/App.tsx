import {
  Marker,
  APIProvider,
  Map,
  InfoWindow,
} from "@vis.gl/react-google-maps";
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
          .filter(Boolean)
      )
    );
    return list.length ? list : DEFAULTS.counties ?? null;
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

function ListingsMap() {
  const [isDark, setIsDark] = useState(false);
  const [listings, setListings] = useState<Listing[]>([]);
  const [isLoading, setLoading] = useState(true);
  const [selected, setSelected] = useState<Listing | null>(null);
  const [filters, setFilters] = useState<Filters>(() =>
    typeof window === "undefined"
      ? DEFAULTS
      : parseFiltersFromSearch(window.location.search)
  );
  const visibleCount = useVisibleListingCount(listings);

  const listingApi = useMemo(() => listingsApi(), []);
  const lat = parseFloat(import.meta.env.VITE_DEFAULT_LATITUDE);
  const lng = parseFloat(import.meta.env.VITE_DEFAULT_LONGITUDE);
  const handleClick = useCallback((l: Listing) => () => setSelected(l), []);
  const refetch = useRef<ReturnType<typeof setTimeout> | null>(null);
  const count = useMemo(() => listings.length, [listings]);
  const [filterBarHeight, setFilterBarHeight] = useState(0);
  const filterBarRef = useRef<HTMLDivElement>(null);

  const fmtInt = (n: number) => Math.round(n).toLocaleString();
  const fmtBaths = (n: number) => (Number.isInteger(n) ? `${n}` : n.toFixed(1));
  const hasYear = selected?.yearBuilt !== -1;
  const hasSqft = selected?.squareFootage !== -1;
  const listedDate = new Date(selected?.listedDate ?? "");
  const daysOld = Math.floor(
    (Date.now() - listedDate.getTime()) / (1000 * 60 * 60 * 24)
  );

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

  const mapStyles = useMemo(
    () => (isDark ? nightModeMapStyles : null),
    [isDark]
  );

  const markers = useMemo(
    () =>
      listings.map((listing) => {
        const hasYear = listing.yearBuilt !== -1;
        const hasSqft = listing.squareFootage !== -1;
        let color = "#cad411";
        if (hasYear && hasSqft) color = "#EA4335";
        else if (hasYear || hasSqft) color = "#d49a11";

        return (
          <Marker
            key={listing.id}
            position={{ lat: listing.latitude, lng: listing.longitude }}
            onClick={handleClick(listing)}
            label={{
              text: `$${listing.price}`,
              fontSize: "12px",
              color: isDark ? "white" : "#111827",
              className: "price-label",
            }}
            icon={{
              path: "M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z",
              fillColor: color,
              fillOpacity: 1,
              strokeColor: "#ffffff",
              strokeWeight: 1,
              scale: 1.5,
              anchor: new google.maps.Point(12, 24),
              labelOrigin: new google.maps.Point(12, 9),
            }}
          />
        );
      }),
    [listings, isDark, handleClick]
  );

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
        styles={mapStyles}
        defaultCenter={{
          lat: lat,
          lng: lng,
        }}
        defaultZoom={parseInt(import.meta.env.VITE_DEFAULT_ZOOM)}
        gestureHandling="greedy"
        disableDefaultUI
      >
        {markers}
        {selected && (
          <InfoWindow
            pixelOffset={[0, -28]}
            position={{ lat: selected.latitude, lng: selected.longitude }}
            onCloseClick={() => setSelected(null)}
          >
            <div style={{ minWidth: 220, lineHeight: 1.25, color: "black" }}>
              {/* Price */}
              <div
                style={{
                  fontWeight: 700,
                  fontSize: 16,
                  marginBottom: 6,
                }}
              >
                ${fmtInt(selected.price)}
              </div>

              {/* Beds / Baths */}
              <div style={{ fontSize: 13, opacity: 0.9, marginBottom: 6 }}>
                {selected.bedrooms} bd • {fmtBaths(selected.bathrooms)} ba
              </div>

              {/* Address */}
              <div style={{ marginBottom: 6 }}>
                {selected.addressLine1}, {selected.city}, {selected.state}
              </div>

              {/* Meta row (Built year + Sqft + Days old) */}
              <div style={{ fontSize: 12, opacity: 0.85, marginBottom: 10 }}>
                {hasYear && <>Built {selected.yearBuilt}</>}
                {hasYear && hasSqft && " • "}
                {hasSqft && <>{fmtInt(selected.squareFootage)} sqft</>}
                {(hasYear || hasSqft) && " • "}
                {/* Always show days old */}
                {daysOld === 0
                  ? "Listed today"
                  : daysOld === 1
                  ? "Listed 1 day ago"
                  : `Listed ${daysOld} days ago`}
              </div>

              {/* Link */}
              <a
                href={`https://www.google.com/search?q=${encodeURI(
                  selected.guid
                )}`}
                target="_blank"
                rel="noreferrer"
                style={{
                  textDecoration: "underline",
                }}
              >
                Google listing →
              </a>
            </div>
          </InfoWindow>
        )}
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

const nightModeMapStyles = [
  { elementType: "geometry", stylers: [{ color: "#242f3e" }] },
  { elementType: "labels.text.stroke", stylers: [{ color: "#242f3e" }] },
  { elementType: "labels.text.fill", stylers: [{ color: "#746855" }] },
  {
    featureType: "administrative.locality",
    elementType: "labels.text.fill",
    stylers: [{ color: "#d59563" }],
  },
  {
    featureType: "poi",
    elementType: "labels.text.fill",
    stylers: [{ color: "#d59563" }],
  },
  {
    featureType: "poi.park",
    elementType: "geometry",
    stylers: [{ color: "#263c3f" }],
  },
  {
    featureType: "poi.park",
    elementType: "labels.text.fill",
    stylers: [{ color: "#6b9a76" }],
  },
  {
    featureType: "road",
    elementType: "geometry",
    stylers: [{ color: "#38414e" }],
  },
  {
    featureType: "road",
    elementType: "geometry.stroke",
    stylers: [{ color: "#212a37" }],
  },
  {
    featureType: "road",
    elementType: "labels.text.fill",
    stylers: [{ color: "#9ca5b3" }],
  },
  {
    featureType: "road.highway",
    elementType: "geometry",
    stylers: [{ color: "#746855" }],
  },
  {
    featureType: "road.highway",
    elementType: "geometry.stroke",
    stylers: [{ color: "#1f2835" }],
  },
  {
    featureType: "road.highway",
    elementType: "labels.text.fill",
    stylers: [{ color: "#f3d19c" }],
  },
  {
    featureType: "transit",
    elementType: "geometry",
    stylers: [{ color: "#2f3948" }],
  },
  {
    featureType: "transit.station",
    elementType: "labels.text.fill",
    stylers: [{ color: "#d59563" }],
  },
  {
    featureType: "water",
    elementType: "geometry",
    stylers: [{ color: "#17263c" }],
  },
  {
    featureType: "water",
    elementType: "labels.text.fill",
    stylers: [{ color: "#515c6d" }],
  },
  {
    featureType: "water",
    elementType: "labels.text.stroke",
    stylers: [{ color: "#17263c" }],
  },
];
