using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.interfaces
{
    public interface ICollideable
    {
        public int GetCollisionMask();
        public void ShouldCollideWith(ICollideable partner);
        public void ProcessCollision(ICollideable partner);
        public GameObject GetGameObject();


    }
}
