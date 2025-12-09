<template>
  <Teleport to="body">
    <div class="fixed top-4 right-4 z-[101] space-y-2 max-w-md w-full pointer-events-none">
      <TransitionGroup name="toast">
        <Toast
          v-for="toast in toasts"
          :key="toast.id"
          :message="toast.message"
          :title="toast.title"
          :type="toast.type"
          :duration="0"
          class="pointer-events-auto"
          @close="remove(toast.id)"
        />
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup>
const { toasts, remove } = useToast()
</script>

<style scoped>
.toast-move,
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

.toast-leave-active {
  position: absolute;
  right: 0;
}
</style>
