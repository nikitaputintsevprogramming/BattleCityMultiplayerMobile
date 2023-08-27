using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour //���� ������� ������, ��� ��� ������� ������
{
    public int Health;
    public GameObject[] HealthIndicators;

    void Update()
    {
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

    [SyncVar(hook = nameof(SyncHealth))] //������ �����, ������� ����� ����������� ��� ������������� ����������
    int _SyncHealth;

    //����� �� ����������, ���� ������ �������� ����� ������
    void SyncHealth(int oldValue, int newValue) //����������� ������ ��� �������� - ������ � �����. 
    {
        Health = newValue;
    }
}