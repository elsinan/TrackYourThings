<template>
  <UApp>
    <RouterView />
  </UApp>
</template>

<script setup lang="ts">
import { useCalendarStore } from './stores/calendarStore'

const store = useCalendarStore()

const cookieValue = document.cookie
  .split('; ')
  .find((row) => row.startsWith('trackId='))
  ?.split('=')[1]

if (cookieValue) {
  store.trackingId = cookieValue
  console.log(cookieValue)
  store.getAllTrackedItems();
}

</script>
