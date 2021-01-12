using System;
using System.Collections;
using UnityEngine;

namespace Patterns
{
    public class KeepTryingUntilYouSucceed
    {
        private readonly float _interval;
        private readonly Action _callback;

        public KeepTryingUntilYouSucceed(float interval, Action callback)
        {
            _interval = interval;
            _callback = callback;
        }

        public IEnumerator Try()
        {
            var result = false;

            while (!result)
            {
                yield return new WaitForSeconds(_interval);
                
                try
                {
                    _callback();
                    result = true;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}