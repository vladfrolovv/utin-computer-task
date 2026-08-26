using UnityEngine;
using UtinComputer.Spheres;
using UtinComputer.Spheres.Charges;
using Zenject;
namespace UtinComputer.Installers
{
    public class SpheresContextInstaller : MonoInstaller
    {
        [SerializeField] private MainSphereView mainSphereView;
        [SerializeField] private ShotSphereView shotSphereView;

        public override void InstallBindings()
        {
            Container.Bind<MainSphereView>().FromInstance(mainSphereView).AsSingle();
            Container.Bind<ShotSphereView>().FromInstance(shotSphereView).AsSingle();

            Container.BindInterfacesAndSelfTo<ChargeController>().AsSingle();
        }
    }
}
