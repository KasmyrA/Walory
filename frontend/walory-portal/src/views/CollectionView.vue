<template>
  <div class="min-h-screen w-full bg-[var(--color-walory-gold-light)] dark:bg-[var(--color-walory-dark-gold-light)] font-roboto flex flex-col text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
    <!-- Header -->
    <div class="flex flex-col md:flex-row justify-between items-center px-4 md:px-20 pt-6 md:pt-12 pb-4 md:pb-6 border-b border-[var(--color-walory-gold-dark)] dark:border-[var(--color-walory-dark-gold-dark)] shadow-sm bg-[var(--color-walory-gold-light)]/80 dark:bg-[var(--color-walory-dark-gold-light)]/80 font-roboto">
      <h1 class="text-2xl md:text-3xl font-bold font-roboto tracking-tight mb-2 md:mb-0">My Collection</h1>
      <span class="text-base md:text-xl font-roboto">
        Today is <span class="font-bold font-roboto">{{ formattedDate }}</span>
      </span>
    </div>
    <div class="flex flex-col gap-8 md:gap-12 px-2 md:px-10 py-6 md:py-10 max-w-6xl mx-auto w-full font-roboto">
      <!-- Templates Management -->
      <div class="bg-[var(--color-walory-silver)] dark:bg-[var(--color-walory-dark-silver)] rounded-2xl shadow-lg border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-4 md:p-8 mb-8 font-roboto">
        <h2 class="text-xl md:text-2xl font-bold mb-4 p-2 font-roboto">Templates</h2>
        <button @click="openTemplateModal" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] font-bold font-roboto px-4 md:px-6 py-2 rounded shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition mb-4">
          Create Template
        </button>
        <!-- Modal for template creation -->
        <div
          v-if="showTemplateModal"
          class="fixed inset-0 flex items-center justify-center z-50 pointer-events-auto backdrop-blur-sm font-roboto"
          style="background: transparent;"
        >
          <div
            class="bg-white dark:bg-[var(--color-walory-dark-gold-light)] rounded-2xl p-8 w-full max-w-2xl shadow-lg relative font-roboto"
            style="max-height: 90vh; overflow-y: auto;"
          >
            <h2 class="text-xl font-bold mb-4 font-roboto">Create Template</h2>
            <!-- Category input -->
            <div class="mb-4">
              <label class="block font-bold mb-1 font-roboto">Category</label>
              <input v-model="templateForm.category" placeholder="Template category" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" required />
            </div>
            <!-- Visibility dropdown -->
            <div class="mb-4">
              <label class="block font-bold mb-1 font-roboto">Visibility</label>
              <select v-model="templateForm.visibility" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                <option :value="0">Public</option>
                <option :value="1">Private</option>
                <option :value="2">Friends</option>
              </select>
            </div>
            <form @submit.prevent="addField" class="flex flex-wrap gap-2 mb-4 font-roboto">
              <input v-model="newField.name" placeholder="Field name" class="border rounded px-2 py-1 flex-1 min-w-[120px] font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" required />
              <select v-model="newField.type" class="border rounded px-2 py-1 min-w-[100px] font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                <option value="string">string</option>
                <option value="number">number</option>
                <option value="boolean">boolean</option>
                <option value="date">date</option>
              </select>
              <input v-model="newField.format" placeholder="Format (optional)" class="border rounded px-2 py-1 min-w-[120px] font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" />
              <label class="flex items-center ml-2 font-roboto">
                <input type="checkbox" v-model="newField.required" class="mr-1" /> Required
              </label>
              <button type="submit" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-2 py-1 rounded font-bold font-roboto">Add</button>
            </form>
            <ul>
              <li v-for="(field, idx) in newTemplateFields" :key="field.name" class="flex items-center gap-2 mb-1 font-roboto">
                <span class="font-mono">{{ field.type }}</span>
                <span class="font-bold font-roboto">{{ field.name }}</span>
                <span v-if="field.format" class="italic text-gray-400 font-roboto">({{ field.format }})</span>
                <span v-if="field.required" class="text-[var(--color-walory-red)] font-bold ml-2 font-roboto">required</span>
                <button @click="removeField(idx)" class="ml-2 text-[var(--color-walory-red)] font-roboto">Remove</button>
              </li>
            </ul>
            <div class="flex gap-2 mt-4 font-roboto">
              <button @click="submitTemplateForm" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-4 py-2 rounded font-bold font-roboto">Create Template</button>
              <button @click="closeTemplateModal" class="bg-gray-300 dark:bg-[var(--color-walory-dark-silver)] text-black dark:text-[var(--color-walory-gold-light)] px-4 py-2 rounded font-roboto">Cancel</button>
            </div>
          </div>
        </div>

        <!-- Modal for template editing -->
        <div
          v-if="editingTemplate"
          class="fixed inset-0 flex items-center justify-center z-50 pointer-events-auto backdrop-blur-sm font-roboto"
          style="background: transparent;"
        >
          <div
            class="bg-white dark:bg-[var(--color-walory-dark-gold-light)] rounded-2xl p-8 w-full max-w-2xl shadow-lg relative font-roboto"
            style="max-height: 90vh; overflow-y: auto;"
          >
            <h2 class="text-xl font-bold mb-4 font-roboto">Edit Template</h2>
            <!-- Category input -->
            <div class="mb-4">
              <label class="block font-bold mb-1 font-roboto">Category</label>
              <input v-model="templateForm.category" placeholder="Template category" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" required />
            </div>
            <!-- Visibility dropdown -->
            <div class="mb-4">
              <label class="block font-bold mb-1 font-roboto">Visibility</label>
              <select v-model="templateForm.visibility" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                <option :value="0">Public</option>
                <option :value="1">Private</option>
                <option :value="2">Friends</option>
              </select>
            </div>
            <form @submit.prevent="addEditField" class="flex flex-wrap gap-2 mb-4 font-roboto">
              <input v-model="editField.name" placeholder="Field name" class="border rounded px-2 py-1 flex-1 min-w-[120px] font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" required />
              <select v-model="editField.type" class="border rounded px-2 py-1 min-w-[100px] font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                <option value="string">string</option>
                <option value="number">number</option>
                <option value="boolean">boolean</option>
                <option value="date">date</option>
              </select>
              <input v-model="editField.format" placeholder="Format (optional)" class="border rounded px-2 py-1 min-w-[120px] font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" />
              <label class="flex items-center ml-2 font-roboto">
                <input type="checkbox" v-model="editField.required" class="mr-1" /> Required
              </label>
              <button type="submit" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-2 py-1 rounded font-bold font-roboto">Add</button>
            </form>
            <ul>
              <li v-for="(field, idx) in editTemplateFields" :key="field.name" class="flex items-center gap-2 mb-1 font-roboto">
                <span class="font-mono">{{ field.type }}</span>
                <span class="font-bold font-roboto">{{ field.name }}</span>
                <span v-if="field.format" class="italic text-gray-400 font-roboto">({{ field.format }})</span>
                <span v-if="field.required" class="text-[var(--color-walory-red)] font-bold ml-2 font-roboto">required</span>
                <button @click="removeEditField(idx)" class="ml-2 text-[var(--color-walory-red)] font-roboto">Remove</button>
              </li>
            </ul>
            <div class="flex gap-2 mt-4 font-roboto">
              <button @click="submitEditTemplateForm" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-4 py-2 rounded font-bold font-roboto">Update</button>
              <button @click="closeEditTemplateModal" class="bg-gray-300 dark:bg-[var(--color-walory-dark-silver)] text-black dark:text-[var(--color-walory-gold-light)] px-4 py-2 rounded font-roboto">Cancel</button>
            </div>
          </div>
        </div>
        <div v-if="templates.length === 0" class="text-gray-500 dark:text-[var(--color-walory-dark-silver)] font-roboto">No templates yet.</div>
        <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6 font-roboto">
          <div v-for="tpl in templates" :key="tpl.templateId" class="bg-white/60 dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-4 flex flex-col justify-between font-roboto">
            <div>
              <div class="flex items-center justify-between mb-2">
                <div class="font-bold text-base md:text-lg font-roboto">{{ tpl.category }}</div>
                <span class="text-xs px-2 md:px-3 py-1 rounded-full font-roboto whitespace-nowrap max-w-full overflow-hidden text-ellipsis"
                  :class="{
                    'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)]': tpl.visibility === 0,
                    'bg-gray-300 text-gray-700 dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)]': tpl.visibility === 1,
                    'bg-blue-200 text-blue-900 dark:bg-blue-900 dark:text-blue-200': tpl.visibility === 2
                  }"
                >
                  {{ visibilityLabel(tpl.visibility) }}
                </span>
              </div>
              <div class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-1 break-all font-roboto">ID: {{ tpl.templateId }}</div>
              <div class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-1 font-roboto">
                <div class="font-bold font-roboto">Fields:</div>
                <ul>
                  <li v-for="field in parseTemplateContent(tpl.content)" :key="field.name" class="font-roboto">
                    <span class="font-mono">{{ field.type }}</span>
                    <span class="font-bold font-roboto">{{ field.name }}</span>
                    <span v-if="field.format" class="italic text-gray-400 font-roboto">({{ field.format }})</span>
                    <span v-if="field.required" class="text-[var(--color-walory-red)] font-bold ml-2 font-roboto">required</span>
                  </li>
                </ul>
              </div>
            </div>
            <div class="flex gap-2 mt-4 font-roboto">
              <button @click="openEditTemplateModal(tpl)" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-3 py-1 rounded font-bold font-roboto shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition">Edit</button>
              <button @click="deleteTemplate(tpl.templateId)" class="bg-[var(--color-walory-red)] hover:bg-red-700 text-white font-bold px-3 py-1 rounded font-roboto shadow transition">Delete</button>
              <button @click="importTemplate(tpl.templateId)" class="bg-blue-200 dark:bg-blue-900 text-blue-900 dark:text-blue-200 px-3 py-1 rounded font-bold font-roboto shadow hover:bg-blue-300 dark:hover:bg-blue-800 transition">Clone</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Modal for create/edit collection -->

            <div
        v-if="showCollectionModal"
        class="fixed inset-0 flex items-center justify-center z-50 pointer-events-auto backdrop-blur-sm font-roboto"
        style="background: transparent;"
      >
        <div
          class="bg-white dark:bg-[var(--color-walory-dark-gold-light)] rounded-2xl p-8 w-full max-w-2xl shadow-lg relative font-roboto"
          style="max-height: 90vh; overflow-y: auto;"
        >
               <div v-if="collectionFormErrors.length" class="mb-2 text-red-600 text-sm font-bold flex flex-col gap-1">
                <div v-for="err in collectionFormErrors" :key="err">{{ err }}</div>
              </div>
          <h2 class="text-xl font-bold mb-4 font-roboto">{{ editingCollection ? 'Edit Collection' : 'Create New Collection' }}</h2>
          <form @submit.prevent="editingCollection ? updateCollection() : createCollection()" class="grid grid-cols-1 md:grid-cols-2 gap-6 font-roboto">
            <div>
              <label class="block font-bold mb-1 font-roboto">Title</label>
              <input v-model="collectionForm.title" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" required />
            </div>
            <div>
              <label class="block font-bold mb-1 font-roboto">Category</label>
              <input v-model="collectionForm.category" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" />
            </div>
            <div>
              <label class="block font-bold mb-1 font-roboto">Template</label>
              <select v-model="collectionForm.walorTemplateId" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                <option value="">None</option>
                <option v-for="tpl in templates" :key="tpl.templateId" :value="tpl.templateId">
                  {{ tpl.category }} ({{ tpl.templateId.slice(0, 8) }})
                </option>
              </select>
            </div>
            <div>
              <label class="block font-bold mb-1 font-roboto">Visibility</label>
              <select v-model="collectionForm.visibility" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]">
                <option :value="0">Public</option>
                <option :value="1">Private</option>
                <option :value="2">Friends</option>
              </select>
            </div>
            <div class="md:col-span-2">
              <label class="block font-bold mb-1 font-roboto">Description</label>
              <textarea v-model="collectionForm.description" class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]" required />
            </div>
            <!-- Thumbnail upload only in edit mode -->
            <div v-if="editingCollection" class="md:col-span-2 flex flex-col gap-2">
              <label class="block font-bold mb-1 font-roboto">Thumbnail Image</label>
              <div class="flex items-center gap-4">
                <img
                  v-if="editModalThumbnailUrl"
                  :src="editModalThumbnailUrl"
                  alt="Collection thumbnail"
                  class="w-24 h-24 object-cover rounded-xl border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)]"
                />
                <div v-else class="w-24 h-24 flex items-center justify-center bg-gray-200 dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl text-gray-400">
                  No Image
                </div>
                <form @submit.prevent="uploadEditModalThumbnail(collectionForm.collectionId)">
                  <input
                    type="file"
                    accept="image/*"
                    ref="editModalFileInput"
                    class="hidden"
                    @change="onEditModalThumbnailFileChange"
                  />
                  <button
                    type="button"
                    @click="triggerEditModalFileInput"
                    class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-3 py-1 rounded font-bold font-roboto shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition"
                  >
                    Choose Image
                  </button>
                  <span class="text-sm text-gray-600 dark:text-[var(--color-walory-silver)]">{{ editModalThumbnailFileName }}</span>
                  <button
                    v-if="editModalThumbnailFile"
                    type="submit"
                    class="bg-blue-200 dark:bg-blue-900 text-blue-900 dark:text-blue-200 px-3 py-1 rounded font-bold font-roboto shadow hover:bg-blue-300 dark:hover:bg-blue-800 transition"
                  >
                    Upload
                  </button>
                </form>
              </div>
            </div>
            <div class="flex gap-2 items-end md:col-span-2 font-roboto">
              <button type="submit" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] font-bold font-roboto px-6 py-2 rounded shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition">
                {{ editingCollection ? 'Update' : 'Create' }}
              </button>
              <button @click="closeCollectionModal" type="button" class="bg-gray-300 dark:bg-[var(--color-walory-dark-silver)] text-black dark:text-[var(--color-walory-gold-light)] px-4 py-2 rounded shadow font-roboto">Cancel</button>
            </div>
          </form>
        </div>
      </div>

      <div class="bg-[var(--color-walory-silver)] dark:bg-[var(--color-walory-dark-silver)] rounded-2xl shadow-lg border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-4 md:p-8 mb-8 font-roboto">
        <h2 class="text-xl md:text-2xl font-bold mb-4 p-2 font-roboto">Your Collections</h2>
        <button @click="openCollectionModal" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] font-bold font-roboto px-4 md:px-6 py-2 rounded shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition mb-4">
          Create Collection
        </button>
        <div v-if="collections.length === 0" class="text-gray-500 dark:text-[var(--color-walory-dark-silver)] font-roboto">No collections yet.</div>
        <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 md:gap-8 font-roboto">
          <div v-for="col in collections" :key="col.collectionId" class="bg-white/60 dark:bg-[var(--color-walory-dark-gold-light)] rounded-2xl shadow border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)] p-4 flex flex-col justify-between font-roboto">
            <div>
              <div class="flex justify-center mb-4">
                <img
                  v-if="col.thumbnailUrl"
                  :src="col.thumbnailUrl"
                  alt="Collection thumbnail"
                  class="w-24 h-24 md:w-32 md:h-32 object-cover rounded-xl border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)]"
                />
                <div v-else class="w-24 h-24 md:w-32 md:h-32 flex items-center justify-center bg-gray-200 dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl text-gray-400">
                  No Image
                </div>
              </div>
              <div class="flex flex-wrap items-center justify-between gap-2 mb-2">
                <div class="font-bold text-base md:text-xl font-roboto break-words">{{ col.title }}</div>
                <span class="text-xs px-2 md:px-3 py-1 rounded-full font-roboto whitespace-nowrap max-w-full overflow-hidden text-ellipsis"
                  :class="{
                    'bg-[var(--color-walory-gold)] text-[var(--color-walory-black)] dark:bg-[var(--color-walory-dark-gold)] dark:text-[var(--color-walory-silver)]': col.visibility === 0,
                    'bg-gray-300 text-gray-700 dark:bg-[var(--color-walory-dark-silver)] dark:text-[var(--color-walory-gold-light)]': col.visibility === 1,
                    'bg-blue-200 text-blue-900 dark:bg-blue-900 dark:text-blue-200': col.visibility === 2
                  }"
                >
                  {{ visibilityLabel(col.visibility) }}
                </span>
              </div>
              <div class="text-gray-700 dark:text-[var(--color-walory-dark-silver)] mb-1 font-roboto break-words">{{ col.description }}</div>
              <div class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-1 font-roboto break-words">Category: {{ col.category }}</div>
              <div class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-1 font-roboto break-words">Template: {{ col.walorTemplateId || 'None' }}</div>
              <div class="text-xs text-gray-500 dark:text-[var(--color-walory-dark-silver)] mb-1 font-roboto">Items: {{ col.walorInstance?.length ?? 0 }}</div>
            </div>
            <div class="flex flex-wrap gap-2 mt-4 font-roboto">
              <button @click="openEditCollectionModal(col)" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black] dark:text-[var(--color-walory-silver)] px-3 py-1 rounded font-bold font-roboto shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition">Edit</button>
              <button @click="deleteCollection(col.collectionId)" class="bg-[var(--color-walory-red)] hover:bg-red-700 text-white font-bold px-3 py-1 rounded font-roboto shadow transition">Delete</button>
              <button @click="openItemsModal(col)" class="bg-blue-200 dark:bg-blue-900 text-blue-900 dark:text-blue-200 px-3 py-1 rounded font-bold font-roboto shadow hover:bg-blue-300 dark:hover:bg-blue-800 transition">Manage Items</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Modal for managing items -->
      <div
        v-if="showItemsModal && itemsModalCollection"
        class="fixed inset-0 flex items-center justify-center z-50 pointer-events-auto backdrop-blur-sm font-roboto"
        style="background: transparent;"
      >
        <div
          class="bg-white dark:bg-[var(--color-walory-dark-gold-light)] rounded-2xl p-8 w-full max-w-2xl shadow-lg relative font-roboto"
          style="max-height: 90vh; overflow-y: auto;"
        >
          <div class="flex justify-between items-center mb-4 font-roboto">
            <h2 class="text-xl font-bold font-roboto">Manage Items in "{{ itemsModalCollection.title }}"</h2>
            <button @click="closeItemsModal" class="text-[var(--color-walory-red)] font-bold text-lg hover:underline font-roboto">Close</button>
          </div>
          <form @submit.prevent="submitItemForm" class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4 font-roboto">
            <div v-for="field in itemFields" :key="field.name" class="font-roboto">
              <label class="block font-bold mb-1 font-roboto">
                {{ field.name }}
                <span v-if="field.required" class="text-[var(--color-walory-red)]">*</span>
              </label>
              <input
                v-if="field.type === 'date'"
                v-model="itemForm[field.name]"
                type="date"
                :required="field.required"
                class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
              />
              <input
                v-else-if="field.type === 'number'"
                v-model="itemForm[field.name]"
                type="number"
                :required="field.required"
                class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
              />
              <input
                v-else-if="field.type === 'boolean'"
                v-model="itemForm[field.name]"
                type="checkbox"
                :required="field.required"
                class="mr-2"
              />
              <input
                v-else
                v-model="itemForm[field.name]"
                type="text"
                :required="field.required"
                class="border rounded px-3 py-2 w-full font-roboto bg-white dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)]"
              />
            </div>
            <!-- Item image upload and preview when editing an item -->
            <div v-if="editingItemId" class="md:col-span-2 flex flex-col gap-2 mb-4">
              <label class="block font-bold mb-1 font-roboto">Item Image</label>
              <div class="flex items-center gap-4">
                <img
                  v-if="itemModalThumbnailUrl && itemModalImageItemId === editingItemId"
                  :src="itemModalThumbnailUrl"
                  alt="Item thumbnail"
                  class="w-24 h-24 object-cover rounded-xl border border-[var(--color-walory-gold)] dark:border-[var(--color-walory-dark-gold)]"
                />
                <div v-else class="w-24 h-24 flex items-center justify-center bg-gray-200 dark:bg-[var(--color-walory-dark-gold-light)] rounded-xl text-gray-400">
                  No Image
                </div>
                <form @submit.prevent="uploadItemModalThumbnail(editingItemId)">
                  <input
                    type="file"
                    accept="image/*"
                    ref="itemModalFileInput"
                    class="hidden"
                    @change="onItemModalThumbnailFileChange"
                  />
                  <button
                    type="button"
                    @click="triggerItemModalFileInput"
                    class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-3 py-1 rounded font-bold font-roboto shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition"
                  >
                    Choose Image
                  </button>
                  <span class="text-sm text-gray-600 dark:text-[var(--color-walory-silver)]">{{ itemModalThumbnailFileName }}</span>
                  <button
                    v-if="itemModalThumbnailFile"
                    type="submit"
                    class="bg-blue-200 dark:bg-blue-900 text-blue-900 dark:text-blue-200 px-3 py-1 rounded font-bold font-roboto shadow hover:bg-blue-300 dark:hover:bg-blue-800 transition"
                  >
                    Upload
                  </button>
                </form>
              </div>
            </div>
            <div class="md:col-span-2 flex gap-2 mt-2 font-roboto">
              <button type="submit" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] font-bold font-roboto px-6 py-2 rounded shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition">
                {{ editingItemId ? 'Update' : 'Add Item' }}
              </button>
              <button v-if="editingItemId" @click="() => { editingItemId = null; resetItemForm(); itemModalThumbnailUrl = null; itemModalImageItemId = null; }" type="button" class="bg-gray-300 dark:bg-[var(--color-walory-dark-silver)] text-black dark:text-[var(--color-walory-gold-light)] px-4 py-2 rounded font-roboto">Cancel Edit</button>
            </div>
          </form>
          <div v-if="!itemsModalCollection.walorInstance || itemsModalCollection.walorInstance.length === 0" class="text-gray-500 dark:text-[var(--color-walory-dark-silver)] font-roboto">No items yet.</div>
          <ul>
            <li v-for="item in itemsModalCollection.walorInstance" :key="item.id" class="flex items-center justify-between border-b py-2 font-roboto">
              <div>
                <span v-if="itemFields.length">
                  <span v-for="field in itemFields" :key="field.name" class="mr-2 font-roboto">
                    <span class="font-bold font-roboto">{{ field.name }}:</span>
                    <span class="font-mono">{{ (typeof item.data === 'string' ? JSON.parse(item.data)[field.name] : item.data?.[field.name]) ?? '' }}</span>
                  </span>
                </span>
                <span v-else class="font-mono text-sm">{{ item.data }}</span>
              </div>
              <div class="flex gap-2 font-roboto">
                <button @click="startEditItem(item)" class="bg-[var(--color-walory-gold)] dark:bg-[var(--color-walory-dark-gold)] text-[var(--color-walory-black)] dark:text-[var(--color-walory-silver)] px-2 py-1 rounded text-xs font-bold font-roboto shadow hover:bg-[var(--color-walory-gold-dark)] dark:hover:bg-[var(--color-walory-dark-gold-dark)] transition">Edit</button>
                <button @click="deleteItem(item.id)" class="bg-[var(--color-walory-red)] hover:bg-red-700 text-white font-bold px-2 py-1 rounded text-xs font-roboto shadow transition">Delete</button>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, nextTick } from 'vue'

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
const collectionFormErrors = ref<string[]>([])
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

// --- Collection Modal State ---
const showCollectionModal = ref(false)

function openCollectionModal() {
  resetCollectionForm()
  editingCollection.value = false
  showCollectionModal.value = true
  collectionFormErrors.value = []
}
function openEditCollectionModal(col: any) {
  Object.assign(collectionForm, col)
  editingCollection.value = true
  showCollectionModal.value = true
  fetchEditModalThumbnail(col.collectionId)
  editModalThumbnailFile.value = null
  editModalThumbnailFileName.value = ''
}
function closeCollectionModal() {
  showCollectionModal.value = false
  resetCollectionForm()
  editingCollection.value = false
  collectionFormErrors.value = []
}

// --- Template Modal State ---
const showTemplateModal = ref(false)
const newTemplateFields = ref<{ name: string; type: string; required: boolean; format?: string }[]>([])
const newField = reactive({ name: '', type: 'string', required: false, format: '' })

function openTemplateModal() {
  showTemplateModal.value = true
  newTemplateFields.value = []
}
function closeTemplateModal() {
  showTemplateModal.value = false
}
function addField() {
  if (!newField.name || !newField.type) return
  newTemplateFields.value.push({ ...newField })
  newField.name = ''
  newField.type = 'string'
  newField.required = false
  newField.format = ''
}
function removeField(idx: number) {
  newTemplateFields.value.splice(idx, 1)
}
async function submitTemplateForm() {
  // Convert fields to JSON schema
  const properties: Record<string, any> = {}
  const required: string[] = []
  newTemplateFields.value.forEach(f => {
    properties[f.name] = { type: f.type }
    if (f.format) properties[f.name].format = f.format
    if (f.required) required.push(f.name)
  })
  const schema = {
    type: 'object',
    required,
    properties,
    additionalProperties: false
  }
  templateForm.content = JSON.stringify(schema)
  templateForm.category = templateForm.category || 'Uncategorized'
  templateForm.visibility = templateForm.visibility ?? 1
  await createTemplate()
  closeTemplateModal()
}

// --- Template Display Helper ---
function parseTemplateContent(content: string) {
  try {
    const obj = typeof content === 'string' ? JSON.parse(content) : content
    const required = Array.isArray(obj.required) ? obj.required : []
    const properties = obj.properties || {}
    return Object.entries(properties).map(([name, prop]: [string, any]) => ({
      name,
      type: prop.type,
      format: prop.format,
      required: required.includes(name)
    }))
  } catch {
    return []
  }
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
  const res = await fetch('http://localhost:8080/collection', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(payload)
  })

  collectionFormErrors.value = []

  if (!res.ok) {
    let errText = await res.text()
    try {
      const errObj = JSON.parse(errText)
      if (errObj.errors) {
        // Flatten errors object to array of messages
        collectionFormErrors.value = Object.values(errObj.errors).flat() as string[]
      } else if (errObj.title) {
        collectionFormErrors.value = [errObj.title]
      } else {
        collectionFormErrors.value = [errText]
      }
    } catch {
      collectionFormErrors.value = [errText]
    }
    return
  }

  collectionFormErrors.value = []
  resetCollectionForm()
  showCollectionModal.value = false
  await fetchCollectionsWithThumbnails()
}

function startEdit(col: any) {
  openEditCollectionModal(col)
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
  showCollectionModal.value = false
  await fetchCollectionsWithThumbnails()
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
  showCollectionModal.value = false
}
function resetCollectionForm() {
  collectionForm.collectionId = ''
  collectionForm.title = ''
  collectionForm.description = ''
  collectionForm.category = ''
  collectionForm.visibility = 0
  collectionForm.walorTemplateId = ''
}

// --- Manage Items Modal State ---
const showItemsModal = ref(false)
const itemsModalCollection = ref<any | null>(null)
const itemFields = ref<{ name: string; type: string; required: boolean; format?: string }[]>([])
const itemForm = reactive<{ [key: string]: any }>({})
const editingItemId = ref<string | null>(null)

// --- Item image upload state ---
const itemModalFileInput = ref<HTMLInputElement | null>(null)
const itemModalThumbnailFile = ref<File | null>(null)
const itemModalThumbnailFileName = ref('')
const itemModalThumbnailUrl = ref<string | null>(null)
const itemModalImageItemId = ref<string | null>(null)

function triggerItemModalFileInput() {
  if (itemModalFileInput.value) itemModalFileInput.value.click()
}
function onItemModalThumbnailFileChange(e: Event) {
  const files = (e.target as HTMLInputElement).files
  if (files && files[0]) {
    itemModalThumbnailFile.value = files[0]
    itemModalThumbnailFileName.value = files[0].name
  } else {
    itemModalThumbnailFile.value = null
    itemModalThumbnailFileName.value = ''
  }
}
async function uploadItemModalThumbnail(itemId: string) {
  const file = itemModalThumbnailFile.value
  if (!file) return
  const formData = new FormData()
  formData.append('Avatar', file)
  try {
    await fetch(`http://localhost:8080/api/WalorInstances/${itemId}/image`, {
      method: 'POST',
      credentials: 'include',
      body: formData
    })
    itemModalThumbnailFile.value = null
    itemModalThumbnailFileName.value = ''
    await fetchItemModalThumbnail(itemId)
    await fetchCollections()
  } catch (e) {
    alert('Failed to upload item image')
  }
}
async function fetchItemModalThumbnail(itemId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/WalorInstances/${itemId}/image`, {
      credentials: 'include'
    })
    if (res.ok) {
      const blob = await res.blob()
      itemModalThumbnailUrl.value = URL.createObjectURL(blob)
      itemModalImageItemId.value = itemId
    } else {
      itemModalThumbnailUrl.value = null
      itemModalImageItemId.value = null
    }
  } catch {
    itemModalThumbnailUrl.value = null
    itemModalImageItemId.value = null
  }
}

function openItemsModal(collection: any) {
  itemsModalCollection.value = collection
  showItemsModal.value = true
  // Parse template fields
  const tpl = templates.value.find(t => t.templateId === collection.walorTemplateId)
  if (tpl) {
    itemFields.value = parseTemplateContent(tpl.content)
  } else {
    itemFields.value = []
  }
  resetItemForm()
}

function closeItemsModal() {
  showItemsModal.value = false
  itemsModalCollection.value = null
  editingItemId.value = null
  resetItemForm()
  itemModalThumbnailUrl.value = null
  itemModalImageItemId.value = null
}

function resetItemForm() {
  itemFields.value.forEach(f => itemForm[f.name] = '')
}

function startEditItem(item: any) {
  editingItemId.value = item.id
  try {
    const data = typeof item.data === 'string' ? JSON.parse(item.data) : item.data
    itemFields.value.forEach(f => itemForm[f.name] = data[f.name] ?? '')
  } catch {
    itemFields.value.forEach(f => itemForm[f.name] = '')
  }
  fetchItemModalThumbnail(item.id)
}

async function submitItemForm() {
  const data: Record<string, any> = {}
  let invalidDate = false
  itemFields.value.forEach(f => {
    let value = itemForm[f.name]
    if (f.type === 'date' && value) {
      // Accept only YYYY-MM-DD
      if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
        invalidDate = true
      }
    }
    data[f.name] = value
  })
  if (invalidDate) {
    alert('Please enter the date in YYYY-MM-DD format.')
    return
  }
  if (editingItemId.value) {
    const payload = {
      walorInstanceId: editingItemId.value,
      data: JSON.stringify(data)
    }
    console.log('PUT payload:', payload)
    await fetch('http://localhost:8080/api/WalorInstances', {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(payload)
    })
  } else {
    const payload = {
      collectionId: itemsModalCollection.value.collectionId,
      data: JSON.stringify(data)
    }
    console.log('POST payload:', payload)
    await fetch('http://localhost:8080/api/WalorInstances', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(payload)
    })
  }
  await fetchCollections()
  itemsModalCollection.value = collections.value.find(c => c.collectionId === itemsModalCollection.value.collectionId)
  resetItemForm()
  editingItemId.value = null
  itemModalThumbnailUrl.value = null
  itemModalImageItemId.value = null
}

async function deleteItem(id: string) {
  await fetch(`http://localhost:8080/api/WalorInstances/${id}`, {
    method: 'DELETE',
    credentials: 'include'
  })
  await fetchCollections()
  itemsModalCollection.value = collections.value.find(c => c.collectionId === itemsModalCollection.value.collectionId)
  itemModalThumbnailUrl.value = null
  itemModalImageItemId.value = null
}

const editTemplateFields = ref<{ name: string; type: string; required: boolean; format?: string }[]>([])
const editField = reactive({ name: '', type: 'string', required: false, format: '' })

function openEditTemplateModal(tpl: any) {
  editingTemplate.value = true
  templateForm.templateId = tpl.templateId
  templateForm.category = tpl.category
  templateForm.visibility = tpl.visibility
  // Parse fields from content
  try {
    const obj = typeof tpl.content === 'string' ? JSON.parse(tpl.content) : tpl.content
    const required = Array.isArray(obj.required) ? obj.required : []
    const properties = obj.properties || {}
    editTemplateFields.value = Object.entries(properties).map(([name, prop]: [string, any]) => ({
      name,
      type: prop.type,
      format: prop.format,
      required: required.includes(name)
    }))
  } catch {
    editTemplateFields.value = []
  }
}
function closeEditTemplateModal() {
  editingTemplate.value = false
  templateForm.templateId = ''
  templateForm.category = ''
  templateForm.visibility = 0
  templateForm.content = ''
  editTemplateFields.value = []
}
function addEditField() {
  if (!editField.name || !editField.type) return
  editTemplateFields.value.push({ ...editField })
  editField.name = ''
  editField.type = 'string'
  editField.required = false
  editField.format = ''
}
function removeEditField(idx: number) {
  editTemplateFields.value.splice(idx, 1)
}
async function submitEditTemplateForm() {
  // Convert fields to JSON schema
  const properties: Record<string, any> = {}
  const required: string[] = []
  editTemplateFields.value.forEach(f => {
    properties[f.name] = { type: f.type }
    if (f.format) properties[f.name].format = f.format
    if (f.required) required.push(f.name)
  })
  const schema = {
    type: 'object',
    required,
    properties,
    additionalProperties: false
  }
  templateForm.content = JSON.stringify(schema)
  await updateTemplate()
  closeEditTemplateModal()
}

const editModalFileInput = ref<HTMLInputElement | null>(null)
const editModalThumbnailFile = ref<File | null>(null)
const editModalThumbnailFileName = ref('')
const editModalThumbnailUrl = ref<string | null>(null)

function triggerEditModalFileInput() {
  if (editModalFileInput.value) editModalFileInput.value.click()
}
function onEditModalThumbnailFileChange(e: Event) {
  const files = (e.target as HTMLInputElement).files
  if (files && files[0]) {
    editModalThumbnailFile.value = files[0]
    editModalThumbnailFileName.value = files[0].name
  } else {
    editModalThumbnailFile.value = null
    editModalThumbnailFileName.value = ''
  }
}
async function uploadEditModalThumbnail(collectionId: string) {
  const file = editModalThumbnailFile.value
  if (!file) return
  const formData = new FormData()
  formData.append('Avatar', file)
  try {
    await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/${collectionId}/image`, {
      method: 'POST',
      credentials: 'include',
      body: formData
    })
    editModalThumbnailFile.value = null
    editModalThumbnailFileName.value = ''
    await fetchEditModalThumbnail(collectionId)
    await fetchCollections() // update list thumbnails if needed
  } catch (e) {
    alert('Failed to upload image')
  }
}
async function fetchEditModalThumbnail(collectionId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/${collectionId}/image`, {
      credentials: 'include'
    })
    if (res.ok) {
      const blob = await res.blob()
      editModalThumbnailUrl.value = URL.createObjectURL(blob)
    } else {
      editModalThumbnailUrl.value = null
    }
  } catch {
    editModalThumbnailUrl.value = null
  }
}

// --- Thumbnails for collection list ---
async function fetchCollectionThumbnail(collectionId: string) {
  try {
    const res = await fetch(`http://localhost:8080/api/collections/${collectionId}/interactions/${collectionId}/image`, {
      credentials: 'include'
    })
    if (res.ok) {
      const blob = await res.blob()
      const url = URL.createObjectURL(blob)
      const col = collections.value.find((c: any) => c.collectionId === collectionId)
      if (col) col.thumbnailUrl = url
    }
  } catch {}
}
async function fetchCollectionsWithThumbnails() {
  const res = await fetch('http://localhost:8080/api/Browse/collections/private', { credentials: 'include' })
  collections.value = await res.json()
  await nextTick()
  for (const col of collections.value) {
    await fetchCollectionThumbnail(col.collectionId)
  }
}

// --- Helpers ---
function visibilityLabel(val: number) {
  if (val === 0) return 'Public'
  if (val === 1) return 'Private'
  if (val === 2) return 'Friends'
  return 'Unknown'
}

function generateGUID() {
  // RFC4122 version 4 compliant UUID
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    const r = Math.random() * 16 | 0,
      v = c === 'x' ? r : (r & 0x3 | 0x8)
    return v.toString(16)
  })
}

onMounted(() => {
  fetchCollectionsWithThumbnails()
  fetchTemplates()
})
</script>