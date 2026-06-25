<script setup lang="ts">
import { ref } from 'vue'
import authState, { setProfilePictureUrl } from '../store'
import { uploadProfilePicture, deleteProfilePicture } from '../api/profilePicture'

const open = ref(false)
const uploading = ref(false)
const viewing = ref(false)
const fileInput = ref<HTMLInputElement>()

function toggle() {
  open.value = !open.value
}

function close() {
  open.value = false
}

function selectFile() {
  fileInput.value?.click()
}

async function handleFileChange(e: Event) {
  const input = e.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return
  uploading.value = true
  try {
    const url = await uploadProfilePicture(file)
    setProfilePictureUrl(url)
    open.value = false
  } catch (err) {
    console.error('Error al subir foto', err)
  } finally {
    uploading.value = false
    input.value = ''
  }
}

async function removePicture() {
  try {
    await deleteProfilePicture()
    setProfilePictureUrl(null)
    open.value = false
  } catch (err) {
    console.error('Error al eliminar foto', err)
  }
}

function viewPicture() {
  viewing.value = true
  open.value = false
}

const defaultAvatar = '/default-avatar.svg'
const pictureUrl = () => authState.profilePictureUrl || defaultAvatar
</script>

<template>
  <div class="profile-dropdown" @click.stop>
    <button class="avatar-btn" @click="toggle" :title="authState.name ?? ''">
      <img :src="pictureUrl()" alt="Avatar" class="avatar-img" />
    </button>

    <div v-if="open" class="dropdown-menu" @click.stop>
      <button class="dropdown-item" @click="viewPicture">
        Ver foto de perfil
      </button>
      <button class="dropdown-item" @click="selectFile">
        {{ authState.profilePictureUrl ? 'Editar foto de perfil' : 'Cargar foto de perfil' }}
      </button>
      <button v-if="authState.profilePictureUrl" class="dropdown-item danger" @click="removePicture">
        Eliminar foto
      </button>
    </div>

    <div v-if="open" class="dropdown-overlay" @click="close"></div>

    <input
      ref="fileInput"
      type="file"
      accept="image/jpeg,image/png,image/webp"
      class="file-input"
      @change="handleFileChange"
    />

    <Teleport to="body">
      <div v-if="viewing" class="modal-overlay" @click.self="viewing = false">
        <div class="modal-content">
          <img :src="pictureUrl()" alt="Foto de perfil" class="modal-img" />
          <button class="modal-close" @click="viewing = false">Cerrar</button>
        </div>
      </div>
    </Teleport>

    <div v-if="uploading" class="uploading-toast">Subiendo foto...</div>
  </div>
</template>

<style scoped>
.profile-dropdown {
  position: relative;
  display: flex;
  align-items: center;
}
.avatar-btn {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  border: 2px solid var(--border-light);
  padding: 0;
  cursor: pointer;
  overflow: hidden;
  background: transparent;
  transition: border-color .15s;
}
.avatar-btn:hover {
  border-color: var(--primary);
}
.avatar-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}
.dropdown-overlay {
  position: fixed;
  inset: 0;
  z-index: 9;
}
.dropdown-menu {
  position: absolute;
  top: calc(100% + 6px);
  right: 0;
  background: var(--bg-elevated);
  border: 1px solid var(--border);
  border-radius: .5rem;
  min-width: 180px;
  z-index: 10;
  overflow: hidden;
  box-shadow: 0 4px 16px rgba(0,0,0,.4);
}
.dropdown-item {
  display: block;
  width: 100%;
  padding: .6rem 1rem;
  border: none;
  background: transparent;
  color: var(--text-muted);
  font-size: .85rem;
  text-align: left;
  cursor: pointer;
  transition: background .1s;
}
.dropdown-item:hover {
  background: var(--bg-surface);
  color: var(--text);
}
.dropdown-item.danger:hover {
  color: var(--error);
}
.file-input {
  display: none;
}
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;
}
.modal-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}
.modal-img {
  max-width: 90vw;
  max-height: 80vh;
  border-radius: .75rem;
  object-fit: contain;
}
.modal-close {
  padding: .5rem 1.5rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: var(--bg-elevated);
  color: var(--text-muted);
  cursor: pointer;
  font-size: .9rem;
}
.modal-close:hover {
  border-color: var(--primary);
  color: var(--primary);
}
.uploading-toast {
  position: fixed;
  bottom: 2rem;
  left: 50%;
  transform: translateX(-50%);
  background: var(--bg-elevated);
  border: 1px solid var(--border);
  padding: .6rem 1.5rem;
  border-radius: .5rem;
  color: var(--text-muted);
  font-size: .85rem;
  z-index: 200;
}
</style>
