using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//mark the object as a network object by inheriting from NetworkBehaviour
//помечаем объект как сетевой, унаследовавшись от NetworkBehaviour
public class Player : NetworkBehaviour
{
    //set the method to be called when the variable is synced
    //задаем метод, который будет выполняться при синхронизации переменной
    [SyncVar(hook = nameof(SyncHealth))]
    private int _syncHealth;
    [field: SerializeField]
    public int Health { get; private set; }

    [SerializeField]
    private GameObject[] healthGos;

    [SerializeField]
    private GameObject bulletPrefab;
    public GameObject FireButton;

    private Joystick _jstick;

    private void Start()
    {
        if (isOwned)
        {
            _jstick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
            Button btn = GameObject.FindGameObjectWithTag("ButtonFIre").GetComponent<Button>();
            btn.onClick.AddListener(TaskOnClick);
        }
    }

    void TaskOnClick()
    {
        Vector3 pos = Input.mousePosition;
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

    void Update()
    {
        //check the ownershop of the object
        //проверяем, есть ли у нас права изменять этот объект
        if (isOwned)
        {
            Movement();
            Damage();
            //SpawnBullet();
        }
        UpdateHelathBar();
    }

    public void Movement()
    {
        float h = _jstick.Horizontal();
        float v = _jstick.Vertical();
        float speedTranslate = 5f * Time.deltaTime;
        float speedRotate = 50f * Time.deltaTime;
        transform.position += transform.up * -v * speedTranslate;
        transform.Rotate(transform.forward * h * speedRotate);
    }

    public void Damage()
    {
        //take HP from self by pressing the H key
        //отнимаем у себя жизнь по нажатию клавиши H
        if (Input.GetKeyDown(KeyCode.H))
        {
            //if we are the server, then go to the changing of the variable
            //если мы являемся сервером, то переходим к непосредственному изменению переменной
            if (isServer)
            {
                ChangeHealthValue(Health - 1);
            }
            else
            {
                //in other case, send a change request to the server
                //в противном случае делаем на сервер запрос об изменении переменной
                CmdChangeHealth(Health - 1);
            }
        }
    }

    //public void SpawnBullet()
    //{
    //    if (Input.GetKeyDown(KeyCode.Mouse1))
    //    {
    //        Vector3 pos = Input.mousePosition;
    //        pos.z = 10f;
    //        pos = Camera.main.ScreenToWorldPoint(pos);
            //if (isServer)
            //{
            //    SpawnBullet(netId, pos);
            //}
            //else
            //{
            //    CmdSpawnBullet(netId, pos);
            //}

//    }
//}

public void UpdateHelathBar()
    {
        for (int i = 0; i < healthGos.Length; i++)
        {
            healthGos[i].SetActive(!(Health - 1 < i));
        }
    }

    //sync hook always required two values - old and new
    //обязательно делаем два значения - старое и новое
    void SyncHealth(int oldValue, int newValue)
    {
        Health = newValue;
    }

    //mark the method for calling and executing only on the server
    //обозначаем, что этот метод будет вызываться и выполняться только на сервере
    [Server]
    public void ChangeHealthValue(int newValue)
    {
        _syncHealth = newValue;

        if (_syncHealth <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    //mark the method for calling on the client and executing on the server
    //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    [Command]
    //be sure to put Cmd at the beginning of the method name
    //обязательно ставим Cmd в начале названия метода
    public void CmdChangeHealth(int newValue)
    {
        //переходим к непосредственному изменению переменной
        ChangeHealthValue(newValue);
    }


    [Server]
    public void SpawnBullet(uint owner, Vector3 target)
    {
        //create a local object of the bullet on the server
        //создаем локальный объект пули на сервере
        GameObject bulletGo = Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);

        //send info about the network object to all players
        //отправляем информацию о сетевом объекте всем игрокам
        NetworkServer.Spawn(bulletGo);

        //init the bullet behaviour
        //инициализируем поведение пули
        bulletGo.GetComponent<Bullet>().Init(owner, target);
    }

    [Command]
    public void CmdSpawnBullet(uint owner, Vector3 target)
    {
        SpawnBullet(owner, target);
    }
}