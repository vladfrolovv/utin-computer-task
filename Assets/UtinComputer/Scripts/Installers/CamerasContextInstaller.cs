using UnityEngine;
using UtinComputer.Cameras;
using Zenject;
namespace UtinComputer.Installers
{
    public class CamerasContextInstaller : MonoInstaller
    {
        [SerializeField] private CameraView cameraView;
        [SerializeField] private CameraRigView cameraRigView;

        public override void InstallBindings()
        {
            Container.Bind<CameraView>().FromInstance(cameraView).AsSingle();
            Container.Bind<CameraRigView>().FromInstance(cameraRigView).AsSingle();

            Container.BindInterfacesAndSelfTo<CameraFollowController>().AsSingle();
            Container.BindInterfacesAndSelfTo<CameraShakeController>().AsSingle();
        }
    }
}
