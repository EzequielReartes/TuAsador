<script setup lang="ts">
import { ref } from 'vue'
import LoginPage from './components/LoginPage.vue'
import UserList from './components/UserList.vue'
import AsadorProfile from './components/AsadorProfile.vue'
import AvailableEvents from './components/AvailableEvents.vue'
import AppliedEvents from './components/AppliedEvents.vue'
import ClientEvents from './components/ClientEvents.vue'
import ProfileDropdown from './components/ProfileDropdown.vue'
import authState, { isAuthenticated, clearAuth } from './store'

const loggedIn = ref(isAuthenticated())
const asadorView = ref<'profile' | 'events' | 'applied' | 'contracted'>('events')

function onLoggedIn() {
  loggedIn.value = true
}

function logout() {
  clearAuth()
  loggedIn.value = false
}
</script>

<template>
  <LoginPage v-if="!loggedIn" @loggedIn="onLoggedIn" />

  <div v-else class="app">
    <header>
      <div>
        <h1>TuAsador</h1>
        <p>Panel de administración</p>
      </div>
      <div class="user-info">
        <ProfileDropdown />
        <span>{{ authState.name }} ({{ authState.role }})</span>
        <button class="logout-btn" @click="logout">Salir</button>
      </div>
    </header>

    <main>
      <template v-if="authState.role === 'Asador'">
        <div class="tabs">
          <button :class="{ active: asadorView === 'events' }" @click="asadorView = 'events'">Eventos Disponibles</button>
          <button :class="{ active: asadorView === 'applied' }" @click="asadorView = 'applied'">Pendientes</button>
          <button :class="{ active: asadorView === 'contracted' }" @click="asadorView = 'contracted'">Contratados</button>
          <button :class="{ active: asadorView === 'profile' }" @click="asadorView = 'profile'">Mi Perfil</button>
        </div>
        <AvailableEvents v-if="asadorView === 'events'" />
        <AppliedEvents v-if="asadorView === 'applied'" filter="pending" />
        <AppliedEvents v-if="asadorView === 'contracted'" filter="accepted" />
        <AsadorProfile v-if="asadorView === 'profile'" />
      </template>
      <ClientEvents v-if="authState.role === 'Cliente'" />
      <UserList v-if="authState.role === 'Admin'" />
    </main>
  </div>
</template>

<style>
* { margin: 0; padding: 0; box-sizing: border-box; }
body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: var(--bg);
  color: var(--text);
}
.app {
  max-width: 960px;
  margin: 0 auto;
  padding: 2rem;
}
header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}
header h1 {
  font-size: 2rem;
  background: var(--gradient);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
header p {
  color: var(--text-muted);
  margin-top: .25rem;
}
.user-info {
  display: flex;
  align-items: center;
  gap: 1rem;
  color: var(--text-muted);
  font-size: .9rem;
}
.logout-btn {
  padding: .4rem 1rem;
  border: 1px solid var(--border-light);
  border-radius: .5rem;
  background: transparent;
  color: var(--text-muted);
  cursor: pointer;
  font-size: .85rem;
}
.logout-btn:hover {
  border-color: var(--error);
  color: var(--error);
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
</style>
