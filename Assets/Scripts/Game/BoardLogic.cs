using System.Collections.Generic;
using Common;
using Containers;
using Core.Utils;
using Game.Gem;
using UnityEngine;

namespace Game
{
    public class BoardLogic
    {
        public GemData [,] Board { get; private set; }
        
        private BoardSettingsContainer _boardSettings;
        private BoardViewController _boardViewController;

        private List<int> _gemColors = new List<int>() {0, 1, 2, 3, 4};

        private int[] previousLeft;
        private int previousBelow;

        public BoardLogic(BoardSettingsContainer boardSettings, BoardViewController boardViewController)
        {
            _boardSettings = boardSettings;
            _boardViewController = boardViewController;
        }

        public void Initialize()
        {
            GenerateBoard(_boardSettings.boardWidth, _boardSettings.boardHeight);
        }
        
        private void GenerateBoard(int boardWidth, int boardHeight)
        {
            Board = new GemData[boardHeight, boardWidth];
            
            previousLeft = new int[boardHeight];
            previousBelow = -1;
            
            for (int row = 0; row < boardHeight; row++)
            {
                for (int col = 0; col < boardWidth; col++)
                {
                    Board[row, col] = CreateGem(col, row);
                }
            }
            
            _boardViewController.GenerateBoardView();
        }

        private GemData CreateGem(int col, int row)
        {
            var possibleGemColors = new List<int>();
            possibleGemColors.AddRange(_gemColors);
            possibleGemColors.Remove(previousLeft[col]);
            possibleGemColors.Remove(previousBelow);

            var validGemColor = possibleGemColors[Random.Range(0, possibleGemColors.Count)];
            var gem = new GemData(new Point(col, row), (GemColor)validGemColor);
            
            previousLeft[col] = validGemColor;
            previousBelow = validGemColor;
            
            return gem;
        }
    }
}