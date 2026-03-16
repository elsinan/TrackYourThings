<script setup lang="ts">
import { shallowRef, watch } from 'vue'

import CalendarInput from './CalendarInput.vue'
import type { DropdownMenuItem } from '@nuxt/ui'
import { useCalendarStore } from '@/stores/calendarStore'
import { CalendarDate } from '@internationalized/date'

const store = useCalendarStore()

const date = shallowRef(
  new CalendarDate(
    store.selectedDate.calendar,
    store.selectedDate.era,
    store.selectedDate.year,
    store.selectedDate.month,
    store.selectedDate.day,
  ),
)

watch(
  () => date.value,
  (newData) => {
    store.selectedDate = newData
  },
  { deep: true },
)

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

    <UCalendar locale="de" v-model="date" size="xl">
      <template #day="{ day }">
        {{ day.day }}
      </template>
    </UCalendar>

    <template #footer>
      <CalendarInput />
    </template>
  </UCard>
</template>
