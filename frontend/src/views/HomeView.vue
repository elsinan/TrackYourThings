<template>
  <header>
    <div class="flex items-center-safe m-5 gap-5 justify-between">
      <div class="flex gap-3 items-center">
        <h1 class="text-2xl font-bold">Track Your</h1>
        <div v-if="items.length === 0">
          <h1 class="text-2xl font-bold">Things</h1>
        </div>
        <div v-else>
          <USelectMenu
            size="xl"
            v-model="value"
            v-bind:class="'text-lg font-bold'"
            :items="items"
            class="w-40"
          />
          <UButton size="xl" color="neutral" icon="i-lucide-plus" @click="createTrackedItem" />
        </div>
      </div>
    </div>
  </header>

  <main>
    <div class="m-5">
      <div v-if="items.length === 0">
        <UCard>
          <div class="flex flex-col items-center gap-5">
            <p>Erstelle ein erstes Ding, welches Du tracken willst.</p>
            <UInput placeholder="Name" v-model="newTrackItemName"/>

            <UButton
            label="Track Item erstellen"
            variant="subtle"
            icon="i-lucide-plus"
            @click="createTrackedItem" />
          </div>
        </UCard>
      </div>
      <div v-else>
        <CalendarItem />
      </div>
    </div>
  </main>

  <footer>
    <div class="flex align-center justify-center p-5">
      <nav class="flex gap-x-10">
        <RouterLink to="/impressum"> Impressum </RouterLink>

        <RouterLink to="/datenschutz"> Datenschutz </RouterLink>
        <UButton
          variant="subtle"
          color="neutral"
          label="Zum Code"
          trailing-icon="i-lucide-github"
          href="https://github.com/elsinan/TrackYourThings"
        />
      </nav>
    </div>
  </footer>
</template>

<script setup lang="ts">
import CalendarItem from '../components/CalendarItem.vue'

import { getApiConfiguration } from '@/api/utils'
import { Configuration, TrackedItemsApi } from '@/api/generated'
import { onBeforeMount, ref, type Ref } from 'vue'

const items: Ref<string[], string[]> = ref([])
const value = ref('')
const newTrackItemName = ref('')

onBeforeMount(async () => {
  const apiConfig: Configuration = getApiConfiguration()
  const trackedItemsApi = new TrackedItemsApi(apiConfig)
  const response = await trackedItemsApi.apiTrackedItemsGet()
  items.value = response.filter((e) => typeof e.title === 'string').map((e) => e.title as string)
})

const createTrackedItem = async () => {
  const apiConfig = getApiConfiguration()
  const trackedItemsApi = new TrackedItemsApi(apiConfig)
  await trackedItemsApi.apiTrackedItemsCreateNamePost({ name: newTrackItemName.value })
}
</script>
