<template>
    <div class="flex flex-col items-center justify-center min-h-screen font-roboto bg-gradient-to-br from-[var(--color-walory-gold-light)] to-[var(--color-walory-silver)] dark:from-[var(--color-walory-dark-gold-light)] dark:to-[var(--color-walory-dark-silver)]">
        <div class="bg-white/90 dark:bg-[var(--color-walory-dark-gold-light)] p-12 rounded-3xl shadow-2xl text-center w-full max-w-xl border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] flex flex-col items-center">
            <h2 class="text-3xl font-extrabold mb-6 font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] tracking-tight">Confirm Email Change</h2>
            <div v-if="loading" class="text-lg font-roboto text-[var(--color-walory-black)]">Confirming...</div>
            <div v-else-if="message" :class="messageClass + ' font-roboto text-lg'">{{ message }}</div>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { ref, onMounted } from 'vue'
    import { useRoute, useRouter } from 'vue-router'

    const route = useRoute()
    const router = useRouter()
    const userId = route.query.userId as string || ''
    const token = route.query.token as string || ''
    const loading = ref(true)
    const message = ref('')
    const messageClass = ref('text-[var(--color-walory-black)]')

    onMounted(async () => {
        if (!userId || !token) {
            message.value = 'Invalid confirmation link.'
            messageClass.value = 'text-red-600 font-bold'
            loading.value = false
            return
        }
        try {
            const url = `${window.API_BASE_URL}/api/auth/confirm-email-change?userId=${encodeURIComponent(userId)}&token=${encodeURIComponent(token)}`
            const res = await fetch(url, {
                method: 'GET',
                credentials: 'include'
            })
            if (!res.ok) {
                const msg = await res.text()
                message.value = msg || 'Failed to confirm email change.'
                messageClass.value = 'text-red-600 font-bold'
            } else {
                message.value = 'Email change confirmed! You have been logged out. Please log in with your new email.'
                messageClass.value = 'text-green-600 font-bold'
                // Logout the user
                await fetch(`${window.API_BASE_URL}/api/auth/logout`, {
                    method: 'POST',
                    credentials: 'include'
                })
                setTimeout(() => {
                    router.push({ name: 'Login' })
                }, 2000)
            }
        } catch {
            message.value = 'Network error.'
            messageClass.value = 'text-red-600 font-bold'
        }
        loading.value = false
    })
</script>