#!/bin/sh

# This is the entrypoint script for running the apps.
cd $APP_ENTRYPOINT
dotnet ${APP_ENTRYPOINT}.dll