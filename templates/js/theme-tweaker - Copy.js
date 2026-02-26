/* theme-tweaker.js (gradient UI updated)
 *
 * Changes requested:
 * - Gradient sections: NO text box
 * - If gradient selected:
 *    - choose start + end colors
 *    - control degrees with a circular dial (draggable knob)
 *
 * Still includes:
 * - full BizBio token set
 * - auto hover colors (darken)
 * - button shapes
 * - social colors: brand vs unified
 *
 * Usage:
 *   <script defer src="./js/theme-tweaker.js"></script>
 */

(() => {
  const BASE_STORAGE_KEY = "themeTweaker:v3";

  const CFG = {
    enabled: true,
    defaultOpen: false,
    position: "right", // "right" | "left"
    storageKey: null,
    

    vars: {
      primary: "--primary-color",
      secondary: "--secondary-color",
      bg: "--bg-color",
      cardBg: "--card-bg-color",
      avatarBorder: "--avatar-border-color",
      text: "--text-color",
      font: "--font-family",

      actionBtnBg: "--action-button-bg-color",
      actionBtnText: "--action-button-text-color",
      infoIconBg: "--info-item-icon-bg-color",
      infoIconText: "--info-item-icon-text-color",
      headerColor: "--header-color",

      ctaBg: "--cta-button-bg-color",

      // Derived (auto)
      primaryHover: "--primary-color-hover",
      secondaryHover: "--secondary-color-hover",
      actionBtnHover: "--action-button-bg-color-hover",
      infoIconHover: "--info-item-icon-bg-color-hover",
      ctaHover: "--cta-button-bg-color-hover",

      // Shapes
      buttonRadius: "--button-radius",

      // Social behavior
      socialUseBrand: "--social-use-brand-colors", // "1" or "0"
      socialUnified: "--social-unified-color"
    },

    defaults: {
      mode: "light", // light | dark
      primary: "#1e40af",
      secondary: "#3b82f6",
      bg: "#f5f5f5",
      cardBg: "#ffffff",
      avatarBorder: "#ffffff",
      text: "#1e293b",
      font: "Inter",
      actionBtnBg: "#1e40af",
      actionBtnText: "#ffffff",
      infoIconBg: "#1e40af",
      infoIconText: "#ffffff",
      headerColor: "#ffffff",

      // CTA default is gradient
      ctaBg: "linear-gradient(135deg, #1e40af 0%, #3b82f6 100%)",

      // UI settings
      buttonShape: "pill", // square | pill | pill
      socialColorMode: "brand", // brand | unified
      socialUnifiedColor: "#1e40af"
    },

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

  const userCfg = window.ThemeTweakerConfig && typeof window.ThemeTweakerConfig === "object"
    ? window.ThemeTweakerConfig
    : {};
  Object.assign(CFG, userCfg);

  if (!CFG.enabled) return;

  const $ = (sel, root = document) => root.querySelector(sel);

  function safeParse(json, fallback) {
    try { return JSON.parse(json); } catch { return fallback; }
  }

  function getStoredState() {
    const raw = localStorage.getItem(CFG.storageKey);
    const s = safeParse(raw, null);
    return s && typeof s === "object" ? s : null;
  }

  function storeState(state) {
    localStorage.setItem(CFG.storageKey, JSON.stringify(state));
  }

  function structuredCloneSafe(obj) {
    if (typeof structuredClone === "function") return structuredClone(obj);
    return JSON.parse(JSON.stringify(obj));
  }

  function ensureGoogleFontLoaded(fontName) {
    console.log("Ensuring Google Font is loaded:", fontName);
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

  // ---------- Color utils ----------
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

  function rgbToHex({ r, g, b }) {
    const to2 = (n) => n.toString(16).padStart(2, "0");
    return "#" + to2(clamp(r)) + to2(clamp(g)) + to2(clamp(b));
  }

  function clamp(n) {
    return Math.max(0, Math.min(255, Math.round(n)));
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
      case "pill": return "14px";
      case "rounded-sm": return "5px";
      case "rounded-md": return "10px";
      case "rounded-lg": return "20px";
      case "circle": return "999px";
      // case "pill":
      default: return "14px";
    }
  }

  // ---------- Gradient builder ----------
  function makeLinearGradient(angleDeg, startHex, endHex) {
    const a = Number.isFinite(angleDeg) ? angleDeg : 0;
    const start = isHexColor(startHex) ? startHex : "#000000";
    const end = isHexColor(endHex) ? endHex : "#ffffff";
    return `linear-gradient(${Math.round(a)}deg, ${start} 0%, ${end} 100%)`;
  }

  // ---------- Apply state ----------
  let baseStylesInjected = false;

  function injectBaseTokenStylesOnce() {
    console.log("Injecting base token styles", { baseStylesInjected });
    if (baseStylesInjected) return;
    baseStylesInjected = true;

    const socialVars = Object.entries(CFG.socialBrandHex)
      .map(([k, v]) => `  --social-${k}: ${v};`)
      .join("\n");
    console.log("Generated social color CSS variables:", socialVars);

    const style = document.createElement("style");
    style.id = "tt-base-token-styles";
    style.textContent = `
:root{
  ${CFG.vars.primary}: ${CFG.defaults.primary};
  ${CFG.vars.secondary}: ${CFG.defaults.secondary};
  ${CFG.vars.bg}: ${CFG.defaults.bg};
  ${CFG.vars.cardBg}: ${CFG.defaults.cardBg};
  ${CFG.vars.avatarBorder}: ${CFG.defaults.avatarBorder};
  ${CFG.vars.text}: ${CFG.defaults.text};
  ${CFG.vars.font}: 'Inter', sans-serif;

  ${CFG.vars.actionBtnBg}: ${CFG.defaults.actionBtnBg};
  ${CFG.vars.actionBtnText}: ${CFG.defaults.actionBtnText};
  ${CFG.vars.infoIconBg}: ${CFG.defaults.infoIconBg};
  ${CFG.vars.infoIconText}: ${CFG.defaults.infoIconText};
  ${CFG.vars.headerColor}: ${CFG.defaults.headerColor};
  ${CFG.vars.ctaBg}: ${CFG.defaults.ctaBg};

  ${CFG.vars.primaryHover}: ${computeHoverColor(CFG.defaults.primary)};
  ${CFG.vars.secondaryHover}: ${computeHoverColor(CFG.defaults.secondary)};
  ${CFG.vars.actionBtnHover}: ${computeHoverColor(CFG.defaults.actionBtnBg)};
  ${CFG.vars.infoIconHover}: ${computeHoverColor(CFG.defaults.infoIconBg)};
  ${CFG.vars.ctaHover}: ${computeHoverColor(CFG.defaults.ctaBg)};

  ${CFG.vars.buttonRadius}: ${getButtonRadius(CFG.defaults.buttonShape)};

  ${CFG.vars.socialUseBrand}: 1;
  ${CFG.vars.socialUnified}: ${CFG.defaults.socialUnifiedColor};

${socialVars}
}

body{
  background: var(${CFG.vars.bg});
  color: var(${CFG.vars.text});
  font-family: var(${CFG.vars.font});
}

/* Social color helper: add class tt-social and data-kind="whatsapp|linkedin|..." */
.tt-social { color: var(${CFG.vars.socialUnified}); }

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

:root[data-tt-social="unified"] .tt-social{color:var(${CFG.vars.socialUnified})}

/* Use this var for your components if you want uniform rounding */
.tt-btn-shape { border-radius: var(${CFG.vars.buttonRadius}); }
`;
    document.head.appendChild(style);
  }

  function applyState(state) {
    console.log("Applying state:", state);
    const root = document.documentElement;

    root.setAttribute("data-theme", state.mode);
    root.classList.toggle("tt-dark", state.mode === "dark");
    root.setAttribute("data-tt-social", state.socialColorMode === "unified" ? "unified" : "brand");

    // Materialize BG/CTA based on chosen kind (solid vs gradient)
    const bgValue = state._bgKind === "gradient"
      ? makeLinearGradient(state._bgAngle, state._bgStart, state._bgEnd)
      : state._bgSolid;

    const ctaValue = state._ctaKind === "gradient"
      ? makeLinearGradient(state._ctaAngle, state._ctaStart, state._ctaEnd)
      : state._ctaSolid;

    state.bg = bgValue;
    state.ctaBg = ctaValue;

    root.style.setProperty(CFG.vars.primary, state.primary);
    root.style.setProperty(CFG.vars.secondary, state.secondary);
    root.style.setProperty(CFG.vars.bg, state.bg);
    root.style.setProperty(CFG.vars.cardBg, state.cardBg);
    root.style.setProperty(CFG.vars.avatarBorder, state.avatarBorder);
    root.style.setProperty(CFG.vars.text, state.text);

    root.style.setProperty(CFG.vars.actionBtnBg, state.actionBtnBg);
    root.style.setProperty(CFG.vars.actionBtnText, state.actionBtnText);
    root.style.setProperty(CFG.vars.infoIconBg, state.infoIconBg);
    root.style.setProperty(CFG.vars.infoIconText, state.infoIconText);
    root.style.setProperty(CFG.vars.headerColor, state.headerColor);
    root.style.setProperty(CFG.vars.ctaBg, state.ctaBg);

    root.style.setProperty(CFG.vars.primaryHover, computeHoverColor(state.primary));
    root.style.setProperty(CFG.vars.secondaryHover, computeHoverColor(state.secondary));
    root.style.setProperty(CFG.vars.actionBtnHover, computeHoverColor(state.actionBtnBg));
    root.style.setProperty(CFG.vars.infoIconHover, computeHoverColor(state.infoIconBg));
    root.style.setProperty(CFG.vars.ctaHover, computeHoverColor(state.ctaBg));

    root.style.setProperty(CFG.vars.buttonRadius, getButtonRadius(state.buttonShape));

    root.style.setProperty(CFG.vars.socialUnified, state.socialUnifiedColor);
    root.style.setProperty(CFG.vars.socialUseBrand, state.socialColorMode === "brand" ? "1" : "0");

    const fontMeta = CFG.fonts.find(f => f.value === state.font);
    if (fontMeta?.google) ensureGoogleFontLoaded(fontMeta.value);

    const fontVar = state.font === "system-ui"
      ? "system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif"
      : `'${state.font}', system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif`;

    root.style.setProperty(CFG.vars.font, fontVar);

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
      // 0deg should be at top (12 o'clock), so shift by -90
      const a = (angleDeg - 90) * (Math.PI / 180);
      return { x: cx + r * Math.cos(a), y: cy + r * Math.sin(a) };
    }

    function describeArc(angleDeg) {
      const end = polarToXY(angleDeg);
      const start = polarToXY(0);
      const largeArc = angleDeg > 180 ? 1 : 0;
      // arc from 0 to angle
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

      // atan2 gives angle where 0 is +x axis, we want 0 at top
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

  // ---------- UI ----------
  function escapeHtml(s) {
    return String(s)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }

  function createUI(state) {
    const style = document.createElement("style");
    style.id = "tt-widget-styles";
    style.textContent = `
    :root {
    --sidebar-width: 400px;
    }
.tt-wrap{
  position:fixed;
  ${CFG.position === "left" ? "left:16px;" : "right:16px;"}
  bottom:16px;
  z-index:2147483647;
  color: black;
  font-family:system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;
}
.tt-fab{
  display:inline-flex;align-items:center;gap:8px;
  padding:10px 12px;border-radius:999px;
  border:1px solid rgba(0,0,0,.12);
  background:rgba(255,255,255,.92);
  backdrop-filter:blur(10px);-webkit-backdrop-filter:blur(10px);
  box-shadow:0 10px 30px rgba(0,0,0,.12);
  cursor:pointer;user-select:none;
}
:root.tt-dark .tt-fab,[data-theme="dark"] .tt-fab{
  background:rgba(17,24,39,.92);
  border-color:rgba(255,255,255,.14);
  color:rgba(255,255,255,.9);
}
.tt-panel{
  margin-top:10px;width:380px;max-width:calc(100vw - 32px);
  border-radius:16px;border:1px solid rgba(0,0,0,.12);
  background:rgba(255,255,255,.92);
  backdrop-filter:blur(10px);-webkit-backdrop-filter:blur(10px);
  box-shadow:0 12px 40px rgba(0,0,0,.14);
  overflow-y: scroll;display:none;
  max-height: 90vh;
}
}
:root.tt-dark .tt-panel,[data-theme="dark"] .tt-panel{
  background:rgba(17,24,39,.92);
  border-color:rgba(255,255,255,.14);
  color:rgba(255,255,255,.9);
}
.tt-panel.tt-open{display:block;}
.tt-head{
  padding:12px 14px;display:flex;align-items:flex-start;justify-content:space-between;gap:10px;
  border-bottom:1px solid rgba(0,0,0,.08);
}
:root.tt-dark .tt-head,[data-theme="dark"] .tt-head{border-bottom-color:rgba(255,255,255,.12);}
.tt-title{font-weight:700;font-size:14px;}
.tt-hint{font-size:11px;opacity:.75;line-height:1.25;margin-top:2px;}
.tt-actions{display:flex;gap:8px;}
.tt-btn{
  border:1px solid rgba(0,0,0,.12);
  background:transparent;color:inherit;
  padding:6px 10px;border-radius:10px;
  cursor:pointer;font-size:12px;
}
:root.tt-dark .tt-btn,[data-theme="dark"] .tt-btn{border-color:rgba(255,255,255,.18);}
.tt-body{padding:12px 14px;display:grid;gap:10px;}
.tt-row{display:grid;grid-template-columns:1fr auto;align-items:center;gap:10px;}
.tt-row label{font-size:12px;opacity:.85;}
.tt-row input[type="color"]{
  width:34px;height:34px;border-radius:30px;border:2px solid rgb(195 195 195);padding:0;background:transparent;
}
:root.tt-dark .tt-row input[type="color"],[data-theme="dark"] .tt-row input[type="color"]{border-color:rgba(255,255,255,.18);}
.tt-row select{
  width:210px;padding:6px 8px;border-radius:10px;border:1px solid rgba(0,0,0,.12);
  background:transparent;color:inherit;font-size:12px;
}
:root.tt-dark .tt-row select,[data-theme="dark"] .tt-row select{border-color:rgba(255,255,255,.18);}
.tt-foot{padding:10px 14px;border-top:1px solid rgba(0,0,0,.08);font-size:11px;opacity:.75;}
:root.tt-dark .tt-foot,[data-theme="dark"] .tt-foot{border-top-color:rgba(255,255,255,.12);}
.tt-chip{display:inline-block;padding:2px 8px;border-radius:999px;border:1px solid rgba(0,0,0,.12);font-size:11px;}
:root.tt-dark .tt-chip,[data-theme="dark"] .tt-chip{border-color:rgba(255,255,255,.18);}
.tt-grid2{display:grid;grid-template-columns:1fr 1fr;gap:10px;}
.tt-sep{height:1px;background:rgba(0,0,0,.08);margin:8px 0;}
:root.tt-dark .tt-sep,[data-theme="dark"] .tt-sep{background:rgba(255,255,255,.12);}
.tt-gradient-row{
  display:grid;
  grid-template-columns: 1fr 1fr auto;
  gap:10px;
  align-items:center;
}
.tt-gradient-box{
  display:grid;
  gap:6px;
}
.tt-gradient-preview{
  height:30px;border-radius:30px;border:2px solid rgb(195 195 195);
}
:root.tt-dark .tt-gradient-preview,[data-theme="dark"] .tt-gradient-preview{border-color:rgba(255,255,255,.18);}
.tt-dial-wrap{
  display:flex;flex-direction:column;align-items:center;gap:6px;
}
.tt-dial-label{font-size:11px;opacity:.8;}
body.tt-show {
    max-width: calc(100% - 400px);
}
`;
    document.head.appendChild(style);
    console.log("Injected widget styles", { style });

    // Initialize internal gradient state if missing
    state._bgKind = state._bgKind || "solid";
    state._bgSolid = state._bgSolid || (isHexColor(CFG.defaults.bg) ? CFG.defaults.bg : "#f5f5f5");
    state._bgStart = state._bgStart || CFG.defaults.primary;
    state._bgEnd = state._bgEnd || CFG.defaults.secondary;
    state._bgAngle = Number.isFinite(state._bgAngle) ? state._bgAngle : 135;

    state._ctaKind = state._ctaKind || "gradient";
    state._ctaSolid = state._ctaSolid || CFG.defaults.primary;
    state._ctaStart = state._ctaStart || CFG.defaults.primary;
    state._ctaEnd = state._ctaEnd || CFG.defaults.secondary;
    state._ctaAngle = Number.isFinite(state._ctaAngle) ? state._ctaAngle : 135;

    const wrap = document.createElement("div");
    wrap.className = "tt-wrap";

    const fab = document.createElement("div");
    fab.className = "tt-fab";
    fab.innerHTML = `<span class="tt-chip">Theme</span><span style="font-size:12px;opacity:.85">Customize</span>`;

    const panel = document.createElement("div");
    panel.className = "tt-panel" + (CFG.defaultOpen ? " tt-open" : "");

    panel.innerHTML = `
<div class="tt-head">
  <div>
    <div class="tt-title">Theme Tweaker</div>
    <div class="tt-hint">Edits CSS variables on <span style="font-family:ui-monospace">:root</span> and saves to localStorage.</div>
  </div>
  <div class="tt-actions">
    <button class="tt-btn" type="button" data-tt="reset">Reset</button>
    <button class="tt-btn" type="button" data-tt="close">Close</button>
  </div>
</div>

<div class="tt-body">
  <div class="tt-row">
    <label>Mode</label>
    <select data-tt="mode">
      <option value="light">Light</option>
      <option value="dark">Dark</option>
    </select>
  </div>

  <div class="tt-row">
    <label>Font</label>
    <select data-tt="font"></select>
  </div>

  <div class="tt-row">
    <label>Text color</label>
    <input data-tt="text" type="color" />
  </div>

  <div class="tt-sep"></div>

  <div class="tt-grid2">
    <div class="tt-row">
      <label>Primary</label>
      <input data-tt="primary" type="color" />
    </div>
    <div class="tt-row">
      <label>Secondary</label>
      <input data-tt="secondary" type="color" />
    </div>
  </div>

  <div class="tt-grid2">
    <div class="tt-row">
      <label>Card BG</label>
      <input data-tt="cardBg" type="color" />
    </div>
    <div class="tt-row">
      <label>Avatar border</label>
      <input data-tt="avatarBorder" type="color" />
    </div>
  </div>

  <div class="tt-sep"></div>

  <div class="tt-row">
    <label>Background</label>
    <select data-tt="bgKind">
      <option value="solid">Solid</option>
      <option value="gradient">Gradient</option>
    </select>
  </div>

  <div data-tt="bgSolidBlock" class="tt-row">
    <label>BG color</label>
    <input data-tt="bgSolid" type="color" />
  </div>

  <div data-tt="bgGradBlock" class="tt-gradient-row">
    <div class="tt-gradient-box">
      <div class="tt-row" style="grid-template-columns:1fr auto">
        <label>Start</label>
        <input data-tt="bgStart" type="color" />
      </div>
      <div class="tt-row" style="grid-template-columns:1fr auto">
        <label>End</label>
        <input data-tt="bgEnd" type="color" />
      </div>
      <div class="tt-gradient-preview" data-tt="bgPreview"></div>
    </div>

    <div class="tt-gradient-box">
      <div class="tt-dial-wrap">
        <div class="tt-dial-label">Angle</div>
        <div data-tt="bgDial"></div>
      </div>
    </div>

    <div></div>
  </div>

  <div class="tt-sep"></div>

  <div class="tt-grid2">
    <div class="tt-row">
      <label>Action btn BG</label>
      <input data-tt="actionBtnBg" type="color" />
    </div>
    <div class="tt-row">
      <label>Action btn text</label>
      <input data-tt="actionBtnText" type="color" />
    </div>
  </div>

  <div class="tt-grid2">
    <div class="tt-row">
      <label>Info icon BG</label>
      <input data-tt="infoIconBg" type="color" />
    </div>
    <div class="tt-row">
      <label>Info icon text</label>
      <input data-tt="infoIconText" type="color" />
    </div>
  </div>

  <div class="tt-row">
    <label>Header color</label>
    <input data-tt="headerColor" type="color" />
  </div>

  <div class="tt-sep"></div>

  <div class="tt-row">
    <label>CTA BG</label>
    <select data-tt="ctaKind">
      <option value="solid">Solid</option>
      <option value="gradient">Gradient</option>
    </select>
  </div>

  <div data-tt="ctaSolidBlock" class="tt-row">
    <label>CTA color</label>
    <input data-tt="ctaSolid" type="color" />
  </div>

  <div data-tt="ctaGradBlock" class="tt-gradient-row">
    <div class="tt-gradient-box">
      <div class="tt-row" style="grid-template-columns:1fr auto">
        <label>Start</label>
        <input data-tt="ctaStart" type="color" />
      </div>
      <div class="tt-row" style="grid-template-columns:1fr auto">
        <label>End</label>
        <input data-tt="ctaEnd" type="color" />
      </div>
      <div class="tt-gradient-preview" data-tt="ctaPreview"></div>
    </div>

    <div class="tt-gradient-box">
      <div class="tt-dial-wrap">
        <div class="tt-dial-label">Angle</div>
        <div data-tt="ctaDial"></div>
      </div>
    </div>

    <div></div>
  </div>

  <div class="tt-sep"></div>

  <div class="tt-row">
    <label>Button shape</label>
    <select data-tt="buttonShape">
      <option value="square">Square</option>
      <option value="rounded-sm">Rounded Small</option>
      <option value="rounded-md">Rounded Medium</option>
      <option value="rounded-lg">Rounded Large</option>
      <option value="pill">Pill</option>
      <option value="circle">Circle</option>
    </select>
  </div>

  <div class="tt-sep"></div>

  <div class="tt-row">
    <label>Social colors</label>
    <select data-tt="socialMode">
      <option value="brand">Use brand colors</option>
      <option value="unified">Use one color for all</option>
    </select>
  </div>
  <div class="tt-row">
    <label>Unified social color</label>
    <input data-tt="socialUnifiedColor" type="color" />
  </div>
  <div class="tt-hint">In your HTML: <span style="font-family:ui-monospace">class="tt-social"</span> + <span style="font-family:ui-monospace">data-kind="whatsapp"</span></div>
</div>

<div class="tt-foot">
  Tip: Use <span style="font-family:ui-monospace">var(--primary-color)</span> etc. in your CSS.
</div>
`;

    wrap.appendChild(fab);
    wrap.appendChild(panel);
    document.body.appendChild(wrap);

    const modeEl = $('[data-tt="mode"]', panel);
    const fontEl = $('[data-tt="font"]', panel);
    const textEl = $('[data-tt="text"]', panel);

    const primaryEl = $('[data-tt="primary"]', panel);
    const secondaryEl = $('[data-tt="secondary"]', panel);

    const cardBgEl = $('[data-tt="cardBg"]', panel);
    const avatarBorderEl = $('[data-tt="avatarBorder"]', panel);

    const bgKindEl = $('[data-tt="bgKind"]', panel);
    const bgSolidBlock = $('[data-tt="bgSolidBlock"]', panel);
    const bgGradBlock = $('[data-tt="bgGradBlock"]', panel);
    const bgSolidEl = $('[data-tt="bgSolid"]', panel);
    const bgStartEl = $('[data-tt="bgStart"]', panel);
    const bgEndEl = $('[data-tt="bgEnd"]', panel);
    const bgDialHost = $('[data-tt="bgDial"]', panel);
    const bgPreview = $('[data-tt="bgPreview"]', panel);

    const actionBtnBgEl = $('[data-tt="actionBtnBg"]', panel);
    const actionBtnTextEl = $('[data-tt="actionBtnText"]', panel);

    const infoIconBgEl = $('[data-tt="infoIconBg"]', panel);
    const infoIconTextEl = $('[data-tt="infoIconText"]', panel);

    const headerColorEl = $('[data-tt="headerColor"]', panel);

    const ctaKindEl = $('[data-tt="ctaKind"]', panel);
    const ctaSolidBlock = $('[data-tt="ctaSolidBlock"]', panel);
    const ctaGradBlock = $('[data-tt="ctaGradBlock"]', panel);
    const ctaSolidEl = $('[data-tt="ctaSolid"]', panel);
    const ctaStartEl = $('[data-tt="ctaStart"]', panel);
    const ctaEndEl = $('[data-tt="ctaEnd"]', panel);
    const ctaDialHost = $('[data-tt="ctaDial"]', panel);
    const ctaPreview = $('[data-tt="ctaPreview"]', panel);

    const buttonShapeEl = $('[data-tt="buttonShape"]', panel);

    const socialModeEl = $('[data-tt="socialMode"]', panel);
    const socialUnifiedColorEl = $('[data-tt="socialUnifiedColor"]', panel);

    const btnReset = $('[data-tt="reset"]', panel);
    const btnClose = $('[data-tt="close"]', panel);

    // Populate fonts
    fontEl.innerHTML = CFG.fonts
      .map(f => `<option value="${escapeHtml(f.value)}">${escapeHtml(f.label)}</option>`)
      .join("");

    // Set UI values
    modeEl.value = state.mode;
    fontEl.value = state.font;
    textEl.value = state.text;

    primaryEl.value = state.primary;
    secondaryEl.value = state.secondary;

    cardBgEl.value = state.cardBg;
    avatarBorderEl.value = state.avatarBorder;

    bgKindEl.value = state._bgKind;
    bgSolidEl.value = state._bgSolid;
    bgStartEl.value = state._bgStart;
    bgEndEl.value = state._bgEnd;

    actionBtnBgEl.value = state.actionBtnBg;
    actionBtnTextEl.value = state.actionBtnText;

    infoIconBgEl.value = state.infoIconBg;
    infoIconTextEl.value = state.infoIconText;

    headerColorEl.value = state.headerColor;

    ctaKindEl.value = state._ctaKind;
    ctaSolidEl.value = state._ctaSolid;
    ctaStartEl.value = state._ctaStart;
    ctaEndEl.value = state._ctaEnd;

    buttonShapeEl.value = state.buttonShape;

    socialModeEl.value = state.socialColorMode;
    socialUnifiedColorEl.value = state.socialUnifiedColor;

    function refreshGradientPreviews() {
      bgPreview.style.background = makeLinearGradient(state._bgAngle, state._bgStart, state._bgEnd);
      ctaPreview.style.background = makeLinearGradient(state._ctaAngle, state._ctaStart, state._ctaEnd);
    }

    function refreshGradVisibility() {
      const bgIsGrad = state._bgKind === "gradient";
      bgSolidBlock.style.display = bgIsGrad ? "none" : "grid";
      bgGradBlock.style.display = bgIsGrad ? "grid" : "none";

      const ctaIsGrad = state._ctaKind === "gradient";
      ctaSolidBlock.style.display = ctaIsGrad ? "none" : "grid";
      ctaGradBlock.style.display = ctaIsGrad ? "grid" : "none";
    }

    // Dials
    const bgDial = createAngleDial(bgDialHost, state._bgAngle, (a) => {
      state._bgAngle = a;
      refreshGradientPreviews();
      persistAndApply();
    });

    const ctaDial = createAngleDial(ctaDialHost, state._ctaAngle, (a) => {
      state._ctaAngle = a;
      refreshGradientPreviews();
      persistAndApply();
    });

    refreshGradVisibility();
    refreshGradientPreviews();

    function persistAndApply() {
      console.log("Persisting state:", state);
      storeState(state);
      applyState(state);
    }

    function togglePanel() { panel.classList.toggle("tt-open"); document.body.classList.toggle('tt-show'); }
    function closePanel() { panel.classList.remove("tt-open"); document.body.classList.remove('tt-show'); }

    fab.addEventListener("click", togglePanel);
    btnClose.addEventListener("click", closePanel);

    // Events
    modeEl.addEventListener("change", () => { state.mode = modeEl.value === "dark" ? "dark" : "light"; persistAndApply(); });
    fontEl.addEventListener("change", () => { state.font = fontEl.value || "system-ui"; persistAndApply(); });
    textEl.addEventListener("input", () => { state.text = textEl.value; persistAndApply(); });

    primaryEl.addEventListener("input", () => { state.primary = primaryEl.value; persistAndApply(); });
    secondaryEl.addEventListener("input", () => { state.secondary = secondaryEl.value; persistAndApply(); });

    cardBgEl.addEventListener("input", () => { state.cardBg = cardBgEl.value; persistAndApply(); });
    avatarBorderEl.addEventListener("input", () => { state.avatarBorder = avatarBorderEl.value; persistAndApply(); });

    bgKindEl.addEventListener("change", () => {
      state._bgKind = bgKindEl.value;
      refreshGradVisibility();
      persistAndApply();
    });
    bgSolidEl.addEventListener("input", () => { state._bgSolid = bgSolidEl.value; persistAndApply(); });

    bgStartEl.addEventListener("input", () => { state._bgStart = bgStartEl.value; refreshGradientPreviews(); persistAndApply(); });
    bgEndEl.addEventListener("input", () => { state._bgEnd = bgEndEl.value; refreshGradientPreviews(); persistAndApply(); });

    actionBtnBgEl.addEventListener("input", () => { state.actionBtnBg = actionBtnBgEl.value; persistAndApply(); });
    actionBtnTextEl.addEventListener("input", () => { state.actionBtnText = actionBtnTextEl.value; persistAndApply(); });

    infoIconBgEl.addEventListener("input", () => { state.infoIconBg = infoIconBgEl.value; persistAndApply(); });
    infoIconTextEl.addEventListener("input", () => { state.infoIconText = infoIconTextEl.value; persistAndApply(); });

    headerColorEl.addEventListener("input", () => { state.headerColor = headerColorEl.value; persistAndApply(); });

    ctaKindEl.addEventListener("change", () => {
      state._ctaKind = ctaKindEl.value;
      refreshGradVisibility();
      persistAndApply();
    });
    ctaSolidEl.addEventListener("input", () => { state._ctaSolid = ctaSolidEl.value; persistAndApply(); });

    ctaStartEl.addEventListener("input", () => { state._ctaStart = ctaStartEl.value; refreshGradientPreviews(); persistAndApply(); });
    ctaEndEl.addEventListener("input", () => { state._ctaEnd = ctaEndEl.value; refreshGradientPreviews(); persistAndApply(); });

    buttonShapeEl.addEventListener("change", () => { state.buttonShape = buttonShapeEl.value; persistAndApply(); });

    socialModeEl.addEventListener("change", () => { state.socialColorMode = socialModeEl.value; persistAndApply(); });
    socialUnifiedColorEl.addEventListener("input", () => { state.socialUnifiedColor = socialUnifiedColorEl.value; persistAndApply(); });

    btnReset.addEventListener("click", () => {
      const fresh = structuredCloneSafe(CFG.defaults);
      Object.assign(state, fresh);

      // reset gradient UI state
      state._bgKind = "solid";
      state._bgSolid = CFG.defaults.bg;
      state._bgStart = CFG.defaults.primary;
      state._bgEnd = CFG.defaults.secondary;
      state._bgAngle = 135;

      state._ctaKind = "gradient";
      state._ctaSolid = CFG.defaults.primary;
      state._ctaStart = CFG.defaults.primary;
      state._ctaEnd = CFG.defaults.secondary;
      state._ctaAngle = 135;

      // reflect in UI
      modeEl.value = state.mode;
      fontEl.value = state.font;
      textEl.value = state.text;

      primaryEl.value = state.primary;
      secondaryEl.value = state.secondary;

      cardBgEl.value = state.cardBg;
      avatarBorderEl.value = state.avatarBorder;

      bgKindEl.value = state._bgKind;
      bgSolidEl.value = state._bgSolid;
      bgStartEl.value = state._bgStart;
      bgEndEl.value = state._bgEnd;
      bgDial.setAngle(state._bgAngle, false);

      actionBtnBgEl.value = state.actionBtnBg;
      actionBtnTextEl.value = state.actionBtnText;

      infoIconBgEl.value = state.infoIconBg;
      infoIconTextEl.value = state.infoIconText;

      headerColorEl.value = state.headerColor;

      ctaKindEl.value = state._ctaKind;
      ctaSolidEl.value = state._ctaSolid;
      ctaStartEl.value = state._ctaStart;
      ctaEndEl.value = state._ctaEnd;
      ctaDial.setAngle(state._ctaAngle, false);

      buttonShapeEl.value = state.buttonShape;
      socialModeEl.value = state.socialColorMode;
      socialUnifiedColorEl.value = state.socialUnifiedColor;

      refreshGradVisibility();
      refreshGradientPreviews();
      persistAndApply();
    });

    // ESC closes
    window.addEventListener("keydown", (e) => { if (e.key === "Escape") closePanel(); });

    // click outside closes
    document.addEventListener("click", (e) => {
      if (!panel.classList.contains("tt-open")) return;
      const target = e.target;
      if (!(target instanceof Node)) return;
      if (!wrap.contains(target)) closePanel();
    });
  }

  function init() {
    CFG.storageKey = `${BASE_STORAGE_KEY}:${getPageKey()}`;
    console.log("Initialized Theme Tweaker with config:", CFG);
    // const stored = getStoredState();
    // const state = Object.assign(structuredCloneSafe(CFG.defaults), stored || {});
console.log("Deriving defaults from host CSS...");
  const hostDefaults = deriveDefaultsFromHostCss();
  const stored = getStoredState(CFG.storageKey);
  console.log('Host-derived defaults:', hostDefaults);
  console.log("Stored state from localStorage:", stored);

  // Stored wins, but hostDefaults fill any missing fields
  const state = Object.assign({}, hostDefaults, stored || {});
  
  applyState(state);
  createUI(state);
    // applyState(state);
    // createUI(state);
  }

  
  function readCssVar(name, fallback = "") {
  const v = getComputedStyle(document.documentElement).getPropertyValue(name);
  const s = (v || "").trim();
  return s || fallback;
}

function rgbStringToHex(rgb) {
  // supports rgb(r,g,b) and rgba(r,g,b,a)
  const m = rgb.match(/rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/i);
  if (!m) return null;
  const r = Number(m[1]), g = Number(m[2]), b = Number(m[3]);
  const to2 = (n) => n.toString(16).padStart(2, "0");
  return `#${to2(r)}${to2(g)}${to2(b)}`;
}

function normalizeColorLike(value) {
  console.log("Normalizing color-like value:", { value });
  const v = (value || "").trim();
  if (!v) return "";
  if (v.startsWith("#")) return v;
  if (v.startsWith("rgb(") || v.startsWith("rgba(")) return rgbStringToHex(v) || v;
  return v; // gradients or other formats
}

function parseLinearGradient(gradientStr) {
  console.log("Parsing linear gradient string:", { gradientStr });
  const s = (gradientStr || "").trim();
  if (!s.toLowerCase().startsWith("linear-gradient")) return null;

  // angle
  const angleMatch = s.match(/linear-gradient\(\s*([-\d.]+)\s*deg/i);
  const angle = angleMatch ? Number(angleMatch[1]) : 180;

  // first two hex colors in the string
  const hexes = s.match(/#([0-9a-f]{3}|[0-9a-f]{6})/ig) || [];
  const start = hexes[0] || null;
  const end = hexes[1] || hexes[0] || null;

  return { angle, start, end };
}


function deriveDefaultsFromHostCss() {
  // Read raw
  const primaryRaw = readCssVar("--primary-color");
  const secondaryRaw = readCssVar("--secondary-color");
  const bgRaw = readCssVar("--bg-color");
  const cardBgRaw = readCssVar("--card-bg-color");
  const avatarBorderRaw = readCssVar("--avatar-border-color");
  const textRaw = readCssVar("--text-color");
  const fontRaw = readCssVar("--font-family");
  const actionBtnBgRaw = readCssVar("--action-button-bg-color");
  const actionBtnTextRaw = readCssVar("--action-button-text-color");
  const infoIconBgRaw = readCssVar("--info-item-icon-bg-color");
  const infoIconTextRaw = readCssVar("--info-item-icon-text-color");
  const headerColorRaw = readCssVar("--header-color");
  const ctaBgRaw = readCssVar("--cta-button-bg-color");

  console.log("Raw CSS values:", {
    primaryRaw,
    secondaryRaw,
    bgRaw,
    cardBgRaw,
    avatarBorderRaw,
    textRaw,
    fontRaw,
    actionBtnBgRaw,
    actionBtnTextRaw,
    infoIconBgRaw,
    infoIconTextRaw,
    headerColorRaw,
    ctaBgRaw
  });

  // Normalize colors (hex/rgb), keep gradients
  const primary = normalizeColorLike(primaryRaw);
  console.log("Normalized primary color:", primary);
  const secondary = normalizeColorLike(secondaryRaw);
  console.log("Normalized secondary color:", secondary);
  const bg = normalizeColorLike(bgRaw);
  console.log("Normalized background color:", bg);
  const cardBg = normalizeColorLike(cardBgRaw);
  console.log("Normalized card background color:", cardBg);
  const avatarBorder = normalizeColorLike(avatarBorderRaw);
  console.log("Normalized avatar border color:", avatarBorder);
  const text = normalizeColorLike(textRaw);
  console.log("Normalized text color:", text);
  const actionBtnBg = normalizeColorLike(actionBtnBgRaw);
  console.log("Normalized action button background color:", actionBtnBg);
  const actionBtnText = normalizeColorLike(actionBtnTextRaw);
  console.log("Normalized action button text color:", actionBtnText);
  const infoIconBg = normalizeColorLike(infoIconBgRaw);
  console.log("Normalized info icon background color:", infoIconBg);
  const infoIconText = normalizeColorLike(infoIconTextRaw);
  console.log("Normalized info icon text color:", infoIconText);
  const headerColor = normalizeColorLike(headerColorRaw);
  console.log("Normalized header color:", headerColor);

  console.log("Normalized CSS values:", {
    primary,
    secondary,
    bg,
    cardBg,
    avatarBorder,
    text,
    actionBtnBg,
    actionBtnText,
    infoIconBg,
    infoIconText,
    headerColor
  });

CFG.defaults.primary = primary;
CFG.defaults.secondary = secondary;
CFG.defaults.bg = bg;
CFG.defaults.cardBg = cardBg;
CFG.defaults.avatarBorder = avatarBorder;
CFG.defaults.text = text;
CFG.defaults.actionBtnBg = actionBtnBg;
CFG.defaults.actionBtnText = actionBtnText;
CFG.defaults.infoIconBg = infoIconBg;
CFG.defaults.infoIconText = infoIconText;
CFG.defaults.headerColor = headerColor;


  

  // Font: keep as-is (often includes fallbacks)
  const font = (fontRaw || "").trim() || "system-ui";

  // CTA: could be gradient or solid
  const ctaBg = (ctaBgRaw || "").trim();

  // Gradient UI state derived from CSS
  const ctaParsed = parseLinearGradient(ctaBg);
  const ctaKind = ctaParsed ? "gradient" : "solid";

  return {
    // theme mode can still be your own default logic
    mode: readCssVar("--theme-mode", "") || "light",

    primary: primary || "#1e40af",
    secondary: secondary || "#3b82f6",
    bg: bg || "#f5f5f5",
    cardBg: cardBg || "#ffffff",
    avatarBorder: avatarBorder || "#ffffff",
    text: text || "#1e293b",
    font,

    actionBtnBg: actionBtnBg || (primary || "#1e40af"),
    actionBtnText: actionBtnText || "#ffffff",
    infoIconBg: infoIconBg || (primary || "#1e40af"),
    infoIconText: infoIconText || "#ffffff",
    headerColor: headerColor || "#ffffff",

    ctaBg: ctaBg || `linear-gradient(135deg, ${primary || "#1e40af"} 0%, ${secondary || "#3b82f6"} 100%)`,

    // Gradient UI for CTA (dial + colors)
    _ctaKind: ctaKind,
    _ctaAngle: ctaParsed?.angle ?? 135,
    _ctaStart: ctaParsed?.start ?? (primary || "#1e40af"),
    _ctaEnd: ctaParsed?.end ?? (secondary || "#3b82f6"),
    _ctaSolid: normalizeColorLike(ctaBg) || (primary || "#1e40af"),

    // Background gradient UI (if host bg is gradient)
    _bgKind: parseLinearGradient(bg)?.angle != null ? "gradient" : "solid",
    _bgAngle: parseLinearGradient(bg)?.angle ?? 135,
    _bgStart: parseLinearGradient(bg)?.start ?? (primary || "#1e40af"),
    _bgEnd: parseLinearGradient(bg)?.end ?? (secondary || "#3b82f6"),
    _bgSolid: normalizeColorLike(bg) || "#f5f5f5",
  };
}

function getPageKey() {
  const { pathname } = window.location;

  // "/siyanda/index.html" → "siyanda/index.html"
  let file = pathname.split("/").filter(Boolean).join("/");

  // If it's a directory (ends with "/"), treat as index
  if (!file || file.endsWith("/")) {
    file += "index";
  }

  // Remove extension if you prefer (optional)
  file = file.replace(/\.html?$/i, "");

  return file || "root";
}



  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }

  window.ThemeTweaker = {
    getState: () => structuredCloneSafe(getStoredState() || CFG.defaults),
    
    reset: () => { console.log("Current state from localStorage:", getStoredState()),
      localStorage.removeItem(CFG.storageKey); location.reload(); 
    console.log("Current state from localStorage after reset:", getStoredState())
  }
  };
})();
