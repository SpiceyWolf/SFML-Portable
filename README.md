#
# **SFML Portable - Making SFML.Net more easily portable**

This portable edition of SFML is not the official release and is maintained by :
SpiceyWolf - SpiceyWolf.ArchaicSoft@outlook.com. 

The aim of this library used to make SFML.Net more easily portable to 32/64 bit and cross platform without the use of compiler tags. Previous versions of portable started to incorporate extra tools to aid in game development with .Net all in 1 compact library.

## **Added Game Development Functionality**

A custom SpriteBatch object has been added to reduce a little bit of the render load when using redundant textures. It is expected to function a little similarly to the XNA/MonoGame standard with an SFML flavor. Support includes Draw(Textures + Properties), Draw(String + Properties), and Draw(Drawable) as a catch all so you can main consistent usage of the SpriteBatch. The SpriteBatch will only apply RenderStates to the String and Texture draw commands, and draw drawables using the same methods as if you called RenderTarget.Draw(Sprite/Text/Other) with the exception that you do not have to maintain the RenderTarget.

Multiple Auto(Type) functions exist to make searching for your assets a little bit easier.

These include :
- Music
- Sound
- Texture
- Sprite
- TextureStack
- SpriteStack

The loaders will take a path to where your assets should go (without the extension) and will search through the compatible formats that SFML is capable of loading and return the first one found. The "Stack" variations will return arrays of the objects. Simply give it a directory where your assets should be and make sure they are named as numbers (0+). The loaders are designed to autofill gaps, so if you have 0-50 and skip 3 files and have 54-X then 50-53 will return generated textures to prevent broken arrays. The intension was to make it so if you have many files, you wont have to hunt down for gaps and deal with a bunch of nonsense, let the program run now and you fill it in later when its convenient for you as the developer. Do Note : These autostack loaders look for names that are made of a number then extension, no combinations and having any random REALLY high numbers like 10000.png in a folder with only 4 files will still make it return an array of size 10000.

(The below original readme has been modified to appear cleaner for git and made accurate to the current edition of sfml.net)

#
# **SFML.Net - Simple and Fast Multimedia Library for .Net**


SFML is a simple, fast, cross-platform and object-oriented multimedia API. It provides access to windowing, graphics, and audio.

It is originally written in C++, and this project is its official binding for .Net languages (C#, VB, ...).

## **Authors**

Laurent Gomila - main developer (laurent@sfml-dev.org)

## **Download**

You can get the latest official release on [SFML Website](http://www.sfml-dev.org/download/sfml.net).
You can also get the current development version from the [Git Repository](https://github.com/LaurentGomila/SFML.Net).

## **Learn**

There is no tutorial for SFML.Net, but since it's a binding you can use the C++ resources:

- [The official tutorials](http://www.sfml-dev.org/tutorials/).

- [The online API documentation](http://www.sfml-dev.org/documentation/)

- [The community wiki](https://github.com/LaurentGomila/SFML/wiki/)

- The community forum : ([English](http://en.sfml-dev.org/forums/)/[French](http://fr.sfml-dev.org/forums/))

Of course, you can also find the SFML.Net API documentation in the SDK.

## **Contribute**

SFML and SFML.Net are open-source projects, and they need your help to go on growing and improving.

Don't hesitate to post suggestions or bug reports on the [forum](http://en.sfml-dev.org/forums/).

Submit patches by e-mail, or post new bugs/features requests on the [task tracker](https://github.com/LaurentGomila/SFML.Net/issues/)

You can even fork the project on GitHub, maintain your own version and send us pull requests periodically to merge your work.
