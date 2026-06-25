import { api } from './index'

export async function uploadProfilePicture(file: File): Promise<string> {
  const formData = new FormData()
  formData.append('file', file)
  const { data } = await api.post<{ profilePictureUrl: string }>('/profile-picture', formData)
  return data.profilePictureUrl
}

export async function deleteProfilePicture(): Promise<void> {
  await api.delete('/profile-picture')
}
