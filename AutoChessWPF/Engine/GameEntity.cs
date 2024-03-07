using System.Runtime.Serialization;

namespace AutoChessWPF
{
    /// <summary> Живое существо (абстрактный класс) </summary>
    [DataContract]
    public abstract class GameEntity
    {
        #region FIELD

        [DataMember]
        protected GameSide _gameSide;
        [DataMember]
        protected bool _isAlive;


        public GameSide GameSide => _gameSide;
        public bool IsAlive => _isAlive;
        [DataMember]
        public int CurrentHealth;

        #endregion

        #region METHODS

        public virtual void Damage(int damage)
        {
            if (!_isAlive)
                return;

            CurrentHealth -= damage;

            if (CurrentHealth <= 0)
                Death();
        }

        public virtual void Death()
        {
            if (!IsAlive)
                return;

            CurrentHealth = 0;
            _isAlive = false;
        }

        #endregion
    }
}
