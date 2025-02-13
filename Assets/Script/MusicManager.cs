using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    DataManager dataController;
    [SerializeField] AudioSource music;
    [SerializeField] AudioSource soundOther, soundBtnClick, soundLevel, soundLevelLoop;
    [SerializeField] AudioClip[] lstBgGPCl,lstBgHomeCl;
    [SerializeField] AudioClip[] lstSoundOtherCl, lstSoundLevelCl;
    [SerializeField] AudioClip[] lstSoundBtn;

    public int GetLengthBGHomeCl()
    {
        return lstBgHomeCl.Length;
    }    

    public int RandomBGGP()
    {
        return Random.Range(0, lstBgGPCl.Length);
    }    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dataController = DataManager.instance;
        ChangeSettingSound();
        ChangeSettingMusic();

        //   Debug.LogError("=========== start music");
    }
    public void MuteAllSound()
    {
        //    soundBtnClick.volume = soundLoopObject.volume = soundOther.volume = 0;
        soundLevelLoop.mute = soundLevel.mute = soundBtnClick.mute = soundOther.mute = true;
    }
    public void MuteAllMusic()
    {
        //  music.volume = 0;
        music.mute = true;
    }
    public void ChangeSettingMusic()
    {
        music.mute = dataController.SaveData().offmusic;

        //  music.volume = (float)_datacontrol.SaveData().musicVolume;
    }
    public void ChangeSettingSound()
    {
        soundLevelLoop.mute = soundLevel.mute = soundBtnClick.mute = soundOther.mute = dataController.SaveData().offsound;
        //soundBtnClick.volume = soundLoopObject.volume = soundOther.volume = (float)_datacontrol.SaveData().soundVolume;
    }
    public void SoundClickButton(int indexSoundBtn = 0)
    {
        soundBtnClick.PlayOneShot(lstSoundBtn[indexSoundBtn]);
        if (EventSystem.current.currentSelectedGameObject != null)
            EventManager.FOLLOW_BUTTON_EVENT(EventSystem.current.currentSelectedGameObject.name, Application.loadedLevelName);
    }
    public void PlaySoundBGHome(bool play, int index, float volume = 1f)
    {

        if (play)
        {
            if (index < 0 || index >= lstBgHomeCl.Length || music.clip == lstBgHomeCl[index])
                return;

            music.volume = volume;
            music.clip = lstBgHomeCl[index];
            music.Play();
        }
        else
        {
            music.Stop();
        }
    }
    public void PlaySoundBGGP(bool play, int index, float volume = 1f)
    {

        if (play)
        {
            if (index < 0 || index >= lstBgGPCl.Length || music.clip == lstBgGPCl[index])
                return;

            music.volume = volume;
            music.clip = lstBgGPCl[index];
            music.Play();
        }
        else
        {
            music.Stop();
        }
    }
    public void PlaySoundOtherOneShot(bool play, int index/*, float volume = 1*/)
    {
        if (play)
        {
            if (index < 0 || index >= lstSoundOtherCl.Length)
                return;
            soundOther.clip = lstSoundOtherCl[index];
            soundOther.PlayOneShot(soundOther.clip/*, volume*/);
        }
        else
        {
            soundOther.Stop();
        }
    }
    public void PlaySoundLevelOneShot(bool play, int index)
    {

        if (play)
        {
            if (index < 0 || index >= lstSoundLevelCl.Length)
                return;
            soundLevel.clip = lstSoundLevelCl[index];
            soundLevel.PlayOneShot(soundLevel.clip/*, volume*/);
        }
        else
        {
            soundLevel.Stop();
        }
    }
    public void PlaySoundLevelLoop(bool play, int index)
    {
        if (play)
        {
            if (index < 0 || index >= lstSoundLevelCl.Length)
                return;
            if (!soundLevelLoop.isPlaying || soundLevelLoop.clip != lstSoundLevelCl[index])
            {
                soundLevelLoop.clip = lstSoundLevelCl[index];
                soundLevelLoop.Play();
            }
        }
        else
        {
            if (soundLevelLoop.isPlaying)
                soundLevelLoop.Stop();
          //  Debug.LogError("============== stop sound level loop");
        }
    }
}
