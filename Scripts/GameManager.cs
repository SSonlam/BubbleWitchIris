using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class GameManager : MonoBehaviour
{
    public static GameManager theManager = null;

    public PlayFabManager thePlayFabManager = null;
    public PlayerBehavior mPlayer = null;
    public Spawner gameSpawner;
    public MapGenerator mapGenerator;
    public GameUIControl uiControl;
    public GameObject playerRoll;
    private PlayerData playerData;
    private gameState currentState;

    public GameObject[] currentBubbleUnit;
    public GameObject[] currentBubbleSpirit;
    public GameObject[] currentBubbleProjectile;
    public GameObject[] currentProjectile;
    public GameObject[] currentSplash;
    public GameObject[] currentWhirlwind;
    public GameObject[] currentBeam;
    public float difficulty;
    public int unitCounter = 0;
    public int bubbleCounter = 0;
    public bool canMove;
    private bool isHelp;
    public bool isInvincible;
    public bool chainBonus;
    public bool controlPause;
    private float chainTime;
    public int chainCount = 0;
    public int chainScore;
    private Vector3 originalPos;

    private int counter = 0;

    //leaderboard shit
    public List<string> playFabIDList = new List<string>();
    public List<string> playFabScoreList = new List<string>();



    private enum gameState
    {
        LOAD,
        QUICK,
        PAUSE,
        HELP,
        RUN,
        LOSE,
        CLEARED,
        UPGRADE,
        NEXT
    }

    void Awake()
    {
        if (!theManager)
        {
            theManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
 
        PlayerBulletBehavior.setParent(mPlayer);
        CaptureBulletBehavior.setParent(mPlayer);
        //PlayerBehavior.SetGameManager(this);
        isInvincible = false;
        uiControl.hideMenu();
        uiControl.hideLost();
        uiControl.hideResult();
        uiControl.hideUpgrade();

        if(RunStatistics.Instance.isQuick == true)
        {
            currentState = gameState.QUICK;
        }
        else
        {
            currentState = gameState.LOAD;
        }
    }

    // Update is called once per frame
    void Update()
    {
        sequenceControl();
    }

    // Method used to contain all the game's control.
    private void runButtonControl()
    {
        if(controlPause == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                setPause();
            }

            // if (Input.GetKeyDown(KeyCode.K))
            //     //playerHit();

            //     if (Input.GetKeyDown(KeyCode.P))
            //     {
            //         setLose();
            //     }

            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     setCleared();
            // }
            // if (Input.GetKeyDown(KeyCode.I))
            // {
            //     if (isInvincible == false)
            //     {
            //         //isInvincible = true;
            //     }
            //     else
            //     {
            //         //isInvincible = false;
            //     }
            // }


            if (Input.GetKeyDown(KeyCode.H))
            {
                setHelp();
            }
        }
    }

    private void pauseButtonControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            setRun();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            uiControl.hideMenu();
            setHelp();
        }
    }

    private void helpButtonControl()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toHideHelp();
            setPause();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            toHideHelp();
            setRun();
        }
    }

    /*
     * Will revisit
     * 
     * */
    private void gamepadControl()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            currentState = gameState.PAUSE;
            uiControl.showMenu();
        }
    }

    // Method used to control all the game's gamestates. 
    private void sequenceControl()
    {
        switch (currentState)
        {
            
            case gameState.LOAD:
                loadSequence();
                break;
            case gameState.QUICK:
                quickSequence();
                break;
            case gameState.RUN:               
                runSequence();
                break;
            case gameState.PAUSE:
                pauseSequence();
                break;
            case gameState.HELP:
                helpSequence();
                break;
            case gameState.LOSE:
                loseSequence();
                break;
            case gameState.CLEARED:
                clearedSequence();
                break;
            case gameState.UPGRADE:
                upgradeSequence();
                break;
            case gameState.NEXT:
                nextSequence();
                break;
            default:
                break;
        }
    }

    /*
     * loadSequence method, run at the beginning of the game. Load all the player's data.
     * Create a new map, spawn bubble behavior and then move to the RUN gamestate.
     * */
    private void loadSequence()
    {
        //PlayFabManager.thePlayFabManager.Login(RunStatistics.Instance.playerName);
        RunStatistics.Instance.totalScore = 0;
        RunStatistics.Instance.stagesCleared = 0;
        RunStatistics.Instance.currentStage = 1;
        RunStatistics.Instance.time = 0f;
        RunStatistics.Instance.bubblesCleared = 0;
        RunStatistics.Instance.currentLife = 3;
        RunStatistics.Instance.trapCount = 0;
        mPlayer.setDefaultState();
        uiControl.defaultUpgrade();
        //Debug.Log("Diff: " + difficulty);
        generateStage(false);
        originalPos = mPlayer.transform.position;

        uiControl.updateStage();
        if (RunStatistics.Instance.isNew == true)
        {
            setHelp();
            RunStatistics.Instance.isNew = false;
        }
        else
        {
            currentState = gameState.RUN;
            unpauseGame();
        }
        
    }

    //set
    private void generateStage(bool isQuick)
    {
        mPlayer.setTrapCount(RunStatistics.Instance.trapCount);
        QuickSaveData quickSave = SaveSystem.quickLoad();
        if (isQuick == true)
        {
            difficulty = quickSave.difficultyValue;
        }
        else
        {
            difficulty = setStageDifficulty(RunStatistics.Instance.currentStage);
        }
        Debug.Log("Current difficulty: " + difficulty);
        //spawn boss bubble every 3 level for alpha playtest

        if(isQuick == false)
        {
            if (RunStatistics.Instance.currentStage % 3 == 0)
            {
                StartCoroutine(PlayApart("Stage_Start",0.5f,"Boss_BG")); 
                mapGenerator.bossGeneration(difficulty);
                mapGenerator.generateNewGrid();
                gameSpawner.spawnBoss(difficulty);
            }
            // spawn normal bubbles
            else
            {
                StartCoroutine(PlayApart("Stage_Start",0.5f,"Normal_BG"));
                mapGenerator.normalGeneration(difficulty);
                mapGenerator.generateNewGrid();
                gameSpawner.spawnNormal(difficulty);
            }
        }
        else
        {
            

            if (RunStatistics.Instance.currentStage % 3 == 0)
            {
                StartCoroutine(PlayApart("Stage_Start",0.5f,"Boss_BG"));
                mapGenerator.bossGeneration(difficulty);
            }
            // spawn normal bubbles
            else
            {
                StartCoroutine(PlayApart("Stage_Start",0.5f,"Normal_BG"));                 
                mapGenerator.normalGeneration(difficulty);
            }

            mapGenerator.Generate(quickSave.seedValue, quickSave.thresholdValue, quickSave.xValue,quickSave.yValue,difficulty);
            mapGenerator.generateNewGrid();
        }
        
    }
    /* Method runs when the player load a quick save data. 
     * 
     * 
     * */
    private void quickSequence()
    {
        quickLoad();
        generateStage(true);
        quickSpawnWorld();
        uiControl.updateStage();
        RunStatistics.Instance.isQuick = false;
        setPause();
    }

    private void quickLoad()
    {
        QuickSaveData theSaveData = SaveSystem.quickLoad();

        RunStatistics.Instance.playerName = theSaveData.playerName;
        RunStatistics.Instance.currentLife = theSaveData.currentLife;
        RunStatistics.Instance.time = theSaveData.time;
        RunStatistics.Instance.stagesCleared = theSaveData.stagesCleared;
        RunStatistics.Instance.totalScore = theSaveData.totalScore;
        RunStatistics.Instance.bubblesCleared = theSaveData.bubblesCleared;
        RunStatistics.Instance.bubblesChainCleared = theSaveData.bubblesChainCleared;
        RunStatistics.Instance.bossCleared = theSaveData.bossCleared;
        RunStatistics.Instance.trapCount = theSaveData.trapCount;

        bubbleCounter = theSaveData.totalBubbles;

        switch (theSaveData.movementState)
        {
            case QuickSaveData.PlayerState.ROLLING:
                mPlayer.setRoll();
                break;

            case QuickSaveData.PlayerState.FOCUS:
                mPlayer.setFocus();
                break;

            case QuickSaveData.PlayerState.DEAD:
                mPlayer.setDead();
                break;

            default:
                mPlayer.setNormal();
                break;
        }

        mPlayer.moveSpeed = theSaveData.moveSpeed;
        mPlayer.normalSpeed = theSaveData.normalSpeed;
        mPlayer.focusSpeed = theSaveData.focusSpeed;

        mPlayer.transform.position = theSaveData.playerCurrentPos.getVectorThree();
        mPlayer.mousePos = theSaveData.mousePos.getVectorTwo();
        mPlayer.movementVector = theSaveData.movementVector.getVectorTwo();
        mPlayer.moveDir = theSaveData.moveDir.getVectorThree();
        mPlayer.slideDir = theSaveData.slideDir.getVectorThree();

        mPlayer.slideSpeed = theSaveData.slideSpeed;

        mPlayer.isDashButtonDown = theSaveData.isDashButtonDown;
        mPlayer.DashAmount = theSaveData.DashAmount;
        mPlayer.dashCoolDown = theSaveData.dashCoolDown;
        mPlayer.dashAfterSec = theSaveData.dashAfterSec;

        mPlayer.captureCoolDown = theSaveData.captureCoolDown;
        mPlayer.captureAfterSec = theSaveData.captureAfterSec;

        mPlayer.shootCoolDown = theSaveData.shootCoolDown;
        mPlayer.shootAfterSec = theSaveData.shootAfterSec;

        mPlayer.beamCoolDown = theSaveData.beamCoolDown;
        mPlayer.beamAfterSec = theSaveData.beamAfterSec;

        mPlayer.beamDuration = theSaveData.beamDuration;
        mPlayer.beamDurationAfterSec = theSaveData.beamDurationAfterSec;

        mPlayer.shootBeam = theSaveData.shootBeam;
        mPlayer.counter = theSaveData.counter;

        mPlayer.showCutIn = theSaveData.showCutIn;
        mPlayer.flag = theSaveData.flag;
        mPlayer.cutInDuration = theSaveData.cutInDuration;
        mPlayer.canMove = theSaveData.canMove;

        mPlayer.isCapturing = theSaveData.isCapturing;

        if(theSaveData.isCapturing == true)
        {
            GameObject e = Instantiate(Resources.Load("Prefabs/BubbleSpirit")) as GameObject;
            e.transform.position = theSaveData.capturedBubble.bubblePosition.getVectorThree();
            e.GetComponent<BubbleSpirit>().SetColor(theSaveData.capturedBubble.color);
            e.transform.SetParent(mPlayer.transform);
            e.GetComponent<BubbleSpirit>().quickSaveRestoreCapture();
            mPlayer.setCaptureBubble(e.GetComponent<BubbleSpirit>());
        }

        mPlayer.extraTrap = theSaveData.extraTrap;
        mPlayer.trapCountCap = theSaveData.trapCountCap;
        mPlayer.trapUpgrade = theSaveData.trapUpgrade;

        mPlayer.spriteBlinkingTimer =theSaveData.spriteBlinkingTimer;
        mPlayer.spriteBlinkingMiniDuration = theSaveData.spriteBlinkingMiniDuration;
        mPlayer.spriteBlinkingTotalTimer = theSaveData.spriteBlinkingTotalTimer;
        mPlayer.spriteBlinkingTotalDuration = theSaveData.spriteBlinkingTotalDuration;

        uiControl.rollCooldown = theSaveData.savedUI.rollCooldown;
        uiControl.rollUI.fillAmount = theSaveData.savedUI.rollFill;
        uiControl.trapCooldown = theSaveData.savedUI.trapCooldown;
        uiControl.trapUI.fillAmount = theSaveData.savedUI.trapFill;
        uiControl.captureCooldown = theSaveData.savedUI.captureCooldown;
        uiControl.captureUI.fillAmount = theSaveData.savedUI.captureFill;
        uiControl.trapCount = theSaveData.savedUI.trapCount;
        uiControl.trapMax = theSaveData.savedUI.trapMax;
        uiControl.captureMax = theSaveData.savedUI.captureMax;
        uiControl.rollMax = theSaveData.savedUI.rollMax;
        uiControl.lifeMax = theSaveData.savedUI.lifeMax;
    }

    private void quickSpawnWorld()
    {
        // Procedural World Spawn Goes Here
        gameSpawner.quickSpawnUnit();
        gameSpawner.quickSpawnEnemyProjectile();
        gameSpawner.quickSpawnPlayerProjectile();
    }

    /*
     * pauseSequence method, run when ESC is pressed. All objects cannot move.
     * Used to show the main menu of the game
     * */
    private void pauseSequence()
    {
        pauseButtonControl();
    }

    // helpSequence method, run when a new game is loaded or H is pressed. All objects cannot move.
    private void helpSequence()
    {
        helpButtonControl();
    }

    /*
     * runSequence method, run when the session is play. 
     * Player, Bubble spirit and bullet can move.
     * */
    private void runSequence()
    {
        runButtonControl();
        RunStatistics.Instance.time += Time.smoothDeltaTime;
        updateChainTimer();

        if (isInvincible == true)
        {
            showRoll();
        }
        else
        {
            hideRoll();
        }

        if (bubbleCounter == 0)
        {
            setCleared();
        }
    }



    /*
     * loseSequence method, used to show the player's final result of current run.
     * */
    private void loseSequence()
    { 
        uiControl.updateLost();
        uiControl.showLost();
        
        if(counter == 0)
        {
            //PlayFabManager.thePlayFabManager.SendLeaderboard(RunStatistics.Instance.totalScore);
            counter++;
        }
        
    }

    /*
     * clearsedSequence method, used to show the player's result in current run.
     * Runs after all bubble spirit on the map are cleared. 
     * */
    private void clearedSequence()
    {
        uiControl.updateResult();
        uiControl.showResult();
    }

    // method used to control the upgrade sequence gamestate
    private void upgradeSequence()
    {
        
        uiControl.updateUpgrade();
    }

    // nextSequence method, used to create a new stage for player
    private void nextSequence()
    {
        //FindObjectOfType<AudioManager>().Stop("Stage_BG");
        //FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
        //Reset player, can be used to keep the trap count from the previus stage.
        RunStatistics.Instance.trapCount = 0;
        uiControl.hideResult();
        uiControl.hideUpgrade();
        clearEnemy(); // Not necessary if everything runs well.
        RunStatistics.Instance.currentStage++;
        generateStage(false);
        uiControl.updateStage();
        originalPos = mPlayer.transform.position;
        currentState = gameState.RUN;
        mPlayer.RestoreDefaultState();
        mPlayer.isCapturing = false;
        unpauseGame();
    }


    private float setStageDifficulty(int stage)
    {
        float difficulty;
        //curve is the stage scale, set to 2 for quick demo of the curve,
        //change to something like 12 for full game.
        int curve = 2;
        //difficulty = 100/(1 + Mathf.Exp(-((stage/curve) - (1.7f *
        //Mathf.Exp(1)))));
        
        // using linear difficulty now ...
        difficulty = Mathf.Min(4.0f * stage, 99.9f);
        return difficulty;
    }

    // Method used to clear all the bubblet spirits and their projectile for a new game
    private void clearEnemy()
    {
        currentBubbleSpirit = GameObject.FindGameObjectsWithTag("BubbleSpirit");
        for(int i = 0; i < currentBubbleSpirit.Length; i++)
        {
            if(currentBubbleSpirit[i] != null)
            {
                Destroy(currentBubbleSpirit[i]);
            }
        }

        currentBubbleUnit = GameObject.FindGameObjectsWithTag("BubbleUnit");
        for(int j = 0; j < currentBubbleUnit.Length; j++)
        {
            if(currentBubbleUnit[j] != null)
            {
                Destroy(currentBubbleUnit[j]);
            }
        }

        currentBubbleProjectile = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for(int i = 0; i < currentBubbleProjectile.Length; i++)
        {
            Destroy(currentBubbleProjectile[i]);
        }

        currentProjectile = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < currentProjectile.Length; i++)
        {
            Destroy(currentProjectile[i]);
        }

        bubbleCounter = 0;
    }

    // Method used to pause the game
    private void pauseGame()
    {
        Time.timeScale = 0;
        canMove = false;
    }
    
    // Method used to unpause the game
    private void unpauseGame()
    {
        Time.timeScale = 1;
        canMove = true;
    }

    public void pauseControl()
    {
        controlPause = true;
    }

    public void unpauseControl()
    {
        controlPause = false;
    }

    #region Data Control

    // Method use the save the player's progress
    public void saveGame()
    {
        SaveSystem.savePlayer();
    }

    /* Method used to save all the save data of a run
     * 
     * 
     * */
    public void quickSave()
    {
        QuickSaveData quickSave = new QuickSaveData();

        // Runstatistic Data
        quickSave.playerName = RunStatistics.Instance.playerName;
        quickSave.currentLife = RunStatistics.Instance.currentLife;
        quickSave.time = RunStatistics.Instance.time;
        quickSave.stagesCleared = (RunStatistics.Instance.stagesCleared);
        quickSave.totalScore = (RunStatistics.Instance.totalScore);
        quickSave.bubblesCleared = (RunStatistics.Instance.bubblesCleared);
        quickSave.bubblesChainCleared = (RunStatistics.Instance.bubblesChainCleared);
        quickSave.bossCleared = (RunStatistics.Instance.bossCleared);
        quickSave.trapCount = (RunStatistics.Instance.trapCount);

        quickSave.totalBubbles = bubbleCounter;

        // Mapp generator Value
        quickSave.seedValue = mapGenerator.seed;
        quickSave.thresholdValue = mapGenerator.threshold;
        quickSave.xValue = mapGenerator.xscale;
        quickSave.yValue = mapGenerator.yscale;
        quickSave.difficultyValue = difficulty;

        // Game Actor Data

        quickSave.movementState = (QuickSaveData.PlayerState)mPlayer.getMovementState();
        Debug.Log("The QuickSave Movement State is " + quickSave.movementState);

        quickSave.moveSpeed = mPlayer.moveSpeed;
        quickSave.normalSpeed = mPlayer.normalSpeed;
        quickSave.focusSpeed = mPlayer.focusSpeed;

        quickSave.playerCurrentPos = new SerializableVector(mPlayer.transform.localPosition);
        quickSave.mousePos = new SerializableVector(mPlayer.mousePos);
        quickSave.movementVector = new SerializableVector(mPlayer.movementVector);
        quickSave.moveDir = new SerializableVector(mPlayer.moveDir);
        quickSave.slideDir = new SerializableVector(mPlayer.slideDir);

        quickSave.slideSpeed = mPlayer.slideSpeed;

        quickSave.isDashButtonDown = mPlayer.isDashButtonDown;
        quickSave.DashAmount = mPlayer.DashAmount;
        quickSave.dashCoolDown = mPlayer.dashCoolDown;
        quickSave.dashAfterSec = mPlayer.dashAfterSec;

        quickSave.captureCoolDown = mPlayer.captureCoolDown;
        quickSave.captureAfterSec = mPlayer.captureAfterSec;

        quickSave.shootCoolDown = mPlayer.shootCoolDown;
        quickSave.shootAfterSec = mPlayer.shootAfterSec;

        quickSave.beamCoolDown = mPlayer.beamCoolDown;
        quickSave.beamAfterSec = mPlayer.beamAfterSec;

        quickSave.beamDuration = mPlayer.beamDuration;
        quickSave.beamDurationAfterSec = mPlayer.beamDurationAfterSec;

        quickSave.shootBeam = mPlayer.shootBeam;
        quickSave.counter = mPlayer.counter;

        quickSave.showCutIn = mPlayer.showCutIn;
        quickSave.flag = mPlayer.flag;
        quickSave.cutInDuration = mPlayer.cutInDuration;
        quickSave.canMove = mPlayer.canMove;

    quickSave.isCapturing = mPlayer.isCapturing;

        if(mPlayer.capturedBubble != null)
        {
            SerializableBubbleSpirit quickBubble = new SerializableBubbleSpirit();
            quickBubble.state = SerializableBubbleSpirit.State.CAPTURED;
            quickBubble.color = mPlayer.capturedBubble.color;
            quickBubble.bubblePosition = new SerializableVector(mPlayer.capturedBubble.transform.position);
            quickSave.capturedBubble = quickBubble;
        }
        
        quickSave.extraTrap = mPlayer.extraTrap;
        quickSave.trapCountCap = mPlayer.trapCountCap;
        quickSave.trapUpgrade = mPlayer.trapUpgrade;

        quickSave.spriteBlinkingTimer = mPlayer.spriteBlinkingTimer;
        quickSave.spriteBlinkingMiniDuration = mPlayer.spriteBlinkingMiniDuration;
        quickSave.spriteBlinkingTotalTimer = mPlayer.spriteBlinkingTotalTimer;
        quickSave.spriteBlinkingTotalDuration = mPlayer.spriteBlinkingTotalDuration;

        //quicksaveUI
        SerializableUI quickSavedUI = new SerializableUI();
        quickSavedUI.rollCooldown = uiControl.rollCooldown;
        quickSavedUI.rollFill = uiControl.rollUI.fillAmount;
        quickSavedUI.trapCooldown = uiControl.trapCooldown;
        quickSavedUI.trapFill = uiControl.trapUI.fillAmount;
        quickSavedUI.captureCooldown = uiControl.captureCooldown;
        quickSavedUI.captureFill = uiControl.captureUI.fillAmount;
        quickSavedUI.trapCount = uiControl.trapCount;
        quickSavedUI.trapMax = uiControl.trapMax;
        quickSavedUI.captureMax = uiControl.captureMax;
        quickSavedUI.rollMax = uiControl.rollMax;
        quickSavedUI.lifeMax = uiControl.lifeMax;
        quickSave.savedUI = quickSavedUI;

        //quickSave.currentBubbleUnit = GameObject.FindGameObjectsWithTag("BubbleUnit");
        currentBubbleUnit = GameObject.FindGameObjectsWithTag("BubbleUnit");
        quickSave.currentBubbleUnit = new SerializableBubbleUnit[currentBubbleUnit.Length];

        for(int i = 0; i < currentBubbleUnit.Length; i++)
        {
            SerializableBubbleUnit saveUnit = new SerializableBubbleUnit();
            saveUnit.unitPosition = new SerializableVector(currentBubbleUnit[i].transform.localPosition);
            saveUnit.initialPosition = new SerializableVector(currentBubbleUnit[i].GetComponent<BubbleUnit>().initialPosition);
            saveUnit.movePosition = new SerializableVector(currentBubbleUnit[i].GetComponent<BubbleUnit>().movePosition);
            saveUnit.timeToMove = currentBubbleUnit[i].GetComponent<BubbleUnit>().timeToMove;
            saveUnit.moveTimer = currentBubbleUnit[i].GetComponent<BubbleUnit>().moveTimer;
            saveUnit.radius = currentBubbleUnit[i].GetComponent<BubbleUnit>().radius;
            saveUnit.bubbleCount = currentBubbleUnit[i].GetComponent<BubbleUnit>().bubbleCount;

            quickSave.currentBubbleUnit[i] = saveUnit;

            List<GameObject> theChildrenBubble = new List<GameObject>();

            foreach (Transform child in currentBubbleUnit[i].transform)
            {
                theChildrenBubble.Add(child.gameObject);
                
            }
            Debug.Log("Children Bubble Counter: " + theChildrenBubble.Count);
            saveUnit.childrenBubble = new SerializableBubbleSpirit[theChildrenBubble.Count];

            for(int j = 0; j < saveUnit.childrenBubble.Length; j++)
            {
                SerializableBubbleSpirit theChildrenSpirit = new SerializableBubbleSpirit();
                theChildrenSpirit.bubblePosition = new SerializableVector(theChildrenBubble[j].transform.position);
                theChildrenSpirit.gridPosition = new SerializableVector(theChildrenBubble[j].GetComponent<BubbleSpirit>().gridPosition);
                theChildrenSpirit.color = theChildrenBubble[j].GetComponent<BubbleSpirit>().color;
                theChildrenSpirit.rebounds = theChildrenBubble[j].GetComponent<BubbleSpirit>().rebounds;
                theChildrenSpirit.launchDirection = new SerializableVector(theChildrenBubble[j].GetComponent<BubbleSpirit>().launchDirection);
                if(theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern != null)
                {
                    theChildrenSpirit.pattern = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.arg;

                    SerializableBubbleProjectile bubbleProjectile = new SerializableBubbleProjectile();

                    bubbleProjectile.velocity = new SerializableVector(theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.arg.bulletPrefab.velocity);
                    bubbleProjectile.angularVelocity = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.arg.bulletPrefab.angularVelocity;
                    bubbleProjectile.acceleration = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.arg.bulletPrefab.acceleration;
                    bubbleProjectile.accelerationTimeout = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.arg.bulletPrefab.accelerationTimeout;

                    theChildrenSpirit.patternPrefab = bubbleProjectile;
                    //theChildrenSpirit.patternParameter = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.velocityParameters;
                }
                //theChildrenSpirit.patternType = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.GetType().ToString();
                //Debug.Log("This Bubble Spirit has the bullet pattern " + theChildrenSpirit.patternType);
                //theChildrenSpirit.patternLifeTime = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.patternLifetime;
                //theChildrenSpirit.lifeTime = theChildrenBubble[j].GetComponent<BubbleSpirit>().pattern.lifetime;
                theChildrenSpirit.cleared = theChildrenBubble[j].GetComponent<BubbleSpirit>().cleared;
                theChildrenSpirit.isChain = theChildrenBubble[j].GetComponent<BubbleSpirit>().isChain;
                saveUnit.childrenBubble[j] = theChildrenSpirit;
            }
        }

        currentBubbleSpirit = GameObject.FindGameObjectsWithTag("BubbleSpirit");
        GameObject[] specificSpirit = new GameObject[currentBubbleSpirit.Length];

        Debug.Log("The length of the currentBubbleSpirit array is " + currentBubbleSpirit.Length);
        quickSave.currentBubbleSpirit = new SerializableBubbleSpirit[currentBubbleSpirit.Length];

        for(int i = 0; i < currentBubbleSpirit.Length; i++)
        {
            SerializableBubbleSpirit savedSpirit = new SerializableBubbleSpirit();
            savedSpirit.bubblePosition = new SerializableVector(currentBubbleSpirit[i].transform.position);
            savedSpirit.bubbleSize = new SerializableVector(currentBubbleSpirit[i].transform.localScale);
            savedSpirit.launchDirection = new SerializableVector(currentBubbleSpirit[i].GetComponent<BubbleSpirit>().launchDirection);
            savedSpirit.bubbleRotation = new SerializableVector(currentBubbleSpirit[i].transform.rotation);

            switch (currentBubbleSpirit[i].GetComponent<BubbleSpirit>().state)
            {
                case BubbleSpirit.State.CLEARED:
                    savedSpirit.state = SerializableBubbleSpirit.State.CLEARED;
                    break;

                case BubbleSpirit.State.LAUNCHED:
                    savedSpirit.state = SerializableBubbleSpirit.State.LAUNCHED;
                    break;

                default:
                    savedSpirit.state = SerializableBubbleSpirit.State.NORMAL;
                    break;
            }

            savedSpirit.color = currentBubbleSpirit[i].GetComponent<BubbleSpirit>().color;
            savedSpirit.rebounds = currentBubbleSpirit[i].GetComponent<BubbleSpirit>().rebounds;
            savedSpirit.cleared = currentBubbleSpirit[i].GetComponent<BubbleSpirit>().cleared;
            savedSpirit.isChain = currentBubbleSpirit[i].GetComponent<BubbleSpirit>().isChain;
            quickSave.currentBubbleSpirit[i] = savedSpirit;
        }

        currentBubbleProjectile = GameObject.FindGameObjectsWithTag("EnemyBullet");
        quickSave.currentBubbleProjectile = new SerializableBubbleProjectile[currentBubbleProjectile.Length];
        for (int i = 0; i < currentBubbleProjectile.Length; i++)
        {
            SerializableBubbleProjectile bubbleProjectile = new SerializableBubbleProjectile();
            bubbleProjectile.projectilePosition = new SerializableVector(currentBubbleProjectile[i].transform.position);
            bubbleProjectile.projectileRotation = new SerializableVector(currentBubbleProjectile[i].transform.localRotation);
            bubbleProjectile.projectileDirection = new SerializableVector(currentBubbleProjectile[i].GetComponent<BubbleBullet>().direction);
            bubbleProjectile.velocity = new SerializableVector(currentBubbleProjectile[i].GetComponent<BubbleBullet>().velocity);
            bubbleProjectile.angularVelocity = currentBubbleProjectile[i].GetComponent<BubbleBullet>().angularVelocity;
            bubbleProjectile.acceleration = currentBubbleProjectile[i].GetComponent<BubbleBullet>().acceleration;
            bubbleProjectile.accelerationTimeout = currentBubbleProjectile[i].GetComponent<BubbleBullet>().accelerationTimeout;
            quickSave.currentBubbleProjectile[i] = bubbleProjectile;
        }


        GameObject[] captureProjectile = GameObject.FindGameObjectsWithTag("Capture");
        quickSave.capturePlayerProjectile = new SerializablePlayerProjectile[captureProjectile.Length];
        for (int i = 0; i < captureProjectile.Length; i++)
        {
            SerializablePlayerProjectile capturePlayerProjectile = new SerializablePlayerProjectile();
            capturePlayerProjectile.bulletDirection = new SerializableVector(captureProjectile[i].transform.position);
            capturePlayerProjectile.bulletRotation = new SerializableVector(captureProjectile[i].transform.localRotation);
            capturePlayerProjectile.rebounds = captureProjectile[i].GetComponent<CaptureBulletBehavior>().rebounds;
            capturePlayerProjectile.disabled = captureProjectile[i].GetComponent<CaptureBulletBehavior>().disabled;
            quickSave.capturePlayerProjectile[i] = capturePlayerProjectile;
        }

        currentProjectile = GameObject.FindGameObjectsWithTag("Bullet");
        quickSave.currentPlayerProjectile = new SerializablePlayerProjectile[currentProjectile.Length];

        for(int i = 0; i < currentProjectile.Length; i++) {
            SerializablePlayerProjectile thePlayerProjectile = new SerializablePlayerProjectile();
            thePlayerProjectile.bulletDirection = new SerializableVector(currentProjectile[i].transform.position);
            thePlayerProjectile.bulletRotation = new SerializableVector(currentProjectile[i].transform.localRotation);
            thePlayerProjectile.lifeSpan = currentProjectile[i].GetComponent<PlayerBulletBehavior>().lifeSpan;
            thePlayerProjectile.disabled = currentProjectile[i].GetComponent<PlayerBulletBehavior>().disabled;
            quickSave.currentPlayerProjectile[i] = thePlayerProjectile;
        }

        currentSplash = GameObject.FindGameObjectsWithTag("Splash");
        quickSave.currentSplash = new SerializablePlayerProjectile[currentSplash.Length];
        for( int i = 0; i < currentSplash.Length; i++)
        {
            SerializablePlayerProjectile splashProjectile = new SerializablePlayerProjectile();
            splashProjectile.bulletDirection = new SerializableVector(currentSplash[i].transform.position);
            splashProjectile.bulletRotation = new SerializableVector(currentSplash[i].transform.localRotation);
            splashProjectile.lifeSpan = currentSplash[i].GetComponent<SplashBullet>().lifeSpan;
            splashProjectile.disabled = currentSplash[i].GetComponent<SplashBullet>().disabled;
            quickSave.currentSplash[i] = splashProjectile;
        }

        currentWhirlwind = GameObject.FindGameObjectsWithTag("SplashDamage");
        quickSave.currentWhirlwind = new SerializablePlayerProjectile[currentWhirlwind.Length];
        for (int i = 0; i < currentWhirlwind.Length; i++)
        {
            SerializablePlayerProjectile whirldwindProjectile = new SerializablePlayerProjectile();
            whirldwindProjectile.bulletDirection = new SerializableVector(currentWhirlwind[i].transform.position);
            whirldwindProjectile.bulletRotation = new SerializableVector(currentWhirlwind[i].transform.localRotation);
            quickSave.currentWhirlwind[i] = whirldwindProjectile;
        }

        currentBeam = GameObject.FindGameObjectsWithTag("Beam");
        quickSave.currentBeam = new SerializablePlayerProjectile[currentBeam.Length];
        for( int i = 0; i < currentBeam.Length; i++)
        {
            SerializablePlayerProjectile beamProjectile = new SerializablePlayerProjectile();
            beamProjectile.bulletDirection = new SerializableVector(currentBeam[i].transform.position);
            beamProjectile.bulletRotation = new SerializableVector(currentBeam[i].transform.localRotation);
            beamProjectile.axis = new SerializableVector(currentBeam[i].GetComponent<SinWaveBullet>().axis);
            beamProjectile.input = new SerializableVector(currentBeam[i].GetComponent<SinWaveBullet>().input);
            beamProjectile.pos = new SerializableVector(currentBeam[i].GetComponent<SinWaveBullet>().pos);
            beamProjectile.MoveSpeed = currentBeam[i].GetComponent<SinWaveBullet>().MoveSpeed;
            beamProjectile.frequency = currentBeam[i].GetComponent<SinWaveBullet>().frequency;
            beamProjectile.magnitude = currentBeam[i].GetComponent<SinWaveBullet>().magnitude;
            beamProjectile.disabled = currentBeam[i].GetComponent<SinWaveBullet>().disabled;
            quickSave.currentBeam[i] = beamProjectile;
        }

        SaveSystem.quickSave(quickSave);
    }

    public void addBubble()
    {
        bubbleCounter++;
    }

    public void bubbleCleared(bool normalClear)
    {
        if (normalClear)
        {
            uiControl.addClearIndicator();
        }

        bubbleCounter--;
    }

    public void updateChainTimer()
    {
        if(chainTime <= 0)
        {
            defaultChainTimer();
        }
        else
        {
            chainTime -= Time.deltaTime;
        }
    }

    private void defaultChainTimer()
    {
        chainBonus = false;
        chainCount = 0;
        chainTime = 0f;
    }

    public void addChain()
    {
        if(chainBonus == false)
        {
            uiControl.isChain = true;
            uiControl.chainTime = 0.5f;
            uiControl.showChainText();
            int theScore = 15;
            RunStatistics.Instance.totalScore += theScore;
            chainScore += theScore;
            chainBonus = true;
            chainTime = 1f;
            chainCount = 0;
        }
        else
        {
            if(chainCount <= 5)
            {
                int theScore = (15 + (int)(15 * 0.25));
                RunStatistics.Instance.totalScore += theScore;
                chainScore += theScore;
                chainTime = 0.5f;
                uiControl.chainTime = 0.5f;
                chainCount++;
            }
            else
            {
                int theScore = (15 + (int)(15 * 0.5)); ;
                RunStatistics.Instance.totalScore += theScore;
                chainScore += theScore;
                chainTime = 0.5f;
                uiControl.chainTime = 0.5f;
                chainCount++;
            }
        }
    }

    #endregion;

    // Method used to restart the run from the beginning
    public void restartLevel()
    {
        uiControl.hideLost();
        clearEnemy();
        RunStatistics.Instance.bubblesChainCleared = new int[BubbleColor.count];
        currentState = gameState.LOAD;
    }

    // Method for button to change game state from Run to Pause
    public void setPause()
    {
        uiControl.showMenu();
        currentState = gameState.PAUSE;
        pauseGame();
    }

    // Method for button to change the game state to HELP
    public void setHelp()
    {
        currentState = gameState.HELP;
        toShowHelp();
    }

    // Method for button to change game state from Pause to Run
    public void setRun()
    {
        uiControl.hideMenu();
        unpauseGame();
        currentState = gameState.RUN;
    }

    // Method for not sure when to use this yet
    public void setLoad()
    {
        currentState = gameState.LOAD;
    }

    // Method for Playerbehavior to change game state to Lose
    public void setLose()
    {
        FindObjectOfType<AudioManager>().Play("Iris_Hit"); 
        FindObjectOfType<AudioManager>().Stop("Stage_BG"); 
        FindObjectOfType<AudioManager>().Play("Stage_Lost");  
        pauseGame();
        clearEnemy();
        SaveSystem.deleteQuick();
        uiControl.updateSendScore();
        currentState = gameState.LOSE;
    }

    public void setCleared()
    {         
        FindObjectOfType<AudioManager>().Stop("Stage_BG");
        //FindObjectOfType<AudioManager>().Play("Stage_Cleared");   
        //FindObjectOfType<AudioManager>().Play("Upgrading_BG");
        StartCoroutine(PlayApart("Stage_Cleared",1.5f,"Upgrading_BG"));
        pauseGame();  
        RunStatistics.Instance.stagesCleared++;
        SaveSystem.deleteQuick();
        currentState = gameState.CLEARED;
    }

    IEnumerator PlayApart(string song1, float seconds, string song2)
    {   
        FindObjectOfType<AudioManager>().Play(song1);   
        //Wait 1 second
        yield return StartCoroutine(WaitIn(seconds));
        //Do process stuff
        FindObjectOfType<AudioManager>().Play(song2);
    }
    
    IEnumerator WaitIn(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
    }


    // Method used to set the game to upgrade sequence
    public void setUpgrade()
    {
        uiControl.hideResult();
        uiControl.showUpgrade();
        currentState = gameState.UPGRADE;
    }

    // Method for button to change game state to NEXT
    public void setNextSequence()
    {       
        FindObjectOfType<AudioManager>().Stop("Upgrading_BG");
        currentState = gameState.NEXT;
    }

    // Method for when the player is hit by an enemy bullet. Reduce 1 life and return to stage's original position
    public void playerHit()
    {
        StartCoroutine(PlayApart("Iris_Hit",0.3f,"Iris_Hit_Voice")); 
        RunStatistics.Instance.currentLife--;
        //mPlayer.transform.position = originalPos;
    }

    // Method to show the help screen
    private void toShowHelp()
    {
        isHelp = true;
        uiControl.helpController();
        pauseGame();
    }

    // Method to hide the help screen
    private void toHideHelp()
    {
        isHelp = false;
        uiControl.hideHelp();
        unpauseGame();
    }

    private void showRoll()
    {
        playerRoll.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/bubble_overlay");
    }

    private void hideRoll()
    {
        playerRoll.GetComponent<SpriteRenderer>().sprite = null;
    }

    public int GetBubbleCounter()
    {
        return bubbleCounter;
    }
}
