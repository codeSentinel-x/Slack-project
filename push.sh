#!/bin/bash

while :
do
    git add --all
    git commit -m "session: $1"
    git push -u origin master 
    sleep 1200
done