#!/bin/bash

echo "=== Testing HomeLab Gym API ==="
echo "Base URL: http://localhost:5008"
echo

echo "1. Get all exercises:"
curl -s http://localhost:5008/api/exercises | jq '.'
echo

echo "2. Get exercises filtered by tag 'push':"
curl -s "http://localhost:5008/api/exercises?tags=push" | jq '.'
echo

echo "3. Get exercises filtered by category 'chest':"
curl -s "http://localhost:5008/api/exercises?category=chest" | jq '.'
echo

echo "4. Search exercises by name 'squat':"
curl -s "http://localhost:5008/api/exercises?name=squat" | jq '.'
echo

echo "=== API is running on http://localhost:5008 ==="
echo "Swagger UI: http://localhost:5008/swagger"
