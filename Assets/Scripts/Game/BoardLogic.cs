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
        
        private bool _isBoardModified = false;
        private bool _isRollBackNeeded = true;

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
        
        public void SwapGems(Point firstGemPosition, Point secondGemPosition)
        {
            Board[firstGemPosition.y, firstGemPosition.x].Position = secondGemPosition;
            Board[secondGemPosition.y, secondGemPosition.x].Position = firstGemPosition;
            
            var firstGemData = Board[firstGemPosition.y, firstGemPosition.x];
            var secondGemData = Board[secondGemPosition.y, secondGemPosition.x];
            
            Board[firstGemPosition.y, firstGemPosition.x] = secondGemData;
            Board[secondGemPosition.y, secondGemPosition.x] = firstGemData;
            
            firstGemData.IsSwapped = true;
            secondGemData.IsSwapped = true;
            
            _isBoardModified = true;
        }

        private List<GemData> destroyedCollector = new List<GemData>();

        public void FindMatchesAndClear()
        {
            if (!_isBoardModified)
                return;

            _isRollBackNeeded = true;
            _isBoardModified = false;
            
            var modifiedGems = GetModifiedGems();
            
            List<GemData> mathedGems = new List<GemData>();
            
            foreach (var gem in modifiedGems)
            {
                mathedGems.Clear();
                    
                var isMatchFound = CheckGemForMatch(gem, mathedGems);

                if (isMatchFound)
                {
                    foreach (var gemData in mathedGems)
                    {
                        gemData.Destroyed = true;
                        destroyedCollector.Add(gemData);
                    }
                    
                    _isRollBackNeeded = false;
                }
                else
                {
                    // rollback
                }
            }
        }

        private bool CheckGemForMatch(GemData gem, List<GemData> result)
        {
            var current = gem;
            result.Add(current);
            Point nextPosition;
            GemData next;
            
            List<GemData> horizontalMatches = new List<GemData>{current};
            List<GemData> verticalMatches = new List<GemData>{current};;

            if (current.Position.x > 0)
            {
                nextPosition = current.Position + Point.Left;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Left, horizontalMatches);
            }

            if (current.Position.x < _boardSettings.boardWidth - 1)
            {
                nextPosition = current.Position + Point.Right;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Right, horizontalMatches);
            }

            if (current.Position.y > 0)
            {
                nextPosition = current.Position + Point.Down;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Down, verticalMatches);
            }

            if (current.Position.y < _boardSettings.boardHeight - 1)
            {
                nextPosition = current.Position + Point.Up;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Up, verticalMatches);
            }

            horizontalMatches.RemoveAt(0);
            if (horizontalMatches.Count >= _boardSettings.minSolutionLength-1)
            {
                result.AddRange(horizontalMatches);
            }

            verticalMatches.RemoveAt(0);
            if (verticalMatches.Count >= _boardSettings.minSolutionLength-1)
            {
                result.AddRange(verticalMatches);
            }
            
            return result.Count >= _boardSettings.minSolutionLength;
        }
        
        private void CheckGemForMatchHelper(GemData gem, Point direction, List<GemData> result)
        {
            var current = gem;
            result.Add(current);
            
            var nextPosition = current.Position + direction;
            
            if (nextPosition.x < 0 || nextPosition.x >= _boardSettings.boardWidth || nextPosition.y < 0 || nextPosition.y >= _boardSettings.boardHeight)
                return;
            
            var next = Board[nextPosition.y, nextPosition.x];
            
            if (next.Color == current.Color)
            {
                CheckGemForMatchHelper(next, direction, result);
            }
        }


        private List<GemData> GetModifiedGems()
        {
            var result = new List<GemData>();

            foreach (var gem in Board)
            {
                if (gem.IsSwapped)
                {
                    result.Add(gem);
                }
            }
            return result;
        }
    }
}