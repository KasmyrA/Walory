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
        error.value = ''
        message.value = ''
        try {
            const response = await fetch(`${window.API_BASE_URL}/api/Avatar/me`, {
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
            const response = await fetch(`${window.API_BASE_URL}/api/Avatar/upload`, {
                method: 'POST',
                body: formData,
                credentials: 'include'
            })
            if (!response.ok) throw new Error('Upload failed')
            message.value = 'Avatar uploaded!'
            fetchAvatar()
            selectedFile.value = null
            setTimeout(() => {
                window.location.reload()
            }, 1000)
        } catch (e: any) {
            error.value = e.message || 'Error uploading avatar'
        }
    }

    // Username change
    async function changeUsername() {
        usernameMsg.value = ''
        usernameErr.value = ''
        try {
            const res = await fetch(`${window.API_BASE_URL}/api/account/change-username`, {
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
            const res = await fetch(`${window.API_BASE_URL}/api/account/description`, {
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
            const res = await fetch(`${window.API_BASE_URL}/api/account/description`, {
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
            const res = await fetch(`${window.API_BASE_URL}/api/account/delete`, {
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

    const newEmail = ref('')
    const emailMsg = ref('')
    const emailErr = ref('')

    async function requestEmailChange() {
        emailMsg.value = ''
        emailErr.value = ''
        if (!newEmail.value) {
            emailErr.value = 'Please enter a new email.'
            return
        }
        try {
            console.log('Request email change payload:', JSON.stringify(newEmail.value))
            const res = await fetch(`${window.API_BASE_URL}/api/auth/request-email-change`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(newEmail.value)
            })
            if (!res.ok) throw new Error(await res.text() || 'Could not request email change')
            emailMsg.value = 'If the email is valid, a confirmation link has been sent.'
            newEmail.value = ''
        } catch (e: any) {
            emailErr.value = e.message || 'Error requesting email change'
        }
    }

    onMounted(() => {
        fetchUsername()
        fetchAvatar()
        fetchDescription()
    })
</script>

<template>
    <div class="min-h-screen w-full bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] font-roboto flex flex-col text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
        <!-- Header -->
        <div class="flex flex-col md:flex-row justify-between items-center px-4 md:px-20 pt-6 md:pt-12 pb-4 md:pb-6 border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] shadow-sm bg-[var(--color-walory-gold-light)]/80 dark:bg-[var(--color-walory-dark-gold-light)]/80">
            <h1 class="text-2xl md:text-3xl font-bold font-roboto tracking-tight mb-2 md:mb-0">Settings</h1>
            <span class="text-base md:text-xl font-roboto">
                Today is <span class="font-bold">{{ formattedDate }}</span>
            </span>
        </div>
        <!-- Settings Card -->
        <div class="flex flex-1 items-center justify-center pb-10 md:pb-16">
            <div class="bg-[var(--color-walory-silver)]/80 dark:bg-[var(--color-walory-dark-silver)]/80 rounded-2xl shadow-lg border border-gray-300 dark:border-[var(--color-walory-dark-gold)] px-2 sm:px-6 md:px-12 lg:px-16 py-8 md:py-14 flex flex-col md:flex-row gap-8 md:gap-16 w-full max-w-6xl">
                <!-- Left: Avatar and name -->
                <div class="flex flex-col items-center justify-center flex-1 min-w-0 md:min-w-[260px] md:max-w-[400px]">
                    <div class="text-2xl md:text-4xl font-roboto font-normal mb-6 text-center break-words">{{ username }}</div>
                    <img v-if="avatarUrl"
                         :src="avatarUrl"
                         alt="Your avatar"
                         class="w-32 h-32 md:w-64 md:h-64 rounded-full object-cover mb-8 border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)]" />
                    <div v-else class="w-32 h-32 md:w-64 md:h-64 rounded-full border-2 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] mb-8 flex items-center justify-center bg-gray-100 dark:bg-[var(--color-walory-dark-gold-light)] text-gray-400 text-2xl">
                        No Avatar
                    </div>
                    <!-- Change picture button -->
                    <form @submit.prevent="uploadAvatar" class="w-full flex flex-col items-center">
                        <input id="avatar-upload"
                               type="file"
                               accept="image/*"
                               @change="onFileChange"
                               class="hidden" />
                        <label for="avatar-upload"
                               class="cursor-pointer bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold py-2 md:py-3 px-6 md:px-10 rounded-full shadow text-center text-base md:text-lg mb-3 dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]">
                            Change picture
                        </label>
                        <span v-if="selectedFile" class="text-base text-gray-700 dark:text-[var(--color-walory-silver)] font-roboto mb-3">{{ selectedFile.name }}</span>
                        <button v-if="selectedFile"
                                type="submit"
                                class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold py-2 md:py-3 px-6 md:px-10 rounded-full shadow text-center text-base md:text-lg dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]">
                            Upload
                        </button>
                    </form>
                    <div v-if="message" class="mt-3 text-center text-[var(--color-walory-green)] font-roboto text-base">{{ message }}</div>
                    <div v-if="error" class="mt-3 text-center text-[var(--color-walory-red)] font-roboto text-base">{{ error }}</div>
                </div>
                <!-- Right: Account actions -->
                <div class="flex flex-col items-center justify-center flex-1 min-w-0 md:min-w-[260px] md:max-w-[400px]">
                    <!-- Username change -->
                    <div class="w-full mb-8">
                        <label class="block text-base md:text-lg mb-2 font-bold" for="username-input">Change username</label>
                        <div class="flex flex-col sm:flex-row gap-2">
                            <input id="username-input"
                                   v-model="newUsername"
                                   type="text"
                                   class="flex-1 border border-gray-300 dark:border-[var(--color-walory-dark-gold)] rounded px-3 py-2 text-base md:text-lg font-roboto bg-white dark:bg-[var(--color-walory-dark-gold-light)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
                                   placeholder="New username" />
                            <button @click="changeUsername"
                                    class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold px-4 md:px-6 rounded transition text-base md:text-lg dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]">
                                Save
                            </button>
                        </div>
                        <div v-if="usernameMsg" class="mt-2 text-[var(--color-walory-green)] text-sm">{{ usernameMsg }}</div>
                        <div v-if="usernameErr" class="mt-2 text-[var(--color-walory-red] text-sm">{{ usernameErr }}</div>
                    </div>
                    <!-- Change email -->
                    <div class="w-full mb-8">
                        <label class="block text-base md:text-lg mb-2 font-bold" for="email-input">Change email</label>
                        <div class="flex flex-col sm:flex-row gap-2">
                            <input id="email-input"
                                   v-model="newEmail"
                                   type="email"
                                   class="flex-1 border border-gray-300 dark:border-[var(--color-walory-dark-gold)] rounded px-3 py-2 text-base md:text-lg font-roboto bg-white dark:bg-[var(--color-walory-dark-gold-light)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
                                   placeholder="New email" />
                            <button @click="requestEmailChange"
                                    class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold px-4 md:px-6 rounded transition text-base md:text-lg dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]">
                                Request Change
                            </button>
                        </div>
                        <div v-if="emailMsg" class="mt-2 text-[var(--color-walory-green)] text-sm">{{ emailMsg }}</div>
                        <div v-if="emailErr" class="mt-2 text-[var(--color-walory-red)] text-sm">{{ emailErr }}</div>
                    </div>
                    <!-- Description -->
                    <div class="w-full mb-8">
                        <label class="block text-base md:text-lg mb-2 font-bold" for="desc-input">Description</label>
                        <textarea id="desc-input"
                                  v-model="description"
                                  rows="3"
                                  class="w-full border border-gray-300 dark:border-[var(--color-walory-dark-gold)] rounded px-3 py-2 text-base md:text-lg font-roboto resize-none bg-white dark:bg-[var(--color-walory-dark-gold-light)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
                                  placeholder="Add a short description..."></textarea>
                        <div class="flex justify-end mt-2">
                            <button @click="saveDescription"
                                    class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold px-4 md:px-6 rounded transition text-base md:text-lg dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]">
                                Save
                            </button>
                        </div>
                        <div v-if="descMsg" class="mt-2 text-[var(--color-walory-green)] text-sm">{{ descMsg }}</div>
                        <div v-if="descErr" class="mt-2 text-[var(--color-walory-red)] text-sm">{{ descErr }}</div>
                    </div>
                    <!-- Delete account -->
                    <div class="w-full flex flex-col items-center mt-8">
                        <button @click="deleteAccount"
                                class="bg-[var(--color-walory-red)] hover:bg-red-700 text-white font-bold px-6 md:px-8 py-2 rounded-full transition text-base md:text-lg">
                            Delete account
                        </button>
                        <div v-if="deleteMsg" class="mt-2 text-[var(--color-walory-green)] text-sm">{{ deleteMsg }}</div>
                        <div v-if="deleteErr" class="mt-2 text-[var(--color-walory-red)] text-sm">{{ deleteErr }}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap');

    .font-roboto {
        font-family: var(--font-roboto), "Roboto", sans-serif;
    }

    /* Theme color classes (add to tailwind.config.js for full support) */
    .bg-walory-gold-light {
        background-color: var(--color-walory-gold-light);
    }

    .bg-walory-gold {
        background-color: var(--color-walory-gold);
    }

    .bg-walory-gold-dark {
        background-color: var(--color-walory-gold-dark);
    }

    .bg-walory-silver {
        background-color: var(--color-walory-silver);
    }

    .text-walory-gold-dark {
        color: var(--color-walory-gold-dark);
    }

    .text-walory-black {
        color: var(--color-walory-black);
    }

    .text-walory-green {
        color: var(--color-walory-green);
    }

    .text-walory-red {
        color: var(--color-walory-red);
    }
</style>