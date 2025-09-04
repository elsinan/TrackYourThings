import { defineStore } from 'pinia'
import { ref } from 'vue'

type CalendarEntry = {
  amount: number
  note: string
}
export const useCounterStore = defineStore('calendar', {
  state: () => ({
    trackedItems :[],

  }),
  actions: {

  }
}
)
