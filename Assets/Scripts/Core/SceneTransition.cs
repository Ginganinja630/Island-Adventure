using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Utility;

namespace RPG.Core
{
    public static class SceneTransition
    {
        public static IEnumerator Initiate(int sceneIndex)
        {
            AudioSource audioSourceCmp = GameObject.FindGameObjectWithTag(
                Constants.GAME_MANAGER_TAG
            ).GetComponent<AudioSource>();

            float duration = 2f;

            while(audioSourceCmp.volume > 0)
            {
                audioSourceCmp.volume -= Time.deltaTime/duration;

                yield return new WaitForEndOfFrame();
            }

            SceneManager.LoadScene(sceneIndex);
        }
    }
}
