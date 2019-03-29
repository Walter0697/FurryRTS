# FURRY RTS

- Hermite spline is being implemented as a hinting fairy, to guide player where to go
- script is inside "/Script/fancy/FairyMovement.cs"

- Both rabbits and Tiger have animations
- Rabbits have idle, moving, and dead animations
- Tigers have idle, moving, dead, and crafting(which is running but nothing else match this for our assets) animations
- All animation related scripts are inside "/Script/Animation"

- Physics and steering behaviors are being used by Rabbit units

- Flocking behavior implemented with separation, cohesion, and alignment forces. (with a unit selected, right click on another friendly unit to have them flock around that unit) Implemented in DefaultUnit.cs
- Seek and wander steering behaviors implemented for the leader of the flock.
- Unity's physics engine is used to apply forces to the objects and keep track of their positions and velocities.