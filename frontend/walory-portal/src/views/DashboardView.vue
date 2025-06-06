<template>
  <div class="min-h-screen w-full bg-walory-gold-light font-roboto flex flex-col text-walory-black">
    <!-- Header -->
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-walory-gold-dark shadow-sm bg-walory-gold-light/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">Dashboard</h1>
      <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <div class="flex flex-1 h-[calc(100vh-100px)] gap-12 px-35 py-10">
      <!-- Left Side: User Info + Notifications (40%) -->
      <div class="flex flex-col gap-7 w-[40%] min-w-[340px] max-w-[520px]">
        <!-- User Info -->
        <div class="bg-white rounded-xl shadow border border-walory-gold p-8 flex flex-col items-center">
          <img
            :src="avatarUrl || 'https://ui-avatars.com/api/?name=' + encodeURIComponent(username || 'User')"
            class="w-32 h-32 rounded-full mb-6 border-2 border-walory-gold object-cover"
            alt="User avatar"
          />
          <div class="font-bold text-2xl mb-2">{{ username }}</div>
          <div class="text-gray-600 text-lg">{{ userEmail }}</div>
        </div>
        <!-- Notifications -->
        <div class="bg-white rounded-xl shadow border border-walory-gold p-8 flex-1 flex flex-col min-h-[250px] max-h-[500px] overflow-y-auto">
          <h2 class="text-2xl font-bold mb-6">Notifications</h2>
          <div v-if="notifLoading" class="text-gray-500">Loading...</div>
          <div v-else-if="notifications.length === 0" class="text-gray-500">No notifications.</div>
          <ul>
            <li
              v-for="notif in notifications"
              :key="notif.id"
              class="flex items-center justify-between mb-4 bg-walory-gold-light/60 rounded px-4 py-3"
              :class="{ 'opacity-60': notif.isRead }"
            >
              <div class="flex-1">
                <div class="text-base">{{ notif.message }}</div>
                <div class="text-xs text-gray-500">{{ formatDate(notif.createdAt) }}</div>
              </div>
              <div class="flex flex-col gap-1 ml-3">
                <button
                  v-if="!notif.isRead"
                  @click="markAsRead(notif.id)"
                  class="text-xs px-2 py-1 rounded bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold border border-walory-gold-dark transition"
                  title="Mark as read"
                >✓</button>
                <button
                  @click="deleteNotif(notif.id)"
                  class="text-xs px-2 py-1 rounded bg-walory-red hover:bg-red-700 text-white font-bold border border-walory-red transition"
                  title="Delete"
                >✕</button>
              </div>
            </li>
          </ul>
        </div>
      </div>
      <!-- Right Side: Collections (60%) -->
      <div class="flex-1 overflow-y-auto">
        <h2 class="text-2xl font-bold mb-8">Your Collections</h2>
        <div v-if="collectionsLoading" class="text-gray-500">Loading...</div>
        <div v-else-if="allCollections.length === 0" class="text-gray-500">No collections found.</div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <div
            v-for="col in allCollections"
            :key="col.collectionId"
            class="bg-white rounded-xl shadow border border-walory-gold p-6 flex flex-col hover:shadow-lg transition"
          >
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-bold">{{ col.title }}</h3>
              <span class="text-xs px-3 py-1 rounded-full"
                :class="{
                  'bg-walory-gold text-walory-black': col.visibility === 0,
                  'bg-gray-300 text-gray-700': col.visibility === 1,
                  'bg-blue-200 text-blue-900': col.visibility === 2
                }"
              >
                {{ visibilityLabel(col.visibility) }}
              </span>
            </div>
            <div class="text-gray-700 mb-1">{{ col.description }}</div>
            <div class="text-xs text-gray-500 mb-1">By: {{ col.author.name }}</div>
            <div class="text-xs text-gray-500 mb-1">
              Items: <span class="font-semibold">{{ col.walorInstance?.length ?? 0 }}</span>
            </div>
            <div v-if="col.category" class="text-xs text-gray-400">Category: {{ col.category }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Date formatting
const now = new Date()
const formattedDate = `${now.getDate()}${getDaySuffix(now.getDate())} ${now.toLocaleString('default', { month: 'long' })}, ${now.getFullYear()}`

function getDaySuffix(day: number) {
  if (day > 3 && day < 21) return 'th'
  switch (day % 10) {
    case 1: return 'st'
    case 2: return 'nd'
    case 3: return 'rd'
    default: return 'th'
  }
}
function formatDate(dateStr: string) {
  const date = new Date(dateStr)
  return date.toLocaleString()
}

// User Info (using your provided functions)
const username = ref('Loading...')
const avatarUrl = ref('')
const userEmail = ref('')

async function fetchUsername() {
  try {
    const res = await fetch('http://localhost:8080/api/account/username', {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch username')
    username.value = await res.text()
  } catch (e: any) {
    username.value = 'Unknown'
  }
}

async function fetchAvatar() {
  try {
    const response = await fetch('http://localhost:8080/api/Avatar/me', {
      credentials: 'include'
    })
    if (!response.ok) throw new Error('Could not fetch avatar')
    const blob = await response.blob()
    avatarUrl.value = URL.createObjectURL(blob)
  } catch (e: any) {
    avatarUrl.value = ''
  }
}

// Optionally fetch email if you want to display it
async function fetchEmail() {
  try {
    const res = await fetch('http://localhost:8080/api/account/email', {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch email')
    userEmail.value = await res.text()
  } catch (e: any) {
    userEmail.value = ''
  }
}

// Notifications
const notifications = ref<any[]>([])
const notifLoading = ref(false)

async function fetchNotifications() {
  notifLoading.value = true
  try {
    const res = await fetch('http://localhost:8080/api/notifcation', { credentials: 'include' })
    if (!res.ok) throw new Error('Could not fetch notifications')
    notifications.value = await res.json()
  } catch {
    notifications.value = []
  } finally {
    notifLoading.value = false
  }
}

async function markAsRead(id: string) {
  await fetch(`http://localhost:8080/api/notifcation/mark-as-read/${id}`, {
    method: 'POST',
    credentials: 'include'
  })
  fetchNotifications()
}

async function deleteNotif(id: string) {
  await fetch(`http://localhost:8080/api/notifcation/${id}`, {
    method: 'DELETE',
    credentials: 'include'
  })
  fetchNotifications()
}

// Collections (all types)
const collectionsLoading = ref(false)
const allCollections = ref<any[]>([])

function shuffle(arr: any[]) {
  for (let i = arr.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1))
    ;[arr[i], arr[j]] = [arr[j], arr[i]]
  }
  return arr
}

async function fetchAllCollections() {
  collectionsLoading.value = true
  try {
    // Fetch all types and merge
    const [pub, priv, friends] = await Promise.all([
      fetch('http://localhost:8080/api/Browse/collections/public', { credentials: 'include' }).then(r => r.ok ? r.json() : []),
      fetch('http://localhost:8080/api/Browse/collections/private', { credentials: 'include' }).then(r => r.ok ? r.json() : []),
      fetch('http://localhost:8080/api/Browse/collections/friends', { credentials: 'include' }).then(r => r.ok ? r.json() : [])
    ])
    allCollections.value = shuffle([...pub, ...priv, ...friends])
  } catch {
    allCollections.value = []
  } finally {
    collectionsLoading.value = false
  }
}

function visibilityLabel(val: number) {
  if (val === 0) return 'Public'
  if (val === 1) return 'Private'
  if (val === 2) return 'Friends'
  return 'Unknown'
}

onMounted(() => {
  fetchUsername()
  fetchAvatar()
  fetchEmail()
  fetchNotifications()
  fetchAllCollections()
})
</script>