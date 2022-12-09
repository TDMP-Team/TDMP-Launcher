using Newtonsoft.Json;
using System.IO;
using TeardownMultiplayerLauncher.Core.Models;

namespace TeardownMultiplayerLauncher.Core.Repositories
{
    internal class StateRepository
    {
        private static readonly string StateFilePath = "state.json";

        public State GetState()
        {
            EnsureStateFileExists();
            return JsonConvert.DeserializeObject<State>(File.ReadAllText(StateFilePath));
        }

        public void SaveState(State state)
        {
            EnsureStateFileExists();
            File.WriteAllText(StateFilePath, JsonConvert.SerializeObject(state));
        }

        private void EnsureStateFileExists()
        {
            if (!File.Exists(StateFilePath))
            {
                File.WriteAllText(StateFilePath, JsonConvert.SerializeObject(new State()));
            }
        }
    }
}
