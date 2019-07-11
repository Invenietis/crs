using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCake.Abstractions
{
    interface ISolutionProducingArtifact
    {
        /// <summary>
        /// Pack the solution: it produce the artifacts.
        /// </summary>
        void Pack();

        ArtifactType ArtifactType { get; }
    }
}
