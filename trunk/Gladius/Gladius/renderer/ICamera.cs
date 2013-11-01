using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gladius.gamestatemanagement.screenmanager;

namespace Gladius.renderer
{
    public interface ICamera : IGameComponent,IUpdateable
    {
        Matrix View { get; set; }
        Matrix Projection { get; set; }
        Vector3 Position { get; set; }
        Vector3 Target { get; set; }
        Vector3 TargetDirection { get; set; }
        void Update(GameTime gameTime);
        void UpdateInput(InputState inputState);
        BoundingFrustum Bounds { get; set; }
        Vector3 Velocity { get; set; }
        Vector3 Acceleration { get; set; }
        Vector3 Forward { get; set; }
        Vector3 Up { get; set; }
        Vector3 DesiredPositionOffset { get; set; }
        Vector3 LookAtOffset { get; set; }

        void SnapTo(Vector3 eye, Vector3 lookat);
    }
}
