using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using TMPro;

public class PlayerController : NetworkBehaviour, IBeforeUpdate
{
    public bool AcceptAnyInput => _isPlayerAlive && !GameManager.MatchIsOver;

    public enum PlayerButtons
    {
        None, Shoot
    }

    [SerializeField]
    private float _moveSpeed;
    [SerializeField]
    private float damage;

    public Animator weaponAnim;
    [SerializeField]
    private PlayerHealth _healthManager;
    [SerializeField]
    private PlayerVisualRenderer _visualRenderer;
    [SerializeField] private TextMeshProUGUI playerNameText;

    public string ANIM_NAME;
    [SerializeField]
    private SpriteType attackerMode;

    //bool isMOVABLE = true;
    private float horizontal;
    private float vertical;

    public Camera worldCam;
    [Networked] private float hor { get; set; }
    [Networked] private float vert { get; set; }
    [Networked] private float scroll { get; set; }
    [Networked] public NetworkBool _isPlayerAlive { get; set; }
    [Networked] public NetworkBool isGrounded { get; private set; }
    [Networked] public NetworkBool isMOVABLE { get; private set; }
    [Networked] private NetworkButtons buttonPrev { get; set; }
    [Networked] private Vector3 serverNextSpawnPoint { get; set; }

    [Networked] public TickTimer respawnTimer { get; private set; }
    [Networked] public TickTimer respawnToNewPointTimer { get; private set; }
    [Networked] public TickTimer walkSoundPlay { get; private set; }
    public bool canMove = true;
    [Networked(OnChanged = nameof(OnNickNameChanged))] private NetworkString<_8> playerName { get; set; }


    /// <summary>
    /// NickName Segment
    /// </summary>
    /// <param name="changed"></param>
    private static void OnNickNameChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.SetPlayerNickName(changed.Behaviour.playerName);
    }

    private void SetPlayerNickName(NetworkString<_8> nickName)
    {
        playerNameText.text = nickName + " " + Object.InputAuthority.PlayerId;
    }

    [Rpc(sources: RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcSetNickname(NetworkString<_8> nickName)
    {
        playerName = nickName;
    }

    public enum SpriteType
    {
        Wizard,
        AxeMan,
        Warrior
    }

    /// <summary>
    /// Called once only on the owner
    /// </summary>
    public override void Spawned()
    {
        _healthManager = GetComponent<PlayerHealth>();
        isMOVABLE = true;
        canMove = true;
        switch (attackerMode)
        {
            case SpriteType.Wizard:
                ANIM_NAME = "Wizard_Anim";
                break;
            case SpriteType.AxeMan:
                ANIM_NAME = "AxeAnim";
                break;
            case SpriteType.Warrior:
                ANIM_NAME = "WarriorSwordAnim";
                break;
            default:
                break;
        }

        // Cursor lockstate
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetLocalObjects();
        _isPlayerAlive = true;
    }

    private void SetLocalObjects()
    {
        if (Utils.IsLocalPlayer(Object))
        {
            RpcSetNickname(PlayerPrefs.GetString("Nickname"));
            gameObject.layer = 8;
        }
        else
        {
            worldCam.enabled = false;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        GlobalManagers.Instance.objectPoolingManager.RemoveNetworkObjectFromDic(Object);
        Destroy(gameObject);
    }

    public void KillPlayer()
    {
        if (Runner.IsServer)
        {
            serverNextSpawnPoint = GlobalManagers.Instance.playerController.GetRandomSpawnPoint();
            respawnToNewPointTimer = TickTimer.CreateFromSeconds(Runner, 4);
        }
        _isPlayerAlive = false;

        _visualRenderer.TriggerDieAnimation();

        respawnTimer = TickTimer.CreateFromSeconds(Runner, 5f);
    }

    private void CheckRespawnTimer()
    {
        if (_isPlayerAlive)
            return;

        if (respawnToNewPointTimer.Expired(Runner))
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
        _visualRenderer.TriggerRespawnAnimation();

        _healthManager.ResetHealth();
    }

    /// <summary>
    /// Called Before the FixedNetworkUpdate
    /// </summary>
    public void BeforeUpdate()
    {
        if (Utils.IsLocalPlayer(Object) && AcceptAnyInput)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        CheckRespawnTimer();
        if(Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            if(AcceptAnyInput && isMOVABLE)
            {
                    Vector3 playerScale = transform.localScale;

                    if (input._horizontalInput != 0 || input._verticalInput != 0)
                    {
                        Vector3 curPos = transform.position;
                        Vector3 targetPos = curPos + _moveSpeed * Runner.DeltaTime * new Vector3(input._horizontalInput, input._verticalInput, 0);

                        // Check for obstacles in the "Obstacles" layer at the target position
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPos, 0.1f, LayerMask.GetMask("Obstacles"));
                        canMove = true;
                        foreach (var collider in colliders)
                        {
                            // You can also check for specific conditions on the obstacle if needed
                            if (collider != null)
                            {
                                canMove = false;
                                break;
                            }
                        }

                        if (canMove)
                        {
                            // Update the character's position
                            transform.position = targetPos;

                            // Update the character's scale based on the horizontal input
                            if (input._horizontalInput < 0)
                                playerScale.x = -Mathf.Abs(playerScale.x);
                            else if (input._horizontalInput > 0)
                                playerScale.x = Mathf.Abs(playerScale.x);

                            transform.localScale = playerScale;
                        }
                    }   

                buttonPrev = input.networkButtons;
            }
        }
    }

    public PlayerData PlayerInputDataStruct()
    {
        PlayerData player = new PlayerData();
        player._horizontalInput = horizontal;

        player._verticalInput = vertical;

        player.networkButtons.Set(PlayerButtons.Shoot, Input.GetButton("Fire1"));

        return player;
    }
}