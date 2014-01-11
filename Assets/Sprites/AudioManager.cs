using UnityEngine;
using System.Collections;

/// <summary>
/// Filename: AudioManager.cs
/// Description: Manager of ground music
/// Author: Star
/// Date: [12/4/31]
/// </summary>
public class AudioManager : MonoBehaviour {
    
    private static AudioManager instance = null;
    
    private static AudioSource m_AudioMgr;
        
    private AudioClip m_PlayClip;
    private string m_CurMusicName = "";
    
    public static AudioManager Instantiate()
    {

		if(instance == null){
			GameObject obj = new GameObject();
			obj.name = "MusicPlayer";
			
			instance = obj.AddComponent<AudioManager>();
		
			m_AudioMgr = obj.AddComponent<AudioSource>();
		}
		
        return instance;
    }
    
	public AudioClip LoadLocal(string audioUrl) {
//		int id = audioUrl.GetHashCode();
//		if (_audioClips.ContainsKey(id)) {
//			return id;
//		}

		AudioClip audio = Resources.Load(audioUrl) as AudioClip;
//		_audioClips.Add(id, audio);
//		_audioUrls.Add(audioUrl);
//		_audioSrc.clip = audio;
		return audio;
	}
	
    /// <summary>
    /// Play the background music which will go through the scene.
    /// </summary>
    /// <param name='fileName'>
    /// File name.
    /// </param>
    public void  PlayBG(string fileName)
    {
      
			if (!fileName.Equals(m_CurMusicName))
	        {
	            m_PlayClip = LoadLocal(fileName);
	            m_AudioMgr.clip = m_PlayClip;
				m_AudioMgr.loop = true;
//			 	if(PlayerPrefs.GetInt(IPrefsKey.Key_Music) == IConst.OPEN){
//					m_AudioMgr.Play();
//				}
       	    	m_CurMusicName = fileName;
	        }
    }
	
	public void ContinueBG(){
		m_AudioMgr.Play();
	}
    
    /// <summary>
    /// Play the background music which will go through the scene.
    /// </summary>
    /// <param name='m_PlayClip'>
    /// M_ play clip.
    /// </param>
    public void PlayBG(AudioClip m_PlayClip)
    {
        m_AudioMgr.clip = m_PlayClip;
        m_AudioMgr.Play();
    }
    
    /// <summary>
    /// Stops the background music.
    /// </summary>
    public void StopBG()
    {
        m_AudioMgr.Stop();
        m_CurMusicName = "";
        //Debug.Log("Stop bm_PlayClipkground music");
    }
    
//    public AudioSource Play(AudioClip clip, Transform emitter, bool loop)
//    {
//        return Play(clip, emitter, 1f, 1f, loop);
//    }
//    
//    public AudioSource Play(AudioClip clip, Transform emitter, float volume, bool loop)
//    {
//        return Play(clip, emitter, volume, 1f, loop);
//    }
//    
//    /// <summary>
//    /// Plays a sound by creating an empty game object with an AudioSource
//    /// and attaching it to the given transform (so it moves with the transform). 
//    /// Destroys it after it finished playing if it dosen't loop.
//    /// </summary>
//    /// <param name='clip'>
//    /// Clip.
//    /// </param>
//    /// <param name='emitter'>
//    /// Emitter.
//    /// </param>
//    /// <param name='volume'>
//    /// Volume.
//    /// </param>
//    /// <param name='pitch'>
//    /// Pitch.
//    /// </param>
//    /// <param name='loop'>
//    /// Loop.
//    /// </param>
//    public AudioSource Play(AudioClip clip, Transform emitter, float volume, float pitch, bool loop)
//    {
//        GameObject go = new GameObject("Audio:"+clip.name);
//        go.transform.position = emitter.position;
//        go.transform.parent = emitter;
//        
//        // create the source
//        AudioSource source = go.AddComponent<AudioSource>();
//        source.clip = clip;
//        source.volume = volume;
//        source.pitch = pitch;
//        source.loop = loop;
//        Debug.Log("clip.length:"+clip.length);
//        if (!loop)
//        {
//            Destroy(go, clip.length);
//        }
//        
//        return source;
//    }
	public AudioSource Play(string fileName, bool loop)
    {
        
		return Play(LoadLocal(fileName), Vector3.zero, 1f, 1f, loop);
		
    }
    
    public AudioSource Play(AudioClip clip, Vector3 point, bool loop)
    {
        return Play(clip, point, 1f, 1f, loop);
    }
    
    public AudioSource Play(AudioClip clip, Vector3 point, float volume, bool loop)
    {
        return Play(clip, point, volume, 1f, loop);
    }
    
    /// <summary>
    /// Plays a sound at the given point in space by creating an empty game object with an AudioSource
    /// in that place and destroys it after it finished playing if it dosen't loop.
    /// </summary>
    /// <param name='clip'>
    /// Clip.
    /// </param>
    /// <param name='point'>
    /// Point.
    /// </param>
    /// <param name='volume'>
    /// Volume.
    /// </param>
    /// <param name='pitch'>
    /// Pitch.
    /// </param>
    /// <param name='loop'>
    /// Loop.
    /// </param>
    public AudioSource Play(AudioClip clip, Vector3 point, float volume, float pitch, bool loop)
    {
//		if(PlayerPrefs.GetInt(IPrefsKey.Key_Sound) == IConst.CLOSE){
//			return null;
//		}
        GameObject go = new GameObject("Audio:" + clip.name);
        go.transform.position = point;
        
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
		
		source.Play();
       
        if (!loop)
        {
            DestroyObject(go, clip.length);
        }
        
        return source;
    }
}