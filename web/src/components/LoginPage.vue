<script setup lang="ts">
import { ref } from 'vue'
import { login } from '../api/auth'
import { setAuth } from '../store'

const emit = defineEmits<{ loggedIn: [] }>()

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

async function handleLogin() {
  error.value = ''
  loading.value = true
  try {
    const res = await login({ email: email.value, password: password.value })
    setAuth(res)
    emit('loggedIn')
  } catch (e: any) {
    error.value = e?.response?.data?.message || 'Error al iniciar sesión'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-wrapper">
    <div class="login-card">
      <h1>TuAsador</h1>
      <p class="sub">Iniciar sesión</p>
      <form @submit.prevent="handleLogin">
        <label>
          Email
          <input v-model="email" type="email" placeholder="admin@tuasador.com" required />
        </label>
        <label>
          Contraseña
          <input v-model="password" type="password" placeholder="TuAsador123!" required />
        </label>
        <p v-if="error" class="error">{{ error }}</p>
        <button type="submit" :disabled="loading">
          {{ loading ? 'Ingresando...' : 'Ingresar' }}
        </button>
      </form>
    </div>
  </div>
</template>

<style scoped>
.login-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
}
.login-card {
  background: var(--bg-surface);
  padding: 2.5rem;
  border-radius: 1rem;
  width: 100%;
  max-width: 400px;
}
.login-card h1 {
  font-size: 2rem;
  background: var(--gradient);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  text-align: center;
}
.sub {
  text-align: center;
  color: var(--text-muted);
  margin: .5rem 0 1.5rem;
}
label {
  display: block;
  margin-bottom: 1rem;
  font-size: .85rem;
  color: var(--text-muted);
}
input {
  width: 100%;
  padding: .6rem .75rem;
  margin-top: .25rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: var(--bg);
  color: var(--text);
  font-size: .9rem;
}
input:focus {
  outline: none;
  border-color: var(--primary);
}
.error {
  color: var(--error);
  font-size: .85rem;
  margin-bottom: .75rem;
}
button {
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
button:disabled {
  opacity: .6;
  cursor: not-allowed;
}
</style>
