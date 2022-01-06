using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonText : MonoBehaviour
{
    private Text _text;
    void Start(){
        
        _text = this.GetComponent<Text>();

        //Set Difficulty
        CheckAndSetDifficulty();
        
        //Set Audio status
        CheckAndSetAudio();
    }
    void Awake(){
        _text = this.GetComponent<Text>();
    }
    public void SetSoundButtonText(){
        if(_text){
            if(SoundManager._sharedInstance._isMuted){
                _text.text = "SOUND OFF";
            }
            else{
                _text.text = "SOUND ON";
            }
        }
    }

    public void SetXPText(string argText){
        if(_text){
            if(_text.text.Contains("XP")){
                _text.text = argText;
            }
        }
    }

    public void SetDifficultyButtonText(){
        if(_text){
            if(_text.text == "EASY"){
                _text.text = "MEDIUM";
                PlayerPrefs.SetInt("Difficulty", 1);
                PlayerPrefs.Save();
                SetMediumDifficulty();
            }
            else if(_text.text == "MEDIUM"){
                _text.text = "INSANE";
                PlayerPrefs.SetInt("Difficulty", 2);
                PlayerPrefs.Save();
                SetInsaneDifficulty();
            }
            else if(_text.text == "INSANE"){
                _text.text = "EASY";
                PlayerPrefs.SetInt("Difficulty", 0);
                PlayerPrefs.Save();
                SetEasyDifficulty();
            }
        }
    }

    public void SetPauseButtonText(){

        Debug.Log("Pause button text changed");
        if(_text){

            string local_temp = _text.text;

            if( local_temp == "PAUSE"){
                _text.text = "RESUME";
                UI_Handler._PlayFlagMain = false;
            }
            else if( local_temp == "RESUME"){
                _text.text = "PAUSE";   
                UI_Handler._PlayFlagMain = true;
            }
        }
    }

    public static void SetEasyDifficulty(){
        //Set the Difficulty for the player
        UI_Handler._sharedInstance._enemyMissileFireRate = 0.5f;
        UI_Handler._sharedInstance._playerMissileFireRate = 3.0f;
        UI_Handler._sharedInstance._missileSpeedEnemy = 1.5f;
        UI_Handler._sharedInstance._missileSpeedPlayer = 3.0f;
    }
    public static void SetMediumDifficulty(){
        //Set the Difficulty for the player
        UI_Handler._sharedInstance._enemyMissileFireRate = 0.75f;
        UI_Handler._sharedInstance._playerMissileFireRate = 4.5f;
        UI_Handler._sharedInstance._missileSpeedEnemy = 2.0f;
        UI_Handler._sharedInstance._missileSpeedPlayer = 4.5f;
    }
    public static void SetInsaneDifficulty(){
        //Set the Difficulty for the player
        UI_Handler._sharedInstance._enemyMissileFireRate = 1.0f;
        UI_Handler._sharedInstance._playerMissileFireRate = 10.0f;
        UI_Handler._sharedInstance._missileSpeedEnemy = 3.0f;
        UI_Handler._sharedInstance._missileSpeedPlayer = 6.0f;
    }

    void CheckAndSetDifficulty(){
        //Get the status on difficulty
        int diffculty_temp = PlayerPrefs.GetInt("Difficulty");

        string local_temp = _text.text;
        
        //Set last set difficulty
        if( local_temp == "EASY" || local_temp == "MEDIUM" || local_temp == "INSANE" ){
            switch(diffculty_temp){
                case 0: _text.text = "EASY"; SetEasyDifficulty();  break;
                case 1: _text.text = "MEDIUM"; SetMediumDifficulty(); break;
                case 2: _text.text = "INSANE"; SetInsaneDifficulty(); break;
            }
        }
    }

    public void CheckAndSetAudio(){
        //Get the status on difficulty
        int sound_temp = PlayerPrefs.GetInt("AudioMute");
        print(sound_temp);
        string local_temp = _text.text;
        print(local_temp);
        //Set last set difficulty
        if( local_temp == "SOUND ON" || local_temp == "SOUND OFF"){
            switch(sound_temp){
                case 0: _text.text = "SOUND OFF"; SoundManager._sharedInstance._isMuted = true; break;
                case 1: _text.text = "SOUND ON"; SoundManager._sharedInstance._isMuted = false;  break;
            }
        }
    }
       public void UnlockLevel(string argName){

        Debug.Log("Unlocked button pressed with argument : "+ argName);

        if(argName == "SNOW"){
            if(PlayerPrefs.GetInt("XP") >= 500){
                PlayerPrefs.SetInt("SNOW", 1);
                PlayerPrefs.SetInt("XP", PlayerPrefs.GetInt("XP") - 500);
                Unlock();
            }
            else
                Debug.Log("Need 500 XP to unlock " + argName);
        }
        else if(argName == "CITY"){
            if(PlayerPrefs.GetInt("XP") >= 1000){
                PlayerPrefs.SetInt("CITY", 1);
                PlayerPrefs.SetInt("XP", PlayerPrefs.GetInt("XP") - 1000);
                this.transform.parent.gameObject.SetActive(false);
                Unlock();
            }
            else
                Debug.Log("Need 1000 XP to unlock " + argName);
        }
        else if(argName == "NIGHT"){
            if(PlayerPrefs.GetInt("XP") >= 2000){
                PlayerPrefs.SetInt("NIGHT", 1);
                PlayerPrefs.SetInt("XP", PlayerPrefs.GetInt("XP") - 2000);
                this.transform.parent.gameObject.SetActive(false);
                Unlock();
            }
            else
                Debug.Log("Need 2000 XP to unlock " + argName);
        }
        else if(argName == "FARM"){
            if(PlayerPrefs.GetInt("XP") >= 3000){
                PlayerPrefs.SetInt("FARM", 1);
                PlayerPrefs.SetInt("XP", PlayerPrefs.GetInt("XP") - 3000);
                this.transform.parent.gameObject.SetActive(false);
                Unlock();
            }
            else
                Debug.Log("Need 3000 XP to unlock " + argName);
        }
    }

     void Unlock(){
        GameObject btn = this.transform.parent.transform.parent.gameObject;
        btn.GetComponentInChildren<Button>().interactable = true;
        this.transform.parent.gameObject.SetActive(false);
        UI_Handler._sharedInstance.UpdateXP();
    }

}