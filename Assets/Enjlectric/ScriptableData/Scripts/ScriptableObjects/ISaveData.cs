namespace Enjlectric.ScriptableData
{
    /// <summary>
    /// Interface for deriving functions to interop with Scriptable Data save manager systems.
    /// </summary>
    public interface ISaveData
    {
        /// <summary>
        /// Restore the data to a given value passed from a SaveDataManager.
        /// </summary>
        /// <param name="valueToRestoreTo">The value to restore to.</param>
        void Restore(object valueToRestoreTo);

        /// <summary>
        /// Reset the SaveData to its default value.
        /// </summary>
        void ResetToDefault();

        /// <summary>
        /// Get the identifier for the data as used in SaveData.
        /// </summary>
        /// <returns>A string used to identify the data.</returns>
        string GetIdentifier();

        /// <summary>
        /// Get the current value of the SaveData. Only necessary to use if in the Base class, as derived classes should have a public Value field of the specific type they are serializing.
        /// </summary>
        /// <returns>The current value, as an object.</returns>
        object GetCurrentValue();
    }
}