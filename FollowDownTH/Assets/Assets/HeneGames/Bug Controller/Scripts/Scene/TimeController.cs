using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.BugController
{
    public class TimeController : MonoBehaviour
    {
        private bool sloMo;
        private float timeScale = 1f;
        private float desiredTimeScale = 1f;

        void Update()
        {
            TimeSystem();

            if(Input.GetKeyDown(KeyCode.T))
            {
                sloMo = !sloMo;
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void TimeSystem()
        {
            Time.timeScale = timeScale;
            timeScale = Mathf.Lerp(timeScale, desiredTimeScale, 0.1f);

            if (sloMo)
            {
                desiredTimeScale = 0.4f;
            }
            else
            {
                desiredTimeScale = 1f;
            }
        }
    }
}