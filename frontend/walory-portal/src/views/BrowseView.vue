<template>
  <div class="min-h-screen w-full bg-walory-gold-light font-roboto flex flex-col text-walory-black">
    <!-- Header -->
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-walory-gold-dark shadow-sm bg-walory-gold-light/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">
        Browse {{ viewMode === 'collections' ? 'Collections' : 'Templates' }}
      </h1>
      <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <!-- Switch View Buttons -->
    <div class="flex justify-center gap-8 mt-10 mb-8">
      <button
        @click="viewMode = 'collections'"
        :class="[
          'px-8 py-2 rounded-full font-bold text-lg transition cursor-pointer border-2',
          viewMode === 'collections'
            ? 'bg-walory-gold text-walory-black border-walory-gold-dark shadow'
            : 'bg-walory-silver text-gray-600 border-transparent hover:bg-walory-gold-light hover:text-walory-black hover:border-walory-gold-dark'
        ]"
        style="transition: background 0.2s, color 0.2s, border 0.2s;"
      >
        Collections
      </button>
      <button
        @click="viewMode = 'templates'"
        :class="[
          'px-8 py-2 rounded-full font-bold text-lg transition cursor-pointer border-2',
          viewMode === 'templates'
            ? 'bg-walory-gold text-walory-black border-walory-gold-dark shadow'
            : 'bg-walory-silver text-gray-600 border-transparent hover:bg-walory-gold-light hover:text-walory-black hover:border-walory-gold-dark'
        ]"
        style="transition: background 0.2s, color 0.2s, border 0.2s;"
      >
        Templates
      </button>
    </div>

    <!-- Collections View -->
    <div v-if="viewMode === 'collections'" class="flex flex-1 items-center justify-center pb-16">
      <div class="bg-walory-silver/80 rounded-2xl shadow-lg border border-gray-300 px-16 py-14 w-[90vw] max-w-7xl min-h-[600px]">
        <div class="flex justify-center gap-8 mb-8">
          <button
            v-for="f in filters"
            :key="f"
            @click="setFilter(f)"
            :class="[
              'px-8 py-2 rounded-full font-bold text-lg transition cursor-pointer border-2',
              filter === f
                ? 'bg-walory-gold text-walory-black border-walory-gold-dark shadow'
                : 'bg-walory-silver text-gray-600 border-transparent hover:bg-walory-gold-light hover:text-walory-black hover:border-walory-gold-dark'
            ]"
            style="transition: background 0.2s, color 0.2s, border 0.2s;"
          >
            {{ f }}
          </button>
        </div>
        <div v-if="loading" class="text-center text-xl text-gray-500">Loading...</div>
        <div v-else-if="collections.length === 0" class="text-center text-xl text-gray-500">No collections found.</div>
        <div v-else>
          <div v-if="!selectedCollection" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-10">
            <div
              v-for="col in collections"
              :key="col.collectionId"
              class="bg-white rounded-xl shadow border border-walory-gold p-6 flex flex-col cursor-pointer hover:shadow-lg transition"
              @click="openCollection(col)"
            >
              <div class="flex items-center justify-between mb-2">
                <h2 class="text-xl font-bold font-roboto">{{ col.title }}</h2>
                <span class="text-xs px-3 py-1 rounded-full"
                  :class="{
                    'bg-walory-gold text-walory-black': col.visibility === 0,
                    'bg-gray-300 text-gray-700': col.visibility === 1,
                    'bg-blue-200 text-blue-900': col.visibility === 2
                  }"
                >
                  {{ visibilityLabel(col.visibility) }}
                </span>
              </div>
              <div class="text-gray-700 mb-2">{{ col.description }}</div>
              <div class="text-sm text-gray-500 mb-2">Category: {{ col.category }}</div>
              <div class="text-sm text-gray-500 mb-2">By: {{ col.author.name }} ({{ col.author.email }})</div>
            </div>
          </div>
          <!-- Collection Details Modal/Card -->
          <div v-else class="relative">
            <button @click="selectedCollection = null" class="absolute top-0 right-0 mt-2 mr-2 bg-walory-gold hover:bg-walory-gold-dark text-walory-black px-4 py-1 rounded-full font-bold shadow">Back</button>
            <div class="bg-white rounded-xl shadow border border-walory-gold p-8 max-w-2xl mx-auto">
              <div class="flex items-center justify-between mb-2">
                <h2 class="text-2xl font-bold font-roboto">{{ selectedCollection.title }}</h2>
                <span class="text-xs px-3 py-1 rounded-full"
                  :class="{
                    'bg-walory-gold text-walory-black': selectedCollection.visibility === 0,
                    'bg-gray-300 text-gray-700': selectedCollection.visibility === 1,
                    'bg-blue-200 text-blue-900': selectedCollection.visibility === 2
                  }"
                >
                  {{ visibilityLabel(selectedCollection.visibility) }}
                </span>
              </div>
              <div class="text-gray-700 mb-2">{{ selectedCollection.description }}</div>
              <div class="text-sm text-gray-500 mb-2">Category: {{ selectedCollection.category }}</div>
              <div class="text-sm text-gray-500 mb-2">By: {{ selectedCollection.author.name }} ({{ selectedCollection.author.email }})</div>
              <!-- Likes and Comments -->
              <div class="flex items-center gap-6 mt-4 mb-4">
                <button
                  @click="toggleLike(selectedCollection.collectionId)"
                  class="flex items-center gap-2 px-4 py-1 rounded-full font-bold transition border-2 cursor-pointer
                    focus:outline-none focus:ring-2 focus:ring-walory-gold-dark"
                  :class="isLiked
                    ? 'bg-walory-gold text-walory-black border-walory-gold-dark shadow'
                    : 'bg-walory-silver text-gray-600 border-transparent hover:bg-walory-gold-light hover:text-walory-black hover:border-walory-gold-dark'"
                  style="transition: background 0.2s, color 0.2s, border 0.2s;"
                  title="Like this collection"
                >
                  <span v-if="isLiked">❤️</span>
                  <span v-else>🤍</span>
                  Like ({{ likesCount }})
                </button>
              </div>
              <div class="mt-6">
                <div class="font-bold mb-2">Items:</div>
                <ul class="pl-4 list-disc">
                  <li v-for="item in selectedCollection.walorInstance" :key="item.id" class="mb-2">
                    <span class="inline-block w-12 h-12 bg-gray-200 rounded mr-3 align-middle"></span>
                    <span v-for="(val, key) in item.data" :key="key" class="mr-2 align-middle">
                      <span class="font-semibold">{{ key }}:</span> {{ val }}
                    </span>
                  </li>
                </ul>
              </div>
              <!-- Comments Section -->
              <div class="mt-8">
                <div class="font-bold mb-2">Comments:</div>
                <form @submit.prevent="addComment" class="flex gap-2 mb-4">
                  <input
                    v-model="newComment"
                    type="text"
                    placeholder="Add a comment..."
                    class="flex-1 border border-gray-300 rounded px-4 py-2 font-roboto text-lg"
                    required
                  />
                  <button
                    type="submit"
                    class="bg-walory-gold hover:bg-walory-gold-dark text-walory-black font-bold px-6 rounded transition text-lg cursor-pointer border-2 border-transparent hover:border-walory-gold-dark focus:outline-none focus:ring-2 focus:ring-walory-gold-dark"
                    style="transition: background 0.2s, color 0.2s, border 0.2s;"
                    title="Post comment"
                  >
                    Post
                  </button>
                </form>
                <ul>
                  <li v-for="comment in comments" :key="comment.commentId" class="mb-3 flex items-start gap-2">
                    <div class="flex-1">
                      <div class="font-bold text-walory-black">
                        {{ comment.author.name }}
                        <span class="text-xs text-gray-500">({{ comment.author.email }})</span>
                      </div>
                      <div class="text-gray-700">{{ comment.content }}</div>
                      <div class="text-xs text-gray-500">{{ formatDate(comment.createdAt) }}</div>
                    </div>
                    <button
                      v-if="comment.author.id === currentUserId"
                      @click="deleteComment(comment.commentId)"
                      class="text-walory-red font-bold ml-2 rounded-full px-2 py-1 transition cursor-pointer border-2 border-transparent hover:bg-walory-red hover:text-white hover:border-walory-red focus:outline-none focus:ring-2 focus:ring-walory-red"
                      style="transition: background 0.2s, color 0.2s, border 0.2s;"
                      title="Delete"
                    >✕</button>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Templates View -->
    <div v-else class="flex flex-1 items-center justify-center pb-16">
      <div class="bg-walory-silver/80 rounded-2xl shadow-lg border border-gray-300 px-16 py-14 w-[90vw] max-w-7xl min-h-[600px]">
        <div class="flex justify-center gap-8 mb-8">
          <button
            v-for="cat in templateCategories"
            :key="cat"
            @click="setTemplateCategory(cat)"
            :class="[
              'px-8 py-2 rounded-full font-bold text-lg transition cursor-pointer border-2',
              templateCategory === cat
                ? 'bg-walory-gold text-walory-black border-walory-gold-dark shadow'
                : 'bg-walory-silver text-gray-600 border-transparent hover:bg-walory-gold-light hover:text-walory-black hover:border-walory-gold-dark'
            ]"
            style="transition: background 0.2s, color 0.2s, border 0.2s;"
          >
            {{ cat }}
          </button>
        </div>
        <div v-if="templatesLoading" class="text-center text-xl text-gray-500">Loading templates...</div>
        <div v-else-if="templates.length === 0" class="text-center text-xl text-gray-500">No templates found.</div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <div
            v-for="tpl in templates"
            :key="tpl.id"
            class="border border-walory-gold rounded-xl p-6 bg-white/60 shadow"
          >
            <div class="flex items-center justify-between mb-2">
              <h3 class="text-lg font-bold">{{ tpl.id.slice(0, 8) }}...</h3>
              <span class="text-xs px-3 py-1 rounded-full"
                :class="{
                  'bg-walory-gold text-walory-black': tpl.visibility === 0,
                  'bg-gray-300 text-gray-700': tpl.visibility === 1,
                  'bg-blue-200 text-blue-900': tpl.visibility === 2
                }"
              >
                {{ visibilityLabel(tpl.visibility) }}
              </span>
            </div>
            <div class="mb-1 text-gray-700">Category: <span class="font-semibold">{{ tpl.category }}</span></div>
            <div class="mb-1 text-gray-700">By: <span class="font-semibold">{{ tpl.author.name }}</span> ({{ tpl.author.email }})</div>
            <div class="mt-2">
              <div class="font-bold mb-1">Fields:</div>
              <ul class="pl-4 list-disc text-sm">
                <li v-for="(prop, key) in tpl.content.properties" :key="key">
                  <span class="font-semibold">{{ key }}</span>
                  <span class="text-gray-500">({{ prop.type }})</span>
                  <span v-if="tpl.content.required && tpl.content.required.includes(key)" class="text-walory-red ml-1">*</span>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- End Templates View -->
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'

const filters = ['Public', 'Friends', 'Private']
const filter = ref('Public')
const collections = ref<any[]>([])
const loading = ref(false)
const selectedCollection = ref<any | null>(null)

const comments = ref<any[]>([])
const newComment = ref('')
const likesCount = ref(0)
const isLiked = ref(false)
const currentUserId = ref('')

// View mode: 'collections' or 'templates'
const viewMode = ref<'collections' | 'templates'>('collections')

// Template categories
const templateCategories = ['Accessible', 'Private']
const templateCategory = ref('Accessible')
const templates = ref<any[]>([])
const templatesLoading = ref(false)

function setTemplateCategory(cat: string) {
  templateCategory.value = cat
  fetchTemplates()
}

async function fetchTemplates() {
  templatesLoading.value = true
  let url = ''
  if (templateCategory.value === 'Accessible') url = 'http://localhost:8080/api/Browse/templates/accessible'
  else if (templateCategory.value === 'Private') url = 'http://localhost:8080/api/Browse/templates/private'
  try {
    const res = await fetch(url, { credentials: 'include' })
    if (!res.ok) throw new Error('Could not fetch templates')
    templates.value = await res.json()
  } catch {
    templates.value = []
  } finally {
    templatesLoading.value = false
  }
}

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

function visibilityLabel(val: number) {
  if (val === 0) return 'Public'
  if (val === 1) return 'Private'
  if (val === 2) return 'Friends'
  return 'Unknown'
}

function formatDate(dateStr: string) {
  const date = new Date(dateStr)
  return date.toLocaleString()
}

function setFilter(f: string) {
  filter.value = f
  selectedCollection.value = null
  fetchCollections()
}

function openCollection(col: any) {
  selectedCollection.value = col
  fetchComments(col.collectionId)
  fetchLikes(col.collectionId)
  checkIfLiked(col.collectionId)
}

async function fetchCollections() {
  loading.value = true
  let url = ''
  if (filter.value === 'Public') url = 'http://localhost:8080/api/Browse/collections/public'
  else if (filter.value === 'Friends') url = 'http://localhost:8080/api/Browse/collections/friends'
  else if (filter.value === 'Private') url = 'http://localhost:8080/api/Browse/collections/private'
  try {
    const res = await fetch(url, { credentials: 'include' })
    if (!res.ok) throw new Error('Could not fetch collections')
    collections.value = await res.json()
  } catch {
    collections.value = []
  } finally {
    loading.value = false
  }
}

async function fetchComments(collectionId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/comments`, {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch comments')
    comments.value = await res.json()
  } catch {
    comments.value = []
  }
}

async function addComment() {
  if (!selectedCollection.value || !newComment.value.trim()) return
  try {
    await fetch(`http://localhost:8080/api/collections/${selectedCollection.value.collectionId}/interactions/comments`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({
        collectionId: selectedCollection.value.collectionId,
        content: newComment.value
      })
    })
    newComment.value = ''
    fetchComments(selectedCollection.value.collectionId)
  } catch {}
}

async function deleteComment(commentId: string) {
  if (!selectedCollection.value) return
  try {
    await fetch(`http://localhost:8080/api/collections/${selectedCollection.value.collectionId}/interactions/comments/${commentId}`, {
      method: 'DELETE',
      credentials: 'include'
    })
    fetchComments(selectedCollection.value.collectionId)
  } catch {}
}

async function fetchLikes(collectionId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/likes/count`, {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not fetch likes')
    likesCount.value = await res.json()
  } catch {
    likesCount.value = 0
  }
}

async function checkIfLiked(collectionId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/check-if-liked`, {
      credentials: 'include'
    })
    if (!res.ok) throw new Error('Could not check like')
    const data = await res.json()
    isLiked.value = !!data.value
  } catch {
    isLiked.value = false
  }
}

async function toggleLike(collectionId: string) {
  try {
    if (isLiked.value) {
      await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/likes`, {
        method: 'DELETE',
        credentials: 'include'
      })
    } else {
      await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/likes`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ collectionId })
      })
    }
    await fetchLikes(collectionId)
    await checkIfLiked(collectionId)
  } catch {}
}

// Fetch current user id for comment delete button
async function fetchCurrentUserId() {
  try {
    const res = await fetch('http://localhost:8080/api/chat/getID', {
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

onMounted(() => {
  fetchCollections()
  fetchCurrentUserId()
  fetchTemplates()
})

// Refresh comments/likes if collection changes
watch(selectedCollection, (col) => {
  if (col) {
    fetchComments(col.collectionId)
    fetchLikes(col.collectionId)
    checkIfLiked(col.collectionId)
  }
})
</script>