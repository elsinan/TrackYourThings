#!/bin/sh

# Docker entrypoint script for handling runtime environment variables
# This allows the frontend to connect to different backend URLs without rebuilding

# If VITE_API_URL is provided as environment variable, update the built files
if [ ! -z "$VITE_API_URL" ]; then
    
    # Replace placeholder API URL in built JavaScript files
    # This is a simple approach for runtime configuration
    find /usr/share/nginx/html -name "*.js" -exec sed -i "s|http://localhost:8080|$VITE_API_URL|g" {} \;
    
else
    echo "Using default API URL (http://localhost:8080)"
fi

# Execute the original command
exec "$@"
