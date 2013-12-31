
dev:
	@echo "Starting Sass watcher and Dotnet watcher"
	sass -w ./Pages ./Pages/Account/Manage/_ManageNav.cshtml.scss:./Pages/Account/Manage/_ManageNav.cshtml.css ./Pages/Account/Manage/_Layout.cshtml.scss:./Pages/Account/Manage/_Layout.cshtml.css ./wwwroot/sass/styles.scss:./wwwroot/styles.css & \
	dotnet watch run --non-interactive -lp https & \
	wait