using Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Grid
{
    public class NumbersController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridPosition gridPosition;
        [ShowInInspector] private Player _player;
        [ShowInInspector] private GridSystem _gridSystem;
        
        [Inject]
        private void Construct(Player player, GridSystem gridSystem)
        {
            _player = player;
            _gridSystem = gridSystem;
        }

        // this is getting called upon pressing on the numbers buttons
        public void OnPress()
        {
            _gridSystem.SetCurrentPressedCube(gridPosition);
            gridPosition.GetLogger().Log(gridPosition.GetNum().ToString(), this);
            _player.SetNumberToPlaceOnBoard(gridPosition.GetNum());

            ChangeColor();
            ToString();       
        }

        public override string ToString()
        {
            var newString = "NumbersController :: This is a pressable number";
            gridPosition.GetLogger().Log(newString, this);   
            return newString;
        }

        private void ChangeColor()
        {
            gridPosition.ChangeColor();
        }
    }
}
