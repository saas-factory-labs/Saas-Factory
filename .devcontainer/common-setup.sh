#!/bin/bash

# Update the package list to ensure the latest version of packages is installed
apt-get update

# Install common dependencies
apt-get install -y \
    git \
    curl \
    vim \
    zsh \
    build-essential

# Optional: Install additional dependencies that are needed in the development environment
# Example: Node.js
curl -fsSL https://deb.nodesource.com/setup_14.x | bash -
apt-get install -y nodejs

# Clean up to reduce the size of the image
apt-get clean && rm -rf /var/lib/apt/lists/*
