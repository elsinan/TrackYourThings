import { defineStore } from 'pinia'
import { CalendarDate } from '@internationalized/date'

export const useCounterStore = defineStore('calendar', {
  state: () => ({
    trackedItems: [],
    itemEntries: [],
    selectedDate: CalendarDate,
    selectedTrackItemId: '',
  }),
  actions: {
    getAllTrackedItems() {},

    getTrackedItem() {},

    createTrackedItem() {},

    modifyTrackedItem() {},

    deleteTrackedItem() {},

    addTrackEntry() {},

    deleteTrackEntry() {},

    modifyTrackEntry() {},
  },
})
