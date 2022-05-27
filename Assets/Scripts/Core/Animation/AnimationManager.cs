using System.Collections.Generic;
using Core.Animation.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Core.Animation
{
    public class AnimationManager : IAnimationManager
    {
        private SortedDictionary<int, Sequence> _subsequences = new SortedDictionary<int, Sequence>();
        
        private Sequence _mainSequence;

        public bool IsPlaying => _mainSequence != null;
        
        public void Enqueue(AnimGroup groupId, Tween tween, float startingTime = 0f)
        {
            var sequence = GetSequence((int)groupId);

            sequence.Insert(startingTime, tween);
        }

        public void EnqueueInterval(AnimGroup groupId, float interval)
        {
            var sequence = GetSequence((int)groupId);

            sequence.AppendInterval(interval);
        }

        public void EnqueueCallback(AnimGroup groupId, TweenCallback callback, float startingTime = 0f)
        {
            var sequence = GetSequence((int)groupId);
            
            sequence.InsertCallback(startingTime, callback);
        }

        public void Play()
        {
            if (_subsequences.Count == 0 || _mainSequence != null)
            {
                return;
            }

            _mainSequence = DOTween.Sequence();
            _mainSequence.onKill += () => { _mainSequence = null; };
            foreach (var sequence in _subsequences.Values)
            {
                _mainSequence.Append(sequence);
            }

            _mainSequence.Play();
        }

        public async UniTask Cancel()
        {
            await UniTask.Yield();

            _mainSequence.Goto(float.MaxValue);
            _mainSequence = null;

            _subsequences.Clear();

            await UniTask.Yield();
        }

        public async UniTask Wait()
        {
            await UniTask.Yield();

            while (IsPlaying)
            {
                await UniTask.WaitUntil(() => !IsPlaying);
                // Wait for 1 more frame because animation manager runs 1 frame behind the game logic
                await UniTask.Yield();
            }
        }
        
        /// <summary>
        /// Get related sequence or create a new one
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private Sequence GetSequence(int groupId)
        {
            if (!_subsequences.TryGetValue(groupId, out Sequence sequence))
            {
                sequence = DOTween.Sequence().Pause();
                sequence.intId = groupId;
                sequence.onKill = () => { _subsequences.Remove(sequence.intId); };
            }

            _subsequences[groupId] = sequence;

            return sequence;
        }

        public void Initialize()
        {
            
        }
    }
}