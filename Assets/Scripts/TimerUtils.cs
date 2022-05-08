using System;
using System.Collections.Generic;
using UnityEngine;
public class TimerUtils : MonoBehaviour
{
    private class Timer
    {
        private Action callback;
        private float timer;
        private float initTimer;
        public bool isSame(Action callback, float t, bool checkTimer = true)
        {
            return this.callback.Equals(callback) && (initTimer == t || (checkTimer ? initTimer == t : true));
        }
        public float InitialTimer => initTimer;
        public Timer(float t, Action callback)
        {
            timer = t;
            initTimer = t;
            this.callback = callback;
        }
        public void Step()
        {
            timer -= Time.deltaTime;
            bool isOK = true;
            if (timer <= 0)
            {
                Invoke(ref isOK);
                if (isOK)
                {
                    TimerUtils.Pop(this);
                }
            }
        }
        private void Invoke(ref bool isOkay)
        {
            try
            {
                callback();
            }
            catch
            {
                Pop(this);
                isOkay = false;
            }
        }
    }
    private static List<Timer> timer = new List<Timer>();
    private void Update()
    {
        for (int i = 0; i < timer.Count; i++)
        {
            if (timer[i] != null)
            {
                timer[i].Step();
            }
        }
        for (int i = 0; i < timer.Count; i++)
        {
            if (timer[i] == null)
            {
                timer.RemoveAt(i);
                i = 0;
            }
        }
    }
    public static void AddTimer(float t, Action callback)
    {
        foreach (var item in timer)
        {
            if (item != null && item.isSame(callback, t))
            {
                return;
            }
        }
        timer.Add(new Timer(t, callback));
    }
    private static void Pop(Timer timer)
    {
        int ind = TimerUtils.timer.IndexOf(timer);
        TimerUtils.timer[ind] = null;
    }
    public static void Cancel(Action action)
    {
        try
        {
            timer.RemoveAll((m) => m.isSame(action, 0, false));
        }
        catch
        {
            return;
        }
    }
}
