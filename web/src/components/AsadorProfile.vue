<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getMyProfile, updateMyProfile, getSpecialties, getMyPortfolioImages, uploadPortfolioImages, deletePortfolioImage, type AsadorProfile, type Specialty, type PortfolioImage } from '../api/asador'

const profile = ref<AsadorProfile | null>(null)
const specialties = ref<Specialty[]>([])
const loading = ref(true)
const saving = ref(false)
const success = ref('')
const error = ref('')

const form = ref({
  description: '',
  instagram: '',
  mainCity: '',
  whatsApp: '',
  specialtyIds: [] as string[],
})

const portfolioImages = ref<PortfolioImage[]>([])
const portfolioLoading = ref(false)
const uploadLoading = ref(false)
const selectedFiles = ref<File[]>([])
const previewUrls = ref<string[]>([])
const uploadError = ref('')

onMounted(async () => {
  try {
    const [p, s] = await Promise.all([getMyProfile(), getSpecialties()])
    profile.value = p
    specialties.value = s
    form.value = {
      description: p.description ?? '',
      instagram: p.instagram ?? '',
      mainCity: p.mainCity,
      whatsApp: p.whatsApp ?? '',
      specialtyIds: p.specialtyIds,
    }
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
  await fetchPortfolio()
})

async function fetchPortfolio() {
  portfolioLoading.value = true
  try {
    portfolioImages.value = await getMyPortfolioImages()
  } catch {
    portfolioImages.value = []
  } finally {
    portfolioLoading.value = false
  }
}

function handleFileSelect(e: Event) {
  const input = e.target as HTMLInputElement
  const files = Array.from(input.files ?? [])
  if (files.length === 0) return
  const total = files.length
  if (total > 5) {
    uploadError.value = 'Máximo 5 imágenes por subida'
    input.value = ''
    return
  }
  previewUrls.value.forEach(u => URL.revokeObjectURL(u))
  selectedFiles.value = files
  previewUrls.value = files.map(f => URL.createObjectURL(f))
  uploadError.value = ''
}

async function handleUpload() {
  if (selectedFiles.value.length === 0) return
  uploadLoading.value = true
  uploadError.value = ''
  try {
    await uploadPortfolioImages(selectedFiles.value)
    selectedFiles.value = []
    previewUrls.value = []
    await fetchPortfolio()
  } catch (e: any) {
    uploadError.value = e?.response?.data?.message || 'Error al subir imágenes'
  } finally {
    uploadLoading.value = false
  }
}

async function handleDelete(id: string) {
  try {
    await deletePortfolioImage(id)
    await fetchPortfolio()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al eliminar imagen'
  }
}

function statusLabel(img: PortfolioImage): string {
  if (img.isApproved === true) return 'Habilitada'
  if (img.isApproved === false) return 'Rechazada'
  return 'Pendiente'
}

function statusClass(img: PortfolioImage): string {
  if (img.isApproved === true) return 'status-habilitada'
  if (img.isApproved === false) return 'status-rechazada'
  return 'status-pendiente'
}

function toggleSpecialty(id: string) {
  const idx = form.value.specialtyIds.indexOf(id)
  if (idx >= 0) form.value.specialtyIds.splice(idx, 1)
  else form.value.specialtyIds.push(id)
}

async function save() {
  saving.value = true
  success.value = ''
  error.value = ''
  try {
    await updateMyProfile({
      description: form.value.description || undefined,
      instagram: form.value.instagram || undefined,
      mainCity: form.value.mainCity,
      whatsApp: form.value.whatsApp || undefined,
      specialtyIds: form.value.specialtyIds,
    })
    success.value = 'Perfil actualizado correctamente'
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al guardar'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="profile">
    <h2>Mi Perfil de Asador</h2>

    <div v-if="loading" class="loading">Cargando perfil...</div>

    <form v-else-if="profile" @submit.prevent="save">
      <div class="status-badge" :class="profile.status.toLowerCase()">
        {{ profile.status }}
      </div>

      <div class="grid">
        <label>
          Nombre
          <input :value="profile.name" disabled />
        </label>
        <label>
          Email
          <input :value="profile.email" disabled />
        </label>
      </div>

      <label>
        WhatsApp
        <input v-model="form.whatsApp" placeholder="+5491112345678" />
      </label>

      <label>
        Ciudad principal
        <input v-model="form.mainCity" placeholder="Ej: Buenos Aires" required />
      </label>

      <label>
        Instagram
        <input v-model="form.instagram" placeholder="@usuario" />
      </label>

      <label>
        Descripción
        <textarea v-model="form.description" rows="4" placeholder="Contanos sobre tu experiencia..."></textarea>
      </label>

      <label>Especialidades</label>
      <div class="chips">
        <button
          v-for="s in specialties"
          :key="s.id"
          type="button"
          class="chip"
          :class="{ active: form.specialtyIds.includes(s.id) }"
          @click="toggleSpecialty(s.id)"
        >
          {{ s.name }}
        </button>
      </div>

      <p v-if="success" class="success">{{ success }}</p>
      <p v-if="error" class="error">{{ error }}</p>

      <button type="submit" :disabled="saving" class="save-btn">
        {{ saving ? 'Guardando...' : 'Guardar cambios' }}
      </button>
    </form>

    <section class="portfolio">
      <h2>Portafolio</h2>

      <div class="upload-area">
        <div class="upload-inputs">
          <input
            type="file"
            accept="image/jpeg,image/png,image/webp"
            multiple
            @change="handleFileSelect"
            :key="'file-input-' + previewUrls.length"
          />
          <button
            v-if="selectedFiles.length > 0"
            :disabled="uploadLoading"
            class="upload-btn"
            @click="handleUpload"
          >
            {{ uploadLoading ? 'Subiendo...' : `Subir ${selectedFiles.length} imagen${selectedFiles.length > 1 ? 'es' : ''}` }}
          </button>
        </div>
        <p v-if="uploadError" class="error">{{ uploadError }}</p>
        <div v-if="previewUrls.length > 0" class="previews">
          <div v-for="(url, i) in previewUrls" :key="url" class="preview-thumb">
            <img :src="url" :alt="'Preview ' + (i + 1)" />
            <span class="preview-name">{{ selectedFiles[i]?.name }}</span>
          </div>
        </div>
      </div>

      <p class="recommendation">
        Recomendación: cargá al menos 3 imágenes para que los clientes conozcan tu trabajo.
      </p>

      <div v-if="portfolioLoading" class="loading">Cargando imágenes...</div>

      <p v-else-if="portfolioImages.length === 0" class="empty">
        No hay imágenes en tu portafolio. Subí tu primera imagen.
      </p>

      <div v-else class="image-grid">
        <div v-for="img in portfolioImages" :key="img.id" class="image-card">
          <img :src="img.imageUrl" :alt="'Imagen de portafolio'" />
          <div class="image-info">
            <span class="image-date">{{ new Date(img.createdAt).toLocaleDateString() }}</span>
            <span class="portfolio-status-badge" :class="statusClass(img)">{{ statusLabel(img) }}</span>
          </div>
          <div class="image-actions">
            <button class="btn-delete" @click="handleDelete(img.id)">Eliminar</button>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
.profile {
  max-width: 600px;
}
h2 {
  margin-bottom: 1.25rem;
  color: var(--text-muted);
  font-size: 1.25rem;
}
.loading {
  color: var(--text-muted);
  padding: 2rem 0;
}
.empty {
  color: var(--text-muted);
  padding: 1rem 0;
}
.status-badge {
  display: inline-block;
  padding: .25rem .75rem;
  border-radius: 999px;
  font-size: .8rem;
  font-weight: 600;
  margin-bottom: 1.25rem;
}
.status-badge.verificado {
  background: #1a3a1a;
  color: var(--primary);
}
.status-badge.pendiente {
  background: #3a2a1a;
  color: #f59e0b;
}
.grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}
label {
  display: block;
  margin-bottom: 1rem;
  font-size: .85rem;
  color: var(--text-muted);
}
input, textarea {
  width: 100%;
  padding: .6rem .75rem;
  margin-top: .25rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: var(--bg);
  color: var(--text);
  font-size: .9rem;
}
input:disabled {
  opacity: .5;
}
input:focus, textarea:focus {
  outline: none;
  border-color: var(--primary);
}
textarea {
  resize: vertical;
}
.chips {
  display: flex;
  flex-wrap: wrap;
  gap: .5rem;
  margin-bottom: 1rem;
}
.chip {
  padding: .4rem 1rem;
  border: 1px solid var(--border-light);
  border-radius: 999px;
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: .85rem;
  transition: all .15s;
}
.chip.active {
  background: var(--primary);
  border-color: var(--primary);
  color: #fff;
}
.success {
  color: var(--success);
  font-size: .85rem;
  margin-bottom: .75rem;
}
.error {
  color: var(--error);
  font-size: .85rem;
  margin-bottom: .75rem;
}
.save-btn {
  padding: .7rem 1.5rem;
  border: none;
  border-radius: .5rem;
  background: var(--gradient);
  color: #fff;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
}
.save-btn:disabled {
  opacity: .6;
  cursor: not-allowed;
}

.portfolio {
  margin-top: 3rem;
  border-top: 1px solid var(--border);
  padding-top: 2rem;
}
.upload-area {
  margin-bottom: 1.5rem;
}
.upload-inputs {
  display: flex;
  gap: .75rem;
  align-items: center;
}
.upload-inputs input[type="file"] {
  flex: 1;
}
.upload-inputs input[type="file"]::file-selector-button {
  padding: .4rem .8rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: var(--bg-elevated);
  color: var(--text);
  cursor: pointer;
  margin-right: .5rem;
}
.upload-btn {
  padding: .5rem 1rem;
  border: none;
  border-radius: .5rem;
  background: var(--gradient);
  color: #fff;
  font-size: .85rem;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
}
.upload-btn:disabled {
  opacity: .6;
  cursor: not-allowed;
}
.previews {
  display: flex;
  gap: .5rem;
  margin-top: .75rem;
  flex-wrap: wrap;
}
.preview-thumb {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: .25rem;
}
.preview-thumb img {
  width: 100px;
  height: 75px;
  border-radius: .5rem;
  object-fit: cover;
}
.preview-name {
  font-size: .65rem;
  color: var(--text-dim);
  max-width: 100px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.recommendation {
  margin-top: -0.75rem;
  margin-bottom: 1.5rem;
  font-size: .8rem;
  color: var(--text-dim);
  font-style: italic;
}
.image-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1rem;
}
.image-card {
  background: var(--bg-surface);
  border-radius: .75rem;
  overflow: hidden;
  border: 1px solid var(--border);
}
.image-card img {
  width: 100%;
  height: 150px;
  object-fit: cover;
  display: block;
}
.image-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: .5rem .75rem;
  gap: .5rem;
}
.image-date {
  font-size: .75rem;
  color: var(--text-dim);
  white-space: nowrap;
}
.portfolio-status-badge {
  display: inline-block;
  padding: .15rem .5rem;
  border-radius: 999px;
  font-size: .7rem;
  font-weight: 600;
  white-space: nowrap;
}
.status-habilitada {
  background: #1a3a1a;
  color: var(--primary);
}
.status-pendiente {
  background: #3a2a1a;
  color: #f59e0b;
}
.status-rechazada {
  background: #3a1a1a;
  color: #ef4444;
}
.image-actions {
  padding: 0 .75rem .6rem;
}
.btn-delete {
  width: 100%;
  padding: .35rem;
  border: none;
  border-radius: .5rem;
  background: transparent;
  color: var(--text-dim);
  font-size: .8rem;
  cursor: pointer;
  transition: all .15s;
}
.btn-delete:hover {
  background: #3a1a1a;
  color: #ef4444;
}
</style>
