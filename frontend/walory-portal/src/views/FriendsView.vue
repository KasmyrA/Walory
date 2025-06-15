<template>
  <div class="min-h-screen w-full bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] font-roboto flex flex-col text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
    <!-- Header -->
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] shadow-sm bg-[var(--color-walory-gold-light)]/80 dark:bg-[var(--color-walory-dark-gold-light)]/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">Friends</h1>
      <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <!-- Friends Card -->
    <div class="flex flex-1 items-center justify-center pb-16">
      <div class="bg-[var(--color-walory-silver)]/80 dark:bg-[var(--color-walory-dark-silver)]/80 rounded-2xl shadow-lg border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] px-20 py-14 flex flex-row gap-24 w-[110vw] max-w-7xl min-h-[540px]">
        <!-- Left: Friends list -->
        <div class="flex flex-col flex-1 min-w-[320px] max-w-[420px]">
          <h2 class="text-2xl font-bold mb-4 font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">Your Friends</h2>
          <div v-if="friends.length === 0" class="text-gray-500 dark:text-[var(--color-walory-dark-silver)]">You have no friends yet.</div>
          <ul>
            <li
              v-for="friend in friends"
              :key="friend.id"
              class="flex items-center justify-between border-b border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] py-3 gap-3"
            >
              <img
                :src="friend.avatarUrl || defaultAvatar(friend)"
                class="w-10 h-10 rounded-full object-cover border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] bg-white dark:bg-[var(--color-walory-dark-gold-light)]"
                :alt="friend.username"
              />
              <span class="font-roboto text-lg text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] flex-1 truncate">
                {{ friend.username }} <span class="text-gray-500 dark:text-[var(--color-walory-silver)] text-sm">({{ friend.email }})</span>
              </span>
              <button
                @click="removeFriend(friend.id)"
                class="bg-[var(--color-walory-red)] hover:bg-red-700 text-white font-bold px-4 py-1 rounded transition"
              >
                Remove
              </button>
            </li>
          </ul>
        </div>
        <!-- Right: Add friend and Invites -->
        <div class="flex flex-col flex-1 min-w-[320px] max-w-[420px] items-center justify-center">
          <h2 class="text-2xl font-bold mb-6 font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">Add Friend</h2>
          <form @submit.prevent="sendInvite" class="flex flex-col gap-4 w-full">
            <input
              v-model="inviteEmail"
              type="email"
              placeholder="Friend's email"
              class="border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] rounded px-4 py-2 font-roboto text-lg bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
              required
            />
            <button
              type="submit"
              class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold py-2 rounded transition text-lg dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]"
            >
              Send Invite
            </button>
          </form>
          <div v-if="inviteMsg" class="mt-4 text-[var(--color-walory-green)]">{{ inviteMsg }}</div>
          <div v-if="inviteErr" class="mt-4 text-[var(--color-walory-red)]">{{ inviteErr }}</div>

          <!-- Invites Section -->
          <div v-if="invites.length" class="mt-8 w-full">
            <h3 class="text-xl font-bold mb-2 font-roboto text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">Invites</h3>
            <ul>
              <li v-for="invite in invites" :key="invite.referenceId" class="flex items-center justify-between mb-2">
                <span class="font-roboto text-base">
                  {{ invite.senderName }}
                  <span class="text-gray-500 dark:text-[var(--color-walory-silver)]">({{ invite.message }})</span>
                </span>
                <button
                  @click="acceptInvite(invite.referenceId)"
                  class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold px-4 py-1 rounded transition text-base dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]"
                >
                  Accept
                </button>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

type Friend = { id: string, username: string, email: string, avatarUrl?: string | null }
type Invite = { id: string, referenceId: string, message: string, senderName: string }

const inviteEmail = ref('')
const inviteMsg = ref('')
const inviteErr = ref('')

const friends = ref<Friend[]>([])
const invites = ref<Invite[]>([])

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

function defaultAvatar(friend: { username: string }) {
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(friend.username)}`
}

async function fetchAvatar(userId: string): Promise<string | null> {
  try {
    const res = await fetch(`http://localhost:8080/api/Avatar/${userId}`, { credentials: 'include' })
    if (!res.ok) return null
    const blob = await res.blob()
    return URL.createObjectURL(blob)
  } catch {
    return null
  }
}

async function sendInvite() {
  inviteMsg.value = ''
  inviteErr.value = ''
  try {
    const res = await fetch('http://localhost:8080/friends/send', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(inviteEmail.value)
    })
    if (!res.ok) throw new Error('Failed to send invite')
    inviteMsg.value = 'Invite sent!'
    inviteEmail.value = ''
  } catch (e: any) {
    inviteErr.value = e.message || 'Error sending invite'
  }
}

async function fetchFriends() {
  try {
    const res = await fetch('http://localhost:8080/friends/list', {
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
  } catch {
    friends.value = []
  }
}

async function fetchInvites() {
  try {
    const res = await fetch('http://localhost:8080/notifcation', { credentials: 'include' })
    if (!res.ok) throw new Error('Could not fetch notifications')
    const data = await res.json()
    const notifArr = Array.isArray(data.value) ? data.value : []
      invites.value = notifArr
        .filter((n: any) => typeof n.message === 'string' && n.message.includes('wysłał Ci zaproszenie'))
        .map((n: any) => ({
          id: n.id,
          referenceId: n.referenceId,
          message: n.message,
          senderName: n.message.split(' ')[0]
        }))
  } catch {
    invites.value = []
  }
}

async function acceptInvite(requestId: string) {
  try {
    // Find the invite notification by referenceId
    const invite = invites.value.find(i => i.referenceId === requestId)
    // Accept the invite
    const res = await fetch(`http://localhost:8080/friends/accept/${requestId}`, {
      method: 'POST',
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not accept invite')
    // Delete the notification using its id (not referenceId)
    if (invite && invite.id) {
      await fetch(`http://localhost:8080/notifcation/${invite.id}`, {
        method: 'DELETE',
        credentials: 'include'
      })
    }
    await fetchFriends()
    await fetchInvites()
  } catch (e: any) {
    alert(e.message || 'Error accepting invite')
  }
}

async function removeFriend(friendId: string) {
  if (!confirm('Remove this friend?')) return
  try {
    const res = await fetch(`http://localhost:8080/friends/remove/${friendId}`, {
      method: 'DELETE',
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not remove friend')
    fetchFriends()
  } catch (e: any) {
    alert(e.message || 'Error removing friend')
  }
}

onMounted(() => {
  fetchFriends()
  fetchInvites()
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