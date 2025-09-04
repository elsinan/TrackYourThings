<script setup lang="ts">
import { ref } from 'vue'
const items = ref(['Sport', 'Meditation', 'Schlaf'])
const value = ref('Sport')
</script>

<template>
  <UApp>
    <header>
      <div class="flex items-center-safe m-5 gap-5 justify-between">
        <div class="flex gap-3 items-center">
          <h1 class="text-2xl font-bold">Track Your</h1>
          <UButtonGroup>
            <USelectMenu
              size="xl"
              v-model="value"
              v-bind:class="'text-lg font-bold'"
              :items="items"
              class="w-40"
            />
            <UButton size="xl" color="neutral" icon="i-lucide-plus" />
          </UButtonGroup>
        </div>
      </div>
    </header>

    <main>
      <div class="m-5">
        <RouterView />
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
  </UApp>
</template>
