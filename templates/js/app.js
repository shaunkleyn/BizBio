import { resolveLinks } from "./fa-resolver.js";
window.bizCardLinks = (jsonUrl) => ({
    card: null,
    loading: false,
    error: "",
    theme: "light",
    get resolved() {
        return resolveLinks(this.card?.links ?? [], { theme: this.theme, preferCssVars: true });
    },
    async load() {
        this.loading = true;
        this.error = "";
        try {
            const res = await fetch(jsonUrl, { cache: "no-store" });
            if (!res.ok)
                throw new Error(`Failed to load JSON (${res.status})`);
            this.card = (await res.json());
            if (this.card?.theme)
                this.theme = this.card.theme;
        }
        catch (e) {
            this.error = e?.message ?? "Failed to load";
        }
        finally {
            this.loading = false;
        }
    }
});
