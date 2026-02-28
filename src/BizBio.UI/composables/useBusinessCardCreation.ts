import { renderSVG } from 'uqr'

export interface ContactButton {
  id: string
  type: string
  value: string
  label: string
}

export interface ContactInfoItem {
  id: string
  type: string
  label: string
  value: string
  href: string
}

export interface SocialLink {
  id: string
  platform: string
  url: string
}

export interface CardSection {
  id: string
  type: 'bio' | 'map' | 'skills' | 'links' | 'gallery' | 'save-contact' | 'share' | 'google-wallet' | 'apple-wallet'
  sortOrder: number
  data: Record<string, any>
}

export const CARD_PATTERNS: Array<{ id: string; label: string }> = [
  { id: 'dots', label: 'Dots' },
  { id: 'grid', label: 'Grid' },
  { id: 'diagonal', label: 'Diagonal' },
  { id: 'cross', label: 'Cross' },
  { id: 'circles', label: 'Circles' },
  { id: 'hex', label: 'Hex' },
  { id: 'waves', label: 'Waves' },
  { id: 'confetti', label: 'Confetti' },
]

export interface CardAppearance {
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
  cardBorderEnabled: boolean
  cardShadowEnabled: boolean
  headingFont: string
  bodyFont: string
  titleColor: string
  subtitleColor: string
  bodyTextColor: string
  infoLabelColor: string
  primaryColor: string
  buttonStyle: 'filled' | 'outlined' | 'ghost' | 'pill'
  buttonTextColor: string
  iconStyle: 'circle' | 'square' | 'rounded' | 'none'
  walletBgColor: string
  walletIssuerName: string
  walletHeroImageUrl: string
  appleWalletBgColor: string
}

export type PlanId = 'free' | 'solo' | 'team' | 'business'

export interface WizardState {
  currentStep: 1 | 2 | 3
  activeTab: 'template' | 'content' | 'appearance'

  // Plan
  selectedPlan: PlanId | null
  subscriptionId: string | null

  // Template
  selectedTemplate: string

  // Basic Info
  firstName: string
  lastName: string
  headline: string
  company: string
  photoUrl: string
  bio: string

  // Contact Buttons
  contactButtons: ContactButton[]

  // Contact Info items
  contactInfo: ContactInfoItem[]

  // Social Links
  socialLinks: SocialLink[]

  // Optional sections
  sections: CardSection[]

  // Appearance
  appearance: CardAppearance

  // Publish
  slug: string
  profileUrl: string
  qrCodeSvg: string
}

export const FREE_TEMPLATES = [
  '01-minimalist-clean',
  '02-corporate-professional',
  '03-bold-creative',
  '04-dark-elegant',
  '05-gradient-modern'
]

export const ALL_TEMPLATES = [
  { name: '01-minimalist-clean', label: 'Minimalist Clean' },
  { name: '02-corporate-professional', label: 'Corporate Professional' },
  { name: '03-bold-creative', label: 'Bold Creative' },
  { name: '04-dark-elegant', label: 'Dark Elegant' },
  { name: '05-gradient-modern', label: 'Gradient Modern' },
  { name: '06-glassmorphism', label: 'Glassmorphism' },
  { name: '07-neumorphism', label: 'Neumorphism' },
  { name: '08-split-screen', label: 'Split Screen' },
  { name: '09-photo-centric', label: 'Photo Centric' },
  { name: '10-tech-futuristic', label: 'Tech Futuristic' },
  { name: '11-organic-nature', label: 'Organic Nature' },
  { name: '12-retro-vintage', label: 'Retro Vintage' },
  { name: '13-social-media-focused', label: 'Social Media' },
  { name: '14-card-stack', label: 'Card Stack' },
  { name: '15-compact-rounded', label: 'Compact Rounded' },
  { name: '16-mono-elegant', label: 'Mono Elegant' },
  { name: '17-vibrant-gradient', label: 'Vibrant Gradient' },
  { name: '18-professional-blue', label: 'Professional Blue' },
  { name: '19-soft-pastel', label: 'Soft Pastel' },
  { name: '20-executive-premium', label: 'Executive Premium' },
  { name: '21-navy-corporate-wave', label: 'Navy Corporate Wave' },
  { name: '22-gradient-wave-stats', label: 'Gradient Wave Stats' },
  { name: '23-glassmorphism-blob', label: 'Glassmorphism Blob' },
  { name: '24-dark-neumorphic', label: 'Dark Neumorphic' },
  { name: '25-designer-portfolio', label: 'Designer Portfolio' },
  { name: '26-teal-profile-stats', label: 'Teal Profile Stats' },
  { name: '27-creative-wave-header', label: 'Creative Wave Header' },
  { name: '28-photo-hero-influencer', label: 'Photo Hero Influencer' },
  { name: '29-blue-dashboard', label: 'Blue Dashboard' },
  { name: '30-corporate-logo-card', label: 'Corporate Logo Card' },
  { name: '31-corporate-contact', label: 'Corporate Contact' },
  { name: '32-team-profile', label: 'Team Profile' },
  { name: '33-review-card', label: 'Review Card' },
  { name: '34-gradient-cutout-logo', label: 'Gradient Cutout Logo' },
  { name: '35-gradient-card', label: 'Gradient Card' },
]

export interface TemplateDefaults {
  primaryColor: string
  bgType: 'solid' | 'gradient'
  bgColor: string
  bgGradientFrom?: string
  bgGradientTo?: string
  bgGradientDir?: string
  cardBgColor: string
  titleColor: string
  subtitleColor: string
  bodyTextColor: string
  infoLabelColor: string
  buttonTextColor: string
  headingFont: string
  bodyFont: string
}

/** Per-template default appearance values extracted from their CSS files. */
export const TEMPLATE_DEFAULTS: Record<string, TemplateDefaults> = {
  '01-minimalist-clean': {
    primaryColor: '#10b981', bgType: 'solid', bgColor: '#f5f5f5',
    cardBgColor: '#ffffff', titleColor: '#1a1a1a', subtitleColor: '#666666',
    bodyTextColor: '#1a1a1a', infoLabelColor: '#999999', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '02-corporate-professional': {
    primaryColor: '#1e40af', bgType: 'gradient', bgColor: '#0f172a',
    bgGradientFrom: '#0f172a', bgGradientTo: '#1e293b', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#1e293b', subtitleColor: '#1e40af',
    bodyTextColor: '#1e293b', infoLabelColor: '#64748b', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '03-bold-creative': {
    primaryColor: '#ff6b6b', bgType: 'solid', bgColor: '#ff6b6b',
    cardBgColor: '#ffffff', titleColor: '#1e293b', subtitleColor: '#ff6b6b',
    bodyTextColor: '#1e293b', infoLabelColor: '#64748b', buttonTextColor: '#ffffff',
    headingFont: 'Poppins', bodyFont: 'Poppins',
  },
  '04-dark-elegant': {
    primaryColor: '#d4af37', bgType: 'solid', bgColor: '#0a0a0a',
    cardBgColor: '#151515', titleColor: '#ffffff', subtitleColor: '#d4af37',
    bodyTextColor: '#ffffff', infoLabelColor: '#888888', buttonTextColor: '#d4af37',
    headingFont: 'Montserrat', bodyFont: 'Montserrat',
  },
  '05-gradient-modern': {
    primaryColor: '#667eea', bgType: 'gradient', bgColor: '#667eea',
    bgGradientFrom: '#667eea', bgGradientTo: '#f093fb', bgGradientDir: '135deg',
    cardBgColor: 'rgba(255,255,255,0.95)', titleColor: '#2d3748', subtitleColor: '#667eea',
    bodyTextColor: '#2d3748', infoLabelColor: '#718096', buttonTextColor: '#ffffff',
    headingFont: 'Poppins', bodyFont: 'Poppins',
  },
  '06-glassmorphism': {
    primaryColor: '#e94560', bgType: 'solid', bgColor: '#1a1a2e',
    cardBgColor: 'rgba(255,255,255,0.1)', titleColor: '#ffffff', subtitleColor: '#e94560',
    bodyTextColor: '#ffffff', infoLabelColor: 'rgba(255,255,255,0.6)', buttonTextColor: '#ffffff',
    headingFont: 'Poppins', bodyFont: 'Poppins',
  },
  '07-neumorphism': {
    primaryColor: '#ee6c4d', bgType: 'solid', bgColor: '#e0e5ec',
    cardBgColor: '#e0e5ec', titleColor: '#293241', subtitleColor: '#ee6c4d',
    bodyTextColor: '#293241', infoLabelColor: '#98c1d9', buttonTextColor: '#3d5a80',
    headingFont: 'Nunito', bodyFont: 'Nunito',
  },
  '08-split-screen': {
    primaryColor: '#3498db', bgType: 'solid', bgColor: '#2c3e50',
    cardBgColor: '#ffffff', titleColor: '#2c3e50', subtitleColor: '#3498db',
    bodyTextColor: '#2c3e50', infoLabelColor: '#7f8c8d', buttonTextColor: '#ffffff',
    headingFont: 'Raleway', bodyFont: 'Raleway',
  },
  '09-photo-centric': {
    primaryColor: '#10b981', bgType: 'solid', bgColor: '#111111',
    cardBgColor: '#111111', titleColor: '#ffffff', subtitleColor: '#10b981',
    bodyTextColor: '#ffffff', infoLabelColor: '#6b7280', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '10-tech-futuristic': {
    primaryColor: '#00ff88', bgType: 'solid', bgColor: '#050510',
    cardBgColor: 'rgba(255,255,255,0.05)', titleColor: '#ffffff', subtitleColor: '#00c8ff',
    bodyTextColor: '#ffffff', infoLabelColor: '#00c8ff', buttonTextColor: '#050510',
    headingFont: 'Orbitron', bodyFont: 'Orbitron',
  },
  '11-organic-nature': {
    primaryColor: '#6d8b74', bgType: 'gradient', bgColor: '#a4c3a2',
    bgGradientFrom: '#a4c3a2', bgGradientTo: '#eae0d5', bgGradientDir: '180deg',
    cardBgColor: 'rgba(255,255,255,0.9)', titleColor: '#5f7161', subtitleColor: '#6d8b74',
    bodyTextColor: '#5f7161', infoLabelColor: '#a4c3a2', buttonTextColor: '#ffffff',
    headingFont: 'Quicksand', bodyFont: 'Quicksand',
  },
  '12-retro-vintage': {
    primaryColor: '#8b4513', bgType: 'solid', bgColor: '#f4e9d8',
    cardBgColor: '#fffef9', titleColor: '#4a3728', subtitleColor: '#8b4513',
    bodyTextColor: '#4a3728', infoLabelColor: '#cd853f', buttonTextColor: '#fffef9',
    headingFont: 'Playfair Display', bodyFont: 'Playfair Display',
  },
  '13-social-media-focused': {
    primaryColor: '#667eea', bgType: 'gradient', bgColor: '#667eea',
    bgGradientFrom: '#667eea', bgGradientTo: '#764ba2', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#1a1a2e', subtitleColor: '#667eea',
    bodyTextColor: '#1a1a2e', infoLabelColor: '#718096', buttonTextColor: '#ffffff',
    headingFont: 'Poppins', bodyFont: 'Poppins',
  },
  '14-card-stack': {
    primaryColor: '#6366f1', bgType: 'gradient', bgColor: '#6366f1',
    bgGradientFrom: '#6366f1', bgGradientTo: '#8b5cf6', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#1e1b4b', subtitleColor: '#6366f1',
    bodyTextColor: '#1e1b4b', infoLabelColor: '#a5b4fc', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '15-compact-rounded': {
    primaryColor: '#f85032', bgType: 'gradient', bgColor: '#f85032',
    bgGradientFrom: '#f85032', bgGradientTo: '#e73827', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#2d3436', subtitleColor: '#f85032',
    bodyTextColor: '#2d3436', infoLabelColor: '#b2bec3', buttonTextColor: '#ffffff',
    headingFont: 'Nunito', bodyFont: 'Nunito',
  },
  '16-mono-elegant': {
    primaryColor: '#000000', bgType: 'solid', bgColor: '#ffffff',
    cardBgColor: '#ffffff', titleColor: '#000000', subtitleColor: '#666666',
    bodyTextColor: '#000000', infoLabelColor: '#999999', buttonTextColor: '#ffffff',
    headingFont: 'Cormorant Garamond', bodyFont: 'Cormorant Garamond',
  },
  '17-vibrant-gradient': {
    primaryColor: '#ff0844', bgType: 'gradient', bgColor: '#ff0844',
    bgGradientFrom: '#ff0844', bgGradientTo: '#feca57', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#2d3436', subtitleColor: '#ff0844',
    bodyTextColor: '#2d3436', infoLabelColor: '#b2bec3', buttonTextColor: '#ffffff',
    headingFont: 'Outfit', bodyFont: 'Outfit',
  },
  '18-professional-blue': {
    primaryColor: '#003366', bgType: 'solid', bgColor: '#f0f4f8',
    cardBgColor: '#ffffff', titleColor: '#003366', subtitleColor: '#66b3ff',
    bodyTextColor: '#003366', infoLabelColor: '#66b3ff', buttonTextColor: '#ffffff',
    headingFont: 'Lato', bodyFont: 'Lato',
  },
  '19-soft-pastel': {
    primaryColor: '#a78bfa', bgType: 'gradient', bgColor: '#fed6e3',
    bgGradientFrom: '#fed6e3', bgGradientTo: '#a78bfa', bgGradientDir: '180deg',
    cardBgColor: 'rgba(255,255,255,0.85)', titleColor: '#4c1d95', subtitleColor: '#a78bfa',
    bodyTextColor: '#4c1d95', infoLabelColor: '#c4b5fd', buttonTextColor: '#ffffff',
    headingFont: 'DM Sans', bodyFont: 'DM Sans',
  },
  '20-executive-premium': {
    primaryColor: '#d4af37', bgType: 'solid', bgColor: '#1a1a2e',
    cardBgColor: '#1f1f35', titleColor: '#ffffff', subtitleColor: '#d4af37',
    bodyTextColor: '#ffffff', infoLabelColor: '#888888', buttonTextColor: '#1a1a2e',
    headingFont: 'Cormorant Garamond', bodyFont: 'Cormorant Garamond',
  },
  '21-navy-corporate-wave': {
    primaryColor: '#00b4d8', bgType: 'solid', bgColor: '#0a2463',
    cardBgColor: '#ffffff', titleColor: '#0a2463', subtitleColor: '#00b4d8',
    bodyTextColor: '#0a2463', infoLabelColor: '#90e0ef', buttonTextColor: '#ffffff',
    headingFont: 'Montserrat', bodyFont: 'Montserrat',
  },
  '22-gradient-wave-stats': {
    primaryColor: '#a855f7', bgType: 'gradient', bgColor: '#a855f7',
    bgGradientFrom: '#a855f7', bgGradientTo: '#ec4899', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#1f2937', subtitleColor: '#a855f7',
    bodyTextColor: '#1f2937', infoLabelColor: '#9ca3af', buttonTextColor: '#ffffff',
    headingFont: 'Plus Jakarta Sans', bodyFont: 'Plus Jakarta Sans',
  },
  '23-glassmorphism-blob': {
    primaryColor: '#ec4899', bgType: 'solid', bgColor: '#1e1b4b',
    cardBgColor: 'rgba(255,255,255,0.1)', titleColor: '#ffffff', subtitleColor: '#ec4899',
    bodyTextColor: '#ffffff', infoLabelColor: 'rgba(255,255,255,0.6)', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '24-dark-neumorphic': {
    primaryColor: '#3b82f6', bgType: 'solid', bgColor: '#2d3748',
    cardBgColor: '#2d3748', titleColor: '#ffffff', subtitleColor: '#3b82f6',
    bodyTextColor: '#ffffff', infoLabelColor: '#a0aec0', buttonTextColor: '#3b82f6',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '25-designer-portfolio': {
    primaryColor: '#3b82f6', bgType: 'solid', bgColor: '#1e293b',
    cardBgColor: '#1e293b', titleColor: '#ffffff', subtitleColor: '#22c55e',
    bodyTextColor: '#ffffff', infoLabelColor: '#64748b', buttonTextColor: '#3b82f6',
    headingFont: 'Space Grotesk', bodyFont: 'Space Grotesk',
  },
  '26-teal-profile-stats': {
    primaryColor: '#0d9488', bgType: 'gradient', bgColor: '#0d9488',
    bgGradientFrom: '#0d9488', bgGradientTo: '#14b8a6', bgGradientDir: '180deg',
    cardBgColor: '#ffffff', titleColor: '#134e4a', subtitleColor: '#0d9488',
    bodyTextColor: '#134e4a', infoLabelColor: '#5eead4', buttonTextColor: '#ffffff',
    headingFont: 'Nunito', bodyFont: 'Nunito',
  },
  '27-creative-wave-header': {
    primaryColor: '#c0847e', bgType: 'gradient', bgColor: '#c0847e',
    bgGradientFrom: '#c0847e', bgGradientTo: '#7eb5b0', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#4a4a4a', subtitleColor: '#c0847e',
    bodyTextColor: '#4a4a4a', infoLabelColor: '#b0b0b0', buttonTextColor: '#ffffff',
    headingFont: 'Josefin Sans', bodyFont: 'Josefin Sans',
  },
  '28-photo-hero-influencer': {
    primaryColor: '#ffffff', bgType: 'solid', bgColor: '#1a1a1a',
    cardBgColor: '#1a1a1a', titleColor: '#ffffff', subtitleColor: '#cccccc',
    bodyTextColor: '#ffffff', infoLabelColor: '#888888', buttonTextColor: '#1a1a1a',
    headingFont: 'Poppins', bodyFont: 'Poppins',
  },
  '29-blue-dashboard': {
    primaryColor: '#2563eb', bgType: 'solid', bgColor: '#f1f5f9',
    cardBgColor: '#ffffff', titleColor: '#1e293b', subtitleColor: '#2563eb',
    bodyTextColor: '#1e293b', infoLabelColor: '#94a3b8', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '30-corporate-logo-card': {
    primaryColor: '#22c55e', bgType: 'solid', bgColor: '#1f2937',
    cardBgColor: '#ffffff', titleColor: '#1f2937', subtitleColor: '#22c55e',
    bodyTextColor: '#1f2937', infoLabelColor: '#9ca3af', buttonTextColor: '#ffffff',
    headingFont: 'Poppins', bodyFont: 'Poppins',
  },
  '31-corporate-contact': {
    primaryColor: '#4f8ef7', bgType: 'solid', bgColor: '#0d1b3e',
    cardBgColor: '#ffffff', titleColor: '#1a2744', subtitleColor: '#4f8ef7',
    bodyTextColor: '#1a2744', infoLabelColor: '#64748b', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '32-team-profile': {
    primaryColor: '#0d1b3e', bgType: 'solid', bgColor: '#0d1b3e',
    cardBgColor: '#ffffff', titleColor: '#ffffff', subtitleColor: '#90caf9',
    bodyTextColor: '#ffffff', infoLabelColor: 'rgba(255,255,255,0.7)', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '33-review-card': {
    primaryColor: '#00b4d8', bgType: 'gradient', bgColor: '#00b4d8',
    bgGradientFrom: '#00b4d8', bgGradientTo: '#7b2d8b', bgGradientDir: '135deg',
    cardBgColor: '#ffffff', titleColor: '#1a1a2e', subtitleColor: '#00b4d8',
    bodyTextColor: '#1a1a2e', infoLabelColor: '#666666', buttonTextColor: '#ffffff',
    headingFont: 'Inter', bodyFont: 'Inter',
  },
  '34-gradient-cutout-logo': {
    primaryColor: '#00c4e0', bgType: 'gradient', bgColor: '#00c4e0',
    bgGradientFrom: '#00c4e0', bgGradientTo: '#b520b5', bgGradientDir: '150deg',
    cardBgColor: '#ffffff', titleColor: '#1a1a2e', subtitleColor: '#00c4e0',
    bodyTextColor: '#1a1a2e', infoLabelColor: '#888888', buttonTextColor: '#ffffff',
    headingFont: 'Poppins', bodyFont: 'Poppins',
  },
  '35-gradient-card': {
    primaryColor: '#9b55c8', bgType: 'solid', bgColor: '#e2e2e2',
    cardBgColor: '#ffffff', titleColor: '#333333', subtitleColor: '#9b55c8',
    bodyTextColor: '#555555', infoLabelColor: '#aaaaaa', buttonTextColor: '#ffffff',
    headingFont: 'Nunito', bodyFont: 'Nunito',
  },
}

export const CARD_PLANS = [
  {
    id: 'free' as PlanId,
    name: 'Free',
    price: 0,
    priceLabel: 'R0',
    description: 'Perfect for getting started',
    features: [
      '1 digital card',
      '5 basic templates',
      'QR code generation',
      'vCard download',
      'Max 2 contact buttons',
      'Max 5 contact info items',
      'Max 5 social links',
      'BizBio badge',
    ],
    popular: false,
  },
  {
    id: 'solo' as PlanId,
    name: 'Solo',
    price: 59,
    priceLabel: 'R59/mo',
    description: 'For individuals and freelancers',
    features: [
      '1 digital card',
      'All premium templates',
      'Custom branding',
      'Analytics dashboard',
      'Unlimited contact items',
      'Unlimited social links',
      '3 contact buttons',
      'No BizBio badge',
      'Map, Skills, Links sections',
      'Wallet integration',
    ],
    popular: true,
  },
  {
    id: 'team' as PlanId,
    name: 'Team',
    price: 99,
    priceLabel: 'R99/mo',
    description: 'For small teams',
    features: [
      '5 digital cards',
      'Everything in Solo',
      'Team management',
    ],
    popular: false,
  },
  {
    id: 'business' as PlanId,
    name: 'Business',
    price: 249,
    priceLabel: 'R249/mo',
    description: 'For growing businesses',
    features: [
      '20 digital cards',
      'Everything in Team',
      'Advanced analytics',
      'Gallery section',
      'Priority support',
    ],
    popular: false,
  },
]

export const OPTIONAL_SECTION_DEFS = [
  { type: 'bio', label: 'Bio', icon: 'fas fa-align-left', plan: 'free' },
  { type: 'save-contact', label: 'Save Contact Button', icon: 'fas fa-address-book', plan: 'free' },
  { type: 'share', label: 'Share Button', icon: 'fas fa-share-alt', plan: 'free' },
  { type: 'map', label: 'Map', icon: 'fas fa-map-marker-alt', plan: 'solo' },
  { type: 'skills', label: 'Skills / Tags', icon: 'fas fa-tags', plan: 'solo' },
  { type: 'links', label: 'Links', icon: 'fas fa-link', plan: 'solo' },
  { type: 'google-wallet', label: 'Google Wallet', icon: 'fab fa-google-pay', plan: 'solo' },
  { type: 'apple-wallet', label: 'Apple Wallet', icon: 'fab fa-apple', plan: 'solo' },
  { type: 'gallery', label: 'Gallery', icon: 'fas fa-images', plan: 'business' },
] as const

const defaultAppearance: CardAppearance = {
  bgType: 'solid',
  bgColor: '#f0f4ff',
  bgGradientFrom: '#667eea',
  bgGradientTo: '#764ba2',
  bgGradientDir: '135deg',
  bgImageUrl: '',
  bgPattern: '',
  cardBgColor: '#ffffff',
  cardBorderColor: '#e2e8f0',
  cardBorderRadius: 16,
  cardBorderEnabled: false,
  cardShadowEnabled: true,
  headingFont: 'Inter',
  bodyFont: 'Inter',
  titleColor: '#1a202c',
  subtitleColor: '#4a5568',
  bodyTextColor: '#718096',
  infoLabelColor: '#a0aec0',
  primaryColor: '#667eea',
  buttonStyle: 'filled',
  buttonTextColor: '#ffffff',
  iconStyle: 'circle',
  walletBgColor: '#1a1a2e',
  walletIssuerName: '',
  walletHeroImageUrl: '',
  appleWalletBgColor: '#000000',
}

export const useBusinessCardCreation = () => {
  const state = useState<WizardState>('businessCardCreation', () => ({
    currentStep: 1,
    activeTab: 'template',
    selectedPlan: null,
    subscriptionId: null,
    selectedTemplate: '01-minimalist-clean',
    firstName: '',
    lastName: '',
    headline: '',
    company: '',
    photoUrl: '',
    bio: '',
    contactButtons: [],
    contactInfo: [],
    socialLinks: [],
    sections: [],
    appearance: { ...defaultAppearance },
    slug: '',
    profileUrl: '',
    qrCodeSvg: '',
  }))

  // Plan limit computed helpers
  const maxContactButtons = computed(() =>
    state.value.selectedPlan === 'free' ? 2 : 3
  )

  const maxContactInfo = computed(() =>
    state.value.selectedPlan === 'free' ? 5 : Infinity
  )

  const maxSocialLinks = computed(() =>
    state.value.selectedPlan === 'free' ? 5 : Infinity
  )

  const canUseTemplate = (name: string) =>
    state.value.selectedPlan !== 'free' || FREE_TEMPLATES.includes(name)

  const planOrder: PlanId[] = ['free', 'solo', 'team', 'business']

  const canAddSection = (type: string): { allowed: boolean; reason?: string } => {
    const def = OPTIONAL_SECTION_DEFS.find(d => d.type === type)
    if (!def) return { allowed: true }

    const currentPlanIdx = planOrder.indexOf(state.value.selectedPlan ?? 'free')
    const requiredIdx = planOrder.indexOf(def.plan as PlanId)

    if (currentPlanIdx < requiredIdx) {
      const planName = def.plan.charAt(0).toUpperCase() + def.plan.slice(1)
      return { allowed: false, reason: `Requires ${planName} plan` }
    }
    return { allowed: true }
  }

  const generateSlug = (firstName: string, lastName: string): string => {
    const base = `${firstName}-${lastName}`
      .toLowerCase()
      .trim()
      .replace(/[^\w\s-]/g, '')
      .replace(/[\s_-]+/g, '-')
      .replace(/^-+|-+$/g, '')
    const suffix = Math.random().toString(36).substring(2, 6)
    return `${base}-${suffix}`
  }

  const generateQrCode = (url: string): string => {
    try {
      return renderSVG(url)
    } catch {
      return ''
    }
  }

  const selectPlan = (planId: PlanId) => {
    state.value.selectedPlan = planId
  }

  const nextStep = () => {
    if (state.value.currentStep < 3) {
      state.value.currentStep = (state.value.currentStep + 1) as 1 | 2 | 3
    }
  }

  const previousStep = () => {
    if (state.value.currentStep > 1) {
      state.value.currentStep = (state.value.currentStep - 1) as 1 | 2 | 3
    }
  }

  const goToStep = (step: 1 | 2 | 3) => {
    state.value.currentStep = step
  }

  const canProceedFromStep = computed(() => {
    switch (state.value.currentStep) {
      case 1:
        return state.value.selectedPlan !== null
      case 2:
        return state.value.firstName.trim().length > 0
      case 3:
        return true
      default:
        return false
    }
  })

  const addContactButton = () => {
    if (state.value.contactButtons.length >= maxContactButtons.value) return
    state.value.contactButtons.push({
      id: Date.now().toString(),
      type: 'phone',
      value: '',
      label: '',
    })
  }

  const removeContactButton = (id: string) => {
    state.value.contactButtons = state.value.contactButtons.filter(b => b.id !== id)
  }

  const addContactInfo = () => {
    if (state.value.contactInfo.length >= maxContactInfo.value) return
    state.value.contactInfo.push({
      id: Date.now().toString(),
      type: 'phone',
      label: '',
      value: '',
      href: '',
    })
  }

  const removeContactInfo = (id: string) => {
    state.value.contactInfo = state.value.contactInfo.filter(i => i.id !== id)
  }

  const addSocialLink = () => {
    if (state.value.socialLinks.length >= maxSocialLinks.value) return
    state.value.socialLinks.push({
      id: Date.now().toString(),
      platform: 'linkedin',
      url: '',
    })
  }

  const removeSocialLink = (id: string) => {
    state.value.socialLinks = state.value.socialLinks.filter(s => s.id !== id)
  }

  const sectionDefaults: Record<string, Record<string, any>> = {
    bio: { header: '', text: '' },
    'save-contact': { header: 'Save Contact', enabled: true },
    share: { header: 'Share Profile', enabled: true },
    map: { header: '', address: '' },
    skills: { header: '', skills: [] },
    links: { header: '', links: [] },
    'google-wallet': { header: '' },
    'apple-wallet': { header: '' },
    gallery: { header: '', images: [] },
  }

  const addSection = (type: CardSection['type']) => {
    const check = canAddSection(type)
    if (!check.allowed) return

    if (state.value.sections.find(s => s.type === type)) return

    state.value.sections.push({
      id: Date.now().toString(),
      type,
      sortOrder: state.value.sections.length,
      data: { ...(sectionDefaults[type] ?? {}) },
    })
  }

  const removeSection = (id: string) => {
    state.value.sections = state.value.sections.filter(s => s.id !== id)
    // Re-index sort orders
    state.value.sections.forEach((s, i) => { s.sortOrder = i })
  }

  const resetWizard = () => {
    state.value = {
      currentStep: 1,
      activeTab: 'template',
      selectedPlan: null,
      subscriptionId: null,
      selectedTemplate: '01-minimalist-clean',
      firstName: '',
      lastName: '',
      headline: '',
      company: '',
      photoUrl: '',
      bio: '',
      contactButtons: [],
      contactInfo: [],
      socialLinks: [],
      sections: [],
      appearance: { ...defaultAppearance },
      slug: '',
      profileUrl: '',
      qrCodeSvg: '',
    }
  }

  return {
    state,
    // Plan
    CARD_PLANS,
    FREE_TEMPLATES,
    ALL_TEMPLATES,
    OPTIONAL_SECTION_DEFS,
    selectPlan,
    // Limits
    maxContactButtons,
    maxContactInfo,
    maxSocialLinks,
    canUseTemplate,
    canAddSection,
    // Navigation
    nextStep,
    previousStep,
    goToStep,
    canProceedFromStep,
    // Content mutations
    addContactButton,
    removeContactButton,
    addContactInfo,
    removeContactInfo,
    addSocialLink,
    removeSocialLink,
    addSection,
    removeSection,
    // Utilities
    generateSlug,
    generateQrCode,
    resetWizard,
  }
}
