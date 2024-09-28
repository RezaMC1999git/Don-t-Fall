using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCurrentLevelParametes : MonoBehaviour
{
    public static FindCurrentLevelParametes instance;
    public LevelParameters[] levelParameterChild;
    public GameController gameController;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }
    public LevelParameters ConnectLevelParametersToItemSlot()
    {
        LevelParameters LPToReturn = null;
        switch (gameController.currentLevel)
        {
            case 1:
                LPToReturn = levelParameterChild[0];
                break;
            case 2:
                LPToReturn = levelParameterChild[1];
                break;
            case 3:
                LPToReturn = levelParameterChild[2];
                break;
            case 4:
                LPToReturn = levelParameterChild[3];
                break;
            case 5:
                LPToReturn = levelParameterChild[4];
                break;
            case 6:
                LPToReturn = levelParameterChild[5];
                break;
            case 7:
                LPToReturn = levelParameterChild[6];
                break;
            case 8:
                LPToReturn = levelParameterChild[7];
                break;
            case 9:
                LPToReturn = levelParameterChild[8];
                break;
            case 10:
                LPToReturn = levelParameterChild[9];
                break;
        }
        return LPToReturn;
    }
}
