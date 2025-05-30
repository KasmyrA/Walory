<template>
  <div
    class="min-h-screen h-screen w-full bg-cover bg-center flex items-center justify-center"
    :style="{ backgroundImage: `url(${backgroundImage})` }"
  >
    <div class="bg-walory-gold-light/60 backdrop-blur-lg shadow-2xl rounded-3xl p-10 w-full max-w-xl mx-4 border border-gray-500 flex flex-col items-center">
      <div class="flex justify-center mb-8">
        <img src="../assets/logo.png" alt="Logo" class="h-24" />
      </div>

      <form v-if="!showRegister" @submit.prevent="handleLogin" class="w-full flex flex-col gap-4">
        <input
          v-model="email"
          type="email"
          placeholder="Email"
          class="w-full border rounded-xl px-4 py-3 focus:outline-none focus:ring-2 focus:ring-walory-gold bg-white/80 placeholder-gray-500 font-roboto text-gray-800"
          required
        />

        <div class="relative">
          <input
            v-model="password"
            :type="showPassword ? 'text' : 'password'"
            placeholder="Password"
            class="w-full border rounded-xl px-4 py-3 pr-12 focus:outline-none focus:ring-2 focus:ring-walory-gold bg-white/80 placeholder-gray-500 font-roboto text-gray-800"
            required
          />
          <button
            type="button"
            class="absolute right-3 top-1/2 -translate-y-1/2"
            @click="showPassword = !showPassword"
            tabindex="-1"
          >
            <img
              :src="showPassword ? hidePwdIcon : showPwdIcon"
              alt="Toggle password visibility"
              class="h-6 w-6"
            />
          </button>
        </div>

        <div class="flex justify-end">
          <button type="button" class="text-gray-500 text-sm hover:underline font-roboto" @click="forgotPassword">
            Forgot password?
          </button>
        </div>

        <button
          type="submit"
          class="w-full bg-walory-gold hover:bg-walory-gold-dark text-gray-900 font-bold py-3 rounded-xl transition duration-200 text-lg font-roboto"
        >
          Log in
        </button>

        <div class="flex items-center my-2">
          <div class="flex-grow border-t border-walory-gold-dark"></div>
          <span class="mx-2 text-gray-900 text-sm font-roboto">or</span>
          <div class="flex-grow border-t border-walory-gold-dark"></div>
        </div>

        <button
          type="button"
          class="w-full bg-walory-gold hover:bg-walory-gold-dark text-gray-900 font-bold py-3 rounded-xl transition duration-200 text-lg font-roboto"
          @click="showRegister = true"
        >
          Register
        </button>
      </form>

      <!-- Registration form -->
      <form v-else @submit.prevent="handleRegister" class="w-full flex flex-col gap-4">
        <input
          v-model="registerName"
          type="text"
          placeholder="Name"
          class="w-full border rounded-xl px-4 py-3 focus:outline-none focus:ring-2 focus:ring-walory-gold bg-white/80 placeholder-gray-500 font-roboto text-gray-900"
          required
        />
        <input
          v-model="registerEmail"
          type="email"
          placeholder="Email"
          class="w-full border rounded-xl px-4 py-3 focus:outline-none focus:ring-2 focus:ring-walory-gold bg-white/80 placeholder-gray-500 font-roboto text-gray-900"
          required
        />
        <input
          v-model="registerPassword"
          type="password"
          placeholder="Password"
          class="w-full border rounded-xl px-4 py-3 focus:outline-none focus:ring-2 focus:ring-walory-gold bg-white/80 placeholder-gray-500 font-roboto text-gray-900"
          required
        />
        <input
          v-model="registerConfirmPassword"
          type="password"
          placeholder="Confirm Password"
          class="w-full border rounded-xl px-4 py-3 focus:outline-none focus:ring-2 focus:ring-walory-gold bg-white/80 placeholder-gray-500 font-roboto text-gray-900"
          required
        />
        <button
          type="submit"
          class="w-full bg-walory-gold hover:bg-walory-gold-dark text-gray-900 font-bold py-3 rounded-xl transition duration-200 text-lg font-roboto"
        >
          Register
        </button>
        <button
          type="button"
          class="w-full bg-walory-gold hover:bg-walory-gold-dark text-gray-900 font-bold py-3 rounded-xl transition duration-200 text-lg font-roboto"
          @click="showRegister = false"
        >
          Back to Login
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import backgroundImage from '../assets/logo-bg.png'
import showPwdIcon from '../assets/showpwd.png'
import hidePwdIcon from '../assets/hidepwd.png'

const router = useRouter() // <-- Add this line

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const showRegister = ref(false)

const registerName = ref('')
const registerEmail = ref('')
const registerPassword = ref('')
const registerConfirmPassword = ref('')

const API_BASE = 'http://localhost:8080' // Change to your API base URL

const handleLogin = async () => {
  try {
    const response = await fetch(`${API_BASE}/api/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({
        email: email.value,
        password: password.value
      })
    })

    let responseBody: any
    let isJson = false

    // Try to parse as JSON, fallback to text
    try {
      responseBody = await response.clone().json()
      isJson = true
    } catch {
      responseBody = await response.text()
    }

    if (!response.ok) {
      const errorMsg = isJson
        ? (responseBody.message || 'Login failed')
        : (responseBody || 'Login failed')
      console.error('Login error:', errorMsg)
      alert(errorMsg)
      return
    }

    const successMsg = isJson
      ? (responseBody.message || 'Login successful!')
      : (responseBody || 'Login successful!')

    alert(successMsg)
    router.push({ name: 'Home' }) // Redirect to HomeView
  } catch (err) {
    console.error('Login exception:', err)
    alert('Login error')
  }
}

const handleRegister = async () => {
  if (registerPassword.value !== registerConfirmPassword.value) {
    alert('Passwords do not match')
    return
  }
  try {
    const response = await fetch(`${API_BASE}/api/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        name: registerName.value,
        email: registerEmail.value,
        password: registerPassword.value
      })
    })
    let responseBody: any
    let isJson = false

    // Try to parse as JSON, fallback to text
    try {
      responseBody = await response.clone().json()
      isJson = true
    } catch {
      responseBody = await response.text()
    }

    if (!response.ok) {
      const errorMsg = isJson
        ? (responseBody.message || 'Registration failed')
        : (responseBody || 'Registration failed')
      console.error('Registration error:', errorMsg)
      alert(errorMsg)
      return
    }

    const successMsg = isJson
      ? (responseBody.message || 'Registration successful! Check your email to confirm the registration.')
      : (responseBody || 'Registration successful! Check your email to confirm the registration.')

    alert(successMsg)
    showRegister.value = false
  } catch (err) {
    console.error('Registration exception:', err)
    alert('Registration error')
  }
}

const forgotPassword = () => {
  alert('Forgot password clicked!')
  // TODO: Implement forgot password logic
}
</script>