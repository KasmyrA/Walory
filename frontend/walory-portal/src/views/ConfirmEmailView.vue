<template>
  <div class="flex flex-col items-center justify-center min-h-screen">
    <div class="bg-white/80 p-8 rounded-xl shadow-lg text-center">
      <div v-if="loading">Confirming your email...</div>
      <div v-else-if="success" class="text-green-600 font-bold">Email confirmed! Redirecting to login...</div>
      <div v-else class="text-red-600 font-bold">Confirmation failed: {{ error }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const loading = ref(true)
const success = ref(false)
const error = ref('')

onMounted(async () => {
  const userId = route.query.userId as string
  const code = route.query.code as string

  if (!userId || !code) {
    error.value = 'Invalid confirmation link.'
    loading.value = false
    return
  }

  try {
    const response = await fetch(
      `http://localhost:8080/api/auth/confirm-email?userId=${encodeURIComponent(userId)}&code=${encodeURIComponent(code)}`,
      { method: 'GET' }
    )
    if (!response.ok) {
      const data = await response.json().catch(() => ({}))
      error.value = data.message || 'Confirmation failed.'
      loading.value = false
      return
    }
    success.value = true
    setTimeout(() => {
      router.push({ name: 'Login' })
    }, 2000)
  } catch (e) {
    error.value = 'Network error.'
  } finally {
    loading.value = false
  }
})
</script>