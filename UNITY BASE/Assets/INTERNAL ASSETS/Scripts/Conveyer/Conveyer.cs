using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

using URandom = UnityEngine.Random;

namespace EnglishKids.SortingTransport
{
    public class Conveyer : ViewObject
    {
        [Serializable]
        private class TweenAnimation
        {
            public float duration;
            public Ease ease;
        }

        //==================================================
        // Fields
        //==================================================

        [Header("Prefabs")]
        [SerializeField] private ViewObject _emptyCell;
        [SerializeField] private ViewObject _startCell;
        [SerializeField] private ViewObject _transportCell;
        [SerializeField] private int _itemsInTransportCell;

        [Header("Tween Animations")]
        [SerializeField] private TweenAnimation _cutSceneTween;
        [SerializeField] private TweenAnimation _gameSceneTween;

        private Spawner _spawner;
        private List<TransportCell> _transportCellList;
        private List<ViewObject> _starsList;
        private int _currentCellIndex;
        private Vector2 _startPosition;
        private bool _wasBuildedCutScene;
        private bool _wasBuildedGameSCene;
        private bool _wasBuildStarScene;
        
        //==================================================
        // Properties
        //==================================================

        public bool IsMoving { get; private set; }

        //==================================================
        // Methods
        //==================================================

        protected override void Init()
        {
            base.Init();
            
            _spawner = Spawner.Instance;

            _transportCellList = new List<TransportCell>();
            _starsList = new List<ViewObject>();

            _startPosition = this.CachedTransform.anchoredPosition;
        }

        public void BuildStartCell()
        {
            if (!_wasBuildedCutScene)            
            {
                // Separate cell
                ViewObject cell = Instantiate(_emptyCell, this.CachedTransform);
                cell.Height = _manager.ReferenceScreenHeight;
                cell.SetActive(true);

                // Target cell
                cell = Instantiate(_startCell, this.CachedTransform);
                cell.SetActive(true);

                // Separate cell
                cell = Instantiate(_emptyCell, this.CachedTransform);
                cell.Height = (_manager.ReferenceScreenHeight - _startCell.Height) * GameConstants.HALF_FACTOR;
                cell.SetActive(true);

                _wasBuildedCutScene = true;
            }
            else
            {
                this.CachedTransform.anchoredPosition = _startPosition;
            }
        }

        public void BuildTransportCell()
        {
            _currentCellIndex = 0;
            
            List<ConveyerItem> targetList = new List<ConveyerItem>();
            FillList(targetList, _manager.LeftDataBlock);
            FillList(targetList, _manager.RightDataBlock);
                        
            int itemsCount = targetList.Count;
            int blocksCount = (itemsCount / _itemsInTransportCell);
            if (itemsCount % _itemsInTransportCell != 0)
                blocksCount++;

            float topOffset = _manager.TopRobotBarOffset * GameConstants.HALF_FACTOR;

            for (int i = 0; i < blocksCount; i++)
            {
                // Transport cell
                TransportCell transportCell;

                if (!_wasBuildedGameSCene)
                {
                    transportCell = Instantiate(_transportCell, this.CachedTransform) as TransportCell;
                    transportCell.Height = _manager.ReferenceScreenHeight - topOffset;
                    transportCell.SetActive(true);

                    _transportCellList.Add(transportCell);
                }
                else
                {
                    transportCell = _transportCellList[i];
                }
                                
                FillTransportCell(transportCell, targetList);
                
                if (!_wasBuildedGameSCene)
                {
                    // Separate cell
                    ViewObject cell = Instantiate(_emptyCell, this.CachedTransform);
                    cell.Height = topOffset;
                    cell.SetActive(true);
                }
            }

            if (_transportCellList.Count > 0)
            {
                _transportCellList[_currentCellIndex].Activate();
                _transportCellList[_currentCellIndex].OnCellIsEmpty -= OnCellIsEmpty;
                _transportCellList[_currentCellIndex].OnCellIsEmpty += OnCellIsEmpty;
            }

            _wasBuildedGameSCene = true;
        }

        public void BuildStarCell(StarsView starsView)
        {
            float topOffset = _manager.TopRobotBarOffset * GameConstants.HALF_FACTOR;
            float sumHeight = _manager.ReferenceScreenHeight - topOffset;
            float cellHeight = sumHeight / _itemsInTransportCell;

            // Main Cell
            ViewObject cell;

            int count = _starsList.Count > 0 ? _starsList.Count : _itemsInTransportCell;
            for (int i = 0; i < count; i++)
            {
                if (!_wasBuildStarScene)
                {
                    cell = Instantiate(_emptyCell, this.CachedTransform);
                    cell.Height = cellHeight;
                    cell.SetActive(true);

                    _starsList.Add(cell);
                }
                else
                    cell = _starsList[i];

                Star star = _spawner.Get(PoolObjectKinds.Star) as Star;
                star.Initialize();
                star.Activate(starsView);
                star.CachedTransform.SetParent(cell.CachedTransform);
                star.CachedTransform.localPosition = Vector3.zero;
                star.CachedTransform.localScale = Vector3.one;
                star.SetActive(true);
            }

            // Separate cell
            cell = Instantiate(_emptyCell, this.CachedTransform);
            cell.Height = topOffset;
            cell.SetActive(true);

            _wasBuildStarScene = true;
        }

        public void Move()
        {
            switch (GameManager.Instance.State)
            {
                case GameStates.CutScene:
                    {
                        float height = (_startCell.CachedTransform.rect.height + _manager.ReferenceScreenHeight) * GameConstants.HALF_FACTOR;
                        Move(_cutSceneTween, height);
                    }
                    break;

                case GameStates.Game:
                    Move(_gameSceneTween, _manager.ReferenceScreenHeight);                    
                    break;

                case GameStates.StarMode:
                    Move(_gameSceneTween, _manager.ReferenceScreenHeight);
                    break;
            }
        }

        private void Move(TweenAnimation animation, float distance)
        {
            this.IsMoving = true;

            Vector3 position = this.CachedTransform.localPosition;
            Vector3 target = position + Vector3.down * distance;

            var secuance = DOTween.Sequence();
            secuance.Append(this.CachedTransform.DOLocalMove(target, animation.duration)).SetEase(animation.ease);
            _audio.PlaySound(Audio.Conveyer);

            secuance.OnComplete(() =>
            {
                _audio.PlaySound(Audio.Conveyer);
                this.IsMoving = false;                                    
            });            
        }

        private void FillList(List<ConveyerItem> list, ColorBlock block)
        {
            foreach (ColorBlock.TransportElement item in block.TransportElements)
            {
                ConveyerItem conveyerElement = _spawner.Get(PoolObjectKinds.ConveyerItem) as ConveyerItem;
                conveyerElement.Initialize();
                conveyerElement.Activate(block, item);

                list.Add(conveyerElement);
            }
        }

        private void FillTransportCell(TransportCell cell, List<ConveyerItem> list)
        {
            int count = 0;

            for (int i = 0; i < _itemsInTransportCell && list.Count > 0; i++)
            {
                int index = URandom.Range(0, list.Count);

                ConveyerItem item = list[index];
                item.CachedTransform.SetParent(cell.CachedTransform);
                item.SetActive(true);

                list.RemoveAt(index);

                count++;
            }

            cell.Configure(count);
        }

        #region Events
        private void OnCellIsEmpty()
        {
            _transportCellList[_currentCellIndex].OnCellIsEmpty -= OnCellIsEmpty;
            _currentCellIndex++;

            if (_currentCellIndex < _transportCellList.Count)
            {
                _transportCellList[_currentCellIndex].Activate();
                _transportCellList[_currentCellIndex].OnCellIsEmpty -= OnCellIsEmpty;
                _transportCellList[_currentCellIndex].OnCellIsEmpty += OnCellIsEmpty;

                Move();
            }
            else
            {
                _manager.State = GameStates.StarMode;
            }
        }
        #endregion
    }
}