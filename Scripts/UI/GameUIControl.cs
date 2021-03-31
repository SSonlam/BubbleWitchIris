using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIControl : MonoBehaviour
{
    public PlayerBehavior thePlayer;
    public Image playerHUDUI;
    public Image mainMenuUI;
    public Image optionUI;
    public Image lostScreenUI;
    public Image resultScreenUI;
    public Image upgradeScreenUI;
    public Image helpScreenUI;
    public Image captureUI;
    public Image trapUI;
    public Image rollUI;
    public Image trapButtonUI;
    public Image captureButtonUI;
    public Image rollButtonUI;
    public Image lifeButtonUI;
    public Image tutorialOne;
    public Image tutorialTwo;
    public Image tutorialThree;
    public Image sendScoreUI;
    public Image trapBarCurrentUI;
    public Image trapBarChargeUI;
    public Text stageUI;
    public Text playerLifeUI;
    public Text scoreUI;
    public Text trapCountUI;
    public Text chainTextUI;


    private CanvasGroup mainMenuGroup;
    private CanvasGroup optionGroup;
    private CanvasGroup lostScreenGroup;
    private CanvasGroup resultScreenGroup;
    private CanvasGroup upgradeScreenGroup;
    private CanvasGroup helpScreenGroup;
    private CanvasGroup sendScoreGroup;
    private CanvasGroup chainTextGroup;

    public float rollCooldown;
    public float trapCooldown;
    public float captureCooldown;
    public int trapCount;
    public bool isChain;
    public bool isChainScore;
    public float chainTime;
    public float scoreTime;
    public float currentCharge = 0f; // the trapBarCharge current charge
    public float chargeCap = 0.4f; // the trapBarCharge cap

    public bool trapMax = false;
    public bool captureMax = false;
    public bool rollMax = false;
    public bool lifeMax = false;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuGroup = mainMenuUI.GetComponent<CanvasGroup>();
        optionGroup = optionUI.GetComponent<CanvasGroup>();
        lostScreenGroup = lostScreenUI.GetComponent<CanvasGroup>();
        resultScreenGroup = resultScreenUI.GetComponent<CanvasGroup>();
        upgradeScreenGroup = upgradeScreenUI.GetComponent<CanvasGroup>();
        helpScreenGroup = helpScreenUI.GetComponent<CanvasGroup>();
        sendScoreGroup = sendScoreUI.GetComponent<CanvasGroup>();
        chainTextGroup = chainTextUI.GetComponent<CanvasGroup>();

        defaultChainText();
        resetTrapCharge();

        /*
        rollCooldown = thePlayer.dashCoolDown;
        trapCooldown = thePlayer.shootCoolDown - 0.0825f;
        captureCooldown = thePlayer.captureCoolDown;
        */
    }


    void Update()
    {
        if (thePlayer.isCapturing == true)
        {
            captureUI.fillAmount = 1;
        }
        rollCooldown = thePlayer.dashCoolDown;
        trapCooldown = thePlayer.shootCoolDown - 0.0825f;
        //captureCooldown = thePlayer.captureAfterSec + .825f;
        captureCooldown = thePlayer.captureCoolDown;
        trapCount = thePlayer.getTrapCount();

        if (GameManager.theManager.canMove == true)
        {
            buttonControl();
        }

        updateLives();
        updateScore();
        updateTrapCount();
        updateCaptureIcon();
        updateUpgradeButton();
        udpateChainText();
        updateTrapBarCurrent();

        if (trapUI.fillAmount < 1)
        {
            updateTrap();
        }
        else
        {
            colorTrap();
        }

        if (captureUI.fillAmount < 1)
        {
            updateCapture();
        }
        else
        {
            colorCapture();
        }

        if (rollUI.fillAmount < 1)
        {
            updateRoll();
        }
        else
        {
            colorRoll();
        }
    }

    private void buttonControl()
    {
        if ((Input.GetMouseButton(1) || Input.GetKey(KeyCode.L)) && trapUI.fillAmount == 1 && trapCount > 0)
        {
            activateTrap();
            trapBarChargeControl();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) || (Input.GetKeyUp(KeyCode.L)))
        {
            resetTrapCharge();
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.K)) && captureUI.fillAmount == 1)
            activateCapture();
        if (Input.GetKeyDown(KeyCode.Space) && rollUI.fillAmount == 1)
            activateRoll();
    }

    // Method Used to show the menu screen
    public void showMenu()
    {
        mainMenuGroup.alpha = 1f;
        mainMenuGroup.blocksRaycasts = true;
        mainMenuGroup.interactable = true;
    }

    // Method used to hide the menu screen
    public void hideMenu()
    {
        mainMenuGroup.alpha = 0f;
        mainMenuGroup.blocksRaycasts = false;
        mainMenuGroup.interactable = false;
    }

    public void showOption()
    {
        optionGroup.alpha = 1f;
        optionGroup.blocksRaycasts = true;
        optionGroup.interactable = true;
    }

    public void hideOption()
    {
        optionGroup.alpha = 0f;
        optionGroup.blocksRaycasts = false;
        optionGroup.interactable = false;
    }

    // Method used to show the lose screen
    public void showLost()
    {
        lostScreenGroup.alpha = 1f;
        lostScreenGroup.blocksRaycasts = true;
        lostScreenGroup.interactable = true;
    }

    // Method used to hide the Lose screen
    public void hideLost()
    {
        lostScreenGroup.alpha = 0f;
        lostScreenGroup.blocksRaycasts = false;
        lostScreenGroup.interactable = false;
    }

    // Method used to show the result screen
    public void showResult()
    {

        resultScreenGroup.alpha = 1f;
        resultScreenGroup.blocksRaycasts = true;
        resultScreenGroup.interactable = true;
    }

    IEnumerator Play(float seconds, string song)
    {
        //Wait a bit
        yield return StartCoroutine(WaitIn(seconds));
        //Do stuff
        FindObjectOfType<AudioManager>().Play(song);
    }

    IEnumerator WaitIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // Method used to hide the result screen
    public void hideResult()
    {

        resultScreenGroup.alpha = 0f;
        resultScreenGroup.blocksRaycasts = false;
        resultScreenGroup.interactable = false;
    }

    public void defaultUpgrade()
    {
        trapMax = false;
        captureMax = false;
        rollMax = false;
        lifeMax = false;
    }

    // Method used to show the upgrade screen
    public void showUpgrade()
    {
        upgradeScreenGroup.alpha = 1f;
        upgradeScreenGroup.blocksRaycasts = true;
        upgradeScreenGroup.interactable = true;
    }

    // Method used to hide the upgrade screen
    public void hideUpgrade()
    {
        upgradeScreenGroup.alpha = 0f;
        upgradeScreenGroup.blocksRaycasts = false;
        upgradeScreenGroup.interactable = false;
    }

    public void helpController()
    {
        showHelp();
        helpPageOne();
    }

    public void showHelp()
    {
        helpScreenGroup.alpha = 1f;
        helpScreenGroup.interactable = true;
        helpScreenGroup.blocksRaycasts = true;
    }

    public void hideHelp()
    {
        helpScreenGroup.alpha = 0f;
        helpScreenGroup.interactable = false;
        helpScreenGroup.blocksRaycasts = false;
    }

    public void helpPageOne()
    {
        showFirstHelp();
        hideSecondHelp();
        hideThirdHelp();
    }

    public void helpPageTwo()
    {
        showSecondHelp();
        hideFirstHelp();
        hideThirdHelp();
    }

    public void helpPageThree()
    {
        hideFirstHelp();
        hideSecondHelp();
        showThirdHelp();
    }

    public void showFirstHelp()
    {
        GameObject pageHelp = helpScreenUI.transform.Find("Page Help").gameObject;
        CanvasGroup firstTutorialGroup = tutorialOne.GetComponent<CanvasGroup>();
        pageHelp.GetComponent<Text>().text = "1/3";

        firstTutorialGroup.alpha = 1;
        firstTutorialGroup.interactable = true;
        firstTutorialGroup.blocksRaycasts = true;
    }

    public void hideFirstHelp()
    {
        CanvasGroup firstTutorialGroup = tutorialOne.GetComponent<CanvasGroup>();

        firstTutorialGroup.alpha = 0;
        firstTutorialGroup.interactable = false;
        firstTutorialGroup.blocksRaycasts = false;
    }

    public void showSecondHelp()
    {
        GameObject pageHelp = helpScreenUI.transform.Find("Page Help").gameObject;
        CanvasGroup secondTutorialGroup = tutorialTwo.GetComponent<CanvasGroup>();
        pageHelp.GetComponent<Text>().text = "2/3";

        secondTutorialGroup.alpha = 1;
        secondTutorialGroup.interactable = true;
        secondTutorialGroup.blocksRaycasts = true;
    }

    public void hideSecondHelp()
    {
        CanvasGroup secondTutorialGroup = tutorialTwo.GetComponent<CanvasGroup>();

        secondTutorialGroup.alpha = 0;
        secondTutorialGroup.interactable = false;
        secondTutorialGroup.blocksRaycasts = false;
    }

    public void showThirdHelp()
    {
        GameObject pageHelp = helpScreenUI.transform.Find("Page Help").gameObject;
        CanvasGroup thirdTutorialGroup = tutorialThree.GetComponent<CanvasGroup>();
        pageHelp.GetComponent<Text>().text = "3/3";

        thirdTutorialGroup.alpha = 1;
        thirdTutorialGroup.interactable = true;
        thirdTutorialGroup.blocksRaycasts = true;
    }

    public void hideThirdHelp()
    {
        CanvasGroup thirdTutorialGroup = tutorialThree.GetComponent<CanvasGroup>();

        thirdTutorialGroup.alpha = 0;
        thirdTutorialGroup.interactable = false;
        thirdTutorialGroup.blocksRaycasts = false;
    }

    public void showSendScore()
    {
        sendScoreGroup.alpha = 1;
        sendScoreGroup.interactable = true;
        sendScoreGroup.blocksRaycasts = true;
    }

    public void hideSendScore()
    {
        sendScoreGroup.alpha = 0;
        sendScoreGroup.interactable = false;
        sendScoreGroup.blocksRaycasts = false;
    }

    // Method used to update the result screen. 
    public void updateResult()
    {
        GameObject statisticBG = resultScreenUI.transform.Find("Statistic Background").gameObject;
        GameObject scoreText = statisticBG.transform.Find("ScoreText").gameObject;
        GameObject stageText = statisticBG.transform.Find("StageText").gameObject;
        GameObject timeText = statisticBG.transform.Find("TimeText").gameObject;
        GameObject clearText = statisticBG.transform.Find("ClearText").gameObject;
        GameObject chainText = statisticBG.transform.Find("ChainText").gameObject;

        scoreText.GetComponent<Text>().text = "Current Score: " + RunStatistics.Instance.totalScore;
        stageText.GetComponent<Text>().text = "Stage Cleared: " + RunStatistics.Instance.stagesCleared;
        timeText.GetComponent<Text>().text = "Session Time: " + System.Math.Round(RunStatistics.Instance.time, 2);
        clearText.GetComponent<Text>().text = "Bubble Cleared: " + RunStatistics.Instance.bubblesCleared;
        int chainCleard = 0;
        for (int i = 0; i < RunStatistics.Instance.bubblesChainCleared.Length; i++)
        {
            chainCleard += RunStatistics.Instance.bubblesChainCleared[i];
        }

        chainText.GetComponent<Text>().text = "Bubble Chained: " + chainCleard;
    }

    // Method used to update the lost screen.
    public void updateLost()
    {
        GameObject statisticBG = lostScreenUI.transform.Find("Statistic Background").gameObject;
        GameObject scoreText = statisticBG.transform.Find("ScoreText").gameObject;
        GameObject stageText = statisticBG.transform.Find("StageText").gameObject;
        GameObject timeText = statisticBG.transform.Find("TimeText").gameObject;
        GameObject clearText = statisticBG.transform.Find("ClearText").gameObject;
        GameObject chainText = statisticBG.transform.Find("ChainText").gameObject;

        scoreText.GetComponent<Text>().text = "Current Score: " + RunStatistics.Instance.totalScore;
        stageText.GetComponent<Text>().text = "Stage Cleared: " + RunStatistics.Instance.stagesCleared;
        timeText.GetComponent<Text>().text = "Session Time: " + System.Math.Round(RunStatistics.Instance.time, 2);
        clearText.GetComponent<Text>().text = "Bubble Cleared: " + RunStatistics.Instance.bubblesCleared;
        int chainCleard = 0;
        for (int i = 0; i < RunStatistics.Instance.bubblesChainCleared.Length; i++)
        {
            chainCleard += RunStatistics.Instance.bubblesChainCleared[i];
        }

        chainText.GetComponent<Text>().text = "Bubble Chained: " + chainCleard;
    }

    // Method used to update the upgrade screen.
    public void updateUpgrade()
    {
        GameObject upgradeBG = upgradeScreenUI.transform.Find("Upgrade Background").gameObject;
        GameObject trapText = upgradeBG.transform.Find("trapText").gameObject;
        GameObject captureText = upgradeBG.transform.Find("captureText").gameObject;
        GameObject rollText = upgradeBG.transform.Find("rollText").gameObject;
        GameObject lifeText = upgradeBG.transform.Find("lifeText").gameObject;

        trapText.GetComponent<Text>().text = "Bubble Trap Cap: " + thePlayer.trapCountCap + " (Max 10)";
        captureText.GetComponent<Text>().text = "Bubble Capture Cooldown: " + thePlayer.captureCoolDown + " (Max 1)";
        rollText.GetComponent<Text>().text = "Roll Cooldown: " + thePlayer.dashCoolDown + " (Max 4)";
        lifeText.GetComponent<Text>().text = "Player Life: " + RunStatistics.Instance.currentLife + " (Max 3)";
    }

    public void updateUpgradeButton()
    {
        if (RunStatistics.Instance.totalScore < 500)
        {
            hideTrapButton();
            hideCaptureButton();
            hideRollButton();
            hideLifeButton();
        }
        else if (RunStatistics.Instance.totalScore > 500 && RunStatistics.Instance.totalScore < 1000)
        {
            if (trapMax != true)
            {
                showTrapButton();
            }
            else
            {
                hideTrapButton();
            }

            if (captureMax != true)
            {
                showCaptureButton();
            }
            else
            {
                hideCaptureButton();
            }
            if (rollMax != true)
            {
                showRollButton();
            }
            else
            {
                hideRollButton();
            }
            hideLifeButton();
        }
        else if (RunStatistics.Instance.totalScore > 1000)
        {
            if (trapMax != true)
            {
                showTrapButton();
            }
            else
            {
                hideTrapButton();
            }

            if (captureMax != true)
            {
                showCaptureButton();
            }
            else
            {
                hideCaptureButton();
            }

            if (rollMax != true)
            {
                showRollButton();
            }
            else
            {
                hideRollButton();
            }

            if (RunStatistics.Instance.currentLife < 3)
            {
                lifeMax = false;
            }
            else
            {
                lifeMax = true;
            }

            if (lifeMax != true)
            {
                showLifeButton();
            }
            else
            {
                hideLifeButton();
            }
        }

    }

    public void showTrapButton()
    {
        CanvasGroup trapButtonCanvas = trapButtonUI.GetComponent<CanvasGroup>();

        trapButtonCanvas.alpha = 1f;
        trapButtonCanvas.interactable = true;
        trapButtonCanvas.blocksRaycasts = true;

    }

    public void hideTrapButton()
    {
        CanvasGroup trapButtonCanvas = trapButtonUI.GetComponent<CanvasGroup>();

        trapButtonCanvas.alpha = 0.25f;
        trapButtonCanvas.interactable = false;
        trapButtonCanvas.blocksRaycasts = false;
    }

    public void showCaptureButton()
    {
        CanvasGroup captureButtonCanvas = captureButtonUI.GetComponent<CanvasGroup>();

        captureButtonCanvas.alpha = 1f;
        captureButtonCanvas.interactable = true;
        captureButtonCanvas.blocksRaycasts = true;
    }

    public void hideCaptureButton()
    {
        CanvasGroup captureButtonCanvas = captureButtonUI.GetComponent<CanvasGroup>();

        captureButtonCanvas.alpha = 0.25f;
        captureButtonCanvas.interactable = false;
        captureButtonCanvas.blocksRaycasts = false;
    }

    public void showRollButton()
    {
        CanvasGroup rollButtonCanvas = rollButtonUI.GetComponent<CanvasGroup>();

        rollButtonCanvas.alpha = 1f;
        rollButtonCanvas.interactable = true;
        rollButtonCanvas.blocksRaycasts = true;
    }

    public void hideRollButton()
    {
        CanvasGroup rollButtonCanvas = rollButtonUI.GetComponent<CanvasGroup>();

        rollButtonCanvas.alpha = 0.25f;
        rollButtonCanvas.interactable = false;
        rollButtonCanvas.blocksRaycasts = false;
    }

    public void showLifeButton()
    {
        CanvasGroup lifeButtonCanvas = lifeButtonUI.GetComponent<CanvasGroup>();

        lifeButtonCanvas.alpha = 1f;
        lifeButtonCanvas.interactable = true;
        lifeButtonCanvas.blocksRaycasts = true;
    }

    public void hideLifeButton()
    {
        CanvasGroup lifeButtonCanvas = lifeButtonUI.GetComponent<CanvasGroup>();

        lifeButtonCanvas.alpha = 0.25f;
        lifeButtonCanvas.interactable = false;
        lifeButtonCanvas.blocksRaycasts = false;
    }

    public void showChainText()
    {
        chainTextGroup.alpha = 1;
    }

    public void hideChainText()
    {
        chainTextGroup.alpha = 0;
    }

    public void setCost(int upgradeCost)
    {
        GameObject upgradeBG = upgradeScreenUI.transform.Find("Upgrade Background").gameObject;
        GameObject costText = upgradeBG.transform.Find("costText").gameObject;
        CanvasGroup costCanvas = costText.GetComponent<CanvasGroup>();
        costCanvas.alpha = 1;
        costText.GetComponent<Text>().text = "Cost: " + upgradeCost + " Points";
    }

    public void hideCost()
    {
        GameObject upgradeBG = upgradeScreenUI.transform.Find("Upgrade Background").gameObject;
        GameObject costText = upgradeBG.transform.Find("costText").gameObject;
        CanvasGroup costCanvas = costText.GetComponent<CanvasGroup>();
        costCanvas.alpha = 0;
    }

    public void setUpgradeHelp(string upgradeHelp)
    {
        GameObject upgradeBG = upgradeScreenUI.transform.Find("Upgrade Background").gameObject;
        GameObject upgradeHelpText = upgradeBG.transform.Find("upgradeHelp").gameObject;
        CanvasGroup upgradeHelpCanvas = upgradeHelpText.GetComponent<CanvasGroup>();
        upgradeHelpCanvas.alpha = 1;
        upgradeHelpText.GetComponent<Text>().text = upgradeHelp;
    }

    public void hideUpgradeHelp()
    {
        GameObject upgradeBG = upgradeScreenUI.transform.Find("Upgrade Background").gameObject;
        GameObject upgradeHelpText = upgradeBG.transform.Find("upgradeHelp").gameObject;
        CanvasGroup upgradeHelpCanvas = upgradeHelpText.GetComponent<CanvasGroup>();
        upgradeHelpCanvas.alpha = 0;
    }

    public void activateRoll()
    {
        rollUI.fillAmount = 0;
    }

    private void colorRoll()
    {
        GameObject iconHUD = rollUI.transform.Find("rollHUD").gameObject;
        Image iconImage = iconHUD.GetComponent<Image>();
        iconImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void updateRoll()
    {
        rollUI.fillAmount += 1 / rollCooldown * Time.smoothDeltaTime;
        GameObject iconHUD = rollUI.transform.Find("rollHUD").gameObject;
        Image iconImage = iconHUD.GetComponent<Image>();
        iconImage.GetComponent<Image>().color = new Color32(90, 89, 89, 255);
    }

    public void activateTrap()
    {
        trapUI.fillAmount = 0;
    }

    private void colorTrap()
    {
        GameObject iconHUD = trapUI.transform.Find("trapHUD").gameObject;
        Image iconImage = iconHUD.GetComponent<Image>();
        iconImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void updateTrap()
    {
        trapUI.fillAmount += 1 / trapCooldown * Time.smoothDeltaTime;
        GameObject iconHUD = trapUI.transform.Find("trapHUD").gameObject;
        Image iconImage = iconHUD.GetComponent<Image>();
        iconImage.GetComponent<Image>().color = new Color32(90, 89, 89, 255);
    }

    public void activateCapture()
    {
        captureUI.fillAmount = 0;
    }

    private void colorCapture()
    {
        GameObject iconHUD = captureUI.transform.Find("captureHUD").gameObject;
        Image iconImage = iconHUD.GetComponent<Image>();
        iconImage.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }

    public void updateCaptureIcon()
    {
        GameObject iconHUD = captureUI.transform.Find("captureHUD").gameObject;

        if (thePlayer.getbubbleSprite() != null)
        {
            switch (thePlayer.getbubbleSprite().color)
            {
                case 0:
                    iconHUD.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/bubble_red_bright");
                    break;
                case 1:
                    iconHUD.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/bubble_blue_bright");
                    break;
                case 2:
                    iconHUD.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/bubble_yellow_bright");
                    break;
                case 3:
                    iconHUD.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/bubble_purple_bright");
                    break;
                case 4:
                    iconHUD.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/bubble_purple_bright");
                    break;
                default:
                    break;
            }
        }
        else
        {
            iconHUD.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/capture");
        }
    }

    public void updateCapture()
    {
        captureUI.fillAmount += 1 / captureCooldown * Time.smoothDeltaTime;
        GameObject iconHUD = captureUI.transform.Find("captureHUD").gameObject;
        Image iconImage = iconHUD.GetComponent<Image>();
        iconImage.GetComponent<Image>().color = new Color32(90, 89, 89, 255);
    }

    public void updateSendScore()
    {
        GameObject sendBackground = sendScoreUI.transform.Find("SendBackground").gameObject;
        GameObject sendScore = sendBackground.transform.Find("SendScore").gameObject;
        Text sendScoreText = sendScore.GetComponent<Text>();
        sendScoreText.text = "Your Score: " + RunStatistics.Instance.totalScore;
    }

    public void updateLives()
    {
        playerLifeUI.text = "x" + RunStatistics.Instance.currentLife;
    }

    public void updateScore()
    {
        scoreUI.text = "SCORE " + RunStatistics.Instance.totalScore;
    }
    public void updateTrapCount()
    {
        trapCountUI.text = "x" + RunStatistics.Instance.trapCount;
    }

    public void updateStage()
    {
        stageUI.text = "STAGE " + RunStatistics.Instance.currentStage;
    }

    public void baseChainText()
    {
        chainTextUI.text = (GameManager.theManager.chainCount + 1) + " CHAIN COMBO";
    }

    public void pointChainText()
    {
        chainTextUI.fontSize = 28;
        chainTextUI.text = GameManager.theManager.chainScore + " POINTS!";
    }

    public void defaultChainText()
    {
        chainTextUI.text = GameManager.theManager.chainCount + 1 + " CHAIN COMBO";
        chainTextUI.fontSize = 20;
        GameManager.theManager.chainScore = 0;
    }

    public void udpateChainText()
    {
        if (isChain == true)
        {
            updateChainTime();
            updateScoreTime();
        }
        else
        {
            hideChainText();
            defaultChainText();
        }
    }

    public void updateChainTime()
    {
        if (chainTime > 0f)
        {
            showChainText();
            baseChainText();
            chainTime -= Time.deltaTime;
            if (chainTime <= 0)
            {
                scoreTime = 1f;
            }
        }
    }

    public void updateScoreTime()
    {
        if (scoreTime > 0f)
        {
            pointChainText();
            scoreTime -= Time.deltaTime;
            if (scoreTime <= 0)
            {
                isChain = false;
            }
        }
    }

    public void addClearIndicator()
    {
        GameObject e = Instantiate(Resources.Load("Prefabs/ClearPointIndicator")) as GameObject;
        e.transform.SetParent(playerHUDUI.transform, false);
    }

    public void updateTrapBarCurrent()
    {
        trapBarCurrentUI.fillAmount = thePlayer.trapCount / 10f;
    }

    /* Control Method For the trapBarCharge
     * 
     * Control the trapBarCharge color indicator
     * As the player held onto RMB or the L key, the bar will charge
     * The fill amount does not go past thePlayer.trapCount/10
     * The fill amount will stop at 0.4 if splash has not been unlocked
     * The fill amount will stop at 0.7 if the beam has not been unlocked
     * */
    public void trapBarChargeControl()
    {
        if (currentCharge <= thePlayer.trapCount / 10f)
        {
            Debug.Log("this is the trap count" + thePlayer.trapCountCap);
            currentCharge += .2f;
            if (currentCharge > thePlayer.trapCount / 10f)
            {
                currentCharge = thePlayer.trapCount / 10f;
            }
            updateTrapBarCharge();
        }
        else
        {
            currentCharge = thePlayer.trapCount / 10f;
        }

    }

    public void updateTrapBarCharge()
    {
        trapBarChargeUI.fillAmount = currentCharge;
    }

    public void resetTrapCharge()
    {
        currentCharge = 0;
        trapBarChargeUI.fillAmount = 0;
    }


}
