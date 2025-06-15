<template>
  <div class="flex flex-col items-center justify-center min-h-screen bg-gradient-to-br from-[var(--color-walory-gold-light)] to-[var(--color-walory-silver)] dark:from-[var(--color-walory-dark-gold-light)] dark:to-[var(--color-walory-dark-silver)] font-roboto">
    <div class="bg-white/90 dark:bg-[var(--color-walory-dark-gold-light)] p-12 rounded-3xl shadow-2xl text-center w-full max-w-xl border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] flex flex-col items-center">
      <h2 class="text-3xl font-extrabold mb-6 font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] tracking-tight">Reset Password</h2>
      <div v-if="!userId || !code" class="text-red-600 font-bold mb-4 font-roboto">
        Invalid or missing reset link.
      </div>
      <form v-else @submit.prevent="submit" class="flex flex-col gap-6 w-full max-w-md">
        <input
          v-model="newPassword"
          type="password"
          placeholder="New Password"
          class="border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] rounded-xl px-5 py-3 font-roboto text-lg text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] bg-white dark:bg-[var(--color-walory-dark-gold-light)] focus:outline-none focus:ring-2 focus:ring-[var(--color-walory-gold-dark)] dark:focus:ring-[var(--color-walory-dark-gold-dark)] transition"
          required
        />
        <input
          v-model="confirmPassword"
          type="password"
          placeholder="Confirm New Password"
          class="border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] rounded-xl px-5 py-3 font-roboto text-lg text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] bg-white dark:bg-[var(--color-walory-dark-gold-light)] focus:outline-none focus:ring-2 focus:ring-[var(--color-walory-gold-dark)] dark:focus:ring-[var(--color-walory-dark-gold-dark)] transition"
          required
        />
        <button
          type="submit"
          class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] font-extrabold py-3 rounded-xl shadow-lg font-roboto text-lg tracking-wide transition"
        >
          Set New Password
        </button>
      </form>
      <div v-if="message" :class="messageClass + ' mt-6 font-roboto text-lg'">{{ message }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const userId = route.query.userId as string || ''
const code = route.query.code as string || ''
const newPassword = ref('')
const confirmPassword = ref('')
const message = ref('')
const messageClass = ref('text-[var(--color-walory-black)]')

const submit = async () => {
  if (!newPassword.value || !confirmPassword.value) {
    message.value = 'Please fill in both fields.'
    messageClass.value = 'text-red-600 font-bold'
    return
  }
  if (newPassword.value !== confirmPassword.value) {
    message.value = 'Passwords do not match.'
    messageClass.value = 'text-red-600 font-bold'
    return
  }
  try {
    const response = await fetch('http://localhost:8080/api/auth/reset-password', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        userId,
        code,
        newPassword: newPassword.value
      })
    })
    if (!response.ok) {
      const msg = await response.text()
      message.value = msg || 'Failed to reset password.'
      messageClass.value = 'text-red-600 font-bold'
      return
    }
    message.value = 'Password reset successful! Redirecting to login...'
    messageClass.value = 'text-green-600 font-bold'
    setTimeout(() => {
      router.push({ name: 'Login' })
    }, 2000)
  } catch {
    message.value = 'Network error.'
    messageClass.value = 'text-red-600 font-bold'
  }
}
</script>