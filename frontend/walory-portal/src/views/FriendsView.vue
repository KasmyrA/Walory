<!-- filepath: src/views/FriendsView.vue -->
<template>
  <div class="max-w-3xl mx-auto py-12">
    <h1 class="text-3xl font-bold mb-8 font-roboto text-walory-black">Friends</h1>

    <!-- Send invite -->
    <form @submit.prevent="sendInvite" class="flex gap-4 mb-8">
      <input
        v-model="inviteEmail"
        type="email"
        placeholder="Friend's email"
        class="flex-1 border border-gray-300 rounded px-4 py-2 font-roboto"
        required
      />
      <button
        type="submit"
        class="bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold px-6 py-2 rounded transition"
      >
        Send Invite
      </button>
    </form>
    <div v-if="inviteMsg" class="mb-4 text-walory-green">{{ inviteMsg }}</div>
    <div v-if="inviteErr" class="mb-4 text-walory-red">{{ inviteErr }}</div>

    <!-- Friends list -->
    <div>
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const inviteEmail = ref('')
const inviteMsg = ref('')
const inviteErr = ref('')

const friends = ref<{ id: string, username: string, email: string }[]>([])

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