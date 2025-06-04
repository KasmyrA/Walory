<template>
  <div class="min-h-screen w-full bg-walory-gold-light font-roboto flex flex-col text-walory-black">
    <!-- Header -->
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-walory-gold-dark shadow-sm bg-walory-gold-light/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">Friends</h1>
      <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <!-- Friends Card -->
    <div class="flex flex-1 items-center justify-center pb-16">
      <div class="bg-walory-silver/80 rounded-2xl shadow-lg border border-gray-300 px-20 py-14 flex flex-row gap-24 w-[110vw] max-w-7xl min-h-[540px]">
        <!-- Left: Friends list -->
        <div class="flex flex-col flex-1 min-w-[320px] max-w-[420px]">
          <h2 class="text-2xl font-bold mb-4 font-roboto text-walory-black">Your Friends</h2>
          <div v-if="friends.length === 0" class="text-gray-500">You have no friends yet.</div>
          <ul>
            <li
              v-for="friend in friends"
              :key="friend.id"
              class="flex items-center justify-between border-b py-3"
            >
              <span class="font-roboto text-lg text-walory-black">{{ friend.username }} <span class="text-gray-500 text-sm">({{ friend.email }})</span></span>
              <button
                @click="removeFriend(friend.id)"
                class="bg-walory-red hover:bg-red-700 text-white font-bold px-4 py-1 rounded transition"
              >
                Remove
              </button>
            </li>
          </ul>
        </div>
        <!-- Right: Add friend -->
        <div class="flex flex-col flex-1 min-w-[320px] max-w-[420px] items-center justify-center">
          <h2 class="text-2xl font-bold mb-6 font-roboto text-walory-black">Add Friend</h2>
          <form @submit.prevent="sendInvite" class="flex flex-col gap-4 w-full">
            <input
              v-model="inviteEmail"
              type="email"
              placeholder="Friend's email"
              class="border border-gray-300 rounded px-4 py-2 font-roboto text-lg"
              required
            />
            <button
              type="submit"
              class="bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold py-2 rounded transition text-lg"
            >
              Send Invite
            </button>
          </form>
          <div v-if="inviteMsg" class="mt-4 text-walory-green">{{ inviteMsg }}</div>
          <div v-if="inviteErr" class="mt-4 text-walory-red">{{ inviteErr }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const inviteEmail = ref('')
const inviteMsg = ref('')
const inviteErr = ref('')

const friends = ref<{ id: string, username: string, email: string }[]>([])

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
    friends.value = await res.json()
  } catch {
    friends.value = []
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

onMounted(fetchFriends)
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