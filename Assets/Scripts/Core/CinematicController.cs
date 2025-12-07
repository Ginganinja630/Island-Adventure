using UnityEngine;
using UnityEngine.Playables;
using RPG.Utility;

namespace RPG.Core
{
    public class CinematicController : MonoBehaviour 
    {
        PlayableDirector playableDirectorcmp;
        Collider colliderCmp;

        [SerializeField] private bool customPlayOnAwake =false;

        private void Awake()
        {
          playableDirectorcmp = GetComponent<PlayableDirector>();
          colliderCmp = GetComponent<Collider>();  
        }
        private void Start()
        {
            colliderCmp.enabled = !PlayerPrefs.HasKey("SceneIndex");

            if(!customPlayOnAwake) return;

            colliderCmp.enabled =false;
            playableDirectorcmp.Play();

        }
        private void OnEnable()
        {
            playableDirectorcmp.played += HandlePlayed;
            playableDirectorcmp.stopped += HandleStopped;
        }
        
        private void OnDisable()
        {
            playableDirectorcmp.played -= HandlePlayed;
            playableDirectorcmp.stopped -= HandleStopped;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Constants.PLAYER_TAG)) return;

            colliderCmp.enabled = false;
            playableDirectorcmp.Play();
        }
        
        private void HandlePlayed(PlayableDirector pd)
        {
            EventManager.RaiseCutsceneUpdated(false);
        }

        private void HandleStopped(PlayableDirector pd)
        {
            EventManager.RaiseCutsceneUpdated(true);
        }
    }

}

