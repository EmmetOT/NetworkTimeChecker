using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    private void Awake()
    {
        NetworkTimeChecker.RequestTime(3f, OnNetworkTimeRequestAnswered);
    }

    private void OnNetworkTimeRequestAnswered(NetworkTimeChecker.Result result, long unixTime)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime).ToLocalTime();

        string source = result == NetworkTimeChecker.Result.OnlineTime ? "the internet" : "the system time";
        Debug.Log($"Received unix time {unixTime} from {source}, which is the time: {dateTime.ToString("dd/MM/yyyy HH:mm:ss")}");
    }
}
