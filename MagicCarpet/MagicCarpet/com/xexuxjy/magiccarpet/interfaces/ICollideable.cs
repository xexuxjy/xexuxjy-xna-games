using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.interfaces
{
    public interface ICollideable
    {
        int GetCollisionMask();
        bool ShouldCollideWith(ICollideable partner);
        void ProcessCollision(ICollideable partner);
        GameObject GetGameObject();
    }
}
