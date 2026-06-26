using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EventSystem;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class GoldenEgg : MonoBehaviour
{
    public float animationTime;
    public float pointTime;
    public ParticleSystem givePointsParticles;
    public Image pointCooldownImage;
    public AudioSource pling;
    public float dropDistance;
    public GameEvent onScorePoint;
    [Range(0f, 1f)]
    public float dashStealDelayFactor;

    [FormerlySerializedAs("Mask")] public LayerMask mask;

    public Vector2 min;
    public Vector2 max;

    private float _timer = 0f;
    private float _pointTimer = 0f;
    private bool _canPickUp;
    private Vector3 _dropStart;
    private Vector3 _dropPosition;

    private List<PlayerController> _inside;

    private PlayerController _currentOwner;

    public void LoadFromSettings()
    {
        animationTime = SettingsManager.Instance.settings.goldenGoosePickUpDelay;
        pointTime = SettingsManager.Instance.settings.goldenGooseSecondsPerPoint;
    }

    private void Awake()
    {
        _inside = new List<PlayerController>();
        LoadFromSettings();
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;
        var t = _timer / animationTime;

        if (!_currentOwner && _canPickUp && _inside.Count > 0)
        {
            _currentOwner = _inside.First();
            _inside.Clear();
            transform.SetParent(_currentOwner.transform);
            _pointTimer = 0f;
        }

        if (_currentOwner)
        {
            _pointTimer += Time.deltaTime;
        }
        else if (!_canPickUp)
        {
            transform.position = Vector3.Lerp(_dropStart, _dropPosition, t) + Vector3.up * (0.5f - Mathf.Abs(t - 0.5f)) * 2f;
        }

        if (t >= 1f)
        {
            _timer = 0f;
            _canPickUp = true;
            transform.localPosition = new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z);
        }

        if (_currentOwner)
        {
            transform.localPosition = new Vector3(0f, 2f, 0f);
        }

        var point_t = _pointTimer / pointTime;
        pointCooldownImage.fillAmount = point_t;
        if (point_t >= 1f && _currentOwner)
        {
            onScorePoint.Raise(this, _currentOwner.gameObject);
            givePointsParticles.Play();
            pling.Play();
            _pointTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        PlayerController p = col.GetComponent<PlayerController>();
        if (p){
            _inside.Add(p);
        }
    }
    
    private void OnTriggerExit(Collider col)
    {
        PlayerController p = col.GetComponent<PlayerController>();
        if (_inside.Contains(p)){
            _inside.Remove(p);
        }
    }

    public void OnDashInto(Component sender, object[] obj)
    {
        if (!_currentOwner) return;
        if (obj.First() is CollisionData col)
        {
            if (sender == _currentOwner && _canPickUp)
            {
                _currentOwner = col.OriginCollider.GetComponent<PlayerController>();
                transform.SetParent(col.OriginCollider.transform);
                _pointTimer = 0f;
                _timer = animationTime * dashStealDelayFactor;
                _canPickUp = false;
            }
        }
    }

    public void OnAttacked(Component sender, object[] _obj)
    {
        if (!_currentOwner) return;
        if (sender == _currentOwner)
        {
            Drop();
        }
    }

    public void Drop()
    {
        _currentOwner = null;
        transform.SetParent(null);
        _dropStart = new Vector3(transform.position.x, 0f, transform.position.z);
        _dropPosition = new Vector3(transform.position.x, 0f, transform.position.z) +
                       new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * dropDistance;
        _dropPosition.x = Mathf.Clamp(_dropPosition.x, min.x, max.x);
        _dropPosition.z = Mathf.Clamp(_dropPosition.z, min.y, max.y);
        if (Physics.Raycast(transform.position, _dropPosition - transform.position, out var hitInfo, mask))
        {
            _dropPosition = hitInfo.point + hitInfo.normal;
            _dropPosition.y = 0f;
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
