using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.terrain;

namespace com.xexuxjy.magiccarpet.interfaces
{
    public interface ICamera : IUpdateable , IGameComponent
    {
        float Pitch {get;set;}
        float Yaw {get;set;}
        float Distance { get; set; }
        Vector3 Position { get; set; }
        Matrix View { get; set; }
        Matrix Projection{ get; set; }
        bool IsInViewFrustum(WorldObject worldObject);
    }
}
