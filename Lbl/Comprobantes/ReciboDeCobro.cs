using Lazaro.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lbl.Comprobantes
{
        [Lbl.Atributos.Nomenclatura(NombreSingular = "Recibo de cobro", Grupo = "Comprobantes")]
        [Lbl.Atributos.Datos(TablaDatos = "recibos", CampoId = "id_recibo", TablaImagenes = "recibos_imagenes")]
        [Lbl.Atributos.Presentacion()]

        [Entity(TableName = "recibos", IdFieldName = "id_recibo")]
        public class ReciboDeCobro : Recibo
        {
                //Heredar constructor
                public ReciboDeCobro(Lfx.Data.IConnection dataBase)
                        : base(dataBase) { }

                public ReciboDeCobro(Lfx.Data.IConnection dataBase, int itemId)
                        : base(dataBase, itemId) { }

                public ReciboDeCobro(Lfx.Data.IConnection dataBase, Lfx.Data.Row row)
                        : base(dataBase, row) { }

                public override void Crear()
                {
                        base.Crear();
                        this.Tipo = Lbl.Comprobantes.Tipo.TodosPorLetra["RC"];
                        this.Concepto = Lbl.Cajas.Concepto.IngresosPorFacturacion;
                        this.Vendedor = new Personas.Persona(this.Connection, Lbl.Sys.Config.Actual.UsuarioConectado.Id);
                }
        }
}