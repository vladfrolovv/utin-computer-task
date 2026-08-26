using UnityEngine;
using UtinComputer.Spheres;
using Zenject;
namespace UtinComputer.Installers
{
    [CreateAssetMenu(menuName = "UtinComputer/Configs Installer", fileName = "ConfigsInstaller")]
    public class ConfigsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private SphereConfig sphereConfig;

        public override void InstallBindings()
        {
            Container.Bind<SphereConfig>().FromInstance(sphereConfig).AsSingle();
        }
    }
}
