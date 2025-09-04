# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Enable Corepack for Yarn
RUN corepack enable

# Copy package files
COPY package.json yarn.lock ./
RUN yarn install --frozen-lockfile

# Copy source code and build
COPY . .
RUN yarn build

# Production stage
FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

# Copy built app from build stage
COPY --from=build /app/dist .

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
