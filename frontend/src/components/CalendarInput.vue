<template>
  <div class="flex flex-col gap-7">
    <div class="flex flex-row justify-between">
      <h2 class="text-2xl font-bold">
        {{
          store.selectedDate.toDate(getLocalTimeZone()).toLocaleDateString('de-DE', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
          })
        }}
      </h2>
      <div class="flex flex-row gap-4">
        <UButton size="xl" icon="i-lucide-trash" variant="outline" color="error" @click="store.dateSelected=false" />
        <UButton size="xl" label="Speichern" />
      </div>
    </div>
    <UButtonGroup>
      <UBadge color="neutral" variant="outline" size="lg" icon="i-lucide-calculator" class="px-2" />
      <UInputNumber variant="outline" placeholder="Menge" />
    </UButtonGroup>
    <UTextarea icon="i-lucide-pencil" placeholder="Notiz hinzufügen..." :rows="5" autoresize />
  </div>
</template>

<script setup lang="ts">
import { useCalendarStore } from '@/stores/calendarStore'
import { getLocalTimeZone } from '@internationalized/date'

const store = useCalendarStore()
</script>
