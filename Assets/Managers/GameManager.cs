using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*ALL OF THIS MAY HAVE TO BE REWRITTEN LATER IN DEVELOPMENT TO BETTER HANDLE THE PROCESS AS SINGLETONS AND ENUMS CAN BE QUITE MESSY ON LARGE SCALE PROJECTS*/
public class GameManager{
    public static GameManager Instance;

    public GameState State;

    public static event System.Action<GameState> OnGameStateChanged;

//just makes this a public instance so other scripts can see it
    void Awake(){
        Instance = this;
    }

    //need initial state dictated here
    void Start(){
        //commented out to not cause any direct issues, this is the format for changing game states
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState) {
        State = newState;

        switch (newState){
            case GameState.DayCity:
                break;
            /*if you want a function to be called on gamestate change, underneath put a name of function like so: HandleDayNightTransition(); 
            this function can be called to outside of this script same way as when invoking a repeating function*/
            case GameState.DayNightTransition:
                break;
            case GameState.NightDayTransition:
                break;
            case GameState.NightRig:
                break;
            case GameState.Minigame:
                break;
            case GameState.MainMenu:
                break;
            case GameState.DeathScren:
                break;
            case GameState.GameOver:
                break;
            case GameState.Victory:
                break;
        }
        //calls for the OnGameStatechanged event to register when the game state is changed
        OnGameStateChanged(newState);
    }
}
//More gamestates will have to be added as the game is further developed
public enum GameState {
    //daytime city builder gamestate
    DayCity,
    //transition between daytime city building to night time (loading screens, cutscenes, etc)
    DayNightTransition,
    //transition between nighttime oil rig exploration and daytime city building (registering impact of night, cutscenes loading, switching game scenes)
    NightDayTransition,
    //nighttime oil rig exploration gamestate
    NightRig,
    //Minigames for during the night time fixing/checking on different machines
    //specific gamestates for each minigame will have to be added
    Minigame,
    //Main menu before game is started up (controls, play, quit, information, etc)
    MainMenu,
    //death screen functions as retry current night or quit
    DeathScren,
    //Game Over only happens when nights are failed so many times or enough damage to rig has been done (needs ironing out)
    GameOver,
    //Gamestate dedicated to if the player beat the game (would need different game states for each ending so other scripts can call to this script for when they need to act)
    //ex. bad ending, good enging, neutral ending, etc.
    Victory
}