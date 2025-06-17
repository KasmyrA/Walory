import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'

declare global {
  interface Window {
    API_BASE_URL: string;
  }
}

// U ycie zmiennej  rodowiskowej z fallbackiem do obecnej logiki
const API_BASE_URL = import.meta.env.VITE_API_URL || (
  window.location.hostname === 'localhost' 
    ? 'http://localhost:8080' 
    : `http://${window.location.hostname}:8080`
);

// Udost pnienie API_BASE_URL globalnie
window.API_BASE_URL = API_BASE_URL;

const app = createApp(App)
app.use(router)
app.mount('#app')
