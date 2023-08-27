using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour //���� ������� ������, ��� ��� ������� ������
{

    [SyncVar(hook = nameof(SyncHealth))] //������ �����, ������� ����� ����������� ��� ������������� ����������
    int _SyncHealth;
    public int Health;
    public GameObject[] HealthIndicators;

    void Update()
    {
        if (isOwned) //���������, ���� �� � ��� ����� �������� ���� ������
        {
            if (Input.GetKeyDown(KeyCode.H)) //�������� � ���� ����� �� ������� ������� H
            {
                if (isServer) //���� �� �������� ��������, �� ��������� � ����������������� ��������� ����������
                    ChangeHealthValue(Health - 1);
                else
                    CmdChangeHealth(Health - 1); //� ��������� ������ ������ �� ������ ������ �� ��������� ����������
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

    //����� �� ����������, ���� ������ �������� ����� ������
    void SyncHealth(int oldValue, int newValue) //����������� ������ ��� �������� - ������ � �����. 
    {
        Health = newValue;
    }

    [Server] //����������, ��� ���� ����� ����� ���������� � ����������� ������ �� �������
    public void ChangeHealthValue(int newValue)
    {
        _SyncHealth = newValue;
    }

    [Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(int newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue); //��������� � ����������������� ��������� ����������
    }
}