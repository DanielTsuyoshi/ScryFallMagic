using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScryFallMagic.Entities
{
    /// <summary>
    /// Representa a versão das cartas.
    /// </summary>
    public class Versao
    {
        [Key]
        public int VersaoId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(50)]
        public string Sigla { get; set; }

        [MaxLength(20)]
        public DateTime DataLancamento { get; set; }

        // Chaves estrangeiras
        public virtual ICollection<Carta> Cartas { get; set; }
    }
}