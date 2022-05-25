using Game.Utils;
using UnityEngine;

namespace Containers
{
    [CreateAssetMenu(menuName = "ZMatch/Containers/BoardSettingsContainer", fileName = nameof(BoardSettingsContainer))]
    public class BoardSettingsContainer : SingletonScriptableObject<BoardSettingsContainer>
    {
        [Header("BoardSettings")]
        public int boardWidth;
        public int boardHeight;
        public int colorCount;
    }
}