#!/bin/bash


while :
do
    git add --all
    git commit -m "session: $1"
    git push -u origin master 
    sleep 3600    
done