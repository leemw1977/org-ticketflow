PROJECT_NAME=OrgTicketflow
CONFIG=Release
# SRC_FOLDER=./cli
# SRC=./cli/OrgTicketFlowCLI.csproj
# OUT=./bin

SERVICE_DIR := ./org-ticketflow-service
SERVICE_OUT := ./bin/org-ticketflow-service
ENV_PATH ?= /Users/leewilliams/src/leemw1977/org-ticketflow/.env

.PHONY: all build clean run

all: build

# Cleans and rebuilds the service
build:
	@echo "Cleaning and building org-ticketflow-service..."
	dotnet clean $(SERVICE_DIR)
	dotnet build $(SERVICE_DIR) -c $(CONFIG) -o $(SERVICE_OUT)
	@echo "Build complete."

# Clean everything
clean:
	@echo "Cleaning all builds..."
	dotnet clean $(SERVICE_DIR)
	rm -rf $(SERVICE_OUT)

# Run the service in development mode
# This assumes the service is set up to run in development mode

run:
	@echo "👀 Watching org-ticketflow-service with .env at $(ENV_PATH)..."
	ORG_TICKETFLOW_ENV_PATH=$(ENV_PATH) \
	dotnet watch --project $(SERVICE_DIR) run


# clean-cli:
# 	@echo "🧹 Cleaning old builds..."
# 	rm -rf $(OUT)
# 	rm -rf $(SRC_FOLDER)/bin
# 	rm -rf $(SRC_FOLDER)/obj

# build-cli: clean-cli build-linux build-macos build-windows

# build-linux:
# 	@echo "🐧 Building for Linux..."
# 	dotnet publish $(SRC) \
# 		-c $(CONFIG) \
# 		-r linux-x64 \
# 		--self-contained true \
# 		-p:PublishSingleFile=true \
# 		-o $(OUT)/linux

# build-macos:
# 	@echo "🍎 Building for macOS..."
# 	dotnet publish $(SRC) \
# 		-c $(CONFIG) \
# 		-r osx-x64 \
# 		--self-contained true \
# 		-p:PublishSingleFile=true \
# 		-o $(OUT)/macos

# build-windows:
# 	@echo "🪟 Building for Windows..."
# 	dotnet publish $(SRC) \
# 		-c $(CONFIG) \
# 		-r win-x64 \
# 		--self-contained true \
# 		-p:PublishSingleFile=true \
# 		-o $(OUT)/windows
