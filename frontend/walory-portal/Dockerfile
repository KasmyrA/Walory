# Use official Node image to build the app
FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# Use Nginx to serve the built files
FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html

# Copy custom nginx config for SPA fallback
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]