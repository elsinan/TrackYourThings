import { defineStore } from 'pinia'
import { getLocalTimeZone, now, toCalendarDate } from '@internationalized/date'
import { TrackedItem } from '@/model/trackedItem'
import type { TrackingEntry } from '@/model/trackingEntry'
import { getApiConfiguration } from '@/api/utils'
import type { Configuration } from '@/api/generated'

export const useCalendarStore = defineStore('calendar', {
  state: () => ({
    trackedItems: [] as TrackedItem[],
    trackingEntries: [] as TrackingEntry[],
    dateSelected: false,
    selectedDate: toCalendarDate(now(getLocalTimeZone())),
    selectedTrackedItem: null as TrackedItem | null,
  }),
  actions: {
    getAllTrackedItems() {
      return this.trackedItems
    },

    async selectTrackedItem(id: number) {
      this.selectedTrackedItem = this.trackedItems.find((e) => {
        return e.id === id
      })!
      // get tracking Entries from backend
      // const apiConfig: Configuration = getApiConfiguration()
      // const trackedItemsApi = new TrackingEntryApi(apiConfig)
      // const response = await trackedItemsApi.apiTrackedItemsGet()
      // // this.trackingEntries =
    },

    createTrackedItem() {},

    modifyTrackedItem() {},

    deleteTrackedItem() {},

    addTrackingEntry() {},

    deleteTrackingEntry() {},

    modifyTrackingEntry() {},
  },
})
