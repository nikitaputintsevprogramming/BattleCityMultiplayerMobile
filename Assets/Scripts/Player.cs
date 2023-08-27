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

    void Update()
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
        PlayerMovement();
        UpdateHealthIndicators();
    }

    void PlayerMovement()
    {
        if (isOwned) 
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float speed = 5f * Time.deltaTime;
            transform.Translate(new Vector2(h * speed, v * speed));
        }
    }

    void UpdateHealthIndicators()
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
    }

    [Command] //обозначаем, что этот метод должен будет выполняться на сервере по запросу клиента
    public void CmdChangeHealth(int newValue) //обязательно ставим Cmd в начале названия метода
    {
        ChangeHealthValue(newValue); //переходим к непосредственному изменению переменной
    }
}