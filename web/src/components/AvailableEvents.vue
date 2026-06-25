<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAvailableEvents, getEventDetail, applyToEvent, type EventItem, type EventDetail } from '../api/events'
import EventDetailModal from './EventDetailModal.vue'

const events = ref<EventItem[]>([])
const loading = ref(true)
const error = ref('')
const applyingId = ref<string | null>(null)
const success = ref('')
const showModal = ref(false)
const selectedEvent = ref<EventDetail | null>(null)
const detailLoading = ref(false)

onMounted(async () => {
  try {
    events.value = await getAvailableEvents()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al cargar eventos'
  } finally {
    loading.value = false
  }
})

async function openDetail(id: string) {
  detailLoading.value = true
  error.value = ''
  try {
    selectedEvent.value = await getEventDetail(id)
    showModal.value = true
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al cargar detalle del evento'
  } finally {
    detailLoading.value = false
  }
}

function closeDetail() {
  showModal.value = false
  selectedEvent.value = null
}

async function handleApply(eventId: string) {
  applyingId.value = eventId
  error.value = ''
  success.value = ''
  try {
    await applyToEvent(eventId)
    events.value = events.value.filter(e => e.id !== eventId)
    success.value = 'Te postulaste correctamente al evento'
    closeDetail()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al postularte'
  } finally {
    applyingId.value = null
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
</script>

<template>
  <div class="available-events">
    <h2>Eventos Disponibles</h2>

    <p v-if="loading" class="loading">Cargando eventos...</p>
    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="success" class="success">{{ success }}</p>

    <p v-if="!loading && events.length === 0" class="empty">
      No hay eventos disponibles por el momento.
    </p>

    <div v-else class="event-grid">
      <div v-for="ev in events" :key="ev.id" class="event-card">
        <div class="event-header">
          <span class="event-type">{{ ev.eventType }}</span>
          <span class="people-count">{{ ev.peopleCount }} pers.</span>
        </div>

        <div class="event-body">
          <div class="info-row">
            <span class="label">Fecha</span>
            <span>{{ formatDate(ev.date) }}</span>
          </div>
          <div class="info-row">
            <span class="label">Horario</span>
            <span>{{ formatTime(ev.time) }}</span>
          </div>
          <div class="info-row">
            <span class="label">Ubicación</span>
            <span>{{ ev.city }}</span>
          </div>
          <div class="info-row" v-if="ev.serviceDesired">
            <span class="label">Servicio</span>
            <span>{{ ev.serviceDesired }}</span>
          </div>
        </div>

        <button
          class="view-btn"
          :disabled="detailLoading"
          @click="openDetail(ev.id)"
        >
          Ver Evento
        </button>
      </div>
    </div>

    <EventDetailModal
      v-if="showModal && selectedEvent"
      :event="selectedEvent"
      :applying="applyingId === selectedEvent.id"
      @close="closeDetail"
      @apply="handleApply"
    />
  </div>
</template>

<style scoped>
.available-events {
  margin-top: 2rem;
}
h2 {
  margin-bottom: 1.25rem;
  color: var(--text-muted);
  font-size: 1.25rem;
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
.event-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: 1rem;
}
.event-card {
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: .75rem;
  overflow: hidden;
}
.event-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: .75rem 1rem;
  background: var(--bg-elevated);
  border-bottom: 1px solid var(--border);
}
.event-type {
  font-weight: 600;
  font-size: .95rem;
}
.people-count {
  font-size: .8rem;
  color: var(--text-dim);
  background: var(--bg);
  padding: .2rem .6rem;
  border-radius: 999px;
}
.event-body {
  padding: .75rem 1rem;
}
.info-row {
  display: flex;
  justify-content: space-between;
  padding: .35rem 0;
  font-size: .85rem;
  border-bottom: 1px solid var(--border);
}
.info-row:last-child {
  border-bottom: none;
}
.info-row .label {
  color: var(--text-dim);
}
.view-btn {
  display: block;
  width: calc(100% - 2rem);
  margin: 0 1rem 1rem;
  padding: .6rem;
  border: none;
  border-radius: .5rem;
  background: var(--gradient);
  color: #fff;
  font-size: .9rem;
  font-weight: 600;
  cursor: pointer;
  transition: opacity .15s;
}
.view-btn:disabled {
  opacity: .6;
  cursor: not-allowed;
}
</style>
