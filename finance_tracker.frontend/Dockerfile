FROM node:20 AS build
WORKDIR /app

# Copy packages and install dependencies
COPY package*.json ./
RUN npm ci

# Copy source code and build
COPY . .
RUN npm run build --prod

# Runtime stage - serve with nginx
FROM nginx:alpine

# Make sure alpine packages are up to date
RUN apk upgrade --no-cache

COPY --from=build /app/dist/finance-tracker/browser /usr/share/nginx/html

# Copy nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]