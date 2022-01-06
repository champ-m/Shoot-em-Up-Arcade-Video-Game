using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Handler : MonoBehaviour
{
    //ENUMERATOR FOR LEVEL
    // private enum LEVEL : int{ DESERT, SNOW , CITY, NIGHT, FARM };
    [HideInInspector] public static UI_Handler _sharedInstance;
    [SerializeField] public static bool _PlayFlagMain;
    private bool _hasStartedALevel;


    //GAMPLAY DATA
    [SerializeField] [Range(0.0f, 120.0f)]
    public float _timeToFinish_Seconds = 60.0f;
    private float _timeToFinish_Minutes = 0.0f;
    private bool _timeOver;
    private bool _finalScoreCalculated;

    //Controlling Difficulty for the player

    [SerializeField] [Range(1.0f, 10.0f)] 
    public float _missileSpeedPlayer = 5.0f;
    
    [SerializeField] [Range(1.0f, 10.0f)] 
    public float _missileSpeedEnemy = 5.0f;

    [SerializeField] [Range(0.0f, 10.0f)] 
    public float _enemyMissileFireRate = 10.0f;

    [SerializeField] [Range(0.0f, 10.0f)] 
    public float _playerMissileFireRate = 10.0f;

    //PLAYER SCORE
    static public int _score;
    static public int _postLeft;
    private int _totalScore;

    
    //References for the Menus and Levels 
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _levelSelectionMenu;
    [SerializeField] private GameObject _inGameMenu;
    [SerializeField] private GameObject _inMenuBackground;
    [SerializeField] private GameObject _inGamePauseMenu;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _LevelDesert;
    [SerializeField] private GameObject _LevelSnow;
    [SerializeField] private GameObject _LevelNight;
    [SerializeField] private GameObject _LevelCity;
    [SerializeField] private GameObject _LevelFarm;

//   [SerializeField] private Image[] _AllLevels;
    [SerializeField] private Text _txt_score;
    [SerializeField] private Text _txt_XP;
    [SerializeField] private Text _txt_postLeft;
    [SerializeField] private Text _txt_totalScore;
    [SerializeField] private Text _txt_highestScore;

    /////////////////////////////////////////

    //UI ELEMENTS
    [Header("UI ELEMENTS")]
    [SerializeField] private Text _textField_Score;
    [SerializeField] private Text _textField_Timer;

    void Awake(){
        _sharedInstance = this;
    }

    void DisableDebugLog(){
        Debug.unityLogger.logEnabled = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        //Disable Console Log
        DisableDebugLog();

        // checkLevelState();
        Debug.Log("Start of UI Handler is called");

        //Check if the game is restarted
        CheckRestartStatus();
        
        //Will go to selection menu after buying
        // CheckAndSetSelectionMenu();


        //Set the camera for the canvas
        this.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

        _score = 0;
        _totalScore = 0;
        _timeOver = false;
        _PlayFlagMain = false;
        _finalScoreCalculated  = false;
    }

    // Update is called once per frame
    void Update()
    {   
        //Update the textfield for score
        UpdateScoreInTextField();

        //Check if no more posts are left
        if( _postLeft == 0 ){
            _PlayFlagMain = false;
        }

        //Update time if time hasn't run out and main flag is still true
        if(!_timeOver && _PlayFlagMain){
            UpdateTime();
            UpdateTimeInTextField();
        }

        //set main flag to false when time run out
        if(_timeOver ){
            _PlayFlagMain = false;
            // if(ObjectPoolingEnemy._sharedInstance)
            //     ObjectPoolingEnemy._sharedInstance.DeactivateAllPooledObject();
            // CalculateFinalScore();
            // UpdateScoreInTextField();
        }

        //check if game stopped playing, pause menu isn't active,
        //final score isn't calculated and player atleast started a level
        if( !_PlayFlagMain && _hasStartedALevel && !_finalScoreCalculated && !_inGamePauseMenu.activeInHierarchy){

            if(ObjectPoolingEnemy._sharedInstance)
                ObjectPoolingEnemy._sharedInstance.DeactivateAllPooledObject();
            CalculateFinalScore();
            UpdateScoreInTextField();
            //Get the GameOver Menu active;
            SetGameOverMenu();
        }
        
    }

    void CalculateFinalScore(){
        if(_postLeft == 0)
            _totalScore = _score;
        else
            _totalScore = _score * _postLeft;
       
        if(_totalScore > PlayerPrefs.GetInt("h_score", 0)){
            PlayerPrefs.SetInt("h_score", _totalScore);
            PlayerPrefs.Save();
        }

        //Set XP for the player
        //Add up existing XP with the score
        PlayerPrefs.SetInt("XP", (PlayerPrefs.GetInt("XP", 0) + _score));
        PlayerPrefs.Save();

        _finalScoreCalculated = true;

     //Get the GameOver Menu active;
        SetGameOverMenu();
    }

    void UpdateScoreInTextField(){
        _textField_Score.text = "Score  "+ _score + "\t\t\tPost Left : " + _postLeft;
    }
    void UpdateTime(){

        if(_timeToFinish_Seconds >= 60){
            _timeToFinish_Minutes ++;
            _timeToFinish_Seconds -= 60;
        }
        else if(_timeToFinish_Seconds <= 0 && _timeToFinish_Minutes > 0){
            _timeToFinish_Minutes --;
            _timeToFinish_Seconds += 60;
        }
        else if(_timeToFinish_Minutes == 0 && (int)_timeToFinish_Seconds == 0)
            _timeOver = true;
        _timeToFinish_Seconds -= Time.deltaTime;
    }
    void UpdateTimeInTextField(){

        if(_timeToFinish_Seconds < 10){
            _textField_Timer.text = "0" + (int)_timeToFinish_Minutes +  " : 0" + (int)_timeToFinish_Seconds;
        }
        else{
            _textField_Timer.text = "0" + (int)_timeToFinish_Minutes +  " : " + (int)_timeToFinish_Seconds;
        }
    }

    void CheckRestartStatus(){
        
        //check if the player restarted a level
        int restartFlag = PlayerPrefs.GetInt("Restarted", 0);

        if(restartFlag == 1){
            
            //Deactivate the MainMenu
            _mainMenu.SetActive(false);
            //Deactivate the menuBackground
            _inMenuBackground.SetActive(false);

            //Play the last level playing
            RestartLastLevel();
        }
        else{
            _inMenuBackground.SetActive(true);
            _hasStartedALevel = false;
            _mainMenu.SetActive(true);
            SetInMenuTextColor("ORANGE");
        }
    }


    //GAMEOVER menu
    //
    public void SetGameOverMenu(){
        _inGameMenu.SetActive(false);
        _gameOverMenu.SetActive(true);
        _txt_score.text = _score + "";
        _txt_postLeft.text = _postLeft + "";
        _txt_totalScore.text = _totalScore + "";
        _txt_highestScore.text = PlayerPrefs.GetInt("h_score", 0) + "";
        
    }
    public void SetHighestScore(int argScore){
        if( argScore > PlayerPrefs.GetInt("h_score", 0))
            PlayerPrefs.SetInt("h_score", argScore);
    }

    //MAIN MENU BUTTONS

    //When User Pressed Play
    public void _InMenusPressedPlay(){

        //Deactivate the main menu
        _mainMenu.SetActive(false);

        //Activate the level selection
        _levelSelectionMenu.SetActive(true);

        //Check level locked and unlocked status
        CheckAndSetLevelLockStatus();

        //Update XP textfield
        UpdateXP();
    }
    
    public void UpdateXP(){
        //Set text for the XP
        if(_txt_XP != null)       
            _txt_XP.text = "XP " + PlayerPrefs.GetInt("XP", 0);    
    }


    // void CheckAndSetSelectionMenu(){
    //     if(PlayerPrefs.GetInt("levelView", 0) == 1){
    //         _InMenusPressedPlay();
    //     }
    // }
 
    // void ReloadSelectionMenu(){
    //     PlayerPrefs.SetInt("levelView", 1);
    //     InGamePressedMainMenu();
    // }

    void CheckAndSetLevelLockStatus(){

        //Find all the Level View
        GameObject[] LevelView = GameObject.FindGameObjectsWithTag("LevelView");
        
        int no_of_levels = LevelView.Length;

        if(no_of_levels != 0){
            
            string levelnameLocal = "SNOW";
             
            for(int i=1 ; i<no_of_levels; i++){
                //Set the level name
                if(i == 2) levelnameLocal = "CITY";
                else if(i == 3) levelnameLocal = "NIGHT";
                else if(i == 4) levelnameLocal = "FARM";

                //Find the unlocked tagged components
                GameObject _unlocked = FindGameObjectInChildWithTag(LevelView[i].gameObject, "unlocked");

                //Find the locked tagged components
                GameObject _locked = FindGameObjectInChildWithTag(LevelView[i].gameObject, "locked");

                if(PlayerPrefs.GetInt(levelnameLocal, 0) == 0 && _unlocked != null)
                    _unlocked.GetComponentInChildren<Button>().interactable = false;
                
                else if(PlayerPrefs.GetInt(levelnameLocal, 0) == 1 && _locked != null)
                    _locked.gameObject.SetActive(false);
            }
        }
    }

     public static GameObject FindGameObjectInChildWithTag (GameObject parent, string argTag)
     {
        //Get the transform of the parent gameobject
        Transform _transfrom = parent.transform;

        //if the tag you are looking for is locked leave the first child
        //if the tag you are looking for is unlocked start from the very first child
        int i = (argTag == "locked")?  1 : 0;

        //iterrate through the list of child and return one with matching tag
        for ( ; i < _transfrom.childCount; i++) {
            if(_transfrom.GetChild(i).gameObject.tag == argTag)
            {
                //Return the chile gameobject
                return _transfrom.GetChild(i).gameObject;
            }
        }
        //Return null
        return null;
    }


    //When User Pressed Sound
    //There is also another call made by this button see inspector
    public void _InMenusPressedSound(){
        
        if(PlayerPrefs.GetInt("AudioMute", 0) == 0){
            PlayerPrefs.SetInt("AudioMute", 1);
            PlayerPrefs.Save();
            SoundManager._sharedInstance._isMuted = false;
        }
        else{
            PlayerPrefs.SetInt("AudioMute", 0);
            PlayerPrefs.Save();
            SoundManager._sharedInstance._isMuted = true;
        }
    }

    //When User Pressed Quit
    public void _InMenusPressedQuit(){
        Application.Quit();
    }

    //LEVEL SELECTION MENU BUTTONS
    //
    public void _InMenusPressedBack(){
        _levelSelectionMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    private void GetLevelReady()
    {
        _inMenuBackground.SetActive(false);
        _levelSelectionMenu.SetActive(false);
        _inGameMenu.SetActive(true);
        _PlayFlagMain = true;
    }

    //IN GAME PAUSE MENU
    public void InGamePressedPause(){
        

        _inGamePauseMenu.SetActive(!_inGamePauseMenu.activeInHierarchy);
        
        if(_inGamePauseMenu.activeInHierarchy)
            Time.timeScale = 0.0f;
        else 
            Time.timeScale = 1.0f;

    }
    public void InGamePressedMainMenu(){      

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        
        Time.timeScale = 1.0f;

    }
    public void InGamePressedRestart(){      

        //Destroy Existing missiles for Enemy
        ObjectPoolingEnemy._sharedInstance.DeactivateAllPooledObject();
        //Destroy Existing missiles for Player
        ObjectPoolingPlayer._sharedInstance.DeactivateAllPooledObject();

        //Find the active level in heirarchy
        GameObject activeLevel = GameObject.FindGameObjectWithTag("Level");
        
        Debug.Log("Name for the level loaded is : " + activeLevel.name);

        //Save the name for the level using playerPrefs
        PlayerPrefs.SetString("LastPlayedLevel", activeLevel.name);
        PlayerPrefs.SetInt("Restarted", 1);
        PlayerPrefs.Save();

        Debug.Log("Reloading...");

        //Restart the scene
        InGamePressedMainMenu();
    }

    void RestartLastLevel(){
        //Get the name for the last loaded level
        string lastLevel = PlayerPrefs.GetString("LastPlayedLevel");

        Debug.Log("Level name was : "+ lastLevel);
        
        //Restart the respective level
        switch(lastLevel){
            case "LevelDesert": _Loadlevel("DESERT"); break;
            case "LevelSnow": _Loadlevel("SNOW"); break;
            case "LevelNight": _Loadlevel("NIGHT"); break;
            case "LevelCity": _Loadlevel("CITY"); break;
            case "LevelFarm": _Loadlevel("FARM"); break;
        }
        Debug.Log("Level loaded");
        
        //Set the last Audio and difficulty
        SetDifficltyWhenRestarting();

        PlayerPrefs.SetInt("Restarted", 0);
        PlayerPrefs.SetString("LastPlayedLevel", "");
        PlayerPrefs.Save();

    }

    void SetDifficltyWhenRestarting(){

        switch(PlayerPrefs.GetInt("Difficulty", 0)){
            case 0: ChangeButtonText.SetEasyDifficulty();  break;
            case 1: ChangeButtonText.SetMediumDifficulty(); break;
            case 2: ChangeButtonText.SetInsaneDifficulty(); break;
        }
        
    }
    public void _Loadlevel(string argLevel)
    {
        GetLevelReady();

        switch (argLevel) {
            case "DESERT":
                SetInMenuTextColor("ORANGE");
                _LevelDesert.SetActive(true);
                _hasStartedALevel = true;
                break;
            case "SNOW":
                SetInMenuTextColor("BLUE");
                _LevelSnow.SetActive(true);
                _hasStartedALevel = true;
                break;
            case "NIGHT":
                SetInMenuTextColor("ORANGE");
                _LevelNight.SetActive(true);
                _hasStartedALevel = true;
                break;
            case "CITY":
                SetInMenuTextColor("PURPLE");
                _LevelCity.SetActive(true);
                _hasStartedALevel = true;
                break;
            case "FARM":
                SetInMenuTextColor("BROWN");
                _LevelFarm.SetActive(true);
                _hasStartedALevel = true;
                break;
        }
    }
    // public void checkLevelState(){
    //     PlayerPrefs.SetInt("level0",1);
    //     for(int i=0; i<_AllLevels.Length;i++){
    //         string levelName = "level"+i;
    //         int levelStat = PlayerPrefs.GetInt(levelName, 0);
    //         if(levelStat==0 || levelStat == null){
    //             _AllLevels[i].GetComponent<Button>().interactable =false;
    //         }
    //     }
    //     PlayerPrefs.Save();
    // }
    void SetInMenuTextColor(string argColor)
    {
        switch (argColor) {
            case "BROWN":
                _textField_Score.color = new Color(0.4235f, 0.2275f, 0.1176f);
                _textField_Timer.color = new Color(0.4235f, 0.2275f, 0.1176f);
                break;
            case "BLUE":
                _textField_Score.color = new Color(0.0353f, 0.1921f, 0.4745f);
                _textField_Timer.color = new Color(0.0353f, 0.1921f, 0.4745f);
                break;
            case "ORANGE":
                _textField_Score.color = new Color(0.9647f, 0.5725f, 0.1176f);
                _textField_Timer.color = new Color(0.9647f, 0.5725f, 0.1176f);
                break;
            case "PURPLE":
                _textField_Score.color = new Color(0.4275f, 0.2000f, 0.4275f);
                _textField_Timer.color = new Color(0.4275f, 0.2000f, 0.4275f);
                break;
        }
    }
}