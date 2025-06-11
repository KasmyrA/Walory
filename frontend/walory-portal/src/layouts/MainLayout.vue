<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRouter, useRoute, RouterLink } from 'vue-router'

// Persist dark mode preference in localStorage
const darkMode = ref(localStorage.getItem('darkMode') === 'true')

// Watch for changes and toggle the 'dark' class on <html>
watch(darkMode, (val) => {
  document.documentElement.classList.toggle('dark', !!val)
  localStorage.setItem('darkMode', val ? 'true' : 'false')
}, { immediate: true })

const router = useRouter()
const route = useRoute()

// Avatar logic for sidebar
const avatarUrl = ref<string | null>(null)
const defaultAvatar = new URL('../assets/no-profile-picture.png', import.meta.url).href
const sidebarUsername = ref('')

async function fetchSidebarAvatar() {
  try {
    const response = await fetch('http://localhost:8080/api/Avatar/me', {
      credentials: 'include'
    })
    if (!response.ok) throw new Error()
    const blob = await response.blob()
    // If the blob is empty, treat as no avatar
    if (blob.size === 0) {
      avatarUrl.value = defaultAvatar
    } else {
      avatarUrl.value = URL.createObjectURL(blob)
    }
  } catch {
    avatarUrl.value = defaultAvatar
  }
}

async function fetchSidebarUsername() {
  try {
    const res = await fetch('http://localhost:8080/api/account/username', {
      credentials: 'include'
    })
    if (!res.ok) throw new Error()
    sidebarUsername.value = await res.text()
  } catch {
    sidebarUsername.value = 'Unknown'
  }
}

onMounted(() => {
  fetchSidebarAvatar()
  fetchSidebarUsername()
})

async function logout() {
  await fetch('http://localhost:8080/api/auth/logout', {
    method: 'POST',
    credentials: 'include', // Important: send cookies
  })
  localStorage.removeItem('authToken')
  router.push({ name: 'Login' })
}

// Sidebar links data
const links = [
  { to: '/dashboard', icon: 'home', label: 'Dashboard' },
  { to: '/collection', icon: 'map', label: 'My collection' },
  { to: '/browse', icon: 'folder', label: 'Browse' },
  { to: '/friends', icon: 'person', label: 'Friends' },
  { to: '/chat', icon: 'send', label: 'Chat' },
  { to: '/settings', icon: 'menu', label: 'Settings' },
  { to: '/export', icon: 'description', label: 'Export' }
]
</script>

<template>
  <div class="flex h-screen">
    <!-- Sidebar -->
    <aside class="w-64 bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] flex flex-col py-6 px-4 shadow-lg">
      <!-- Logo and Title -->
      <div class="flex flex-col items-center mb-8">
        <RouterLink to="/dashboard">
          <img src="../assets/logo.png" alt="Logo" class="h-16 mb-2 cursor-pointer" />
        </RouterLink>
      </div>

      <!-- Navigation -->
      <nav class="flex flex-col gap-3 flex-1 font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] text-base">
        <RouterLink
          v-for="link in links"
          :key="link.to"
          :to="link.to"
          class="flex items-center gap-3 py-2.5 px-4 rounded-lg font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition text-lg"
          :class="{ 'bg-[var(--color-walory-gold-dark)] dark:bg-[var(--color-walory-dark-gold-dark)] font-bold': route.path === link.to }"
        >
          <span class="material-symbols-outlined text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] text-xl">{{ link.icon }}</span>
          <span class="font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">{{ link.label }}</span>
        </RouterLink>
      </nav>

      <hr class="my-6 border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)]" />

      <!-- User info and logout -->
      <div class="flex flex-col gap-4">
        <div class="flex items-center gap-3 px-2 mb-2">
          <img
            :src="avatarUrl || defaultAvatar"
            alt="User avatar"
            class="h-8 w-8 rounded-full border"
          />
          <span class="font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] text-base truncate">{{ sidebarUsername }}</span>
        </div>
        <button
          @click="logout"
          class="flex items-center justify-center gap-2 w-full py-3 rounded-xl bg-[var(--color-walory-red)] hover:bg-red-600 text-white font-bold font-roboto text-lg transition"
        >
          <span class="material-symbols-outlined text-white">logout</span>
          <span class="font-roboto text-white">Log out</span>
        </button>
      </div>

      <!-- Dark mode toggle -->
      <div class="flex items-center justify-between mt-6 mb-2 px-2">
        <span class="font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">Dark mode</span>
        <label class="inline-flex items-center cursor-pointer relative">
          <input type="checkbox" v-model="darkMode" class="sr-only peer" />
          <div class="w-10 h-6 bg-[var(--color-walory-silver)] dark:bg-[var(--color-walory-dark-silver)] rounded-full peer peer-checked:bg-[var(--color-walory-dark-gold-light)] transition"></div>
          <div
            class="absolute w-4 h-4 bg-white dark:bg-[var(--color-walory-dark-gold)] rounded-full shadow -translate-y-1/2 left-1 top-1/2 transition peer-checked:translate-x-4"
            :class="darkMode ? 'translate-x-4' : ''"
          ></div>
        </label>
      </div>
    </aside>

    <!-- Main content -->
    <main class="flex-1 bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] overflow-auto">
      <router-view />
    </main>
  </div>
</template>