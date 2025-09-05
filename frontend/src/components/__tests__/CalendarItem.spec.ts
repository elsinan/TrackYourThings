import { describe, it, expect, beforeAll } from 'vitest'
import { mount } from '@vue/test-utils'
import CalendarItem from '../CalendarItem.vue'
import { setActivePinia, createPinia } from 'pinia'
import router from '../../router/index'

beforeAll(async () => {
  setActivePinia(createPinia())
  router.push('/')
  await router.isReady()
})

describe('CalendarItem.vue', () => {
  it('renders the current month and year for the calendar', async () => {
    const wrapper = mount(CalendarItem, {
      global: {
        plugins: [router],
      },
    })
    expect(wrapper.text()).toContain(
      new Date().toLocaleDateString('de-DE', {
        year: 'numeric',
        month: 'long',
      }),
    )
  })
})
