import { createRouter, createWebHistory } from 'vue-router'

import LoginView from '../views/LoginView.vue'

const routes = [
  {
  path: '/',
  component: () => import('../layouts/MainLayout.vue'),
  children: [
    { path: 'dashboard', name: 'Dashboard', component: () => import('../views/DashboardView.vue') },
    { path: 'settings', name: 'Settings', component: () => import('../views/SettingsView.vue') },
    { path: 'friends', name: 'Friends', component: () => import('../views/FriendsView.vue') },
    { path: 'chat', name: 'Chat', component: () => import('../views/ChatView.vue') },
    { path: 'browse', name: 'Browse', component: () => import('../views/BrowseView.vue') },
    { path: 'collection', name: 'Collection', component: () => import('../views/CollectionView.vue') },
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
