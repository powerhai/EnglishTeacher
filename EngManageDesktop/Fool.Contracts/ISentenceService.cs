using System.Collections.Generic;
using Fool.Models;
namespace Fool.Contracts
{
    public interface ISentenceService
    {
        IEnumerable<Sentence> GetSentencesOfText(int textId);
    }
}