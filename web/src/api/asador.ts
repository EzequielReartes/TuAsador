import { api } from './index'

export interface AsadorProfile {
  id: string
  name: string
  email: string
  whatsApp: string | null
  description: string | null
  instagram: string | null
  photoUrl: string | null
  mainCity: string
  status: string
  specialtyIds: string[]
  specialtyNames: string[]
}

export interface UpdateAsadorProfile {
  description?: string
  instagram?: string
  photoUrl?: string
  mainCity: string
  whatsApp?: string
  specialtyIds: string[]
}

export interface Specialty {
  id: string
  name: string
}

export interface PortfolioImage {
  id: string
  imageUrl: string
  isApproved: boolean | null
  createdAt: string
}

export async function getMyProfile(): Promise<AsadorProfile> {
  const { data } = await api.get<AsadorProfile>('/asador/profile')
  return data
}

export async function updateMyProfile(data: UpdateAsadorProfile): Promise<void> {
  await api.put('/asador/profile', data)
}

export async function getSpecialties(): Promise<Specialty[]> {
  const { data } = await api.get<Specialty[]>('/specialties')
  return data
}

export async function getMyPortfolioImages(): Promise<PortfolioImage[]> {
  const { data } = await api.get<PortfolioImage[]>('/asador/portfolio')
  return data
}

export async function uploadPortfolioImages(files: File[]): Promise<void> {
  const formData = new FormData()
  for (const file of files) {
    formData.append('files', file)
  }
  await api.post('/asador/portfolio', formData)
}

export async function deletePortfolioImage(id: string): Promise<void> {
  await api.delete(`/asador/portfolio/${id}`)
}
