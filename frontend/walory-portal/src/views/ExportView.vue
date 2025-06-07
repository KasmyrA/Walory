<template>
  <div class="min-h-screen w-full bg-walory-gold-light font-roboto flex flex-col text-walory-black">
    <div class="flex justify-between items-center px-20 pt-12 pb-6 border-b border-walory-gold-dark shadow-sm bg-walory-gold-light/80">
      <h1 class="text-3xl font-bold font-roboto tracking-tight">Export Collections</h1>
        <span class="text-xl font-roboto">
        Today is <span class="font-bold">{{ formattedDate }}</span>
      </span>
    </div>
    <div class="flex flex-col gap-8 px-10 py-10 max-w-3xl mx-auto w-full">
      <div class="bg-white rounded-2xl shadow-lg border border-walory-gold p-8">
        <h2 class="text-2xl font-bold mb-4">Export Options</h2>
        <form @submit.prevent="exportPdf" class="flex flex-col gap-4">
          <div>
            <label class="block font-bold mb-1">Category (optional)</label>
            <input v-model="category" class="border rounded px-3 py-2 w-full" placeholder="Leave empty to export all categories" />
          </div>
          <button type="submit" class="bg-walory-gold text-walory-black font-bold px-6 py-2 rounded shadow hover:bg-walory-gold-dark transition">
            Export PDF
          </button>
        </form>
        <div v-if="exporting" class="mt-4 text-walory-gold-dark">Exporting...</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

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

const category = ref('')
const exporting = ref(false)

async function exportPdf() {
  exporting.value = true
  let url = 'http://localhost:8080/api/pdf/export-pdf'
  if (category.value.trim() !== '') {
    url += `?category=${encodeURIComponent(category.value.trim())}`
  }
  try {
    const res = await fetch(url, {
      method: 'GET',
      credentials: 'include'
    })
    if (!res.ok) {
      const errText = await res.text()
      alert('Failed to export PDF: ' + errText)
      exporting.value = false
      return
    }
    const blob = await res.blob()
    // Try to get filename from Content-Disposition header
    let filename = 'collections.pdf'
    const disposition = res.headers.get('Content-Disposition')
    if (disposition) {
      const match = disposition.match(/filename="?([^"]+)"?/)
      if (match) filename = match[1]
    }
    // Download the file
    const link = document.createElement('a')
    link.href = URL.createObjectURL(blob)
    link.download = filename
    document.body.appendChild(link)
    link.click()
    setTimeout(() => {
      URL.revokeObjectURL(link.href)
      document.body.removeChild(link)
    }, 100)
  } catch (e) {
    alert('Error exporting PDF.')
  }
  exporting.value = false
}
</script>