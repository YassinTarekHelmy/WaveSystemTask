using UnityEngine;
using WaveSystem.SceneManagement;
using WaveSystem.Utilities;

namespace WaveSystem.SceneManagement
{
    public class GamePlaySceneInitializer : SceneInitializer
    {
        public override void InitializeScene()
        {
            // Initialize the ObjectPool

            ObjectPool.Instance.InitializePool();

            // Load Prefabs and pre-warm the ObjectPool for each enemy type

            GameObject[] enemyPrefabsToPreWarm = Resources.LoadAll<GameObject>("Prefabs/Enemies");
            
            Debug.Log($"Pre-warming {enemyPrefabsToPreWarm.Length} enemy prefabs in the ObjectPool.");

            foreach (GameObject enemyPrefab in enemyPrefabsToPreWarm)
            {
                ObjectPool.Instance.PreWarm(enemyPrefab, 150);
            }
        }
    }
}