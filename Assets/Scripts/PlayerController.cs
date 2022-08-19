using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D _playerRigidbody2D;
    Animator _playerAnimator;

    public bool playerCanMove = true;
    public bool playerNeedsToStop = false;

    private Vector3 _bottomLeftLevelLimit;
    private Vector3 _topRightLevelLimit;

    [SerializeField] float playerMoveSpeed = 1.5f;

    public static PlayerController instance;

    float reduceTimeWhenStay;
    float reduceTimeWhenWalk;

    [SerializeField] float minutesOfStayToEndGame;
    [SerializeField] float minutesOfWalkToEndGame;

    [SerializeField] bool canInteract = false;
    [SerializeField] Interactable activeInteractableObject;

    [SerializeField] bool canPickupCoin = false;
    [SerializeField] Coin activeCoinThatCanPickup;

    public bool isBoundsDisable = false;

    private bool canPlaySlotMachine;

    private bool canTryRelationship;

    private bool canMakeLove;

    private bool canPetCat;

    Girl activeGirl;

    [SerializeField] GameObject eKeyImage;

    Rock activeRock;
    bool canFlipRock;

    int currentFootstepsIndex;

    [SerializeField] GameObject askUIObj;
    bool canChooseWhatToDo;

    string activeMessageToUI;

    [SerializeField] GameObject noEnoughtoneyUI;

    bool canPlaySlotMachineTempAsk;

    [SerializeField] GameObject loseUi;
    [SerializeField] GameObject winUi;

    [SerializeField] Animator shadowAnimator;

    bool canPressE;

    [SerializeField] GameObject loveBubble;
    [SerializeField] GameObject brokenHeart;

    bool activeBubble;

    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        reduceTimeWhenStay = CalculateIterationValue(0, 1, minutesOfStayToEndGame);
        reduceTimeWhenWalk = CalculateIterationValue(0, 1, minutesOfWalkToEndGame);

        instance = this;
    }

    private void Update()
    {
        if (GameManager.instance.GetGameEndsStatus() || !GameManager.instance.GameStartedStatus())
        {
            return;
        }

        if ((canPlaySlotMachine && !activeBubble)||
            canTryRelationship ||
            canMakeLove ||
            canPetCat ||
            canFlipRock ||
            canPickupCoin ||
            canInteract &&
            !canChooseWhatToDo)
        {
            eKeyImage.SetActive(true);
            canPressE = true;
        } else
        {
            eKeyImage.SetActive(false);
            canPressE = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && canPressE)
        {
            eKeyImage.SetActive(false);

            _playerAnimator.SetFloat("moveX", 0);
            _playerAnimator.SetFloat("moveY", 0);
            _playerRigidbody2D.velocity = Vector2.zero;

            if (canFlipRock)
            {
                activeRock.Flip();
                return;
            }

            if (canPlaySlotMachine)
            {
                winUi.SetActive(false);
                loseUi.SetActive(false);

                AskScreen.instance.Activate("I can stay here and try my luck for 1 coin!");

                canPlaySlotMachineTempAsk = true;
                canChooseWhatToDo = true;
                return;
            }

            if (canPickupCoin)
            {
                activeCoinThatCanPickup.Pickup();
                activeCoinThatCanPickup = null;
                canPickupCoin = false;
                return;
            }

            if (canMakeLove)
            {
                canMakeLove = false;
                activeGirl.MakeLove();
                return;
            }

            if (canPetCat)
            {
                canPetCat = false;
                PetCat.instance.ShowLove();
                return;
            }

            if (canTryRelationship)
            {
                AskScreen.instance.Activate("This girl looks lonely. I could try to keep her company for life, but I do not know if it will work. It might take some time. Try to have a relationship?");
                canChooseWhatToDo = true;
                return;
            }

            AskScreen.instance.Activate(activeMessageToUI);
            canChooseWhatToDo = true;
        }

        if (Input.GetKeyDown(KeyCode.N) && canTryRelationship)
        {
            GirlMechanicHandler.instance.TryRelationship(activeGirl, true);
            CanTryRelationshipDisable();
            AskScreen.instance.Deactivate();
        }

        if (Input.GetKeyDown(KeyCode.N) && canChooseWhatToDo)
        {
            canChooseWhatToDo = false;
            AskScreen.instance.Deactivate();
        }

        if (Input.GetKeyDown(KeyCode.Y) && canChooseWhatToDo)
        {
            canChooseWhatToDo = false;
            AskScreen.instance.Deactivate();

            if (canPlaySlotMachineTempAsk)
            {
                canPlaySlotMachineTempAsk = false;

                if (GameManager.instance.GetCurrentCoins() <= 0)
                {
                    StartCoroutine(MoneyCor(noEnoughtoneyUI));
                    return;
                }

                MakeOneGameOnSlotMachine();
            }

            if (canInteract)
            {
                if (activeInteractableObject.GetRequiedCoint() > GameManager.instance.GetCurrentCoins())
                {
                    StartCoroutine(MoneyCor(noEnoughtoneyUI));
                    return;
                }

                activeInteractableObject.Activate();
                activeInteractableObject = null;
                canInteract = false;
            }

            
            if (canTryRelationship)
            {
                GirlMechanicHandler.instance.TryRelationship(activeGirl);
                CanTryRelationshipDisable();
            }
        }

        if (isBoundsDisable)
        {
            if (Input.GetKeyUp(KeyCode.D) && BridgeBody.instance.ReturnInfiniteModState())
            {
                BridgeBody.instance.MoveBridgeBodyToInitial();
                BridgeBody.instance.DisableInfiniteMod();
            }
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.GameStartedStatus())
        {
            return;
        }

        ReduceTimeWhenMoving();

        if (canChooseWhatToDo)
        {
            return;
        }

        MovingController();
    }

    private IEnumerator MoneyCor(GameObject bubble)
    {
        bubble.SetActive(true);
        activeBubble = true;
        playerNeedsToStop = true;

        yield return new WaitForSeconds(1.5f);

        activeBubble = false;
        playerNeedsToStop = false;
        bubble.SetActive(false);
    }

    public void CanFlipRockEnable(Rock rock)
    {
        activeRock = rock;
        canFlipRock = true;
    }

    public void CanFlipRockDisable()
    {
        activeRock = null;
        canFlipRock = false;
    }

    public void CanPetCatEnable()
    {
        canPetCat = true;
    }

    public void CanPetCatDisable()
    {
        canPetCat = false;
    }

    public void CanMakeLoveActive(Girl girl)
    {
        activeGirl = girl;
        canMakeLove = true;
    }

    public void CanMakeLoveDisable()
    {
        activeGirl = null;
        canMakeLove = false;
    }

    public void CanTryRelationshipActive(Girl girl)
    {
        activeGirl = girl;
        canTryRelationship = true;
    }

    public void CanTryRelationshipDisable()
    {
        activeGirl = null;
        canTryRelationship = false;
    }
    
    public void MakeOneGameOnSlotMachine()
    {
        GameManager.instance.ReduceTime(0.02f);
        GameManager.instance.ReduceCoins(1);
        int i = Random.Range(0, 3);
        print(i);
        GameManager.instance.AddCoin(i);

        if (i > 0)
        {
            StartCoroutine(MoneyCor(winUi));
            AudioManager.instance.PlayObjectsSFX(4, true);
        } else
        {
            StartCoroutine(MoneyCor(loseUi));
            AudioManager.instance.PlayObjectsSFX(5, true);
        }
    }

    public void ActivateAbilityToPlaySlotMachine()
    {
        canPlaySlotMachine = true;
    }

    public void DisableAbilityToPlaySlotMachine()
    {
        canPlaySlotMachine = false;
    }

    public void CanPickupCoin(Coin coin)
    {
        activeCoinThatCanPickup = coin;
        canPickupCoin = true;
    }

    public void RemoveAbilityToPickupCoin()
    {
        activeCoinThatCanPickup = null;
        canPickupCoin = false;
    }

    public void SetInteractableObject(Interactable interactableObject, string activeMessage)
    {
        activeMessageToUI = activeMessage;
        activeInteractableObject = interactableObject;
        canInteract = true;
    }

    public void RemoveInteractableObject()
    {
        activeMessageToUI = "";
        activeInteractableObject = null;
        canInteract = false;
    }

    private void MovingController()
    {
        bool isShadowAnimationActive = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 ? true : false;
        shadowAnimator.SetBool("Run", isShadowAnimationActive);

        if (playerCanMove && !playerNeedsToStop)
        {
            _playerRigidbody2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * playerMoveSpeed;

            GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal") < 0 ? true : false;

            _playerAnimator.SetFloat("moveX", _playerRigidbody2D.velocity.x);
            _playerAnimator.SetFloat("moveY", _playerRigidbody2D.velocity.y);
        }
        else
        {
            _playerRigidbody2D.velocity = Vector2.zero;
        }

        if (!isBoundsDisable)
        {
            //Clamp player in tilemap bounds. Bounds were set-up via CameraController.cs
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, _bottomLeftLevelLimit.x, _topRightLevelLimit.x),
                                             Mathf.Clamp(transform.position.y, _bottomLeftLevelLimit.y, _topRightLevelLimit.y),
                                             transform.position.z);
        }
        
    }

    public void StopPlayer()
    {
        playerNeedsToStop = true;
        _playerAnimator.SetFloat("moveX", 0);
        _playerAnimator.SetFloat("moveY", 0);
        _playerRigidbody2D.velocity = Vector2.zero;
    }

    private void ReduceTimeWhenMoving()
    {
        if (GameManager.instance.GetFadingState() != true)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                GameManager.instance.ReduceTime(reduceTimeWhenWalk);
                shadowAnimator.SetBool("Run", true);
            }
            else
            {
                GameManager.instance.ReduceTime(reduceTimeWhenStay);
                shadowAnimator.SetBool("Run", false);
            }
        }
    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight)
    {
        _bottomLeftLevelLimit = botLeft + new Vector3(1f, 1f, 0);
        _topRightLevelLimit = topRight + new Vector3(-1f, -1f, 0);
    }

    float CalculateIterationValue(float minValue, float maxValue, float timeInMinutes)
    {
        //3000 is a value of iteration per minute in FixedUpdate
        return ((maxValue - minValue) / (3000 * timeInMinutes));
    }

    public void PlayFootstepsSound()
    {
        AudioManager.instance.PlayFootsteps(currentFootstepsIndex);
    }

    public int GetCurrentFootstepsIndex()
    {
        return currentFootstepsIndex;
    }

    public void ShowLove()
    {
        StartCoroutine(ShowObjectOnOneSecond(loveBubble));
    }

    public void ShowBrokenHeart()
    {
        StartCoroutine(ShowObjectOnOneSecond(brokenHeart));
    }

    IEnumerator ShowObjectOnOneSecond(GameObject gameObj)
    {
        gameObj.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        gameObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BoundsDisabler")
        {
            print("Bounds is disabled");
            isBoundsDisable = true;
        }

        if (collision.tag == "BoundsEnabler")
        {
            print("Bounds is enabled");
            isBoundsDisable = false;
        }

        if (collision.tag == "BridgePiece")
        {
            BridgeBody.instance.CheckPlayerStep(collision.gameObject.GetComponent<BridgePiece>().index);
            currentFootstepsIndex = 2;
        }

        if (collision.tag == "BG_Wood")
        {
            currentFootstepsIndex = 2;
        }

        if (collision.tag == "BG_Sand")
        {
            currentFootstepsIndex = 1;
        }

        if (collision.tag == "BG_Grass")
        {
            currentFootstepsIndex = 0;
        }

        if (collision.tag == "BG_Metal")
        {
            currentFootstepsIndex = 3;
        }
    }
}
