import { api } from './index'

export interface LoginRequest {
  email: string
  password: string
}

export interface AuthResponse {
  token: string
  userId: string
  name: string
  email: string
  role: string
  profilePictureUrl?: string | null
}

export async function login(data: LoginRequest): Promise<AuthResponse> {
  const { data: res } = await api.post<AuthResponse>('/auth/login', data)
  return res
}

export async function register(data: {
  name: string
  email: string
  password: string
  whatsApp?: string
  role: string
}): Promise<AuthResponse> {
  const { data: res } = await api.post<AuthResponse>('/auth/register', data)
  return res
}
