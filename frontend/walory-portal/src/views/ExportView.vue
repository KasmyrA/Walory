<template>
    <div class="min-h-screen w-full bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] font-roboto flex flex-col text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
        <!-- Header -->
        <div class="flex flex-col md:flex-row justify-between items-center px-4 md:px-20 pt-6 md:pt-12 pb-4 md:pb-6 border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] shadow-sm bg-[var(--color-walory-gold-light)]/80 dark:bg-[var(--color-walory-dark-gold-light)]/80">
            <h1 class="text-2xl md:text-3xl font-bold font-roboto tracking-tight mb-2 md:mb-0">Export Collections</h1>
            <span class="text-base md:text-xl font-roboto">
                Today is <span class="font-bold">{{ formattedDate }}</span>
            </span>
        </div>
        <!-- Export Options Card -->
        <div class="flex flex-1 items-center justify-center pb-10 md:pb-16">
            <div class="bg-[var(--color-walory-silver)] dark:bg-[var(--color-walory-dark-silver)] rounded-2xl shadow-lg border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] px-4 sm:px-8 md:px-12 py-8 md:py-14 w-full max-w-2xl">
                <h2 class="text-xl md:text-2xl font-bold mb-4">Export Options</h2>
                <form @submit.prevent="exportPdf" class="flex flex-col gap-4">
                    <div>
                        <label class="block font-bold mb-1">Category (optional)</label>
                        <select v-model="category"
                                class="border rounded px-3 py-2 w-full bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                            <option value="">All categories</option>
                            <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                        </select>
                    </div>
                    <button type="submit" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] font-bold px-6 py-2 rounded shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition">
                        Export PDF
                    </button>
                </form>
                <div v-if="exporting" class="mt-4 text-[var(--color-walory-gold-dark)] dark:text-[var(--color-walory-dark-gold-dark)]">Exporting...</div>
                <div v-if="errorMsg" class="mt-4 text-red-600">{{ errorMsg }}</div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { ref, onMounted } from 'vue'

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

    const category = ref('')
    const categories = ref<string[]>([])
    const exporting = ref(false)
    const errorMsg = ref('')

    onMounted(async () => {
        try {
            const res = await fetch(`${window.API_BASE_URL}/api/pdf/categories`, {
                credentials: 'include'
            })
            if (res.ok) {
                categories.value = await res.json()
            }
        } catch {
            // ignore error, just don't show categories
        }
    })

    async function exportPdf() {
        exporting.value = true
        errorMsg.value = ''
        let url = `${window.API_BASE_URL}/api/pdf/export-pdf`
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
                errorMsg.value = 'Failed to export PDF: ' + errText
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
            errorMsg.value = 'Error exporting PDF.'
        }
        exporting.value = false
    }
</script>