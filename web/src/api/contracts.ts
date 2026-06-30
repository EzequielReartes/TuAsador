import { api } from './index'

export interface ContractDto {
  id: string
  status: string
  createdAt: string

  eventId: string
  eventDate: string
  eventTime: string
  eventCity: string
  eventAddress: string
  eventPeopleCount: number
  eventType: string
  eventServiceDesired: string | null
  eventNotes: string | null
  eventImageUrls: string[]

  asadorProfileId: string
  asadorName: string
  asadorPhotoUrl: string | null
  asadorWhatsApp: string | null
  asadorMainCity: string
  asadorAverageRating: number
  asadorSpecialtyNames: string[]
}

export async function getMyContracts(): Promise<ContractDto[]> {
  const { data } = await api.get<ContractDto[]>('/contracts/mine')
  return data
}

export async function cancelContract(id: string): Promise<void> {
  await api.put(`/contracts/${id}/cancel`)
}

export async function finishContract(id: string): Promise<void> {
  await api.put(`/contracts/${id}/finish`)
}
