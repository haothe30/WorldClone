using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataParamManager
{
    public static bool ResumeFromAds = false, ResumeFromOtherAction = false, playingAnim = false, dragging, activeCountPlay, isTuroring = false;
    public static int indexPackIAP;
    public static float timeDelayShowAds = 60, timeCollapsedBannerFL = 15f, timeDelayLoadNative = 15f;
    public static string packBuyIAP, idNativeAds;
    public static System.DateTime beginShowInter, timePlaying;
    public const string SAVEDATA = "savedata";
    public static Action displayTicket, displayRemoveAds;
    public static float percentTakePicture, percentDown;
    public enum STATEGAMEPLAY
    {
        BEGIN, PLAY, RESULT, TIMEOUT, PAUSE
    }
    public enum TYPERESOURCES
    {
        VIDEO, IAP
    }
    public static STATEGAMEPLAY state = STATEGAMEPLAY.BEGIN;

    static WaitForSeconds time0_0_5s = new WaitForSeconds(0.05f);
    static WaitForSeconds time0_1s = new WaitForSeconds(0.1f);
    static WaitForSeconds time1s = new WaitForSeconds(1f);
    static WaitForSeconds time0_5s = new WaitForSeconds(0.5f);
    static WaitForSeconds time1_5s = new WaitForSeconds(1.5f);
    static WaitForSeconds time2s = new WaitForSeconds(2f);
    static WaitForSeconds time0_2s = new WaitForSeconds(0.2f);
    static WaitForSeconds time5s = new WaitForSeconds(5f);
    static WaitForSeconds timeCollapsedBanner = new WaitForSeconds(timeCollapsedBannerFL);
    static WaitForSeconds timeNativeRefresh = new WaitForSeconds(timeDelayLoadNative);
    public static void SetTimeCollapsedBanner()
    {
        timeCollapsedBanner = new WaitForSeconds(timeCollapsedBannerFL);
        //  Debug.LogError("================ set time collapsed banner:" + timeCollapsedBanner);
    }
    public static void SetTimeRefreshNative()
    {
        timeNativeRefresh = new WaitForSeconds(timeDelayLoadNative);
        //  Debug.LogError("================ set time collapsed banner:" + timeCollapsedBanner);
    }
    public static WaitForSeconds TIMECOLLAPSEDBANNER()
    {
        return timeCollapsedBanner;
    }
    public static WaitForSeconds TIMEREFRESHNATIVE()
    {
        return timeNativeRefresh;
    }
    public static WaitForSeconds GETTIME5S()
    {
        return time5s;
    }
    public static WaitForSeconds GETTIME0_0_5S()
    {
        return time0_0_5s;
    }
    public static WaitForSeconds GETTIME0_1S()
    {
        return time0_1s;
    }
    public static WaitForSeconds GETTIME0_2S()
    {
        return time0_2s;
    }
    public static WaitForSeconds GETTIME0_5S()
    {
        return time0_5s;
    }
    public static WaitForSeconds GETTIME1S()
    {
        return time1s;
    }
    public static WaitForSeconds GETTIME1_5S()
    {
        return time1_5s;
    }
    public static WaitForSeconds GETTIME2S()
    {
        return time2s;
    }

    static string result = "";
    public static string ConverTimeToString(TimeSpan timeSpan, bool keepHours)
    {
        result = "";
        // Xây dựng chuỗi từ các thành phần của TimeSpan

        // Thêm ngày nếu có
        if (timeSpan.Days > 0)
        {
            result += $"{timeSpan.Days:00}:";
        }

        // Thêm giờ nếu có hoặc nếu không có ngày và giờ > 0 và keepHours được thiết lập thành true
        if ((timeSpan.Hours > 0 || timeSpan.Days == 0) && keepHours)
        {
            result += $"{timeSpan.Hours:00}:";
        }

        // Thêm phút và giây
        result += $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";

        return result;
    }
    static string[] formatTime = { "mm':'ss", "hh':'mm':'ss" };
    public static string ConvertTime(double time, int indexFormatTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string strTime = timeSpan.ToString(formatTime[indexFormatTime]);
        return strTime;
    }
    public static string ShortenString(string input, int maxLength)
    {
        if (input.Length <= maxLength)
        {
            return input; // Nếu chuỗi ngắn hơn hoặc bằng maxLength, trả về chuỗi gốc
        }

        return input.Substring(0, maxLength - 3) + "..."; // Lấy phần đầu chuỗi và thêm "..."
    }
}
