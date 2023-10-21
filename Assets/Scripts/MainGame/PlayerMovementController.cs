using Fusion;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementController : NetworkBehaviour, IBeforeUpdate
{
    public bool AcceptAnyInput => _isPlayerAlive && !GameManager.MatchIsOver;
    public enum PlayerButtons
    {
        None, Jump, Shoot
    }

    [Header("Components")]
    [SerializeField] private PlayerVisualRenderer _visualRenderer;
    //[SerializeField] private LocalCameraHandler localCameraHandler;
    private PlayerWeaponController _weaponController;
    private PlayerHealthManager _healthManager;
    private Rigidbody rb;

    #region Params
    [Header("Parameters")]
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float jumpForce = 50;
    [field: SerializeField] public float rotationSpeed;
    public float UpDownRotationSpeed = 50.0f;
    private float horizontal;
    private float vertical;
    public float interval = 1.0f;
    private float accumulatedTime = 0.0f;
    #endregion

    private Vector2 _viewInputHandler = Vector2.zero;
    private Vector3 movementDirection = Vector3.zero;
    private bool isWalking = false;

    [Header("Ext-GO's")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundDetectionTransform;
    [SerializeField] private TextMeshProUGUI playerNameText;
    //[SerializeField] private ParticleSystem _deadChunksParticle;

    [Networked] private float hor { get; set; }
    [Networked] private float vert { get; set; }
    [Networked] private float scroll { get; set; }
    [Networked] public NetworkBool _isPlayerAlive {  get; set; }
    [Networked] public NetworkBool isGrounded { get; private set; }
    [Networked] private NetworkButtons buttonPrev { get; set; }
    [Networked] private Vector3 serverNextSpawnPoint { get; set; }

    [Networked] public TickTimer respawnTimer { get; private set; }
    [Networked] public TickTimer respawnToNewPointTimer { get; private set; }
    [Networked] public TickTimer walkSoundPlay { get; private set; }
    [Networked(OnChanged = nameof(OnNickNameChanged))] private NetworkString<_8> playerName { get; set; }



    private static void OnNickNameChanged(Changed<PlayerMovementController> changed)
    {
        changed.Behaviour.SetPlayerNickName(changed.Behaviour.playerName);
    }

    private void SetPlayerNickName(NetworkString<_8> nickName)
    {
        playerNameText.text = nickName + " " + Object.InputAuthority.PlayerId;
    }

    [Rpc(sources:RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcSetNickname(NetworkString<_8> nickName)
    {
        playerName = nickName;
    }

    private void SetLocalObjects()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            RpcSetNickname(PlayerPrefs.GetString("Nickname"));
            //SoundManager.Instance.PlayMusic(Sounds.GameMusic);
        }
        else
        {
            //localCameraHandler._localCamera.enabled = false;
            //GetComponent<NetworkRigidbody>().InterpolationDataSource = InterpolationDataSources.Snapshots;
        }
    }

    // Called on all active players
    public override void Spawned()  
    {
        //rb = GetComponent<Rigidbody>();
        _weaponController = GetComponent<PlayerWeaponController>();
        _healthManager = GetComponent<PlayerHealthManager>();
        // Cursor lockstate
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetLocalObjects();
        _isPlayerAlive = true;
        
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GlobalManagers.Instance.objectPoolingManager.RemoveNetworkObjectFromDic(Object);
        Destroy(gameObject);
    }

    public void KillPlayer()
    {
        if(Runner.IsServer)
        {
            serverNextSpawnPoint = GlobalManagers.Instance.playerController.GetRandomSpawnPoint();
            respawnToNewPointTimer = TickTimer.CreateFromSeconds(Runner, 4);
        }
        _isPlayerAlive = false;
        //rb.useGravity = false;

        //_deadChunksParticle.Play();
        _visualRenderer.TriggerDieAnimation();

        respawnTimer = TickTimer.CreateFromSeconds(Runner, 5f);
    }

    public override void FixedUpdateNetwork()
    {
        CheckRespawnTimer();
        if (Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            if (AcceptAnyInput)
            {
                // Save the current X rotation
                Vector3 playerScale = transform.localScale;

                Vector3 curPos = transform.position;
                Vector3 targetPos = curPos + new Vector3(input._horizontalInput, input._verticalInput, 0) * moveSpeed * Runner.DeltaTime;
                // Update the character's position and direction
                transform.position = targetPos;

                if (horizontal < 0)
                    playerScale.x = -1 * Mathf.Abs(horizontal);
                else if (horizontal > 0)
                    playerScale.x = Mathf.Abs(horizontal);

                transform.localScale = playerScale;

                // Save normalized input values for animation
                hor = input._horizontalInput;
                vert = input._verticalInput;

                // Check if the accumulated time has reached the interval
                if ((hor!=0 || vert!=0) && !isWalking)
                {
                    isWalking = true;
                    StartCoroutine(PlayWalkSound());
                }

                buttonPrev = input.networkButtons;
            }
        }
    }

    private IEnumerator PlayWalkSound()
    {
        yield return new WaitForSeconds(.4f);
        SoundManager.Instance.PlayEffect(Sounds.PlayerMovement);
        isWalking = false;
    }

    private void CheckRespawnTimer()
    {
        if (_isPlayerAlive)
            return;

        if(respawnToNewPointTimer.Expired(Runner))
        {
            GetComponent<NetworkRigidbody>().TeleportToPosition(serverNextSpawnPoint);
            respawnToNewPointTimer = TickTimer.None;
        }

        if (respawnTimer.Expired(Runner))
        {
            respawnTimer = TickTimer.None;
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        _isPlayerAlive = true;
        //rb.useGravity = true;
        _visualRenderer.TriggerRespawnAnimation();
        _healthManager.ResetHealth();
    }

    // Called before FixedUpdateNetwork
    public void BeforeUpdate()
    {
        if(Utils.IsLocalPlayer(Object) && AcceptAnyInput)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            //_viewInputHandler.x = Input.GetAxisRaw("Mouse X");
            //_viewInputHandler.y = Input.GetAxisRaw("Mouse Y") * -1;
            //scroll = Input.GetAxis("Mouse ScrollWheel");

            //localCameraHandler.SetViewInputVector(_viewInputHandler);
        }
    }

    //public override void Render()
    //{
    //    _visualRenderer.RenderVisuals(new Vector3(hor, 0, vert), _weaponController.holdingShootKey);
    //}


    public PlayerData PlayerInputDataStruct()
    {
        PlayerData player = new PlayerData();
        player._horizontalInput = horizontal;

        player._verticalInput = vertical;

        player.networkButtons.Set(PlayerButtons.Shoot, Input.GetButton("Fire1"));

        return player;
    }

}
