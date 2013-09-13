using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Gladius.control
{
    public class SoundManager
    {

        public void LoadContent(ContentManager manager)
        {
         
        }


        public void PlaySingleSound(String soundPath)
        {
            SoundEffectInstancePool pool = null;
            if (!m_poolDictionary.TryGetValue(soundPath, out pool))
            {
                pool = new SoundEffectInstancePool(soundPath, m_contentManager);
                m_poolDictionary[soundPath] = pool;
            }

            InstancePoolPair pair = new InstancePoolPair(pool);
            m_currentInstances.Add(pair);
            pair.instance.Play();

        }

        public void Update(GameTime gameTime)
        {
            foreach (InstancePoolPair instance in m_currentInstances)
            {
                if (instance.instance.State == SoundState.Stopped)
                {
                    instance.Free();
                    m_currentInstances.Remove(instance);
                }
            }

        }


        private struct InstancePoolPair
        {
            public InstancePoolPair(SoundEffectInstancePool p)
            {
                pool = p;
                instance = p.Get();
            }
            public void Free()
            {
                pool.Free(instance);
            }
            public SoundEffectInstance instance;
            private SoundEffectInstancePool pool;
        }

        private Dictionary<String, SoundEffectInstancePool> m_poolDictionary = new Dictionary<String, SoundEffectInstancePool>();
        ContentManager m_contentManager;

        private List<InstancePoolPair> m_currentInstances = new List<InstancePoolPair>();
    }




    public class SoundEffectInstancePool
    {
        public SoundEffectInstancePool(String soundPath, ContentManager manager)
        {
            m_effect = manager.Load<SoundEffect>(soundPath);
        }

        public SoundEffectInstance Get()
        {
            if(m_instances.Count == 0)
            {
                m_instances.Push(m_effect.CreateInstance());
            }
            return m_instances.Pop();
        }

        public void Free(SoundEffectInstance instance)
        {
            m_instances.Push(instance);
        }
        SoundEffect m_effect;
        Stack<SoundEffectInstance> m_instances;
    }
}
