using Core.Service.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Core.Animation.Interfaces
{
    public interface IAnimationManager : IService
    {
        /// <summary>
        /// Returns the current state of the animation manager
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Creates a main sequence for all the created sequences in the queue then plays it
        /// </summary>
        void Play();

        /// <summary>
        /// Cancels the current sequence (Sequence will be completed immediately)
        /// </summary>
        UniTask Cancel();

        /// <summary>
        /// Waits for the animations complete
        /// </summary>
        UniTask Wait();
        
        /// <summary>
        /// Appends a tween into swap sequence with starting time
        /// </summary>
        /// <param name="tween">Tween</param>
        /// <param name="startingTime"></param>
        void Enqueue(AnimGroup groupId, Tween tween, float startingTime = 0f);

    }
    
    public enum AnimGroup
    {
        Gravity = 100,
        Destroy = 200,
        Swap = 300,
        Rollback = 400,
    }
}