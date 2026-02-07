/* theme-tweaker.js
 *
 * Drop-in floating UI to tweak theme tokens on ANY static HTML page.
 * - primary color
 * - secondary color
 * - background color
 * - text color
 * - light/dark mode toggle
 * - font picker (auto-loads Google Fonts when needed)
 *
 * Usage:
 *   <script defer src="./js/theme-tweaker.js"></script>
 *
 * Optional config (before including script):
 *   <script>
 *     window.ThemeTweakerConfig = { enabled: true, defaultOpen: false, position: "right" };
 *   </script>
 */

(() => {
  const CFG = {
    enabled: true,
    defaultOpen: false,
    position: "right", // "right" | "left"
    storageKey: "themeTweaker:v1",
    // CSS variables we manage:
    vars: {
      primary: "--primary-color",
      secondary: "--secondary-color",
      background: "--bg-color",
      text: "--text-color",
      font: "--font-family"
    },
    // Reasonable defaults
    defaults: {
      mode: "light", // "light" | "dark"
      primary: "#0A66C2",
      secondary: "#E4405F",
      background: "#FFFFFF",
      text: "#111827",
      font: "system-ui"
    },
    // Fonts (includes system + google)
    fonts: [
      { label: "System UI", value: "system-ui" },
      { label: "Inter", value: "Inter", google: true },
      { label: "Roboto", value: "Roboto", google: true },
      { label: "Open Sans", value: "Open Sans", google: true },
      { label: "Poppins", value: "Poppins", google: true },
      { label: "Montserrat", value: "Montserrat", google: true },
      { label: "Source Sans 3", value: "Source Sans 3", google: true },
      { label: "Merriweather", value: "Merriweather", google: true }
    ]
  };

  // Merge user config if present
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

  function ensureGoogleFontLoaded(fontName) {
    // Avoid duplicating
    const id = `tt-font-${fontName.replace(/\s+/g, "-").toLowerCase()}`;
    if (document.getElementById(id)) return;

    // Preconnect (optional, but nice)
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

  function applyState(state) {
    const root = document.documentElement;

    // Mode: set attribute + helpful class
    root.setAttribute("data-theme", state.mode);
    root.classList.toggle("tt-dark", state.mode === "dark");

    // CSS vars
    root.style.setProperty(CFG.vars.primary, state.primary);
    root.style.setProperty(CFG.vars.secondary, state.secondary);
    root.style.setProperty(CFG.vars.background, state.background);
    root.style.setProperty(CFG.vars.text, state.text);

    const fontVar = state.font === "system-ui"
      ? "system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif"
      : `'${state.font}', system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif`;

    root.style.setProperty(CFG.vars.font, fontVar);

    // Optionally load Google font if selected
    const fontMeta = CFG.fonts.find(f => f.value === state.font);
    if (fontMeta?.google) ensureGoogleFontLoaded(fontMeta.value);

    // Provide a base "hook" style for pages that want to use these tokens
    injectBaseTokenStylesOnce();
  }

  let baseStylesInjected = false;
  function injectBaseTokenStylesOnce() {
    if (baseStylesInjected) return;
    baseStylesInjected = true;

    const style = document.createElement("style");
    style.id = "tt-base-token-styles";
    style.textContent = `
      :root {
        ${CFG.vars.primary}: ${CFG.defaults.primary};
        ${CFG.vars.secondary}: ${CFG.defaults.secondary};
        ${CFG.vars.background}: ${CFG.defaults.background};
        ${CFG.vars.text}: ${CFG.defaults.text};
        ${CFG.vars.font}: system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;
      }
      body {
        background: var(${CFG.vars.background});
        color: var(${CFG.vars.text});
        font-family: var(${CFG.vars.font});
      }
    `;
    document.head.appendChild(style);
  }

  function createUI(state) {
    // Styles for the widget
    const style = document.createElement("style");
    style.id = "tt-widget-styles";
    style.textContent = `
      .tt-wrap {
        position: fixed;
        ${CFG.position === "left" ? "left: 16px;" : "right: 16px;"}
        bottom: 16px;
        z-index: 2147483647;
        font-family: system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;
      }
      .tt-fab {
        display: inline-flex;
        align-items: center;
        gap: 8px;
        padding: 10px 12px;
        border-radius: 999px;
        border: 1px solid rgba(0,0,0,.12);
        background: rgba(255,255,255,.92);
        backdrop-filter: blur(10px);
        -webkit-backdrop-filter: blur(10px);
        box-shadow: 0 10px 30px rgba(0,0,0,.12);
        cursor: pointer;
        user-select: none;
      }
      :root.tt-dark .tt-fab,
      [data-theme="dark"] .tt-fab {
        background: rgba(17,24,39,.92);
        border-color: rgba(255,255,255,.14);
        color: rgba(255,255,255,.9);
      }
      .tt-panel {
        margin-top: 10px;
        width: 320px;
        max-width: calc(100vw - 32px);
        border-radius: 16px;
        border: 1px solid rgba(0,0,0,.12);
        background: rgba(255,255,255,.92);
        backdrop-filter: blur(10px);
        -webkit-backdrop-filter: blur(10px);
        box-shadow: 0 12px 40px rgba(0,0,0,.14);
        overflow: hidden;
        display: none;
      }
      :root.tt-dark .tt-panel,
      [data-theme="dark"] .tt-panel {
        background: rgba(17,24,39,.92);
        border-color: rgba(255,255,255,.14);
        color: rgba(255,255,255,.9);
      }
      .tt-panel.tt-open { display: block; }
      .tt-head {
        padding: 12px 14px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 10px;
        border-bottom: 1px solid rgba(0,0,0,.08);
      }
      :root.tt-dark .tt-head,
      [data-theme="dark"] .tt-head {
        border-bottom-color: rgba(255,255,255,.12);
      }
      .tt-title { font-weight: 700; font-size: 14px; }
      .tt-actions { display: flex; gap: 8px; }
      .tt-btn {
        border: 1px solid rgba(0,0,0,.12);
        background: transparent;
        color: inherit;
        padding: 6px 10px;
        border-radius: 10px;
        cursor: pointer;
        font-size: 12px;
      }
      :root.tt-dark .tt-btn,
      [data-theme="dark"] .tt-btn {
        border-color: rgba(255,255,255,.18);
      }
      .tt-body { padding: 12px 14px; display: grid; gap: 10px; }
      .tt-row { display: grid; grid-template-columns: 1fr auto; align-items: center; gap: 10px; }
      .tt-row label { font-size: 12px; opacity: .85; }
      .tt-row input[type="color"] {
        width: 44px; height: 28px; border-radius: 8px; border: 1px solid rgba(0,0,0,.12); padding: 0; background: transparent;
      }
      :root.tt-dark .tt-row input[type="color"],
      [data-theme="dark"] .tt-row input[type="color"] {
        border-color: rgba(255,255,255,.18);
      }
      .tt-row select {
        width: 170px;
        padding: 6px 8px;
        border-radius: 10px;
        border: 1px solid rgba(0,0,0,.12);
        background: transparent;
        color: inherit;
        font-size: 12px;
      }
      :root.tt-dark .tt-row select,
      [data-theme="dark"] .tt-row select {
        border-color: rgba(255,255,255,.18);
      }
      .tt-foot {
        padding: 10px 14px;
        border-top: 1px solid rgba(0,0,0,.08);
        font-size: 11px;
        opacity: .75;
      }
      :root.tt-dark .tt-foot,
      [data-theme="dark"] .tt-foot {
        border-top-color: rgba(255,255,255,.12);
      }
      .tt-chip {
        display: inline-block;
        padding: 2px 8px;
        border-radius: 999px;
        border: 1px solid rgba(0,0,0,.12);
        font-size: 11px;
      }
      :root.tt-dark .tt-chip,
      [data-theme="dark"] .tt-chip {
        border-color: rgba(255,255,255,.18);
      }
      .tt-preview {
        display:flex;
        gap: 8px;
        align-items:center;
        margin-top: 6px;
      }
      .tt-swatch {
        width: 14px;
        height: 14px;
        border-radius: 4px;
        border: 1px solid rgba(0,0,0,.12);
      }
      :root.tt-dark .tt-swatch,
      [data-theme="dark"] .tt-swatch {
        border-color: rgba(255,255,255,.18);
      }
      .tt-kbd {
        font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
      }
    `;
    document.head.appendChild(style);

    // DOM
    const wrap = document.createElement("div");
    wrap.className = "tt-wrap";

    const fab = document.createElement("div");
    fab.className = "tt-fab";
    fab.innerHTML = `
      <span class="tt-chip">Theme</span>
      <span style="font-size:12px;opacity:.85">Customize</span>
    `;

    const panel = document.createElement("div");
    panel.className = "tt-panel" + (CFG.defaultOpen ? " tt-open" : "");

    panel.innerHTML = `
      <div class="tt-head">
        <div>
          <div class="tt-title">Theme Tweaker</div>
          <div class="tt-preview">
            <span class="tt-swatch" data-tt="sw-primary"></span>
            <span class="tt-swatch" data-tt="sw-secondary"></span>
            <span class="tt-swatch" data-tt="sw-bg"></span>
            <span class="tt-swatch" data-tt="sw-text"></span>
          </div>
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
          <label>Primary color</label>
          <input data-tt="primary" type="color" />
        </div>

        <div class="tt-row">
          <label>Secondary color</label>
          <input data-tt="secondary" type="color" />
        </div>

        <div class="tt-row">
          <label>Background</label>
          <input data-tt="background" type="color" />
        </div>

        <div class="tt-row">
          <label>Text color</label>
          <input data-tt="text" type="color" />
        </div>

        <div class="tt-row">
          <label>Font</label>
          <select data-tt="font"></select>
        </div>

        <div style="display:grid;gap:6px;font-size:12px;opacity:.9">
          <div><span class="tt-kbd">--primary-color</span> / <span class="tt-kbd">--secondary-color</span></div>
          <div><span class="tt-kbd">--bg-color</span> / <span class="tt-kbd">--text-color</span></div>
          <div><span class="tt-kbd">--font-family</span></div>
        </div>
      </div>

      <div class="tt-foot">
        Saved in <span class="tt-kbd">localStorage</span>. Refresh-safe.
      </div>
    `;

    wrap.appendChild(fab);
    wrap.appendChild(panel);
    document.body.appendChild(wrap);

    // Wiring
    const modeEl = $('[data-tt="mode"]', panel);
    const primaryEl = $('[data-tt="primary"]', panel);
    const secondaryEl = $('[data-tt="secondary"]', panel);
    const backgroundEl = $('[data-tt="background"]', panel);
    const textEl = $('[data-tt="text"]', panel);
    const fontEl = $('[data-tt="font"]', panel);

    const swPrimary = $('[data-tt="sw-primary"]', panel);
    const swSecondary = $('[data-tt="sw-secondary"]', panel);
    const swBg = $('[data-tt="sw-bg"]', panel);
    const swText = $('[data-tt="sw-text"]', panel);

    const btnReset = $('[data-tt="reset"]', panel);
    const btnClose = $('[data-tt="close"]', panel);

    // Populate fonts
    fontEl.innerHTML = CFG.fonts
      .map(f => `<option value="${escapeHtml(f.value)}">${escapeHtml(f.label)}</option>`)
      .join("");

    // Set inputs from state
    modeEl.value = state.mode;
    primaryEl.value = state.primary;
    secondaryEl.value = state.secondary;
    backgroundEl.value = state.background;
    textEl.value = state.text;
    fontEl.value = state.font;

    // Update swatches
    function updateSwatches() {
      swPrimary.style.background = state.primary;
      swSecondary.style.background = state.secondary;
      swBg.style.background = state.background;
      swText.style.background = state.text;
    }
    updateSwatches();

    function persistAndApply() {
      storeState(state);
      applyState(state);
      updateSwatches();
    }

    function openPanel() { panel.classList.add("tt-open"); }
    function closePanel() { panel.classList.remove("tt-open"); }
    function togglePanel() { panel.classList.toggle("tt-open"); }

    fab.addEventListener("click", togglePanel);
    btnClose.addEventListener("click", closePanel);

    // Input events
    modeEl.addEventListener("change", () => {
      state.mode = modeEl.value === "dark" ? "dark" : "light";
      // If switching mode, you might want auto-default bg/text:
      // (We keep your chosen colors as-is, but you can uncomment below)
      // if (state.mode === "dark") { state.background = "#111827"; state.text = "#F9FAFB"; }
      persistAndApply();
    });

    primaryEl.addEventListener("input", () => { state.primary = primaryEl.value; persistAndApply(); });
    secondaryEl.addEventListener("input", () => { state.secondary = secondaryEl.value; persistAndApply(); });
    backgroundEl.addEventListener("input", () => { state.background = backgroundEl.value; persistAndApply(); });
    textEl.addEventListener("input", () => { state.text = textEl.value; persistAndApply(); });

    fontEl.addEventListener("change", () => {
      state.font = fontEl.value || "system-ui";
      persistAndApply();
    });

    btnReset.addEventListener("click", () => {
      Object.assign(state, structuredCloneSafe(CFG.defaults));
      // reflect in UI
      modeEl.value = state.mode;
      primaryEl.value = state.primary;
      secondaryEl.value = state.secondary;
      backgroundEl.value = state.background;
      textEl.value = state.text;
      fontEl.value = state.font;
      persistAndApply();
    });

    // Close on ESC
    window.addEventListener("keydown", (e) => {
      if (e.key === "Escape") closePanel();
    });

    // Click outside closes
    document.addEventListener("click", (e) => {
      if (!panel.classList.contains("tt-open")) return;
      const target = e.target;
      if (!(target instanceof Node)) return;
      if (!wrap.contains(target)) closePanel();
    });
  }

  function structuredCloneSafe(obj) {
    if (typeof structuredClone === "function") return structuredClone(obj);
    return JSON.parse(JSON.stringify(obj));
  }

  function escapeHtml(s) {
    return String(s)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;")
      .replace(/'/g, "&#039;");
  }

  function init() {
    // Load saved state or default
    const stored = getStoredState();
    const state = Object.assign(structuredCloneSafe(CFG.defaults), stored || {});

    // Apply state immediately
    applyState(state);

    // Build UI after DOM is ready
    createUI(state);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
