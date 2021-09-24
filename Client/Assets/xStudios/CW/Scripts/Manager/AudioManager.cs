using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using XLua;
using X;


namespace X
{
    [LuaCallCSharp]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField]
        AudioSource audioSource;

        bool isStart = false;
        float time = 0.0f;
        float volumeDelay = 0.0f;


        public void ClearMusic()
        {
            StopMusic();
        }

        public void PlaySound( string path )
        {
            if ( !Setting.EnabledSound )
            {
                return;
            }

            Utility.LoadAudio( this , path , AudioType.WAV , OnLoadSound );
        }

        void OnLoadSound( AudioClip ac , bool err )
        {
            if ( err )
            {
                return;
            }

            audioSource.PlayOneShot( ac , Setting.SoundVolume );
        }


        public void MusicVolume( float v )
        {
            audioSource.volume = v;
        }

        public void PlayMusic( string path )
        {
            if ( !Setting.EnabledMusic )
            {
                return;
            }

            audioSource.Stop();

            Utility.LoadAudio( this , path , AudioType.OGGVORBIS , OnLoadMusic );
        }

        void OnLoadMusic( AudioClip ac , bool err )
        {
            if ( err )
            {
                return;
            }

            if ( audioSource.clip != null )
            {
                Destroy( audioSource.clip );
                audioSource.clip = null;
            }

            audioSource.clip = ac;
            audioSource.loop = true;
            audioSource.volume = Setting.MusicVolume;
            audioSource.Play();

            isStart = false;
        }

        public void StopMusic( float d = 0.5f )
        {
            if ( d <= 0f )
            {
                audioSource.Stop();
            }
            else
            {
                time = d;
                volumeDelay = Setting.MusicVolume / time;
                isStart = true;
            }
        }

        public void PauseMusic( bool b )
        {
            if ( b )
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.Play();
            }
        }

        void Update()
        {
            if ( !isStart )
            {
                return;
            }

            time -= Time.deltaTime;

            if ( time <= 0f )
            {
                isStart = false;
                audioSource.Stop();
            }
            else
            {
                audioSource.volume = volumeDelay * time;
            }
        }

    }

}

