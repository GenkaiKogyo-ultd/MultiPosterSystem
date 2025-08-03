using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using VRC.SDK3.Video.Components;

namespace Wacky612.MultiPosterSystem
{
    public class MultiPosterSystemBuildProcessor : IProcessSceneWithReport
    {
        public int callbackOrder => 0;
        
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            foreach (var mps in Object.FindObjectsOfType<MultiPosterSystem>(true))
            {
                var dynamicPosters = new SerializedObject(mps.GetComponentInChildren<DynamicPosters>());
                var videoPlayer    = new SerializedObject(mps.GetComponentInChildren<VRCUnityVideoPlayer>());

                var oldRenderTexture = dynamicPosters.FindProperty("_renderTexture").objectReferenceValue;
                var newRenderTexture = new RenderTexture((RenderTexture) oldRenderTexture);

                dynamicPosters.FindProperty("_renderTexture").objectReferenceValue = newRenderTexture;
                videoPlayer.FindProperty("targetTexture").objectReferenceValue = newRenderTexture;

                dynamicPosters.ApplyModifiedProperties();
                videoPlayer.ApplyModifiedProperties();
            }
        }
    }
}
