using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDemon : FatherEnemy
{
    private enum State
    {
        Waiting,
        TPs,
        BossLancer,
        BossShield,
        BossVoice,
        SideLava,
        SwitchFase,
        Dead
    }

    private Transform player;

    public int pointsToGive = 500;

    public HealthBarBoss healthBar;

    private State currentState;
    private State[] statesToRandomize;

    [SerializeField]
    private BossRangeOfActivation rangeOfActivation;

    [SerializeField]
    private GameObject[] TPs;

    [SerializeField]
    private GameObject downLancer,
        meteorsParticles,
        sideLava,
        laserBall,
        laserBall2,
        laserCenter,
        spotA,
        spotA2,
        spotB,
        soul, 
        sprite,
        spawnPoint,
        spiritLancer,
        spiritShield,
        spiritVoice;

    public List<GameObject> lasers = new List<GameObject>();
    public List<GameObject> preLasers = new List<GameObject>();

    public List<GameObject> lasers2 = new List<GameObject>();
    public List<GameObject> preLasers2 = new List<GameObject>();

    private GameObject meteorsParticlesGO;

    [SerializeField]
    private float maxHealth = 500f, 
        waitLancersParticlesDuration = 3f,
        downLancersAttackDuration = 1f,
        waitingDuration = 2f,
        meteorsAnimationDuration = 2f,
        meteorsDuration = 8f,
        laserBallAnimationDuration = 1,
        laserBallPreDuration = 4,
        laserBallNoActivationDuration = 1.5f,
        laserBallDuration = 20,
        laserBallSpeed = 25,
        switchFaseDuration = 2f,
        soulForce = 35,
        magnetRange = 25,
        magnetForce = 10,
        soulingDuration = 3,
        sideLavaAnimationDuration = 2,
        sideLavaDuration = 0.5f;

    private float currentHealth, 
        quantity,
        quantitySideLava,
        lancersParticlesStartTime,
        downLancersAttackStartTime,
        waitingStartTime,
        meteorsAnimationStartTime,
        meteorsStartTime,
        laserBallStartTime,
        switchFaseStartTime,
        soulingStartTime,
        soulLerpValue,
        soulLerpValue2,
        sideLavaStartTime,
        sideLavaAmStartTime;

    private bool switchingFase = false,
        inTP = false,
        areTPsMissing = true,
        isAttackDone,
        particlesEnded,
        particlesCreated,
        isActivated = false,
        inDownLancers = false,
        firstLoop = false,
        meteoring = false,
        wantsToMeteor = false,
        laseringBall = false,
        souling = false,
        soulingDone = false,
        inSideLava = false,
        isSideLavaDone,
        animationEnded,
        animationStarted;

    [SerializeField]
    private BoxCollider2D areaLancers,
        areaMeteors,
        areaSideLava;

    private int actualFase = 0, 
        posTP,
        previousValue,
        direction = 1,
        actualSoul = 0;

    private int[] notSpawn, notSpawnSL;

    [SerializeField]
    private int quantityMeteors = 40,
        quantityOfSouls = 20;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<playerController>().gameObject.transform;
        SwitchState(State.Waiting);


        statesToRandomize = new State[5];
        statesToRandomize[0] = State.TPs;
        statesToRandomize[1] = State.BossLancer;
        statesToRandomize[2] = State.BossShield;
        statesToRandomize[3] = State.BossVoice;
        statesToRandomize[4] = State.Waiting;

        /*statesToRandomize = new State[2];
        statesToRandomize[0] = State.Waiting;
        statesToRandomize[1] = State.SideLava;*/

        currentHealth = maxHealth;

        meteorsParticlesGO = Instantiate(meteorsParticles);
        meteorsParticlesGO.transform.position = new Vector3(areaMeteors.transform.position.x + areaMeteors.offset.x, areaMeteors.transform.position.y - 5, areaMeteors.transform.position.z);
        meteorsParticlesGO.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (inTP)
        {

        }
        else
        {
            
        }

        #region souls
        if (souling) {
            soulLerpValue =  Lerp(0, quantityOfSouls, soulingStartTime, soulingDuration);
            if (soulLerpValue >= actualSoul)
            {
                GameObject g = Instantiate(soul, player.transform.position, Quaternion.identity);
                g.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * soulForce, ForceMode2D.Impulse);
                actualSoul++;
            }
            if (soulLerpValue >= quantityOfSouls) {
                souling = false;
            }
        }

        Collider2D[] souls = Physics2D.OverlapCircleAll(sprite.transform.position, magnetRange);

        if (souls.Length > 0)
        {
            foreach (Collider2D soul in souls)
            {
                if (soul.tag == "SoulV2")
                {
                    soul.transform.Translate((sprite.transform.position - soul.transform.position).normalized * magnetForce * Time.deltaTime);
                }
            }
        }
        
        Collider2D[] souls2 = Physics2D.OverlapCircleAll(spawnPoint.transform.position, magnetRange);

        if (souls2.Length > 0)
        {
            foreach (Collider2D soul1 in souls2)
            {
                if (soul1.tag == "SoulV3")
                {
                    soul1.transform.Translate((spawnPoint.transform.position - soul1.transform.position).normalized * magnetForce * Time.deltaTime);
                }
            }
        }
        #endregion

        #region lancers
        if (inDownLancers)
        {
            if (Time.time >= lancersParticlesStartTime + waitLancersParticlesDuration)
            {
                particlesEnded = true;
                BossDemonPool.BossDemonPoolInstance.DisableParticles();
            }
            if (!isAttackDone)
            {
                Vector2 positionStart = areaLancers.transform.position;
                positionStart.x = positionStart.x + (downLancer.transform.localScale.x / 2);
                //Create Lancers in correct positions
                for (int i = 0; i < quantity; i++)
                {
                    bool found = false;
                    for (int j = 0; j < notSpawn.Length; j++)
                    {
                        if (notSpawn[j] == i)
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        Vector3 lancerPosition = new Vector3(positionStart.x + i * (downLancer.transform.localScale.x), positionStart.y, 0);

                        if (particlesEnded)
                        {
                            GameObject lancer = BossDemonPool.BossDemonPoolInstance.GetLancer();
                            lancer.transform.position = lancerPosition;
                            lancer.SetActive(true);
                        }
                        if (!particlesCreated)
                        {
                            GameObject particle = BossDemonPool.BossDemonPoolInstance.GetParticle();
                            particle.transform.position = lancerPosition;
                            particle.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                            particle.SetActive(true);
                            //Instantiate(lancerParticles, lancerPosition, Quaternion.Euler(-90f,0f,0f));
                        }

                    }
                }
                if (particlesEnded)
                {
                    isAttackDone = true;
                    downLancersAttackStartTime = Time.time;
                }
            }
            if (!particlesCreated)
            {
                particlesCreated = true;
            }


            if (isAttackDone && Time.time >= downLancersAttackStartTime + downLancersAttackDuration)
            {
                inDownLancers = false;
                spiritLancer.SetActive(false);
                spiritLancer.transform.parent.transform.GetComponent<Animator>().Play("NotShowing");

                BossDemonPool.BossDemonPoolInstance.DisableAll();

                //SwitchState(State.Waiting);
            }
        }
        #endregion

        #region Meteors
        if (meteoring)
        {
            //Maybe canviarlo una mica
            Camera.main.GetComponent<Animator>().SetTrigger("shake");
            float value = Lerp(0, quantityMeteors, meteorsStartTime, meteorsDuration);
            if (previousValue < (int)value)
            {
                //generate Meteor
                GameObject meteor = BossDemonPool.BossDemonPoolInstance.GetMeteor();

                Vector2 position = new Vector2(Random.Range(areaMeteors.transform.position.x + meteor.transform.localScale.x, areaMeteors.transform.position.x + areaMeteors.size.x - meteor.transform.localScale.x), areaMeteors.transform.position.y);

                meteor.transform.position = position;
                //float scale = Random.Range(0.2f, 1.4f);
                float scale = GetRandomValueForScale();
                meteor.transform.localScale = new Vector3(scale, scale, scale);
                meteor.SetActive(true);
                previousValue = (int)value;
            }
            if (value == quantityMeteors)
            {
                meteoring = false;
                spiritShield.transform.parent.transform.GetComponent<Animator>().Play("NotShowing");
                spiritShield.SetActive(false);

            }
        }
        #endregion

        #region LaserBall
        if (laseringBall)
        {
            if (Time.time >= laserBallStartTime + laserBallAnimationDuration + laserBallPreDuration + laserBallDuration)
            {
                laseringBall = false;
                spiritVoice.SetActive(false);
                spiritVoice.transform.parent.transform.GetComponent<Animator>().Play("NotShowing");

                laserBall.SetActive(false);
                preLasers[0].SetActive(false);
                preLasers[1].SetActive(false);
                preLasers[2].SetActive(false);
                preLasers[3].SetActive(false);
                lasers[0].SetActive(false);
                lasers[1].SetActive(false);
                lasers[2].SetActive(false);
                lasers[3].SetActive(false);

                laserBall2.SetActive(false);
                preLasers2[0].SetActive(false);
                preLasers2[1].SetActive(false);
                preLasers2[2].SetActive(false);
                preLasers2[3].SetActive(false);
                lasers2[0].SetActive(false);
                lasers2[1].SetActive(false);
                lasers2[2].SetActive(false);
                lasers2[3].SetActive(false);
            }
            else
            {
                laserBall.transform.Rotate(0, 0, laserBallSpeed * Time.deltaTime * direction);
                laserBall.SetActive(true);

                laserBall2.transform.Rotate(0, 0, laserBallSpeed * Time.deltaTime * (-1)*direction);
                laserBall2.SetActive(true);
                if (Time.time >= laserBallStartTime + laserBallAnimationDuration + laserBallPreDuration)
                {
                    lasers[0].SetActive(true);
                    lasers[1].SetActive(true);
                    lasers[2].SetActive(true);
                    lasers[3].SetActive(true);

                    lasers2[0].SetActive(true);
                    lasers2[1].SetActive(true);
                    lasers2[2].SetActive(true);
                    lasers2[3].SetActive(true);
                    /*
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
                            //lasers[3].SetActive(true);
                        }
                    }*/
                }
                else
                {
                    if (Time.time >= laserBallStartTime + laserBallAnimationDuration + laserBallNoActivationDuration)
                    {
                        lasers[0].SetActive(true);
                        lasers[1].SetActive(true);
                        lasers[2].SetActive(true);
                        lasers[3].SetActive(true);

                        lasers2[0].SetActive(true);
                        lasers2[1].SetActive(true);
                        lasers2[2].SetActive(true);
                        lasers2[3].SetActive(true);
                        /*if (actualFase == 2)
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
                                //lasers[3].SetActive(true);
                            }
                        }*/
                    }
                    float pos;
                    float pos2;
                    /*
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
                            //preLasers[3].SetActive(true);
                        }
                    }*/
                    pos = Lerp(laserCenter.transform.position.x, spotA.transform.position.x, (laserBallStartTime + laserBallAnimationDuration), laserBallPreDuration);
                    pos2 = Lerp(laserCenter.transform.position.y, spotA.transform.position.y, (laserBallStartTime + laserBallAnimationDuration), laserBallPreDuration);
                    preLasers[0].SetActive(true);
                    preLasers[1].SetActive(true);
                    preLasers[2].SetActive(true);
                    preLasers[3].SetActive(true);
                    laserBall.transform.position = new Vector3(pos, pos2, laserBall.transform.position.z);

                    pos = Lerp(laserCenter.transform.position.x, spotA2.transform.position.x, (laserBallStartTime + laserBallAnimationDuration), laserBallPreDuration);
                    pos2 = Lerp(laserCenter.transform.position.y, spotA2.transform.position.y, (laserBallStartTime + laserBallAnimationDuration), laserBallPreDuration);
                    preLasers2[0].SetActive(true);
                    preLasers2[1].SetActive(true);
                    preLasers2[2].SetActive(true);
                    preLasers2[3].SetActive(true);
                    laserBall2.transform.position = new Vector3(pos, pos2, laserBall2.transform.position.z);
                }
            }
        }
        #endregion

        #region Tps comentat i borrar
        /*if (areTPsMissing)
        {
            if (inTP)
            {
                if (TPs[posTP].GetComponent<TPFunctionality>().hasFinished())
                {
                    bool flag = false;
                    inTP = false;
                    for (int i = 0; i < TPs.Length; i++)
                    {
                        if (!TPs[i].GetComponent<TPFunctionality>().hasBeenActivated())
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        areTPsMissing = false;
                    }
                }
            }
        }*/
        /*
        if (areTPsMissing)
        {
            if (!inTP) {
                if (Input.GetKeyDown("p"))
                {
                    int iterations = 0;
                    inTP = true;
                    bool ok = false;
                    while (!ok)
                    {
                        iterations++;
                        posTP = Random.Range(0, TPs.Length);

                        if (!TPs[posTP].GetComponent<TPFunctionality>().hasBeenActivated())
                        {
                            ok = true;
                        }
                        if (iterations > TPs.Length)
                        {
                            areTPsMissing = false;
                            break;
                            //aqui evitar que faci mes tps xq no queden i evitar que faci start al de abaix
                        }
                    }
                    if (areTPsMissing)
                    {
                        TPs[posTP].GetComponent<TPFunctionality>().startTP();
                        inTP = true;
                    }

                }
            }
            else
            {
                if (TPs[posTP].GetComponent<TPFunctionality>().hasFinished())
                {
                    inTP = false;
                }
            }
        }
        if (Input.GetKeyDown("l"))
        {

        }*/

        #endregion

        #region sideLava
        if (inSideLava)
        {
            if (Time.time >= sideLavaAmStartTime + sideLavaAnimationDuration)
            {
                animationEnded = true;
                BossDemonPool.BossDemonPoolInstance.DisableParticlesSL();
            }
            if (!isSideLavaDone)
            {
                Vector2 positionStart = areaSideLava.transform.position;
                positionStart.y = positionStart.y + (sideLava.transform.GetComponentInChildren<BoxCollider2D>().transform.localScale.y / 2);
                //Create Lancers in correct positions
                Debug.Log("--------" + quantitySideLava);
                for (int i = 0; i < quantitySideLava; i++)
                {
                    bool found = false;
                    for (int j = 0; j < notSpawnSL.Length; j++)
                    {
                        if (notSpawnSL[j] == i)
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        Vector3 lavaPosition = new Vector3(positionStart.x, positionStart.y + i * (sideLava.transform.GetComponentInChildren<BoxCollider2D>().transform.localScale.y), 0);

                        if (animationEnded)
                        {
                            GameObject sl = BossDemonPool.BossDemonPoolInstance.GetSideLava();
                            sl.transform.position = lavaPosition;
                            sl.SetActive(true);
                        }
                        if (!animationStarted)
                        {
                            GameObject slam = BossDemonPool.BossDemonPoolInstance.GetSideLavaAm();
                            slam.transform.position = lavaPosition;
                            //particle.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                            slam.SetActive(true);

                        }

                    }
                }
                if (animationEnded)
                {
                    isSideLavaDone = true;
                    sideLavaStartTime = Time.time;
                }
            }
            if (!animationStarted)
            {
                animationStarted = true;
            }


            if (isSideLavaDone && Time.time >= sideLavaStartTime + sideLavaDuration)
            {
                inSideLava = false;
                Debug.Log("UIEDDUOHIDHUOHDOUHOUIPA");
                BossDemonPool.BossDemonPoolInstance.DisableAllSL();

            }
        }
        #endregion

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

        switch (currentState)
        {
            case State.Waiting:
                UpdateWaitingState();
                break;
            case State.TPs:
                UpdateTPsState();
                break;
            case State.BossLancer:
                UpdateBossLancerState();
                break;
            case State.BossShield:
                UpdateBossShieldState();
                break;
            case State.BossVoice:
                UpdateBossVoiceState();
                break;
            case State.SideLava:
                UpdateSideLavaState();
                break;
            case State.SwitchFase:
                UpdateSwitchFaseState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    //---------WAITING-------------
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
            }

        }
        else
        {
            if (Time.time >= waitingStartTime + waitingDuration)
            {

                //SwitchState(State.Walking);
                SwitchState(RandomBehaviour());

            }
        }
    }

    private void ExitWaitingState()
    {

    }
    #endregion

    //-------------TPs---------------
    #region TPs
    private void EnterTPsState()
    {
        firstLoop = true;
        inTP = true;
        bool ok = false;
        while (!ok)
        {
            posTP = Random.Range(0, TPs.Length);

            if (!TPs[posTP].GetComponent<TPFunctionality>().hasBeenActivated())
            {
                ok = true;
            }
        }

    }
    private void UpdateTPsState()
    {
        if (!inDownLancers && !meteoring && !laseringBall)
        {
            if (firstLoop)
            {
                firstLoop = false;
                TPs[posTP].GetComponent<TPFunctionality>().startTP();
            }

            if (TPs[posTP].GetComponent<TPFunctionality>().hasFinished())
            {
                bool flag = false;
                inTP = false;
                for (int i = 0; i < TPs.Length; i++)
                {
                    if (!TPs[i].GetComponent<TPFunctionality>().hasBeenActivated())
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    areTPsMissing = false;
                    //treiem de la possibilitat de estat el de TPs
                    statesToRandomize = new State[4];
                    statesToRandomize[0] = State.BossLancer;
                    statesToRandomize[1] = State.BossShield;
                    statesToRandomize[2] = State.BossVoice;
                    statesToRandomize[3] = State.Waiting;
                }
                SwitchState(State.Waiting);
            }
        }
        else
        {
            //waiting until everythingfinished
        }
    }
    private void ExitTPsState()
    {

    }
    #endregion

    //----------BOSS LANCER-------------
    #region BOSSLANCER
    private void EnterBossLancerState()
    {
        StartSouling();
        spiritLancer.SetActive(true);

        spiritLancer.transform.parent.GetComponent<Animator>().Play("SpiritSpawn");

        isAttackDone = false;
        particlesEnded = false;
        particlesCreated = false;
        Vector3 areaSize = areaLancers.size;

        quantity = Mathf.Round(areaSize.x / downLancer.transform.localScale.x);

        float quantityEmpty = Mathf.Round(quantity * 0.2f);


        notSpawn = new int[((int)(quantityEmpty))];

        bool isOK;

        //Lancers where not to spawn
        for (int i = 0; i < quantityEmpty; i++)
        {
            int attempt = 0;
            isOK = false;
            while (!isOK)
            {
                bool found = false;
                attempt = Random.Range(0, (int)quantity);

                for (int j = 0; j < i; j++)
                {
                    if (notSpawn[j] == attempt)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    isOK = true;
                }
            }

            notSpawn[i] = attempt;
        }
    }

    private void UpdateBossLancerState()
    {
        if (souling)
        {

        }
        else
        {
            if (!soulingDone)
            {
                inDownLancers = true;
                lancersParticlesStartTime = Time.time;
                soulingDone = true;
            }
            else
            {
                if (Time.time >= lancersParticlesStartTime + waitLancersParticlesDuration)
                {
                    SwitchState(RandomBehaviour());
                }
            }
            //duringAnimation
        }


    }

    private void ExitBossLancerState()
    {
        //spriteAnimator.Play("LancerIdle");
    }
    #endregion

    //----------BOSS SHIELD-----------
    #region BOSSSHIELD
    private void EnterBossShieldState()
    {
        StartSouling();
        spiritShield.SetActive(true);
        spiritShield.transform.parent.GetComponent<Animator>().Play("SpiritSpawn");
    }
    private void UpdateBossShieldState()
    {
        if (souling)
        {

        }
        else
        {
            if (!soulingDone)
            {
                meteorsParticlesGO.SetActive(false);
                meteorsAnimationStartTime = Time.time;
                previousValue = 0;
                //spriteAnimator.Play("ShieldMeteors");
                soulingDone = true;
                wantsToMeteor = true;
            }
            else
            {
                if (Time.time >= meteorsAnimationStartTime + meteorsAnimationDuration)
                {
                    meteorsStartTime = Time.time;
                    SwitchState(RandomBehaviour());
                    meteorsParticlesGO.SetActive(true);
                    meteoring = true;
                    wantsToMeteor = false;

                }
            }
        }
    }
    private void ExitBossShieldState()
    {
        //spriteAnimator.Play("ShieldIdle");
    }
    #endregion

    //----------BOSS VOICE-------------
    #region BOSSVOICE
    private void EnterBossVoiceState()
    {
        StartSouling();
        spiritVoice.SetActive(true);
        spiritVoice.transform.parent.GetComponent<Animator>().Play("SpiritSpawn");
        //audio7.clip = clip7;
        //audio7.Play();
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
    private void UpdateBossVoiceState()
    {
        if (souling)
        {

        }
        else
        {
            if (!soulingDone)
            {
                laserBallStartTime = Time.time;
                soulingDone = true;
            }
            else
            {
                if (Time.time >= laserBallStartTime + laserBallAnimationDuration)
                {
                    laseringBall = true;
                    laserBall.transform.position = laserCenter.transform.position;

                    laserBall2.transform.position = laserCenter.transform.position;

                    SwitchState(State.Waiting);
                }
            }
        }
    }
    private void ExitBossVoiceState()
    {

    }
    #endregion

    //---------SIDE LAVA--------------
    #region SIDELAVA
    private void EnterSideLavaState()
    {
        Debug.Log("SLENTER");
        inSideLava = true;
        sideLavaAmStartTime = Time.time;

        isSideLavaDone = false;
        animationEnded = false;
        animationStarted = false;
        Vector3 areaSize = areaSideLava.size;

        quantitySideLava = Mathf.Round(areaSize.y / sideLava.transform.GetComponentInChildren<BoxCollider2D>().transform.localScale.y);

        float quantityEmpty = Mathf.Round(quantitySideLava * 0.2f);


        notSpawnSL = new int[((int)(quantityEmpty))];

        bool isOK;

        //Lancers where not to spawn
        for (int i = 0; i < quantityEmpty; i++)
        {
            int attempt = 0;
            isOK = false;
            while (!isOK)
            {
                bool found = false;
                attempt = Random.Range(0, (int)quantitySideLava);

                for (int j = 0; j < i; j++)
                {
                    if (notSpawnSL[j] == attempt)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    isOK = true;
                }
            }

            notSpawnSL[i] = attempt;
        }
    }
    private void UpdateSideLavaState()
    {
        if (Time.time >= sideLavaAmStartTime + sideLavaAnimationDuration)
        {
            SwitchState(RandomBehaviour());
        }
    }
    private void ExitSideLavaState()
    {

    }
    #endregion

    //---------SWITCHFASE--------------
    #region SWITCHFASE
    private void EnterSwitchFaseState()
    {
        /*
        spriteAnimator.Play("ShieldIdle");
        switch (actualFase)
        {
            case 1:
                //spriteAnimator.Play("boss1SwitchFaseAnimation1");

                break;
            case 2:

                meteorsAnimationDuration = meteorsAnimationDurationV2;
                meteorsDuration = meteorsDurationV2;
                quantityMeteors = quantityMeteorsV2;
                speed = speedV2;
                venomAnimationDuration = venomAnimationDurationV2;
                walkingDuration = walkingDurationV2;
                frontAttackSpeed = frontAttackSpeedV2;
                flippingDuration = flippingDurationV2;
                frontAttackDuration = frontAttackDurationV2;

                statesToRandomize = new State[13];

                statesToRandomize[0] = State.Meteors;
                statesToRandomize[1] = State.Venom;
                statesToRandomize[2] = State.FrontAttack;
                statesToRandomize[3] = State.Meteors;
                statesToRandomize[4] = State.Venom;
                statesToRandomize[5] = State.FrontAttack;
                statesToRandomize[6] = State.Meteors;
                statesToRandomize[7] = State.Venom;
                statesToRandomize[8] = State.FrontAttack;
                statesToRandomize[9] = State.Meteors;
                statesToRandomize[10] = State.Venom;
                statesToRandomize[11] = State.FrontAttack;
                statesToRandomize[12] = State.JumpAttack;

                venomAnimation = "ShieldVenomV2";
                //spriteAnimator.Play("boss1SwitchFaseAnimation2");
                break;
            case 3:

                meteorsAnimationDuration = meteorsAnimationDurationV3;
                meteorsDuration = meteorsDurationV3;
                quantityMeteors = quantityMeteorsV3;
                speed = speedV3;
                venomAnimationDuration = venomAnimationDurationV3;
                walkingDuration = walkingDurationV3;
                frontAttackSpeed = frontAttackSpeedV3;
                flippingDuration = flippingDurationV3;
                jumpDuration = jumpDurationV3;
                jumpLevitationDuration = jumpLevitationDurationV3;

                venomAnimation = "ShieldVenomV3";
                jumpAnimation = "ShieldJumpV2";
                //spriteAnimator.Play("boss1SwitchFaseAnimation3");
                break;
        }

        if ((!isRight && sprite.transform.localRotation.y >= 0) || (isRight && sprite.transform.localRotation.y < 0))
        {
            sprite.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        */
        switchingFase = true;
        switchFaseStartTime = Time.time;
    }

    private void UpdateSwitchFaseState()
    {
        Camera.main.GetComponent<Animator>().SetTrigger("shake");
        /*float value = startPos.y;
        if (actualFase == 2)
        {
            value = Lerp(startPos.y, endPos.transform.position.y, switchFaseStartTime, switchFaseDuration);
        }
        if (actualFase == 3)
        {
            value = Lerp(endPos.transform.position.y, startPos.y, switchFaseStartTime, switchFaseDuration);
        }

        sprite.transform.position = new Vector3(sprite.transform.position.x, value, sprite.transform.position.z);
        */
        if (Time.time >= switchFaseStartTime + switchFaseDuration)
        {
            SwitchState(State.Waiting);
        }
    }

    private void ExitSwitchFaseState()
    {
        switchingFase = false;
    }
    #endregion

    //------------DEAD-----------------
    #region DEAD
    private void EnterDeadState()
    {
        healthBar.gameObject.SetActive(false);
        /*
        globalLight.intensity = startIntensity;
        Instantiate(outBurstSoul, new Vector3(sprite.transform.position.x, sprite.transform.position.y - 5, sprite.transform.position.z), outBurstSoul.transform.rotation);
        GameObject p = Instantiate(portal, new Vector3(sprite.transform.position.x + 5, sprite.transform.position.y - 3, sprite.transform.position.z), portal.transform.rotation);
        p.GetComponent<EndLevel>().nextLevelName = "L1W2";*/
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
            case State.TPs:
                ExitTPsState();
                break;
            case State.BossLancer:
                ExitBossLancerState();
                break;
            case State.BossShield:
                ExitBossShieldState();
                break;
            case State.BossVoice:
                ExitBossVoiceState();
                break;
            case State.SideLava:
                ExitSideLavaState();
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
            case State.TPs:
                EnterTPsState();
                break;
            case State.BossLancer:
                EnterBossLancerState();
                break;
            case State.BossShield:
                EnterBossShieldState();
                break;
            case State.BossVoice:
                EnterBossVoiceState();
                break;
            case State.SideLava:
                EnterSideLavaState();
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

    private State RandomBehaviour()
    {
        int iterations = 0;

        bool isOk = false;
        while (!isOk)
        {
            iterations++;
            int pos = Random.Range(0, (statesToRandomize.Length));
            if (!(statesToRandomize[pos] == State.TPs && (!areTPsMissing || souling))
                && !(statesToRandomize[pos] == State.BossLancer && (inDownLancers || meteoring || wantsToMeteor || laseringBall || souling))
                && !(statesToRandomize[pos] == State.BossShield && (inDownLancers || meteoring || wantsToMeteor || laseringBall || souling))
                && !(statesToRandomize[pos] == State.BossVoice && (inDownLancers || meteoring || wantsToMeteor || laseringBall || souling))
                && !(statesToRandomize[pos] == State.SideLava && inSideLava))
            {
                return statesToRandomize[pos];
            }
            if (iterations > statesToRandomize.Length*100)
            {
                return State.Waiting;
            }
        }
        //no hauria
        return State.Waiting;
    }

    private float Lerp(float start, float end, float timeStartedLerping, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        float result = Vector3.Lerp(new Vector3(start, 0, 0), new Vector3(end, 0, 0), percentageComplete).x;

        return result;
    }

    private void StartSouling()
    {
        souling = true;
        soulingStartTime = Time.time;
        actualSoul = 0;
        soulingDone = false;
    }

    private float GetRandomValueForScale()
    {
        float rand = Random.value;
        if (rand >= 0.3f) //70%
            return Random.Range(0.3f, 0.8f);
        if (rand >= 0.1f)
            return Random.Range(0.8f, 1.1f);

        return Random.Range(1.1f, 1.4f);
    }

    public override void applyKnockback(float[] position)
    {
        //nothing
    }

    public override void Damage(float[] attackDetails, bool wantKnockback)
    {
        
        if (!switchingFase)
        {
            currentHealth -= attackDetails[0];
            healthBar.SetHealth(currentHealth);

            //Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            //particleDamage.Play();
            

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
        Debug.Log("THE FINAL BOSSS");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }
}
