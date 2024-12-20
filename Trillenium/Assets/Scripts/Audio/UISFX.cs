using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISFX : MonoBehaviour
{

    //REFERENCES//

    //SFX AudioSource where SFX plays from
    public AudioSource sfxSource;

    //The AudioManager script which holds all the SFX audio to play during gameplay.
    public AudioManager audioManager;


    void Start()
    {
        sfxSource = GameObject.Find("SFXSource").GetComponent<AudioSource>();
        audioManager = sfxSource.GetComponent<AudioManager>();
    }
    //Called on PointerEnter EventTrigger
    //Play UIHoverSFX when hovering over a UI Button in a Menu.

    public void PlayHover()
    {
        sfxSource.PlayOneShot(audioManager.uiHover);
    }

    //Called on the PointerDown EventTrigger
    //Plays the Click SFX when clicking a Button in a Menu.
    public void PlayClick()
    {
        sfxSource.PlayOneShot(audioManager.uiClick);
    }

    //Plays the SFX for closing out of a menu
    public void PlayCancel()
    {
        sfxSource.PlayOneShot(audioManager.uiClose);
    }
}
