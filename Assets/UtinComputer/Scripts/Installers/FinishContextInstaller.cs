using UnityEngine;
using UtinComputer.Finish;
using Zenject;
namespace UtinComputer.Installers
{
    public class FinishContextInstaller : MonoInstaller
    {
        [SerializeField] private DoorView doorPrefab;
        [SerializeField] private Transform doorParent;
        [SerializeField] private FinishConfig finishConfig;

        public override void InstallBindings()
        {
            Container.Bind<FinishConfig>().FromInstance(finishConfig).AsSingle();

            Container.BindInterfacesAndSelfTo<FinishController>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameFlowController>().AsSingle();

            Container.Bind<DoorView>()
                .FromComponentInNewPrefab(doorPrefab)
                .UnderTransform(doorParent)
                .AsSingle()
                .NonLazy();
        }
    }
}
