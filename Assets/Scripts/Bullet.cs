using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour 
{
    private uint _owner;
    private bool _inited;
    private Vector3 _target;

    private View _view;

    public void Start()
    {
        _view = Camera.main.GetComponent<View>();
    }

    [Server]
    public void Init(uint owner, Vector3 target)
    {
        _owner = owner;
        _target = target;
        _inited = true;
    }

    void Update() 
    {
        if (_inited && isServer) 
        {
            transform.Translate(Vector3.up * 5f * Time.deltaTime);

            foreach (var item in Physics2D.OverlapCircleAll(transform.position, 0.5f)) 
            {
                Player player = item.GetComponent<Player>();
                if (player) 
                {
                    if (player.netId != _owner) 
                    {
                        player.ChangeHealthValue(player.Health - 1);
                        NetworkServer.Destroy(gameObject);
                    }
                }
            }

            if (transform.position.x < _view.min.x || transform.position.x > _view.max.x ||
                transform.position.y < _view.min.y || transform.position.y > _view.max.y)
            {
                NetworkServer.Destroy(gameObject);
            }
             
            
        }
    }
}