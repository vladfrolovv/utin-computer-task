using UnityEngine;
using UtinComputer.Cameras;
using Zenject;
namespace UtinComputer.Installers
{
    public class CamerasContextInstaller : MonoInstaller
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private CameraRigView cameraRigView;
        [SerializeField] private CameraConfig cameraConfig;

        public override void InstallBindings()
        {
            Container.Bind<CameraView>().FromInstance(cameraView).AsSingle();
            Container.Bind<CameraRigView>().FromInstance(cameraRigView).AsSingle();

            Container.Bind<CameraConfig>().FromInstance(cameraConfig).AsSingle();

            Container.BindInterfacesAndSelfTo<CameraFollowController>().AsSingle();
            Container.BindInterfacesAndSelfTo<CameraShakeController>().AsSingle();
        }
    }
}
