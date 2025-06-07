<template>
  <div class="min-h-screen w-full bg-walory-gold-light font-roboto flex flex-col text-walory-black">
    <!-- Header -->
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-walory-gold-dark shadow-sm bg-walory-gold-light/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">My Collection</h1>
      <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <div class="flex flex-col gap-12 px-10 py-10 max-w-6xl mx-auto w-full">
      <!-- Templates Management -->
      <div class="bg-white rounded-2xl shadow-lg border border-walory-gold p-8 mb-8">
        <h2 class="text-2xl font-bold mb-4">Templates</h2>
        <form @submit.prevent="editingTemplate ? updateTemplate() : createTemplate()" class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-4">
          <div>
            <label class="block font-bold mb-1">Category</label>
            <input v-model="templateForm.category" class="border rounded px-3 py-2 w-full" required />
          </div>
          <div>
            <label class="block font-bold mb-1">Visibility</label>
            <select v-model="templateForm.visibility" class="border rounded px-3 py-2 w-full">
              <option :value="0">Public</option>
              <option :value="1">Private</option>
              <option :value="2">Friends</option>
            </select>
          </div>
          <div class="md:col-span-2">
            <label class="block font-bold mb-1">Content (JSON)</label>
            <textarea v-model="templateForm.content" class="border rounded px-3 py-2 w-full" required />
          </div>
          <div class="flex gap-2 items-end">
            <button type="submit" class="bg-walory-gold text-walory-black font-bold px-6 py-2 rounded shadow hover:bg-walory-gold-dark transition">
              {{ editingTemplate ? 'Update' : 'Create' }}
            </button>
            <button v-if="editingTemplate" @click="cancelEditTemplate" type="button" class="bg-gray-300 px-4 py-2 rounded shadow">Cancel</button>
          </div>
        </form>
        <div v-if="templates.length === 0" class="text-gray-500">No templates yet.</div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div v-for="tpl in templates" :key="tpl.templateId" class="bg-walory-silver/60 rounded-xl shadow border border-walory-gold p-4 flex flex-col justify-between">
            <div>
              <div class="flex items-center justify-between mb-2">
                <div class="font-bold text-lg">{{ tpl.category }}</div>
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
              <div class="text-xs text-gray-500 mb-1 break-all">ID: {{ tpl.templateId }}</div>
              <div class="text-xs text-gray-500 mb-1">Content: <span class="font-mono">{{ tpl.content }}</span></div>
            </div>
            <div class="flex gap-2 mt-4">
              <button @click="startEditTemplate(tpl)" class="bg-walory-gold px-3 py-1 rounded font-bold shadow hover:bg-walory-gold-dark transition">Edit</button>
              <button @click="deleteTemplate(tpl.templateId)" class="bg-walory-red text-white px-3 py-1 rounded font-bold shadow hover:bg-red-700 transition">Delete</button>
              <button @click="importTemplate(tpl.templateId)" class="bg-blue-200 text-blue-900 px-3 py-1 rounded font-bold shadow hover:bg-blue-300 transition">Import</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Create/Edit Collection -->
      <div class="bg-white rounded-2xl shadow-lg border border-walory-gold p-8 mb-8">
        <h2 class="text-2xl font-bold mb-4">{{ editingCollection ? 'Edit Collection' : 'Create New Collection' }}</h2>
        <form @submit.prevent="editingCollection ? updateCollection() : createCollection()" class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <label class="block font-bold mb-1">Title</label>
            <input v-model="collectionForm.title" class="border rounded px-3 py-2 w-full" required />
          </div>
          <div>
            <label class="block font-bold mb-1">Category</label>
            <input v-model="collectionForm.category" class="border rounded px-3 py-2 w-full" />
          </div>
          <div>
            <label class="block font-bold mb-1">Template</label>
            <select v-model="collectionForm.walorTemplateId" class="border rounded px-3 py-2 w-full">
              <option value="">None</option>
              <option v-for="tpl in templates" :key="tpl.templateId" :value="tpl.templateId">
                {{ tpl.category }} ({{ tpl.templateId.slice(0, 8) }})
              </option>
            </select>
          </div>
          <div>
            <label class="block font-bold mb-1">Visibility</label>
            <select v-model="collectionForm.visibility" class="border rounded px-3 py-2 w-full">
              <option :value="0">Public</option>
              <option :value="1">Private</option>
              <option :value="2">Friends</option>
            </select>
          </div>
          <div class="md:col-span-2">
            <label class="block font-bold mb-1">Description</label>
            <textarea v-model="collectionForm.description" class="border rounded px-3 py-2 w-full" required />
          </div>
          <div class="flex gap-2 items-end">
            <button type="submit" class="bg-walory-gold text-walory-black font-bold px-6 py-2 rounded shadow hover:bg-walory-gold-dark transition">
              {{ editingCollection ? 'Update' : 'Create' }}
            </button>
            <button v-if="editingCollection" @click="cancelEdit" type="button" class="bg-gray-300 px-4 py-2 rounded shadow">Cancel</button>
          </div>
        </form>
      </div>

      <!-- Collections List -->
      <div>
        <h2 class="text-2xl font-bold mb-4">Your Collections</h2>
        <div v-if="collections.length === 0" class="text-gray-500">No collections yet.</div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div v-for="col in collections" :key="col.collectionId" class="bg-walory-silver/60 rounded-2xl shadow border border-walory-gold p-6 flex flex-col justify-between">
            <div>
              <div class="flex items-center justify-between mb-2">
                <div class="font-bold text-xl">{{ col.title }}</div>
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
              <div class="text-gray-700 mb-1">{{ col.description }}</div>
              <div class="text-xs text-gray-500 mb-1">Category: {{ col.category }}</div>
              <div class="text-xs text-gray-500 mb-1">Template: {{ col.walorTemplateId || 'None' }}</div>
              <div class="text-xs text-gray-500 mb-1">Items: {{ col.walorInstance?.length ?? 0 }}</div>
            </div>
            <div class="flex gap-2 mt-4">
              <button @click="startEdit(col)" class="bg-walory-gold px-3 py-1 rounded font-bold shadow hover:bg-walory-gold-dark transition">Edit</button>
              <button @click="deleteCollection(col.collectionId)" class="bg-walory-red text-white px-3 py-1 rounded font-bold shadow hover:bg-red-700 transition">Delete</button>
              <button @click="selectCollection(col)" class="bg-blue-200 text-blue-900 px-3 py-1 rounded font-bold shadow hover:bg-blue-300 transition">Manage Items</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Manage Items (Walor Instances) -->
      <div v-if="selectedCollection" class="bg-white rounded-2xl shadow-lg border border-walory-gold p-8 mb-8">
        <div class="flex justify-between items-center mb-4">
          <h2 class="text-xl font-bold">Manage Items in "{{ selectedCollection.title }}"</h2>
          <button @click="selectedCollection = null" class="text-walory-red font-bold text-lg hover:underline">Close</button>
        </div>
        <form @submit.prevent="addWalorInstance" class="flex gap-2 mb-4">
          <input v-model="walorInstanceForm.data" class="border rounded px-3 py-2 flex-1" placeholder="Item data (JSON or string)" required />
          <button type="submit" class="bg-walory-gold text-walory-black font-bold px-4 py-2 rounded shadow hover:bg-walory-gold-dark transition">Add Item</button>
        </form>
        <div v-if="selectedCollection.walorInstance?.length === 0" class="text-gray-500">No items yet.</div>
        <ul>
          <li v-for="item in selectedCollection.walorInstance" :key="item.id" class="flex items-center justify-between border-b py-2">
            <div>
              <span class="font-mono text-sm">{{ item.data }}</span>
            </div>
            <div class="flex gap-2">
              <button @click="startEditWalor(item)" class="bg-walory-gold px-2 py-1 rounded text-xs font-bold shadow hover:bg-walory-gold-dark transition">Edit</button>
              <button @click="deleteWalorInstance(item.id)" class="bg-walory-red text-white px-2 py-1 rounded text-xs font-bold shadow hover:bg-red-700 transition">Delete</button>
            </div>
          </li>
        </ul>
        <!-- Edit Walor Instance -->
        <div v-if="editingWalorInstance" class="mt-4">
          <form @submit.prevent="updateWalorInstance" class="flex gap-2">
            <input v-model="walorInstanceForm.data" class="border rounded px-3 py-2 flex-1" required />
            <button type="submit" class="bg-walory-gold text-walory-black font-bold px-4 py-2 rounded shadow hover:bg-walory-gold-dark transition">Update</button>
            <button @click="cancelEditWalor" type="button" class="bg-gray-300 px-4 py-2 rounded shadow">Cancel</button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'

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
function formatDate(dateStr: string) {
  const date = new Date(dateStr)
  return date.toLocaleString()
}


// --- State ---
const collections = ref<any[]>([])
const templates = ref<any[]>([])
const selectedCollection = ref<any | null>(null)
const editingCollection = ref(false)
const editingWalorInstance = ref(false)
const editingTemplate = ref(false)
const collectionForm = reactive({
  collectionId: '',
  title: '',
  description: '',
  category: '',
  visibility: 0,
  walorTemplateId: ''
})
const walorInstanceForm = reactive({
  id: '',
  data: ''
})
const templateForm = reactive({
  templateId: '',
  category: '',
  content: '',
  visibility: 0
})

function generateGUID() {
  // RFC4122 version 4 compliant UUID
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    const r = Math.random() * 16 | 0,
      v = c === 'x' ? r : (r & 0x3 | 0x8)
    return v.toString(16)
  })
}

// --- Fetch Collections & Templates ---
async function fetchCollections() {
  const res = await fetch('http://localhost:8080/api/Browse/collections/private', { credentials: 'include' })
  collections.value = await res.json()
}
async function fetchTemplates() {
  // Fetch only private templates (created by the user)
  const res = await fetch('http://localhost:8080/api/Browse/templates/private', { credentials: 'include' })
  const data = await res.json()
  // Normalize to match the rest of the code (id -> templateId)
  templates.value = data.map((tpl: any) => ({
    ...tpl,
    templateId: tpl.id,
    content: typeof tpl.content === 'object' ? JSON.stringify(tpl.content, null, 2) : tpl.content
  }))
}

// --- Template CRUD ---
async function createTemplate() {
  await fetch('http://localhost:8080/WalorTemplates', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({
      category: templateForm.category,
      content: templateForm.content,
      visibility: templateForm.visibility
    })
  })
  resetTemplateForm()
  fetchTemplates()
}
function startEditTemplate(tpl: any) {
  editingTemplate.value = true
  templateForm.templateId = tpl.templateId
  templateForm.category = tpl.category
  templateForm.content = tpl.content
  templateForm.visibility = tpl.visibility
}
async function updateTemplate() {
  await fetch('http://localhost:8080/WalorTemplates', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({
      templateId: templateForm.templateId,
      category: templateForm.category,
      content: templateForm.content,
      visibility: templateForm.visibility
    })
  })
  resetTemplateForm()
  editingTemplate.value = false
  fetchTemplates()
}
async function deleteTemplate(id: string) {
  await fetch(`http://localhost:8080/WalorTemplates/${id}`, {
    method: 'DELETE',
    credentials: 'include'
  })
  fetchTemplates()
}
async function importTemplate(id: string) {
  await fetch(`http://localhost:8080/WalorTemplates/${id}/import`, {
    method: 'POST',
    credentials: 'include'
  })
  fetchTemplates()
}
function cancelEditTemplate() {
  resetTemplateForm()
  editingTemplate.value = false
}
function resetTemplateForm() {
  templateForm.templateId = ''
  templateForm.category = ''
  templateForm.content = ''
  templateForm.visibility = 0
}

// --- Collection CRUD ---
async function createCollection() {
  const newId = generateGUID()
  // Fetch user ID and email from backend endpoints
  let userId = ''
  let userEmail = ''
  try {
    const idRes = await fetch('http://localhost:8080/api/chat/getID', { credentials: 'include' })
    const idJson = await idRes.json()
    userId = idJson.value // <-- Use .value from the JSON response
    console.log('Fetched userId:', userId)
    const emailRes = await fetch('http://localhost:8080/api/account/username', { credentials: 'include' })
    userEmail = await emailRes.text()
  } catch (e) {
    alert('Could not fetch user ID or email. Cannot create collection.')
    return
  }
  // Validate GUID format (simple check)
  const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i
  if (!guidRegex.test(userId)) {
    alert('User ID is not a valid GUID. Cannot create collection.')
    return
  }
  const author = {
    id: userId,
    email: userEmail,
    name: "Unknown"
  }

  // Ensure walorTemplateId is null if not set
  const walorTemplateId = collectionForm.walorTemplateId && collectionForm.walorTemplateId !== '' 
    ? collectionForm.walorTemplateId 
    : null

  // Ensure all required fields are present and not empty
  const payload = {
    command: "",
    collectionDTO: {
      collectionId: newId,
      title: collectionForm.title || 'Untitled',
      description: collectionForm.description || 'No description',
      category: collectionForm.category || 'Uncategorized',
      visibility: collectionForm.visibility,
      walorTemplateId,
      author,
      walorInstance: []
    }
  }

  // Print the payload for backend testing
  console.log('Collection payload:', JSON.stringify(payload, null, 2))

  const res = await fetch('http://localhost:8080/collection', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(payload)
  })

  if (!res.ok) {
    const errText = await res.text()
    console.error('Backend response:', res.status, errText)
    alert('Failed to create collection: ' + errText)
    return
  }

  resetCollectionForm()
  fetchCollections()
}

function startEdit(col: any) {
  editingCollection.value = true
  Object.assign(collectionForm, col)
}
async function updateCollection() {
  await fetch('http://localhost:8080/collection', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({
      collectionId: collectionForm.collectionId,
      title: collectionForm.title,
      description: collectionForm.description,
      visibility: collectionForm.visibility
    })
  })
  resetCollectionForm()
  editingCollection.value = false
  fetchCollections()
}
async function deleteCollection(id: string) {
  await fetch(`http://localhost:8080/collection/${id}`, {
    method: 'DELETE',
    credentials: 'include'
  })
  fetchCollections()
}
function cancelEdit() {
  resetCollectionForm()
  editingCollection.value = false
}
function resetCollectionForm() {
  collectionForm.collectionId = ''
  collectionForm.title = ''
  collectionForm.description = ''
  collectionForm.category = ''
  collectionForm.visibility = 0
  collectionForm.walorTemplateId = ''
}

// --- Walor Instances CRUD ---
function selectCollection(col: any) {
  selectedCollection.value = col
}
async function addWalorInstance() {
  if (!selectedCollection.value) return
  await fetch('http://localhost:8080/api/WalorInstances', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({
      collectionId: selectedCollection.value.collectionId,
      data: walorInstanceForm.data
    })
  })
  walorInstanceForm.data = ''
  fetchCollections()
  selectedCollection.value = collections.value.find(c => c.collectionId === selectedCollection.value.collectionId)
}
function startEditWalor(item: any) {
  editingWalorInstance.value = true
  walorInstanceForm.id = item.id
  walorInstanceForm.data = item.data
}
async function updateWalorInstance() {
  await fetch('http://localhost:8080/api/WalorInstances', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify({
      walorInstanceId: walorInstanceForm.id,
      data: walorInstanceForm.data
    })
  })
  editingWalorInstance.value = false
  walorInstanceForm.id = ''
  walorInstanceForm.data = ''
  fetchCollections()
  if (selectedCollection.value)
    selectedCollection.value = collections.value.find(c => c.collectionId === selectedCollection.value.collectionId)
}
function cancelEditWalor() {
  editingWalorInstance.value = false
  walorInstanceForm.id = ''
  walorInstanceForm.data = ''
}
async function deleteWalorInstance(id: string) {
  await fetch(`http://localhost:8080/api/WalorInstances/${id}`, {
    method: 'DELETE',
    credentials: 'include'
  })
  fetchCollections()
  if (selectedCollection.value)
    selectedCollection.value = collections.value.find(c => c.collectionId === selectedCollection.value.collectionId)
}

// --- Helpers ---
function visibilityLabel(val: number) {
  if (val === 0) return 'Public'
  if (val === 1) return 'Private'
  if (val === 2) return 'Friends'
  return 'Unknown'
}

onMounted(() => {
  fetchCollections()
  fetchTemplates()
})
</script>