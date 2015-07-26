using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public enum TransactionStatus : int
    {
        Aperta,
        InAttesa,
        Conclusa
    }
}