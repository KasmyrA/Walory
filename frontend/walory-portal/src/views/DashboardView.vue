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
            const res = await fetch(`${window.API_BASE_URL}/api/account/username`, {
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
            const response = await fetch(`${window.API_BASE_URL}/api/Avatar/me`, {
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
            const res = await fetch(`${window.API_BASE_URL}/api/account/email`, {
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
            const res = await fetch(`${window.API_BASE_URL}/notifcation`, { credentials: 'include' })
            if (!res.ok) throw new Error('Could not fetch notifications')
            const resJson = await res.json()
            notifications.value = Array.isArray(resJson.value) ? resJson.value : []
        } catch {
            notifications.value = []
        } finally {
            notifLoading.value = false
        }
    }

    async function markAsRead(id: string) {
        await fetch(`${window.API_BASE_URL}/notifcation/mark-as-read/${id}`, {
            method: 'POST',
            credentials: 'include'
        })
        fetchNotifications()
    }

    async function deleteNotif(id: string) {
        await fetch(`${window.API_BASE_URL}/notifcation/${id}`, {
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
                fetch(`${window.API_BASE_URL}/api/Browse/collections/public`, { credentials: 'include' }).then(r => r.ok ? r.json() : []),
                fetch(`${window.API_BASE_URL}/api/Browse/collections/private`, { credentials: 'include' }).then(r => r.ok ? r.json() : []),
                fetch(`${window.API_BASE_URL}/api/Browse/collections/friends`, { credentials: 'include' }).then(r => r.ok ? r.json() : [])
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

<template>
    <div class="min-h-screen w-full bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] font-roboto flex flex-col text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
        <!-- Header -->
        <div class="flex justify-between items-center px-4 md:px-20 pt-6 md:pt-12 pb-4 md:pb-6 border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] shadow-sm bg-[var(--color-walory-gold-light)]/80 dark:bg-[var(--color-walory-dark-gold-light)]/80">
            <h1 class="text-2xl md:text-3xl font-roboto font-bold tracking-tight">Dashboard</h1>
            <span class="text-base md:text-xl font-roboto">
                Today is <span class="font-bold">{{ formattedDate }}</span>
            </span>
        </div>
        <div class="flex flex-1 flex-col md:flex-row h-auto md:h-[calc(100vh-100px)] gap-6 md:gap-12 px-2 md:px-8 lg:px-20 py-4 md:py-10">
            <!-- Left Side: User Info + Notifications (40%) -->
            <div class="flex flex-col gap-5 w-full md:w-[40%] min-w-0 md:min-w-[340px] md:max-w-[520px] mx-auto md:mx-0">
                <!-- User Info -->
                <div class="bg-[var(--color-walory-silver)] dark:bg-[var(--color-walory-dark-silver)] rounded-xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-6 md:p-8 flex flex-col items-center">
                    <img :src="avatarUrl || 'https://ui-avatars.com/api/?name=' + encodeURIComponent(username || 'User')"
                         class="w-24 h-24 md:w-32 md:h-32 rounded-full mb-4 md:mb-6 border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] object-cover"
                         alt="User avatar" />
                    <div class="font-roboto font-bold text-xl md:text-2xl mb-1 md:mb-2">{{ username }}</div>
                    <div class="text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] text-base md:text-lg break-all text-center">{{ userEmail }}</div>
                </div>
                <!-- Notifications -->
                <div class="bg-[var(--color-walory-silver)] dark:bg-[var(--color-walory-dark-silver)] rounded-xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-6 md:p-8 flex-1 flex flex-col min-h-[180px] md:min-h-[250px] max-h-[300px] md:max-h-[500px] overflow-y-auto">
                    <h2 class="text-xl md:text-2xl font-roboto font-bold mb-4 md:mb-6">Notifications</h2>
                    <div v-if="notifLoading" class="text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-dark-gold-dark)]">Loading...</div>
                    <div v-else-if="notifications.length === 0" class="text-[var(--color-walory-black)] dark:text-[var(--color-walory-dark-gold-dark)]">No notifications.</div>
                    <ul>
                        <li v-for="notif in notifications"
                            :key="notif.id"
                            class="flex items-center justify-between mb-3 md:mb-4 bg-[var(--color-walory-gold-light)]/60 dark:bg-[var(--color-walory-dark-gold-light)]/60 rounded px-3 md:px-4 py-2 md:py-3"
                            :class="{ 'opacity-60': notif.isRead }">
                            <div class="flex-1">
                                <div class="text-sm md:text-base">{{ notif.message }}</div>
                                <div class="text-xs text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-dark-gold-dark)]">{{ formatDate(notif.createdAt) }}</div>
                            </div>
                            <div class="flex flex-col gap-1 ml-2 md:ml-3">
                                <button v-if="!notif.isRead"
                                        @click="markAsRead(notif.id)"
                                        class="text-xs px-2 py-1 rounded bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] font-bold border border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] transition"
                                        title="Mark as read">
                                    ✓
                                </button>
                                <button @click="deleteNotif(notif.id)"
                                        class="text-xs px-2 py-1 rounded bg-[var(--color-walory-red)] hover:bg-red-700 text-white font-bold border border-[var(--color-walory-red)] transition"
                                        title="Delete">
                                    ✕
                                </button>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>
            <!-- Right Side: Collections (60%) -->
            <div class="flex-1 w-full md:w-[60%] overflow-y-auto bg-[var(--color-walory-silver)] dark:bg-[var(--color-walory-dark-silver)] rounded-xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-6 md:p-8 mt-6 md:mt-0">
                <h2 class="text-xl md:text-2xl font-roboto font-bold mb-6 md:mb-8 text-center md:text-left">Collections you can view</h2>
                <div v-if="collectionsLoading" class="text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-dark-gold-dark)]">Loading...</div>
                <div v-else-if="allCollections.length === 0" class="text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-dark-gold-dark)]">No collections found.</div>
                <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 md:gap-8">
                    <div v-for="col in allCollections"
                         :key="col.collectionId"
                         class="bg-white dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-4 md:p-6 flex flex-col hover:shadow-lg transition">
                        <div class="flex flex-wrap items-center justify-between gap-2 mb-2">
                            <h3 class="text-base md:text-lg font-bold break-words">{{ col.title }}</h3>
                            <span class="text-xs px-2 py-1 rounded-full whitespace-nowrap max-w-full overflow-hidden text-ellipsis"
                                  :class="{
                  'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)]': col.visibility === 0,
                  'bg-gray-300 text-gray-700 dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)]': col.visibility === 1,
                  'bg-blue-200 text-blue-900 dark:bg-blue-900 dark:text-blue-200': col.visibility === 2
                }">
                                {{ visibilityLabel(col.visibility) }}
                            </span>
                        </div>
                        <div class="text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] mb-1 break-words">{{ col.description }}</div>
                        <div class="text-xs text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-silver)] mb-1 break-words">By: {{ col.author.name }}</div>
                        <div class="text-xs text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-silver)] mb-1">
                            Items: <span class="font-semibold">{{ col.walorInstance?.length ?? 0 }}</span>
                        </div>
                        <div v-if="col.category" class="text-xs text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-silver)] break-words">Category: {{ col.category }}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>