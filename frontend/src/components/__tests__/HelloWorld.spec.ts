import { createPinia, setActivePinia } from 'pinia'

beforeEach(() => {
  setActivePinia(createPinia())
})

it('renders properly', () => {
  const wrapper = mount(CalenderItem, {
    global: {
      plugins: [createPinia()],
    },
  })
  expect(wrapper.text()).toContain('Kalender')
})