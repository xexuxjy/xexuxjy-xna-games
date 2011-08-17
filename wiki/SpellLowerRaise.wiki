Details for lower / raise ground
  * Instant cast utility spell
  * Facing / Targetted
  * Highspeed sphere collision
  * collision only processed if it's a terrain collision
    * terrain is raised and lowered by X (with clamps)
    * height changes have world max & min bounds
    * raising land cannot change the height of any square that contains a castle
    * Height change takes place over Y seconds
    * height change brush will be simple point,radius and lerp to height