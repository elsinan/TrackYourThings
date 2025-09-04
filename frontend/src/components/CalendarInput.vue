<template>
  <div class="flex flex-col gap-7">
    <div class="flex flex-row justify-between">
      <h2 class="text-2xl font-bold">01.09.2025</h2>
      <UButton size="xl" label="Speichern" />
    </div>
    <UButtonGroup>
      <UBadge color="neutral" variant="outline" size="lg" icon="i-lucide-calculator" class="px-2" />
      <UInputNumber variant="outline" placeholder="Menge" />
    </UButtonGroup>
    <UTextarea icon="i-lucide-pencil" placeholder="Notiz hinzufügen..." :rows="5" autoresize />
  </div>
</template>

<script setup lang="ts"></script>
