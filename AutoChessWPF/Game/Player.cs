using System.Runtime.Serialization;

namespace AutoChessWPF
{
    /// <summary> Класс Игрока </summary>
    [DataContract]
    public class Player : GameEntity
    {
        [DataMember]
        /// <summary> Очки за которые покупаются персонажи </summary>
        private int _mana;

        public int PlayerMana => _mana;

        public Player(int health, int mana)
        {
            CurrentHealth = health;
            _mana = mana;

            _isAlive = true;

            GameEvents.OnFightEnd += NewFightMana;
        }

        public void SpendMana(int value)
        {
            _mana -= value;
        }

        public void PlusMana(int value)
        {
            _mana += value;
        }

        public override void Death()
        {
            base.Death();

            GameSide winSide = (_gameSide == GameSide.PLAYER1) ? (GameSide.PLAYER2) : (GameSide.PLAYER1);
            GameEvents.SendOnGameEnd(winSide);
        }

        /// <summary> Обновление маны в новом раунде игры </summary>
        public void NewFightMana()
        {
            PlusMana(GameManager.PlayerMana);
        }
    }
}
