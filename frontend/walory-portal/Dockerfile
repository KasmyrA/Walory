# Etap 1: Budowanie aplikacji
FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# Etap 2: Serwowanie aplikacji (np. przez nginx)
FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html

# Opcjonalnie: kopiowanie w³asnej konfiguracji nginx
# COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
