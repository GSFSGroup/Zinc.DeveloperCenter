#!/usr/bin/env bash

# After creating this from the template, run this command:
# chmod +x set-execute-bits.sh && set-execute-bits.sh
find . \( -name "*.sh" -or -name "docker-healthcheck" \) -exec chmod a+x {} \; -exec ls \-al {} \;
