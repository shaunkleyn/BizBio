import { resolveLinks, type ResolvedLink, type ThemeName } from "./fa-resolver.js";

type CardJson = {
  theme?: ThemeName;
  links?: any[];
};

declare global {
  interface Window {
    bizCardLinks: (jsonUrl: string) => any;
  }
}

window.bizCardLinks = (jsonUrl: string) => ({
  card: null as CardJson | null,
  loading: false,
  error: "",
  theme: "light" as ThemeName,

  get resolved(): ResolvedLink[] {
    return resolveLinks(this.card?.links ?? [], { theme: this.theme, preferCssVars: true });
  },

  async load() {
    this.loading = true;
    this.error = "";
    try {
      const res = await fetch(jsonUrl, { cache: "no-store" });
      if (!res.ok) throw new Error(`Failed to load JSON (${res.status})`);
      this.card = (await res.json()) as CardJson;
      if (this.card?.theme) this.theme = this.card.theme;
    } catch (e: any) {
      this.error = e?.message ?? "Failed to load";
    } finally {
      this.loading = false;
    }
  }
});
