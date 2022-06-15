using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using System.Globalization;
using System.Threading;

/// <summary>
/// This class allows you to get a time from the internet, which is less easily manipulated than the system time.
/// </summary>
public static class NetworkTimeChecker
{
    public static long OfflineTime => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public enum Result { OnlineTime, OfflineTime };

    private static Thread m_connectionThread;
    private static Thread m_timeoutThread;

    public static bool IsRunning => m_connectionThread != null && m_timeoutThread != null && (m_connectionThread.IsAlive || m_timeoutThread.IsAlive);

    /// <summary>
    /// Will try to get the unix epoch time from the internet, with a timeout given in seconds.
    /// Either way, the given callback will return a result, though if the result is offline it could be wrong if the user has changed
    /// their system time.
    /// </summary>
    public static bool RequestTime(float timeout, Action<Result, long> callback)
    {
        if (IsRunning)
        {
            Debug.LogError("Can only have one active network time request at a time.");
            return false;
        }

        m_connectionThread = new Thread(new ThreadStart(() => RequestTime(callback)));
        m_connectionThread.IsBackground = true;
        m_connectionThread.Start();

        m_timeoutThread = new Thread(new ThreadStart(() => Timeout(timeout)));
        m_timeoutThread.IsBackground = true;
        m_timeoutThread.Start();

        return true;
    }

    private static void Timeout(float timeout)
    {
        Thread.Sleep(TimeSpan.FromSeconds(timeout));

        if (m_connectionThread != null && m_connectionThread.IsAlive)
            m_connectionThread.Abort();
    }

    private static void RequestTime(Action<Result, long> callback)
    {
        TcpClient client = null;

        try
        {
            client = new TcpClient("time.nist.gov", 13);
            using (StreamReader streamReader = new StreamReader(client.GetStream()))
            {
                string response = streamReader.ReadToEnd();
                string utcDateTimeString = response.Substring(7, 17);
                DateTime localDateTime = DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                DateTimeOffset dateTimeOffset = new DateTimeOffset(localDateTime);
                long unixDateTime = dateTimeOffset.ToUnixTimeSeconds();

                callback?.Invoke(Result.OnlineTime, unixDateTime);
            }

            client.Close();
            m_timeoutThread.Abort();
        }
        catch (Exception e)
        {
            callback?.Invoke(Result.OfflineTime, OfflineTime);

            if (client != null)
                client.Close();

            m_timeoutThread.Abort();

            if (e is ThreadAbortException)
                throw e;
        }
    }
}