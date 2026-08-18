SHELL := /bin/bash

SOLUTION    := ModularMonolith.slnx
API_PROJECT := src/Api/ModularMonolith.Api
CONFIG      ?= Debug

ENV_PORT    := $(shell sed -n 's|.*localhost:\([0-9]*\).*|\1|p' .env 2>/dev/null | head -1)
PORT        ?= $(if $(ENV_PORT),$(ENV_PORT),8080)

PROJECT_Auth := src/Modules/Auth/ModularMonolith.Modules.Auth
PROJECT_Core := src/Modules/Core/ModularMonolith.Modules.Core

MODULE  ?=
NAME    ?=
CONTEXT ?= $(MODULE)
DIR     ?= Migrations
SCRIPTS_DIR := scripts
OUT     ?= $(SCRIPTS_DIR)/$(CONTEXT)-migration.sql
MODULE_PROJECT = $(PROJECT_$(MODULE))

CYAN   := \033[36m
GREEN  := \033[32m
YELLOW := \033[33m
RED    := \033[31m
DIM    := \033[2m
BOLD   := \033[1m
RESET  := \033[0m

.DEFAULT_GOAL := help
.PHONY: help setup restore build rebuild run watch clean format test env info outdated migrations-add migrations-remove migrations-script

help: ## 📖  Show this help
	@printf "\n  $(BOLD)$(CYAN)ModularMonolith$(RESET) $(DIM)· .NET 10 modular monolith$(RESET)\n\n"
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## /{printf "  $(GREEN)%-9s$(RESET) %s\n", $$1, $$2}' $(MAKEFILE_LIST)
	@printf "\n  $(DIM)Overrides: make run PORT=9000 CONFIG=Release$(RESET)\n\n"

setup: env restore build ## 🌱  First-time setup: env + restore + build
	@printf "$(GREEN)🌱  Ready. Run 'make run' to start.$(RESET)\n"

env: ## 🔐  Create .env from .env.example
	@if [ -f .env ]; then \
		printf "$(YELLOW)⚠️   .env already exists — leaving it alone$(RESET)\n"; \
	else \
		cp .env.example .env; \
		printf "$(GREEN)✅  Created .env from .env.example$(RESET)\n"; \
	fi

restore: ## 📦  Restore NuGet packages + local tools
	@printf "$(CYAN)📦  Restoring packages…$(RESET)\n"
	@dotnet restore $(SOLUTION)
	@dotnet tool restore
	@printf "$(GREEN)✅  Restore complete$(RESET)\n"

build: ## 🔨  Build the solution
	@printf "$(CYAN)🔨  Building ($(CONFIG))…$(RESET)\n"
	@dotnet build $(SOLUTION) -c $(CONFIG) --nologo
	@printf "$(GREEN)✅  Build succeeded$(RESET)\n"

rebuild: clean build ## ♻️   Clean, then build from scratch

run: ## 🚀  Run the API
	@printf "$(CYAN)🚀  Starting API → $(BOLD)http://localhost:$(PORT)$(RESET)\n"
	@ASPNETCORE_URLS=http://localhost:$(PORT) dotnet run --project $(API_PROJECT) -c $(CONFIG)

watch: ## 👀  Run the API with hot reload
	@printf "$(CYAN)👀  Watching → $(BOLD)http://localhost:$(PORT)$(RESET)\n"
	@ASPNETCORE_URLS=http://localhost:$(PORT) dotnet watch --project $(API_PROJECT) run

test: ## 🧪  Run tests
	@if [ -z "$$(find tests -name '*.csproj' 2>/dev/null)" ]; then \
		printf "$(YELLOW)🧪  No test projects yet — skipping$(RESET)\n"; \
	else \
		printf "$(CYAN)🧪  Running tests…$(RESET)\n"; \
		dotnet test $(SOLUTION) -c $(CONFIG) --nologo; \
		printf "$(GREEN)✅  Tests passed$(RESET)\n"; \
	fi

migrations-add: ## 🧬  Add a migration (make migrations-add MODULE=Auth NAME=Init [CONTEXT=AuthCatalog DIR=CatalogMigrations])
	@if [ -z "$(MODULE)" ] || [ -z "$(NAME)" ]; then \
		printf "$(RED)❌  Usage: make migrations-add MODULE=<Auth|Core> NAME=<MigrationName>$(RESET)\n"; \
		exit 1; \
	fi
	@if [ -z "$(MODULE_PROJECT)" ]; then \
		printf "$(RED)❌  Unknown MODULE '$(MODULE)' — expected Auth or Core$(RESET)\n"; \
		exit 1; \
	fi
	@printf "$(CYAN)🧬  Adding migration '$(NAME)' to $(MODULE) ($(CONTEXT)DbContext)…$(RESET)\n"
	@dotnet ef migrations add $(NAME) \
		--project $(MODULE_PROJECT) \
		--startup-project $(API_PROJECT) \
		--context $(CONTEXT)DbContext \
		--output-dir Persistence/$(DIR)
	@printf "$(GREEN)✅  Migration '$(NAME)' added to $(MODULE)$(RESET)\n"

migrations-remove: ## 🗑️   Remove the last migration (make migrations-remove MODULE=Auth [CONTEXT=AuthCatalog])
	@if [ -z "$(MODULE)" ]; then \
		printf "$(RED)❌  Usage: make migrations-remove MODULE=<Auth|Core>$(RESET)\n"; \
		exit 1; \
	fi
	@if [ -z "$(MODULE_PROJECT)" ]; then \
		printf "$(RED)❌  Unknown MODULE '$(MODULE)' — expected Auth or Core$(RESET)\n"; \
		exit 1; \
	fi
	@printf "$(CYAN)🗑️   Removing last migration from $(MODULE) ($(CONTEXT)DbContext)…$(RESET)\n"
	@dotnet ef migrations remove \
		--project $(MODULE_PROJECT) \
		--startup-project $(API_PROJECT) \
		--context $(CONTEXT)DbContext
	@printf "$(GREEN)✅  Last migration removed from $(MODULE)$(RESET)\n"

migrations-script: ## 📜  Generate an idempotent SQL script, generate-only (make migrations-script MODULE=Auth [CONTEXT=AuthCatalog OUT=path.sql])
	@if [ -z "$(MODULE)" ]; then \
		printf "$(RED)❌  Usage: make migrations-script MODULE=<Auth|Core> [OUT=path.sql]$(RESET)\n"; \
		exit 1; \
	fi
	@if [ -z "$(MODULE_PROJECT)" ]; then \
		printf "$(RED)❌  Unknown MODULE '$(MODULE)' — expected Auth or Core$(RESET)\n"; \
		exit 1; \
	fi
	@printf "$(CYAN)📜  Generating idempotent SQL script for $(MODULE) ($(CONTEXT)DbContext) → $(OUT)…$(RESET)\n"
	@mkdir -p $(dir $(OUT))
	@dotnet ef migrations script --idempotent \
		--project $(MODULE_PROJECT) \
		--startup-project $(API_PROJECT) \
		--context $(CONTEXT)DbContext \
		-o $(OUT)
	@printf "$(GREEN)✅  Script written to $(OUT)$(RESET)\n"

format: ## 🎨  Format code in place
	@printf "$(CYAN)🎨  Formatting…$(RESET)\n"
	@dotnet format $(SOLUTION)
	@printf "$(GREEN)✅  Formatted$(RESET)\n"

clean: ## 🧹  Remove build output
	@printf "$(YELLOW)🧹  Cleaning bin/ and obj/…$(RESET)\n"
	@find . -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + 2>/dev/null || true
	@printf "$(GREEN)✅  Clean$(RESET)\n"

info: ## ℹ️   Show SDK version and projects
	@printf "$(CYAN)ℹ️   SDK$(RESET)      $$(dotnet --version)\n"
	@printf "$(CYAN)ℹ️   Config$(RESET)   $(CONFIG) · port $(PORT)\n"
	@printf "$(CYAN)ℹ️   Projects$(RESET)\n"
	@dotnet sln $(SOLUTION) list | grep -F .csproj | sed 's|^|      |'

outdated: ## 🔍  List outdated NuGet packages
	@printf "$(CYAN)🔍  Checking for outdated packages…$(RESET)\n"
	@dotnet list $(SOLUTION) package --outdated
