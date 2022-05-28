using Common;
using Core.Input;
using Core.Service.Interfaces;
using Game.Gem;
using Lean.Pool;
using UnityEngine;
using static Containers.ContainerFacade;

namespace Game
{
    public class BoardViewController : ITearDownService
    {
        #region Private Fields
        
        private BoardLogic _boardLogic;
        private GameObject _gemPrefab;
        private Transform _gemParentContainer;
        
        #endregion

        #region Public Functions

        public BoardViewController(Transform gemParentContainer)
        {
            _gemPrefab = PrefabContainer.GemPrefab;
            _gemParentContainer = gemParentContainer;

            InputHandler.Tap += OnTap;
            InputHandler.Swipe += OnSwipe;

            _boardLogic = new BoardLogic(BoardSettings, this);
            _boardLogic.Initialize();
        }

        public void TearDown()
        {
            InputHandler.Tap -= OnTap;
            InputHandler.Swipe -= OnSwipe;
        }

        /// <summary>
        /// Generate gem view as much as board size
        /// </summary>
        public void GenerateBoardView()
        {
            foreach (var gemData in _boardLogic.Board)
            {
                GenerateGemView(gemData);
            }
        }

        /// <summary>
        /// Spawns a gem and sets its data
        /// </summary>
        /// <param name="gemData"></param>
        public void GenerateGemView(GemData gemData)
        {
            if (LeanPool.Spawn(_gemPrefab, _gemParentContainer, true).TryGetComponent(out GemView gemView))
            {
                gemView.SetGemData(gemData);
            }
        }

        /// <summary>
        /// This function runs logic functions below in execution order
        ///     1-FindMatchAndClear
        ///     2-SettleBoard
        ///     3-FillEmptySlots
        /// </summary>
        public void RunLogic()
        {
            // We need to run logic in this order to ensure chain of responsibility
            _boardLogic.FindMatchesAndClear();
            _boardLogic.SettleBoard();
            _boardLogic.FillEmptySlots();
        }
        
        #endregion

        #region Private Functions

        /// <summary>
        /// OnSwipe event handler, returns the chip view where the swipe started and swipe direction 
        /// </summary>
        /// <param name="gemView">Collided gem view</param>
        /// <param name="swipeDirection">Swipe direction</param>
        private void OnSwipe(GemView gemView, Point swipeDirection)
        {
            Debug.Log($"Swipe event received ({gemView.Data.Position}) with direction => ({swipeDirection})");

            if (GemView.PreviousSelected != null)
            {
                GemView.PreviousSelected.Deselect();
            }

            // Calculate new gem position
            Point toPosition = gemView.Data.Position + swipeDirection;

            // check if the new position is valid or not -is there any gem at position-
            if (!IsPositionValid(toPosition)) return;

            // swap the gems at starting position of swipe and its neighbour which is found with swipe direction
            _boardLogic.SwapGems(gemView.Data, _boardLogic.Board[toPosition.y, toPosition.x]);
        }

        /// <summary>
        /// checks if the position is within the board boundaries
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsPositionValid(Point position)
        {
            if (position.x < 0 || position.x >= BoardSettings.BoardWidth)
            {
                return false;
            }

            if (position.y < 0 || position.y >= BoardSettings.BoardHeight)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// OnTap event handler, returns the tapped chip view
        /// </summary>
        /// <param name="gemView"></param>
        private void OnTap(GemView gemView)
        {
            Debug.Log($"Tap event received ({gemView.Data.Position})");

            // if there is not any selected gem, select it
            if (GemView.PreviousSelected == null)
            {
                gemView.Select();
            }
            else
            {
                // if the tapped gem is already selected, deselect it
                if (GemView.PreviousSelected.Data.Position == gemView.Data.Position)
                {
                    gemView.Deselect();
                }
                else
                {
                    // if there is already a different selected gem and the tapped gem is its neighbour, swap them
                    if (gemView.GetAdjacents().Contains(GemView.PreviousSelected.Data.Position))
                    {
                        _boardLogic.SwapGems(gemView.Data, GemView.PreviousSelected.Data);
                        GemView.PreviousSelected.Deselect();
                    }
                    // if there is already a different selected gem and the tapped gem is not its neighbour, deselect previous one and select this
                    else
                    {
                        gemView.Select();
                    }
                }
            }
        }
        
        #endregion
        
    }
}