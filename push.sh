#!/bin/bash

git add --all
git commit -m "session: $1"
git push -u origin master 
sleep 300
