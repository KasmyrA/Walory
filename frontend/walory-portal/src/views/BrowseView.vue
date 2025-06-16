<template>
  <div class="min-h-screen w-full bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] font-roboto flex flex-col text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
    <!-- Header -->
    <div class="flex flex-col md:flex-row justify-between items-center px-4 md:px-20 pt-6 md:pt-12 pb-4 md:pb-6 border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] shadow-sm bg-[var(--color-walory-gold-light)]/80 dark:bg-[var(--color-walory-dark-gold-light)]/80">
      <h1 class="text-2xl md:text-3xl font-bold font-roboto tracking-tight mb-2 md:mb-0">
        Browse {{ viewMode === 'collections' ? 'Collections' : 'Templates' }}
      </h1>
      <span class="text-base md:text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <!-- Switch View Buttons -->
    <div class="flex flex-col sm:flex-row justify-center gap-4 sm:gap-8 mt-6 md:mt-10 mb-6 md:mb-8">
      <button
        @click="viewMode = 'collections'"
        :class="[
          'px-6 md:px-8 py-2 rounded-full font-bold text-base md:text-lg transition cursor-pointer border-2',
          viewMode === 'collections'
              ? 'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] border-[var(--color-walory-gold-dark)] shadow dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)] dark:border-[var(--color-walory-dark-gold-dark)]'
              : 'bg-[var(--color-walory-silver)] text-gray-600 border-transparent hover:bg-[var(--color-walory-gold-light)] hover:text-[var(--color-walory-black)] hover:border-[var(--color-walory-gold-dark)] dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)] dark:hover:bg-[var(--color-walory-dark-gold-light)] dark:hover:text-[var(--color-walory-silver)] dark:hover:border-[var(--color-walory-dark-gold-dark)]'
          ]"
        style="transition: background 0.2s, color 0.2s, border 0.2s;"
      >
        Collections
      </button>
      <button
        @click="viewMode = 'templates'"
        :class="[
          'px-6 md:px-8 py-2 rounded-full font-bold text-base md:text-lg transition cursor-pointer border-2',
          viewMode === 'templates'
            ? 'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] border-[var(--color-walory-gold-dark)] shadow dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)] dark:border-[var(--color-walory-dark-gold-dark)]'
            : 'bg-[var(--color-walory-silver)] text-gray-600 border-transparent hover:bg-[var(--color-walory-gold-light)] hover:text-[var(--color-walory-black)] hover:border-[var(--color-walory-gold-dark)] dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)] dark:hover:bg-[var(--color-walory-dark-gold-light)] dark:hover:text-[var(--color-walory-silver)] dark:hover:border-[var(--color-walory-dark-gold-dark)]'
        ]"
        style="transition: background 0.2s, color 0.2s, border 0.2s;"
      >
        Templates
      </button>
    </div>

    <!-- Collections View -->
    <div v-if="viewMode === 'collections'" class="flex flex-1 items-center justify-center pb-10 md:pb-16">
      <div class="bg-[var(--color-walory-silver)]/80 dark:bg-[var(--color-walory-dark-silver)]/80 rounded-2xl shadow-lg border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] px-2 sm:px-6 md:px-16 py-8 md:py-14 w-full max-w-7xl min-h-[400px] md:min-h-[600px]">
        <div class="flex flex-col sm:flex-row justify-center gap-4 sm:gap-8 mb-6 md:mb-8">
          <button
            v-for="f in filters"
            :key="f"
            @click="setFilter(f)"
            :class="[
              'px-6 md:px-8 py-2 rounded-full font-bold text-base md:text-lg transition cursor-pointer border-2',
              filter === f
                ? 'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] border-[var(--color-walory-gold-dark)] shadow dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)] dark:border-[var(--color-walory-dark-gold-dark)]'
                : 'bg-[var(--color-walory-silver)] text-gray-600 border-transparent hover:bg-[var(--color-walory-gold-light)] hover:text-[var(--color-walory-black)] hover:border-[var(--color-walory-gold-dark)] dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)] dark:hover:bg-[var(--color-walory-dark-gold-light)] dark:hover:text-[var(--color-walory-silver)] dark:hover:border-[var(--color-walory-dark-gold-dark)]'
            ]"
            style="transition: background 0.2s, color 0.2s, border 0.2s;"
          >
            {{ f }}
          </button>
        </div>
        <div v-if="loading" class="text-center text-lg md:text-xl text-gray-500 dark:text-[var(--color-walory-dark-silver)]">Loading...</div>
        <div v-else-if="collections.length === 0" class="text-center text-lg md:text-xl text-gray-500 dark:text-[var(--color-walory-silver)]">No collections found.</div>
        <div v-else>
          <div v-if="!selectedCollection" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 md:gap-10">
            <div
              v-for="col in collections"
              :key="col.collectionId"
              class="bg-white dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-4 md:p-6 flex flex-col cursor-pointer hover:shadow-lg transition"
              @click="openCollection(col)"
            >
              <!-- Collection thumbnail -->
              <div class="flex justify-center mb-3">
                <img
                  v-if="col.thumbnailUrl"
                  :src="col.thumbnailUrl"
                  alt="Collection thumbnail"
                  class="w-20 h-20 md:w-24 md:h-24 object-cover rounded-xl border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] cursor-zoom-in"
                  @click.stop="enlargeImage(col.thumbnailUrl)"
                />
                <div v-else class="w-20 h-20 md:w-24 md:h-24 flex items-center justify-center bg-gray-200 dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl text-gray-400">
                  No Image
                </div>
              </div>
              <div class="flex flex-wrap items-center justify-between gap-2 mb-2">
                <h2 class="text-base md:text-xl font-bold font-roboto break-words">{{ col.title }}</h2>
                <span class="text-xs px-2 md:px-3 py-1 rounded-full whitespace-nowrap max-w-full overflow-hidden text-ellipsis"
                  :class="{
                    'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)]': col.visibility === 0,
                    'bg-gray-300 text-gray-700 dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)]': col.visibility === 1,
                    'bg-blue-200 text-blue-900 dark:bg-blue-900 dark:text-blue-200': col.visibility === 2
                  }"
                >
                  {{ visibilityLabel(col.visibility) }}
                </span>
              </div>
              <div class="text-gray-700 dark:text-[var(--color-walory-dark-silver)] mb-2 break-words">{{ col.description }}</div>
              <div class="text-sm text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-2 break-words">Category: {{ col.category }}</div>
              <div class="text-sm text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-2 break-words">By: {{ col.author.name }} ({{ col.author.email }})</div>
            </div>
          </div>
          <!-- Collection Details Modal/Card -->
          <div v-else class="relative">
            <button @click="selectedCollection = null" class="absolute top-0 right-0 mt-2 mr-2 bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] px-4 py-1 rounded-full font-bold shadow dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)]">Back</button>
            <div class="bg-white dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-8 max-w-2xl mx-auto">
              <!-- Collection thumbnail -->
              <div class="flex justify-center mb-4">
                <img
                  v-if="selectedCollection.thumbnailUrl"
                  :src="selectedCollection.thumbnailUrl"
                  alt="Collection thumbnail"
                  class="w-32 h-32 object-cover rounded-xl border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] cursor-zoom-in"
                  @click="enlargeImage(selectedCollection.thumbnailUrl)"
                />
                <div v-else class="w-32 h-32 flex items-center justify-center bg-gray-200 dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl text-gray-400">
                  No Image
                </div>
              </div>
              <div class="flex items-center justify-between mb-2">
                <h2 class="text-2xl font-bold font-roboto">{{ selectedCollection.title }}</h2>
                <span class="text-xs px-3 py-1 rounded-full"
                  :class="{
                    'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)]': selectedCollection.visibility === 0,
                    'bg-gray-300 text-gray-700 dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)]': selectedCollection.visibility === 1,
                    'bg-blue-200 text-blue-900 dark:bg-blue-900 dark:text-blue-200': selectedCollection.visibility === 2
                  }"
                >
                  {{ visibilityLabel(selectedCollection.visibility) }}
                </span>
              </div>
              <div class="text-gray-700 dark:text-[var(--color-walory-dark-silver)] mb-2">{{ selectedCollection.description }}</div>
              <div class="text-sm text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-2">Category: {{ selectedCollection.category }}</div>
              <div class="text-sm text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-2">By: {{ selectedCollection.author.name }} ({{ selectedCollection.author.email }})</div>
              <!-- Likes and Comments -->
              <div class="flex items-center gap-6 mt-4 mb-4">
                <button
                  @click="toggleLike(selectedCollection.collectionId)"
                  class="flex items-center gap-2 px-4 py-1 rounded-full font-bold transition border-2 cursor-pointer
                    focus:outline-none focus:ring-2 focus:ring-[var(--color-walory-gold-dark)] dark:focus:ring-[var(--color-walory-dark-gold-dark)]"
                  :class="isLiked
                    ? 'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] border-[var(--color-walory-gold-dark)] shadow dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)] dark:border-[var(--color-walory-dark-gold-dark)]'
                    : 'bg-[var(--color-walory-silver)] text-gray-600 border-transparent hover:bg-[var(--color-walory-gold-light)] hover:text-[var(--color-walory-black)] hover:border-[var(--color-walory-gold-dark)] dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)] dark:hover:bg-[var(--color-walory-dark-gold-light)] dark:hover:text-[var(--color-walory-silver)]'
                  "
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
                  <li v-for="item in selectedCollection.walorInstance" :key="item.id" class="mb-2 flex items-center">
                    <!-- Item image -->
                    <span class="inline-block w-12 h-12 mr-3 align-middle">
                      <img
                        v-if="itemImages[item.id]"
                        :src="itemImages[item.id]"
                        alt="Item image"
                        class="w-12 h-12 object-cover rounded border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] cursor-zoom-in"
                        @click.stop="enlargeImage(itemImages[item.id])"
                      />
                      <span v-else class="w-12 h-12 flex items-center justify-center bg-gray-200 dark:bg-[var(--color-walory-dark-gold)] rounded text-gray-400">No Image</span>
                    </span>
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
                    class="flex-1 border border-gray-300 dark:border-[var(--color-walory-dark-gold)] rounded px-4 py-2 font-roboto text-lg bg-white dark:bg-[var(--color-walory-dark-gold-light)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
                    required
                  />
                  <button
                    type="submit"
                    class="bg-[var(--color-walory-gold)] hover:bg-[var(--color-walory-gold-dark)] text-[var(--color-walory-black)] font-bold px-6 rounded transition text-lg cursor-pointer border-2 border-transparent hover:border-[var(--color-walory-gold-dark)] focus:outline-none focus:ring-2 focus:ring-[var(--color-walory-gold-dark)] dark:bg-[var(--color-walory-dark-gold)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] dark:text-[var(--color-walory-silver)] dark:hover:border-[var(--color-walory-dark-gold-dark)] dark:focus:ring-[var(--color-walory-dark-gold-dark)]"
                    style="transition: background 0.2s, color 0.2s, border 0.2s;"
                    title="Post comment"
                  >
                    Post
                  </button>
                </form>
                <ul>
                  <li v-for="comment in comments" :key="comment.commentId" class="mb-3 flex items-start gap-2">
                    <div class="flex-1">
                      <div class="font-bold text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                        {{ comment.author.name }}
                        <span class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)]">({{ comment.author.email }})</span>
                      </div>
                      <div class="text-gray-700 dark:text-[var(--color-walory-dark-silver)]">{{ comment.content }}</div>
                      <div class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)]">{{ formatDate(comment.createdAt) }}</div>
                    </div>
                    <button
                      v-if="comment.author.id === currentUserId"
                      @click="deleteComment(comment.commentId)"
                      class="text-[var(--color-walory-red)] font-bold ml-2 rounded-full px-2 py-1 transition cursor-pointer border-2 border-transparent hover:bg-[var(--color-walory-red)] hover:text-white hover:border-[var(--color-walory-red)] focus:outline-none focus:ring-2 focus:ring-[var(--color-walory-red)]"
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
    <div v-else class="flex flex-1 items-center justify-center pb-10 md:pb-16">
      <div class="bg-[var(--color-walory-silver)]/80 dark:bg-[var(--color-walory-dark-silver)]/80 rounded-2xl shadow-lg border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] px-2 sm:px-6 md:px-16 py-8 md:py-14 w-full max-w-7xl min-h-[400px] md:min-h-[600px]">
        <div class="flex flex-col sm:flex-row justify-center gap-4 sm:gap-8 mb-6 md:mb-8">
          <button
            v-for="cat in templateCategories"
            :key="cat"
            @click="setTemplateCategory(cat)"
            :class="[
              'px-6 md:px-8 py-2 rounded-full font-bold text-base md:text-lg transition cursor-pointer border-2',
              templateCategory === cat
                ? 'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] border-[var(--color-walory-gold-dark)] shadow dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)] dark:border-[var(--color-walory-dark-gold-dark)]'
                : 'bg-[var(--color-walory-silver)] text-gray-600 border-transparent hover:bg-[var(--color-walory-gold-light)] hover:text-[var(--color-walory-black)] hover:border-[var(--color-walory-gold-dark)] dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)] dark:hover:bg-[var(--color-walory-dark-gold-light)] dark:hover:text-[var(--color-walory-silver)] dark:hover:border-[var(--color-walory-dark-gold-dark)]'
            ]"
            style="transition: background 0.2s, color 0.2s, border 0.2s;"
          >
            {{ cat }}
          </button>
        </div>
        <div v-if="templatesLoading" class="text-center text-lg md:text-xl text-gray-500 dark:text-[var(--color-walory-dark-silver)]">Loading templates...</div>
        <div v-else-if="templates.length === 0" class="text-center text-lg md:text-xl text-gray-500 dark:text-[var(--color-walory-dark-silver)]">No templates found.</div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 md:gap-8">
          <div
            v-for="tpl in templates"
            :key="tpl.id"
            class="border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] rounded-xl p-4 md:p-6 bg-white/60 dark:bg-[var(--color-walory-dark-gold-light)] shadow"
          >
            <div class="flex flex-wrap items-center justify-between gap-2 mb-2">
              <h3 class="text-base md:text-lg font-bold break-words">{{ tpl.id.slice(0, 8) }}...</h3>
              <span class="text-xs px-2 md:px-3 py-1 rounded-full whitespace-nowrap max-w-full overflow-hidden text-ellipsis"
                :class="{
                  'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)]': tpl.visibility === 0,
                  'bg-gray-300 text-gray-700 dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)]': tpl.visibility === 1,
                  'bg-blue-200 text-blue-900 dark:bg-blue-900 dark:text-blue-200': tpl.visibility === 2
                }"
              >
                {{ visibilityLabel(tpl.visibility) }}
              </span>
            </div>
            <div class="mb-1 text-gray-700 dark:text-[var(--color-walory-dark-silver)] break-words">Category: <span class="font-semibold">{{ tpl.category }}</span></div>
            <div class="mb-1 text-gray-700 dark:text-[var(--color-walory-dark-silver)] break-words">By: <span class="font-semibold">{{ tpl.author.name }}</span> ({{ tpl.author.email }})</div>
            <div class="mt-2">
              <div class="font-bold mb-1">Fields:</div>
              <ul class="pl-4 list-disc text-sm">
                <li v-for="(prop, key) in tpl.content.properties" :key="key">
                  <span class="font-semibold">{{ key }}</span>
                  <span class="text-gray-500 dark:text-[var(--color-walory-dark-silver)]">({{ prop.type }})</span>
                  <span v-if="tpl.content.required && tpl.content.required.includes(key)" class="text-[var(--color-walory-red)] ml-1">*</span>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- Enlarged Image Modal -->
    <div
      v-if="enlargedImageUrl"
      class="fixed inset-0 z-50 flex items-center justify-center"
      style="background: rgba(0,0,0,0.01); backdrop-filter: blur(2px);"
      @click.self="closeEnlargedImage"
    >
      <button
        @click="closeEnlargedImage"
        class="absolute top-8 right-8 bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] rounded-full px-4 py-2 font-bold shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition text-2xl z-60"
        style="border: none;"
      >✕</button>
      <div class="relative">
        <img :src="enlargedImageUrl" class="max-h-[80vh] max-w-[90vw] rounded-xl shadow-2xl border-4 border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)]" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from 'vue'

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

// For thumbnails and item images
const collectionThumbnails = ref<{ [id: string]: string }>({})
const itemImages = ref<{ [id: string]: string }>({})

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
  fetchCollectionThumbnail(col.collectionId)
  fetchAllItemImages(col)
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
    await nextTick()
    // Fetch thumbnails for all collections
    for (const col of collections.value) {
      fetchCollectionThumbnail(col.collectionId)
    }
  } catch {
    collections.value = []
  } finally {
    loading.value = false
  }
}

async function fetchCollectionThumbnail(collectionId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/${collectionId}/image`, {
      credentials: 'include'
    })
    if (res.ok) {
      const blob = await res.blob()
      const url = URL.createObjectURL(blob)
      collectionThumbnails.value[collectionId] = url
      // Attach to collection object for template
      const col = collections.value.find((c: any) => c.collectionId === collectionId)
      if (col) col.thumbnailUrl = url
      if (selectedCollection.value && selectedCollection.value.collectionId === collectionId) {
        selectedCollection.value.thumbnailUrl = url
      }
    }
  } catch {}
}

async function fetchAllItemImages(col: any) {
  if (!col || !col.walorInstance) return
  for (const item of col.walorInstance) {
    await fetchItemImage(item.id)
  }
}

async function fetchItemImage(itemId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/WalorInstances/${itemId}/image`, {
      credentials: 'include'
    })
    if (res.ok) {
      const blob = await res.blob()
      const url = URL.createObjectURL(blob)
      itemImages.value[itemId] = url
    } else {
      itemImages.value[itemId] = ''
    }
  } catch {
    itemImages.value[itemId] = ''
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
    fetchCollectionThumbnail(col.collectionId)
    fetchAllItemImages(col)
  }
})

const enlargedImageUrl = ref<string | null>(null)

function enlargeImage(url: string) {
  enlargedImageUrl.value = url
}
function closeEnlargedImage() {
  enlargedImageUrl.value = null
}
</script>