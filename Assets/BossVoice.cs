using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVoice : FatherEnemy
{
    private BossRangeOfActivation rangeOfActivation;
    private enum State
    {
        Waiting,
        Projectiles,
        Plataform,
        Altars,
        Laser,
        LaserBall,
        SwitchFase,
        Dead
    }

    private State currentState;
    private State[] statesToRandomize;

    private bool isActivated = false,
        switchingFase = false,
        hasInmunity = true,
        firstLoop = true,
        hasAltars = false,
        isAltars = false,
        isStarting = true,
        spikes = false,
        platforming = false,
        doAdvice = false,
        laseringBall = false;

    public HealthBarBoss healthBar;

    public int pointsToGive = 100;

    private int actualFase = 0,
        numAltars = 2,
        numAltarsV2 = 4,
        numAltarsV3 = 6,
        platformNumber,
        platformNumberNext,
        type,
        direction = 1;

    [SerializeField]
    private int ammountBullets = 4,
        ammountBulletsV2 = 5,
        ammountBulletsV3 = 6;

    private int[] activateAltars;

    [SerializeField]
    private float maxHealth = 500f,
        rateBullets = 2f,
        rateBulletsV3 = 1.5f,
        waitingDuration = 4f,
        waitingDurationV2 = 3f,
        waitingDurationV3 = 2f,
        altarsPreDuration = 6f,
        altarsPreDurationV2 = 6f,
        altarsPreDurationV3 = 8f,
        altarsDuration = 2f,
        projectilesDuration = 6f,
        plataformAnimationDuration = 1.5f,
        plataformDuration = 1f,
        plataformDurationV2 = 1f,
        platformAdviceDuration = 1f,
        plataformsMaxDuration = 10f,
        plataformsMaxDurationV2 = 10.5f,
        switchFaseDuration = 2f,
        spikesDuration = 6f,
        spikesDurationV2 = 5f,
        spikesExtraDuration = 1f,
        spikesExtraDuration2 = 1f,
        laserDuration = 1f,
        laserDurationV3 = 1f,
        laserAnimationDuration = 1.5f,
        laserStartAngle = 225,
        laserEndAngle = -45,
        laserStartAngleV2 = 135,
        laserEndAngleV2 = 405,
        laserStartAngleV3 = 225,
        laserEndAngleV3 = -45,
        laserBallAnimationDuration = 1,
        laserBallPreDuration = 3,
        laserBallNoActivationDuration = 2,
        laserBallNoActivationDurationV3 = 2,
        laserBallDuration = 5,
        laserBallSpeed = 25,
        laserBallSpeedV3 = 15;

    private float currentHealth, 
        waitingStartTime,
        altarsStartTime,
        projectilesStartTime,
        plataformStartTime,
        nextPlatformStartTime,
        switchFaseStartTime,
        spikesStartTime,
        laserStartTime,
        laserBallStartTime;

    [SerializeField]
    private ParticleSystem
        groundParticles,
        roofParticles,
        rightParticles,
        leftParticles;

    public List<AltarBehaviour> altars = new List<AltarBehaviour>();
    public List<GameObject> platforms = new List<GameObject>();
    public List<GameObject> lasers = new List<GameObject>();
    public List<GameObject> preLasers = new List<GameObject>();

    [SerializeField]
    private GameObject shield,
        sprite,
        portal,
        portalSpawnPoint,
        endPos,
        platform1,
        platform2,
        groundSpikes,
        endGroundSpikes,
        roofSpikes,
        endRoofSpikes,
        leftSpikes,
        rightSpikes,
        startPlatforms,
        endPlatforms,
        advice,
        laser,
        laserCenter,
        laserBall,
        spotA,
        spotB;

    private Animator spriteAnimator;

    private Vector3 
        startPos,
        spikestStartPos,
        spikestStartPos2,
        platformsStartPos;

    [SerializeField]
    private AudioClip clip,
        clip2,
        clip3,
        clip4,
        clip5,
        clip6,
        clip7;

    [SerializeField]
    private AudioSource audio,
        audio2,
        audio3,
        audio4,
        audio5,
        audio6,
        audio7;

    // Start is called before the first frame update
    void Start()
    {
        rangeOfActivation = transform.Find("Range").gameObject.GetComponent<BoxCollider2D>().GetComponent<BossRangeOfActivation>();
        spriteAnimator = sprite.GetComponent<Animator>();
        GetComponent<FireVoiceBullets>().SetAmmount(ammountBullets);
        currentHealth = maxHealth;
        SwitchState(State.Waiting);
        startPos = sprite.transform.position;
        spikestStartPos = groundSpikes.transform.position;
        spikestStartPos2 = roofSpikes.transform.position;
        platformsStartPos = startPlatforms.transform.position;

        statesToRandomize = new State[6];
        statesToRandomize[0] = State.Projectiles;
        statesToRandomize[1] = State.Projectiles;
        statesToRandomize[2] = State.Projectiles;
        statesToRandomize[3] = State.Plataform;
        statesToRandomize[4] = State.Laser;
        statesToRandomize[5] = State.LaserBall;
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<FireVoiceBullets>().Shoot();
        if (!isStarting)
        {
            bool found = false;

            foreach (AltarBehaviour o in altars)
            {
                if (o.gameObject.activeInHierarchy)
                {
                    found = true;
                }
            }

            if (!found)
            {
                hasAltars = false;
            }
            else
            {
                hasAltars = true;
            }
            if (!hasAltars)
            {
                hasInmunity = false;
            }
            else
            {
                hasInmunity = true;
            }
            if (!isAltars)
            {
                if (!hasInmunity)
                {
                    shield.SetActive(false);
                    audio.clip = clip;
                    audio.Play();
                    SwitchState(State.Altars);
                }
                else
                {
                    shield.SetActive(true);
                }
            }
        }

        #region Switch fase
        if (healthBar.GetPercentageOfHealth() <= 0.66 && actualFase == 1)
        {
            actualFase = 2;
            //previousFase = 1;
            SwitchState(State.SwitchFase);

        }
        if (healthBar.GetPercentageOfHealth() <= 0.33 && actualFase == 2)
        {
            actualFase = 3;
            //previousFase = 2;
            SwitchState(State.SwitchFase);
        }
        #endregion

        #region Spikes
        if (spikes)
        {

            if (Time.time >= spikesStartTime + spikesDuration)
            {
                if (Time.time >= spikesStartTime + spikesDuration + spikesExtraDuration)
                {
                    if (actualFase == 2)
                    {
                        groundSpikes.transform.position = spikestStartPos;
                        spikes = false;
                    }

                    //groundSpikes.SetActive(false);


                    if(actualFase == 3)
                    {
                        roofSpikes.transform.position = spikestStartPos2;
                        platform1.SetActive(false);
                        platform2.SetActive(false);
                        if (Time.time >= spikesStartTime + spikesDuration + spikesExtraDuration + spikesExtraDuration2)
                        {
                            spikes = false;
                            leftParticles.Play();
                            rightParticles.Play();
                            leftSpikes.SetActive(true);
                            rightSpikes.SetActive(true);
                            Camera.main.GetComponent<Animator>().SetTrigger("shake");
                        }

                    }
                }
            }
            else
            {
                float value = spikestStartPos.y;
                if (actualFase == 2)
                {
                    value = Lerp(spikestStartPos.y, endGroundSpikes.transform.position.y, spikesStartTime, spikesDuration);
                    groundSpikes.transform.position = new Vector3(groundSpikes.transform.position.x, value, groundSpikes.transform.position.z);
                }
                if (actualFase == 3)
                {
                    value = Lerp(spikestStartPos2.y, endRoofSpikes.transform.position.y, spikesStartTime, spikesDuration);
                    roofSpikes.transform.position = new Vector3(roofSpikes.transform.position.x, value, roofSpikes.transform.position.z);
                }

                
            }
        }
        #endregion

        #region Platform
        if (platforming)
        {
            if (Time.time >= plataformStartTime + plataformAnimationDuration + plataformsMaxDuration)
            {
                platforming = false;
                platforms[platformNumber].transform.position = platformsStartPos;
                platforms[platformNumber].SetActive(false);
                advice.SetActive(false);
            }
            else
            {
                if (Time.time >= nextPlatformStartTime + plataformDuration - platformAdviceDuration && doAdvice && !(Time.time >= plataformStartTime + plataformAnimationDuration + plataformsMaxDuration - plataformDuration))
                {
                    //fer advice
                    platformNumberNext = Random.Range(0, platforms.Count);
                    advice.transform.position = new Vector3(platforms[platformNumberNext].transform.position.x, advice.transform.position.y, advice.transform.position.z);
                    advice.SetActive(true);
                    advice.GetComponent<Animator>().Play("Advice");
                    doAdvice = false;
                }
                if (Time.time >= nextPlatformStartTime + plataformDuration)
                {
                    //acaba la platform d'ara
                    nextPlatformStartTime = Time.time;
                    platforms[platformNumber].transform.position = platformsStartPos;
                    platforms[platformNumber].SetActive(false);

                    platformNumber = platformNumberNext;

                    platformsStartPos = platforms[platformNumber].transform.position;
                    platforms[platformNumber].SetActive(true);
                    doAdvice = true;
                    advice.SetActive(false);
                }
                else
                {
                    float value = Lerp(platformsStartPos.y, endPlatforms.transform.position.y, nextPlatformStartTime, plataformDuration);

                    platforms[platformNumber].transform.position = new Vector3(platforms[platformNumber].transform.position.x, value, platforms[platformNumber].transform.position.z);
                }
            }
            /*
            if (Time.time >= plataformStartTime + plataformAnimationDuration + plataformDuration)
            {

                //platforming = false;
                platforms[platformNumber].transform.position = platformsStartPos;
                platforms[platformNumber].SetActive(false);
            }
            else
            {

                float value = Lerp(platformsStartPos.y, endPlatforms.transform.position.y, (plataformStartTime + plataformAnimationDuration), plataformDuration);

                platforms[platformNumber].transform.position = new Vector3(platforms[platformNumber].transform.position.x, value, platforms[platformNumber].transform.position.z);
                //platforms[platformNumber].transform.position = Vector3.MoveTowards(platforms[platformNumber].transform.position, endPlatforms.transform.position, 20 * Time.deltaTime);
            }*/
        }
        #endregion

        #region LaserBall
        if (laseringBall)
        {
            if (Time.time >= laserBallStartTime + laserBallAnimationDuration + laserBallPreDuration + laserBallDuration)
            {
                laseringBall = false;
                laserBall.SetActive(false);
                preLasers[0].SetActive(false);
                preLasers[1].SetActive(false);
                preLasers[2].SetActive(false);
                preLasers[3].SetActive(false);
                lasers[0].SetActive(false);
                lasers[1].SetActive(false);
                lasers[2].SetActive(false);
                lasers[3].SetActive(false);
            }
            else
            {
                laserBall.transform.Rotate(0, 0, laserBallSpeed * Time.deltaTime * direction);
                laserBall.SetActive(true);
                if (Time.time >= laserBallStartTime + laserBallAnimationDuration + laserBallPreDuration)
                {
                    //no cal(?)
                    if (actualFase == 2)
                    {

                        lasers[0].SetActive(true);
                        lasers[1].SetActive(true);

                        //laserBall.transform.position = spotB.transform.position;
                    }
                    else
                    {
                        laserBall.SetActive(true);
                        laserBall.transform.position = spotA.transform.position;

                        if (actualFase == 1)
                        {
                            lasers[0].SetActive(true);

                        }
                        else
                        {

                            lasers[0].SetActive(true);
                            lasers[1].SetActive(true);
                            lasers[2].SetActive(true);
                            lasers[3].SetActive(true);
                        }
                    }
                }
                else
                {
                    if (Time.time >= laserBallStartTime + laserBallAnimationDuration + laserBallNoActivationDuration)
                    {
                        if (actualFase == 2)
                        {

                            lasers[0].SetActive(true);
                            lasers[1].SetActive(true);

                            //laserBall.transform.position = spotB.transform.position;
                        }
                        else
                        {
                            //laserBall.transform.position = spotA.transform.position;
                            if (actualFase == 1)
                            {
                                lasers[0].SetActive(true);

                            }
                            else
                            {
                                lasers[0].SetActive(true);
                                lasers[1].SetActive(true);
                                lasers[2].SetActive(true);
                                lasers[3].SetActive(true);
                            }
                        }
                    }
                    float pos;
                    if (actualFase == 2)
                    {
                        pos = Lerp(laserCenter.transform.position.y, spotB.transform.position.y, (laserBallStartTime + laserBallAnimationDuration), laserBallPreDuration);
                        preLasers[0].SetActive(true);
                        preLasers[1].SetActive(true);
                    }
                    else
                    {
                        pos = Lerp(laserCenter.transform.position.y, spotA.transform.position.y, (laserBallStartTime + laserBallAnimationDuration), laserBallPreDuration);
                        if (actualFase == 1)
                        {
                            preLasers[0].SetActive(true);
                        }
                        else
                        {
                            preLasers[0].SetActive(true);
                            preLasers[1].SetActive(true);
                            preLasers[2].SetActive(true);
                            preLasers[3].SetActive(true);
                        }
                    }
                    laserBall.transform.position = new Vector3(laserBall.transform.position.x, pos, laserBall.transform.position.z);
                }
            }
        }
        #endregion

        switch (currentState)
        {

            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.Projectiles:
                UpdateProjectilesState();
                break;
            case State.Plataform:
                UpdatePlataformState();
                break;
            case State.Altars:
                UpdateAltarsState();
                break;
            case State.Laser:
                UpdateLaserState();
                break;
            case State.LaserBall:
                UpdateLaserBallState();
                break;
            case State.SwitchFase:
                UpdateSwitchFaseState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    //-------WAITING-----------
    #region WAITING
    private void EnterWaitingState()
    {
        if (isActivated)
        {
            waitingStartTime = Time.time;
        }

    }

    private void UpdateWaitingState()
    {
        if (!isActivated)
        {
            if (rangeOfActivation.inRange())
            {
                waitingStartTime = Time.time;
                healthBar.SetMaxHealth(maxHealth);
                healthBar.gameObject.SetActive(true);
                isActivated = true;
                actualFase = 1;
                SwitchState(State.Altars);
            }

        }
        else
        {
            if (Time.time >= waitingStartTime + waitingDuration)
            {
                if (!hasAltars) {
                    SwitchState(State.Altars);
                }
                else
                {
                    //SwitchState(State.Projectiles);
                    //SwitchState(State.Plataform);
                    SwitchState(RandomBehaviour());
                }
            }

        }
    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //------PROJECTILES-----
    #region PROJECTILES
    private void EnterProjectilesState()
    {
        projectilesStartTime = Time.time;
        firstLoop = true;
        audio5.clip = clip5;
        audio5.Play();
    }
    private void UpdateProjectilesState()
    {

        if (Time.time >= projectilesStartTime + projectilesDuration){
            SwitchState(State.Waiting);
        }
        else
        {
            if (firstLoop)
            {
                firstLoop = false;
                GetComponent<FireVoiceBullets>().Shoot();
            }
        }
    }
    private void ExitProjectilesState()
    {
        GetComponent<FireVoiceBullets>().StopShoot();
    }
    #endregion

    //-----PLATAFORM------
    #region PLATAFORM
    private void EnterPlataformState()
    {
        plataformStartTime = Time.time;
        //platformNumber = Random.Range(0,platforms.Count);
        doAdvice = true;
        audio3.clip = clip3;
        audio3.Play();
    }
    private void UpdatePlataformState()
    {

        if (Time.time >= plataformStartTime + plataformAnimationDuration)
        {
            platforming = true;
            //platforms[platformNumber].SetActive(true);
            nextPlatformStartTime = Time.time;
            platformNumber = platformNumberNext;
            platformsStartPos = platforms[platformNumber].transform.position;
            platforms[platformNumber].SetActive(true);
            advice.SetActive(false);
            doAdvice = true;

            SwitchState(State.Waiting);
        }
        else
        {
            if (Time.time >= plataformStartTime + plataformAnimationDuration - platformAdviceDuration && doAdvice)
            {
                platformNumberNext = Random.Range(0, platforms.Count);
                advice.transform.position = new Vector3(platforms[platformNumberNext].transform.position.x, advice.transform.position.y, advice.transform.position.z);
                advice.SetActive(true);
                advice.GetComponent<Animator>().Play("Advice");
                doAdvice = false;
            }
        }
    }
    private void ExitPlataformState()
    {
        advice.SetActive(false);
    }
    #endregion

    //-------ALTARS------
    #region ALTARS
    private void EnterAltarsState()
    {
        altarsStartTime = Time.time;
        firstLoop = true;
        isAltars = true;
        Invoke("activateAltarsSound", altarsPreDuration - 1f);
    }
    private void UpdateAltarsState()
    {
        if (Time.time >= altarsStartTime + altarsPreDuration)
        {
            if (firstLoop)
            {

                firstLoop = false;
                activateAltars = new int[numAltars];
                bool isOK;
                if (isStarting)
                {
                    spriteAnimator.Play("VoiceSwitchFase");
                }
                else 
                {
                    spriteAnimator.Play("VoiceAltars");
                }

                //Altars to activate
                for (int i = 0; i < numAltars; i++)
                {
                    int attempt = 0;
                    isOK = false;
                    while (!isOK)
                    {
                        bool found = false;
                        attempt = Random.Range(0, altars.Count);

                        for (int j = 0; j < i; j++)
                        {
                            if (activateAltars[j] == attempt)
                            {
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            isOK = true;
                        }
                    }

                    activateAltars[i] = attempt;
                }

                foreach (int i in activateAltars)
                {
                    AltarBehaviour o = altars[i];
                    o.Activate();
                }
                isAltars = false;
            }
            if (Time.time >= altarsStartTime + altarsPreDuration + altarsDuration)
            {
                //SwitchState(State.Projectiles);
                SwitchState(RandomBehaviour());
            }
            else
            {
                //animacio invoca altars
            }
        }
        else
        {
            //animacio de pocho sense escut
        }
    }

    private void activateAltarsSound()
    {
        audio6.clip = clip6;
        audio6.Play();
    }

    private void ExitAltarsState()
    {
        isStarting = false;
        spriteAnimator.Play("VoiceIdle");
    }
    #endregion

    //-------LASER------
    #region LASER
    private void EnterLaserState()
    {
        laserStartTime = Time.time;
        type = Random.Range(0, 2);
        audio4.clip = clip4;
        audio4.Play();
    }
    private void UpdateLaserState()
    {
        if (Time.time >= laserStartTime + laserAnimationDuration)
        {
            if (Time.time >= laserStartTime + laserAnimationDuration + laserDuration)
            {

                SwitchState(State.Waiting);
            }
            else
            {
                laser.SetActive(true);
                float angle;
                if (type == 0) 
                {
                    angle = Lerp(laserEndAngle, laserStartAngle, laserStartTime + laserAnimationDuration, laserDuration);
                }
                else
                {
                    angle = Lerp(laserStartAngle, laserEndAngle, laserStartTime + laserAnimationDuration, laserDuration);
                }
                laser.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

    }
    private void ExitLaserState()
    {
        laser.SetActive(false);
        laser.GetComponentInChildren<Laser>().StopParticles();
    }
    #endregion

    //-------LASERBALL------
    #region LASERBALL
    private void EnterLaserBallState()
    {
        laserBallStartTime = Time.time;
        audio7.clip = clip7;
        audio7.Play();
        int val = Random.Range(0, 2);
        if (val == 0)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
    }
    private void UpdateLaserBallState()
    {
        if (Time.time >= laserBallStartTime + laserBallAnimationDuration)
        {
            laseringBall = true;
            laserBall.transform.position = laserCenter.transform.position;
            SwitchState(State.Waiting);
        }
    }
    private void ExitLaserBallState()
    {

    }
    #endregion

    //------SWITCHFASE----
    #region SWITCHFASE
    private void EnterSwitchFaseState()
    {
        laseringBall = false;
        laserBall.SetActive(false);
        preLasers[0].SetActive(false);
        preLasers[1].SetActive(false);
        preLasers[2].SetActive(false);
        preLasers[3].SetActive(false);
        lasers[0].SetActive(false);
        lasers[1].SetActive(false);
        lasers[2].SetActive(false);
        lasers[3].SetActive(false);
        CancelInvoke("activateAltarsSound");
        audio2.clip = clip2;
        audio2.Play();
        switch (actualFase)
        {
            case 1:
                //spriteAnimator.Play("boss1SwitchFaseAnimation1");

                break;
            case 2:
                numAltars = numAltarsV2;
                GetComponent<FireVoiceBullets>().SetAmmount(ammountBulletsV2);
                //GetComponent<FireVoiceBullets>().ChangeAngles(-165, 165);
                GetComponent<FireVoiceBullets>().ChangeAngles(-105, -255);
                groundParticles.Play();
                plataformDuration = plataformDurationV2;
                plataformsMaxDuration = plataformsMaxDurationV2;
                laserEndAngle = laserEndAngleV2;
                laserStartAngle = laserStartAngleV2;
                waitingDuration = waitingDurationV2;
                altarsPreDuration = altarsPreDurationV2;
                //spriteAnimator.Play("boss1SwitchFaseAnimation2");
                break;
            case 3:
                numAltars = numAltarsV3;
                groundSpikes.SetActive(false);
                roofParticles.Play();
                spikesDuration = spikesDurationV2;
                laserEndAngle = laserEndAngleV3;
                laserStartAngle = laserStartAngleV3;
                laserDuration = laserDurationV3;
                waitingDuration = waitingDurationV3;
                altarsPreDuration = altarsPreDurationV3;
                laserBallNoActivationDuration = laserBallNoActivationDurationV3;
                laserBallSpeed = laserBallSpeedV3;

                GetComponent<FireVoiceBullets>().SetAmmount(ammountBulletsV3);
                GetComponent<FireVoiceBullets>().SetFireRate(rateBulletsV3);
                GetComponent<FireVoiceBullets>().ChangeAngles(-75, 75);

                //spriteAnimator.Play("boss1SwitchFaseAnimation3");
                break;
        }

        switchingFase = true;
        switchFaseStartTime = Time.time;
    }
    private void UpdateSwitchFaseState()
    {
        Camera.main.GetComponent<Animator>().SetTrigger("shake");
        float value = startPos.y;
        if (actualFase == 2)
        {
            value = Lerp(startPos.y, endPos.transform.position.y, switchFaseStartTime, switchFaseDuration);
        }
        if(actualFase == 3)
        {
            value = Lerp( endPos.transform.position.y, startPos.y, switchFaseStartTime, switchFaseDuration);
        }

        sprite.transform.position = new Vector3( sprite.transform.position.x, value, sprite.transform.position.z);

        if (Time.time >= switchFaseStartTime + switchFaseDuration)
        {

            isStarting = true;
            hasInmunity = true;
            shield.SetActive(true);
            SwitchState(State.Altars);
        }
    }
    private void ExitSwitchFaseState()
    {
        if (actualFase == 2)
        {
            platform1.SetActive(true);
            platform2.SetActive(true);
        }
        spikes = true;
        spikesStartTime = Time.time;
        groundSpikes.SetActive(true);
        switchingFase = false;
    }
    #endregion

    //------DEAD------
    #region DEAD
    private void EnterDeadState()
    {
        healthBar.gameObject.SetActive(false);
        //Instantiate(hardSkinSoul, sprite.transform.position + new Vector3(0, 1, 0), hardSkinSoul.transform.rotation);
        GameObject p = Instantiate(portal, portalSpawnPoint.transform.position, portal.transform.rotation) as GameObject;
        p.GetComponent<EndLevel>().nextLevelName = "L1W4";
        Destroy(gameObject);
    }
    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }
    #endregion

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Waiting:
                ExitWaitingState();
                break;
            case State.Projectiles:
                ExitProjectilesState();
                break;
            case State.Plataform:
                ExitPlataformState();
                break;
            case State.Altars:
                ExitAltarsState();
                break;
            case State.Laser:
                ExitLaserState();
                break;            
            case State.LaserBall:
                ExitLaserBallState();
                break;
            case State.SwitchFase:
                ExitSwitchFaseState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Waiting:
                EnterWaitingState();
                break;
            case State.Projectiles:
                EnterProjectilesState();
                break;
            case State.Plataform:
                EnterPlataformState();
                break;
            case State.Altars:
                EnterAltarsState();
                break;
            case State.Laser:
                EnterLaserState();
                break;            
            case State.LaserBall:
                EnterLaserBallState();
                break;
            case State.SwitchFase:
                EnterSwitchFaseState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }
    private float Lerp(float start, float end, float timeStartedLerping, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        float result = Vector3.Lerp(new Vector3(start, 0, 0), new Vector3(end, 0, 0), percentageComplete).x;

        return result;
    }

    public override void applyKnockback(float[] position)
    {
        //Nothing
    }

    private State RandomBehaviour()
    {
        /*
        if (meteoring && venoming && isFrontAttack)
        {
            if (actualFase == 3)
            {
                return State.JumpAttack;
            }
            else
            {
                return State.Walking;
            }
        }*/

        bool isOk = false;
        while (!isOk)
        {
            int pos = Random.Range(0, (statesToRandomize.Length));
            if (!(statesToRandomize[pos] == State.Plataform && platforming)
                && !(statesToRandomize[pos] == State.LaserBall && laseringBall))
            {
                return statesToRandomize[pos];
            }
        }
        //no hauria
        return State.Waiting;
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {

        if (!switchingFase && !hasInmunity)
        {
            currentHealth -= attackDetails[0];
            healthBar.SetHealth(currentHealth);

            //Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            //particleDamage.Play();
            /*
            if (attackDetails[1] > alive.transform.position.x)
            {
                damageDirection = -1;
            }
            else
            {
                damageDirection = 1;
            }*/

            //Hit particle

            if (currentHealth <= 0.0f)
            {
                SwitchState(State.Dead);
                GameManager.Instance.addPoints(pointsToGive);
            }
        }
    }

    public override void mostraMissatge()
    {
        Debug.Log("IM A BOSS");
    }
}
