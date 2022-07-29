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

    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        instance = this;
    }

    void FixedUpdate()
    {
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

        //Clamp player in tilemap bounds. Bounds were set-up via CameraController.cs
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
