Details of castle spell
  * Can only be cast by magicians
  * travels at high speed in direct line or in targetted direction
  * collision area mapped as sphere
  * ignores any contacts apart from those with the ground.
  * if a contact with the ground occurs then a castle is formed if :
    * ground is 'solid' (not water)
    * space around contact point must be large enough to contain castle.
    * must not contains obstructing objects (boulders etc)
    * must not be within a given distance of any enemy castle.

  * see castle restrictions on main object definition
  * casting the castle spell on an existing castle will increase it to the next level if conditions are met.
  * mana cost
    * mana cost changes with level of castle
  * cooldown time
