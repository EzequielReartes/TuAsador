import { api } from './index'

export interface EventItem {
  id: string
  clientId: string
  clientName: string
  date: string
  time: string
  city: string
  address: string
  peopleCount: number
  eventType: string
  serviceDesired: string | null
  notes: string | null
  status: string
  createdAt: string
  applicationCount: number
  imageUrls: string[]
  applicationStatus?: string
}

export interface EventDetail {
  id: string
  clientId: string
  clientName: string
  date: string
  time: string
  city: string
  address: string
  peopleCount: number
  eventType: string
  serviceDesired: string | null
  notes: string | null
  status: string
  createdAt: string
  applications: EventApplication[]
  hasApplied: boolean
  imageUrls: string[]
  contractId: string | null
  contractStatus: string | null
}

export interface EventApplication {
  id: string
  asadorProfileId: string
  asadorName: string
  asadorPhotoUrl: string | null
  whatsApp: string | null
  mainCity: string
  averageRating: number
  status: string
  createdAt: string
  description: string | null
  specialtyNames: string[]
}

export interface CreateEventRequest {
  date: string
  time: string
  city: string
  address: string
  peopleCount: number
  eventType: string
  serviceDesired?: string
  notes?: string
}

export async function getAvailableEvents(): Promise<EventItem[]> {
  const { data } = await api.get<EventItem[]>('/events')
  return data
}

export async function getMyEvents(): Promise<EventItem[]> {
  const { data } = await api.get<EventItem[]>('/events/mine')
  return data
}

export async function getAppliedEvents(): Promise<EventItem[]> {
  const { data } = await api.get<EventItem[]>('/events/applied')
  return data
}

export async function getEventDetail(id: string): Promise<EventDetail> {
  const { data } = await api.get<EventDetail>(`/events/${id}`)
  return data
}

export async function createEvent(request: CreateEventRequest): Promise<{ id: string }> {
  const { data } = await api.post<{ id: string }>('/events', request)
  return data
}

export async function applyToEvent(eventId: string): Promise<void> {
  await api.post(`/events/${eventId}/apply`)
}

export async function getApplications(eventId: string): Promise<EventApplication[]> {
  const { data } = await api.get<EventApplication[]>(`/events/${eventId}/applications`)
  return data
}

export async function selectApplication(eventId: string, applicationId: string): Promise<void> {
  await api.put(`/events/${eventId}/applications/${applicationId}/select`)
}

export async function uploadEventImages(eventId: string, files: File[]): Promise<{ id: string; imageUrl: string }[]> {
  const formData = new FormData()
  files.forEach(f => formData.append('files', f))
  const { data } = await api.post<{ id: string; imageUrl: string }[]>(`/events/${eventId}/images`, formData)
  return data
}

export async function deleteEventImage(eventId: string, imageId: string): Promise<void> {
  await api.delete(`/events/${eventId}/images/${imageId}`)
}
