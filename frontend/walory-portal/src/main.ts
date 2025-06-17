import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'

const API_BASE_URL = window.location.hostname === 'localhost' 
  ? 'http://localhost:8080'
  : `http://${window.location.hostname}:8080`;
window.API_BASE_URL = API_BASE_URL;

const app = createApp(App)
app.use(router)
app.mount('#app')
