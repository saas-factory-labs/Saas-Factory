#!/bin/bash

function print_usage() {
    echo "Usage: ./docker-manage.sh [command]"
    echo "Commands:"
    echo "  dev-up      - Start development environment with hot reload"
    echo "  dev-down    - Stop development environment"
    echo "  prod-up     - Start production environment"
    echo "  prod-down   - Stop production environment"
    echo "  logs        - Show logs from all services"
    echo "  clean       - Remove all containers, volumes, and images"
    return 0
}

case "$1" in
    "dev-up")
        docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --build
        ;;
    "dev-down")
        docker-compose -f docker-compose.yml -f docker-compose.override.yml down
        ;;
    "prod-up")
        docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
        ;;
    "prod-down")
        docker-compose -f docker-compose.yml -f docker-compose.prod.yml down
        ;;
    "logs")
        docker-compose logs -f
        ;;
    "clean")
        docker-compose -f docker-compose.yml -f docker-compose.override.yml down -v
        docker-compose -f docker-compose.yml -f docker-compose.prod.yml down -v
        docker system prune -af
        ;;
    *)
        print_usage
        exit 1
        ;;
esac