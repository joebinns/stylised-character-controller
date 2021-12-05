# Physics Based Character Controller
Based on the approach devised by [Toyful Games](https://www.toyfulgames.com/) for [Very Very Valet](https://www.toyfulgames.com/very-very-valet). In [this video](https://www.youtube.com/watch?v=qdskE8PJy6Q&ab_channel=ToyfulGames) from the team's development blog, the various techniques for the movement are outlined and explained. The video also provides snippets of code, though incomplete in places. The source code was not provided by Toyful Games due to it being tied up in the complex otherworkings of Very Very Valet. This project aims to be an independent pure re-creation of the physics based character controller.

Some additional polish inspired by discussions found in Toyful Games blog posts on [character animations](https://www.toyfulgames.com/blog/character-animations) and [shaders and effects](https://www.toyfulgames.com/blog/deep-dive-shaders-and-effects) are also included. Tese implementations exist from a personal desire to have them in my own projects. I have made them optional, as some implementations are rather rough and generalised. The implemented features include blob shadows on characters (makes platforming much nicer), silhouettes on characters when obscured from view, dust particles and squash and stretch (using Unity's transform scale).

The project makes use of Unity's Universal Render Pipeline (URP) to facilitate some of these graphical features.

All contributions big and small are welcome and encouraged!
