using UnityEngine;

namespace WaveSystem.SceneManagement
{
    [DefaultExecutionOrder(-1)]
    public abstract class SceneInitializer : MonoBehaviour, ISceneInitializer, ISceneDeInitializer
    {

        void Awake()
        {
            InitializeScene();
        }

        public abstract void InitializeScene();

        public virtual void DeInitializeScene()
        {
            // no op
        }
    }
}