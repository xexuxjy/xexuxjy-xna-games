using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using GameStateManagement;

namespace Gladius.renderer.particles
{
    public class ParticleManager 
    {
        public void Init(ContentManager content,GameScreen gameScreen)
        {
            // This sample uses five different particle systems.

            List<Projectile> projectiles = new List<Projectile>();
            TimeSpan timeToNextProjectile = TimeSpan.Zero;



            // Construct our particle system components.
            //explosionParticles = new ParticleSystem(gameScreen,content, "ExplosionSettings");
            //explosionSmokeParticles = new ParticleSystem(gameScreen, content, "ExplosionSmokeSettings");
            //projectileTrailParticles = new ParticleSystem(gameScreen, content, "ProjectileTrailSettings");
            //smokePlumeParticles = new ParticleSystem(gameScreen, content, "SmokePlumeSettings");
            //fireParticles = new ParticleSystem(gameScreen, content, "FireSettings");
            rainParticles = new RainParticleSystem(gameScreen, "RainSettings");

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            rainParticles.DrawOrder = 100;
            //smokePlumeParticles.DrawOrder = 100;
            //explosionSmokeParticles.DrawOrder = 200;
            //projectileTrailParticles.DrawOrder = 300;
            //explosionParticles.DrawOrder = 400;
            //fireParticles.DrawOrder = 500;

            // Register the particle system components.
            //Components.Add(explosionParticles);
            //Components.Add(explosionSmokeParticles);
            //Components.Add(projectileTrailParticles);
            //Components.Add(smokePlumeParticles);
            //Components.Add(fireParticles);


        }
        ParticleSystem rainParticles;
        //ParticleSystem explosionParticles;
        //ParticleSystem explosionSmokeParticles;
        //ParticleSystem projectileTrailParticles;
        //ParticleSystem smokePlumeParticles;
        //ParticleSystem fireParticles;

    }
}
