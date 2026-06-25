import { api } from './index'

export interface AdminUser {
  id: string
  name: string
  email: string
  userName: string
  role: string
  phoneNumber: string | null
  whatsApp: string | null
  isActive: boolean
  createdAt: string
}

export interface PortfolioImageItem {
  id: string
  asadorProfileId: string
  asadorName: string
  imageUrl: string
  isApproved: boolean | null
  createdAt: string
}

export async function getAdminUsers(): Promise<AdminUser[]> {
  const { data } = await api.get<AdminUser[]>('/admin/users')
  return data
}

export async function toggleUserActive(id: string): Promise<void> {
  await api.put(`/admin/users/${id}/toggle-active`)
}

export async function getPortfolioImages(pendingOnly = true): Promise<PortfolioImageItem[]> {
  const { data } = await api.get<PortfolioImageItem[]>('/admin/portfolio-images', {
    params: { pendingOnly }
  })
  return data
}

export async function approveImage(id: string): Promise<void> {
  await api.put(`/admin/portfolio-images/${id}/approve`)
}

export async function rejectImage(id: string): Promise<void> {
  await api.put(`/admin/portfolio-images/${id}/reject`)
}
