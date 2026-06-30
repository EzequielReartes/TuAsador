<script setup lang="ts">
import { ref } from 'vue'
import { createRating, type CreateRatingRequest } from '../api/ratings'

const props = defineProps<{
  contractId: string
  asadorName: string
}>()

const emit = defineEmits<{
  close: []
  submitted: []
}>()

const punctuality = ref(0)
const presence = ref(0)
const performance = ref(0)
const comment = ref('')
const submitting = ref(false)
const error = ref('')
const success = ref('')

const canSubmit = ref(false)

function toggleStar(category: 'punctuality' | 'presence' | 'performance', val: number) {
  if (category === 'punctuality') punctuality.value = val
  else if (category === 'presence') presence.value = val
  else performance.value = val

  canSubmit.value = punctuality.value > 0 && presence.value > 0 && performance.value > 0
}

async function handleSubmit() {
  if (!canSubmit.value || submitting.value) return
  submitting.value = true
  error.value = ''
  try {
    await createRating({
      contractId: props.contractId,
      punctualityScore: punctuality.value,
      presenceScore: presence.value,
      performanceScore: performance.value,
      comment: comment.value || undefined
    })
    success.value = '¡Valoración enviada! Gracias por tu opinión.'
    setTimeout(() => emit('submitted'), 1500)
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al enviar valoración'
  } finally {
    submitting.value = false
  }
}

function handleSkip() {
  emit('submitted')
}

function starLabel(val: number): string {
  if (val === 1) return 'Muy malo'
  if (val === 2) return 'Malo'
  if (val === 3) return 'Regular'
  if (val === 4) return 'Bueno'
  if (val === 5) return 'Excelente'
  return ''
}
</script>

<template>
  <div class="modal-overlay" @click.self="emit('close')">
    <div class="modal-card">
      <button class="modal-close" @click="emit('close')">&times;</button>

      <div class="modal-header">
        <h3>¡El evento terminó!</h3>
        <p class="subtitle">Valorá a <strong>{{ asadorName }}</strong> por su servicio</p>
      </div>

      <p v-if="error" class="error">{{ error }}</p>
      <p v-if="success" class="success">{{ success }}</p>

      <template v-if="!success">
        <div class="rating-row">
          <span class="rating-label">Puntualidad</span>
          <div class="stars">
            <button
              v-for="i in 5" :key="i"
              class="star-btn"
              :class="{ filled: i <= punctuality }"
              @click="toggleStar('punctuality', i)"
            >★</button>
          </div>
          <span class="star-desc">{{ starLabel(punctuality) }}</span>
        </div>

        <div class="rating-row">
          <span class="rating-label">Presencia</span>
          <div class="stars">
            <button
              v-for="i in 5" :key="i"
              class="star-btn"
              :class="{ filled: i <= presence }"
              @click="toggleStar('presence', i)"
            >★</button>
          </div>
          <span class="star-desc">{{ starLabel(presence) }}</span>
        </div>

        <div class="rating-row">
          <span class="rating-label">Performance</span>
          <div class="stars">
            <button
              v-for="i in 5" :key="i"
              class="star-btn"
              :class="{ filled: i <= performance }"
              @click="toggleStar('performance', i)"
            >★</button>
          </div>
          <span class="star-desc">{{ starLabel(performance) }}</span>
        </div>

        <label class="comment-label">
          Comentario adicional (opcional)
          <textarea v-model="comment" rows="3" placeholder="Contanos cómo fue tu experiencia..."></textarea>
        </label>

        <div class="actions">
          <button class="skip-btn" @click="handleSkip">Omitir</button>
          <button
            class="submit-btn"
            :disabled="!canSubmit || submitting"
            @click="handleSubmit"
          >
            {{ submitting ? 'Enviando...' : 'Enviar valoración' }}
          </button>
        </div>

        <p class="incentive">Tu opinión ayuda a la comunidad a elegir al mejor asador 🔥</p>
      </template>
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
  z-index: 1200;
  padding: 1rem;
}
.modal-card {
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: .75rem;
  width: 100%;
  max-width: 440px;
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
  padding: 1.5rem;
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
  z-index: 10;
}
.modal-close:hover {
  color: var(--text);
}
.modal-header {
  text-align: center;
  margin-bottom: 1.5rem;
}
.modal-header h3 {
  font-size: 1.2rem;
  margin-bottom: .35rem;
}
.modal-header .subtitle {
  font-size: .85rem;
  color: var(--text-muted);
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
  text-align: center;
  padding: 1rem;
}
.rating-row {
  display: flex;
  align-items: center;
  gap: .75rem;
  margin-bottom: 1rem;
  padding: .5rem 0;
  border-bottom: 1px solid var(--border);
}
.rating-label {
  font-size: .85rem;
  color: var(--text-muted);
  width: 90px;
  flex-shrink: 0;
}
.stars {
  display: flex;
  gap: .25rem;
}
.star-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: var(--text-dim);
  cursor: pointer;
  padding: 0;
  line-height: 1;
  transition: color .15s, transform .1s;
}
.star-btn:hover {
  transform: scale(1.2);
}
.star-btn.filled {
  color: #f59e0b;
}
.star-desc {
  font-size: .75rem;
  color: var(--text-dim);
  min-width: 70px;
}
.comment-label {
  display: block;
  font-size: .85rem;
  color: var(--text-muted);
  margin-bottom: 1rem;
}
.comment-label textarea {
  width: 100%;
  padding: .6rem .75rem;
  margin-top: .35rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: var(--bg);
  color: var(--text);
  font-size: .9rem;
  resize: vertical;
  font-family: inherit;
}
.comment-label textarea:focus {
  outline: none;
  border-color: var(--primary);
}
.actions {
  display: flex;
  gap: .75rem;
  margin-bottom: 1rem;
}
.skip-btn {
  flex: 1;
  padding: .6rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: transparent;
  color: var(--text-muted);
  font-size: .9rem;
  cursor: pointer;
}
.skip-btn:hover {
  border-color: var(--text-dim);
  color: var(--text);
}
.submit-btn {
  flex: 2;
  padding: .6rem;
  border: none;
  border-radius: .5rem;
  background: var(--gradient);
  color: #fff;
  font-size: .9rem;
  font-weight: 600;
  cursor: pointer;
}
.submit-btn:disabled {
  opacity: .6;
  cursor: not-allowed;
}
.incentive {
  text-align: center;
  font-size: .8rem;
  color: var(--text-dim);
}
</style>
