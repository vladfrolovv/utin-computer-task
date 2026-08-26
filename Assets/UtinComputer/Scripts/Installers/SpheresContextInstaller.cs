using UnityEngine;
using UtinComputer.Spheres;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Installers
{
    public class SpheresContextInstaller : MonoInstaller
    {
        [SerializeField] private MainSphereView mainSphereView;
        [SerializeField] private ShootingSphereView shootingSphereView;
        [SerializeField] private SphereConfig sphereConfig;

        public override void InstallBindings()
        {
            Container.Bind<MainSphereView>().FromInstance(mainSphereView).AsSingle();
            Container.Bind<ShootingSphereView>().FromInstance(shootingSphereView).AsSingle();

            Container.Bind<SphereConfig>().FromInstance(sphereConfig).AsSingle();

            Container.BindInterfacesAndSelfTo<SphereChargeController>().AsSingle();
        }
    }
}
