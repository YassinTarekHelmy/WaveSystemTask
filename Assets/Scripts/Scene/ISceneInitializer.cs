using UnityEngine;

namespace WaveSystem.SceneManagement
{
    public interface ISceneInitializer
    {
        void InitializeScene();
    }

    public interface ISceneDeInitializer
    {
        void DeInitializeScene();
    }
}
