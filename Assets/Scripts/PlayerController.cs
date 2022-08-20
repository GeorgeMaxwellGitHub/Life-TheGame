using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    //Cached component references
    Rigidbody2D _playerRigidbody2D;
    Animator _playerAnimator;

    [SerializeField] GameObject GameObjWitchEKeyImage;
    [SerializeField] GameObject noEnoughMoneyUIGameObj;
    [SerializeField] GameObject loseBubbleUiGameObj;
    [SerializeField] GameObject winBubbleUiGameObj;
    [SerializeField] GameObject loveHeartBubbleUiGameObj;
    [SerializeField] GameObject brokenHeartBubbleUiGameObj;

    [SerializeField] Animator playerShadowAnimator;

    Interactable _currentInteractableObject;
    Coin _currentCoin;
    Girl _currentGirl;
    Rock _currentRock;

    //Config
    [SerializeField] float playerMoveSpeed = 1.5f;

    [SerializeField] float minutesOfStayToEndGame;
    [SerializeField] float minutesOfWalkToEndGame;

    Vector3 _bottomLeftLevelLimit;
    Vector3 _topRightLevelLimit;

    float _timeToReduceWhenStay;
    float _timeToReduceWhenWalk;

    [SerializeField] string messageShowsBeforePlayingSlotMachine;
    [SerializeField] string messageShowsBeforeTryRelationship;

    //States
    bool _playerCanMove = true;
    bool _playerNeedsToStop = false;

    bool _canPressEKey;

    bool _canInteractWithInteractableObjects = false;
    
    bool _isConfimationWindowOpen;
    string _currentMessageToConfirmationWindowFromInteractableObject;

    bool _canPickupCurrentCoin = false;
    
    bool _isBoundsDisable = false;

    bool _canTryRelationshipWithCurrentGirl;
    bool _canMakeLoveWithCurrentGirl;
    
    bool _canPetCat;

    bool _isPlaySlotMachineConfirmationWindowOpen;
    bool _canInteractWitchSlotMachine;
    bool _isSlotMachineBubbleActive;

    bool _canFlipCurrentRock;

    int _currentFootstepsSoundIndex;

    void Start()
    {
        instance = this;

        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        _timeToReduceWhenStay = CalculateIterationValue(0, 1, minutesOfStayToEndGame);
        _timeToReduceWhenWalk = CalculateIterationValue(0, 1, minutesOfWalkToEndGame);
    }

    private void Update()
    {
        if (GameManager.instance.GetGameEndsStatus() || !GameManager.instance.GameStartedStatus())
        {
            return;
        }

        if ((_canInteractWitchSlotMachine && !_isSlotMachineBubbleActive)||
            _canTryRelationshipWithCurrentGirl ||
            _canMakeLoveWithCurrentGirl ||
            _canPetCat ||
            _canFlipCurrentRock ||
            _canPickupCurrentCoin ||
            _canInteractWithInteractableObjects && !_isConfimationWindowOpen)
        {
            GameObjWitchEKeyImage.SetActive(true);
            _canPressEKey = true;
        } else
        {
            GameObjWitchEKeyImage.SetActive(false);
            _canPressEKey = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && _canPressEKey)
        {
            GameObjWitchEKeyImage.SetActive(false);

            //This will stop the player
            _playerAnimator.SetFloat("moveX", 0);
            _playerAnimator.SetFloat("moveY", 0);
            _playerRigidbody2D.velocity = Vector2.zero;

            if (_canFlipCurrentRock)
            {
                _currentRock.Flip();
                return;
            }

            if (_canInteractWitchSlotMachine)
            {
                winBubbleUiGameObj.SetActive(false);
                loseBubbleUiGameObj.SetActive(false);

                AskScreen.instance.Activate(messageShowsBeforePlayingSlotMachine);

                //Two variables looks like identical, but _isPlaySlotMachineConfirmationWindowOpen sets  which type of confirmation windows open
                //_isConfimationWindowOpen indicates that confirmation window currently open in general
                _isPlaySlotMachineConfirmationWindowOpen = true;
                _isConfimationWindowOpen = true;

                return;
            }

            if (_canPickupCurrentCoin)
            {
                _currentCoin.Pickup();
                _currentCoin = null;
                _canPickupCurrentCoin = false;
                return;
            }

            if (_canMakeLoveWithCurrentGirl)
            {
                _canMakeLoveWithCurrentGirl = false;
                _currentGirl.MakeLove();
                return;
            }

            if (_canPetCat)
            {
                _canPetCat = false;
                PetCat.instance.ShowLove();
                return;
            }

            if (_canTryRelationshipWithCurrentGirl)
            {
                AskScreen.instance.Activate(messageShowsBeforeTryRelationship);
                _isConfimationWindowOpen = true;
                return;
            }

            AskScreen.instance.Activate(_currentMessageToConfirmationWindowFromInteractableObject);
            _isConfimationWindowOpen = true;
        }

        //Triggered when the player refused to try relationship with girl
        if (Input.GetKeyDown(KeyCode.N) && _canTryRelationshipWithCurrentGirl)
        {
            GirlMechanicHandler.instance.TryRelationship(_currentGirl, true);
            SetTryRelationshipActive(false, null);
            AskScreen.instance.Deactivate();
        }

        if (Input.GetKeyDown(KeyCode.N) && _isConfimationWindowOpen)
        {
            _isConfimationWindowOpen = false;
            AskScreen.instance.Deactivate();
        }

        if (Input.GetKeyDown(KeyCode.Y) && _isConfimationWindowOpen)
        {
            _isConfimationWindowOpen = false;
            AskScreen.instance.Deactivate();

            if (_isPlaySlotMachineConfirmationWindowOpen)
            {
                _isPlaySlotMachineConfirmationWindowOpen = false;

                if (GameManager.instance.GetCurrentCoins() <= 0)
                {
                    StartCoroutine(MoneyCor(noEnoughMoneyUIGameObj));
                    return;
                }

                MakeOneGameOnSlotMachine();
            }

            if (_canInteractWithInteractableObjects)
            {
                if (_currentInteractableObject.GetRequiedCoint() > GameManager.instance.GetCurrentCoins())
                {
                    StartCoroutine(MoneyCor(noEnoughMoneyUIGameObj));
                    return;
                }

                _currentInteractableObject.Activate();
                _currentInteractableObject = null;
                _canInteractWithInteractableObjects = false;
            }

            
            if (_canTryRelationshipWithCurrentGirl)
            {
                GirlMechanicHandler.instance.TryRelationship(_currentGirl);
                SetTryRelationshipActive(false, null);
            }
        }

        if (_isBoundsDisable)
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

        if (_isConfimationWindowOpen)
        {
            return;
        }

        MovingController();
    }
    private void ReduceTimeWhenMoving()
    {
        if (GameManager.instance.GetFadingState() != true)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                GameManager.instance.ReduceTime(_timeToReduceWhenWalk);
                playerShadowAnimator.SetBool("Run", true);
            }
            else
            {
                GameManager.instance.ReduceTime(_timeToReduceWhenStay);
                playerShadowAnimator.SetBool("Run", false);
            }
        }
    }

    private void MovingController()
    {
        bool isShadowAnimationActive = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 ? true : false;
        playerShadowAnimator.SetBool("Run", isShadowAnimationActive);

        if (_playerCanMove && !_playerNeedsToStop)
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

        if (!_isBoundsDisable)
        {
            //Clamp player in tilemap bounds. Bounds were set-up via CameraController.cs
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, _bottomLeftLevelLimit.x, _topRightLevelLimit.x),
                                             Mathf.Clamp(transform.position.y, _bottomLeftLevelLimit.y, _topRightLevelLimit.y),
                                             transform.position.z);
        }
    }

    private IEnumerator MoneyCor(GameObject bubble)
    {
        bubble.SetActive(true);
        _isSlotMachineBubbleActive = true;
        _playerNeedsToStop = true;

        yield return new WaitForSeconds(1.5f);

        _isSlotMachineBubbleActive = false;
        _playerNeedsToStop = false;
        bubble.SetActive(false);
    }

    public void SetFlipRockOptionActive(bool value, Rock rock)
    {
        if (value == true && rock == null)
        {
            throw new System.Exception("Parameter rock can't be equal null, if option condiiton is true!");
        }

        if (value == false && rock != null)
        {
            throw new System.Exception("Rock parameter must be null, if flip rock option condition equals false!");
        }

        _canFlipCurrentRock = value;
        _currentRock = rock;
    }

    public void SetPetCatOptionActive(bool value)
    {
        _canPetCat = value;
    }

    public void SetCanMakeLoveWithGirlActive(bool value, Girl girl)
    {
        if (value == true && girl == null)
        {
            throw new System.Exception("Parameter girl can't be equal null, if option condiiton is true!");
        }

        if (value == false && girl != null)
        {
            throw new System.Exception("Girl parameter must be null, if flip rock option condition equals false!");
        }

        _canMakeLoveWithCurrentGirl = value;
        _currentGirl = girl;
    }

    public void SetTryRelationshipActive(bool value, Girl girl)
    {
        if (value == true && girl == null)
        {
            throw new System.Exception("Parameter girl can't be equal null, if option condiiton is true!");
        }

        if (value == false && girl != null)
        {
            throw new System.Exception("Girl parameter must be null, if flip rock option condition equals false!");
        }

        _canTryRelationshipWithCurrentGirl = value;
        _currentGirl = girl;
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
            StartCoroutine(MoneyCor(winBubbleUiGameObj));
            AudioManager.instance.PlayObjectsSFX(4, true);
        } else
        {
            StartCoroutine(MoneyCor(loseBubbleUiGameObj));
            AudioManager.instance.PlayObjectsSFX(5, true);
        }
    }

    public void SetInteractWithSlotMachineOptionActive(bool value)
    {
        _canInteractWitchSlotMachine = value;
    }

    public void SetCanPickupCoinActive(bool value, Coin coin)
    {
        if (value == true && coin == null)
        {
            throw new System.Exception("Parameter coin can't be equal null, if option condiiton is true!");
        }

        if (value == false && coin != null)
        {
            throw new System.Exception("Coin parameter must be null, if flip rock option condition equals false!");
        }

        _canPickupCurrentCoin = value;
        _currentCoin = coin;
    }

    public void SetInteractableObjectActive(bool value, Interactable interactableObject, string activeMessage)
    {
        if (value == true && interactableObject == null)
        {
            Debug.LogError("Parameter interactableObject can't be equal null, if option condiiton is true!");
        }

        if (value == false && interactableObject != null)
        {
            Debug.LogWarning("InteractableObject parameter must be null, if flip rock option condition equals false! This code can use as so, but please make sure that you send correct parameters");
        }

        if (value)
        {
            _canInteractWithInteractableObjects = true;
            _currentInteractableObject = interactableObject;
            _currentMessageToConfirmationWindowFromInteractableObject = activeMessage;
        } else
        {
            _canInteractWithInteractableObjects = false;
            _currentInteractableObject = null;
            _currentMessageToConfirmationWindowFromInteractableObject = "";
        }
    }

    public void CompletleStopPlayer()
    {
        _playerNeedsToStop = true;
        _playerAnimator.SetFloat("moveX", 0);
        _playerAnimator.SetFloat("moveY", 0);
        _playerRigidbody2D.velocity = Vector2.zero;
    }

    //Get info about bottom left and top right corners of the camera and clamp camera between them
    public void SetBoundsForCamera(Vector3 botLeft, Vector3 topRight)
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
        AudioManager.instance.PlayFootsteps(_currentFootstepsSoundIndex);
    }

    public int GetCurrentPlayerFootstepsIndex()
    {
        return _currentFootstepsSoundIndex;
    }

    public void ShowLoveHeart()
    {
        StartCoroutine(ShowObjectAndWaitCor(loveHeartBubbleUiGameObj, 2.5f));
    }

    public void ShowBrokenHeart()
    {
        StartCoroutine(ShowObjectAndWaitCor(brokenHeartBubbleUiGameObj, 2.5f));
    }

    IEnumerator ShowObjectAndWaitCor(GameObject gameObj, float time)
    {
        gameObj.SetActive(true);

        yield return new WaitForSeconds(time);

        gameObj.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BoundsDisabler")
        {
            _isBoundsDisable = true;
        }

        if (collision.tag == "BoundsEnabler")
        {
            _isBoundsDisable = false;
        }

        if (collision.tag == "BridgePiece")
        {
            BridgeBody.instance.CheckPlayerStep(collision.gameObject.GetComponent<BridgePiece>().index);
            _currentFootstepsSoundIndex = 2;
        }

        FootstepsSoundsIndexChanger(collision);
    }

    private void FootstepsSoundsIndexChanger(Collider2D collision)
    {
        if (collision.tag == "BG_Wood")
        {
            _currentFootstepsSoundIndex = 2;
        }

        if (collision.tag == "BG_Sand")
        {
            _currentFootstepsSoundIndex = 1;
        }

        if (collision.tag == "BG_Grass")
        {
            _currentFootstepsSoundIndex = 0;
        }

        if (collision.tag == "BG_Metal")
        {
            _currentFootstepsSoundIndex = 3;
        }
    }
}
