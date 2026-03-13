import { Configuration } from '@/api/generated'

export function getApiConfiguration(): Configuration {
  return new Configuration({
    basePath: import.meta.env.VITE_BACKEND_URL,
    credentials: 'include',
  })
}
