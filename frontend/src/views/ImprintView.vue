<template>
  <div>
    <UButton
      icon="i-lucide-arrow-left"
      color="neutral"
      variant="outline"
      label="Zurück zur Startseite"
      @click="$router.push('/')"
    />
    <h1>Impressum</h1>
  </div>
</template>

<script setup lang="ts"></script>
