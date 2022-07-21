using System;
using Core.ConfigModule;

namespace Core.GameSettingsModule
{
    [Serializable]
    public class GameSettings : JsonConfigData<GameSettings>
    {
        protected override ConfigName ConfigName { get; } = new ConfigName("gamesetting");

        public AudioSettings AudioSettings { get; } = new AudioSettings();
        public LevelDifficultyType LevelDifficulty { get; set; } = LevelDifficultyType.Easy;
    }
}
