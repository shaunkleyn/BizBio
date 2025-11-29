<script setup>
import { ref } from 'vue'

const categories = ref([
  {
    name: 'Real Estate Agents',
    icon: 'fas fa-home',
    description: 'Find local real estate professionals'
  },
  {
    name: 'Financial Advisors',
    icon: 'fas fa-chart-line',
    description: 'Connect with financial experts'
  },
  {
    name: 'Healthcare Professionals',
    icon: 'fas fa-heartbeat',
    description: 'Discover healthcare providers'
  },
  {
    name: 'Legal Services',
    icon: 'fas fa-gavel',
    description: 'Find legal professionals'
  },
  {
    name: 'Tech & IT Consultants',
    icon: 'fas fa-laptop-code',
    description: 'Connect with tech experts'
  },
  {
    name: 'Management Consultants',
    icon: 'fas fa-briefcase',
    description: 'Find business consultants'
  }
])
</script>

<template>
  <div class="min-h-screen bg-gray-50 py-12">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h1 class="text-4xl font-heading font-bold text-brand-dark-text mb-4">
          Business Categories
        </h1>
        <p class="text-xl text-brand-gray-text max-w-2xl mx-auto">
          Browse professionals by category
        </p>
      </div>

      <div class="grid sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="(category, index) in categories"
          :key="index"
          class="bg-white rounded-lg p-6 hover:shadow-lg transition-all cursor-pointer group"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center text-primary text-xl group-hover:bg-primary group-hover:text-white transition-all">
              <i :class="category.icon"></i>
            </div>
            <div>
              <h3 class="font-heading font-bold text-brand-dark-text">{{ category.name }}</h3>
              <p class="text-sm text-brand-gray-text">{{ category.description }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
