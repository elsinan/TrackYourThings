import { defineStore } from 'pinia'
import { getLocalTimeZone, now, toCalendarDate } from '@internationalized/date'

export const useCalendarStore = defineStore('calendar', {
  state: () => ({
    trackedItems: [],
    trackingEntries: {},
    dateSelected: false,
    selectedDate: toCalendarDate(now(getLocalTimeZone())),
    selectedTrackItemId: '',
    selectedTrackItem: '',
  }),
  actions: {
    getAllTrackedItems() {},

    getTrackedItem() {},

    createTrackedItem() {},

    modifyTrackedItem() {},

    deleteTrackedItem() {},

    addTrackingEntry() {},

    deleteTrackingEntry() {},

    modifyTrackingEntry() {},
  },
})
