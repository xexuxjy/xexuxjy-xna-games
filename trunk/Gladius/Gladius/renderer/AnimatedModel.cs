using System;
using SkinnedModel;

namespace Gladius.renderer
{
    public class AnimatedModel 
    {
        public AnimatedModel(SkinningData skinningData)
        {
            m_skinningData = skinningData;
        }

        private SkinningData m_skinningData;
    }
}
