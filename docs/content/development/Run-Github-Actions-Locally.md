---
title: Code Structure
slug: run-github-actions-locally
---


- Nuget.Config


# Development workflow

# Run Github Action workflows locally with act
act -P self-hosted=ghcr.io/catthehacker/ubuntu:medium

act -P self-hosted=ghcr.io/catthehacker/ubuntu:medium -j publish-packages

dokumenter hvordan jeg kører jobs og specifikt job med act kommand lokalt samt at agent type skal være ubuntu-latest i stedet for self hosted før det virker så jeg skal sende agent type ind med en parameter i stedet for at bruge self hosted

hvis en af workflows har en syntax fejl vil act kommando ikke virke så selvom det job/workflow man vil køre ikke har fejl så skal alle andre workflows også være korrekte