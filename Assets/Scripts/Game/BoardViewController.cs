﻿using Common;
using Core;
using Core.Input;
using Game.Gem;
using Lean.Pool;
using UnityEngine;
using static Containers.ContainerFacade;

namespace Game
{
    public class BoardViewController
    {
        private BoardLogic _boardLogic;
        private GameObject _gemPrefab;
        private Transform _gemParentContainer;
        
        public BoardViewController(Transform gemParentContainer)
        {
            _gemPrefab = PrefabContainer.GemPrefab;
            _gemParentContainer = gemParentContainer;

            InputHandler.Tap += OnTap;
            InputHandler.Swipe += OnSwipe;
            
            _boardLogic = new BoardLogic(BoardSettings, this);
            _boardLogic.Initialize();
        }

        public void GenerateBoardView()
        {
            foreach (var gemData in _boardLogic.Board)
            {
                GenerateGemView(gemData);
            }
        }

        private void GenerateGemView(GemData gemData)
        {
            if (LeanPool.Spawn(_gemPrefab, _gemParentContainer, true).TryGetComponent(out GemView gemView))
            {
                gemView.SetGemData(gemData);
            }
        }

        private void OnSwipe(GemView gemView, Directions swipeDirection)
        {
            Debug.Log($"Swipe event received ({gemView.Data.Position})" );
        }

        private void OnTap(GemView gemView)
        {
            Debug.Log($"Tap event received ({gemView.Data.Position})");
            
            if (GemView.PreviousSelected == null)
            {
                gemView.Select();
            }
            else
            {
                if (GemView.PreviousSelected.Data.Position == gemView.Data.Position)
                {
                    gemView.Deselect();
                }
                else
                {
                    if (gemView.GetAdjacents().Contains(GemView.PreviousSelected.Data.Position))
                    {
                        _boardLogic.SwapGems(gemView.Data.Position, GemView.PreviousSelected.Data.Position);
                        GemView.PreviousSelected.Deselect();
                    }
                    else
                    {
                        gemView.Select();
                    }
                }
            }
        }

    }
}