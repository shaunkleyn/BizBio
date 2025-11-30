import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/Home.vue')
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/auth/Login.vue')
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('../views/auth/Register.vue')
  },
  {
    path: '/verify-email',
    name: 'VerifyEmail',
    component: () => import('../views/auth/VerifyEmail.vue')
  },
  {
    path: '/forgot-password',
    name: 'ForgotPassword',
    component: () => import('../views/auth/ForgotPassword.vue')
  },
  {
    path: '/reset-password',
    name: 'ResetPassword',
    component: () => import('../views/auth/ResetPassword.vue')
  },
  {
    path: '/pricing',
    name: 'Pricing',
    component: () => import('../views/Pricing.vue')
  },
  {
    path: '/categories',
    name: 'Categories',
    component: () => import('../views/Categories.vue')
  },
  {
    path: '/search',
    name: 'Search',
    component: () => import('../views/Search.vue')
  },
  {
    path: '/help',
    name: 'Help',
    component: () => import('../views/Help.vue')
  },
  {
    path: '/contact',
    name: 'Contact',
    component: () => import('../views/Contact.vue')
  },
  {
    path: '/terms',
    name: 'Terms',
    component: () => import('../views/Terms.vue')
  },
  {
    path: '/privacy',
    name: 'Privacy',
    component: () => import('../views/Privacy.vue')
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('../views/dashboard/Dashboard.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/dashboard/profile',
    name: 'ProfileEditor',
    component: () => import('../views/dashboard/ProfileEditor.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/dashboard/menu',
    name: 'MenuEditor',
    component: () => import('../views/dashboard/MenuEditor.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/dashboard/tables',
    name: 'TablesManager',
    component: () => import('../views/dashboard/TablesManager.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/dashboard/subscription',
    name: 'Subscription',
    component: () => import('../views/dashboard/Subscription.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/:slug',
    name: 'ProfileView',
    component: () => import('../views/ProfileView.vue')
  },
  {
    path: '/menu/:profileSlug',
    name: 'MenuView',
    component: () => import('../views/MenuView.vue')
  },
  {
    path: '/menu/:profileSlug/table/:tableId',
    name: 'TableMenuView',
    component: () => import('../views/TableMenuView.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Navigation guard for authentication
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'Login', query: { redirect: to.fullPath } })
  } else {
    next()
  }
})

export default router
