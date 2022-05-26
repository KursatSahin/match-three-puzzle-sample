using System;
using System.Collections.Generic;
using Game.Gem;
using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "ZMatch/Containers/PrefabContainer", fileName = nameof(PrefabContainer))]
    public class PrefabContainer : SingletonScriptableObject<PrefabContainer>
    {
        public GameObject GemPrefab;
    }
}