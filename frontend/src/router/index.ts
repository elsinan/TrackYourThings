import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LandingPage from '../views/LandingPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'landing',
      component: LandingPage,
    },
    {
      path: '/home',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/impressum',
      name: 'impressum',
      component: () => import('../views/ImprintView.vue'),
    },
    {
      path: '/datenschutz',
      name: 'datenschutz',
      component: () => import('../views/DataPrivacyView.vue'),
    },
  ],
})

export default router
