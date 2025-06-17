<script setup lang="ts">
    import { onMounted } from 'vue'
    import { useRouter } from 'vue-router'

    const router = useRouter()

    onMounted(async () => {
        // Try to check if the user is logged in (adjust endpoint if needed)
        try {
            const res = await fetch(`${window.API_BASE_URL}/api/account/username`, { credentials: 'include' })
            if (res.ok) {
                router.replace('/dashboard')
            } else {
                router.replace('/login')
            }
        } catch {
            router.replace('/login')
        }
    })
</script>

<template>
    <div></div>
</template>