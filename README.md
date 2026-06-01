# Vision-Pro-Volume-Interactions
Unity PolySpatial workshop exploring Apple Vision Pro volume spaces. Learn how to move objects from volumetric windows into open spatial environments. Created for ACAD217 (USC Extended Reality Design Class).

---

## Part 1 | Basic Setup for Pulling Objects out of Portal

1. Open the **Portal** sample scene
2. Add the **cube prefab** from the PolySpatial Sample Scene (don't nest it inside anything)
3. Add the **manager** at the same hierarchy level as the cube
4. Disable **`Object Volume Limiter (Script)`** on the cube

> If you leave `Object Volume Limiter` enabled, the cube will snap back to its original position whenever you try to move it.

At this point you can move the cube in and out of the portal, but it will still return to its starting position when you let go. Part 2 covers how to fix that.

---

## Part 2 | Stopping the Object from Snapping Back

The snap-back behavior comes from a script,  `Bounded Object Behavior`. Instead of disabling the component, you'll replace the script with a custom version that adds a `disableReturn` toggle.

### 1. Replace the script

Replace `Assets/ExampleAssets/Scripts/Bounded/BoundedObjectBehavior.cs` with the modified script provided in the repo


### 2. Enable the toggle

With the script replaced, select the cube in the hierarchy and check **`Disable Return`** in the Inspector under the `Bounded Object Behavior` component. The object will now stay wherever you leave it after releasing.

---

## Notes

### Custom materials

If you assign a custom material to the cube, it will revert when you interact with it. This is because the `Bounded Object Behavior` script handles both a default and a hover material and swaps between them automatically. To use your own materials, assign them to the **`Default Material`** and **`Selected Material`** fields on the component instead of setting them directly on the mesh.
