using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "ZMatch/Containers/BoardSettingsContainer", fileName = nameof(BoardSettingsContainer))]
    public class BoardSettingsContainer : SingletonScriptableObject<BoardSettingsContainer>
    {
        [Header("BoardSettings")]
        public int boardWidth = 8;
        public int boardHeight = 8;
        public int colorCount = 5;
        public int minSolutionLength = 3;
    }
}