using UnityEngine;

namespace Waves
{
    public interface IWaveDifficultyModifier
    {
        void ApplyToEnemy(GameObject enemy, float difficultyMultiplier);
    }
}

