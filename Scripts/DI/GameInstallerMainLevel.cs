using Gameplay;
using Grid;
using Levels;
using UI;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace DI
{
    public class GameInstallerMainLevel : MonoInstaller
    {
        [Header("Loggers")]
        [SerializeField] private Logger playerLogger;
        [SerializeField] private Logger systemLogger;
        [SerializeField] private Logger mapLogger;
        [SerializeField] private Logger levelsLogger;
        
        [Header("Systems")]
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private MapGenerator mapGenerator; 
        
        [Header("Gameplay")]
        [SerializeField] private Player player;
        [SerializeField] private Undo undo;
        [SerializeField] private Submit submit;
        [SerializeField] private Hint hint;
        
        [Header("UI")]
        [SerializeField] private Fader fader;

        [SerializeField] private GridPosition gridPosition;
        public override void InstallBindings()
        {
            Container.Bind<Logger>().WithId("PlayerLogger").FromInstance(playerLogger).NonLazy();
            Container.Bind<Logger>().WithId("SystemLogger").FromInstance(systemLogger).NonLazy();
            Container.Bind<Logger>().WithId("MapLogger").FromInstance(mapLogger);
            Container.Bind<Logger>().WithId("LevelsLogger").FromInstance(levelsLogger);
            
            Container.Bind<GridSystem>().FromInstance(gridSystem).AsSingle();
            Container.Bind<LevelManager>().FromInstance(levelManager).AsSingle();
            Container.Bind<MapGenerator>().FromInstance(mapGenerator).AsSingle();
            
            Container.Bind<Player>().FromInstance(player).AsSingle();
            Container.Bind<Undo>().FromInstance(undo).AsSingle();
            Container.Bind<Submit>().FromInstance(submit).AsSingle();
            Container.Bind<Hint>().FromInstance(hint).AsSingle();
            Container.Bind<Fader>().FromInstance(fader).AsSingle();
            
            // Binding the prefab and ensuring that dependencies are injected
            Container.Bind<GridPosition>()
                .FromComponentInNewPrefab(gridPosition)
                .AsTransient();
        }

    }
}
