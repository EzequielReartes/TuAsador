<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAsadorPublicProfile, type AsadorPublicProfile } from '../api/asador'

const props = defineProps<{
  profileId: string
  applicationId?: string | null
  selecting?: boolean
}>()

const emit = defineEmits<{
  close: []
  select: [applicationId: string]
}>()

const profile = ref<AsadorPublicProfile | null>(null)
const loading = ref(true)
const error = ref('')
const fullscreenUrl = ref('')

onMounted(async () => {
  try {
    profile.value = await getAsadorPublicProfile(props.profileId)
  } catch {
    error.value = 'Error al cargar el perfil del asador'
  } finally {
    loading.value = false
  }
})

function formatWhatsApp(phone: string): string {
  const clean = phone.replace(/\D/g, '')
  return `https://wa.me/${clean}`
}

function formatInstagram(user: string): string {
  const handle = user.replace('@', '')
  return `https://instagram.com/${handle}`
}

function ratingColor(val: number): string {
  if (val >= 4) return '#46cf5d'
  if (val >= 3) return '#f59e0b'
  return '#ef4444'
}

function fireEmojis(val: number): ('full' | 'half')[] {
  const items: ('full' | 'half')[] = []
  const full = Math.floor(val)
  for (let i = 0; i < full; i++) items.push('full')
  if (val - full >= 0.25) items.push('half')
  return items
}
</script>

<template>
  <div class="modal-overlay" @click.self="emit('close')">
    <div class="modal-card">
      <button class="modal-close" @click="emit('close')">&times;</button>

      <div v-if="loading" class="loading">Cargando perfil...</div>

      <div v-else-if="error" class="error">{{ error }}</div>

      <template v-else-if="profile">
        <div class="modal-header">
          <div class="header-left">
            <div class="avatar">
              <img v-if="profile.photoUrl" :src="profile.photoUrl" alt="Foto" />
              <span v-else>{{ profile.name.charAt(0) }}</span>
            </div>
            <div>
              <div class="name">{{ profile.name }}</div>
              <div class="city">{{ profile.mainCity }}</div>
            </div>
          </div>
          <span class="status-badge" :class="profile.status.toLowerCase()">
            {{ profile.status === 'Verificado' ? 'Verificado' : 'Pendiente' }}
          </span>
        </div>

        <div class="contact-row" v-if="profile.whatsApp || profile.instagram">
          <a v-if="profile.whatsApp" :href="formatWhatsApp(profile.whatsApp)" target="_blank" class="contact-link">
            <span class="contact-icon">📱</span> {{ profile.whatsApp }}
          </a>
          <a v-if="profile.instagram" :href="formatInstagram(profile.instagram)" target="_blank" class="contact-link">
            <span class="contact-icon">📷</span> {{ profile.instagram }}
          </a>
        </div>

        <p class="description" v-if="profile.description">{{ profile.description }}</p>

        <div class="ratings-section">
          <div class="rating-main">
            <span class="rating-main-label">Valoración</span>
            <span class="rating-star">🔥</span>
            <span class="rating-value" :style="{ color: ratingColor(profile.averageRating) }">
              {{ profile.averageRating.toFixed(1) }}
            </span>
          </div>
          <div class="rating-breakdown">
            <div class="rating-item">
              <span class="rating-label">Puntualidad</span>
              <div class="rating-fire">
                <span v-for="(t, i) in fireEmojis(profile.punctualityRating)" :key="i" class="fire-emoji" :class="{ 'fire-half': t === 'half' }">🔥</span>
              </div>
            </div>
            <div class="rating-item">
              <span class="rating-label">Presencia</span>
              <div class="rating-fire">
                <span v-for="(t, i) in fireEmojis(profile.presenceRating)" :key="i" class="fire-emoji" :class="{ 'fire-half': t === 'half' }">🔥</span>
              </div>
            </div>
            <div class="rating-item">
              <span class="rating-label">Performance</span>
              <div class="rating-fire">
                <span v-for="(t, i) in fireEmojis(profile.performanceRating)" :key="i" class="fire-emoji" :class="{ 'fire-half': t === 'half' }">🔥</span>
              </div>
            </div>
          </div>
        </div>

        <div class="specialties-section" v-if="profile.specialtyNames.length">
          <span v-for="s in profile.specialtyNames" :key="s" class="spec-chip">{{ s }}</span>
        </div>

        <div class="portfolio-section" v-if="profile.portfolioImages.length">
          <h4>Portafolio</h4>
          <div class="portfolio-grid">
            <img
              v-for="img in profile.portfolioImages"
              :key="img.id"
              :src="img.imageUrl"
              alt="Portafolio"
              class="portfolio-img"
              @click="fullscreenUrl = img.imageUrl"
            />
          </div>
        </div>

        <div v-if="applicationId" class="select-section">
          <button
            class="select-asador-btn"
            :disabled="selecting"
            @click="emit('select', applicationId)"
          >
            {{ selecting ? 'Seleccionando...' : 'Seleccionar Asador' }}
          </button>
        </div>

        <div v-if="fullscreenUrl" class="fullscreen-overlay" @click.self="fullscreenUrl = ''">
          <button class="fullscreen-close" @click="fullscreenUrl = ''">&times;</button>
          <img :src="fullscreenUrl" class="fullscreen-img" />
        </div>
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
  z-index: 1100;
  padding: 1rem;
}
.modal-card {
  background: var(--bg-surface);
  border: 1px solid var(--border);
  border-radius: .75rem;
  width: 100%;
  max-width: 500px;
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
  z-index: 10;
}
.modal-close:hover {
  color: var(--text);
}

.loading, .error {
  padding: 2rem 1rem;
  text-align: center;
  color: var(--text-muted);
  font-size: .9rem;
}
.error {
  color: var(--error);
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.25rem 1rem .75rem;
  border-bottom: 1px solid var(--border);
}
.header-left {
  display: flex;
  align-items: center;
  gap: .75rem;
}
.avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  background: var(--gradient);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.2rem;
  color: #fff;
  flex-shrink: 0;
  overflow: hidden;
}
.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.name {
  font-weight: 600;
  font-size: 1.05rem;
}
.city {
  font-size: .8rem;
  color: var(--text-dim);
  margin-top: .15rem;
}
.status-badge {
  padding: .25rem .65rem;
  border-radius: 999px;
  font-size: .7rem;
  font-weight: 600;
  white-space: nowrap;
}
.status-badge.verificado {
  background: #1a3a1a;
  color: var(--primary);
}
.status-badge.pendiente {
  background: #3a2a1a;
  color: #f59e0b;
}

.contact-row {
  display: flex;
  gap: 1rem;
  padding: .75rem 1rem;
  border-bottom: 1px solid var(--border);
  flex-wrap: wrap;
}
.contact-link {
  display: inline-flex;
  align-items: center;
  gap: .35rem;
  color: var(--text-muted);
  font-size: .85rem;
  text-decoration: none;
  transition: color .15s;
}
.contact-link:hover {
  color: var(--primary);
}
.contact-icon {
  font-size: 1rem;
}

.description {
  padding: .75rem 1rem;
  font-size: .85rem;
  color: var(--text-muted);
  line-height: 1.5;
  border-bottom: 1px solid var(--border);
  white-space: pre-wrap;
}

.ratings-section {
  padding: .75rem 1rem;
  border-bottom: 1px solid var(--border);
}
.rating-main {
  display: flex;
  align-items: center;
  gap: .5rem;
  margin-bottom: .75rem;
}
.rating-main-label {
  color: var(--text-dim);
  font-size: .85rem;
  margin-right: auto;
}
.rating-star {
  font-size: 1.5rem;
}
.rating-value {
  font-size: 1.5rem;
  font-weight: 700;
}
.rating-breakdown {
  display: flex;
  flex-direction: column;
  gap: .5rem;
}
.rating-item {
  display: flex;
  align-items: flex-end;
  gap: .35rem;
  font-size: .85rem;
}
.rating-label {
  color: var(--text-dim);
  width: 75px;
  flex-shrink: 0;
  line-height: 1.2;
}
.rating-fire {
  display: flex;
  align-items: flex-end;
  gap: 0.1rem;
  flex: 1;
  line-height: 1;
}
.fire-emoji {
  font-size: 1.3rem;
  line-height: 1;
}
.fire-half {
  font-size: 0.7rem;
  line-height: 1;
}

.specialties-section {
  display: flex;
  flex-wrap: wrap;
  gap: .35rem;
  padding: .75rem 1rem;
  border-bottom: 1px solid var(--border);
}
.spec-chip {
  padding: .25rem .65rem;
  border-radius: 999px;
  background: var(--bg-elevated);
  color: var(--text-muted);
  font-size: .75rem;
}

.portfolio-section {
  padding: 1rem;
}
.portfolio-section h4 {
  font-size: .85rem;
  color: var(--text-dim);
  margin-bottom: .75rem;
  text-transform: uppercase;
  letter-spacing: .05em;
}
.portfolio-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(130px, 1fr));
  gap: .5rem;
}
.portfolio-img {
  width: 100%;
  height: 110px;
  object-fit: cover;
  border-radius: .5rem;
  cursor: pointer;
  border: 1px solid var(--border-light);
  transition: opacity .15s;
}
.portfolio-img:hover {
  opacity: .8;
}

.select-section {
  padding: 1rem;
  border-top: 1px solid var(--border);
}
.select-asador-btn {
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
.select-asador-btn:disabled {
  opacity: .6;
  cursor: not-allowed;
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
