import { useAuthStore } from '@/stores/auth'
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeUnAuthView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView, 
      meta: {
        layout: 'Main'
      },
      beforeEnter: (to, from, next) => {
        const authStore = useAuthStore()

        if (authStore.isLoggedIn) {
          next({ name: 'homeAuth' })
        } else {
          next({name: 'homeUnAuth'}) 
        }
      }
    },
    
    {
      path: '/files',
      name: 'files',
      component: () => import('../views/FilesView.vue'),
    },

    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/AboutUsView.vue'),
    
    },
    {
      path: '/home',
      name: 'homeAuth',
      component: () => import('../views/HomeView.vue'),
    },
      {
      path: '/',
      name: 'homeUnAuth',
      component: () => import('../views/HomeUnAuthView.vue'),

    }
  ],
})

export default router