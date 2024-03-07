using System;

namespace AutoChessWPF
{
    /// <summary> События игры </summary>
    public class GameEvents
    {
        /// <summary> Начало боя </summary>
        public static Action? OnFightStart { get; set; }
        /// <summary> Конец боя </summary>
        public static Action? OnFightEnd { get; set; }
        /// <summary> Обновление View </summary>
        public static Action? OnLastUpdate { get; set; }
        /// <summary> Конец игры </summary>
        public static Action<GameSide>? OnGameEnd { get; set; }


        /// <summary> Начало боя </summary>
        public static void SendFightStart() => OnFightStart?.Invoke();
        /// <summary> Конец боя </summary>
        public static void SendOnFightEnd() => OnFightEnd?.Invoke();
        /// <summary> Обновление View </summary>
        public static void SendOnLastUpdate() => OnLastUpdate?.Invoke();
        /// <summary> Конец игры </summary>
        public static void SendOnGameEnd(GameSide winSide) => OnGameEnd?.Invoke(winSide);

    }
}
