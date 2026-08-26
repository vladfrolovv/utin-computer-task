using UnityEngine;
using UtinComputer.Map;
using Zenject;
namespace UtinComputer.Installers
{
    public class MapContextInstaller : MonoInstaller
    {
        [SerializeField] private MapView mapView;
        [SerializeField] private MapConfig mapConfig;
        [SerializeField] private TreeView[] treePrefabs;

        public override void InstallBindings()
        {
            Container.Bind<MapView>().FromInstance(mapView).AsSingle();
            Container.Bind<MapConfig>().FromInstance(mapConfig).AsSingle();

            Container.BindInterfacesAndSelfTo<MapGenerationController>().AsSingle();

            foreach (TreeView treePrefab in treePrefabs)
            {
                Container.BindMemoryPool<TreeView, TreeViewPool>()
                    .WithInitialSize(mapConfig.PooledTreesPerPrefab)
                    .FromComponentInNewPrefab(treePrefab)
                    .UnderTransform(mapView.transform);
            }
        }
    }
}
