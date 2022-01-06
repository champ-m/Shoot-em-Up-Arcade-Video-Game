using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingEnemy : MonoBehaviour{

    public static ObjectPoolingEnemy _sharedInstance;
    public static bool _updateTarget;


    //List to hold all the instantited object
    private List<GameObject> _pooledObject;
    //Object to pool
    [SerializeField] private GameObject _objectToPool;
    //no of instances
    [SerializeField] private int _amountToPool;

    // private GameObject[] Targets;

    void Awake(){
        _sharedInstance = this;
    }
    // Start is called before the first frame update
    void Start(){

        //Initialized the list 
        _pooledObject = new List<GameObject>();
        //GetRocketSpawned
        SpawnMissiles();
        //Set Missile Target
        SetMissileTarget();
    }

    void Update(){
        if(_updateTarget){
            SetMissileTarget();
            _updateTarget = false;
        }
    }

    void SpawnMissiles(){

        //Create a temporary variable
        GameObject tmp;

        for( int i = 0; i < _amountToPool; i++ ){
            
            //Will represent a random position on x in game
            Vector2 spawnPose = new Vector2(Random.Range(-9.5f, 9.5f), 6.0f);
            //Instantiate required no of object
            tmp = Instantiate(_objectToPool, spawnPose, Quaternion.identity);
            tmp.GetComponent<CircleCollider2D>().enabled = false;
            tmp.GetComponent<Missile>()._isEnemyRocket = true;
            tmp.GetComponent<Missile>()._initialPose = spawnPose;
            //Set active to false
            tmp.SetActive(false);
            //Add the object to List
            _pooledObject.Add(tmp);
        }   
    }

    public void DeactivateAllPooledObject(){
        //Deactivate the object which is active in hierarchy
        for( int i = 0; i < _amountToPool; i++ ){
            if(_pooledObject[i].activeInHierarchy){
                _pooledObject[i].GetComponent<CircleCollider2D>().enabled = false;
                _pooledObject[i].SetActive(false);
            }
        }
    }
    public GameObject GetPooledObject(){

        //Return the object which isn't active in hierarchy
        for( int i = 0; i < _amountToPool; i++ ){
            if(!_pooledObject[i].activeInHierarchy){
                _pooledObject[i].GetComponent<CircleCollider2D>().enabled = true;
                _pooledObject[i].GetComponent<TrailRenderer>().enabled = true;
                return _pooledObject[i];
            }
        }
        return null;
    }

    public static int GetActivePost(){
        GameObject[] argTargetpose = GameObject.FindGameObjectsWithTag("post");
        return argTargetpose.Length;
    }
    
    public void SetMissileTarget(){

        GameObject[] argTargetpose = GameObject.FindGameObjectsWithTag("post");

        if(argTargetpose.Length == 0){
            return;
        }

        foreach(GameObject objTemp in _pooledObject){
            
            if(objTemp.activeInHierarchy)
                continue;

            int post_no;

            do{
                post_no =Random.Range(0,argTargetpose.Length);

            }while(argTargetpose.Length != 0 && !argTargetpose[post_no].activeInHierarchy);

            // Debug.Log(post_no);
            Missile objTempMissile = objTemp.GetComponent<Missile>();

            //Set destination for the missile
            objTempMissile._destPosition = (Vector2)argTargetpose[post_no].transform.position;

            //Get Direction of Enemy Missile
            Vector2 dir =  objTempMissile._destPosition - (Vector2)objTempMissile.gameObject.transform.position;

            //Find the angle betweeen perpendicular and hypotenes
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //Set the gameobject rotation according to the newly found angle
            objTemp.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Debug.Log("GameObject with name : "+objTemp.name + " initialized to target number " + post_no +" With position : " + argTargetpose[post_no].transform.position);
        }
    }
}