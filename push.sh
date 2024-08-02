#!/bin/bash

    git add --all
    git commit -m "$1"
    git push -u origin master #--force 
    git log | grep commit -m 1

