import type { TrackingEntry } from './trackingEntry'

export class TrackedItem {
  id: number
  title: string
  trackingEntries: TrackingEntry

  constructor(id: number, title: string, trackingEntries: TrackingEntry) {
    this.id = id
    this.title = title
    this.trackingEntries = trackingEntries
  }
}
