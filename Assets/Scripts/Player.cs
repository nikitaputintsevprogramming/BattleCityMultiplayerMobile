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
        if (isOwned) //���������, ���� �� � ��� ����� �������� ���� ������
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

    //����� �� ����������, ���� ������ �������� ����� ������
    void SyncHealth(int oldValue, int newValue) //����������� ������ ��� �������� - ������ � �����. 
    {
        Health = newValue;
    }

    [Server] //����������, ��� ���� ����� ����� ���������� � ����������� ������ �� �������
    public void ChangeHealthValue(int newValue)
    {
        _SyncHealth = newValue;
        if (_SyncHealth <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    [Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(int newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue); //��������� � ����������������� ��������� ����������
    }

    // ����� ���� �� �������
    [Server]
    public void SpawnBullet(uint owner, Vector3 target)
    {
        GameObject bulletGo = Instantiate(BulletPrefab, transform.position, Quaternion.identity); //������� ��������� ������ ���� �� �������
        NetworkServer.Spawn(bulletGo); //���������� ���������� � ������� ������� ���� �������.
        bulletGo.GetComponent<Bullet>().Init(owner, target); //�������������� ��������� ����
    }
    [Command]
    // ������ �� ����� ���� �� ������� �������
    public void CmdSpawnBullet(uint owner, Vector3 target)
    {
        SpawnBullet(owner, target);
    }


}