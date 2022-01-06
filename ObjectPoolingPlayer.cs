using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingPlayer : MonoBehaviour{

    public static ObjectPoolingPlayer _sharedInstance;
    //List to hold all the instantited object
    private List<GameObject> _pooledObject;
    //Object to pool
    [SerializeField] private GameObject _objectToPool;
    //no of instances
    [SerializeField] private int _amountToPool;

    void Awake(){
        _sharedInstance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Initialized the list 
        _pooledObject = new List<GameObject>();
        
        //Create a temporary variable
        GameObject tmp;

        for( int i = 0; i < _amountToPool; i++ ){
            //Instantiate required no of object
            tmp = Instantiate(_objectToPool);
            tmp.GetComponent<Missile>()._isEnemyRocket = false;
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
                _pooledObject[i].GetComponent<TrailRenderer>().enabled = true;
                return _pooledObject[i];
            }
        }
        return null;
    }
}