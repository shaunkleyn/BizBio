export const useMenuCreation = () => {
  const menuData = useState('menuCreation', () => ({
    // Step 1: Entity Selection
    selectedEntity: null as any,

    // Step 2: Plan Selection
    selectedPlan: null as any,

    // Step 3: Menu Profile Info
    menuProfile: {
      name: '',
      description: '',
      businessName: '',
      businessLogo: null as File | null,
      businessLogoUrl: '',
      cuisine: '',
      phoneNumber: '',
      email: '',
      address: '',
      city: '',
      country: '',
      workingHours: {
        monday: { open: '09:00', close: '17:00', closed: false },
        tuesday: { open: '09:00', close: '17:00', closed: false },
        wednesday: { open: '09:00', close: '17:00', closed: false },
        thursday: { open: '09:00', close: '17:00', closed: false },
        friday: { open: '09:00', close: '17:00', closed: false },
        saturday: { open: '09:00', close: '17:00', closed: false },
        sunday: { open: '09:00', close: '17:00', closed: true },
      },
      // SEO fields
      enableSEO: false,
      slug: '',
      metaTitle: '',
      metaDescription: '',
      keywords: ''
    },

    // Step 4: Categories
    categories: [] as Array<{
      id: string
      name: string
      description: string
      icon: string
      order: number
    }>,

    // Step 5: Menu Items
    menuItems: [] as Array<{
      id: string
      categoryId: string
      name: string
      description: string
      price: number
      image: File | null
      imageUrl: string
      allergens: string[]
      dietary: string[]
      available: boolean
      featured: boolean
    }>,

    // Trial information
    trial: {
      startDate: null as Date | null,
      endDate: null as Date | null,
      daysRemaining: 14
    }
  }))

  const currentStep = useState('menuCreationStep', () => 1)

  // Menu plan tiers with limits
  const menuPlans = [
    {
      id: 'menu-starter',
      name: 'Starter',
      displayName: 'Menu Starter',
      price: 99,
      description: 'Perfect for small cafes and food trucks',
      features: [
        '1 menu',
        'Up to 3 categories',
        'Up to 20 menu items',
        'Basic customization',
        'QR code generation',
        'Mobile responsive',
      ],
      limits: {
        maxMenus: 1,
        categories: 3,
        itemsPerCategory: 20,
        totalItems: 20,
        images: true,
      },
      popular: false,
      trial: true,
      trialDays: 14
    },
    {
      id: 'menu-professional',
      name: 'Professional',
      displayName: 'Menu Professional',
      price: 199,
      description: 'Ideal for restaurants and bistros',
      features: [
        'Up to 3 menus',
        'Up to 10 categories per menu',
        'Up to 100 menu items per menu',
        'Advanced customization',
        'QR code generation',
        'Mobile responsive',
        'Allergen information',
        'Multi-language support',
      ],
      limits: {
        maxMenus: 3,
        categories: 10,
        itemsPerCategory: 100,
        totalItems: 100,
        images: true,
      },
      popular: true,
      trial: true,
      trialDays: 14
    },
    {
      id: 'menu-premium',
      name: 'Premium',
      displayName: 'Menu Premium',
      price: 399,
      description: 'For large restaurants and chains',
      features: [
        'Unlimited menus',
        'Unlimited categories',
        'Unlimited menu items',
        'Full customization',
        'QR code generation',
        'Mobile responsive',
        'Allergen information',
        'Multi-language support',
        'Analytics dashboard',
        'Priority support',
      ],
      limits: {
        maxMenus: 999,
        categories: 999,
        itemsPerCategory: 999,
        totalItems: 999,
        images: true,
      },
      popular: false,
      trial: true,
      trialDays: 14
    }
  ]

  const selectEntity = (entity: any) => {
    menuData.value.selectedEntity = entity
  }

  const selectPlan = (plan: any) => {
    menuData.value.selectedPlan = plan
    menuData.value.trial.startDate = new Date()
    const endDate = new Date()
    endDate.setDate(endDate.getDate() + plan.trialDays)
    menuData.value.trial.endDate = endDate
    menuData.value.trial.daysRemaining = plan.trialDays
  }

  const addCategory = (category: { name: string; description: string; icon: string }) => {
    const plan = menuData.value.selectedPlan
    if (plan && menuData.value.categories.length >= plan.limits.categories) {
      throw new Error(`You can only add up to ${plan.limits.categories} categories with your current plan`)
    }

    const newCategory = {
      id: Date.now().toString(),
      ...category,
      order: menuData.value.categories.length
    }
    menuData.value.categories.push(newCategory)
    return newCategory
  }

  const removeCategory = (categoryId: string) => {
    menuData.value.categories = menuData.value.categories.filter(c => c.id !== categoryId)
    // Also remove all items in this category
    menuData.value.menuItems = menuData.value.menuItems.filter(item => item.categoryId !== categoryId)
  }

  const addMenuItem = (item: any) => {
    const plan = menuData.value.selectedPlan
    if (plan && menuData.value.menuItems.length >= plan.limits.totalItems) {
      throw new Error(`You can only add up to ${plan.limits.totalItems} items with your current plan`)
    }

    const categoryItems = menuData.value.menuItems.filter(i => i.categoryId === item.categoryId)
    if (plan && categoryItems.length >= plan.limits.itemsPerCategory) {
      throw new Error(`You can only add up to ${plan.limits.itemsPerCategory} items per category with your current plan`)
    }

    const newItem = {
      id: Date.now().toString(),
      ...item,
      allergens: item.allergens || [],
      dietary: item.dietary || [],
      available: true,
      featured: false
    }
    menuData.value.menuItems.push(newItem)
    return newItem
  }

  const removeMenuItem = (itemId: string) => {
    menuData.value.menuItems = menuData.value.menuItems.filter(item => item.id !== itemId)
  }

  const nextStep = () => {
    if (currentStep.value < 5) {
      currentStep.value++
    }
  }

  const previousStep = () => {
    if (currentStep.value > 1) {
      currentStep.value--
    }
  }

  const goToStep = (step: number) => {
    if (step >= 1 && step <= 5) {
      currentStep.value = step
    }
  }

  const resetMenuCreation = () => {
    menuData.value = {
      selectedEntity: null,
      selectedPlan: null,
      menuProfile: {
        name: '',
        description: '',
        businessName: '',
        businessLogo: null,
        businessLogoUrl: '',
        cuisine: '',
        phoneNumber: '',
        email: '',
        address: '',
        city: '',
        country: '',
        workingHours: {
          monday: { open: '09:00', close: '17:00', closed: false },
          tuesday: { open: '09:00', close: '17:00', closed: false },
          wednesday: { open: '09:00', close: '17:00', closed: false },
          thursday: { open: '09:00', close: '17:00', closed: false },
          friday: { open: '09:00', close: '17:00', closed: false },
          saturday: { open: '09:00', close: '17:00', closed: false },
          sunday: { open: '09:00', close: '17:00', closed: true },
        },
        enableSEO: false,
        slug: '',
        metaTitle: '',
        metaDescription: '',
        keywords: ''
      },
      categories: [],
      menuItems: [],
      trial: {
        startDate: null,
        endDate: null,
        daysRemaining: 14
      }
    }
    currentStep.value = 1
  }

  const canProceedToNextStep = computed(() => {
    switch (currentStep.value) {
      case 1:
        // Can proceed if plan is selected
        return menuData.value.selectedPlan !== null
      case 2:
        // Can proceed if menu profile is filled
        return menuData.value.menuProfile.name &&
               menuData.value.menuProfile.businessName &&
               menuData.value.menuProfile.cuisine
      case 3:
        // Can proceed if at least one category exists
        return menuData.value.categories.length > 0
      case 4:
        // Can always complete from menu items step
        return true
      case 5:
        return true // Can always proceed from step 5
      default:
        return false
    }
  })

  // Helper function to generate URL-friendly slug
  const generateSlug = (text: string): string => {
    return text
      .toLowerCase()
      .trim()
      .replace(/[^\w\s-]/g, '') // Remove special characters
      .replace(/[\s_-]+/g, '-') // Replace spaces and underscores with hyphens
      .replace(/^-+|-+$/g, '') // Remove leading/trailing hyphens
  }

  return {
    menuData,
    currentStep,
    menuPlans,
    selectEntity,
    selectPlan,
    addCategory,
    removeCategory,
    addMenuItem,
    removeMenuItem,
    nextStep,
    previousStep,
    goToStep,
    resetMenuCreation,
    canProceedToNextStep,
    generateSlug
  }
}
