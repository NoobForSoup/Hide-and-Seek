using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventCodes
{
    #region Game Handler: 0-9
    public const byte startGame = 0;
    public const byte endGame = 1;
    public const byte restartGame = 2;

    public const byte playerLoaded = 5;
    #endregion
    #region Game Related 10-19
    public const byte releaseSeeker = 10;
    #endregion
    #region Player Related 20-29
    public const byte damagePlayer = 20;
    public const byte playerDeath = 21;
    #endregion
    #region Chat Related 40-49
    public const byte chatMessage = 40;
    #endregion
    #region Object Related 50-59
    public const byte useDoor = 50;
    #endregion
}
