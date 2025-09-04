<script setup lang="ts">
import { shallowRef } from 'vue'

import { CalendarDate } from '@internationalized/date'
import CalendarInput from './CalendarInput.vue'
import type { DropdownMenuItem } from '@nuxt/ui'

const data = shallowRef([
  new CalendarDate(2025, 9, 1),
  new CalendarDate(2025, 9, 2),
  new CalendarDate(2025, 9, 3),
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

    <template #footer>
      <CalendarInput />
    </template>
  </UCard>
</template>
