#!/bin/bash

clean_up() {
    echo "Cleaning up..."
    rm -rf certs/certificates volumes
    echo "Clean up complete."
}

if [[ "$1" == "clean" ]]; then
    clean_up
fi

if [[ ! -d "certs/certificates" ]]; then
    echo "Generating certificates..."
    if cd certs && sh generate.sh; then
        cd .. || exit 1
        echo "Certificates generated successfully."
    else
        echo "Failed to generate certificates." >&2
        exit 1
    fi
else
    echo "Certificates already exist."
fi

echo "Starting Docker Compose..."
docker-compose --env-file .env.docker up --build
