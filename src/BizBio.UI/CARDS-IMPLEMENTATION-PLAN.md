                                                                                                                        
 Here is Claude's plan:                                                                                                 
╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌╌
 Business Cards Wizard – Implementation Plan                                                                            
                                                                                                                      
 Context                                                                                                                
                                                                                                                        
 The dashboard currently has a "Business Cards" section that is disabled (opacity-60, cursor-not-allowed, "Coming Soon" 
  badge). This plan enables that section and adds a full multi-step onboarding wizard that guides users through         
 selecting a plan, building their digital business card, and publishing it. Profile data is persisted via the backend
 API using the entity/catalog data model. The wizard lives at /dashboard/cards/create (full-page route).

 ---
 Files to Modify

 ┌───────────────────────────┬─────────────────────────────────────────────────────────┐
 │           File            │                         Change                          │
 ├───────────────────────────┼─────────────────────────────────────────────────────────┤
 │ pages/dashboard/index.vue │ Enable Business Cards section; add "Get Started" button │
 ├───────────────────────────┼─────────────────────────────────────────────────────────┤
 │ composables/useApi.ts     │ Confirm useProfilesApi() shape; extend if needed        │
 └───────────────────────────┴─────────────────────────────────────────────────────────┘

 Files to Create

 ┌────────────────────────────────────────────────────────────┬─────────────────────────────────────────────────────┐
 │                            File                            │                       Purpose                       │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ pages/dashboard/cards/create.vue                           │ Wizard host page                                    │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ pages/dashboard/cards/index.vue                            │ Cards landing (redirects to create if no profile,   │
 │                                                            │ else shows profiles list)                           │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ composables/useBusinessCardCreation.ts                     │ Wizard state, plan logic, section management        │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/WizardLayout.vue                   │ Outer shell: progress bar, step indicator, nav      │
 │                                                            │ buttons                                             │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/Step1Plan.vue                      │ Plan selection step                                 │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/HelpMeChooseFlow.vue               │ 4-question quiz → recommended plan                  │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/Step2Build.vue                     │ Build step with tab bar + phone preview             │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/tabs/TemplateTab.vue               │ Template grid with lock indicators                  │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/tabs/ContentTab.vue                │ Section list with drag-drop                         │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/tabs/AppearanceTab.vue             │ Theme/color/font settings                           │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/sections/BasicInfoSection.vue      │ Name, title, company, photo, bio                    │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/sections/ContactButtonsSection.vue │ 1–3 action buttons                                  │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/sections/ContactInfoSection.vue    │ List-style contact items                            │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/sections/SocialSection.vue         │ Social platform links                               │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │                                                            │ Reusable shell for optional sections (bio, map,     │
 │ components/cards/wizard/sections/GenericSection.vue        │ skills, links, gallery, save-contact, share,        │
 │                                                            │ wallet)                                             │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/CardPhonePreview.vue               │ Phone-frame iframe preview (always visible in Step  │
 │                                                            │ 2)                                                  │
 ├────────────────────────────────────────────────────────────┼─────────────────────────────────────────────────────┤
 │ components/cards/wizard/Step3Publish.vue                   │ QR code, profile URL, analytics summary, Finish     │
 └────────────────────────────────────────────────────────────┴─────────────────────────────────────────────────────┘

 ---
 Step 1 — Plan Selection

 Layout

 Progress indicator at top (Step 1 · Step 2 · Step 3). Two choices on screen:

 Primary card — "Help Me Choose" (default, visually prominent)
 Secondary link — "I already know which plan I want →" (small, like a skip link)

 "Help Me Choose" Quiz (HelpMeChooseFlow.vue)

 Slides through 4 questions. Progress dots show position. Each question is a single-choice card selection:

 1. How many people need a digital card?
 "Just me" / "2–5 people" / "6–20 people"
 2. Do you need analytics? (who viewed your card, where from, what device)
 "No, not needed" / "Yes, I'd like basic stats" / "Yes, I need detailed analytics"
 3. Do you want custom branding — your own colours, fonts, and logo?
 "No, defaults are fine" / "Yes, custom colours" / "Yes, full custom branding"
 4. Do you need Google or Apple Wallet integration?
 "No thanks" / "Yes, at least one" / "Yes, both"

 Recommendation logic (maps answers → plan):
 Free:  all "No/Just me" answers
 Solo:  just me + any yes to analytics or branding
 Team:  2–5 people
 Business: 6–20 people
 Show recommended plan card with CTA "Start with [Plan]" + "See all plans" link to switch.

 Direct Plan Selection

 Grid of 4 plan cards (Free, Solo, Team, Business) with features list from CONNECT_PLANS.md:
 - Free R0 — 1 profile, 5 basic templates, QR code, vCard download, BizBio badge
 - Solo R59/mo — All premium templates, custom branding, analytics, no badge
 - Team R99/mo — 5 profiles, everything in Solo
 - Business R249/mo — 20 profiles, team management, advanced analytics

 Selecting a plan calls useSubscriptionsApi().subscribe(ProductType.Cards, tierId) — or starts a trial. On success,
 advance to Step 2.

 If the user already has a Cards subscription, skip Step 1 entirely (same pattern as MenuCreationWizard).

 ---
 Step 2 — Build Your Card

 Layout

 Two-column layout:
 - Left (tabs, scrollable, ~55% width): Tab bar + tab content
 - Right (sticky, ~45% width): CardPhonePreview.vue — always visible throughout all tabs

 Tab bar: Template · Content · Appearance

 ---
 Tab 1 — Template (TemplateTab.vue)

 Grid of template thumbnails (2 or 3 columns). Each thumbnail:
 - Loads a static preview screenshot (/templates/previews/{name}.jpg) OR generated on demand
 - Template name below
 - If template is premium (not in Free plan's 5 templates) and user is on Free: show lock icon overlay + "Solo plan
 required" tooltip
 - Selected template has a highlighted border

 Free plan templates (first 5): 01-minimalist-clean, 02-corporate-professional, 03-bold-creative, 04-dark-elegant,
 05-gradient-modern
 All others require Solo or above.

 Clicking a template updates the iframe preview immediately.

 Free tier templates: defined as a constant array in useBusinessCardCreation.ts.

 ---
 Tab 2 — Content (ContentTab.vue)

 Scrollable section list. Sections are divided into fixed (always present, can't be removed) and optional (removable,
 sortable between sections).

 Fixed Sections (at top, in order)

 1. Basic Info (BasicInfoSection.vue)
 - Profile photo upload (or URL) — uses useUploadsApi()
 - First name, Last name (required)
 - Headline / Job Title
 - Company / Organisation
 - Bio / About text (textarea, shows char limit based on plan)

 2. Contact Buttons (ContactButtonsSection.vue)
 - 1–3 buttons (Free: max 2; Solo+: max 3)
 - Each button: select type + enter value
 - Button types: Phone (tel:), Email (mailto:), WhatsApp (https://wa.me/), SMS, Facebook Messenger, Telegram, LinkedIn,
  Instagram, Twitter/X, Skype, Zoom, Calendar link, Website, Custom URL
 - Each button row has a drag handle for reordering within the section
 - "+ Add Button" (disabled when at plan limit)

 3. Contact Info (ContactInfoSection.vue)
 - List-style rows; each row: icon, label, value, action URL
 - Free plan: max 5 items. Solo+: unlimited
 - Row types: Phone, Email, Website, Address (opens Maps), LinkedIn profile, WhatsApp, Birthday, Custom
 - Each row is drag-sortable with .drag-handle (uses existing useDragDrop composable)
 - Trash icon to remove

 4. Social Links (SocialSection.vue)
 - Icon grid; each item: platform selector + URL
 - Free plan: max 5. Solo+: unlimited
 - Platforms: LinkedIn, Facebook, Instagram, Twitter/X, TikTok, YouTube, Pinterest, Snapchat, GitHub, Dribbble,
 Behance, WhatsApp, Telegram, Discord, Spotify, SoundCloud, Twitch
 - Drag-sortable rows
 - "+ Add Social Link"

 Optional Sections (sortable order, removable)

 The section list footer shows "+ Add Section" which opens a bottom sheet / dropdown listing available sections.
 Sections already added are greyed out in the list.

 ┌─────────────────────┬──────────┬────────────────────────────────────────┐
 │       Section       │   Plan   │                Content                 │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Bio                 │ Free+    │ Richtext / plain textarea              │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Map                 │ Solo+    │ Address input → embedded Google Maps   │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Skills / Tags       │ Solo+    │ Tag chips (add/remove), drag-sortable  │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Links               │ Solo+    │ Title + URL pairs, drag-sortable       │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Gallery             │ Business │ Image grid upload (max 9 images)       │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Save Contact Button │ Free+    │ vCard download, just a toggle (on/off) │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Share Button        │ Free+    │ Share profile link, just a toggle      │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Google Wallet       │ Solo+    │ Adds Google Wallet pass button         │
 ├─────────────────────┼──────────┼────────────────────────────────────────┤
 │ Apple Wallet        │ Solo+    │ Adds Apple Wallet button               │
 └─────────────────────┴──────────┴────────────────────────────────────────┘

 Each optional section has:
 - Drag handle at the top (to reorder between sections)
 - Section title + collapse chevron
 - Remove (×) button top-right
 - Locked overlay if plan insufficient

 ---
 Tab 3 — Appearance (AppearanceTab.vue)

 Grouped settings panels (accordion or flat list):

 Background
 - Type toggle: Solid Color / Gradient / Image / Pattern
 - Solid: single color picker
 - Gradient: 2 color pickers + direction (radial/linear, angle)
 - Image: upload or URL
 - Pattern: grid of 8–10 predefined SVG patterns + accent color picker

 Card
 - Card background color
 - Card border: toggle, color, radius (slider px)
 - Card shadow: toggle, intensity

 Typography
 - Heading font (Google Fonts: Inter, Poppins, Playfair Display, Montserrat, DM Sans, Raleway, Roboto, Lato, Open Sans,
  Nunito — dropdown)
 - Body font (same list)
 - Title color picker
 - Subtitle color picker
 - Body text color picker
 - Info label color picker

 Buttons / Icons
 - Button style: Filled / Outlined / Ghost / Pill
 - Primary color (used for buttons + icons)
 - Button text color
 - Icon style: Circle / Square / Rounded / None

 Wallet Branding (shown only if wallet sections are active)
 - Google Wallet background color
 - Google Wallet hero image upload
 - Issuer name text
 - Apple Wallet background color

 All appearance changes update the phone preview in real-time via postMessage into the iframe.

 ---
 CardPhonePreview.vue — Live iframe Preview

 ┌──────────────┐
 │ ┌──────────┐ │  ← phone frame (CSS device mockup)
 │ │          │ │
 │ │  <iframe>│ │
 │ │  srcdoc= │ │
 │ │  [HTML]  │ │
 │ └──────────┘ │
 └──────────────┘

 Implementation:
 1. Watch wizardState (template + content + appearance) using watchEffect
 2. On change: fetch selected template HTML (once, cache per template name)
 3. Inject data into template HTML using the same selector-based population logic as bizbio-engine.js (replicate the
 populateTemplate() method in a composable useTemplatePopulator.ts)
 4. Set CSS variables for appearance settings via <style> injection in <head>
 5. Set as iframe.srcdoc — no server round-trip needed
 6. Debounce updates 300ms to avoid thrashing

 The template HTML files are fetched via /templates/{name}.html (already served by the server middleware from
 profiles/templates/). The <base href="/templates/"> tag is added just like the engine does it.

 Preview composable useTemplatePopulator.ts:
 - Reuses the selector lists from bizbio-engine.js (name, title, company, bio, photo, email, phone, website, social
 selectors)
 - Builds the full HTML string from template + data + CSS var overrides
 - Returns the complete srcdoc string

 ---
 Wizard State — useBusinessCardCreation.ts

 interface WizardState {
   currentStep: 1 | 2 | 3
   activeTab: 'template' | 'content' | 'appearance'

   // Plan
   selectedPlan: 'free' | 'solo' | 'team' | 'business' | null
   subscriptionId: number | null

   // Template
   selectedTemplate: string  // e.g. "06-glassmorphism"

   // Basic Info
   firstName: string
   lastName: string
   headline: string
   company: string
   photoUrl: string
   bio: string

   // Contact Buttons (max 3)
   contactButtons: Array<{ id: string, type: string, value: string, label: string }>

   // Contact Info items
   contactInfo: Array<{ id: string, type: string, label: string, value: string, href: string }>

   // Social Links
   socialLinks: Array<{ id: string, platform: string, url: string }>

   // Optional sections (sortable list)
   sections: Array<{
     id: string
     type: 'bio' | 'map' | 'skills' | 'links' | 'gallery' | 'save-contact' | 'share' | 'google-wallet' | 'apple-wallet'
     sortOrder: number
     data: Record<string, any>
   }>

   // Appearance
   appearance: {
     bgType: 'solid' | 'gradient' | 'image' | 'pattern'
     bgColor: string
     bgGradientFrom: string
     bgGradientTo: string
     bgGradientDir: string
     bgImageUrl: string
     bgPattern: string
     cardBgColor: string
     cardBorderColor: string
     cardBorderRadius: number
     headingFont: string
     bodyFont: string
     titleColor: string
     subtitleColor: string
     bodyTextColor: string
     primaryColor: string
     buttonStyle: 'filled' | 'outlined' | 'ghost' | 'pill'
     iconStyle: 'circle' | 'square' | 'rounded' | 'none'
     walletBgColor: string
     walletIssuerName: string
   }

   // Publish
   slug: string
   profileUrl: string
   qrCodeSvg: string
 }

 Plan limits enforced by computed helpers in the composable:
 const maxContactButtons = computed(() => selectedPlan === 'free' ? 2 : 3)
 const maxContactInfo    = computed(() => selectedPlan === 'free' ? 5 : Infinity)
 const maxSocialLinks    = computed(() => selectedPlan === 'free' ? 5 : Infinity)
 const freeTemplates     =
 ['01-minimalist-clean','02-corporate-professional','03-bold-creative','04-dark-elegant','05-gradient-modern']
 const canUseTemplate    = (name) => selectedPlan !== 'free' || freeTemplates.includes(name)
 const canAddSection     = (type) => { /* returns false with planRequirement message if on Free */ }

 ---
 Step 3 — Publish

 Left panel:
 - "Your card is live!" heading
 - Profile URL: bizbio.co.za/{slug} with copy-to-clipboard button
 - Share button (Web Share API fallback to clipboard)

 Right panel:
 - QR code (generate using uqr — already in node_modules)
 - Download QR as SVG/PNG button

 Analytics teaser (show for Free plan with upgrade nudge; show real data for paid):
 - "Profile views this week — upgrade to Solo to see analytics"

 Finish button → navigates to /dashboard/cards (the cards landing page showing the user's profiles)

 ---
 Save Flow

 On "Next" from Step 2 → Step 3, the wizard calls:

 const profilesApi = useProfilesApi()

 const payload = {
   slug: state.slug || generateSlug(state.firstName, state.lastName),
   template: state.selectedTemplate,
   basicInfo: { firstName, lastName, headline, company, photoUrl, bio },
   contactButtons: state.contactButtons,
   contactInfo: state.contactInfo,
   socialLinks: state.socialLinks,
   sections: state.sections,
   appearance: state.appearance,
   subscriptionId: state.subscriptionId
 }

 const result = await profilesApi.create(payload)
 state.profileUrl = `/${result.slug}/`
 state.qrCodeSvg  = generateQr(state.profileUrl)  // uqr

 If the /profiles API endpoint doesn't yet support this full payload, a new endpoint POST /api/v1/profiles/card must be
  added to BizBio.API that accepts this structure and persists it using the entity/catalog/catalogcategory/catalogitem
 tables.

 Backend mapping (for API team / future implementation):
 - Entity (type=Person): slug, name="{firstName} {lastName}", logo=photoUrl
 - Catalog (linked to Entity): template name, appearance JSON stored in description/metadata field
 - CatalogCategory per section type: contact-buttons, contact-info, social, bio, map, skills, links, gallery, etc.
 - CatalogItem per row in each section: stores JSON data (label, value, href, platform, etc.)

 ---
 Dashboard Changes (pages/dashboard/index.vue)

 Remove the disabled state from the Business Cards product card:
 - Remove opacity-60 cursor-not-allowed
 - Remove "Coming Soon" badge
 - Add "Get Started" button that links to /dashboard/cards/create

 The "Business Cards" card should visually match the active "Digital Menu" card.

 ---
 QR Code Generation

 Use the existing uqr package (already installed):
 import { renderSVG } from 'uqr'
 const qrSvg = renderSVG(`https://bizbio.co.za/${slug}`)

 ---
 Drag-and-Drop

 Use the existing useDragDrop composable (SortableJS, already set up):
 - Contact Buttons: enableSortable on the buttons list container
 - Contact Info: enableSortable on the info items container
 - Social Links: enableSortable on the social items container
 - Sections list: enableSortable on the outer sections container (drag entire sections)
 - Items within sections (skills, links): enableSortable per section

 All drag handles use the .drag-handle class (already styled in the app).

 ---
 Routing & Auth

 - /dashboard/cards → pages/dashboard/cards/index.vue
 - /dashboard/cards/create → pages/dashboard/cards/create.vue
 - Both routes require auth middleware (same as all /dashboard/** routes — already handled by middleware/auth.ts)
 - Add to nuxt.config.ts route rules: ssr: false for /dashboard/cards/**

 ---
 Verification

 1. Navigate to /dashboard — Business Cards section should be enabled with a "Get Started" button
 2. Click "Get Started" → lands on /dashboard/cards/create
 3. Step 1: Complete "Help Me Choose" quiz → verify correct plan is recommended
 4. Step 1: Select a plan directly → verify plan is recorded in wizard state
 5. Step 2 Template tab: select a template → phone preview updates to show that template
 6. Step 2 Template tab: on Free plan, premium templates show lock indicator; clicking them does not select them
 7. Step 2 Content tab: fill in Basic Info → phone preview updates in real time
 8. Step 2 Content tab: add Contact Button → preview updates; Free plan blocks adding 3rd button
 9. Step 2 Content tab: drag-reorder Contact Info items → order reflects in preview
 10. Step 2 Content tab: add optional section (e.g., Skills) → section appears in preview
 11. Step 2 Content tab: Free plan → lock shown on premium sections (Map, Gallery, etc.)
 12. Step 2 Appearance tab: change primary color → preview updates live
 13. Step 3: QR code renders and links to the correct profile URL
 14. Step 3: "Copy link" button copies URL to clipboard
 15. Step 3: "Finish" → navigate to /dashboard/cards
 16. Verify API call is made with correct payload on save