# kcg-arena-space-wars

# Asteroids Game

This is a simple Asteroids-style game implemented in C# using OpenTK for rendering.

## Build Instructions

1. Ensure you have .NET 7.0 SDK installed on your system.
2. Clone this repository to your local machine.
3. Open a terminal and navigate to the project root directory.
4. Run the following command to build the project:

## Usage Instructions

1. After building the project, run the game using the following command:

2. How to play the game:

Use arrow keys to move the ship:

Up arrow: Accelerate
Left/Right arrows: Rotate
Spacebar: Shoot
R key: Start/stop recording inputs
P key: Start/stop replaying recorded inputs
ESC key: Close the game

## Running Tests

To run the unit tests, use the following command:

## Project Structure

- GameState: Contains game logic and state management
- GameInput: Handles user input and AI control
- GameCamera: Manages the game camera
- GameRenderer: Handles rendering using OpenGL
- GameMain: The main application that ties everything together
- Tests: Contains unit tests for the game

## Dependencies

This project uses OpenTK for OpenGL bindings and windowing.
