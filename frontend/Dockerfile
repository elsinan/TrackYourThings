# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Enable Corepack for Yarn
RUN corepack enable

# Copy package files
COPY package.json yarn.lock ./
RUN yarn install --frozen-lockfile

# Copy source code
COPY . .

# Build argument for API URL (can be overridden during build)
ARG VITE_API_URL=http://localhost:8080

# Set environment variable for build
ENV VITE_API_URL=$VITE_API_URL

# Build the application
RUN yarn build

# Production stage
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

# Copy built app from build stage
COPY --from=build /app/dist .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Add script to handle runtime environment variables
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

EXPOSE 80

ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]
