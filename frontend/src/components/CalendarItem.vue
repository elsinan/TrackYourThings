<script setup lang="ts">
import { shallowRef } from 'vue'

import CalendarInput from './CalendarInput.vue'
import type { DropdownMenuItem } from '@nuxt/ui'
import { useCalendarStore } from '@/stores/calendarStore'

const store = useCalendarStore()

const data = shallowRef([

])
const items: DropdownMenuItem[] = [
  {
    label: 'Namen ändern',
    icon: 'i-lucide-type-outline',
    onSelect: () => {
      // Handle name change
    },
  },
  {
    label: 'Daten exportieren',
    icon: 'i-lucide-file-json',
    onSelect: () => {
      // Handle data export
    },
  },
  {
    label: 'Löschen',
    icon: 'i-lucide-trash',
    color: 'warning',
    onSelect: () => {
      // Handle delete
    },
  },
]
</script>

<template>
  <UCard>
    <template #header>
      <div class="flex justify-between items-center flex-wrap gap-5">
        <h2 class="text-2xl font-bold">Kalender</h2>

        <UDropdownMenu :items="items">
          <UButton
            size="xl"
            color="neutral"
            variant="subtle"
            icon="i-lucide-settings"
            trailing-icon="i-lucide-chevron-down"
          />
        </UDropdownMenu>
      </div>
    </template>

    <UCalendar locale="de" v-model="data" size="xl">
      <template #day="{ day }">
        {{ day.day }}
      </template>
    </UCalendar>

    <template #footer v-if="store.dateSelected">
      <CalendarInput />
    </template>
  </UCard>
</template>
