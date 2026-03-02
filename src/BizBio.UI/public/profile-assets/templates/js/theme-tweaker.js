/* theme-tweaker.js
 * - Injects ./css/theme-tweaker.css
 * - Fetches ./partials/theme-tweaker.html and mounts after DOM ready
 * - Uses Alpine to render standard fields from a 1-line registry
 * - Keeps your gradient controls (BG + CTA) with dial controls
 */

(() => {
  if (window.__ThemeTweakerMounted) return;
  window.__ThemeTweakerMounted = true;

  const BASE_STORAGE_KEY = "themeTweaker:v3";

  const CFG = {
    enabled: true,
    defaultOpen: false,
    position: "right", // "right" | "left"

    cssUrl: "./css/theme-tweaker.css",
    htmlTemplateUrl: "./partials/theme-tweaker.html",

    storageKey: null,

    fonts: [
      { label: "Inter", value: "Inter", google: true },
      { label: "System UI", value: "system-ui" },
      { label: "Roboto", value: "Roboto", google: true },
      { label: "Open Sans", value: "Open Sans", google: true },
      { label: "Poppins", value: "Poppins", google: true },
      { label: "Montserrat", value: "Montserrat", google: true },
      { label: "Source Sans 3", value: "Source Sans 3", google: true },
      { label: "Merriweather", value: "Merriweather", google: true }
    ],

    socialBrandHex: {
      whatsapp: "#25D366",
      linkedin: "#0A66C2",
      instagram: "#E4405F",
      facebook: "#1877F2",
      x: "#000000",
      twitter: "#1DA1F2",
      youtube: "#FF0000",
      tiktok: "#000000",
      github: "#181717",
      stackoverflow: "#F58025",
      website: "#2563EB",
      email: "#6B7280",
      phone: "#10B981",
      map: "#EA4335",
      link: "#6B7280"
    }
  };

  // allow overrides
  const userCfg =
    window.ThemeTweakerConfig && typeof window.ThemeTweakerConfig === "object"
      ? window.ThemeTweakerConfig
      : {};
  Object.assign(CFG, userCfg);

  if (!CFG.enabled) return;

  const $ = (sel, root = document) => root.querySelector(sel);

  // ----------------------------
  // 1-LINE REGISTRATION HERE
  // ----------------------------
  const TT_FIELDS = [
    // selects
    { key: "mode", kind: "select", label: "Mode", options: ["light", "dark"], default: "light" },
    { key: "font", kind: "select", label: "Font", optionsFromCfg: "fonts", default: "Inter" },
    { key: "buttonShape", kind: "select", label: "Button shape", options: ["square","rounded-sm","rounded-md","rounded-lg","pill","circle"], default: "pill" },
    { key: "socialColorMode", kind: "select", label: "Social colors", options: ["brand","unified"], default: "brand" },

    // colors
    { key: "text", cssVar: "--text-color", kind: "color", label: "Text color", default: "#1e293b", derive: true },
    { key: "primary", cssVar: "--primary-color", kind: "color", label: "Primary", default: "#1e40af", derive: true },
    { key: "secondary", cssVar: "--secondary-color", kind: "color", label: "Secondary", default: "#3b82f6", derive: true },
    { key: "cardBg", cssVar: "--card-bg-color", kind: "color", label: "Card BG", default: "#ffffff", derive: true },
    { key: "avatarBorder", cssVar: "--avatar-border-color", kind: "color", label: "Avatar border", default: "#ffffff", derive: true },

    { key: "actionBtnBg", cssVar: "--action-button-bg-color", kind: "color", label: "Action btn BG", default: "#1e40af", derive: true },
    { key: "actionBtnText", cssVar: "--action-button-text-color", kind: "color", label: "Action btn text", default: "#ffffff", derive: true },

    { key: "infoIconBg", cssVar: "--info-item-icon-bg-color", kind: "color", label: "Info icon BG", default: "#1e40af", derive: true },
    { key: "infoIconText", cssVar: "--info-item-icon-text-color", kind: "color", label: "Info icon text", default: "#ffffff", derive: true },

    { key: "headerColor", cssVar: "--header-color", kind: "color", label: "Header color", default: "#ffffff", derive: true },

    { key: "socialUnifiedColor", cssVar: "--social-unified-color", kind: "color", label: "Unified social color", default: "#1e40af" },

    // ✅ your new variable (add more like this = batch add)
    { key: "taglineText", cssVar: "--tagline-text-color", kind: "color", label: "Tagline text", default: "#ffffff", derive: true }
  ];

  const TT_SECTIONS = [
  {
    section: "Global",
    rows: [
      {
        layout: "single",
        fields: [
          { key: "mode", kind: "select", label: "Mode", options: ["light", "dark"], default: "light" },
        ]
      },
      {
        layout: "single",
        fields: [
          { key: "font", kind: "select", label: "Font", optionsFromCfg: "fonts", default: "Inter" },
        ]
      },
      {
        layout: "single",
        fields: [
          { key: "bg", cssVar: "--page-background-color", kind: "color", label: "Page Background", default: "#111827", derive: true },
        ]
      },
      
      {
        layout: "pair",
        fields: [
          { key: "primary", cssVar: "--primary-color", kind: "color", label: "Primary Colour", default: "#111827", derive: true },
          { key: "secondary", cssVar: "--secondary-color", kind: "color", label: "Secondary Color", default: "#111827", derive: true },
        ]
      },
      
      
    ]
  },

  {
    section: "Card",
    rows: [
      {
        layout: "single",
        fields: [
          { key: "cardRadius", cssVar: "--card-radius", kind: "select", label: "Card Radius", 
            options: [
              {key: "none", label: "None", value: "0px"}, 
              {key: "sm", label: "Small", value: "5px"}, 
              {key: "md", label: "Medium", value: "10px"}, 
              {key: "lg", label: "Large", value: "20px"}, 
              {key: "xl", label: "Extra Large", value: "30px"}
            ], default: "md" },
        ]
      },
      {
        // BG + text next to each other
        layout: "pair",
        fields: [
          { key: "cardBg", cssVar: "--card-bg-color", kind: "color", label: "Background", default: "#ffffff", derive: true },
          { key: "cardText", cssVar: "--card-text-color", kind: "color", label: "Text", default: "#111827", derive: true },
          
        ]
      },
      {
        layout: "single",
        fields: [
          { key: "taglineText", cssVar: "--tagline-text-color", kind: "color", label: "Tagline text", default: "#ffffff", derive: true },
        ]
      }
    ]
  },

  {
    section: "Header",
    rows: [
      {
        layout: "pair",
        fields: [
          { key: "headerBg", cssVar: "--header-bg-color", kind: "color", label: "Background", default: "#111827", derive: true },
          { key: "headerText", cssVar: "--header-text-color", kind: "color", label: "Text", default: "#ffffff", derive: true },
        ]
      },
      {
        layout: "single",
        fields: [
          { key: "subheadingText", cssVar: "--subheading-text-color", kind: "color", label: "Subheading text", default: "#cbd5e1", derive: true },
        ]
      }
    ]
  }
];


  // ----------------------------
  // Utilities
  // ----------------------------
  function safeParse(json, fallback) {
    try { return JSON.parse(json); } catch { return fallback; }
  }

  function structuredCloneSafe(obj) {
    if (typeof structuredClone === "function") return structuredClone(obj);
    return JSON.parse(JSON.stringify(obj));
  }

  function ensureGoogleFontLoaded(fontName) {
    const id = `tt-font-${fontName.replace(/\s+/g, "-").toLowerCase()}`;
    if (document.getElementById(id)) return;

    if (!document.getElementById("tt-preconnect-gfonts")) {
      const pre1 = document.createElement("link");
      pre1.id = "tt-preconnect-gfonts";
      pre1.rel = "preconnect";
      pre1.href = "https://fonts.googleapis.com";
      document.head.appendChild(pre1);

      const pre2 = document.createElement("link");
      pre2.rel = "preconnect";
      pre2.href = "https://fonts.gstatic.com";
      pre2.crossOrigin = "anonymous";
      document.head.appendChild(pre2);
    }

    const link = document.createElement("link");
    link.id = id;
    link.rel = "stylesheet";
    const family = encodeURIComponent(fontName).replace(/%20/g, "+");
    link.href = `https://fonts.googleapis.com/css2?family=${family}:wght@300;400;500;600;700&display=swap`;
    document.head.appendChild(link);
  }

  function ensureCssLoaded(href) {
    return new Promise((resolve, reject) => {
      const already = [...document.styleSheets].some(s => s.href && s.href.includes(href));
      if (already) return resolve();

      const id = "tt-widget-css";
      if (document.getElementById(id)) return resolve();

      const link = document.createElement("link");
      link.id = id;
      link.rel = "stylesheet";
      link.href = href;
      link.onload = () => resolve();
      link.onerror = () => reject(new Error(`ThemeTweaker: failed to load CSS ${href}`));
      document.head.appendChild(link);
    });
  }

  function readCssVar(name, fallback = "") {
    const v = getComputedStyle(document.documentElement).getPropertyValue(name);
    const s = (v || "").trim();
    return s || fallback;
  }

  function rgbStringToHex(rgb) {
    const m = rgb.match(/rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/i);
    if (!m) return null;
    const r = Number(m[1]), g = Number(m[2]), b = Number(m[3]);
    const to2 = (n) => n.toString(16).padStart(2, "0");
    return `#${to2(r)}${to2(g)}${to2(b)}`;
  }

  function normalizeColorLike(value) {
    const v = (value || "").trim();
    if (!v) return "";
    if (v.startsWith("#")) return v;
    if (v.startsWith("rgb(") || v.startsWith("rgba(")) return rgbStringToHex(v) || v;
    return v; // keep gradients as-is
  }

  function isHexColor(v) {
    return typeof v === "string" && /^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(v.trim());
  }

  function expandHex(hex) {
    const h = hex.replace("#", "").trim();
    if (h.length === 3) return "#" + h.split("").map(c => c + c).join("");
    return "#" + h;
  }

  function hexToRgb(hex) {
    const h = expandHex(hex).replace("#", "");
    return {
      r: parseInt(h.slice(0, 2), 16),
      g: parseInt(h.slice(2, 4), 16),
      b: parseInt(h.slice(4, 6), 16)
    };
  }

  function clamp(n) {
    return Math.max(0, Math.min(255, Math.round(n)));
  }

  function rgbToHex({ r, g, b }) {
    const to2 = (n) => n.toString(16).padStart(2, "0");
    return "#" + to2(clamp(r)) + to2(clamp(g)) + to2(clamp(b));
  }

  function darkenHex(hex, amount = 0.12) {
    if (!isHexColor(hex)) return hex;
    const { r, g, b } = hexToRgb(hex);
    return rgbToHex({ r: r * (1 - amount), g: g * (1 - amount), b: b * (1 - amount) });
  }

  function darkenGradientString(gradient, amount = 0.10) {
    if (typeof gradient !== "string") return gradient;
    const matches = gradient.match(/#([0-9a-f]{3}|[0-9a-f]{6})/gi);
    if (!matches || matches.length === 0) return gradient;

    let out = gradient;
    for (const m of matches) {
      const darker = darkenHex(m, amount);
      out = out.replace(new RegExp(m.replace("#", "\\#"), "g"), darker);
    }
    return out;
  }

  function computeHoverColor(value) {
    const v = String(value || "").trim();
    if (isHexColor(v)) return darkenHex(v, 0.12);
    if (v.toLowerCase().includes("gradient(")) return darkenGradientString(v, 0.10);
    return v;
  }

  function getButtonRadius(shape) {
    switch (shape) {
      case "square": return "0px";
      case "rounded-sm": return "5px";
      case "rounded-md": return "10px";
      case "rounded-lg": return "20px";
      case "circle": return "999px";
      case "pill":
      default: return "14px";
    }
  }

  function makeLinearGradient(angleDeg, startHex, endHex) {
    const a = Number.isFinite(angleDeg) ? angleDeg : 0;
    const start = isHexColor(startHex) ? startHex : "#000000";
    const end = isHexColor(endHex) ? endHex : "#ffffff";
    return `linear-gradient(${Math.round(a)}deg, ${start} 0%, ${end} 100%)`;
  }

  function parseLinearGradient(gradientStr) {
    const s = (gradientStr || "").trim();
    if (!s.toLowerCase().startsWith("linear-gradient")) return null;

    const angleMatch = s.match(/linear-gradient\(\s*([-\d.]+)\s*deg/i);
    const angle = angleMatch ? Number(angleMatch[1]) : 180;

    const hexes = s.match(/#([0-9a-f]{3}|[0-9a-f]{6})/ig) || [];
    const start = hexes[0] || null;
    const end = hexes[1] || hexes[0] || null;

    return { angle, start, end };
  }

  function getPageKey() {
    const { pathname } = window.location;
    let file = pathname.split("/").filter(Boolean).join("/");
    if (!file || file.endsWith("/")) file += "index";
    file = file.replace(/\.html?$/i, "");
    return file || "root";
  }

  function buildDefaults() {
    const d = {};
    for (const f of TT_FIELDS) d[f.key] = f.default;
    // gradient UI defaults
    d._bgKind = "solid";
    d._bgSolid = "#f5f5f5";
    d._bgStart = "#1e40af";
    d._bgEnd = "#3b82f6";
    d._bgAngle = 135;

    d._ctaKind = "gradient";
    d._ctaSolid = "#1e40af";
    d._ctaStart = "#1e40af";
    d._ctaEnd = "#3b82f6";
    d._ctaAngle = 135;

    return d;
  }

  function deriveFromHostCss() {
    const out = {};
    for (const f of TT_FIELDS) {
      if (!f.derive || !f.cssVar) continue;
      const raw = readCssVar(f.cssVar);
      if (!raw) continue;
      out[f.key] = normalizeColorLike(raw);
    }

    // background + cta (special cases)
    const bgRaw = readCssVar("--bg-color");
    const bgParsed = parseLinearGradient(bgRaw);
    if (bgParsed) {
      out._bgKind = "gradient";
      out._bgAngle = bgParsed.angle ?? 135;
      out._bgStart = bgParsed.start ?? out.primary ?? "#1e40af";
      out._bgEnd = bgParsed.end ?? out.secondary ?? "#3b82f6";
    } else {
      out._bgKind = "solid";
      out._bgSolid = normalizeColorLike(bgRaw) || "#f5f5f5";
    }

    const ctaRaw = readCssVar("--cta-button-bg-color");
    const ctaParsed = parseLinearGradient(ctaRaw);
    if (ctaParsed) {
      out._ctaKind = "gradient";
      out._ctaAngle = ctaParsed.angle ?? 135;
      out._ctaStart = ctaParsed.start ?? out.primary ?? "#1e40af";
      out._ctaEnd = ctaParsed.end ?? out.secondary ?? "#3b82f6";
    } else {
      out._ctaKind = "solid";
      out._ctaSolid = normalizeColorLike(ctaRaw) || (out.primary ?? "#1e40af");
    }

    return out;
  }

  function getStoredState(key) {
    const raw = localStorage.getItem(key);
    const s = safeParse(raw, null);
    return s && typeof s === "object" ? s : null;
  }

  function storeState(state, key) {
    localStorage.setItem(key, JSON.stringify(state));
  }

  // ---- base token style injection (only social vars + helpers) ----
  let baseStylesInjected = false;
  function injectBaseTokenStylesOnce() {
    if (baseStylesInjected) return;
    baseStylesInjected = true;

    const socialVars = Object.entries(CFG.socialBrandHex)
      .map(([k, v]) => `  --social-${k}: ${v};`)
      .join("\n");

    const style = document.createElement("style");
    style.id = "tt-base-token-styles";
    style.textContent = `
:root{
${socialVars}
  --social-use-brand-colors: 1;
}

/* Social helper */
.tt-social { color: var(--social-unified-color); }
:root[data-tt-social="brand"] .tt-social[data-kind="whatsapp"]{color:var(--social-whatsapp)}
:root[data-tt-social="brand"] .tt-social[data-kind="linkedin"]{color:var(--social-linkedin)}
:root[data-tt-social="brand"] .tt-social[data-kind="instagram"]{color:var(--social-instagram)}
:root[data-tt-social="brand"] .tt-social[data-kind="facebook"]{color:var(--social-facebook)}
:root[data-tt-social="brand"] .tt-social[data-kind="x"]{color:var(--social-x)}
:root[data-tt-social="brand"] .tt-social[data-kind="twitter"]{color:var(--social-twitter)}
:root[data-tt-social="brand"] .tt-social[data-kind="youtube"]{color:var(--social-youtube)}
:root[data-tt-social="brand"] .tt-social[data-kind="tiktok"]{color:var(--social-tiktok)}
:root[data-tt-social="brand"] .tt-social[data-kind="github"]{color:var(--social-github)}
:root[data-tt-social="brand"] .tt-social[data-kind="stackoverflow"]{color:var(--social-stackoverflow)}
:root[data-tt-social="brand"] .tt-social[data-kind="website"]{color:var(--social-website)}
:root[data-tt-social="brand"] .tt-social[data-kind="email"]{color:var(--social-email)}
:root[data-tt-social="brand"] .tt-social[data-kind="phone"]{color:var(--social-phone)}
:root[data-tt-social="brand"] .tt-social[data-kind="map"]{color:var(--social-map)}
:root[data-tt-social="brand"] .tt-social[data-kind="link"]{color:var(--social-link)}
:root[data-tt-social="unified"] .tt-social{color:var(--social-unified-color)}
`;
    document.head.appendChild(style);
  }

  function applyState(state) {
    const root = document.documentElement;

    // mode + social flags
    root.setAttribute("data-theme", state.mode);
    root.classList.toggle("tt-dark", state.mode === "dark");
    root.setAttribute("data-tt-social", state.socialColorMode === "unified" ? "unified" : "brand");

    // background + cta materialize
    const bgValue = state._bgKind === "gradient"
      ? makeLinearGradient(state._bgAngle, state._bgStart, state._bgEnd)
      : state._bgSolid;

    const ctaValue = state._ctaKind === "gradient"
      ? makeLinearGradient(state._ctaAngle, state._ctaStart, state._ctaEnd)
      : state._ctaSolid;

    root.style.setProperty("--bg-color", bgValue);
    root.style.setProperty("--cta-button-bg-color", ctaValue);

    // apply registry css vars
     for (const f of allFields()) {
       if (!f.cssVar) continue;
       if (state[f.key] == null) continue;
       console.log("applying", f.key, "->", state[f.key]);
        root.style.setProperty(f.cssVar, state[f.key]);
      }

    // derived helpers
    root.style.setProperty("--primary-color-hover", computeHoverColor(state.primary));
    root.style.setProperty("--secondary-color-hover", computeHoverColor(state.secondary));
    root.style.setProperty("--action-button-bg-color-hover", computeHoverColor(state.actionBtnBg));
    root.style.setProperty("--info-item-icon-bg-color-hover", computeHoverColor(state.infoIconBg));
    root.style.setProperty("--cta-button-bg-color-hover", computeHoverColor(ctaValue));
    root.style.setProperty("--button-radius", getButtonRadius(state.buttonShape));

    // font
    const fontMeta = CFG.fonts.find(f => f.value === state.font);
    if (fontMeta?.google) ensureGoogleFontLoaded(fontMeta.value);

    const fontVar = state.font === "system-ui"
      ? "system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif"
      : `'${state.font}', system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif`;

    root.style.setProperty("--font-family", fontVar);

    injectBaseTokenStylesOnce();
  }

  // ---------- Dial control ----------
  function createAngleDial(container, initialAngle, onChange) {
    const size = 84;
    const r = 34;
    const cx = size / 2;
    const cy = size / 2;
    const strokeW = 8;

    const svgNS = "http://www.w3.org/2000/svg";
    const svg = document.createElementNS(svgNS, "svg");
    svg.setAttribute("width", String(size));
    svg.setAttribute("height", String(size));
    svg.setAttribute("viewBox", `0 0 ${size} ${size}`);
    svg.style.touchAction = "none";

    const track = document.createElementNS(svgNS, "circle");
    track.setAttribute("cx", String(cx));
    track.setAttribute("cy", String(cy));
    track.setAttribute("r", String(r));
    track.setAttribute("fill", "none");
    track.setAttribute("stroke", "rgba(0,0,0,.18)");
    track.setAttribute("stroke-width", String(strokeW));

    const prog = document.createElementNS(svgNS, "path");
    prog.setAttribute("fill", "none");
    prog.setAttribute("stroke", "rgba(0,0,0,.55)");
    prog.setAttribute("stroke-width", String(strokeW));
    prog.setAttribute("stroke-linecap", "round");

    const knob = document.createElementNS(svgNS, "circle");
    knob.setAttribute("r", "7");
    knob.setAttribute("fill", "rgba(255,255,255,.95)");
    knob.setAttribute("stroke", "rgba(0,0,0,.25)");
    knob.setAttribute("stroke-width", "1");

    const label = document.createElement("div");
    label.style.position = "absolute";
    label.style.inset = "0";
    label.style.display = "grid";
    label.style.placeItems = "center";
    label.style.fontSize = "12px";
    label.style.fontWeight = "700";
    label.style.pointerEvents = "none";

    const wrap = document.createElement("div");
    wrap.style.position = "relative";
    wrap.style.width = `${size}px`;
    wrap.style.height = `${size}px`;
    wrap.appendChild(svg);
    wrap.appendChild(label);

    svg.appendChild(track);
    svg.appendChild(prog);
    svg.appendChild(knob);
    container.appendChild(wrap);

    function polarToXY(angleDeg) {
      const a = (angleDeg - 90) * (Math.PI / 180);
      return { x: cx + r * Math.cos(a), y: cy + r * Math.sin(a) };
    }

    function describeArc(angleDeg) {
      const end = polarToXY(angleDeg);
      const start = polarToXY(0);
      const largeArc = angleDeg > 180 ? 1 : 0;
      return `M ${start.x} ${start.y} A ${r} ${r} 0 ${largeArc} 1 ${end.x} ${end.y}`;
    }

    function setAngle(angleDeg, fire = true) {
      let a = angleDeg % 360;
      if (a < 0) a += 360;

      prog.setAttribute("d", describeArc(a));
      const p = polarToXY(a);
      knob.setAttribute("cx", String(p.x));
      knob.setAttribute("cy", String(p.y));
      label.textContent = `${Math.round(a)}°`;

      if (fire) onChange(Math.round(a));
    }

    function angleFromEvent(e) {
      const rect = svg.getBoundingClientRect();
      const clientX = e.touches ? e.touches[0].clientX : e.clientX;
      const clientY = e.touches ? e.touches[0].clientY : e.clientY;
      const x = clientX - rect.left;
      const y = clientY - rect.top;

      const dx = x - cx;
      const dy = y - cy;

      let deg = Math.atan2(dy, dx) * (180 / Math.PI);
      deg += 90;
      if (deg < 0) deg += 360;
      return deg;
    }

    let dragging = false;

    const startDrag = (e) => {
      dragging = true;
      e.preventDefault?.();
      setAngle(angleFromEvent(e));
      window.addEventListener("mousemove", onMove);
      window.addEventListener("mouseup", endDrag);
      window.addEventListener("touchmove", onMove, { passive: false });
      window.addEventListener("touchend", endDrag);
    };

    const onMove = (e) => {
      if (!dragging) return;
      e.preventDefault?.();
      setAngle(angleFromEvent(e));
    };

    const endDrag = () => {
      dragging = false;
      window.removeEventListener("mousemove", onMove);
      window.removeEventListener("mouseup", endDrag);
      window.removeEventListener("touchmove", onMove);
      window.removeEventListener("touchend", endDrag);
    };

    svg.addEventListener("mousedown", startDrag);
    svg.addEventListener("touchstart", startDrag, { passive: false });

    setAngle(Number.isFinite(initialAngle) ? initialAngle : 0, false);

    return { setAngle };
  }

  async function mountUIFromTemplate() {
    const res = await fetch(CFG.htmlTemplateUrl, { cache: "no-store" });
    if (!res.ok) throw new Error(`ThemeTweaker: failed to load ${CFG.htmlTemplateUrl}`);

    const html = await res.text();
    const host = document.createElement("div");
    host.innerHTML = html;

    const tpl = host.querySelector("#tt-template");
    if (!tpl) throw new Error("ThemeTweaker: missing <template id='tt-template'>");

    document.body.appendChild(tpl.content.cloneNode(true));

    const wrap = document.getElementById("theme-tweaker");
    if (!wrap) throw new Error("ThemeTweaker: missing #theme-tweaker after mount");

    // position
    if (CFG.position === "left") {
      wrap.style.left = "16px";
      wrap.style.right = "auto";
    } else {
      wrap.style.right = "16px";
      wrap.style.left = "auto";
    }

    return wrap;
  }

  function wireGradientUi(wrap, state, onChanged) {
    const panel = $(".tt-panel", wrap);

    const bgDialHost = $('[data-tt="bgDial"]', panel);
    const ctaDialHost = $('[data-tt="ctaDial"]', panel);
    const bgPreview = $('[data-tt="bgPreview"]', panel);
    const ctaPreview = $('[data-tt="ctaPreview"]', panel);

    const refreshPreviews = () => {
      if (bgPreview) bgPreview.style.background = makeLinearGradient(state._bgAngle, state._bgStart, state._bgEnd);
      if (ctaPreview) ctaPreview.style.background = makeLinearGradient(state._ctaAngle, state._ctaStart, state._ctaEnd);
    };

    if (bgDialHost) {
      createAngleDial(bgDialHost, state._bgAngle, (a) => {
        state._bgAngle = a;
        refreshPreviews();
        onChanged();
      });
    }

    if (ctaDialHost) {
      createAngleDial(ctaDialHost, state._ctaAngle, (a) => {
        state._ctaAngle = a;
        refreshPreviews();
        onChanged();
      });
    }

    refreshPreviews();
  }

  function allFields() {
  return TT_SECTIONS.flatMap(sec => sec.rows.flatMap(r => r.fields));
}

function buildDefaults() {
  const d = {};
  for (const f of allFields()) d[f.key] = f.default;

  // keep your gradient UI defaults too
  d._bgKind = "solid";
  d._bgSolid = "#f5f5f5";
  d._bgStart = "#1e40af";
  d._bgEnd = "#3b82f6";
  d._bgAngle = 135;

  d._ctaKind = "gradient";
  d._ctaSolid = "#1e40af";
  d._ctaStart = "#1e40af";
  d._ctaEnd = "#3b82f6";
  d._ctaAngle = 135;

  return d;
}

function deriveFromHostCss() {
  const out = {};
  for (const f of allFields()) {
    if (!f.derive || !f.cssVar) continue;
    const raw = readCssVar(f.cssVar);
    if (!raw) continue;
    out[f.key] = normalizeColorLike(raw);
  }
  return out;
}


  async function init() {
    CFG.storageKey = `${BASE_STORAGE_KEY}:${getPageKey()}`;
    await ensureCssLoaded(CFG.cssUrl);

    const defaults = buildDefaults();
    const hostDefaults = Object.assign({}, defaults, deriveFromHostCss());
    const stored = getStoredState(CFG.storageKey);
    const state = Object.assign({}, hostDefaults, stored || {});

    // expose to Alpine component
    window.__TT = window.__TT || {};
    window.__TT.cfg = CFG;
    window.__TT.fields = TT_FIELDS;
    window.__TT.state = state;
    window.__TT.sections = TT_SECTIONS;


    // apply initial state
    applyState(state);

    // mount UI
    const wrap = await mountUIFromTemplate();

    // add body shift if open
    if (CFG.defaultOpen) document.body.classList.add("tt-show");

    // wire gradient dials (non-Alpine)
    const persistAndApply = () => {
      storeState(state, CFG.storageKey);
      applyState(state);
    };
    wireGradientUi(wrap, state, persistAndApply);

    // ensure Alpine sees the newly injected DOM
    // Alpine v3 normally auto-inits via mutation observer, but this is a safe nudge.
    if (window.Alpine && typeof window.Alpine.initTree === "function") {
      window.Alpine.initTree(wrap);
    }

    // close on ESC
    window.addEventListener("keydown", (e) => {
      if (e.key === "Escape") {
        const panel = $(".tt-panel", wrap);
        panel?.classList.remove("tt-open");
        document.body.classList.remove("tt-show");
        // also update Alpine open flag if present
        wrap.__x?.$data && (wrap.__x.$data.open = false);
      }
    });
  }

  // Alpine component factory (global)
  window.themeTweaker = function themeTweaker() {
    return {
      open: CFG.defaultOpen,
      fields: TT_FIELDS,
      fonts: CFG.fonts,
      state: window.__TT?.state || {},
      sections: window.__TT.sections,

      init() {
        // grab shared state (set in init())
        this.state = window.__TT.state;

        // click outside closes
        document.addEventListener("click", (e) => {
          const wrap = document.getElementById("theme-tweaker");
          const panel = wrap?.querySelector(".tt-panel");
          if (!panel || !panel.classList.contains("tt-open")) return;
          if (!wrap.contains(e.target)) this.close();
        });
      },

      togglePanel() {
        this.open = !this.open;
        document.body.classList.toggle("tt-show", this.open);
      },

      close() {
        this.open = false;
        document.body.classList.remove("tt-show");
      },

      changed() {
        console.log("changed", this.state);
        // persist + apply (same state object)
        localStorage.setItem(CFG.storageKey, JSON.stringify(this.state));
        // apply
        // (call internal applyState via closure)
        applyState(this.state);
      },

      reset() {
        const fresh = buildDefaults();
        const host = deriveFromHostCss();
        const merged = Object.assign({}, fresh, host);

        // mutate existing state object in-place so Alpine stays reactive
        for (const k of Object.keys(this.state)) delete this.state[k];
        Object.assign(this.state, merged);

        localStorage.removeItem(CFG.storageKey);
        localStorage.setItem(CFG.storageKey, JSON.stringify(this.state));
        applyState(this.state);

        // refresh gradient previews (dials already set visually, but previews will update)
        const wrap = document.getElementById("theme-tweaker");
        const bgPreview = wrap?.querySelector('[data-tt="bgPreview"]');
        const ctaPreview = wrap?.querySelector('[data-tt="ctaPreview"]');
        if (bgPreview) bgPreview.style.background = makeLinearGradient(this.state._bgAngle, this.state._bgStart, this.state._bgEnd);
        if (ctaPreview) ctaPreview.style.background = makeLinearGradient(this.state._ctaAngle, this.state._ctaStart, this.state._ctaEnd);
      }
    };
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => void init());
  } else {
    void init();
  }

  // public API
  window.ThemeTweaker = {
    getState: () => structuredCloneSafe(getStoredState(CFG.storageKey) || buildDefaults()),
    reset: () => {
      localStorage.removeItem(CFG.storageKey);
      location.reload();
    }
  };
})();
