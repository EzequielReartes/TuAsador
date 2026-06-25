<script setup lang="ts">
import { ref } from 'vue'
import type { EventDetail } from '../api/events'

defineProps<{
  event: EventDetail
  applying: boolean
}>()

const emit = defineEmits<{
  close: []
  apply: [eventId: string]
}>()

const fullscreenUrl = ref('')

function openFullscreen(url: string) {
  fullscreenUrl.value = url
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
  <div class="modal-overlay" @click.self="emit('close')">
    <div class="modal-card">
      <button class="modal-close" @click="emit('close')">&times;</button>

      <div class="modal-header">
        <span class="event-type">{{ event.eventType }}</span>
        <span class="people-count">{{ event.peopleCount }} pers.</span>
      </div>

      <div class="modal-images" v-if="event.imageUrls.length">
        <span class="images-label">Imágenes de referencia</span>
        <div class="images-grid">
          <img
            v-for="(url, i) in event.imageUrls"
            :key="i"
            :src="url"
            alt="Referencia"
            class="modal-img"
            @click="openFullscreen(url)"
          />
        </div>
      </div>

      <!-- Fullscreen viewer -->
      <div v-if="fullscreenUrl" class="fullscreen-overlay" @click.self="fullscreenUrl = ''">
        <button class="fullscreen-close" @click="fullscreenUrl = ''">&times;</button>
        <img :src="fullscreenUrl" class="fullscreen-img" />
      </div>

      <div class="modal-body">
        <div class="info-row">
          <span class="label">Fecha</span>
          <span>{{ formatDate(event.date) }}</span>
        </div>
        <div class="info-row">
          <span class="label">Horario</span>
          <span>{{ formatTime(event.time) }}</span>
        </div>
        <div class="info-row">
          <span class="label">Ubicación</span>
          <span>{{ event.city }}, {{ event.address }}</span>
        </div>
        <div class="info-row" v-if="event.serviceDesired">
          <span class="label">Servicio</span>
          <span>{{ event.serviceDesired }}</span>
        </div>
        <div class="info-row" v-if="event.notes">
          <span class="label">Notas</span>
          <span>{{ event.notes }}</span>
        </div>
        <div class="info-row">
          <span class="label">Cliente</span>
          <span>{{ event.clientName }}</span>
        </div>
      </div>

      <button
        class="apply-btn"
        :disabled="applying"
        @click="emit('apply', event.id)"
      >
        {{ applying ? 'Postulando...' : 'Postularme' }}
      </button>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}
.modal-card {
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: .75rem;
  width: 100%;
  max-width: 480px;
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
}
.modal-close {
  position: absolute;
  top: .5rem;
  right: .75rem;
  background: none;
  border: none;
  color: var(--text-dim);
  font-size: 1.5rem;
  cursor: pointer;
  line-height: 1;
  padding: .25rem;
}
.modal-close:hover {
  color: var(--text);
}
.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1rem .75rem;
  border-bottom: 1px solid var(--border);
}
.event-type {
  font-weight: 600;
  font-size: 1rem;
}
.people-count {
  font-size: .8rem;
  color: var(--text-dim);
  background: var(--bg);
  padding: .2rem .6rem;
  border-radius: 999px;
}
.modal-body {
  padding: .75rem 1rem;
}
.info-row {
  display: flex;
  justify-content: space-between;
  padding: .45rem 0;
  font-size: .85rem;
  border-bottom: 1px solid var(--border);
}
.info-row:last-child {
  border-bottom: none;
}
.info-row .label {
  color: var(--text-dim);
}
.apply-btn {
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
.apply-btn:disabled {
  opacity: .6;
  cursor: not-allowed;
}

/* Images */
.modal-images {
  padding: .75rem 1rem;
  border-bottom: 1px solid var(--border);
  text-align: center;
}
.images-label {
  display: block;
  font-size: .75rem;
  color: var(--text-dim);
  margin-bottom: .5rem;
  text-transform: uppercase;
  letter-spacing: .05em;
}
.images-grid {
  display: flex;
  justify-content: center;
  gap: .5rem;
  flex-wrap: wrap;
}
.modal-img {
  width: 130px;
  height: 100px;
  object-fit: cover;
  border-radius: .5rem;
  cursor: pointer;
  border: 1px solid var(--border-light);
  transition: opacity .15s;
}
.modal-img:hover {
  opacity: .8;
}
.fullscreen-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 2rem;
}
.fullscreen-close {
  position: absolute;
  top: 1rem;
  right: 1.5rem;
  background: none;
  border: none;
  color: #fff;
  font-size: 2.5rem;
  cursor: pointer;
  line-height: 1;
}
.fullscreen-img {
  max-width: 100%;
  max-height: 90vh;
  object-fit: contain;
  border-radius: .5rem;
}
</style>
