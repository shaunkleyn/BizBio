# BaseModal Component

A reusable, standardized modal component with consistent styling across the entire application.

## Features

- ✅ Gradient header with title & subtitle
- ✅ Mesh card background
- ✅ Scrollable body with fixed header/footer
- ✅ Customizable size (sm, md, lg, xl, full)
- ✅ Flexible footer (custom or default buttons)
- ✅ Loading states
- ✅ Disabled states
- ✅ Blur backdrop with fade animation
- ✅ Red circular close button

## Basic Usage

```vue
<template>
  <BaseModal
    v-model="showModal"
    title="Create Item"
    subtitle="Add a new menu item"
    @confirm="handleSave"
  >
    <!-- Your form content here -->
    <div class="space-y-4">
      <input v-model="form.name" placeholder="Item name" />
    </div>
  </BaseModal>
</template>

<script setup>
import { ref } from 'vue'

const showModal = ref(false)
const form = ref({ name: '' })

function handleSave() {
  // Save logic
  console.log('Saving...', form.value)
  showModal.value = false
}
</script>
```

## Props

### Required
- `modelValue: boolean` - Controls modal visibility (use v-model)
- `title: string` - Modal title

### Optional
- `subtitle?: string` - Subtitle below title
- `size?: 'sm' | 'md' | 'lg' | 'xl' | 'full'` - Modal width (default: 'md')
- `showClose?: boolean` - Show close button (default: true)
- `showFooter?: boolean` - Show footer (default: true)
- `showCancel?: boolean` - Show cancel button (default: true)
- `showConfirm?: boolean` - Show confirm button (default: true)
- `cancelText?: string` - Cancel button text (default: 'Cancel')
- `confirmText?: string` - Confirm button text (default: 'Confirm')
- `confirmIcon?: string` - Icon class for confirm button (default: 'fas fa-check')
- `confirmLoading?: boolean` - Show loading spinner on confirm button (default: false)
- `confirmDisabled?: boolean` - Disable confirm button (default: false)
- `footerAlign?: 'left' | 'center' | 'right' | 'between'` - Footer alignment (default: 'right')
- `cancelButtonClass?: string` - Additional classes for cancel button
- `confirmButtonClass?: string` - Additional classes for confirm button

## Events

- `@update:modelValue` - Emitted when modal is closed
- `@close` - Emitted when modal is closed
- `@confirm` - Emitted when confirm button is clicked

## Slots

### Default Slot
The main content area (scrollable)

```vue
<BaseModal v-model="show" title="Title">
  <div>Your content here</div>
</BaseModal>
```

### Footer Slot
Custom footer (overrides default buttons)

```vue
<BaseModal v-model="show" title="Title">
  <div>Content</div>
  
  <template #footer>
    <div class="flex justify-between w-full">
      <button @click="handleDelete">Delete</button>
      <button @click="handleSave">Save</button>
    </div>
  </template>
</BaseModal>
```

## Examples

### 1. Simple Confirmation Dialog

```vue
<BaseModal
  v-model="showDelete"
  title="Delete Item?"
  subtitle="This action cannot be undone"
  size="sm"
  cancelText="No, Keep It"
  confirmText="Yes, Delete"
  confirmIcon="fas fa-trash"
  @confirm="deleteItem"
>
  <p class="text-md-on-surface-variant">
    Are you sure you want to delete "{{ item.name }}"?
  </p>
</BaseModal>
```

### 2. Form with Loading State

```vue
<template>
  <BaseModal
    v-model="showCreate"
    title="Create Extra"
    subtitle="Add a new extra option"
    confirmText="Create"
    confirmIcon="fas fa-save"
    :confirmLoading="saving"
    :confirmDisabled="!isValid"
    @confirm="handleCreate"
  >
    <div class="space-y-6">
      <div>
        <label class="block text-sm font-bold text-md-on-surface mb-2">
          <i class="fas fa-tag text-md-primary mr-2"></i>
          Name <span class="text-md-error">*</span>
        </label>
        <input
          v-model="form.name"
          type="text"
          class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
        />
      </div>
      
      <div>
        <label class="block text-sm font-bold text-md-on-surface mb-2">
          <i class="fas fa-dollar-sign text-md-primary mr-2"></i>
          Price <span class="text-md-error">*</span>
        </label>
        <input
          v-model.number="form.price"
          type="number"
          class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
        />
      </div>
    </div>
  </BaseModal>
</template>

<script setup>
import { ref, computed } from 'vue'

const showCreate = ref(false)
const saving = ref(false)
const form = ref({ name: '', price: 0 })

const isValid = computed(() => form.value.name && form.value.price > 0)

async function handleCreate() {
  saving.value = true
  try {
    // API call
    await createExtra(form.value)
    showCreate.value = false
  } finally {
    saving.value = false
  }
}
</script>
```

### 3. Custom Footer

```vue
<BaseModal
  v-model="showEdit"
  title="Edit Bundle"
  :showFooter="false"
>
  <div>Content here</div>
  
  <template #footer>
    <div class="flex justify-between w-full">
      <button
        @click="handleDelete"
        class="px-6 py-3 bg-md-error text-md-on-error rounded-xl"
      >
        <i class="fas fa-trash mr-2"></i>
        Delete Bundle
      </button>
      <div class="flex gap-3">
        <button @click="showEdit = false">Cancel</button>
        <button @click="handleSave" class="btn-gradient">Save</button>
      </div>
    </div>
  </template>
</BaseModal>
```

### 4. Different Sizes

```vue
<!-- Small modal for confirmations -->
<BaseModal size="sm" title="Confirm" v-model="show">
  <p>Are you sure?</p>
</BaseModal>

<!-- Medium modal for forms (default) -->
<BaseModal size="md" title="Create Item" v-model="show">
  <form>...</form>
</BaseModal>

<!-- Large modal for complex forms -->
<BaseModal size="lg" title="Bundle Configuration" v-model="show">
  <div>Complex form...</div>
</BaseModal>

<!-- Extra large modal -->
<BaseModal size="xl" title="Select Items" v-model="show">
  <div>Grid of items...</div>
</BaseModal>

<!-- Full width modal -->
<BaseModal size="full" title="Preview" v-model="show">
  <div>Full preview...</div>
</BaseModal>
```

### 5. No Footer Buttons

```vue
<BaseModal
  v-model="showInfo"
  title="Item Details"
  subtitle="View item information"
  :showFooter="false"
>
  <div class="space-y-4">
    <p><strong>Name:</strong> {{ item.name }}</p>
    <p><strong>Price:</strong> R{{ item.price }}</p>
  </div>
</BaseModal>
```

### 6. Footer Alignment Options

```vue
<!-- Right aligned (default) -->
<BaseModal footerAlign="right" ... />

<!-- Left aligned -->
<BaseModal footerAlign="left" ... />

<!-- Center aligned -->
<BaseModal footerAlign="center" ... />

<!-- Space between -->
<BaseModal 
  footerAlign="between"
  cancelButtonClass="flex-1"
  confirmButtonClass="flex-1"
  ... 
/>
```

## Migration Example

### Before (Old Style)
```vue
<template>
  <div
    v-if="showModal"
    class="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center"
    @click="showModal = false"
  >
    <div class="bg-md-surface rounded-2xl p-6 max-w-2xl w-full" @click.stop>
      <h2 class="text-2xl font-bold mb-4">Create Item</h2>
      
      <form @submit.prevent="save" class="space-y-4">
        <input v-model="form.name" />
        
        <div class="flex gap-3">
          <button @click="showModal = false">Cancel</button>
          <button type="submit" :disabled="saving">
            {{ saving ? 'Saving...' : 'Save' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
```

### After (Using BaseModal)
```vue
<template>
  <BaseModal
    v-model="showModal"
    title="Create Item"
    subtitle="Add a new menu item"
    confirmText="Save"
    :confirmLoading="saving"
    @confirm="save"
  >
    <div class="space-y-4">
      <input v-model="form.name" />
    </div>
  </BaseModal>
</template>
```

Much cleaner! 🎉

## Styling Consistency

All modals using `BaseModal` automatically get:
- ✅ `.modal-overlay` - Blur backdrop
- ✅ `.modal-content` - Mesh gradient background
- ✅ `.modal-header` - Gradient overlay header
- ✅ `.modal-footer` - Mesh gradient footer
- ✅ `.modal-close-btn` - Red circular close button
- ✅ Gradient text titles
- ✅ Consistent spacing (p-6)
- ✅ Proper scrolling (body only)
- ✅ Fade & slide animation
- ✅ Shadow effects

No need to remember classes or structure - it's all built in!
