#!/bin/sh
echo "Ensure that there is an .env file"
if [ ! -f ./.env ]
then
    echo "Copies sample env to .env"
    cp ./.env.devcontainer ./.env
else
    echo "There is already an .env file, not doing anything"
fi
npm install -g azure-functions-core-tools@4 --unsafe-perm true