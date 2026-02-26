import { resolveLinks } from "./fa-resolver";
window.bizCardLinks = (jsonUrl) => ({
    card: null,
    loading: false,
    error: "",
    theme: "light",
    get resolved() {
        console.log("Resolving links for card:", this.card);
        return resolveLinks(this.card?.links ?? [], this.theme);
    },
    async load() {
        this.loading = true;
        this.error = "";
        try {
            const res = await fetch(jsonUrl, { cache: "no-store" });
            if (!res.ok)
                throw new Error(`Failed to load JSON (${res.status})`);
            this.card = (await res.json());
            console.log("Loaded card data:", this.card);
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
