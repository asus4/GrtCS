using System.Runtime.CompilerServices;

namespace GRT
{
    public abstract class GRTBase
    {
        #region Properties
        protected string classId;
        #endregion

        #region Public

        /// <summary>
        /// Default GRTBase Constructor
        /// </summary>
        /// <param name="id">a string representing the class ID of the inheriting type</param>
        public GRTBase(string id = "")
        {

        }

        /// <summary>
        /// This copies the GRTBase variables from the GRTBase pointer to the instance that calls the function.
        /// </summary>
        /// <param name="grtBase">A GRTBase from which the values will be copied to the instance that calls the function</param>
        /// <returns>Returns true if the copy was successfull, false otherwise</returns>
        public bool CopyGRTBaseVariables(GRTBase grtBase)
        {
            if (grtBase == null)
            {
                Log.Error("copyBaseVariables(GRTBase grtBase) - grtBase is null");
                return false;
            }
            classId = grtBase.classId;
            return true;
        }


        /// <summary>
        /// Gets the id of the class that is inheriting from this base class, e.g., if the KNN Classifier class inherits from Classifier, which inherits from MLBase, which inherits from GRTBase, then the classId will be the id of the KNN class
        /// </summary>
        public string Id => classId;

        /// <summary>
        /// This functions the GRT version number and revision as a std::string. If you do not want the revision number then set the returnRevision parameter to false.
        /// </summary>
        /// <param name="returnRevision">sets if the revision number should be added to the std::string that is returned. Default value is true.</param>
        /// <returns>returns the GRT version number and revision as a std::string.</returns>
        public string GetGRTVersion(bool returnRevision)
        {
            var version = GRTVersionInfo.VERSION;
            if (returnRevision)
            {
                version += " revision: " + GRTVersionInfo.REVISION;
            }
            return version;
        }

        public string GetGRTRevison => GRTVersionInfo.REVISION;



        /// <summary>
        /// Scales the input value x (which should be in the range [minSource maxSource]) to a value in the new target range of [minTarget maxTarget].
        /// </summary>
        /// <param name="x">the value that should be scaled</param>
        /// <param name="minSource">the minimum range that x originates from</param>
        /// <param name="maxSource">the maximum range that x originates from</param>
        /// <param name="minTarget">the minimum range that x should be scaled to</param>
        /// <param name="maxTarget">the maximum range that x should be scaled to</param>
        /// <param name="constrain">sets if the scaled value should be constrained to the target range</param>
        /// <returns>returns a new value that has been scaled based on the input parameters</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Scale(double x, ref double minSource, ref double maxSource, ref double minTarget, ref double maxTarget, bool constrain = false)
        {
            return GRT.Scale(x, ref minSource, ref maxSource, ref minTarget, ref maxTarget, constrain);
        }


        #endregion
    }

}
