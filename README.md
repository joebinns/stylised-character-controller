# Stylised Character Controller
A **stylised physics based character controller** made in **Unity 3D**.

Before you read on, get a hands on **feel** for the project over at [itch.io](https://joebinns.itch.io/stylised-character-controller).
Additionally, watch the [demo](https://youtu.be/3GsXkzbfNBo) and listen to my exploration into [oscillators for game development](https://youtu.be/0gWJDWCvLUY).

[<img alt="Stylised Character Controller: Demo" width="503" src="https://joebinns.com/documents/fake_thumbnails/stylised_character_controller_thumbnail_time.png" />](https://youtu.be/3GsXkzbfNBo)
[<img alt="Oscillators for Game Development" width="503" src="https://joebinns.com/documents/fake_thumbnails/the_joy_of_oscillators_thumbnail_time.png" />](https://youtu.be/0gWJDWCvLUY)

The **character controller** is based on the floating capsule approach devised by [Toyful Games](https://www.toyfulgames.com/) for [Very Very Valet](https://www.toyfulgames.com/very-very-valet). In [a video](https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames) from the team's development blog, the various techniques for the movement are outlined and explained. The video also provides snippets of code, though incomplete in places. The source code was not provided by Toyful Games due to it being tied up in the complex otherworkings of Very Very Valet. This project aims to be a (fanmade) independent pure re-creation of their physics based character controller.

**Additional stylisation** inspired by discussions found in Toyful Games blog posts on [character animations](https://www.toyfulgames.com/blog/character-animations) and [shaders and effects](https://www.toyfulgames.com/blog/deep-dive-shaders-and-effects) are also included. These implementations exist from a personal desire to have them in my own projects. The project makes use of Unity's Universal Render Pipeline (URP) to facilitate some of these graphical features.

## Features
- **Physics based character controller**, as described [here](https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames).
- **Squash and stretch** on the character makes motion appear more fluid and bouncy, it is a principle of animation.
- **Dithered silhouettes** appear on the character when obscured from view, letting the player know where they are at all times.
- **Top down blob shadows** on characters make 3D platforming feel sharper, and they look great when combined with Unity's inbuilt shadows on the environment.
- **Dust particles** appear when characters move, making the character feel more alive.
- **Sound effects** play when the character moves and jumps, to bring everything together.
- **Oscillators**, **Torsional Oscillators** and much more...

## Installation
Open the project in Unity. Open the demo scene located at [*stylised-character-controller/Assets/Scenes/*](https://github.com/joebinns/stylised-character-controller/tree/main/Assets/Scenes).

## Usage
When running the project in the Game view, use <kbd>W</kbd><kbd>A</kbd><kbd>S</kbd><kbd>D</kbd> and <kbd>Space</kbd> to move and jump the character. Press <kbd>F1</kbd> to toggle oscillator gizmos.

## Contributing
1. Fork the repository.
2. Create a branch for your feature: `git checkout -b my-shiny-feature`.
4. Commit your changes: `git commit -am 'Added my super shiny feature'`.
5. Push to the branch: `git push origin my-shiny-feature`.
6. Submit a pull request.

All contributions big and small are appreciated and encouraged!

## Credits
### Physics based character controller [^1]
Credits for the clever character controller goes to [Toyful Games](https://www.toyfulgames.com/). A large portion of this system's code is their own as presented in [this video](https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames).

### Blob shadows
The blob shadows use [Nyahoon Games'](http://nyahoon.com/products) asset [Dynamic Shadow Projector for URP](http://nyahoon.com/products/dynamic-shadow-projector).

### Path Creator
The platforms follow paths using Sebastian Lague's very useful [Path Creator](https://github.com/SebLague/Path-Creator) (a project which I have previously contributed to). This has been adapted to work with [*Oscillator.cs*].

### Sound Effects
The sound effects and audio were kindly created and arranged by [Clara Summerton](mailto:clarasummerton@gmail.com).

### All else
Is [my own](https://joebinns.com/).

## License (MIT License)
See [this page](https://github.com/joebinns/PhysicsBasedCharacterController/blob/main/LICENSE) for more information.

[^1]: **Disclaimer:** This project is fanmade! The quality of the project does not reflect the quality of Toyful Games or their products.
