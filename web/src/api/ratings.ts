import { api } from './index'

export interface CreateRatingRequest {
  contractId: string
  punctualityScore: number
  presenceScore: number
  performanceScore: number
  comment?: string
}

export async function createRating(request: CreateRatingRequest): Promise<void> {
  await api.post('/ratings', request)
}
