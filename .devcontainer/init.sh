#!/bin/bash
echo "Ensure that there is an .env file"
pushd $(dirname "${0}") > /dev/null
cd ../
if [ ! -f ./.env ]
then
    echo "Copies sample env to .env"
    cp ./.env.sample ./.env
else
    echo "There is already an .env file, not doing anything"
fi
