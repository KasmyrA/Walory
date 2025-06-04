<template>
  <div class="min-h-screen w-full bg-walory-gold-light font-roboto flex flex-col text-walory-black">
    <!-- Header -->
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-walory-gold-dark shadow-sm bg-walory-gold-light/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">Settings</h1>
      <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <!-- Settings Card -->
    <div class="flex flex-1 items-center justify-center pb-16">
      <div class="bg-walory-silver/80 rounded-2xl shadow-lg border border-gray-300 px-16 py-14 flex flex-row gap-16 w-[90vw] max-w-6xl">
        <!-- Left: Avatar and name -->
        <div class="flex flex-col items-center justify-center flex-1 min-w-[300px]">
          <div class="text-4xl font-roboto font-normal mb-6">{{ username }}</div>
          <img
            v-if="avatarUrl"
            :src="avatarUrl"
            alt="Your avatar"
            class="w-64 h-64 rounded-full object-cover mb-8 border-2 border-walory-gold"
          />
          <div v-else class="w-64 h-64 rounded-full border-2 border-walory-gold mb-8 flex items-center justify-center bg-gray-100 text-gray-400 text-2xl">
            No Avatar
          </div>
          <!-- Change picture button -->
          <form @submit.prevent="uploadAvatar" class="w-full flex flex-col items-center">
            <input
              id="avatar-upload"
              type="file"
              accept="image/*"
              @change="onFileChange"
              class="hidden"
            />
            <label
              for="avatar-upload"
              class="cursor-pointer bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold py-3 px-10 rounded-full shadow text-center text-lg mb-3"
            >
              Change picture
            </label>
            <span v-if="selectedFile" class="text-base text-gray-700 font-roboto mb-3">{{ selectedFile.name }}</span>
            <button
              v-if="selectedFile"
              type="submit"
              class="bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold py-3 px-10 rounded-full shadow text-center text-lg"
            >
              Upload
            </button>
          </form>
          <div v-if="message" class="mt-3 text-center text-walory-green font-roboto text-base">{{ message }}</div>
          <div v-if="error" class="mt-3 text-center text-walory-red font-roboto text-base">{{ error }}</div>
        </div>
        <!-- Right: Account actions -->
        <div class="flex flex-col items-center justify-center flex-1 min-w-[320px]">
          <!-- Username change -->
          <div class="w-full mb-8">
            <label class="block text-lg mb-2 font-bold" for="username-input">Change username</label>
            <div class="flex gap-2">
              <input
                id="username-input"
                v-model="newUsername"
                type="text"
                class="flex-1 border border-gray-300 rounded px-3 py-2 text-lg font-roboto"
                placeholder="New username"
              />
              <button
                @click="changeUsername"
                class="bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold px-6 rounded transition text-lg"
              >
                Save
              </button>
            </div>
            <div v-if="usernameMsg" class="mt-2 text-walory-green text-sm">{{ usernameMsg }}</div>
            <div v-if="usernameErr" class="mt-2 text-walory-red text-sm">{{ usernameErr }}</div>
          </div>
          <!-- Description -->
          <div class="w-full mb-8">
            <label class="block text-lg mb-2 font-bold" for="desc-input">Description</label>
            <textarea
              id="desc-input"
              v-model="description"
              rows="3"
              class="w-full border border-gray-300 rounded px-3 py-2 text-lg font-roboto resize-none"
              placeholder="Add a short description..."
            ></textarea>
            <div class="flex justify-end mt-2">
              <button
                @click="saveDescription"
                class="bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold px-6 rounded transition text-lg"
              >
                Save
              </button>
            </div>
            <div v-if="descMsg" class="mt-2 text-walory-green text-sm">{{ descMsg }}</div>
            <div v-if="descErr" class="mt-2 text-walory-red text-sm">{{ descErr }}</div>
          </div>
          <!-- Delete account -->
          <div class="w-full flex flex-col items-center mt-8">
            <button
              @click="deleteAccount"
              class="bg-walory-red hover:bg-red-700 text-white font-bold px-8 py-2 rounded-full transition text-lg"
            >
              Delete account
            </button>
            <div v-if="deleteMsg" class="mt-2 text-walory-green text-sm">{{ deleteMsg }}</div>
            <div v-if="deleteErr" class="mt-2 text-walory-red text-sm">{{ deleteErr }}</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const username = ref('')
const email = ref('collector01@example.com')
const avatarUrl = ref<string | null>(null)
const selectedFile = ref<File | null>(null)
const message = ref('')
const error = ref('')

// Right side state
const newUsername = ref('')
const usernameMsg = ref('')
const usernameErr = ref('')

const description = ref('')
const descMsg = ref('')
const descErr = ref('')

const deleteMsg = ref('')
const deleteErr = ref('')

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

function onFileChange(e: Event) {
  const files = (e.target as HTMLInputElement).files
  selectedFile.value = files && files[0] ? files[0] : null
}

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
  error.value = ''
  message.value = ''
  try {
    const response = await fetch('http://localhost:8080/api/Avatar/me', {
      credentials: 'include'
    })
    if (!response.ok) throw new Error('Could not fetch avatar')
    const blob = await response.blob()
    avatarUrl.value = URL.createObjectURL(blob)
  } catch (e: any) {
    error.value = e.message || 'Error fetching avatar'
  }
}

async function uploadAvatar() {
  error.value = ''
  message.value = ''
  if (!selectedFile.value) return
  const formData = new FormData()
  formData.append('Avatar', selectedFile.value)
  try {
    const response = await fetch('http://localhost:8080/api/Avatar/upload', {
      method: 'POST',
      body: formData,
      credentials: 'include'
    })
    if (!response.ok) throw new Error('Upload failed')
    message.value = 'Avatar uploaded!'
    fetchAvatar()
    selectedFile.value = null
  } catch (e: any) {
    error.value = e.message || 'Error uploading avatar'
  }
}

// Username change
async function changeUsername() {
  usernameMsg.value = ''
  usernameErr.value = ''
  try {
    const res = await fetch('http://localhost:8080/api/account/change-username', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ newName: newUsername.value })
    })
    if (!res.ok) throw new Error('Could not change username')
    usernameMsg.value = 'Username changed!'
    newUsername.value = ''
    await fetchUsername() // Update username after change
  } catch (e: any) {
    usernameErr.value = e.message || 'Error changing username'
  }
}

// Description
async function fetchDescription() {
  descMsg.value = ''
  descErr.value = ''
  try {
    const res = await fetch('http://localhost:8080/api/account/description', {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch description')
    description.value = await res.text()
  } catch (e: any) {
    descErr.value = e.message || 'Error fetching description'
  }
}

async function saveDescription() {
  descMsg.value = ''
  descErr.value = ''
  try {
    const res = await fetch('http://localhost:8080/api/account/description', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ description: description.value })
    })
    if (!res.ok) throw new Error('Could not save description')
    descMsg.value = 'Description saved!'
  } catch (e: any) {
    descErr.value = e.message || 'Error saving description'
  }
}

// Delete account
async function deleteAccount() {
  deleteMsg.value = ''
  deleteErr.value = ''
  if (!confirm('Are you sure you want to delete your account? This action cannot be undone.')) return
  try {
    const res = await fetch('http://localhost:8080/api/account/delete', {
      method: 'DELETE',
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not delete account')
    deleteMsg.value = 'Account deleted. Logging out...'
    setTimeout(() => {
      window.location.href = '/login'
    }, 2000)
  } catch (e: any) {
    deleteErr.value = e.message || 'Error deleting account'
  }
}

onMounted(() => {
  fetchUsername()
  fetchAvatar()
  fetchDescription()
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