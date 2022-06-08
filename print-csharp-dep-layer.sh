#!/bin/bash
set -e

# Use the output of this script to update csharp dependency layer sections of the Dockerfile.
# You only need to update the Dockerfile if additional projects are added or the existing project names/paths change.
#
# This is a helper for a hack because:
#  1. the Dockerfile `COPY` command does not support recursive copying of a filetype while preserving directory (i.e., like rsync)
#  2. nuget now stores its csharp package references in the projects' csproj files
#
# This script generates 2 sections:
#  1. An `ARG` section to list the directory paths that can be created given permissions of a particular container user
#  2. A `COPY` section to copy the sln and csproj files from the project to the same directory inside the container

export FILES=$(find . -type f \( -iname "*.csproj" -o -iname "*.sln" -o -iname "*.Build.props" \) -not -path "*/node_modules/*" | sort -t '\0' -n)
# export DIRS=$(find . -type f \( -iname "*.csproj" -o -iname "*.sln" \) -not -path "*/node_modules/*" -exec dirname {} \; | sort -t '\0' -n)

# echo "#### ARG FOR THE TOP OF THE DOCKERFILE ####"
# echo ""
# echo "ARG CSHARP_DEP_LAYER_PROJECTS=\"\\"
# for DIR in $DIRS
# do
#     if [[ $DIR != "." ]]
#     then
#         CLEANDIR=$(echo -n $DIR | sed -En "s/\.\///p")
#         echo "    /build/$CLEANDIR/ \\"
#     fi
# done
# echo "\""

# echo ""
# echo ""

echo "#### COPY statements for the csharp dependency layer ####"
echo ""
for FILE in $FILES
do
    DIR=$(dirname $FILE)
    echo "    COPY --chown=node:node $FILE $FILE"
done
