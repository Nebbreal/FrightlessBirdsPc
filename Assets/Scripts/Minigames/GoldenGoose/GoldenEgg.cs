using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EventSystem;
using System.Collections.Generic;

public class GoldenEgg : MonoBehaviour
{
    public float AnimationTime;
    public float PointTime;
    public ParticleSystem GivePointsParticles;
    public Image PointCooldownImage;
    public AudioSource Pling;
    public float DropDistance;
    public GameEvent onScorePoint;
    [Range(0f, 1f)]
    public float DashStealDelayFactor;

    public LayerMask Mask;

    public Vector2 min;
    public Vector2 max;

    private float _timer = 0f;
    private float _pointTimer = 0f;
    private bool _canPickUp;
    private Vector3 dropStart;
    private Vector3 dropPosition;

    private List<PlayerController> inside;

    private PlayerController currentOwner;

    public void LoadFromSettings()
    {
        AnimationTime = SettingsManager.Instance.Settings.GGPickUpDelay;
        PointTime = SettingsManager.Instance.Settings.GGSecondsPerPoint;
    }

    private void Awake()
    {
        inside = new List<PlayerController>();
        LoadFromSettings();
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;
        var t = _timer / AnimationTime;

        if (!currentOwner && _canPickUp && inside.Count > 0)
        {
            currentOwner = inside.First();
            inside.Clear();
            transform.SetParent(currentOwner.transform);
            _pointTimer = 0f;
        }

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
            _canPickUp = true;
            transform.localPosition = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
        }

        if (currentOwner)
        {
            transform.localPosition = new Vector3(0f, 2f, 0f);
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
        PlayerController p = col.GetComponent<PlayerController>();
        if (p){
            inside.Add(p);
        }
    }
    
    private void OnTriggerExit(Collider col)
    {
        PlayerController p = col.GetComponent<PlayerController>();
        if (inside.Contains(p)){
            inside.Remove(p);
        }
    }

    public void OnDashInto(Component sender, object[] obj)
    {
        if (!currentOwner) return;
        if (obj.First() is CollisionData col)
        {
            if (sender == currentOwner && _canPickUp)
            {
                currentOwner = col.OriginCollider.GetComponent<PlayerController>();
                transform.SetParent(col.OriginCollider.transform);
                _pointTimer = 0f;
                _timer = AnimationTime * DashStealDelayFactor;
                _canPickUp = false;
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
        dropStart = new Vector3(transform.position.x, 0f, transform.position.z);
        dropPosition = new Vector3(transform.position.x, 0f, transform.position.z) +
                       new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * DropDistance;
        dropPosition.x = Mathf.Clamp(dropPosition.x, min.x, max.x);
        dropPosition.z = Mathf.Clamp(dropPosition.z, min.y, max.y);
        if (Physics.Raycast(transform.position, dropPosition - transform.position, out var hitInfo, Mask))
        {
            dropPosition = hitInfo.point + hitInfo.normal;
            dropPosition.y = 0f;
        }
        _pointTimer = 0f;
        _timer = 0f;
        _canPickUp = false;
    }

    public void DestroyEgg()
    {
        Destroy(gameObject);
    }
}
