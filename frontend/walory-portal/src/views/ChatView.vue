<script setup lang="ts">
    import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue'

    // Define a Friend type that allows avatarUrl to be string | null | undefined
    type Friend = {
        id: string
        email: string
        fullName?: string
        avatarUrl?: string | null
    }

    const currentUserId = ref<string>('')
    const friends = ref<Friend[]>([])
    const selectedFriend = ref<Friend | null>(null)
    const messages = ref<{ id: string, content: string, isMine: boolean, timestamp: string }[]>([])
    const newMessage = ref('')
    const sending = ref(false)
    const sendError = ref('')
    const messagesEnd = ref<HTMLElement | null>(null)
    const pollingInterval = ref<number | null>(null)

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

    function formatTime(timestamp: string) {
        if (!timestamp) return ''
        const date = new Date(timestamp)
        if (isNaN(date.getTime())) return ''
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    }

    function defaultAvatar(friend: { fullName?: string, email: string }) {
        // fallback avatar
        return `https://ui-avatars.com/api/?name=${encodeURIComponent(friend.fullName || friend.email)}`
    }

    // Fetch avatar for a userId
    async function fetchAvatar(userId: string): Promise<string | null> {
        try {
            const res = await fetch(`${window.API_BASE_URL}/api/Avatar/${userId}`, { credentials: 'include' })
            if (!res.ok) return null
            const blob = await res.blob()
            return URL.createObjectURL(blob)
        } catch {
            return null
        }
    }

    // Fetch current user id from backend
    async function fetchCurrentUserId() {
        try {
            const res = await fetch(`${window.API_BASE_URL}/api/chat/getID`, {
                credentials: 'include'
            })
            if (!res.ok) throw new Error('Could not fetch user id')
            const data = await res.json()
            if (data.isSuccess && data.value) {
                currentUserId.value = data.value
            } else {
                currentUserId.value = ''
            }
        } catch {
            currentUserId.value = ''
        }
    }

    // Fetch friends and their avatars
    async function fetchFriends() {
        try {
            const res = await fetch(`${window.API_BASE_URL}/friends/list`, {
                credentials: 'include'
            })
            if (!res.ok) throw new Error('Could not fetch friends')
            const friendsArr = await res.json()
            // Fetch avatars in parallel
            const withAvatars: Friend[] = await Promise.all(
                friendsArr.map(async (f: any) => {
                    const avatarUrl = await fetchAvatar(f.id)
                    return { ...f, avatarUrl }
                })
            )
            friends.value = withAvatars
            // If selectedFriend is set, update its avatarUrl as well
            if (selectedFriend.value) {
                const updated = withAvatars.find(f => f.id === selectedFriend.value?.id)
                if (updated) selectedFriend.value.avatarUrl = updated.avatarUrl
            }
        } catch {
            friends.value = []
        }
    }

    // Start polling for new messages every 2 seconds when a friend is selected
    async function startPolling(friendId: string) {
        stopPolling()
        pollingInterval.value = window.setInterval(() => {
            fetchMessages(friendId)
        }, 2000)
    }

    // Stop polling when unselecting or unmounting
    function stopPolling() {
        if (pollingInterval.value) {
            clearInterval(pollingInterval.value)
            pollingInterval.value = null
        }
    }

    // Update selectFriend to start polling and fetch avatar
    async function selectFriend(friend: Friend) {
        // Fetch avatar if not already loaded
        if (!friend.avatarUrl) {
            friend.avatarUrl = await fetchAvatar(friend.id)
        }
        selectedFriend.value = { ...friend }
        await fetchMessages(friend.id)
        await markAsRead(friend.id)
        await removeMessageNotifications(friend.id)
        await nextTick()
        scrollToBottom()
        startPolling(friend.id)
    }

    async function fetchMessages(friendId: string) {
        try {
            const res = await fetch(`${window.API_BASE_URL}/api/chat/messages/${friendId}`, {
                credentials: 'include'
            })
            if (!res.ok) throw new Error('Could not fetch messages')
            const data = await res.json()
            messages.value = data.map((msg: any) => ({
                id: msg.id,
                content: msg.content,
                isMine: msg.senderId === currentUserId.value,
                timestamp: msg.sentAt
            }))
        } catch {
            messages.value = []
        }
    }

    async function sendMessage() {
        if (!selectedFriend.value || !newMessage.value.trim()) return
        sending.value = true
        sendError.value = ''
        try {
            const res = await fetch(`${window.API_BASE_URL}/api/chat/send`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify({
                    receiverId: selectedFriend.value.id,
                    content: newMessage.value
                })
            })
            if (!res.ok) throw new Error('Failed to send message')
            newMessage.value = ''
            await fetchMessages(selectedFriend.value.id)
            await nextTick()
            scrollToBottom()
        } catch (e: any) {
            sendError.value = e.message || 'Error sending message'
        } finally {
            sending.value = false
        }
    }

    // New endpoints for mark-as-read and remove-message-notifications
    async function markAsRead(friendId: string) {
        try {
            await fetch(`${window.API_BASE_URL}/api/chat/mark-as-read/${friendId}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify({ friendId })
            })
        } catch { }
    }

    async function removeMessageNotifications(friendId: string) {
        try {
            await fetch(`${window.API_BASE_URL}/api/chat/remove-message-notifications/${friendId}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify({ friendId })
            })
        } catch { }
    }

    function scrollToBottom() {
        if (messagesEnd.value) {
            messagesEnd.value.scrollTop = messagesEnd.value.scrollHeight
        }
    }

    onMounted(async () => {
        await fetchCurrentUserId()
        await fetchFriends()
    })

    watch(selectedFriend, () => {
        nextTick(scrollToBottom)
    })

    onUnmounted(stopPolling)

    watch(selectedFriend, (newVal) => {
        if (!newVal) stopPolling()
    })
</script>

<template>
    <div class="min-h-screen w-full bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] font-roboto flex flex-col text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
        <!-- Header -->
        <div class="flex flex-col md:flex-row justify-between items-center px-4 md:px-20 pt-6 md:pt-12 pb-4 md:pb-6 border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] shadow-sm bg-[var(--color-walory-gold-light)]/80 dark:bg-[var(--color-walory-dark-gold-light)]/80">
            <h1 class="text-2xl md:text-3xl font-bold font-roboto tracking-tight mb-2 md:mb-0">Chat</h1>
            <span class="text-base md:text-xl font-roboto">
                Today is <span class="font-bold">{{ formattedDate }}</span>
            </span>
        </div>
        <!-- Chat Card -->
        <div class="flex flex-1 items-center justify-center pb-10 md:pb-16">
            <div class="bg-[var(--color-walory-silver)]/80 dark:bg-[var(--color-walory-dark-silver)]/80 rounded-2xl shadow-lg border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] px-2 sm:px-6 md:px-16 py-8 md:py-14 w-full max-w-6xl min-h-[400px] md:min-h-[600px] flex flex-col md:flex-row gap-8 md:gap-16">
                <!-- Left: Friends list -->
                <div class="flex flex-col flex-1 min-w-0 max-w-full md:min-w-[260px] md:max-w-[340px] border-b md:border-b-0 md:border-r border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] pb-6 md:pb-0 md:pr-8">
                    <h2 class="text-xl md:text-2xl font-bold mb-4 font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">Friends</h2>
                    <div v-if="friends.length === 0" class="text-gray-500 dark:text-[var(--color-walory-dark-silver)]">No friends to chat with.</div>
                    <ul>
                        <li v-for="friend in friends"
                            :key="friend.id"
                            @click="selectFriend(friend)"
                            :class="[
                'flex items-center justify-between py-3 px-2 rounded cursor-pointer transition gap-2',
                selectedFriend && selectedFriend.id === friend.id
                  ? 'bg-[var(--color-walory-gold)]/60 font-bold dark:bg-[var(--color-walory-dark-gold)]/60'
                  : 'hover:bg-[var(--color-walory-gold)]/30 dark:hover:bg-[var(--color-walory-dark-gold)]/30'
              ]">
                            <img :src="friend.avatarUrl || defaultAvatar(friend)"
                                 class="w-10 h-10 rounded-full object-cover border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] bg-white dark:bg-[var(--color-walory-dark-gold-light)]"
                                 :alt="friend.fullName || friend.email" />
                            <span class="font-roboto text-base md:text-lg text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] truncate">{{ friend.fullName || friend.email }}</span>
                        </li>
                    </ul>
                </div>
                <!-- Right: Chat window -->
                <div class="flex flex-col flex-1 min-w-0 max-w-full md:min-w-[320px] md:max-w-[600px] h-[400px] md:h-[480px]">
                    <div v-if="!selectedFriend" class="flex flex-1 items-center justify-center text-gray-400 dark:text-[var(--color-walory-silver)] text-lg md:text-xl">
                        Select a friend to start chatting.
                    </div>
                    <div v-else class="flex flex-col h-full">
                        <!-- Chat header -->
                        <div class="flex items-center border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] pb-2 mb-2 gap-4">
                            <img :src="selectedFriend.avatarUrl || defaultAvatar(selectedFriend)"
                                 class="w-10 md:w-12 h-10 md:h-12 rounded-full object-cover border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] bg-white dark:bg-[var(--color-walory-dark-gold-light)]"
                                 :alt="selectedFriend.fullName || selectedFriend.email" />
                            <div>
                                <span class="text-base md:text-xl font-bold font-roboto">{{ selectedFriend.fullName || selectedFriend.email }}</span>
                                <span class="ml-2 text-gray-500 dark:text-[var(--color-walory-silver)] text-xs md:text-base">{{ selectedFriend.email }}</span>
                            </div>
                        </div>
                        <!-- Messages -->
                        <div ref="messagesEnd" class="flex-1 overflow-y-auto mb-4 px-2" style="scroll-behavior: smooth;">
                            <div v-for="msg in messages"
                                 :key="msg.id"
                                 :class="[
                  'mb-2 flex',
                  msg.isMine ? 'justify-end' : 'justify-start'
                ]">
                                <div :class="[
                    'px-4 py-2 rounded-lg max-w-[85%] md:max-w-[70%] break-words',
                    msg.isMine
                      ? 'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)]'
                      : 'bg-white text-[var(--color-walory-black)] border border-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold-light)] dark:text-[var(--color-walory-silver)] dark:border-[var(--color-walory-dark-gold)]'
                  ]">
                                    <span>{{ msg.content }}</span>
                                    <div class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)] text-right mt-1">{{ formatTime(msg.timestamp) }}</div>
                                </div>
                            </div>
                        </div>
                        <!-- Send message -->
                        <form @submit.prevent="sendMessage" class="flex gap-2 mt-auto">
                            <input v-model="newMessage"
                                   type="text"
                                   placeholder="Type your message..."
                                   class="flex-1 border border-gray-300 dark:border-[var(--color-walory-dark-gold)] rounded px-4 py-2 font-roboto text-base md:text-lg bg-white dark:bg-[var(--color-walory-dark-gold-light)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
                                   :disabled="sending"
                                   required
                                   autocomplete="off" />
                            <button type="submit"
                                    class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold px-4 md:px-6 rounded transition text-base md:text-lg dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]"
                                    :disabled="sending || !newMessage.trim()">
                                Send
                            </button>
                        </form>
                        <div v-if="sendError" class="mt-2 text-[var(--color-walory-red)] text-sm">{{ sendError }}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>