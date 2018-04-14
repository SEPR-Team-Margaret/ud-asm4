using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static AudioClip drawingSound, turnPageSound, turnPageReverseSound, cardSound, cardReverseSound;
	static AudioSource audioSrc;

	// Use this for initialization
	void Start () {

		drawingSound = Resources.Load<AudioClip> ("Sounds/drawing");
		turnPageSound = Resources.Load<AudioClip> ("Sounds/turnPage");
		turnPageReverseSound = Resources.Load<AudioClip> ("Sounds/turnPageReverse");
		cardSound = Resources.Load<AudioClip> ("Sounds/card");
		cardReverseSound = Resources.Load<AudioClip> ("Sounds/cardReverse");

		audioSrc = GetComponent<AudioSource> ();

	}


	/// <summary>
	/// 
	/// plays an audio file using the AudioSource based on the string passed
	/// 
	/// </summary>
	/// /// <param name="sound">Audio clip name.</param>
	public static void PlaySound(string sound){

        try {
            
		    switch (sound) 
		    {
			    case "drawing":
				    audioSrc.PlayOneShot(drawingSound);
			    	break;
		    	case "turnPage":
				    audioSrc.PlayOneShot(turnPageSound);
			    	break;
		    	case "turnPageReverse":
				    audioSrc.PlayOneShot(turnPageReverseSound);
			    	break;
		    	case "card":
				    audioSrc.PlayOneShot(cardSound);
			    	break;
                case "cardReverse":
				    audioSrc.PlayOneShot(cardReverseSound);
			    	break;
		    }

        } catch (System.NullReferenceException e) {
            Debug.Log("Unable to play sound: " + sound);
        }
	}
}
