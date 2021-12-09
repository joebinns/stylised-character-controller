# Stylised Character Controller
Based on the approach devised by [Toyful Games](https://www.toyfulgames.com/) for [Very Very Valet](https://www.toyfulgames.com/very-very-valet). In [this video](https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames) from the team's development blog, the various techniques for the movement are outlined and explained. The video also provides snippets of code, though incomplete in places. The source code was not provided by [Toyful Games](https://www.toyfulgames.com/) due to it being tied up in the complex otherworkings of [Very Very Valet](https://www.toyfulgames.com/very-very-valet). This project aims to be an independent pure re-creation of their physics based character controller.

Additional polish inspired by discussions found in [Toyful Games](https://www.toyfulgames.com/) blog posts on [character animations](https://www.toyfulgames.com/blog/character-animations) and [shaders and effects](https://www.toyfulgames.com/blog/deep-dive-shaders-and-effects) are also included. These implementations exist from a personal desire to have them in my own projects. The project makes use of Unity's Universal Render Pipeline (URP) to facilitate some of these graphical features.

## Features
- Physics based character controller, as described [here](https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames).
- Squash and stretch on the character makes motion appear more fluid and bouncy, it is a principle of animation.
- Dithered silhouettes appear on the character when obscured from view, letting the player know where they are at all times.
- Top down blob shadows on characters make 3D platforming feel sharper, and they look great when combined with Unity's inbuilt shadows on the environment.
- Dust particles appear when characters move, making the character feel more alive.

## Installation
Open the project in Unity. Open the demo scene located at PhysicsBasedCharacterController/Assets/Scenes/.

## Contributing
1. Fork the repository.
2. Create a branch for your feature: `git checkout -b my-shiny-feature`.
4. Commit your changes: `git commit -am 'Added my super shiny feature'`.
5. Push to the branch: `git push origin my-shiny-feature`.
6. Submit a pull request.

All contributions big and small are appreciated and encouraged!

## Credits
### Physics based character controller
Credits for the clever character controller idea goes to [Toyful Games](https://www.toyfulgames.com/). A large portion of this system's code is their own as presented in [this video](https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames).

### Blob shadows
The blob shadows use [Nyahoon Games'](http://nyahoon.com/products) asset [Dynamic Shadow Projector for URP](http://nyahoon.com/products/dynamic-shadow-projector).

### All else
Is my own.

## License (MIT License)
See [this page](https://github.com/joebinns/PhysicsBasedCharacterController/blob/main/LICENSE) for more information.
