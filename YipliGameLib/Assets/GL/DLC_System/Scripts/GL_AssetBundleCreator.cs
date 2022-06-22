#if UNITY_EDITOR
using UnityEditor;

namespace Yipli.GameLib.DlcSystem
{
    public class GL_AssetBundleCreator : Editor
    {
        [MenuItem("Assets/GL_DLC/Make All Windows DLC Bundles")]
        static void MakeAllWindowsBundleFiles()
        {
            BuildPipeline.BuildAssetBundles("Assets/GL/AB/Resources/Windows", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }

        [MenuItem("Assets/GL_DLC/Make All Android DLC Bundles")]
        static void MakeAllAndroidBundleFiles()
        {
            BuildPipeline.BuildAssetBundles("Assets/GL/AB/Resources/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
        }

        [MenuItem("Assets/GL_DLC/Make All iOS DLC Bundles")]
        static void MakeAlliOSBundleFiles()
        {
            BuildPipeline.BuildAssetBundles("Assets/GL/AB/Resources/iOS", BuildAssetBundleOptions.None, BuildTarget.iOS);
        }
    }
}
#endif