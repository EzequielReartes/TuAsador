import { reactive } from 'vue'

interface AuthState {
  token: string | null
  userId: string | null
  name: string | null
  email: string | null
  role: string | null
  profilePictureUrl: string | null
}

const state = reactive<AuthState>({
  token: localStorage.getItem('token'),
  userId: localStorage.getItem('userId'),
  name: localStorage.getItem('name'),
  email: localStorage.getItem('email'),
  role: localStorage.getItem('role'),
  profilePictureUrl: localStorage.getItem('profilePictureUrl'),
})

export function setAuth(data: {
  token: string
  userId: string
  name: string
  email: string
  role: string
  profilePictureUrl?: string | null
}) {
  state.token = data.token
  state.userId = data.userId
  state.name = data.name
  state.email = data.email
  state.role = data.role
  state.profilePictureUrl = data.profilePictureUrl ?? null
  localStorage.setItem('token', data.token)
  localStorage.setItem('userId', data.userId)
  localStorage.setItem('name', data.name)
  localStorage.setItem('email', data.email)
  localStorage.setItem('role', data.role)
  if (data.profilePictureUrl != null) {
    localStorage.setItem('profilePictureUrl', data.profilePictureUrl)
  } else {
    localStorage.removeItem('profilePictureUrl')
  }
}

export function setProfilePictureUrl(url: string | null) {
  state.profilePictureUrl = url
  if (url) {
    localStorage.setItem('profilePictureUrl', url)
  } else {
    localStorage.removeItem('profilePictureUrl')
  }
}

export function clearAuth() {
  state.token = null
  state.userId = null
  state.name = null
  state.email = null
  state.role = null
  localStorage.clear()
}

export function isAuthenticated(): boolean {
  return !!state.token
}

export default state
