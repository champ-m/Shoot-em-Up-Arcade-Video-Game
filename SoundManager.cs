using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _sharedInstance;
    [SerializeField] private AudioClip _missileExplosion;
    [SerializeField] private AudioClip _postExplosion;
    [SerializeField] private AudioClip _missileLaunch;
    public bool _isMuted;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = this.GetComponent<AudioSource>();  
        _sharedInstance = this;
        
        //Check the audio setting
        switch (PlayerPrefs.GetInt("AudioMute")){
            case 0: SoundManager._sharedInstance._isMuted = true; break;
            case 1: SoundManager._sharedInstance._isMuted = false;  break;
        }
    }

    public void PlayMissileExplosion(){
        if(!_isMuted)
            _audioSource.PlayOneShot(_missileExplosion);
    }

    public void PlayPostExplosion(){
        if(!_isMuted)
            _audioSource.PlayOneShot(_postExplosion);
    }

    public void PlayMissileLaunch(){
        if(!_isMuted)
            _audioSource.PlayOneShot(_missileLaunch);
    }

}