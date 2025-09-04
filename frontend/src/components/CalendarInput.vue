<template>
  <div class="flex flex-col gap-7">
    <div class="flex flex-row justify-between">
      <h2 class="text-2xl font-bold">01.09.2025</h2>
      <UButton size="xl" label="Speichern" />
    </div>
    <UButtonGroup>
      <UBadge color="neutral" variant="subtle" size="lg" icon="i-lucide-calculator" />
      <UInputNumber placeholder="Menge" />
    </UButtonGroup>
    <UTextarea icon="i-lucide-pencil" placeholder="Notiz hinzufügen..." :rows="5"  autoresize/>
  </div>
</template>

<script setup lang="ts"></script>
