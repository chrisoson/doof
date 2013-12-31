
dev:
	@echo "Starting Sass watcher and Dotnet watcher"
	sass -w ./Pages ./wwwroot/sass/styles.scss:./wwwroot/styles.css & \
	dotnet watch run --non-interactive -lp https & \
	wait