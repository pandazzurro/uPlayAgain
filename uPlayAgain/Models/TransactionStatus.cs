using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public enum TransactionStatus
    {
        Aperta,
        InAttesa,
        Conclusa
    }
}