using UnityEngine;

public class Crosshair : MonoBehaviour
{
    //ENUMERATOR FOR ROCKETS
    // private enum MISSILE : int{ PLAYER_SIMPLE, PLAYER_POINTY, ENEMY_SIMPLE, ENEMY_POINTY, PLAYER_SPECIAL };

    //GAMEPLAY FLAGS & Data
    [Header("Gameplay Flags & Data")]
    public bool _isPlaying;
  

    //DATAFIELDS PUBLIC
    [Header("Datafields")]

    [SerializeField]
    [Range(0.0f, 45.0f)] private float _crossHairLag = 45.0f;
    [SerializeField] private GameObject _weapon;    

    

    //DATAFIELDS PRIVATE
    private Vector2 _MousePose;
    private float _nextTimeToLaunchRocketE;
    private float _nextTimeToLaunchRocketP;

    void Start()
    {
        Debug.Log("Start of Crosshair is called");
#if UNITY_ANDROID
        this.GetComponent<SpriteRenderer>().sprite = null;
#endif
        _isPlaying = true;
        _nextTimeToLaunchRocketE = 0.0f;
        _nextTimeToLaunchRocketP = 0.0f;

        //Get the number of post in the active scene
        UI_Handler._postLeft = ObjectPoolingEnemy.GetActivePost();

        //Set the Main play flag to true
        UI_Handler._PlayFlagMain = _isPlaying;
    }

    void Update()
    {

        if (UI_Handler._PlayFlagMain ){
            
            //Get Mouse pose according to the world space
            _MousePose = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Set Crosshair Position
            SetCrossHairPosition();

            //Set Weapon Rotation
            SetWeaponRotation();

            if(Input.GetButtonDown("Fire1") && Time.time >= _nextTimeToLaunchRocketP){
                
                //Play Launch audio for the missiles
                SoundManager._sharedInstance.PlayMissileLaunch();

#if UNITY_ANDROID
                SetWeaponRotation();
                FireMissileOPAndroid();
#else
                FireMissileOP();
#endif
                _nextTimeToLaunchRocketP = Time.time + (1.0f/UI_Handler._sharedInstance._playerMissileFireRate);
            }

            //Enemy Missile Rocket
            if(Time.time >= _nextTimeToLaunchRocketE){
                //Play Launch audio for missiles
                SoundManager._sharedInstance.PlayMissileLaunch();

                FireEnemyMissileOP();
                _nextTimeToLaunchRocketE = Time.time + (1.0f/UI_Handler._sharedInstance._enemyMissileFireRate);
            }
        }
    }

    void FireMissileOP(){

        //Get an objcet from the pool
        GameObject _missileLocal = ObjectPoolingPlayer._sharedInstance.GetPooledObject();
        if( _missileLocal != null ){
            _missileLocal.transform.position = _weapon.transform.position;
            _missileLocal.transform.rotation = _weapon.transform.rotation;
            _missileLocal.transform.localScale = new Vector2(0.07f, 0.07f);
            _missileLocal.SetActive(true);
        }

        //Calculate the _direction of force
        Vector2 _direction = (this.gameObject.transform.position - _weapon.transform.position).normalized;
        
        //Add Force to the missile
        _missileLocal.GetComponent<Rigidbody2D>().AddForce(_direction * UI_Handler._sharedInstance._missileSpeedPlayer * 100.0f);
        
        //Set the destination for the missile
        _missileLocal.GetComponent<Missile>()._destPosition = this.gameObject.transform.position;
    }

    //for Android
    void FireMissileOPAndroid(){

        //Get an objcet from the pool
        GameObject _missileLocal = ObjectPoolingPlayer._sharedInstance.GetPooledObject();
        if( _missileLocal != null ){
            _missileLocal.transform.position = _weapon.transform.position;
            _missileLocal.transform.rotation = _weapon.transform.rotation;
            _missileLocal.transform.localScale = new Vector2(0.07f, 0.07f);
            _missileLocal.SetActive(true);
        }

        //Calculate the _direction of force
        Vector2 _direction = (_MousePose - (Vector2)_weapon.transform.position).normalized;
        
        //Add Force to the missile
        _missileLocal.GetComponent<Rigidbody2D>().AddForce(_direction * UI_Handler._sharedInstance._missileSpeedPlayer * 100.0f);
        
        //Set the destination for the missile
        _missileLocal.GetComponent<Missile>()._destPosition = _MousePose;
    }
    void FireEnemyMissileOP()
    {
        GameObject _missileLocal = ObjectPoolingEnemy._sharedInstance.GetPooledObject();
        if(_missileLocal != null){

            _missileLocal.transform.localScale = new Vector2(0.07f, 0.07f);
            _missileLocal.SetActive(true);
            
            // ObjectPoolingEnemy._updateTarget = true;
            //Direction for the missile
            Vector2 direction = (_missileLocal.GetComponent<Missile>()._destPosition - (Vector2)_missileLocal.transform.position);
            Debug.Log("Direction for Missile : " + direction);

            //Add Force to the missile
            _missileLocal.GetComponent<Rigidbody2D>().AddForce(direction * UI_Handler._sharedInstance._missileSpeedEnemy * 10.0f);
            Debug.Log("Added Force to Missile");
        }
    }
    
    void SetCrossHairPosition(){
        //TO SET THE POSITON OF CROSSHAIR
        //Lerp through the positions inbetween for a smooth effect
        this.gameObject.transform.position = Vector2.Lerp(_MousePose, this.transform.position, (Time.fixedDeltaTime * _crossHairLag));
    }

    void SetWeaponRotation(){
        //TO SET THE ROTATION OF WEAPON
        //Get the gun position
#if UNITY_ANDROID
        Vector2 dir = _MousePose - (Vector2)_weapon.transform.position;
#else
        Vector2 dir = this.gameObject.transform.position - _weapon.transform.position;
#endif
        //Find the angle betweeen perpendicular and hypotenes
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //Set the gameobject rotation according to the newly found angle
        _weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}