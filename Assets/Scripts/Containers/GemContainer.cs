using System;
using System.Collections.Generic;
using Game.Gem;
using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "ZMatch/Containers/GemContainer", fileName = nameof(GemContainer))]
    public class GemContainer : SingletonScriptableObject<GemContainer>
    {
        public List<Sprite> Gems;
    }
}