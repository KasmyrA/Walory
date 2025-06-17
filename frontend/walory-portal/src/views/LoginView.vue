<template>
  <div
    class="min-h-screen h-screen w-full bg-cover bg-center flex items-center justify-center"
    :style="{ backgroundImage: `url(${backgroundImage})` }"
  >
    <div class="bg-walory-gold-light/60 backdrop-blur-lg shadow-2xl rounded-3xl p-10 w-full max-w-xl mx-4 border border-gray-500 flex flex-col items-center">
      <div class="flex justify-center mb-8">
        <img src="../assets/logo.png" alt="Logo" class="h-24" />
      </div>

      <!-- Login Form -->
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

        <!-- Login error message -->
        <div v-if="loginError" class="text-red-600 text-sm font-roboto text-center">{{ loginError }}</div>

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
        <!-- Password strength meter -->
        <div v-if="registerPassword" class="w-full mt-1">
          <div class="h-2 rounded bg-gray-200 overflow-hidden">
            <div
              :class="passwordStrength.color"
              class="h-2 transition-all duration-300"
              :style="{ width: passwordStrength.percent + '%' }"
            ></div>
          </div>
          <div class="text-xs mt-1 font-bold" :class="{
            'text-red-600': passwordStrength.label === 'Weak',
            'text-yellow-600': passwordStrength.label === 'Fair' || passwordStrength.label === 'Good',
            'text-green-700': passwordStrength.label === 'Strong' || passwordStrength.label === 'Very strong'
          }">
            {{ passwordStrength.label }}
          </div>
        </div>
        <input
          v-model="registerConfirmPassword"
          type="password"
          placeholder="Confirm Password"
          class="w-full border rounded-xl px-4 py-3 focus:outline-none focus:ring-2 focus:ring-walory-gold bg-white/80 placeholder-gray-500 font-roboto text-gray-900"
          required
        />

        <!-- Registration error messages -->
        <div v-if="registerErrors.length" class="text-red-600 text-sm font-roboto text-center flex flex-col gap-1">
          <div v-for="err in registerErrors" :key="err" class="break-words">{{ err }}</div>
        </div>
        <div v-if="registerSuccess" class="text-green-700 text-sm font-roboto text-center">{{ registerSuccess }}</div>

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
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import backgroundImage from '../assets/logo-bg.png'
import showPwdIcon from '../assets/showpwd.png'
import hidePwdIcon from '../assets/hidepwd.png'

const router = useRouter()

const email = ref('')
const password = ref('')
const showPassword = ref(false)
const showRegister = ref(false)
const loginError = ref('')

const registerName = ref('')
const registerEmail = ref('')
const registerPassword = ref('')
const registerConfirmPassword = ref('')
const registerErrors = ref<string[]>([])
const registerSuccess = ref('')

const API_BASE = 'http://localhost:8080'

const handleLogin = async () => {
  loginError.value = ''
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
      loginError.value = errorMsg
      return
    }

    router.push({ name: 'Home' })
  } catch (err) {
    loginError.value = 'Login error'
  }
}

const handleRegister = async () => {
  registerErrors.value = []
  registerSuccess.value = ''
  if (registerPassword.value !== registerConfirmPassword.value) {
    registerErrors.value.push('Passwords do not match')
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

    try {
      responseBody = await response.clone().json()
      isJson = true
    } catch {
      responseBody = await response.text()
    }

    if (!response.ok) {
      // If backend returns an array of errors
      if (isJson && Array.isArray(responseBody)) {
        registerErrors.value = responseBody.map((err: any) => err.description || err.message || 'Registration failed')
      } else if (isJson && responseBody.message) {
        registerErrors.value = [responseBody.message]
      } else if (typeof responseBody === 'string') {
        registerErrors.value = [responseBody]
      } else {
        registerErrors.value = ['Registration failed']
      }
      return
    }

    registerSuccess.value = isJson
      ? (responseBody.message || 'Registration successful! Check your email to confirm the registration.')
      : (responseBody || 'Registration successful! Check your email to confirm the registration.')

    // Optionally, clear form fields
    registerName.value = ''
    registerEmail.value = ''
    registerPassword.value = ''
    registerConfirmPassword.value = ''
  } catch (err) {
    registerErrors.value = ['Registration error']
  }
}

const forgotPassword = async () => {
  const userEmail = prompt('Enter your email to reset your password:')
  if (!userEmail) return
  try {
    const response = await fetch('http://localhost:8080/api/auth/forgot-password', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(userEmail)
    })
    if (!response.ok) {
      const msg = await response.text()
      alert(msg || 'Failed to send reset email.')
      return
    }
    alert('If the email exists, a reset link has been sent.')
  } catch (err) {
    alert('Failed to send reset email.')
  }
}

const passwordStrength = computed(() => {
  const pwd = registerPassword.value
  let score = 0
  if (pwd.length >= 8) score++
  if (/[A-Z]/.test(pwd)) score++
  if (/[0-9]/.test(pwd)) score++
  if (/[^A-Za-z0-9]/.test(pwd)) score++
  if (pwd.length >= 12) score++ // bonus for long passwords

  if (score <= 1) return { label: 'Weak', color: 'bg-red-400', percent: 20 }
  if (score === 2) return { label: 'Fair', color: 'bg-yellow-400', percent: 40 }
  if (score === 3) return { label: 'Good', color: 'bg-yellow-500', percent: 60 }
  if (score === 4) return { label: 'Strong', color: 'bg-green-500', percent: 80 }
  return { label: 'Very strong', color: 'bg-green-700', percent: 100 }
})
</script>