using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    uint owner;
    bool inited;
    Vector3 target;

    [Server]
    public void Init(uint owner, Vector3 target)
    {
        this.owner = owner; //��� ������ �������
        this.target = target; //���� ������ ������ ����
        inited = true;
    }

    void Update()
    {
        if (inited && isServer)
        {
            transform.Translate((target - transform.position).normalized * 0.04f);

            foreach (var item in Physics2D.OverlapCircleAll(transform.position, 0.5f))
            {
                Player player = item.GetComponent<Player>();
                if (player)
                {
                    if (player.netId != owner)
                    {
                        player.ChangeHealthValue(player.Health - 1); //�������� ���� ����� �� �������� � �������� SyncVar
                        NetworkServer.Destroy(gameObject); //���������� ����
                    }
                }
            }

            if (Vector3.Distance(transform.position, target) < 0.1f) //���� �������� �������� �����
            {
                NetworkServer.Destroy(gameObject); //������ �� ����� ����������
            }
        }
    }
}