<template>
  <div class="m-5 flex flex-col gap-5 items-center">
    <h1 class="text-2xl">Willkommen bei</h1>

    <h1 class="text-3xl font-bold">Track Your Things</h1>
    <div class="flex flex-col gap-5 items-center">
      <p>Mithilfe des Codes können Deine Daten abgespeichert werden.</p>
      <p>
        Wenn Du noch keinen Code hast, generiere einen neuen oder nutze die Seite ohne Speicherung.
      </p>
      <div class="flex gap-5 justify-center items-start w-full">
        <UTextarea
          placeholder="Code eingeben"
          :rows="1"
          autoresize
          class="w-full"
          v-model:model-value="code"
        />
        <UButton label="Neuen Code erstellen" variant="subtle" color="neutral" />
      </div>
      <UButton :label="code ? 'Weiter mit Code' : 'Weiter ohne Code'" icon="i-lucide-arrow-right" @click="goToHome" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
const router = useRouter()

const code = ref('')

const goToHome = () => {
  router.push('/home')
}
</script>

<style scoped></style>
