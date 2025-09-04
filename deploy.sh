#!/bin/bash

# TrackYourThings Deployment Script
# This script can be used to deploy the application to a server

set -e

# Configuration
REGISTRY="ghcr.io"
REPO_NAME="elsinan/trackyourthings"
COMPOSE_FILE="docker-compose.yml"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
NC='\033[0m' # No Color

# Functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        log_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
}

# Pull latest images
pull_images() {
    log_info "Pulling latest images..."
    docker-compose -f $COMPOSE_FILE pull
}

# Deploy application
deploy() {
    log_info "Deploying TrackYourThings application..."
    
    # Stop existing containers
    docker-compose -f $COMPOSE_FILE down
    
    # Start new containers
    docker-compose -f $COMPOSE_FILE up -d
    
    log_info "Waiting for services to be ready..."
    sleep 30
    
    # Health check
    if curl -f http://localhost:8080/health > /dev/null 2>&1; then
        log_info "✅ Backend is healthy"
    else
        log_warn "⚠️  Backend health check failed"
    fi
    
    if curl -f http://localhost/health > /dev/null 2>&1; then
        log_info "✅ Frontend is healthy"
    else
        log_warn "⚠️  Frontend health check failed"
    fi
    
    log_info "🚀 Deployment completed!"
    log_info "Frontend: http://localhost"
    log_info "Backend API: http://localhost:8080"
}

# Rollback function
rollback() {
    log_warn "Rolling back to previous version..."
    docker-compose -f $COMPOSE_FILE down
    # Here you would restore previous images
    log_info "Rollback completed"
}

# Main execution
main() {
    case "${1:-deploy}" in
        "deploy")
            check_docker
            pull_images
            deploy
            ;;
        "rollback")
            rollback
            ;;
        "logs")
            docker-compose -f $COMPOSE_FILE logs -f
            ;;
        "status")
            docker-compose -f $COMPOSE_FILE ps
            ;;
        *)
            echo "Usage: $0 {deploy|rollback|logs|status}"
            exit 1
            ;;
    esac
}

main "$@"
