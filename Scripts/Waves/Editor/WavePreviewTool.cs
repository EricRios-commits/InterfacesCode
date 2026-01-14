#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Waves
{
    [CustomEditor(typeof(WaveGenerationProfile))]
    public class WavePreviewTool : Editor
    {
        private int previewWaveNumber = 1;
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Wave Preview", EditorStyles.boldLabel);
            
            previewWaveNumber = EditorGUILayout.IntField("Wave Number", previewWaveNumber);
            
            if (GUILayout.Button("Generate Preview"))
            {
                var profile = target as WaveGenerationProfile;
                var generator = new StandardWaveGenerator();
                var waveData = generator.PreviewWave(previewWaveNumber, profile);
                Debug.Log($"=== WAVE {waveData.WaveNumber} PREVIEW ===");
                Debug.Log($"Type: {waveData.Type}");
                Debug.Log($"Difficulty: {waveData.DifficultyMultiplier:F2}x");
                Debug.Log($"Total Enemies: {waveData.TotalEnemyCount}");
                Debug.Log($"Spawn Interval: {waveData.SpawnInterval:F2}s");
                Debug.Log($"Composition:");
                foreach (var entry in waveData.EnemiesToSpawn)
                {
                    string enemyName = entry.EnemyPrefab != null ? entry.EnemyPrefab.name : "NULL";
                    Debug.Log($"  - {entry.Count}x {enemyName} ({entry.Tier})");
                }
            }
            if (GUILayout.Button("Preview Waves 1-20"))
            {
                var profile = target as WaveGenerationProfile;
                var generator = new StandardWaveGenerator();
                
                Debug.Log("=== WAVE PREVIEW 1-20 ===");
                for (int i = 1; i <= 20; i++)
                {
                    var waveData = generator.PreviewWave(i, profile);
                    Debug.Log($"Wave {i}: {waveData.Type} | {waveData.TotalEnemyCount} enemies | {waveData.DifficultyMultiplier:F2}x difficulty");
                }
            }
        }
    }
}
#endif

