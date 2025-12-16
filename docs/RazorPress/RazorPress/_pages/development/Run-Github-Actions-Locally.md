---
title: Run Github Action workflows locally
---

## Run all workflows ##
act -P self-hosted=ghcr.io/catthehacker/ubuntu:medium

## Run specific workflow ##
act -P self-hosted=ghcr.io/catthehacker/ubuntu:medium -j publish-packages

# Troubleshooting #

- Agent type needs to be ubuntu-latest instead of self hosted for it to work
- If one of the workflows has a syntax error, the act command will not work, so even if the job/workflow you want to run has no errors, all other workflows must also be correct.


