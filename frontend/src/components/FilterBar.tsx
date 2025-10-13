import { useEffect, useId, useMemo, useRef, useState } from "react";
import "./FilterBar.css";
import type { Filters } from "../models/Filters";

type FilterBarProps = {
  value: Filters;
  onChange: (next: Filters) => void;
  className?: string;
};

type MenuKey = "price" | "beds" | "baths" | "days";

const PRICE_PRESETS = [
  { label: "Any", min: null, max: null },
  { label: "≤ $2500", min: null, max: 2500 },
  { label: "≤ $3000", min: null, max: 3000 },
  { label: "≤ $3500", min: null, max: 3500 },
];

const BED_CHOICES = [null, 1, 2, 3, 4, 5] as const;
const BATH_CHOICES = [null, 1, 1.5, 2, 2.5, 3] as const;

const DAYS_CHOICES = [
  { label: "Any", value: null },
  { label: "1 day", value: 1 },
  { label: "7 days", value: 7 },
  { label: "14 days", value: 14 },
  { label: "30 days", value: 30 },
  { label: "90 days", value: 90 },
];

function fmtMoney(n: number | null | undefined) {
  if (n == null) return "";
  if (n >= 1_000_000)
    return `$${(n / 1_000_000).toFixed(n % 1_000_000 ? 1 : 0)}M`;
  if (n >= 1_000) return `$${(n / 1_000).toFixed(n % 1_000 ? 1 : 0)}k`;
  return `$${n}`;
}
function pillLabelPrice(
  min: number | null | undefined,
  max: number | null | undefined
) {
  if (!min && !max) return "Price";
  if (min && max) return `${fmtMoney(min)}–${fmtMoney(max)}`;
  if (min) return `${fmtMoney(min)}+`;
  return `≤ ${fmtMoney(max!)}`;
}
function pillLabelBeds(minBeds: number | null | undefined) {
  if (!minBeds) return "Beds";
  return `${minBeds}+ bd`;
}
function pillLabelBaths(minBaths: number | null | undefined) {
  if (!minBaths) return "Baths";
  return `${minBaths}+ ba`;
}
function pillLabelDays(d: number | null | undefined) {
  if (!d) return "Days";
  return `≤ ${d}d`;
}

function useClickOutside<T extends HTMLElement>(onOutside: () => void) {
  const ref = useRef<T | null>(null);
  useEffect(() => {
    function handler(e: MouseEvent) {
      if (ref.current && !ref.current.contains(e.target as Node)) onOutside();
    }
    document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, [onOutside]);
  return ref;
}

export default function FilterBar({
  value,
  onChange,
  className,
}: FilterBarProps) {
  const [openMenu, setOpenMenu] = useState<MenuKey | null>(null);

  const [draft, setDraft] = useState<Filters>(value);

  useEffect(() => {
    setDraft(value);
  }, [value]);

  const rootRef = useClickOutside<HTMLDivElement>(() => setOpenMenu(null));

  function open(key: MenuKey) {
    setDraft(value);
    setOpenMenu((k) => (k === key ? null : key));
  }

  function applyDraft() {
    onChange(draft);
    setOpenMenu(null);
  }

  function clearKey<K extends keyof Filters>(...keys: K[]) {
    const next = { ...draft };
    keys.forEach((k) => (next[k] = null));
    setDraft(next);
  }

  const pillsActive = useMemo(() => {
    const act: Partial<Record<MenuKey, boolean>> = {};
    act.price = !!(value.minPrice || value.maxPrice);
    act.beds = !!value.minBedrooms;
    act.baths = !!value.minBathrooms;
    act.days = value.maxDaysOld != null;
    return act;
  }, [value]);

  const idBase = useId();

  const isMobile =
    typeof window !== "undefined" && window.matchMedia
      ? window.matchMedia("(max-width: 767.98px)").matches
      : false;

  return (
    <div ref={rootRef} className={`zbar ${className ?? ""}`}>
      <div className="zbar-inner">
        {openMenu && (
          <div className={`backdrop show`} onClick={() => setOpenMenu(null)} />
        )}
        {/* PRICE */}
        <div className={`pill ${pillsActive.price ? "active" : ""}`}>
          <button
            aria-controls={`${idBase}-price`}
            aria-expanded={openMenu === "price"}
            onClick={() => open("price")}
          >
            {pillLabelPrice(value.minPrice ?? null, value.maxPrice ?? null)}
          </button>
          {openMenu === "price" && (
            <div
              className={`menu ${isMobile ? "sheet" : ""}`}
              id={`${idBase}-price`}
              role="dialog"
              aria-label="Price filter"
            >
              <div className="menu-row">
                <div className="field">
                  <label>Min</label>
                  <input
                    inputMode="numeric"
                    placeholder="$ Min"
                    value={draft.minPrice ?? ""}
                    onChange={(e) =>
                      setDraft((d) => ({
                        ...d,
                        minPrice: e.target.value
                          ? Number(e.target.value.replace(/[^\d]/g, ""))
                          : null,
                      }))
                    }
                  />
                </div>
                <div className="sep">—</div>
                <div className="field">
                  <label>Max</label>
                  <input
                    inputMode="numeric"
                    placeholder="$ Max"
                    value={draft.maxPrice ?? ""}
                    onChange={(e) =>
                      setDraft((d) => ({
                        ...d,
                        maxPrice: e.target.value
                          ? Number(e.target.value.replace(/[^\d]/g, ""))
                          : null,
                      }))
                    }
                  />
                </div>
              </div>

              <div className="chips">
                {PRICE_PRESETS.map((p) => (
                  <button
                    key={p.label}
                    className="chip"
                    onClick={() =>
                      setDraft((d) => ({
                        ...d,
                        minPrice: p.min,
                        maxPrice: p.max,
                      }))
                    }
                  >
                    {p.label}
                  </button>
                ))}
              </div>

              <div className="menu-actions">
                <button
                  className="link"
                  onClick={() => clearKey("minPrice", "maxPrice")}
                >
                  Clear
                </button>
                <div className="spacer" />
                <button className="apply" onClick={applyDraft}>
                  Apply
                </button>
              </div>
            </div>
          )}
        </div>

        {/* BEDS */}
        <div className={`pill ${pillsActive.beds ? "active" : ""}`}>
          <button
            aria-controls={`${idBase}-beds`}
            aria-expanded={openMenu === "beds"}
            onClick={() => open("beds")}
          >
            {pillLabelBeds(value.minBedrooms ?? null)}
          </button>
          {openMenu === "beds" && (
            <div
              className={`menu ${isMobile ? "sheet" : ""}`}
              id={`${idBase}-beds`}
              role="dialog"
              aria-label="Beds filter"
            >
              <div className="chips wrap">
                {BED_CHOICES.map((b, i) => (
                  <button
                    key={String(b) + i}
                    className={`chip ${
                      draft.minBedrooms === b ? "selected" : ""
                    }`}
                    onClick={() => setDraft((d) => ({ ...d, minBedrooms: b }))}
                  >
                    {b == null ? "Any" : `${b}+ bd`}
                  </button>
                ))}
              </div>
              <div className="menu-actions">
                <button
                  className="link"
                  onClick={() => clearKey("minBedrooms")}
                >
                  Clear
                </button>
                <div className="spacer" />
                <button className="apply" onClick={applyDraft}>
                  Apply
                </button>
              </div>
            </div>
          )}
        </div>

        {/* BATHS */}
        <div className={`pill ${pillsActive.baths ? "active" : ""}`}>
          <button
            aria-controls={`${idBase}-baths`}
            aria-expanded={openMenu === "baths"}
            onClick={() => open("baths")}
          >
            {pillLabelBaths(value.minBathrooms ?? null)}
          </button>
          {openMenu === "baths" && (
            <div
              className={`menu ${isMobile ? "sheet" : ""}`}
              id={`${idBase}-baths`}
              role="dialog"
              aria-label="Baths filter"
            >
              <div className="chips wrap">
                {BATH_CHOICES.map((b, i) => (
                  <button
                    key={String(b) + i}
                    className={`chip ${
                      draft.minBathrooms === b ? "selected" : ""
                    }`}
                    onClick={() => setDraft((d) => ({ ...d, minBathrooms: b }))}
                  >
                    {b == null ? "Any" : `${b}+ ba`}
                  </button>
                ))}
              </div>
              <div className="menu-actions">
                <button
                  className="link"
                  onClick={() => clearKey("minBathrooms")}
                >
                  Clear
                </button>
                <div className="spacer" />
                <button className="apply" onClick={applyDraft}>
                  Apply
                </button>
              </div>
            </div>
          )}
        </div>

        {/* DAYS */}
        <div className={`pill ${pillsActive.days ? "active" : ""}`}>
          <button
            aria-controls={`${idBase}-days`}
            aria-expanded={openMenu === "days"}
            onClick={() => open("days")}
          >
            {pillLabelDays(value.maxDaysOld ?? null)}
          </button>
          {openMenu === "days" && (
            <div
              className={`menu ${isMobile ? "sheet" : ""}`}
              id={`${idBase}-days`}
              role="dialog"
              aria-label="Days on market filter"
            >
              <div className="chips wrap">
                {DAYS_CHOICES.map((d) => (
                  <button
                    key={String(d.value) + d.label}
                    className={`chip ${
                      draft.maxDaysOld === d.value ? "selected" : ""
                    }`}
                    onClick={() =>
                      setDraft((s) => ({ ...s, maxDaysOld: d.value }))
                    }
                  >
                    {d.label}
                  </button>
                ))}
              </div>

              <div className="menu-row">
                <div className="field grow">
                  <label>Custom (days ≤)</label>
                  <input
                    inputMode="numeric"
                    placeholder="e.g., 21"
                    value={draft.maxDaysOld ?? ""}
                    onChange={(e) =>
                      setDraft((s) => ({
                        ...s,
                        maxDaysOld: e.target.value
                          ? Number(e.target.value.replace(/[^\d]/g, ""))
                          : null,
                      }))
                    }
                  />
                </div>
              </div>

              <div className="menu-actions">
                <button className="link" onClick={() => clearKey("maxDaysOld")}>
                  Clear
                </button>
                <div className="spacer" />
                <button className="apply" onClick={applyDraft}>
                  Apply
                </button>
              </div>
            </div>
          )}
        </div>

        {/* CLEAR ALL (appears only when any pill is active) */}
        {(pillsActive.price ||
          pillsActive.beds ||
          pillsActive.baths ||
          pillsActive.days) && (
          <button
            className="clear-all"
            onClick={() =>
              onChange({
                minPrice: null,
                maxPrice: null,
                minBedrooms: null,
                minBathrooms: null,
                maxDaysOld: null,
              })
            }
          >
            Clear all
          </button>
        )}
      </div>
    </div>
  );
}
