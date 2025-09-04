#!/bin/bash

# Development helper script for TrackYourThings (Yarn version)

set -e

# Colors
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
RED='\033[0;31m'
NC='\033[0m'

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if yarn is installed
check_yarn() {
    if ! command -v yarn &> /dev/null; then
        log_error "Yarn is not installed. Please install yarn first:"
        echo "npm install -g yarn"
        exit 1
    fi
}

# Setup frontend
setup_frontend() {
    log_info "Setting up frontend..."
    cd frontend
    yarn install
    cd ..
}

# Setup backend
setup_backend() {
    log_info "Setting up backend..."
    cd backend
    dotnet restore
    cd ..
}

# Start development servers
dev() {
    log_info "Starting development environment..."
    
    # Start backend with database
    log_info "Starting backend and database..."
    docker-compose -f backend/docker-compose-local.yml up -d
    
    # Wait for backend to be ready
    log_info "Waiting for backend to be ready..."
    sleep 10
    
    # Start frontend dev server
    log_info "Starting frontend dev server..."
    cd frontend
    yarn dev
}

# Build everything
build() {
    log_info "Building application..."
    
    # Build backend
    cd backend
    dotnet build --configuration Release
    cd ..
    
    # Build frontend
    cd frontend
    yarn build
    cd ..
    
    log_info "✅ Build completed!"
}

# Test everything
test() {
    log_info "Running tests..."
    
    # Test backend
    cd backend
    dotnet test --configuration Release
    cd ..
    
    # Test frontend
    cd frontend
    yarn test:unit --run
    cd ..
    
    log_info "✅ Tests completed!"
}

# Lint and format
lint() {
    log_info "Linting and formatting..."
    
    cd frontend
    yarn lint
    yarn format
    cd ..
    
    log_info "✅ Linting completed!"
}

# Clean everything
clean() {
    log_info "Cleaning build artifacts..."
    
    # Clean backend
    cd backend
    dotnet clean
    rm -rf bin obj
    cd ..
    
    # Clean frontend
    cd frontend
    rm -rf dist node_modules/.cache
    cd ..
    
    # Clean Docker
    docker-compose -f backend/docker-compose-local.yml down -v
    
    log_info "✅ Cleaning completed!"
}

# Main function
main() {
    case "${1:-help}" in
        "setup")
            check_yarn
            setup_backend
            setup_frontend
            ;;
        "dev")
            dev
            ;;
        "build")
            build
            ;;
        "test")
            test
            ;;
        "lint")
            lint
            ;;
        "clean")
            clean
            ;;
        "help"|*)
            echo "TrackYourThings Development Helper (Yarn Edition)"
            echo ""
            echo "Usage: $0 {setup|dev|build|test|lint|clean}"
            echo ""
            echo "Commands:"
            echo "  setup  - Install all dependencies"
            echo "  dev    - Start development environment"
            echo "  build  - Build both frontend and backend"
            echo "  test   - Run all tests"
            echo "  lint   - Lint and format code"
            echo "  clean  - Clean all build artifacts"
            ;;
    esac
}

main "$@"
