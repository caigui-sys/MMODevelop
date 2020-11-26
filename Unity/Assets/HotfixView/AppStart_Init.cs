using UnityEngine;

namespace ET
{
    public class AppStart_Init: AEvent<EventType.AppStart>
    {
        public override async ETTask Run(EventType.AppStart args)
        {
            Game.Scene.AddComponent<TimerComponent>();


            // 下载ab包
            //await BundleHelper.DownloadBundle("1111");

            // 加载配置
            Game.Scene.AddComponent<ResourcesComponent>();
            
            ResourcesComponent.Instance.LoadBundle("config.unity3d");
            Game.Scene.AddComponent<ConfigComponent>();
            ResourcesComponent.Instance.UnloadBundle("config.unity3d");
            
            Game.Scene.AddComponent<OpcodeTypeComponent>();
            Game.Scene.AddComponent<MessageDispatcherComponent>();
            //Game.Scene.AddComponent<UIEventComponent>();

            ResourcesComponent.Instance.LoadBundle("unit.unity3d");

            Scene zoneScene = await SceneFactory.CreateZoneScene(0, 0, "Game");

            await Game.EventSystem.Publish(new EventType.AppStartInitFinish() { ZoneScene = zoneScene });
        }
    }

    private void Setting()
    {
        //setting
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        ////QualitySettings.SetQualityLevel(1);
        Application.runInBackground = true;

        //Define
        Define.IsRemoteDown = IsRemoteDown;
        Define.IsRunBundle = IsRunBundle;
#if !UNITY_EDITOR
            Define.IsRunBundle = true;
#endif
        Define.IsAsync = Define.IsRunBundle;
        //设置pc版 分辨率
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        Screen.SetResolution(UIComponent.designResolutionWight/2, UIComponent.designResolutionHeight/2, false);
#endif
        //path
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        string streamingAssetsPath = Application.dataPath + "/../../Release/{0}/StreamingAssets/";
        string type = "PC";
#if UNITY_ANDROID
            type = "Android";
#endif

#if UNITY_IOS
                type = "IOS";
#endif
        string fold = string.Format(streamingAssetsPath, type);
        string datapath = Application.persistentDataPath + "/" + Application.productName + "/";
        if (!Define.IsAsync)
        {
            datapath = Application.dataPath + "/" + "[Resources]/Config/";
        }
        else if (Define.IsRunBundle && Define.IsRemoteDown == false)
        {
            datapath = fold;
        }
        PathHelper.Regiest(fold, Application.persistentDataPath, datapath + "datafile");
#else
            string datapath = Application.persistentDataPath+ "/";
                PathHelper.Regiest(Application.streamingAssetsPath+"/", Application.persistentDataPath,datapath+"datafile"); ;
#endif
        Log.Info(string.Format("AppHotfixResPath:{0} AppResPath:{1} AppResPath4Web:{2} tempPath:{3}",
            PathHelper.AppHotfixResPath, PathHelper.AppResPath, PathHelper.AppResPath4Web, Application.temporaryCachePath));
    }
}