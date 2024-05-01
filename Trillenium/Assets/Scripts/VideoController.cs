using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{
    #region Variables
    public GameObject mainCamera; // Main camera. YOU HAVE TO MANUALLY SET THIS.
    public VideoPlayer videoPlayer; // Video player attached to camera. Script should automatically set this.
    public VideoPlayer player; // To access pausing capabilities. Script should automatically set this.
    int videoState; // Controls whether or not the game is paused (0 = paused, 1 = unpaused) and is controlled with the space bar;
    public bool gamePaused; // Just for personal extra readability;
    bool end; //Boolean used to disable pausing when end is reached (ultimately just to avoid any potential bugs);

    public InputActionAsset controls;
    public InputActionReference pauseCutscene;
    public InputActionReference skipCutscene;
    public GameObject blackSquare;
    #endregion

    void Awake()
    {
        videoPlayer = mainCamera.GetComponent<VideoPlayer>();
        // Play on awake defaults to true. Set it to false to avoid the url set
        // below to auto-start playback since we're in Start().
        videoPlayer.playOnAwake = false;

        // By default, VideoPlayers added to a camera will use the far plane.
        // Let's target the near plane instead.

        //Uncommenting this disables video play
        //videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        // This will cause our Scene to be completely opaque.
        videoPlayer.targetCameraAlpha = 1;

        // Video will NOT loop.
        videoPlayer.isLooping = false;

        // For detecting when the video has reached the end.
        videoPlayer.loopPointReached += EndReached;

        //Enables New Input System inputs for pausing and skipping cutscenes 
        OnEnable();
    }

    void Start()
    {
        // The video is playing.
        videoState = 1;

        // Is the game paused? Different from the video itself being over.
        // Since the scene is starting, the game should NOT be paused yet.
        gamePaused = false;

        end = false;

        player = mainCamera.GetComponent<VideoPlayer>();

        // Loads video into video player and plays it.
       // videoPlayer.url = "ENTER VIDEO URL HERE";
        videoPlayer.Play();
    }

    //Enable input for pausing or skipping the cutscene while it's playing.
    private void OnEnable()
    {
        pauseCutscene.action.performed += VideoPause;
        pauseCutscene.action.Enable();

        skipCutscene.action.performed += SkipVideo;
        skipCutscene.action.Enable();
    }

    private void OnDisable()
    {
        pauseCutscene.action.performed -= VideoPause;
        pauseCutscene.action.Disable();
        skipCutscene.action.performed -= SkipVideo;
        skipCutscene.action.Disable();
    }

    void Update()
    {
        // Controller for pausing the game.
        // if (Input.GetKeyDown(KeyCode.Space) && end == false)
        // {
        //     VideoPause();
        // }
    }

    //After a cutscene's done, load the next scene in the build index after 1 second.
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        Debug.Log("VIDEO IS OVER");
        end = true;
        // *****INSERT CODE FOR TRANSITIONING INTO NEXT SCENE*****
        //Invoke("LoadNextScene", 1f);
        FadeToBlack();
    }

    void VideoPause(InputAction.CallbackContext context)
    {
        if(end == false)
        {
                if (videoState > 0)
                {
                    player.Pause();
                    gamePaused = true;
                    // The video is paused.
                    videoState = 0;
                }
                else if (gamePaused == true)
                {
                    player.Play();
                    gamePaused = false;
                    // The video is playing.
                    videoState = 1;
                }
        }
        else if(end == true)
        {
            Debug.Log("CAN'T PAUSE NOW");
            gamePaused = false;
        }
    }

    //Fades screen to black after cutscene ends OR after skipping cutscene
    void FadeToBlack()
    {
        blackSquare.SetActive(true);
        blackSquare.GetComponent<Animator>().Play("FadeToBlack");
        Invoke("LoadNextScene", 2f);
    }

    void SkipVideo(InputAction.CallbackContext context)
    {
        if(end == false)
        {
                    player.Pause();
                    gamePaused = true;
                    // The video is paused.
                    videoState = 0;
                    FadeToBlack();
                    //LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
