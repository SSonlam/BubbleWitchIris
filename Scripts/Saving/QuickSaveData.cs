using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class QuickSaveData 
{
    // Runstatistic Section
    public string playerName;
    public int currentLife;
    public float time;
    public int currentStage;
    public int stagesCleared;
    public int totalScore;
    public int bubblesCleared;
    public int bossCleared;
    public int trapCount;
    public int[] bubblesChainCleared;

    // Game Manager Section
    public int totalBubbles;
    public SerializableBubbleUnit[] currentBubbleUnit;
    public SerializableBubbleSpirit[] currentBubbleSpirit;
    public SerializableBubbleProjectile[] currentBubbleProjectile;
    public SerializablePlayerProjectile[] currentPlayerProjectile;
    public SerializablePlayerProjectile[] capturePlayerProjectile;
    public SerializablePlayerProjectile[] currentSplash;
    public SerializablePlayerProjectile[] currentWhirlwind;
    public SerializablePlayerProjectile[] currentBeam;

    // Map Generator Section
    public double seedValue;
    public double thresholdValue;
    public float xValue;
    public float yValue;
    public float difficultyValue;
    

    // Player section
    public float moveSpeed;
    public float normalSpeed;
    public float focusSpeed;

    public enum PlayerState
    {
        NORMAL,
        ROLLING,
        FOCUS,
        DEAD
    };

    public PlayerState movementState;

    public SerializableVector playerCurrentPos;
    public SerializableVector mousePos;
    public SerializableVector movementVector;
    public SerializableVector moveDir;
    public SerializableVector slideDir;
    public float slideSpeed;

    public bool isDashButtonDown;
    public float DashAmount;
    public float dashCoolDown;
    public float dashAfterSec;

    public float captureCoolDown;
    public float captureAfterSec;

    public float shootCoolDown;
    public float shootAfterSec;

    public bool isCapturing;
    public SerializableBubbleSpirit capturedBubble;

    public float extraTrap;
    public int trapCountCap;

    public float beamCoolDown;
    public float beamAfterSec;

    public float beamDuration;
    public float beamDurationAfterSec;

    public bool shootBeam;
    public int counter;

    public bool showCutIn;
    public bool flag;
    public float cutInDuration;
    public bool canMove;

    //Increase by 0.25f or 0.5f when upgrading 
    public float trapUpgrade;

    public float spriteBlinkingTimer;
    public float spriteBlinkingMiniDuration;
    public float spriteBlinkingTotalTimer;
    public float spriteBlinkingTotalDuration;
    public bool startBlinking;

    // UI section
    public SerializableUI savedUI;

    public QuickSaveData()
    {
    }



}
