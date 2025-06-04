<template>
  <div class="min-h-screen w-full bg-walory-gold-light font-roboto flex flex-col text-walory-black">
    <!-- Header -->
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-walory-gold-dark shadow-sm bg-walory-gold-light/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">Chat</h1>
      <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <!-- Chat Card -->
    <div class="flex flex-1 items-center justify-center pb-16">
      <div class="bg-walory-silver/80 rounded-2xl shadow-lg border border-gray-300 px-16 py-14 flex flex-row gap-16 w-[90vw] max-w-6xl min-h-[600px]">
        <!-- Left: Friends list -->
        <div class="flex flex-col flex-1 min-w-[260px] max-w-[340px] border-r border-walory-gold-dark pr-8">
          <h2 class="text-2xl font-bold mb-4 font-roboto text-walory-black">Friends</h2>
          <div v-if="friends.length === 0" class="text-gray-500">No friends to chat with.</div>
          <ul>
            <li
              v-for="friend in friends"
              :key="friend.id"
              @click="selectFriend(friend)"
              :class="[
                'flex items-center justify-between py-3 px-2 rounded cursor-pointer transition',
                selectedFriend && selectedFriend.id === friend.id
                  ? 'bg-walory-gold/60 font-bold'
                  : 'hover:bg-walory-gold/30'
              ]"
            >
              <span class="font-roboto text-lg text-walory-black">{{ friend.fullName || friend.email }}</span>
            </li>
          </ul>
        </div>
        <!-- Right: Chat window -->
        <div class="flex flex-col flex-1 min-w-[320px] max-w-[600px] h-[480px]">
          <div v-if="!selectedFriend" class="flex flex-1 items-center justify-center text-gray-400 text-xl">
            Select a friend to start chatting.
          </div>
          <div v-else class="flex flex-col h-full">
            <!-- Chat header -->
            <div class="flex items-center border-b border-walory-gold-dark pb-2 mb-2">
              <span class="text-xl font-bold font-roboto">{{ selectedFriend.fullName || selectedFriend.email }}</span>
              <span class="ml-2 text-gray-500 text-base">{{ selectedFriend.email }}</span>
            </div>
            <!-- Messages -->
            <div ref="messagesEnd" class="flex-1 overflow-y-auto mb-4 px-2" style="scroll-behavior: smooth;">
              <div
                v-for="msg in messages"
                :key="msg.id"
                :class="[
                  'mb-2 flex',
                  msg.isMine ? 'justify-end' : 'justify-start'
                ]"
              >
                <div
                  :class="[
                    'px-4 py-2 rounded-lg max-w-[70%] break-words',
                    msg.isMine
                      ? 'bg-walory-gold text-walory-black'
                      : 'bg-white text-walory-black border border-walory-gold'
                  ]"
                >
                  <span>{{ msg.content }}</span>
                  <div class="text-xs text-gray-500 text-right mt-1">{{ formatTime(msg.timestamp) }}</div>
                </div>
              </div>
            </div>
            <!-- Send message -->
            <form @submit.prevent="sendMessage" class="flex gap-2 mt-auto">
              <input
                v-model="newMessage"
                type="text"
                placeholder="Type your message..."
                class="flex-1 border border-gray-300 rounded px-4 py-2 font-roboto text-lg"
                :disabled="sending"
                required
                autocomplete="off"
              />
              <button
                type="submit"
                class="bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold px-6 rounded transition text-lg"
                :disabled="sending || !newMessage.trim()"
              >
                Send
              </button>
            </form>
            <div v-if="sendError" class="mt-2 text-walory-red text-sm">{{ sendError }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from 'vue'

// You need to know the current user's id. Let's fetch it once on mount.
const currentUserId = ref<string>('')

const friends = ref<{ id: string, email: string, fullName?: string }[]>([])
const selectedFriend = ref<{ id: string, email: string, fullName?: string } | null>(null)
const messages = ref<{ id: string, content: string, isMine: boolean, timestamp: string }[]>([])
const newMessage = ref('')
const sending = ref(false)
const sendError = ref('')
const messagesEnd = ref<HTMLElement | null>(null)

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

function formatTime(timestamp: string) {
  if (!timestamp) return ''
  const date = new Date(timestamp)
  if (isNaN(date.getTime())) return ''
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// Fetch current user id from backend (assuming you have such endpoint)
async function fetchCurrentUserId() {
  try {
    const res = await fetch('http://localhost:8080/api/account/username', {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch user info')
    // If you have a userId endpoint, use it. If not, you need to add one.
    // For now, let's assume you have /api/account/id that returns the id as plain text:
    // currentUserId.value = await res.text()
    // If not, hardcode for testing:
    // currentUserId.value = 'fe63d946-28a7-4c5d-8f7e-96802f26328d'
    // Or, if your /api/account/username returns JSON with id:
    // const data = await res.json(); currentUserId.value = data.id
    // For now, fallback to hardcoded for your sample:
    currentUserId.value = 'fe63d946-28a7-4c5d-8f7e-96802f26328d'
  } catch {
    currentUserId.value = ''
  }
}

async function fetchFriends() {
  try {
    const res = await fetch('http://localhost:8080/friends/list', {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch friends')
    friends.value = await res.json()
  } catch {
    friends.value = []
  }
}

async function selectFriend(friend: { id: string, email: string, fullName?: string }) {
  selectedFriend.value = friend
  await fetchMessages(friend.id)
  await markAsRead(friend.id)
  await removeMessageNotifications(friend.id)
  await nextTick()
  scrollToBottom()
}

async function fetchMessages(friendId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/chat/messages/${friendId}`, {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch messages')
    const data = await res.json()
    // Map the API response to the format expected by the template
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
    const res = await fetch('http://localhost:8080/api/chat/send', {
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

async function markAsRead(friendId: string) {
  try {
    await fetch(`http://localhost:8080/api/chat/mark-as-read/${friendId}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ friendId })
    })
  } catch {}
}

async function removeMessageNotifications(friendId: string) {
  try {
    await fetch(`http://localhost:8080/api/chat/remove-message-notifications/${friendId}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ friendId })
    })
  } catch {}
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
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap');

.font-roboto {
  font-family: var(--font-roboto), "Roboto", sans-serif;
}

/* Theme color classes (add to tailwind.config.js for full support) */
.bg-walory-gold-light { background-color: var(--color-walory-gold-light); }
.bg-walory-gold { background-color: var(--color-walory-gold); }
.bg-walory-gold-dark { background-color: var(--color-walory-gold-dark); }
.bg-walory-silver { background-color: var(--color-walory-silver); }
.text-walory-gold-dark { color: var(--color-walory-gold-dark); }
.text-walory-black { color: var(--color-walory-black); }
.text-walory-green { color: var(--color-walory-green); }
.text-walory-red { color: var(--color-walory-red); }
</style>