<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  getMyEvents, getEventDetail, createEvent, selectApplication, uploadEventImages,
  type EventItem, type EventDetail, type CreateEventRequest
} from '../api/events'
import { getSpecialties, type Specialty } from '../api/asador'

const view = ref<'list' | 'create' | 'contracted' | 'detail'>('list')
const events = ref<EventItem[]>([])
const contractedEvents = computed(() => events.value.filter(e => e.status === 'Asignado'))
const selectedEvent = ref<EventDetail | null>(null)
const loading = ref(true)
const error = ref('')
const success = ref('')
const selectingId = ref<string | null>(null)
const specialties = ref<Specialty[]>([])
const imageFiles = ref<File[]>([])
const imagePreviews = ref<string[]>([])

const form = ref<CreateEventRequest>({
  date: '',
  time: '',
  city: '',
  address: '',
  peopleCount: 1,
  eventType: '',
  serviceDesired: '',
  notes: ''
})

const eventTypes = [
  'Cumpleaños',
  'Evento empresarial',
  'Cena familiar',
  'Casamiento',
  'Fiesta',
  'Otro'
]

onMounted(async () => {
  await Promise.all([fetchEvents(), fetchSpecialties()])
})

async function fetchSpecialties() {
  try {
    specialties.value = await getSpecialties()
  } catch {
    specialties.value = []
  }
}

async function fetchEvents() {
  loading.value = true
  error.value = ''
  try {
    events.value = await getMyEvents()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al cargar eventos'
  } finally {
    loading.value = false
  }
}

async function handleCreate() {
  error.value = ''
  success.value = ''
  try {
    const { id } = await createEvent(form.value)

    if (imageFiles.value.length > 0) {
      await uploadEventImages(id, imageFiles.value)
    }

    success.value = 'Evento creado correctamente'
    form.value = { date: '', time: '', city: '', address: '', peopleCount: 1, eventType: '', serviceDesired: '', notes: '' }
    imageFiles.value = []
    imagePreviews.value = []
    view.value = 'list'
    await fetchEvents()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al crear evento'
  }
}

function handleFileSelect(e: Event) {
  const input = e.target as HTMLInputElement
  if (!input.files) return

  const selected = Array.from(input.files)
  const remaining = 3 - imageFiles.value.length
  if (selected.length > remaining) {
    error.value = `Solo puedes agregar hasta ${remaining} imagen(es) más`
    input.value = ''
    return
  }

  selected.forEach(file => {
    imageFiles.value.push(file)
    const reader = new FileReader()
    reader.onload = () => imagePreviews.value.push(reader.result as string)
    reader.readAsDataURL(file)
  })

  input.value = ''
}

function removeImage(index: number) {
  imageFiles.value.splice(index, 1)
  imagePreviews.value.splice(index, 1)
}

async function openDetail(id: string) {
  error.value = ''
  selectedEvent.value = null
  view.value = 'detail'
  try {
    selectedEvent.value = await getEventDetail(id)
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al cargar detalle'
  }
}

async function handleSelect(applicationId: string) {
  if (!selectedEvent.value) return
  selectingId.value = applicationId
  error.value = ''
  success.value = ''
  try {
    await selectApplication(selectedEvent.value.id, applicationId)
    success.value = 'Asador seleccionado correctamente'
    selectedEvent.value = await getEventDetail(selectedEvent.value.id)
    await fetchEvents()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al seleccionar asador'
  } finally {
    selectingId.value = null
  }
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString('es-AR', {
    day: 'numeric', month: 'long', year: 'numeric'
  })
}

function formatTime(timeStr: string) {
  const [h, m] = timeStr.split(':')
  return `${h}:${m} hs`
}

function formatDateTime(dateStr: string) {
  return new Date(dateStr).toLocaleDateString('es-AR', {
    day: 'numeric', month: 'short', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

function getStatusClass(status: string) {
  if (status === 'Disponible') return 'status-available'
  if (status === 'Asignado') return 'status-assigned'
  return ''
}
</script>

<template>
  <div class="client-events">
    <div class="tabs">
      <button :class="{ active: view === 'list' }" @click="view = 'list'">Mis Eventos</button>
      <button :class="{ active: view === 'contracted' }" @click="view = 'contracted'">Contratados</button>
      <button :class="{ active: view === 'create' }" @click="view = 'create'">Crear Evento</button>
    </div>

    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="success" class="success">{{ success }}</p>

    <!-- List view -->
    <template v-if="view === 'list'">
      <p v-if="loading" class="loading">Cargando eventos...</p>
      <p v-else-if="events.length === 0" class="empty">
        No creaste ningún evento todavía.
      </p>
      <div v-else class="event-list">
        <div
          v-for="ev in events"
          :key="ev.id"
          class="event-row"
          @click="openDetail(ev.id)"
        >
          <div class="event-main">
            <span class="ev-type">{{ ev.eventType }}</span>
            <span class="ev-date">{{ formatDate(ev.date) }} - {{ formatTime(ev.time) }}</span>
            <span class="ev-location">{{ ev.city }}, {{ ev.address }}</span>
          </div>
          <div class="event-meta">
            <span class="ev-people">{{ ev.peopleCount }} pers.</span>
            <span class="ev-status" :class="getStatusClass(ev.status)">{{ ev.status }}</span>
            <span class="ev-apps">{{ ev.applicationCount }} postulaciones</span>
          </div>
          <span class="arrow">&rarr;</span>
        </div>
      </div>
    </template>

    <!-- Contracted view -->
    <template v-else-if="view === 'contracted'">
      <p v-if="loading" class="loading">Cargando eventos...</p>
      <p v-else-if="contractedEvents.length === 0" class="empty">
        No tenés eventos contratados todavía.
      </p>
      <div v-else class="event-list">
        <div
          v-for="ev in contractedEvents"
          :key="ev.id"
          class="event-row"
          @click="openDetail(ev.id)"
        >
          <div class="event-main">
            <span class="ev-type">{{ ev.eventType }}</span>
            <span class="ev-date">{{ formatDate(ev.date) }} - {{ formatTime(ev.time) }}</span>
            <span class="ev-location">{{ ev.city }}, {{ ev.address }}</span>
          </div>
          <div class="event-meta">
            <span class="ev-people">{{ ev.peopleCount }} pers.</span>
            <span class="ev-status status-assigned">Asignado</span>
          </div>
          <span class="arrow">&rarr;</span>
        </div>
      </div>
    </template>

    <!-- Create view -->
    <form v-else-if="view === 'create'" class="create-form" @submit.prevent="handleCreate">
      <div class="grid">
        <label>
          Fecha
          <input v-model="form.date" type="date" required />
        </label>
        <label>
          Horario
          <input v-model="form.time" type="time" required />
        </label>
      </div>

      <div class="grid">
        <label>
          Ciudad
          <input v-model="form.city" placeholder="Ej: Buenos Aires" required />
        </label>
        <label>
          Dirección
          <input v-model="form.address" placeholder="Ej: Av. Siempre Viva 123" required />
        </label>
      </div>

      <div class="grid">
        <label>
          Cantidad de personas
          <input v-model.number="form.peopleCount" type="number" min="1" required />
        </label>
        <label>
          Tipo de evento
          <select v-model="form.eventType" required>
            <option value="" disabled>Seleccionar...</option>
            <option v-for="t in eventTypes" :key="t" :value="t">{{ t }}</option>
          </select>
        </label>
      </div>

      <label>
        Servicio deseado
        <select v-model="form.serviceDesired">
          <option value="">Seleccionar...</option>
          <option v-for="s in specialties" :key="s.id" :value="s.name">{{ s.name }}</option>
        </select>
      </label>

      <label>
        Notas adicionales
        <textarea v-model="form.notes" rows="3" placeholder="Cualquier detalle que quieras agregar..."></textarea>
      </label>

      <div class="image-upload">
        <span class="label">Imágenes de referencia (opcional, máx. 3)</span>
        <div class="previews" v-if="imagePreviews.length">
          <div v-for="(preview, i) in imagePreviews" :key="i" class="preview-item">
            <img :src="preview" alt="preview" />
            <button type="button" class="remove-img" @click="removeImage(i)">&times;</button>
          </div>
        </div>
        <label v-if="imageFiles.length < 3" class="file-btn">
          <input type="file" accept="image/*" multiple @change="handleFileSelect" />
          + Agregar imagen
        </label>
      </div>

      <button type="submit" class="create-btn">Publicar Evento</button>
    </form>

    <!-- Detail view -->
    <template v-else-if="view === 'detail' && selectedEvent">
      <button class="back-btn" @click="view = 'list'">&larr; Volver</button>

      <div class="detail-card">
        <div class="detail-header">
          <h3>{{ selectedEvent.eventType }}</h3>
          <span class="ev-status" :class="getStatusClass(selectedEvent.status)">{{ selectedEvent.status }}</span>
        </div>
        <div class="detail-body">
          <div class="dl">
            <div class="dt">Fecha</div><div class="dd">{{ formatDate(selectedEvent.date) }}</div>
            <div class="dt">Horario</div><div class="dd">{{ formatTime(selectedEvent.time) }}</div>
            <div class="dt">Ubicación</div><div class="dd">{{ selectedEvent.city }}, {{ selectedEvent.address }}</div>
            <div class="dt">Personas</div><div class="dd">{{ selectedEvent.peopleCount }}</div>
            <div class="dt" v-if="selectedEvent.serviceDesired">Servicio</div>
            <div class="dd" v-if="selectedEvent.serviceDesired">{{ selectedEvent.serviceDesired }}</div>
            <div class="dt" v-if="selectedEvent.notes">Notas</div>
            <div class="dd" v-if="selectedEvent.notes">{{ selectedEvent.notes }}</div>
          </div>
          <div class="event-images" v-if="selectedEvent.imageUrls.length">
            <span class="label">Imágenes de referencia</span>
            <div class="image-gallery">
              <img v-for="(url, i) in selectedEvent.imageUrls" :key="i" :src="url" alt="Evento" />
            </div>
          </div>
        </div>
      </div>

      <div class="applications-section" v-if="selectedEvent.status === 'Disponible'">
        <h3>Postulaciones ({{ selectedEvent.applications.length }})</h3>

        <p v-if="selectedEvent.applications.length === 0" class="empty">
          Aún no hay asadores postulados.
        </p>

        <div v-else class="app-grid">
          <div v-for="app in selectedEvent.applications" :key="app.id" class="app-card">
            <div class="app-header">
              <div class="app-avatar">{{ app.asadorName.charAt(0) }}</div>
              <div>
                <div class="app-name">{{ app.asadorName }}</div>
                <div class="app-city">{{ app.mainCity }}</div>
              </div>
            </div>

            <div class="app-rating" v-if="app.averageRating > 0">
              {{ app.averageRating.toFixed(1) }} ★
            </div>

            <div class="app-specialties" v-if="app.specialtyNames.length">
              <span v-for="s in app.specialtyNames" :key="s" class="spec-chip">{{ s }}</span>
            </div>

            <p class="app-desc" v-if="app.description">{{ app.description }}</p>

            <div class="app-meta">
              <span class="app-status">{{ app.status }}</span>
              <span class="app-date">Postulado {{ formatDateTime(app.createdAt) }}</span>
            </div>

            <button
              v-if="app.status === 'Pendiente'"
              class="select-btn"
              :disabled="selectingId === app.id"
              @click="handleSelect(app.id)"
            >
              {{ selectingId === app.id ? 'Seleccionando...' : 'Seleccionar Asador' }}
            </button>
          </div>
        </div>
      </div>

      <div class="assigned-info" v-else-if="selectedEvent.status === 'Asignado'">
        <p class="success">Este evento ya tiene un asador asignado.</p>
        <div v-for="app in selectedEvent.applications.filter(a => a.status === 'Aceptada')" :key="app.id" class="app-card selected">
          <div class="app-header">
            <div class="app-avatar">{{ app.asadorName.charAt(0) }}</div>
            <div>
              <div class="app-name">{{ app.asadorName }}</div>
              <div class="app-city">{{ app.mainCity }}</div>
            </div>
          </div>
          <div class="app-specialties" v-if="app.specialtyNames.length">
            <span v-for="s in app.specialtyNames" :key="s" class="spec-chip">{{ s }}</span>
          </div>
          <div class="app-meta">
            <span class="app-status accepted">Aceptada</span>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.client-events {
  max-width: 720px;
}
.tabs {
  display: flex;
  gap: .5rem;
  margin-bottom: 1.5rem;
  border-bottom: 1px solid var(--border);
  padding-bottom: .5rem;
}
.tabs button {
  padding: .4rem 1rem;
  border: none;
  border-radius: .5rem;
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: .9rem;
}
.tabs button.active {
  background: var(--bg-elevated);
  color: var(--primary);
  font-weight: 600;
}
.loading, .empty {
  color: var(--text-muted);
  padding: 2rem 0;
}
.error {
  color: var(--error);
  font-size: .85rem;
  margin-bottom: .75rem;
}
.success {
  color: var(--success);
  font-size: .85rem;
  margin-bottom: .75rem;
}

/* Event list */
.event-list {
  display: flex;
  flex-direction: column;
  gap: .5rem;
}
.event-row {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: .75rem;
  cursor: pointer;
  transition: border-color .15s;
}
.event-row:hover {
  border-color: var(--primary);
}
.event-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: .15rem;
}
.ev-type {
  font-weight: 600;
  font-size: .95rem;
}
.ev-date, .ev-location {
  font-size: .8rem;
  color: var(--text-dim);
}
.event-meta {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: .25rem;
  font-size: .8rem;
}
.ev-people {
  color: var(--text-muted);
}
.ev-status {
  padding: .15rem .5rem;
  border-radius: 999px;
  font-size: .7rem;
  font-weight: 600;
}
.ev-status.status-available {
  background: #1a3a1a;
  color: var(--primary);
}
.ev-status.status-assigned {
  background: #1a2a3a;
  color: #60a5fa;
}
.ev-apps {
  color: var(--text-dim);
}
.arrow {
  color: var(--text-dim);
  font-size: 1.2rem;
}

/* Create form */
.create-form {
  background: var(--bg-surface);
  padding: 1.5rem;
  border-radius: .75rem;
  border: 1px solid var(--border);
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
input, select, textarea {
  width: 100%;
  padding: .6rem .75rem;
  margin-top: .25rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: var(--bg);
  color: var(--text);
  font-size: .9rem;
}
input:focus, select:focus, textarea:focus {
  outline: none;
  border-color: var(--primary);
}
textarea {
  resize: vertical;
}
select {
  cursor: pointer;
}
.create-btn {
  width: 100%;
  padding: .7rem;
  border: none;
  border-radius: .5rem;
  background: var(--gradient);
  color: #fff;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
}

/* Detail */
.back-btn {
  padding: .4rem 1rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: .85rem;
  margin-bottom: 1rem;
}
.back-btn:hover {
  border-color: var(--primary);
  color: var(--primary);
}
.detail-card {
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: .75rem;
  overflow: hidden;
  margin-bottom: 1.5rem;
}
.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: var(--bg-elevated);
  border-bottom: 1px solid var(--border);
}
.detail-header h3 {
  font-size: 1.1rem;
}
.detail-body {
  padding: 1rem;
}
.dl {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: .5rem 1rem;
  font-size: .85rem;
}
.dt {
  color: var(--text-dim);
  white-space: nowrap;
}
.dd {
  color: var(--text);
}

/* Applications */
.applications-section h3 {
  font-size: 1.1rem;
  color: var(--text-muted);
  margin-bottom: 1rem;
}
.app-grid {
  display: flex;
  flex-direction: column;
  gap: .75rem;
}
.app-card {
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: .75rem;
  padding: 1rem;
}
.app-card.selected {
  border-color: var(--primary);
}
.app-header {
  display: flex;
  align-items: center;
  gap: .75rem;
  margin-bottom: .75rem;
}
.app-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: var(--gradient);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1rem;
  color: #fff;
  flex-shrink: 0;
}
.app-name {
  font-weight: 600;
  font-size: .95rem;
}
.app-city {
  font-size: .8rem;
  color: var(--text-dim);
}
.app-rating {
  font-size: .85rem;
  color: #f59e0b;
  margin-bottom: .5rem;
}
.app-specialties {
  display: flex;
  flex-wrap: wrap;
  gap: .35rem;
  margin-bottom: .5rem;
}
.spec-chip {
  padding: .2rem .6rem;
  border-radius: 999px;
  background: var(--bg-elevated);
  color: var(--text-muted);
  font-size: .75rem;
}
.app-desc {
  font-size: .85rem;
  color: var(--text-muted);
  margin-bottom: .5rem;
  line-height: 1.4;
}
.app-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: .8rem;
  color: var(--text-dim);
  margin-bottom: .75rem;
}
.app-status {
  padding: .15rem .5rem;
  border-radius: 999px;
  font-size: .7rem;
  font-weight: 600;
  background: #3a2a1a;
  color: #f59e0b;
}
.app-status.accepted {
  background: #1a3a1a;
  color: var(--primary);
}
.select-btn {
  width: 100%;
  padding: .6rem;
  border: none;
  border-radius: .5rem;
  background: var(--gradient);
  color: #fff;
  font-size: .9rem;
  font-weight: 600;
  cursor: pointer;
}
.select-btn:disabled {
  opacity: .6;
  cursor: not-allowed;
}
.assigned-info {
  margin-top: 1rem;
}

/* Image upload */
.image-upload {
  margin-bottom: 1rem;
}
.image-upload .label {
  display: block;
  font-size: .85rem;
  color: var(--text-muted);
  margin-bottom: .5rem;
}
.previews {
  display: flex;
  gap: .5rem;
  flex-wrap: wrap;
  margin-bottom: .5rem;
}
.preview-item {
  position: relative;
  width: 100px;
  height: 100px;
  border-radius: .5rem;
  overflow: hidden;
  border: 1px solid var(--border-light);
}
.preview-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.remove-img {
  position: absolute;
  top: 2px;
  right: 2px;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  border: none;
  background: rgba(0,0,0,.7);
  color: #fff;
  font-size: 1rem;
  line-height: 1;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
}
.file-btn {
  display: inline-flex;
  padding: .5rem 1rem;
  border: 1px dashed var(--border-light);
  border-radius: .5rem;
  color: var(--text-muted);
  font-size: .85rem;
  cursor: pointer;
  transition: border-color .15s;
}
.file-btn:hover {
  border-color: var(--primary);
  color: var(--primary);
}
.file-btn input {
  display: none;
}

/* Event images gallery */
.event-images {
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid var(--border);
}
.event-images .label {
  display: block;
  font-size: .85rem;
  color: var(--text-muted);
  margin-bottom: .5rem;
}
.image-gallery {
  display: flex;
  gap: .5rem;
  flex-wrap: wrap;
}
.image-gallery img {
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: .5rem;
  border: 1px solid var(--border-light);
}
</style>
