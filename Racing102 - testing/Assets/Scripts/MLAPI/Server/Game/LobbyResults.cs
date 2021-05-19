using System.Collections.Generic;

namespace Racing102.Server
{
    /// <summary>
    /// Simple data-storage of the choices made in the lobby screen for all players
    /// in the lobby. This object is passed from the lobby scene to the gameplay
    /// scene, so that the game knows how to set up the players' characters.
    /// </summary>
    public class LobbyResults
    {
        public readonly Dictionary<ulong, CarSelectChoice> Choices = new Dictionary<ulong, CarSelectChoice>();

        public struct CarSelectChoice
        {
            public int PlayerNumber;
            public CarTypeEnum Car;
            public int Appearance;

            public CarSelectChoice(int playerNumber, CarTypeEnum carClass, int appearanceIdx)
            {
                PlayerNumber = playerNumber;
                Car = carClass;
                Appearance = appearanceIdx;
            }
        }
    }
}
