using System.Collections.Generic;

namespace ImageNexus.BenScharbach.Common.PasswordGenerator
{
    /// <summary>
    /// Interface for the <see cref="PasswordGenerator"/>.
    /// </summary>
    public interface IPasswordGenerator
    {
        /// <summary>
        /// Gets or sets to use the lowercase Alpha set.
        /// </summary>
        bool UseAlphaLowercaseSet { get; set; }

        /// <summary>
        /// Gets or sets to use the uppercase Alpha set.
        /// </summary>
        bool UseAlphaUppercaseSet { get; set; }

        /// <summary>
        /// Gets or sets to use the numeric set.
        /// </summary>
        bool UseNumericSet { get; set; }

        /// <summary>
        /// Gets or sets to use the symbol set.
        /// </summary>
        bool UseSymbolSet { get; set; }

        /// <summary>
        /// Generates a password.
        /// </summary>
        /// <returns>Generated password as string.</returns>
        string Generate();

        /// <summary>
        /// Generates a batch of passwords using the .Net Parallel.ForEach contruct.
        /// </summary>
        /// <returns>Collection of generated passwords as string.</returns>
        List<string> GenerateParallel(int batchSize);
    }
}