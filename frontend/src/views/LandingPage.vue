<template>
  <div class="flex flex-col gap-10 items-center h-full self-center">
    <div class="m-5 flex flex-col gap-5 items-center">
      <h1 class="text-2xl">Willkommen bei</h1>

      <h1 class="text-3xl font-bold">Track Your Things</h1>
    </div>
    <UCard>
      <div class="flex flex-col gap-5 items-center">
        <p>Passkey-Anmeldung</p>
        <div class="flex flex-col gap-5 items-center">
          <UInput
            label="Nutzername"
            v-model="userName"
            placeholder="Nutzername"
            class="w-full max-w-sm"
          />
          <div class="flex gap-5 justify-center items-start w-full">
            <UButton label="Registrieren" variant="subtle" color="neutral" @click="register" />
            <UButton label="Anmelden" icon="i-lucide-arrow-right" @click="login" />
          </div>
        </div>
      </div>
    </UCard>
  </div>
</template>

<script setup lang="ts">
import {
  PasskeyApi,
  type AuthenticatorAssertionRawResponse,
  type AuthenticatorAttestationRawResponse,
} from '@/api/generated'
import { getApiConfiguration } from '@/api/utils'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
const router = useRouter()
import { startAuthentication, startRegistration } from '@simplewebauthn/browser'

const userName = ref('')

const register = async () => {
  const apiConfig = getApiConfiguration()
  const passkeyApi = new PasskeyApi(apiConfig)
  const response = await passkeyApi.apiAuthPasskeyRegisterBeginPost({
    registerBeginRequest: {
      username: userName.value,
    },
  })

  const optionsJSON = JSON.parse(JSON.stringify(response, (_, v) => (v === null ? undefined : v)))
  const credential = await startRegistration({ optionsJSON })

  passkeyApi.apiAuthPasskeyRegisterCompletePost({
    username: userName.value,
    authenticatorAttestationRawResponse:
      credential as unknown as AuthenticatorAttestationRawResponse,
  })

  router.push('/home')
}

const login = async () => {
  const apiConfig = getApiConfiguration()
  const passkeyApi = new PasskeyApi(apiConfig)
  const response = await passkeyApi.apiAuthPasskeyLoginBeginPost({
    loginBeginRequest: {
      username: userName.value,
    },
  })
  const optionsJSON = JSON.parse(JSON.stringify(response, (_, v) => (v === null ? undefined : v)))
  const credential = await startAuthentication({ optionsJSON })

  await passkeyApi.apiAuthPasskeyLoginCompletePost({
    authenticatorAssertionRawResponse: credential as unknown as AuthenticatorAssertionRawResponse,
  })
  router.push('/home')
}
</script>

<style scoped></style>
