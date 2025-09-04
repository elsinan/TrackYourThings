import { describe, it, expect } from 'vitest'

import { mount } from '@vue/test-utils'
import CalenderItem from '../CalendarItem.vue'

describe('HelloWorld', () => {
  it('renders properly', () => {
    const wrapper = mount(CalenderItem)
    expect(wrapper.text()).toContain('Kalender')
  })
})
