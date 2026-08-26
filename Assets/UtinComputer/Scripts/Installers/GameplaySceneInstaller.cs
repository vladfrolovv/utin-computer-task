using UnityEngine;
using UtinComputer.Cameras;
using UtinComputer.Spheres;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Installers
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [Header("Cameras")]
        [SerializeField] private CameraView cameraView;
        [SerializeField] private CameraRigView cameraRigView;

        [Header("Spheres")]
        [SerializeField] private MainSphereView mainSphereView;
        [SerializeField] private ShotSphereView shotSphereView;
        [SerializeField] private SphereConfig sphereConfig;

        public override void InstallBindings()
        {
            Container.Bind<CameraView>().FromInstance(cameraView).AsSingle();
            Container.Bind<CameraRigView>().FromInstance(cameraRigView).AsSingle();

            Container.BindInterfacesAndSelfTo<CameraFollowController>().AsSingle();

            Container.Bind<MainSphereView>().FromInstance(mainSphereView).AsSingle();
            Container.Bind<ShotSphereView>().FromInstance(shotSphereView).AsSingle();
            Container.Bind<SphereConfig>().FromInstance(sphereConfig).AsSingle();

            Container.BindInterfacesAndSelfTo<ChargeController>().AsSingle();
        }
    }
}
