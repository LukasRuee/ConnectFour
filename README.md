# Connect Four (Unity WebGL)
A lightweight, browser-based implementation of Connect Four, built with Unity and exported as a WebGL project. This was created as an experiment in Unity’s WebGL pipeline.
Live demo: https://lukasruee.github.io/ConnectFour/

Connect Four is a classic two-player game: each player alternately drops a token into one of seven columns, trying to get four in a row (horizontally, vertically, or diagonally). This version is built in Unity, exported to WebGL, and hosted via GitHub Pages.

This project is primarily a demo / proof-of-concept to:
- Explore Unity’s WebGL workflow
- Understand performance trade-offs in WebGL builds
- Experiment with simple game logic in a web context

## How to Play
- Open the live demo
- Click on a column to drop your token.
- Players alternate turns.
- Try to connect four tokens in a row — horizontally, vertically, or diagonally.
- The game detects a win or a draw.
- Reset whenever you want.

## Game Logic
- Grid of 7 × 6 (columns × rows)
- Turn-based: two players
- Win detection in four directions: horizontal, vertical, two diagonals

## Technical Details
- Engine: Unity (2021.3.45f2)
- Target Platform: WebGL
