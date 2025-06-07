<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

onMounted(async () => {
  // Try to check if the user is logged in (adjust endpoint if needed)
  try {
    const res = await fetch('http://localhost:8080/api/account/username', { credentials: 'include' })
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