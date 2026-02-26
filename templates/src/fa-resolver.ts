/**
 * Font Awesome resolver for “digital business card” links.
 *
 * ✅ Derives:
 *  - title (human label)
 *  - kind (semantic category)
 *  - href (normalized)
 *  - Font Awesome classes (style + icon)
 *  - brand color (CSS var fallback)
 *  - aria-label
 *
 * ✅ Supports:
 *  - plain strings or objects in JSON
 *  - email/phone detection with or without schemes
 *  - socials by hostname
 *  - per-theme icon sets
 *  - per-theme color sets
 *  - custom domain overrides
 *  - Font Awesome 6 (solid/brands)
 *
 * You can paste this into a .ts file OR use as JS (remove types).
 */

export type ThemeName = "light" | "dark" | "mono";

export type LinkInput =
  | string
  | {
      url: string;
      title?: string;
      kind?: Kind;
    };

export type Kind =
  | "email"
  | "phone"
  | "whatsapp"
  | "map"
  | "website"
  | "link"
  | "linkedin"
  | "instagram"
  | "facebook"
  | "x"
  | "twitter"
  | "youtube"
  | "tiktok"
  | "github"
  | "stackoverflow"
  | "pinterest"
  | "reddit"
  | "snapchat"
  | "twitch"
  | "vimeo"
  | "spotify"
  | "soundcloud"
  | "paypal"
  | "telegram"
  | "discord"
  | "signal";

export type FAStyle = "fa-solid" | "fa-brands" | "fa-regular";
export type ResolvedFA = { style: FAStyle; icon: string };

export type ResolvedLink = {
  url: string;        // original
  href: string;       // normalized for <a href>
  kind: Kind;
  title: string;
  fa: ResolvedFA;
  color: string;      // hex or css var (e.g. "var(--social-linkedin)")
  ariaLabel: string;
};

export type ResolverOptions = {
  theme?: ThemeName;

  /**
   * Optional: override detected kind/title/fa/color based on domain.
   * Keys can be domains ("my-company.com") or substring patterns ("forms.gle").
   */
  domainOverrides?: Record<
    string,
    Partial<Pick<ResolvedLink, "kind" | "title"> & { fa: Partial<ResolvedFA>; color: string }>
  >;

  /**
   * Optional: if you want to return CSS vars by default (recommended),
   * keep this true. If false, returns hex values where available.
   */
  preferCssVars?: boolean;
};

/** -----------------------
 *  Default maps
 *  ----------------------*/

// Hostname -> Kind (social detection)
const HOST_KIND_MAP: Record<string, Kind> = {
  "linkedin.com": "linkedin",
  "instagram.com": "instagram",
  "facebook.com": "facebook",
  "x.com": "x",
  "twitter.com": "twitter",
  "youtube.com": "youtube",
  "youtu.be": "youtube",
  "tiktok.com": "tiktok",
  "github.com": "github",
  "stackoverflow.com": "stackoverflow",
  "pinterest.com": "pinterest",
  "reddit.com": "reddit",
  "snapchat.com": "snapchat",
  "twitch.tv": "twitch",
  "vimeo.com": "vimeo",
  "spotify.com": "spotify",
  "soundcloud.com": "soundcloud",
  "paypal.me": "paypal",
  "paypal.com": "paypal",
  "t.me": "telegram",
  "telegram.me": "telegram",
  "discord.gg": "discord",
  "discord.com": "discord",
  "signal.me": "signal",
  "wa.me": "whatsapp",
  "whatsapp.com": "whatsapp"
};

// Default titles by kind (can be overridden)
const DEFAULT_TITLE: Record<Kind, string> = {
  email: "Email",
  phone: "Call",
  whatsapp: "WhatsApp",
  map: "Location",
  website: "Website",
  link: "Link",

  linkedin: "LinkedIn",
  instagram: "Instagram",
  facebook: "Facebook",
  x: "X",
  twitter: "Twitter",
  youtube: "YouTube",
  tiktok: "TikTok",
  github: "GitHub",
  stackoverflow: "Stack Overflow",
  pinterest: "Pinterest",
  reddit: "Reddit",
  snapchat: "Snapchat",
  twitch: "Twitch",
  vimeo: "Vimeo",
  spotify: "Spotify",
  soundcloud: "SoundCloud",
  paypal: "PayPal",
  telegram: "Telegram",
  discord: "Discord",
  signal: "Signal"
};

// Per-theme icon sets (semantic kind -> FA style/icon)
const ICON_SETS: = {

    email: { style: "fa-solid", icon: "fa-envelope" },
    phone: { style: "fa-solid", icon: "fa-phone" },
    whatsapp: { style: "fa-brands", icon: "fa-whatsapp" },
    map: { style: "fa-solid", icon: "fa-location-dot" },
    website: { style: "fa-solid", icon: "fa-globe" },
    link: { style: "fa-solid", icon: "fa-link" },
    linkedin: { style: "fa-brands", icon: "fa-linkedin" },
    instagram: { style: "fa-brands", icon: "fa-instagram" },
    facebook: { style: "fa-brands", icon: "fa-facebook" },
    x: { style: "fa-brands", icon: "fa-x-twitter" }, // FA6 uses fa-x-twitter
    twitter: { style: "fa-brands", icon: "fa-twitter" },
    youtube: { style: "fa-brands", icon: "fa-youtube" },
    tiktok: { style: "fa-brands", icon: "fa-tiktok" },
    github: { style: "fa-brands", icon: "fa-github" },
    stackoverflow: { style: "fa-brands", icon: "fa-stack-overflow" },
    pinterest: { style: "fa-brands", icon: "fa-pinterest" },
    reddit: { style: "fa-brands", icon: "fa-reddit" },
    snapchat: { style: "fa-brands", icon: "fa-snapchat" },
    twitch: { style: "fa-brands", icon: "fa-twitch" },
    vimeo: { style: "fa-brands", icon: "fa-vimeo" },
    spotify: { style: "fa-brands", icon: "fa-spotify" },
    soundcloud: { style: "fa-brands", icon: "fa-soundcloud" },
    paypal: { style: "fa-brands", icon: "fa-paypal" },
    telegram: { style: "fa-brands", icon: "fa-telegram" },
    discord: { style: "fa-brands", icon: "fa-discord" },
    signal: { style: "fa-brands", icon: "fa-signal-messenger" }
  }
};

// Dark/Mono inherit icons from light unless you want different ones
ICON_SETS.dark = { ...ICON_SETS.light };
ICON_SETS.mono = { ...ICON_SETS.light };

// Brand colors (hex) — you can choose to use CSS vars instead
const BRAND_HEX: Partial<Record<Kind, string>> = {
  whatsapp: "#25D366",
  linkedin: "#0A66C2",
  facebook: "#1877F2",
  instagram: "#E4405F", // flat fallback; IG is gradient in reality
  x: "#000000",
  twitter: "#1DA1F2",
  youtube: "#FF0000",
  tiktok: "#000000",
  github: "#181717",
  stackoverflow: "#F58025",
  pinterest: "#BD081C",
  reddit: "#FF4500",
  snapchat: "#FFFC00",
  twitch: "#9146FF",
  vimeo: "#1AB7EA",
  spotify: "#1DB954",
  soundcloud: "#FF5500",
  paypal: "#003087",
  telegram: "#26A5E4",
  discord: "#5865F2",
  signal: "#3A76F0",
  email: "#6B7280",
  phone: "#10B981",
  website: "#2563EB",
  map: "#EA4335",
  link: "#6B7280"
};

// Per-theme colors: use CSS vars by default; mono makes everything neutral
const COLOR_SETS: Record<ThemeName, Partial<Record<Kind, string>>> = {
  light: {},
  dark: {},
  mono: {}
};

// Light/Dark: map to CSS variables (recommended)
for (const k of Object.keys(DEFAULT_TITLE) as Kind[]) {
  COLOR_SETS.light[k] = `var(--social-${k})`;
  COLOR_SETS.dark[k] = `var(--social-${k})`;
}
// Mono: neutral
for (const k of Object.keys(DEFAULT_TITLE) as Kind[]) {
  COLOR_SETS.mono[k] = "var(--social-mono)";
}

/** -----------------------
 *  Public API
 *  ----------------------*/

export function resolveLinks(
  inputs: LinkInput[],
  opts: ResolverOptions = {}
): ResolvedLink[] {
  const theme: ThemeName = opts.theme ?? "light";
  const preferCssVars = opts.preferCssVars ?? true;

  return inputs
    .map((x) => (typeof x === "string" ? { url: x } : x))
    .filter((x) => !!x.url && String(x.url).trim().length > 0)
    .map((x) => resolveOne(x.url, x, { theme, preferCssVars, domainOverrides: opts.domainOverrides }));
}

export function resolveOne(
  url: string,
  input: Partial<{ title: string; kind: Kind }> = {},
  opts: (ResolverOptions & { theme: ThemeName; preferCssVars: boolean }) | ResolverOptions = {}
): ResolvedLink {
  const theme: ThemeName = (opts.theme ?? "light") as ThemeName;
  const preferCssVars = opts.preferCssVars ?? true;
  const original = url.trim();

  // 1) detect kind + normalized href
  const detected = detectKindAndHref(original);

  // allow explicit kind override from JSON input
  const kind: Kind = (input?.kind ?? detected.kind) as Kind;

  // 2) domain override (optional)
  const override = findOverride(original, opts.domainOverrides);

  // 3) title: input override > override.title > derived default (with nice email option)
  const derivedTitle = deriveTitle(kind, original);
  const title = cleanText(input?.title) || cleanText(override?.title) || derivedTitle;

  // 4) icon: theme set + override icon/style
  const iconSet = ICON_SETS[theme];
  const baseFa = iconSet[kind] ?? iconSet.link ?? { style: "fa-solid", icon: "fa-link" };

  const fa: ResolvedFA = {
    style: (override?.fa?.style as FAStyle) || baseFa.style,
    icon: override?.fa?.icon || baseFa.icon
  };

  // 5) color: prefer CSS vars -> theme colors; else hex fallback
  const themeColors = COLOR_SETS[theme] ?? COLOR_SETS.light;
  const cssVarColor = override?.color || themeColors[kind] || `var(--social-link)`;
  const hexColor = override?.color || BRAND_HEX[kind] || BRAND_HEX.link || "#6B7280";
  const color = preferCssVars ? cssVarColor : hexColor;


  // 6) aria label
  const ariaLabel = buildAriaLabel(kind, title, detected.href);

  // 7) href: override may want to keep detected href; input doesn't override href here
  const href = detected.href;

  return {
    url: original,
    href,
    kind,
    title,
    fa,
    color,
    ariaLabel
  };
}

/** -----------------------
 *  Detection helpers
 *  ----------------------*/

function detectKindAndHref(url: string): { kind: Kind; href: string } {
  const u = url.trim();
  const lower = u.toLowerCase();

  // mailto
  if (lower.startsWith("mailto:")) {
    const email = u.slice(7).trim();
    return { kind: "email", href: `mailto:${email}` };
  }

  // tel
  if (lower.startsWith("tel:")) {
    const phone = u.slice(4).trim();
    const digits = extractDigits(phone) || phone;
    return { kind: "phone", href: `tel:${digits}` };
  }

  // WhatsApp URLs
  if (lower.includes("wa.me/") || lower.includes("whatsapp.com")) {
    const digits = extractDigits(u);
    return { kind: "whatsapp", href: digits ? `https://wa.me/${digits}` : u };
  }

  // Maps
  if (
    lower.includes("google.com/maps") ||
    lower.includes("maps.google") ||
    lower.includes("goo.gl/maps")
  ) {
    return { kind: "map", href: u };
  }

  // Plain email without scheme
  if (looksLikeEmail(u)) {
    return { kind: "email", href: `mailto:${u}` };
  }

  // Plain phone without scheme
  if (looksLikePhone(u)) {
    const digits = extractDigits(u) || u;
    return { kind: "phone", href: `tel:${digits}` };
  }

  // Try hostname detection (socials / website)
  const host = safeHostname(u);
  const kindFromHost = hostToKind(host, lower);

  if (kindFromHost && kindFromHost !== "website") {
    return { kind: kindFromHost, href: normalizeHttp(u) };
  }

  // default website/link
  return { kind: host ? "website" : "link", href: normalizeHttp(u) };
}

function hostToKind(host: string, lowerUrl: string): Kind | null {
  const h = (host || "").toLowerCase();
  if (!h) return null;

  // direct match / substring match
  for (const domain of Object.keys(HOST_KIND_MAP)) {
    if (h === domain || h.endsWith("." + domain) || lowerUrl.includes(domain)) {
      return HOST_KIND_MAP[domain];
    }
  }
  return "website";
}

function normalizeHttp(url: string): string {
  // Keep original if it already has a scheme
  if (/^[a-z][a-z0-9+\-.]*:\/\//i.test(url)) return url;
  // Allow protocol-relative
  if (url.startsWith("//")) return "https:" + url;
  return "https://" + url;
}

function safeHostname(url: string): string {
  try {
    return new URL(url).hostname;
  } catch {
    try {
      return new URL("https://" + url).hostname;
    } catch {
      return "";
    }
  }
}

function looksLikeEmail(v: string): boolean {
  const s = v.trim();
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(s);
}

function looksLikePhone(v: string): boolean {
  const digits = extractDigits(v);
  return digits.length >= 9 && digits.length <= 15;
}

function extractDigits(v: string): string {
  return String(v || "").replace(/\D/g, "");
}

/** -----------------------
 *  Title + overrides
 *  ----------------------*/

function deriveTitle(kind: Kind, url: string): string {
  // Show actual email for mailto when short enough
  if (kind === "email") {
    const email = url.toLowerCase().startsWith("mailto:") ? url.slice(7).trim() : url.trim();
    return email.length > 0 && email.length <= 32 ? email : DEFAULT_TITLE.email;
  }

  // For website, use hostname if possible
  if (kind === "website" || kind === "link") {
    const host = safeHostname(url) || "";
    const pretty = host.replace(/^www\./i, "");
    return pretty || (kind === "website" ? DEFAULT_TITLE.website : DEFAULT_TITLE.link);
  }

  return DEFAULT_TITLE[kind] || "Link";
}

function buildAriaLabel(kind: Kind, title: string, href: string): string {
  // Keep short and descriptive for screen readers
  if (kind === "email") return `Email: ${title}`;
  if (kind === "phone") return `Call`;
  if (kind === "map") return `Open location`;
  if (kind === "website") return `Open ${title}`;
  return `Open ${title}`;
}

function cleanText(v?: string): string {
  return (v ?? "").trim();
}

function findOverride(
  url: string,
  overrides?: ResolverOptions["domainOverrides"]
) {
  if (!overrides) return null;
  const lower = url.toLowerCase();

  // Longest key wins (more specific)
  const keys = Object.keys(overrides).sort((a, b) => b.length - a.length);
  for (const k of keys) {
    if (lower.includes(k.toLowerCase())) return overrides[k];
  }
  return null;
}

/** -----------------------
 *  CSS tokens helper
 *  ----------------------*/

/**
 * Generates :root CSS variables for all supported kinds.
 * - If preferCssVars = true, returns variables pointing to hex defaults (useful bootstrap).
 * - Mono var is included.
 */
export function buildSocialCssVariables(): string {
  const kinds = Object.keys(DEFAULT_TITLE) as Kind[];

  const lines: string[] = [];
  lines.push(":root {");
  lines.push("  /* Social / action colors */");

  // mono
  lines.push(`  --social-mono: #6B7280;`);

  for (const k of kinds) {
    const hex = BRAND_HEX[k] || "#6B7280";
    lines.push(`  --social-${k}: ${hex};`);
  }

  lines.push("}");
  return lines.join("\n");
}

/**
 * Optional: validate your Font Awesome icons exist in your kit/version.
 * (This is a soft check: you can log these and test in browser.)
 */
export function listIconsForTheme(theme: ThemeName = "light"): ResolvedFA[] {
  const set = ICON_SETS[theme] ?? ICON_SETS.light;
  const unique = new Map<string, ResolvedFA>();
  for (const k of Object.keys(DEFAULT_TITLE) as Kind[]) {
    const fa = set[k];
    if (fa) unique.set(`${fa.style}|${fa.icon}`, fa);
  }
  return Array.from(unique.values());
}
