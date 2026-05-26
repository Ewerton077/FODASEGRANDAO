using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backendconfigconecta.Models
{
    public class UpdateRequest
    {
        public int Id { get; set; }


        public ApplicationUser User { get; set; } = null!;

        public string Campo { get; set; } = string.Empty;
        public string NovoValor { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        public bool Aprovado { get; set; }
        public bool Revisado { get; set; }

        public DateTime DataSolicitacao { get; set; } = DateTime.Now;
    }

}

//DEFININDO A CLASSE PARA SOLICITAÇÃO DE ATUALIZAÇÃO DE DADOS