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
    public class VideoManager : Singleton<VideoManager>
    {
        [SerializeField]
        RawImage rawImage;
        [SerializeField]
        VideoPlayer videoPlayer;
        [SerializeField]
        Text text;

        public delegate void OnPlayOver();
        OnPlayOver onPlayOver = null;

        bool isPlaying;
        public bool IsPlaying { get { return isPlaying; } }

        protected override void InitSingleton()
        {
            videoPlayer.loopPointReached += PlayOver;

            rawImage.enabled = false;

            text.gameObject.SetActive( false );

            gameObject.SetActive( false );
        }

        public void SetText( string str )
        {
            text.text = str;
        }

        public void PlayCenter( string url , OnPlayOver over )
        {
            RectTransform trans = GetComponent<RectTransform>();
            trans.anchoredPosition = new Vector2( 0.0f , 0.0f );

            gameObject.SetActive( true );

            videoPlayer.Stop();
            videoPlayer.url = url;
            videoPlayer.Play();

            rawImage.enabled = true;

            isPlaying = true;

#if UNITY_EDITOR
            Debug.Log( "playMovieCenter " + url );
#endif
        }

        public void Play( string url , OnPlayOver over )
        {
            RectTransform trans = GetComponent<RectTransform>();

            onPlayOver = over;

            gameObject.SetActive( true );

            videoPlayer.Stop();
            videoPlayer.url = url;
            videoPlayer.Play();

            rawImage.enabled = true;

            isPlaying = true;

#if UNITY_EDITOR
            Debug.Log( "playMovie " + url );
#endif
        }

        public void OnClick()
        {
            if ( text.gameObject.activeSelf )
            {
                StopMovie();
            }
            else
            {
                text.gameObject.SetActive( true );
            }
        }

        public void StopMovie()
        {
            isPlaying = false;

            videoPlayer.Stop();
            rawImage.enabled = false;
            text.gameObject.SetActive( false );

            gameObject.SetActive( false );

            videoPlayer.targetTexture.Release();

            if ( onPlayOver != null )
            {
                onPlayOver();
            }
        }

        public void PlayOver( VideoPlayer p )
        {
#if UNITY_EDITOR
            Debug.Log( "onPlayOver" );
#endif
            StopMovie();
        }



    }

}
