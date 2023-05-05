#!/bin/bash
echo "Ensure that there is an .env file"
pushd $(dirname "${0}") > /dev/null
cd ../
if [ ! -f ./.env ]
then
    echo "Copies sample env to .env"
    cp ./.env.sample ./.env
    source .env
    echo "Include connection strings in environment"
    export SA_PASSWORD=${SA_PASSWORD}
    export ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection}
    export ConnectionStrings__Redis=${ConnectionStrings__Redis}
    export ConnectionStrings__AzureStorage=${ConnectionStrings__AzureStorage}
else
    echo "There is already an .env file, not doing anything"
fi
npm install -g azure-functions-core-tools@4 --unsafe-perm true