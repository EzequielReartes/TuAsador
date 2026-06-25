<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getAppliedEvents, getEventDetail, type EventItem, type EventDetail } from '../api/events'
import EventDetailModal from './EventDetailModal.vue'

const props = withDefaults(defineProps<{ filter?: 'all' | 'pending' | 'accepted' }>(), { filter: 'all' })

const events = ref<EventItem[]>([])
const filteredEvents = computed(() => {
  if (props.filter === 'pending') return events.value.filter(e => e.applicationStatus === 'Pendiente')
  if (props.filter === 'accepted') return events.value.filter(e => e.applicationStatus === 'Aceptada')
  return events.value
})
const loading = ref(true)
const error = ref('')
const showModal = ref(false)
const selectedEvent = ref<EventDetail | null>(null)
const detailLoading = ref(false)

onMounted(async () => {
  try {
    events.value = await getAppliedEvents()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al cargar eventos postulados'
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

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString('es-AR', {
    day: 'numeric', month: 'long', year: 'numeric'
  })
}

function formatTime(timeStr: string) {
  const [h, m] = timeStr.split(':')
  return `${h}:${m} hs`
}

function statusClass(status: string) {
  if (status === 'Pendiente') return 'status-pending'
  if (status === 'Aceptada') return 'status-accepted'
  if (status === 'Rechazada') return 'status-rejected'
  return ''
}

function statusLabel(status: string) {
  if (status === 'Pendiente') return 'Pendiente'
  if (status === 'Aceptada') return 'Aceptada'
  if (status === 'Rechazada') return 'Rechazada'
  return status
}
</script>

<template>
  <div class="applied-events">
    <h2>{{ filter === 'accepted' ? 'Eventos Contratados' : filter === 'pending' ? 'Eventos Pendientes' : 'Eventos Postulados' }}</h2>

    <p v-if="loading" class="loading">Cargando eventos...</p>
    <p v-if="error" class="error">{{ error }}</p>

    <p v-if="!loading && filteredEvents.length === 0" class="empty">
      {{ filter === 'accepted' ? 'No tenés eventos contratados todavía.' : 'No te postulaste a ningún evento todavía.' }}
    </p>

    <div v-else class="event-grid">
      <div v-for="ev in filteredEvents" :key="ev.id" class="event-card">
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
          <div class="info-row">
            <span class="label">Estado</span>
            <span class="app-status" :class="statusClass(ev.applicationStatus || '')">{{ statusLabel(ev.applicationStatus || '') }}</span>
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
      :applying="false"
      @close="closeDetail"
    />
  </div>
</template>

<style scoped>
.applied-events {
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
.app-status {
  padding: .15rem .5rem;
  border-radius: 999px;
  font-size: .7rem;
  font-weight: 600;
}
.app-status.status-pending {
  background: #3a2a1a;
  color: #f59e0b;
}
.app-status.status-accepted {
  background: #1a3a1a;
  color: var(--primary);
}
.app-status.status-rejected {
  background: #3a1a1a;
  color: #ef4444;
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
