import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'

import HomeView from '../views/HomeView.vue'

const routes = [
  {
    path: '/',
    component: () => import('../layouts/MainLayout.vue'),
    children: [
      { path: '', name: 'Home', component: HomeView },
      { path: 'dashboard', name: 'Dashboard', component: () => import('../views/DashboardView.vue') },
      { path: 'settings', name: 'Settings', component: () => import('../views/SettingsView.vue') },
      { path: 'friends', name: 'Friends', component: () => import('../views/FriendsView.vue') },
      { path: 'chat', name: 'Chat', component: () => import('../views/ChatView.vue') },
      { path: 'browse', name: 'Browse', component: () => import('../views/BrowseView.vue') },
      { path: 'collection', name: 'Collection', component: () => import('../views/CollectionView.vue') },
      { path: 'export', name: 'Export', component: () => import('../views/ExportView.vue') },
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
  },
  {
  path: '/auth/reset-password',
  name: 'ResetPassword',
  component: () => import('../views/ResetPasswordView.vue')
  },
  {
  path: '/auth/confirm-email-change',
  name: 'ConfirmEmailChange',
  component: () => import('../views/ConfirmEmailChangeView.vue')
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router