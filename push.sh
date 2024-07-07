#!/bin/bash

git add --all
git commit -m "session: $1"
git push -u origin main 
sleep 300
