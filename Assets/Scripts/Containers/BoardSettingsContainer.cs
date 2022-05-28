using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "ZMatch/Containers/BoardSettingsContainer", fileName = nameof(BoardSettingsContainer))]
    public class BoardSettingsContainer : SingletonScriptableObject<BoardSettingsContainer>
    {
        [Header("BoardSettings")]
        public int BoardWidth = 8;
        public int BoardHeight = 8;
        public int ColorCount = 5;
        public int MinSolutionLength = 3;
    }
}