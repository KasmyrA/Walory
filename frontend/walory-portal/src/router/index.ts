import { createRouter, createWebHistory } from 'vue-router'

import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'

const routes = [
  {
  path: '/',
  component: () => import('../layouts/MainLayout.vue'),
  children: [
    { path: 'home', name: 'Home', component: () => import('../views/HomeView.vue') },
    // ...other routes
  ]
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView
  },
  {
  path: '/confirm-email',
  name: 'ConfirmEmail',
  component: () => import('../views/ConfirmEmailView.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
