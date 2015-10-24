using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public enum ProposalStatus : int
    {
        DaApprovare,
        Accettata,
        Rifiutata,
        Annullata
    }    
}