using UnityEngine;

public class Missile : MonoBehaviour
{
    [HideInInspector] public Vector2 _destPosition;
    [HideInInspector] public Vector2 _initialPose;
    [HideInInspector] public bool _isEnemyRocket;
    private float _distanceFromDest;
    private Animator _animator;
    public bool _distructionEnabled;
    private SpriteRenderer _spriteRenderer;
    private Sprite _defaultSprite;


    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultSprite = _spriteRenderer.sprite;
        _animator.Play("default");
        _distructionEnabled = false;
    }

    void Update()
    {
        //Calculate the Distance to Target Destination
        _distanceFromDest = (_destPosition - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;


        //Check if the distance is less than 0.05f
        if ( _distanceFromDest <= 0.05f )
            _distructionEnabled = true;

        //Destroy the Object once reached at the Target position
        if (_distructionEnabled)
        {
            Debug.Log("Playing Animations");
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            this.GetComponent<TrailRenderer>().enabled = false;
            _animator.Play("explosion");
            _distructionEnabled = false;
            
        }

    }
    public void FinishedPlayingAnimation()
    {
        _animator.Play("default");
        Debug.Log("Finshed Playing Animations");
        _spriteRenderer.sprite = _defaultSprite;
        this.gameObject.transform.position = _initialPose;
        this.gameObject.SetActive(false);
        ObjectPoolingEnemy._updateTarget = true;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "EnemyMissile" && !this._isEnemyRocket){
            SoundManager._sharedInstance.PlayMissileExplosion();
            this._distructionEnabled = true;
            other.GetComponent<Missile>()._distructionEnabled = true;
            other.GetComponent<CircleCollider2D>().enabled = false;
            UI_Handler._score += 10;
        }

        if(other.gameObject.tag == "post" && this._isEnemyRocket){
            this.GetComponent<CircleCollider2D>().enabled = false;
            other.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            SoundManager._sharedInstance.PlayMissileExplosion();
            this._distructionEnabled = true;
            other.GetComponent<Post>()._executeExplosion = true;
            UI_Handler._postLeft --;
            SoundManager._sharedInstance.PlayPostExplosion();
        }
    }
}