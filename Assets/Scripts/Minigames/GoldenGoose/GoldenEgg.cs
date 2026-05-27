using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EventSystem;

public class GoldenEgg : MonoBehaviour
{
    public float AnimationTime;
    public float PointTime;
    public ParticleSystem GivePointsParticles;
    public Image PointCooldownImage;
    public AudioSource Pling;
    public float DropDistance;
    public GameEvent onScorePoint;

    private float _timer = 0f;
    private float _pointTimer = 0f;
    private bool _canPickUp;
    private Vector3 dropStart;
    private Vector3 dropPosition;

    private PlayerController currentOwner;

    private void Update()
    {
        _timer += Time.deltaTime;
        var t = _timer / AnimationTime;
        if (currentOwner)
        {
            _pointTimer += Time.deltaTime;
        }
        else if (!_canPickUp)
        {
            transform.position = Vector3.Lerp(dropStart, dropPosition, t) + Vector3.up * (0.5f - Mathf.Abs(t - 0.5f)) * 2f;
        }

        if (t >= 1f)
        {
            _timer = 0f;
            t = 0f;
            _canPickUp = true;
            transform.localPosition = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
        }

        if (currentOwner)
        {
            transform.localPosition = new Vector3(0f, 2f, 0f); //.Set(0f, 2f + (t - 0.5f) * 2f, 0f);
        }

        var point_t = _pointTimer / PointTime;
        PointCooldownImage.fillAmount = point_t;
        if (point_t >= 1f && currentOwner)
        {
            onScorePoint.Raise(this, currentOwner.gameObject);
            GivePointsParticles.Play();
            Pling.Play();
            _pointTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!currentOwner && _canPickUp)
        {
            currentOwner = col.GetComponent<PlayerController>();
            transform.SetParent(col.transform);
            _pointTimer = 0f;
        }
    }

    public void OnDashInto(Component sender, object[] obj)
    {
        if (!currentOwner) return;
        if (obj.First() is CollisionData col)
        {
            if (sender == currentOwner)
            {
                currentOwner = col.OriginCollider.GetComponent<PlayerController>();
                transform.SetParent(col.OriginCollider.transform);
                _pointTimer = 0f;
            }
        }
    }

    public void OnAttacked(Component sender, object[] _obj)
    {
        if (!currentOwner) return;
        if (sender == currentOwner)
        {
            Drop();
        }
    }

    public void Drop()
    {
        currentOwner = null;
        transform.SetParent(null);
        dropStart = transform.position;
        dropPosition = transform.position +
                       new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * DropDistance;
        _pointTimer = 0f;
        _timer = 0f;
        _canPickUp = false;
    }

    public void DestroyEgg()
    {
        Destroy(gameObject);
    }
}
