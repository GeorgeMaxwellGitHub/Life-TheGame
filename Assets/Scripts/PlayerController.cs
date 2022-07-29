using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D _playerRigitbody2D;
    Animator _playerAnimator;

    public bool playerCanMove = true;
    public bool playerNeedsToStop = false;

    private Vector3 _bottomLeftLevelLimit;
    private Vector3 _topRightLevelLimit;

    [SerializeField] float playerMoveSpeed = 1.5f;

    public static PlayerController instance;

    void Start()
    {
        _playerRigitbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        instance = this;
    }

    void FixedUpdate()
    {
        if (playerCanMove && !playerNeedsToStop)
        {
            _playerRigitbody2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * playerMoveSpeed;

            GetComponent<SpriteRenderer>().flipX = Input.GetAxisRaw("Horizontal") < 0 ? true : false;

            _playerAnimator.SetFloat("moveX", _playerRigitbody2D.velocity.x);
            _playerAnimator.SetFloat("moveY", _playerRigitbody2D.velocity.y);

            //if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            //{
            //    _playerAnimator.SetBool("isMoving", false);
            //}

        }
        else
        {
            _playerRigitbody2D.velocity = Vector2.zero;
        }

        if (Input.GetAxisRaw("Horizontal") == 1f ||
            Input.GetAxisRaw("Horizontal") == -1f ||
            Input.GetAxisRaw("Vertical") == 1f ||
            Input.GetAxisRaw("Vertical") == -1f)
        {
            if (!playerNeedsToStop)
            {
                //_playerAnimator.SetBool("isMoving", true);
                //if (playerCanMove)
                //{
                //    //определяем в какую сторону игрок будет смотреть когда остановится
                //    _playerAnimator.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
                //    _playerAnimator.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
                //}
            }
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _bottomLeftLevelLimit.x, _topRightLevelLimit.x),
                                         Mathf.Clamp(transform.position.y, _bottomLeftLevelLimit.y, _topRightLevelLimit.y),
                                         transform.position.z);
    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight)
    {
        _bottomLeftLevelLimit = botLeft + new Vector3(1f, 1f, 0);
        _topRightLevelLimit = topRight + new Vector3(-1f, -1f, 0);
    }
}
