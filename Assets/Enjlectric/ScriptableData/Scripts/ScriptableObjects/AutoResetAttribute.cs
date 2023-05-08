using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enjlectric.ScriptableData
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AutoResetAttribute : Attribute
    {
        public string copySourceName;
        public string copySourceTestName;

        public AutoResetAttribute(string copySourceName, string copySourceTestName)
        {
            this.copySourceName = copySourceName;
            this.copySourceTestName = copySourceTestName;
        }
    }

}