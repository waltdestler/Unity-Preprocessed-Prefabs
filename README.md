# Unity Prefab Pre-Processing


"Pre-processing" prefabs is a relatively easy editor-extension technique that can be used to speed up content-creation pipelines, reduce code complexity, and even improve performance. This Github project shows how to use this technique in one simple example scenario, and this README contains a description of both *why* this technique is useful and *how* to implement it.

Although this Github project uses the Unity3D game engine to showcase this technique, many other game engines also have the concept of a "prefab", and this technique should work with any such engine so long as it exposes the scripting APIs.

## Why is pre-processing prefabs useful? ##

To best illustrate *why* this technique is useful, let's suppose that we're writing a game for the Apple iPad, and we want our game to be available for both modern devices and older devices. As you may know, modern iPad screens have a much higher pixel resolution than older iPad screens as well as having more RAM available for use by apps.

Because the newer iPads have high-resolution screens, we of course want a lot of high-resolution textures to make our game look stunningly beautiful on those new iPads. But those high-def textures won't work on the older iPads because they use too much RAM (and might even look bad when rendered at lower resolution).

The solution is of course to create lower-resolution versions of those textures and, on older iPads, use those instead of the high-res textures. But this creates a new problem, which is, how does Unity know which textures to use? Unfortunately, Unity has no built-in mechanism for picking the right-resolution texture depending on what kind of iPad it's running on, so we have to come up with our own solution. A couple obvious solutions that come to mind are:

1. Create two versions of every texture-using prefab by hand, one version which uses the low-res textures and another which uses the high-res textures.
2. Write a special script that we attach to the prefab that sets the appropriate low-res or high-res texture at runtime depending on which iPad the game is running on.

The problem with the first solution is that it doubles the amount of work it takes us developers to create the prefabs since we're now creating two versions of each.

And the second solution also has problems. For example, a special script that gets run on instantiation of the prefab would add unwanted complexity to our runtime code (now we have to worry about *when* our new script gets run relative to other scripts that it might affect) that may cause bugs and maintanence issues. Not only does it add complexity, but it makes instantiating these prefabs a bit slower, so it hurts performance. And even worse, if our script contains references to both the low-res and high-res textures, then Unity will actually load them *both* into RAM, which makes loading even slower and uses even more RAM than if we always used the high-res texture!

So if neither of these two options are practical, is there a 3rd option? Well, what if we do something *like* first option and have two versions of each prefab, but instead of creating the two versions by hand, we only create one by hand (say, using the low-res textures) and have the computer create the other automatically (swapping out low-res textures for high-res textures)? If we could do that, then using two prefabs wouldn't create double the work, and we wouldn't need a special runtime script that makes our code more complicated and hurts performance.

This idea of having the computer create the high-res prefab automatically based on the low-res prefab is an example of what I call "prefab pre-processing".

## How a developer uses the prefab pre-processor ##

Although you can of course build your own pre-processor tool however you like to suit your own needs, for this example I have chosen a simple developer workflow:

1. The developer creates a "workbench" Unity scene file in which they create one or more game objects that they will want to create prefabs for. (In this example Unity project, this is the `PrefabBuilder` scene.)
2. The developer selects the object for which they want to create low-res and high-res prefabs. (The `kitten` object in the example scene.)
3. The developer selects the `PrefabBuilder` menu and then clicks `Build Prefabs From Selected`.
4. The prefab pre-processor script automatically creates two prefabs in the Resources folder, one named after the selected object (`kitten`), and one with an `_hd` extension (`kitten_hd`).
5. When running the game, the game will automatically instantiate the correct prefab (`kitten` or `kitten_hd`) depending on what kind of iPad the game is running on.

## How to implement a prefab pre-processor ##

Implementing a prefab pre-processor is fairly straightforward, though you'll need to know the correct programming APIs to call for you game engine. Example source code for the Unity3D game engine is located in [PrefabBuilder.cs](https://github.com/waltdestler/Unity-Preprocessed-Prefabs/blob/master/Assets/Editor/PrefabBuilder.cs).

At a high level, the steps for pre-processing and creating a prefab goes like this:

1. Handle a command by the developer to create the prefabs. The example code responds to a menu command and then gets the currently-selected game object from Unity.
2. Create a copy of the object in the scene so that we can modify it without screwing up the original. (In Unity we simply call `Instantiate` which creates a copy of the object in the scene view.
3. For every texture in that new copy... (This example uses the `GetComponentsInChildren` method to find all of the `SpriteRenderer` components.)
	4. Find out its file path. (In Unity we can use the `AssetDatabase.GetAssetPath` method.)
	5. Determine the file path of the `_hd` version. For example, if the path of the original texture is `Assets/kitten.jpg`, then the path of the high-res texture will be `Assets/kitten_hd.jpg`. (In Unity we can use the `Path` class to help us parse and modify the file path.)
	6. If there is indeed a `_hd` version of the texture at that path, then load it and swap out (in the copied object) the original texture with the new texture.
7. Create a prefab from the copied object. (In Unity we can create a prefab by using the `PrefabUtility.CreatePrefab` method.)
8. Delete the copied object, leaving just the original (and the newly-created prefab) behind.

Modifying the texture as in the steps above is just one example of how you can pre-process a prefab. Indeed, you can do essentially any kind of pre-processing you want to a prefab. Just about anything your code can do at runtime to an object it can do while pre-processing a prefab.

## Testing the example project ##

This example project contains two scene files:

* `PrefabBuilder` is the "workbench" scene in which the developer creates the game object that they want to create prefabs from. To test the example prefab pre-processor, select the `kitten` object, click the `PrefabBuilder` menu, and then click `Build Prefabs From Selected`. Two prefabs, `kitten` and `kitten_hd` will be created in the Resources folder.
* `Test` is a very simple test scene containing a user interface with two buttons: one that instantiates the `kitten` prefab, and one that instantiates the `kitten_hd` prefab. You can clearly see that the `kitten_hd` version is the much sharper of the two.

## Other uses for prefab pre-processing ##

Swapping out textures for different-resolution variants is just one (albeit very useful) use for prefab pre-processing. There are doubtless thousands of uses for prefab pre-processing, but here are just a few:

* Adding/removing scripts from objects depending on the build platform. (For example, a pre-processor could strip out `TouchInputHandler` scripts on PC platforms while stripping out `MouseInputHandler` on mobile platforms.)
* Remove testing objects and scripts that shouldn't be present in release builds.
* Create different-colored variants of a character for each player in a multiplayer game.
* Remove enemies that shouldn't be present on lower-difficulty versions of a level.
* Pre-calculate expensive computations that are too slow to calculate at runtime.