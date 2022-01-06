using UnityEngine;

public class Post : MonoBehaviour
{
    // [HideInInspector] 
    public bool _executeExplosion;
    private Animator _animator;
    

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _executeExplosion = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(_executeExplosion){
            _animator.Play("explosion");
            Debug.Log("Post" + this.gameObject.name + " Destroying");
            _executeExplosion = false;
        }
        
    }
    public void FinishedPlayingAnimation()
    {
        Debug.Log("Post" + this.gameObject.name + " Destroyed");
        this.gameObject.SetActive(false);
    }
}