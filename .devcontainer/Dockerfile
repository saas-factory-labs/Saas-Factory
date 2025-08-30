# Start from the official .NET image
FROM mcr.microsoft.com/devcontainers/dotnet:9.0 AS base

# RUN chmod +x setup-dev-env.sh

# RUN sudo ./setup-dev-env.sh

# Install Python, Node.js, and other dependencies, Tailwind CSS, and .NET Workload
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        build-essential \
        libssl-dev \
        npm \
        python3 \
        python3-pip && \
    npm install -g typescript && \
    npm install -g tailwindcss && \
    dotnet workload install aspire && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*



# Install SQL Server dependencies and SQL Server itself
# RUN apt-get update && \
#     apt-get install -y \
#     curl \
#     apt-transport-https \
#     graphviz \
#     git-lfs \

    # Install Microsoft Azure and SQL Server tools
    # apt-get install -y \
    # mssql-server \
    # mssql-tools && \
    # curl https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2019.list | tee /etc/apt/sources.list.d/msprod.list && \
    # ACCEPT_EULA=Y MSSQL_PID=Developer apt-get update && \
    # apt-get install -y mssql-server mssql-tools

# Install Azure Functions Core Tools (via npm)
# RUN npm i -g azure-functions-core-tools@4 --unsafe-perm true

# Install Microsoft Azure Developer CLI
# RUN curl -fsSL https://aka.ms/install-azd.sh | bash

# Install AZCopy
# RUN wget https://aka.ms/downloadazcopy-v10-linux -O azcopy.tar.gz && \
#     tar -xf azcopy.tar.gz && \
#     sudo cp ./azcopy_linux_amd64_*/azcopy /usr/bin/

# Install k6 for load testing
# RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 6A030B21BA07F4FB && \
#     echo "deb https://dl.k6.io/deb stable main" | tee /etc/apt/sources.list.d/k6.list && \
#     apt-get update && apt-get install -y k6

# Install Pulumi for Infrastructure as Code
# RUN curl -fsSL https://get.pulumi.com | sh

# Install GitHub CLI
# RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | tee /usr/share/keyrings/githubcli-archive-keyring.gpg && \
#     apt-add-repository https://cli.github.com/packages && \
#     apt-get update && apt-get install -y gh

# Configure SQL Server to run in Developer mode and accept the EULA
# RUN /opt/mssql/bin/mssql-conf set-sa-password
# RUN /opt/mssql/bin/mssql-conf set memory.memorylimitmb 2048  # Optional: limit memory usage

# Expose SQL Server default port
# EXPOSE 1433

# Set a default password for SA
# ENV SA_PASSWORD="YourStrong!Passw0rd"
# ENV ACCEPT_EULA="Y"

# Expose other ports if necessary (3000 for Node.js, 5000 for .NET, etc.)
EXPOSE 3000 5000 8000 1433

# Run SQL Server
CMD ["/opt/mssql/bin/sqlservr"]

# Set the working directory for the DevContainer
WORKDIR /workspace

# Copy your application code if needed (this is typically done in the Codespace)
COPY . .

# Restore project dependencies
# RUN dotnet restore

# Build the project in Release mode
# RUN dotnet build -c Release