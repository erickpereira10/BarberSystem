using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace BarberSystem.Dados {

    [Table("dbo.CONTAS_PAGAR")]
    public partial class CONTAS_PAGAR {
   
        [Key]
        public int? codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string descricao { get; set; }
        public virtual FORNECEDORES fornecedor { get; set; }
        public DateTime? data_vencto { get; set; }
        public DateTime? data_pagto { get; set; }
        public double? vl_unitario { get; set; }
        public double? vl_total { get; set; }

    }

}