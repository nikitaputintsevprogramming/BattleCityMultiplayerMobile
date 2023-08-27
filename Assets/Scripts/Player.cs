using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour //даем системе понять, что это сетевой объект
{

    [SyncVar(hook = nameof(SyncHealth))] //задаем метод, который будет выполняться при синхронизации переменной
    int _SyncHealth;
    public int Health;
    public GameObject[] HealthIndicators;

    public GameObject BulletPrefab;

    private void Update()
    {
        PlayerDamage();
        SpawnBullet();
        PlayerMovement();
        UpdateHealthIndicators();
    }

    public void SpawnBullet()
    {
        if (isOwned) //проверяем, есть ли у нас права изменять этот объект
        {
          if (Input.GetKeyDown(KeyCode.Mouse1))
          {
            Vector3 pos = Input.mousePosition;
            pos.z = 10f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            if (isServer)
                SpawnBullet(netId, pos);
            else
                CmdSpawnBullet(netId, pos);
          }
        }
    }

    public void PlayerDamage()
    {
        if (isOwned) //проверяем, есть ли у нас права изменять этот объект
        {
            if (Input.GetKeyDown(KeyCode.H)) //отнимаем у себя жизнь по нажатию клавиши H
            {
                if (isServer) //если мы являемся сервером, то переходим к непосредственному изменению переменной
                    ChangeHealthValue(Health - 1);
                else
                    CmdChangeHealth(Health - 1); //в противном случае делаем на сервер запрос об изменении переменной
            }
        }
    }

    public void PlayerMovement()
    {
        if (isOwned) 
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float speed = 5f * Time.deltaTime;
            transform.Translate(new Vector2(h * speed, v * speed));
        }
    }

    public void UpdateHealthIndicators()
    {
        for (int i = 0; i < HealthIndicators.Length; i++)
        {
            HealthIndicators[i].SetActive(!(Health - 1 < i));
        }
    }

    //метод не выполнится, если старое значение равно новому
    void SyncHealth(int oldValue, int newValue) //обязательно делаем два значения - старое и новое. 
    {
        Health = newValue;
    }

    [Server] //обозначаем, что этот метод будет вызываться и выполняться только на сервере
    public void ChangeHealthValue(int newValue)
    {
        _SyncHealth = newValue;
        if (_SyncHealth <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    [Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(int newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue); //переходим к непосредственному изменению переменной
    }

    // Спавн пули на сервере
    [Server]
    public void SpawnBullet(uint owner, Vector3 target)
    {
        GameObject bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity); //Создаем локальный объект пули на сервере
        NetworkServer.Spawn(bulletGo); //отправляем информацию о сетевом объекте всем игрокам.
        bulletGo.GetComponent<Bullet>().Init(owner, target); //инициализируем поведение пули
    }
    [Command]
    // Запрос на спавн пули со стороны клиента
    public void CmdSpawnBullet(uint owner, Vector3 target)
    {
        SpawnBullet(owner, target);
    }


}