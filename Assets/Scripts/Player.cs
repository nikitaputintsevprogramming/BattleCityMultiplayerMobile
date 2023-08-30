using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour 
{
    [SyncVar(hook = nameof(SyncHealth))]
    private int _syncHealth;
    [field: SerializeField]
    public int Health { get; private set; }

    [SerializeField]
    private GameObject[] healthGos;

    [SerializeField]
    private GameObject bulletPrefab;

    private int _pointsCount;
    private Joystick _joystick;
    private View _view;

    private void Start()
    {
        GameObject.FindGameObjectWithTag("ButtonFIre").GetComponent<Button>().onClick.AddListener(Fire);
        _joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        _view = Camera.main.GetComponent<View>();
    }

    void Update() 
    {
        if (isOwned) {
            float h = _joystick.Horizontal();
            float v = _joystick.Vertical();
            float speedMove = 5f * Time.deltaTime;
            float speedRotate = 50f * Time.deltaTime;
            transform.Translate(new Vector2(0f, v * speedMove));
            transform.Rotate(0f, 0f, -h * speedRotate);

            if (transform.position.x < _view.min.x || transform.position.x > _view.max.x ||
                transform.position.y < _view.min.y || transform.position.y > _view.max.y)
            {
                transform.position = Vector3.MoveTowards(this.transform.position, new Vector2(_view.max.x/2f, _view.max.y/2f), 70f * Time.deltaTime);
            }
        }

        for (int i = 0; i < healthGos.Length; i++) 
        {
            healthGos[i].SetActive(!(Health - 1 < i));
        }
    }

    public void Fire()
    {
        if (isOwned)
        {

            Vector3 pos = Vector3.up;
            pos.z = 10f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            if (isServer)
            {
                SpawnBullet(netId, pos);
            }
            else
            {
                CmdSpawnBullet(netId, pos);
            }
        }

    }

    void SyncHealth(int oldValue, int newValue) 
    {
        Health = newValue;
    }

    [Server]
    public void ChangeHealthValue(int newValue) 
    {
        _syncHealth = newValue;

        if (_syncHealth <= 0) {
            NetworkServer.Destroy(gameObject);
        }
    }

    [Command]
    public void CmdChangeHealth(int newValue) 
    {
        ChangeHealthValue(newValue);
    }

    public override void OnStartClient() 
    {
        base.OnStartClient();
    }

    [Server]
    public void SpawnBullet(uint owner, Vector3 target) 
    {
        GameObject bulletGo = Instantiate(bulletPrefab, transform.position, this.transform.rotation);

        NetworkServer.Spawn(bulletGo);

        bulletGo.GetComponent<Bullet>().Init(owner, target);
    }

    [Command]
    public void CmdSpawnBullet(uint owner, Vector3 target) 
    {
        SpawnBullet(owner, target);
    }
}