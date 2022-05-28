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
        
        /// <summary>
        /// Swap given gems
        /// </summary>
        /// <param name="firstGem">First gem</param>
        /// <param name="secondGem">Second gem</param>
        public void SwapGems(GemData firstGem, GemData secondGem)
        {
            // Block input helper during animations done 
            _eventDispatcher.Fire(GameEventType.BlockInputHandler);

            // Swap gems on array
            Board[firstGem.Position.y, firstGem.Position.x] = secondGem;
            Board[secondGem.Position.y, secondGem.Position.x] = firstGem;
         
            // Mark gems is swapped
            firstGem.IsSwapped = true;
            secondGem.IsSwapped = true;
            
            // Swap gems on position data this refers their position on board array. Also this trigger gem view to move to new position
            (firstGem.Position, secondGem.Position) = (secondGem.Position, firstGem.Position);

            // Mark gems is modified
            firstGem.IsModified = true;
            secondGem.IsModified = true;
            
            // Mark board is modified
            _isBoardModified = true;
        }
        
        /// <summary>
        /// Swap last swapped gems back to their previous positions
        /// </summary>
        /// <param name="firstGem"></param>
        /// <param name="secondGem"></param>
        public void RollBack(GemData firstGem, GemData secondGem)
        {
            // Swap gems on array
            Board[firstGem.Position.y, firstGem.Position.x] = secondGem;
            Board[secondGem.Position.y, secondGem.Position.x] = firstGem;

            // Swap gems on position data this refers their position on board array. Also this trigger gem view to move to new position
            (firstGem.Position, secondGem.Position) = (secondGem.Position, firstGem.Position);
        }

        /// <summary>
        /// Find mathed gems and destroy them.
        /// Starting point of finding matches are gems which were marked as swapped. From these points the function traversing same color adjacents and try to find matches
        /// If match found at all matched gems destroy at the end of function.  
        /// </summary>
        public void FindMatchesAndClear()
        {
            if (!_isBoardModified)
                return;

            var _isRollBackNeeded = true;
            
            // Get modified gems
            var modifiedGems = GetModifiedGems();
            
            List<GemData> mathedGems = new List<GemData>();
            
            // Check each modified gem and its adjacents for matches
            foreach (var gem in modifiedGems)
            {
                mathedGems.Clear();
                
                // Check this modified gem and its adjacents for matches
                var isMatchFound = CheckGemForMatch(gem, mathedGems);
                
                // If match found at all gems on mathedGems list destroy and collect from _destroyedCollector
                if (isMatchFound)
                {
                    foreach (var gemData in mathedGems)
                    {
                        // Mark gem as destroyed
                        gemData.Destroyed = true;
                        _destroyedCollector.Add(gemData);
                    }
                }

                // If match not found and gem is swapped and also other modified gem are on same situation then roll back gem to its previous position
                _isRollBackNeeded = _isRollBackNeeded && (gem.IsSwapped && !isMatchFound);
                
                // 
                gem.IsSwapped = false;
                gem.IsModified = false;
            }

            // If roll back needed then roll back modified gems
            if (_isRollBackNeeded)
            {
                RollBack(modifiedGems[0], modifiedGems[1]);
            }

            // Marked board as not modified
            _isBoardModified = false;
        }
        
        /// <summary>
        /// After matches are found and destroyed, this function make all pieces fall to destroyed down-adjacents position
        /// </summary>
        public void SettleBoard()
        {
            // Get modified columns
            HashSet<int> fallingColumns = new HashSet<int>();
            foreach (var destroyedGem in _destroyedCollector)
            {
                fallingColumns.Add(destroyedGem.Position.x);
            }
            
            // Check if modified columns are empty
            if (fallingColumns.Count < 1)
                return;

            foreach (var column in fallingColumns)
            {
                int fallingDistance = 0;

                // Traverse the column from bottom row to top row
                for (int row = 0; row < _boardSettings.BoardHeight; row++)
                {
                    var gemData = Board[row, column];
                    
                    // If gem is destroyed then increment falling distance and set board array null
                    if (gemData.Destroyed)
                    {
                        Board[row, column] = null;
                        fallingDistance++;
                    }
                    // If gem is not destroyed
                    else
                    {
                        
                        if (fallingDistance > 0)
                        {
                            // Calculate landing position of gem after falling complete
                            Point landingPosition = new Point( column, row - fallingDistance );
                            
                            // Update relevant slot of board array with this gemData 
                            Board[landingPosition.y, landingPosition.x] = gemData;
                            
                            // Set null to board slot which holds gemData previously
                            Board[row, column] = null;
                            
                            // Update gem data with landing position. Also this trigger gem view to move to new the position
                            gemData.Position = landingPosition;
                            
                            // Mark gem as modified 
                            gemData.IsModified = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill all empty slots with new gems
        /// This function traverse only empty columns from bottom row to top row, and fill empty slots at the top of column 
        /// </summary>
        public void FillEmptySlots()
        {
            // Get modified columns
            HashSet<int> emptyColumns = new HashSet<int>();
            foreach (var destroyedGem in _destroyedCollector)
            {
                emptyColumns.Add(destroyedGem.Position.x);
            }

            // Check if modified columns are empty
            if (emptyColumns.Count < 1)
                return;
            
            foreach (var column in emptyColumns)
            {
                // Set starting generation position row index  
                var generatorRowIndex = _boardSettings.BoardHeight;
                
                // Traverse the column from bottom row to top row
                for (int row = 0; row < _boardSettings.BoardHeight; row++)
                {
                    var gemData = Board[row, column];

                    // If gemData is is null create a new gem data for this empty slot
                    if (gemData == null)
                    {
                        // Create a new gemData for this position
                        gemData = CreateGem(column, row);
                        
                        // Change gemData position to same column but generator row. This means gem view spawn the top of same column and will fall down to this position
                        gemData.Position = new Point(column, generatorRowIndex++);

                        // Created gemData set it to board array
                        Board[row, column] = gemData;
                        
                        // Spawn gem wiew with this gemData
                        _boardViewController.GenerateGemView(gemData);

                        // Update gem data with original position. Also this trigger gem view to move to new the position
                        gemData.Position = new Point(column, row);
                        
                        // Mark gem as modified
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

        /// <summary>
        /// Generate board randomly
        /// </summary>
        /// <param name="boardWidth">Width size of board</param>
        /// <param name="boardHeight">Height size of board</param>
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

        /// <summary>
        /// Create a gem with given position and random color, but not the same as the left previous and below gems color
        /// </summary>
        /// <param name="col">Column</param>
        /// <param name="row">Row</param>
        /// <returns>Created gemData instance</returns>
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
        
        /// <summary>
        /// Creates a new gem with given position and random color
        /// </summary>
        /// <param name="col">Column</param>
        /// <param name="row">Row</param>
        /// <returns>Created gemData instance</returns>
        private GemData CreateGem(int col, int row)
        {
            var validGemColor = _gemColors[Random.Range(0, _gemColors.Count)];
            var gem = new GemData(new Point(col, row), (GemColor)validGemColor);
            
            return gem;
        }
        
        /// <summary>
        /// Check gem and its adjacents for possible match
        /// </summary>
        /// <param name="gem">GemData instance</param>
        /// <param name="result">List of matched gems which is using for returning parameter</param>
        /// <returns>returns TRUE if matched gems count more than minimum solution count, else FALSE</returns>
        private bool CheckGemForMatch(GemData gem, List<GemData> result)
        {
            var current = gem;
            result.Add(current);
            Point nextPosition;
            GemData next;
            
            List<GemData> horizontalMatches = new List<GemData>{current};
            List<GemData> verticalMatches = new List<GemData>{current};

            // Check up left for horizontal matches
            if (current.Position.x > 0)
            {
                nextPosition = current.Position + Point.Left;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Left, horizontalMatches);
            }

            // Check up right for horizontal matches
            if (current.Position.x < _boardSettings.BoardWidth - 1)
            {
                nextPosition = current.Position + Point.Right;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Right, horizontalMatches);
            }

            // Check down adjacents for vertical matches
            if (current.Position.y > 0)
            {
                nextPosition = current.Position + Point.Down;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Down, verticalMatches);
            }

            // Check up adjacents for vertical matches 
            if (current.Position.y < _boardSettings.BoardHeight - 1)
            {
                nextPosition = current.Position + Point.Up;
                next = Board[nextPosition.y, nextPosition.x];
                if(next.Color == current.Color)
                    CheckGemForMatchHelper(next, Point.Up, verticalMatches);
            }

            // remove first gem from the list because of avoiding duplicates current gem
            horizontalMatches.RemoveAt(0);
            if (horizontalMatches.Count >= _boardSettings.MinSolutionLength-1)
            {
                result.AddRange(horizontalMatches);
            }
            
            // remove first gem from the list because of avoiding duplicate current gem
            verticalMatches.RemoveAt(0);
            if (verticalMatches.Count >= _boardSettings.MinSolutionLength-1)
            {
                result.AddRange(verticalMatches);
            }
            
            return result.Count >= _boardSettings.MinSolutionLength;
        }
        
        /// <summary>
        /// Recursive helper function for CheckGemForMatch.
        /// This function will check the gem at the given position and adjacent gems on only one direction of the same color.
        /// </summary>
        /// <param name="gem"></param>
        /// <param name="direction"></param>
        /// <param name="result"></param>
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
        
        /// <summary>
        /// Get modified gems on board
        /// </summary>
        /// <returns>List of gemData</returns>
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