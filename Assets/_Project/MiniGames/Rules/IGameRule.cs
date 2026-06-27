namespace Project.MiniGames.Rules
{
    public interface IGameRule
    {
        bool IsGameOver { get; }
        int CalculateReward();
        void OnGameStarted();
        void OnScoreChanged(int amount);
        void OnPlayerDeath();
        void OnDistanceChanged(float distance);
        void OnGameComplete();
    }
}