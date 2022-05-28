using System.Collections.Generic;
using Common;
using Containers;
using Core.Event;
using Core.Service;
using Game.Gem;
using UnityEngine;

namespace Game
{
    public class BoardLogic
    {
        #region Public Fields
        public GemData [,] Board { get; private set; }
        
        #endregion

        #region Private Fields
        
        private BoardSettingsContainer _boardSettings;
        private BoardViewController _boardViewController;
        private IEventDispatcher _eventDispatcher;
        
        private List<GemData> _destroyedCollector = new List<GemData>();

        private List<int> _gemColors = new List<int>() {0, 1, 2, 3, 4};

        private int[] _previousLeft;
        private int _previousBelow;
        
        private bool _isBoardModified = false;
        
        #endregion

        #region Public Functions

        public BoardLogic(BoardSettingsContainer boardSettings, BoardViewController boardViewController)
        {
            _boardSettings = boardSettings;
            _boardViewController = boardViewController;
            _eventDispatcher = ServiceLocator.Instance.Get<IEventDispatcher>();
        }

        public void Initialize()
        {
            GenerateBoard(_boardSettings.BoardWidth, _boardSettings.BoardHeight);
        }
        
        public void SwapGems(GemData firstGem, GemData secondGem)
        {
            _eventDispatcher.Fire(GameEventType.BlockInputHandler);

            Board[firstGem.Position.y, firstGem.Position.x] = secondGem;
            Board[secondGem.Position.y, secondGem.Position.x] = firstGem;
         
            firstGem.IsSwapped = true;
            secondGem.IsSwapped = true;
            
            (firstGem.Position, secondGem.Position) = (secondGem.Position, firstGem.Position);

            firstGem.IsModified = true;
            secondGem.IsModified = true;
            
            _isBoardModified = true;
        }
        
        public void RollBack(GemData firstGem, GemData secondGem)
        {
            Board[firstGem.Position.y, firstGem.Position.x] = secondGem;
            Board[secondGem.Position.y, secondGem.Position.x] = firstGem;

            (firstGem.Position, secondGem.Position) = (secondGem.Position, firstGem.Position);
        }

        public void FindMatchesAndClear()
        {
            if (!_isBoardModified)
                return;

            var _isRollBackNeeded = true;
            
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
                        _destroyedCollector.Add(gemData);
                    }
                }

                _isRollBackNeeded = _isRollBackNeeded && (gem.IsSwapped && !isMatchFound);
                
                gem.IsSwapped = false;
                gem.IsModified = false;
            }

            if (_isRollBackNeeded)
            {
                RollBack(modifiedGems[0], modifiedGems[1]);
            }

            _isBoardModified = false;
        }
        
        public void SettleBoard()
        {
            HashSet<int> fallingColumns = new HashSet<int>();
            foreach (var destroyedGem in _destroyedCollector)
            {
                fallingColumns.Add(destroyedGem.Position.x);
            }

            if (fallingColumns.Count < 1)
                return;

            foreach (var column in fallingColumns)
            {
                int fallingDistance = 0;

                // Traverse the column from bottom to top
                for (int row = 0; row < _boardSettings.BoardHeight; row++)
                {
                    var gemData = Board[row, column];

                    if (gemData.Destroyed)
                    {
                        Board[row, column] = null;
                        fallingDistance++;
                    }
                    else
                    {
                        if (fallingDistance > 0)
                        {
                            Point landingPosition = new Point( column, row - fallingDistance );
                            
                            Board[landingPosition.y, landingPosition.x] = gemData;
                            Board[row, column] = null;
                            
                            gemData.Position = landingPosition;
                            gemData.IsModified = true;
                        }
                    }
                }
            }
        }

        public void FillEmptySlots()
        {
            HashSet<int> emptyColumns = new HashSet<int>();
            foreach (var destroyedGem in _destroyedCollector)
            {
                emptyColumns.Add(destroyedGem.Position.x);
            }

            if (emptyColumns.Count < 1)
                return;

            foreach (var column in emptyColumns)
            {
                var generatorRowIndex = _boardSettings.BoardHeight;

                for (int row = 0; row < _boardSettings.BoardHeight; row++)
                {
                    var gemData = Board[row, column];

                    if (gemData == null)
                    {
                        gemData = CreateGem(column, row);
                        gemData.Position = new Point(column, generatorRowIndex++);

                        Board[row, column] = gemData;
                        
                        _boardViewController.GenerateGemView(gemData);

                        gemData.Position = new Point(column, row);
                        
                        gemData.IsModified = true;
                    }
                }
            }
            
            _eventDispatcher.Fire(GameEventType.UpdateScore, new ScoreUpdateEvent( _destroyedCollector.Count));
            _destroyedCollector.Clear();
            _isBoardModified = true;
        }

        #endregion

        #region Private Functions

        private void GenerateBoard(int boardWidth, int boardHeight)
        {
            Board = new GemData[boardHeight, boardWidth];
            
            _previousLeft = new int[boardHeight];
            _previousBelow = -1;
            
            for (int row = 0; row < boardHeight; row++)
            {
                for (int col = 0; col < boardWidth; col++)
                {
                    Board[row, col] = CreatePossibleGem(col, row);
                }
            }
            
            _boardViewController.GenerateBoardView();
        }

        private GemData CreatePossibleGem(int col, int row)
        {
            var possibleGemColors = new List<int>();
            possibleGemColors.AddRange(_gemColors);
            possibleGemColors.Remove(_previousLeft[col]);
            possibleGemColors.Remove(_previousBelow);

            var validGemColor = possibleGemColors[Random.Range(0, possibleGemColors.Count)];
            var gem = new GemData(new Point(col, row), (GemColor)validGemColor);
            
            _previousLeft[col] = validGemColor;
            _previousBelow = validGemColor;
            
            return gem;
        }
        
        private GemData CreateGem(int col, int row)
        {
            var validGemColor = _gemColors[Random.Range(0, _gemColors.Count)];
            var gem = new GemData(new Point(col, row), (GemColor)validGemColor);
            
            return gem;
        }
        
        private bool CheckGemForMatch(GemData gem, List<GemData> result)
        {
            var current = gem;
            result.Add(current);
            Point nextPosition;
            GemData next;
            
            List<GemData> horizontalMatches = new List<GemData>{current};
            List<GemData> verticalMatches = new List<GemData>{current};

            if (current.Position.x > 0)
            {
                nextPosition = current.Position + Point.Left;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Left, horizontalMatches);
            }

            if (current.Position.x < _boardSettings.BoardWidth - 1)
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

            if (current.Position.y < _boardSettings.BoardHeight - 1)
            {
                nextPosition = current.Position + Point.Up;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Up, verticalMatches);
            }

            horizontalMatches.RemoveAt(0);
            if (horizontalMatches.Count >= _boardSettings.MinSolutionLength-1)
            {
                result.AddRange(horizontalMatches);
            }

            verticalMatches.RemoveAt(0);
            if (verticalMatches.Count >= _boardSettings.MinSolutionLength-1)
            {
                result.AddRange(verticalMatches);
            }
            
            return result.Count >= _boardSettings.MinSolutionLength;
        }
        
        private void CheckGemForMatchHelper(GemData gem, Point direction, List<GemData> result)
        {
            var current = gem;
            result.Add(current);
            
            var nextPosition = current.Position + direction;
            
            if (nextPosition.x < 0 || nextPosition.x >= _boardSettings.BoardWidth || nextPosition.y < 0 || nextPosition.y >= _boardSettings.BoardHeight)
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
                if (gem.IsModified)
                {
                    result.Add(gem);
                }
            }
            return result;
        }
        #endregion
    }

}