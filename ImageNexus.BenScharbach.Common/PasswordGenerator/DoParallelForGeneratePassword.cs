using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImageNexus.BenScharbach.Common.PasswordGenerator
{
    public class StringWrapper
    {
        public string Password;
    }

    /// <summary>
    /// Returns a string collection using a ParallelFor action.
    /// </summary>
    public static class DoParallelForGeneratePassword
    {
        private static ObjectState _objectState;

        /// <summary>
        /// Objectstate class used to pass into parallel thread.
        /// </summary>
        private class ObjectState
        {
            public PasswordGenerator PasswordGenerator;
            public int PasswordLength;
            public int MaxLength;
        }

        /// <summary>
        /// populate with your comments
        /// </summary>
        /// <returns>Collection of <see cref="object"/> updated.</returns>
        public static List<StringWrapper> DoParallelForAction(PasswordGenerator passwordGenerator, int passwordLength, int batchSize)
        {
            _objectState = new ObjectState
                {
                    PasswordGenerator = passwordGenerator,
                    PasswordLength = passwordLength,
                    MaxLength = PasswordGenerator.MaxPasswordLength,
                };

            var passwordStrings = new List<StringWrapper>(batchSize);
            for (var i = 0; i < batchSize; i++)
            {
                passwordStrings.Add(new StringWrapper());
            }
            
            /*var parallelOptions = new ParallelOptions
            {
                CancellationToken = new CancellationToken(),
                MaxDegreeOfParallelism = 4
            };*/          

            var result = Parallel.ForEach(passwordStrings, () => _objectState, DoActionCallback, DoLocalFinalCallback);            

            return passwordStrings;
        }

        /// <summary>
        /// ParallelFor LocalFinal callback
        /// </summary>
        /// <param name="objectState">Instance of <see cref="ObjectState"/></param>
        private static void DoLocalFinalCallback(ObjectState objectState)
        {
            // empty
        }

        /// <summary>
        /// ParallelFor DoAction callback
        /// </summary>
        /// <param name="iterationItem">Instance of <see cref="object"/></param>
        /// <param name="parallelLoopState">Instance of <see cref="ParallelLoopState"/></param>
        /// <param name="objectState">Instance of <see cref="ObjectState"/></param>
        /// <returns>Instance of <see cref="ObjectState"/></returns>
        private static ObjectState DoActionCallback(StringWrapper iterationItem, ParallelLoopState parallelLoopState,
                                                    ObjectState objectState)
        {
            if (iterationItem == null)
            {
                return objectState;
            }

            var characterSets = objectState.PasswordGenerator.CharacterSets;
            var characterSetsForRandom = objectState.PasswordGenerator.CharacterSetsForRandom;
            var characterSetsCount = objectState.PasswordGenerator.CharacterSetsCount;
            var passwordLength = objectState.PasswordLength;
            var maxPasswordLength = objectState.MaxLength;

            var passwordStringBuilder = new StringBuilder(passwordLength, maxPasswordLength);
            for (var i = 0; i < passwordLength; i++)
            {
                passwordStringBuilder.Append(PasswordGenerator.RetrieveRandomCharacter(characterSets, characterSetsForRandom, characterSetsCount));
            }

            iterationItem.Password = passwordStringBuilder.ToString();

            return objectState;
        }
    }
}
