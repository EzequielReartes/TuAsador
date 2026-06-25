<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { getUsers, type User } from '../api'
import { getAdminUsers, toggleUserActive, getPortfolioImages, approveImage, rejectImage, type AdminUser, type PortfolioImageItem } from '../api/admin'
import authState from '../store'

const isAdmin = computed(() => authState.role === 'Admin')

const adminTab = ref<'users' | 'portfolios'>('users')

const users = ref<(User | AdminUser)[]>([])
const loading = ref(true)
const error = ref('')

const images = ref<PortfolioImageItem[]>([])
const loadingImages = ref(true)

const roleColors: Record<string, string> = {
  Admin: 'var(--primary-dark)',
  Asador: 'var(--primary)',
  Cliente: 'var(--primary-light)'
}

async function fetchUsers() {
  loading.value = true
  error.value = ''
  try {
    if (isAdmin.value) {
      users.value = await getAdminUsers()
    } else {
      users.value = await getUsers()
    }
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al cargar usuarios'
  } finally {
    loading.value = false
  }
}

async function fetchImages() {
  loadingImages.value = true
  try {
    images.value = await getPortfolioImages(true)
  } catch {
    images.value = []
  } finally {
    loadingImages.value = false
  }
}

async function handleToggleActive(id: string) {
  try {
    await toggleUserActive(id)
    await fetchUsers()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al cambiar estado'
  }
}

async function handleApproveImage(id: string) {
  try {
    await approveImage(id)
    await fetchImages()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al aprobar imagen'
  }
}

async function handleRejectImage(id: string) {
  try {
    await rejectImage(id)
    await fetchImages()
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al rechazar imagen'
  }
}

onMounted(async () => {
  await fetchUsers()
  if (isAdmin.value) {
    await fetchImages()
  }
})
</script>

<template>
  <div class="user-list">
    <div v-if="isAdmin" class="admin-tabs">
      <button :class="{ active: adminTab === 'users' }" @click="adminTab = 'users'">Usuarios</button>
      <button :class="{ active: adminTab === 'portfolios' }" @click="adminTab = 'portfolios'">Portafolios</button>
      <span class="count-badge" v-if="images.length > 0">{{ images.length }} pendientes</span>
    </div>

    <p v-if="error" class="error">{{ error }}</p>

    <template v-if="adminTab === 'users'">
      <p v-if="loading" class="loading">Cargando usuarios...</p>
      <p v-else-if="users.length === 0" class="empty">
        No hay usuarios registrados.
      </p>
      <table v-else>
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Email</th>
            <th>Usuario</th>
            <th>Rol</th>
            <th>WhatsApp</th>
            <th>Registro</th>
            <th v-if="isAdmin">Activo</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td>{{ user.name }}</td>
            <td>{{ user.email }}</td>
            <td>{{ user.userName }}</td>
            <td>
              <span class="role-badge" :style="{ background: roleColors[user.role] || 'var(--text-dim)' }">
                {{ user.role }}
              </span>
            </td>
            <td>{{ user.whatsApp ?? '-' }}</td>
            <td>{{ new Date(user.createdAt).toLocaleDateString() }}</td>
            <td v-if="isAdmin">
              <label class="toggle">
                <input
                  type="checkbox"
                  :checked="(user as AdminUser).isActive"
                  @change="handleToggleActive(user.id)"
                />
                <span class="slider"></span>
              </label>
            </td>
          </tr>
        </tbody>
      </table>
    </template>

    <template v-if="adminTab === 'portfolios'">
      <p v-if="loadingImages" class="loading">Cargando imágenes...</p>
      <p v-else-if="images.length === 0" class="empty">
        No hay imágenes pendientes de aprobación.
      </p>
      <div v-else class="image-grid">
        <div v-for="img in images" :key="img.id" class="image-card">
          <img :src="img.imageUrl" :alt="'Portfolio de ' + img.asadorName" />
          <div class="image-info">
            <span class="asador-name">{{ img.asadorName }}</span>
            <span class="image-date">{{ new Date(img.createdAt).toLocaleDateString() }}</span>
          </div>
          <div class="image-actions" v-if="img.isApproved === null">
            <button class="btn-approve" @click="handleApproveImage(img.id)">Aprobar</button>
            <button class="btn-reject" @click="handleRejectImage(img.id)">Rechazar</button>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.user-list h2 {
  margin-bottom: 1rem;
  font-size: 1.25rem;
  color: var(--text-muted);
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
table {
  width: 100%;
  border-collapse: collapse;
}
th {
  text-align: left;
  padding: .75rem .5rem;
  font-size: .75rem;
  text-transform: uppercase;
  letter-spacing: .05em;
  color: var(--text-dim);
  border-bottom: 1px solid var(--border);
}
td {
  padding: .75rem .5rem;
  border-bottom: 1px solid var(--border);
  font-size: .9rem;
}
.role-badge {
  display: inline-block;
  padding: .2rem .6rem;
  border-radius: 999px;
  font-size: .75rem;
  font-weight: 600;
  color: #fff;
}
.admin-tabs {
  display: flex;
  gap: .5rem;
  align-items: center;
  margin-bottom: 1.5rem;
  border-bottom: 1px solid var(--border);
  padding-bottom: .5rem;
}
.admin-tabs button {
  padding: .4rem 1rem;
  border: none;
  border-radius: .5rem;
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: .9rem;
}
.admin-tabs button.active {
  background: var(--bg-elevated);
  color: var(--primary);
  font-weight: 600;
}
.count-badge {
  margin-left: auto;
  background: var(--primary-darker);
  color: #fff;
  padding: .2rem .7rem;
  border-radius: 999px;
  font-size: .75rem;
  font-weight: 600;
}
.toggle {
  position: relative;
  display: inline-block;
  width: 40px;
  height: 22px;
  cursor: pointer;
}
.toggle input {
  opacity: 0;
  width: 0;
  height: 0;
}
.slider {
  position: absolute;
  inset: 0;
  background: var(--border-light);
  border-radius: 999px;
  transition: .2s;
}
.slider::before {
  content: '';
  position: absolute;
  width: 16px;
  height: 16px;
  left: 3px;
  bottom: 3px;
  background: #fff;
  border-radius: 50%;
  transition: .2s;
}
.toggle input:checked + .slider {
  background: var(--primary);
}
.toggle input:checked + .slider::before {
  transform: translateX(18px);
}
.image-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
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
  height: 180px;
  object-fit: cover;
  display: block;
}
.image-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: .75rem;
}
.asador-name {
  font-weight: 600;
  font-size: .9rem;
}
.image-date {
  font-size: .8rem;
  color: var(--text-dim);
}
.image-actions {
  display: flex;
  gap: .5rem;
  padding: 0 .75rem .75rem;
}
.btn-approve, .btn-reject {
  flex: 1;
  padding: .5rem;
  border: none;
  border-radius: .5rem;
  font-size: .85rem;
  font-weight: 600;
  cursor: pointer;
  color: #fff;
}
.btn-approve {
  background: var(--primary-dark);
}
.btn-approve:hover {
  background: var(--primary);
}
.btn-reject {
  background: #991b1b;
}
.btn-reject:hover {
  background: var(--error);
}
</style>
