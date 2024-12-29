using Levels;
using UI;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private Level levelPrefab; 
        [SerializeField] private Logger levelsLogger;
        [SerializeField] private Logger mapLogger;
        
        [SerializeField] private LevelMapCreator levelMapCreator; 
        [SerializeField] private Fader fader;
        public override void InstallBindings()
        {
            Container.Bind<Logger>().WithId("LevelsLogger").FromInstance(levelsLogger);
            Container.Bind<Logger>().WithId("MapLogger").FromInstance(mapLogger);

            Container.Bind<LevelMapCreator>().FromInstance(levelMapCreator).AsSingle();
            Container.Bind<Fader>().FromInstance(fader).AsSingle();
            
            // Bind the Level prefab
            Container.BindFactory<Level, Level.Factory>()
                .FromComponentInNewPrefab(levelPrefab)
                .AsTransient();
        }
    }
}
