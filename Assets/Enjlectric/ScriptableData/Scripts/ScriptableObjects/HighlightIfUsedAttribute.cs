using System;
using UnityEngine;

namespace Enjlectric.ScriptableData
{
    /// <summary>
    /// The HighlightIfUsed Attribute highlights the editor field being used as per the project's ScriptableData configuration.
    /// It is not intended to be used on any random variables.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HighlightIfUsedAttribute : PropertyAttribute
    {
        public bool highlightForProductionMode;

        /// <summary>
        /// The HighlightIfUsed Attribute highlights the editor field being used as per the project's ScriptableData configuration.
        /// </summary>
        /// <param name="highlightForProductionMode">If true, highlights the field when in Production mode. If false, the field gets highlighted if in Debug mode instead.</param>
        public HighlightIfUsedAttribute(bool highlightForProductionMode)
        {
            this.highlightForProductionMode = highlightForProductionMode;
        }
    }
}