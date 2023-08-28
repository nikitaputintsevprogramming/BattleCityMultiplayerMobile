using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private uint _owner;
    private bool _inited;
    private Vector3 _target;

    public float BulletSpeed = 1000f;

    [Server]
    public void Init(uint owner, Vector3 target)
    {
        //who did the shot
        //��� ������ �������
        _owner = owner;

        //where the bullet should go
        //���� ������ ������ ����
        _target = target;
        _inited = true;
    }

    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * BulletSpeed);
    }

    void Update()
    {
        if (_inited && isServer)
        {
            //transform.Translate((_target - transform.position).normalized * 0.04f);

            foreach (var item in Physics2D.OverlapCircleAll(transform.position, 0.5f))
            {
                Player player = item.GetComponent<Player>();
                if (player)
                {
                    if (player.netId != _owner)
                    {
                        //take one HP by analogy with the SyncVar example
                        //�������� ���� ����� �� �������� � �������� SyncVar
                        player.ChangeHealthValue(player.Health - 1);
                        //destroy the bullet
                        //���������� ����
                        NetworkServer.Destroy(gameObject);
                    }
                }
            }

            //bullet has reached the final destination
            //���� �������� �������� �����
            if (Vector3.Distance(transform.position, _target) < 0.1f)
            {
                //then we should destroy it
                //������ �� ����� ����������
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}