import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'

// U¿ycie zmiennej œrodowiskowej z fallbackiem do obecnej logiki
const API_BASE_URL = import.meta.env.VITE_API_URL || (
  window.location.hostname === 'localhost' 
    ? 'http://localhost:8080' 
    : `http://${window.location.hostname}:8080`
);

// Udostêpnienie API_BASE_URL globalnie
window.API_BASE_URL = API_BASE_URL;

const app = createApp(App)
app.use(router)
app.mount('#app')
