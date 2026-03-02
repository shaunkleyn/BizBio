const SOCIAL_ICON_MAP = {
  "linkedin.com": "linkedin",
  "instagram.com": "instagram",
  "facebook.com": "facebook",
  "twitter.com": "x",
  "x.com": "x",
  "youtube.com": "youtube",
  "tiktok.com": "tiktok",
  "github.com": "github",
  "wa.me": "whatsapp",
  "whatsapp.com": "whatsapp",
  "tel:": "phone",
  "mailto:": "email",
  ".vcf": "phone"
};

const ICON_SETS = {
    email: { style: "fa-solid", icon: "fa-envelope" },
    mail: { style: "fa-solid", icon: "fa-envelope" },
    phone: { style: "fa-solid", icon: "fa-phone" },
    call: { style: "fa-solid", icon: "fa-phone" },
    whatsapp: { style: "fa-brands", icon: "fa-whatsapp" },
    map: { style: "fa-solid", icon: "fa-location-dot" },
    website: { style: "fa-solid", icon: "fa-globe" },
    site: { style: "fa-solid", icon: "fa-globe" },
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

function resolveSocialIcon(social) {
  if (social.icon) return social.icon;
    console.log("Resolving icon for URL:", social);
  const url = (social || "").toLowerCase();
  console.log("Resolving icon for URL:", url);
  const match = Object.keys(SOCIAL_ICON_MAP).find(domain => url.includes(domain));
  console.log("Matched domain for icon:", match);
   const icon = Object.keys(ICON_SETS).find(kind => kind == SOCIAL_ICON_MAP[match]);
   console.log("Resolved icon kind:", icon, "with icon data:", ICON_SETS[icon].icon);
  return match ? ICON_SETS[icon].style + " " + ICON_SETS[icon].icon : "globe";
}

function resolveSocialName(social) {
  if (social.icon) return social.icon;
    console.log("Resolving icon for URL:", social);
  const url = (social || "").toLowerCase();
  console.log("Resolving icon for URL:", url);
  const match = Object.keys(SOCIAL_ICON_MAP).find(domain => url.includes(domain));
  console.log("Matched domain for icon:", match);
   const icon = Object.keys(ICON_SETS).find(kind => kind == SOCIAL_ICON_MAP[match]);
   console.log("Resolved icon kind:", icon, "with icon data:", ICON_SETS[icon].icon);
  return icon ? icon : match ? SOCIAL_ICON_MAP[match] : "";
}

function getFaIconFromLabel(label) {
    const normalisedLabel = (label || "").toLowerCase();
    const icon = Object.keys(ICON_SETS).find(kind => kind == normalisedLabel);
    return icon ? ICON_SETS[icon].style + " " + ICON_SETS[icon].icon : "fa fa-user";
}

function bizCard(jsonUrl) {
      return {
        card: null,
        loading: false,
        error: "",

        async load() {
          this.loading = true;
          this.error = "";
          try {
            const res = await fetch(jsonUrl, { cache: "no-store" });
            if (!res.ok) throw new Error(`Failed to load JSON (${res.status})`);
            this.card = await res.json();
          } catch (e) {
            this.error = e?.message ?? "Failed to load card data";
          } finally {
            this.loading = false;
          }
        },

        get fullName() {
          if (!this.card) return "";
          return `${this.card.person.firstName} ${this.card.person.lastName}`.trim();
        },

        get headline() {
          if (!this.card) return "";
          const t = this.card.person.title || "";
          const c = this.card.person.company || "";
          return [t, c].filter(Boolean).join(" • ");
        },

        get addressLine() {
          const a = this.card?.contact?.address;
          if (!a) return "";
          return [a.line1, a.city, a.region, a.postalCode, a.country].filter(Boolean).join(", ");
        },

        actionHref(a) {
          switch (a.type) {
            case "tel": return `tel:${a.value}`;
            case "mailto": return `mailto:${a.value}`;
            case "whatsapp": return `https://wa.me/${String(a.value).replace(/[^0-9]/g,'')}`;
            case "vcard": return a.url;
            default: return a.url || "#";
          }
        }
      }
    }