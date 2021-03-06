using Lazaro.Orm.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lbl.Tareas
{
        /// <summary>
        /// Representa una novedad de una tarea.
        /// </summary>
        [Lbl.Atributos.Nomenclatura(NombreSingular = "Novedad", Grupo = "Tareas")]
        [Lbl.Atributos.Datos(TablaDatos = "tickets_eventos", CampoId = "id_evento")]
        [Lbl.Atributos.Presentacion()]
        public class Novedad : ElementoDeDatos
        {
                private Tarea m_Tarea = null;
                private Lbl.Personas.Persona m_Persona = null;

                public Novedad(Lfx.Data.IConnection dataBase)
                        : base(dataBase) { }

                public Novedad(Lfx.Data.IConnection dataBase, int itemId)
                        : base(dataBase, itemId) { }

                public Novedad(Lfx.Data.IConnection dataBase, Lfx.Data.Row row)
                        : base(dataBase, row) { }


                /// <summary>
                /// Devuelve o establece el estado del elemento. El valor de esta propiedad tiene diferentes significados para cada clase derivada.
                /// </summary>
                [Column(Name = "estado")]
                public int Estado
                {
                        get
                        {
                                return this.GetFieldValue<int>("estado");
                        }
                        set
                        {
                                this.Registro["estado"] = value;
                        }
                }


                public string Descripcion
                {
                        get
                        {
                                return this.GetFieldValue<string>("descripcion");
                        }
                        set
                        {
                                this.Registro["descripcion"] = value;
                        }
                }

                public Tarea Tarea
                {
                        get
                        {
                                return m_Tarea;
                        }
                        set
                        {
                                m_Tarea = value;
                                if (value != null)
                                        this.Registro["id_ticket"] = value.Id;
                                else
                                        this.Registro["id_ticket"] = null;
                        }
                }

                public Lbl.Personas.Persona Persona
                {
                        get
                        {
                                return m_Persona;
                        }
                        set
                        {
                                m_Persona = value;
                                if (value != null)
                                        this.Registro["id_tecnico"] = value.Id;
                                else
                                        this.Registro["id_tecnico"] = null;
                        }
                }

                public int Minutos
                {
                        get
                        {
                                return this.GetFieldValue<int>("minutos_tecnico");
                        }
                        set
                        {
                                this.Registro["minutos_tecnico"] = value;
                        }
                }

                public bool Privado
                {
                        get
                        {
                                return this.GetFieldValue<int>("privado") != 0;
                        }
                        set
                        {
                                this.Registro["privado"] = value ? 1 : 0;
                        }
                }


                public override void OnLoad()
                {
                        if (m_Registro != null) {
                                if (this.GetFieldValue<int>("id_ticket") != 0)
                                        m_Tarea = new Tarea(this.Connection, System.Convert.ToInt32(this.Registro["id_ticket"]));
                                else
                                        m_Tarea = null;
                        }
                        base.OnLoad();
                }

                public override void Crear()
                {
                        base.Crear();
                        this.Estado = 1;
                }

                public override Lfx.Types.OperationResult Guardar()
                {
                        qGen.IStatement Comando;

                        if (this.Existe == false) {
                                Comando = new qGen.Insert(this.TablaDatos);
                                Comando.ColumnValues.AddWithValue("fecha", new qGen.SqlExpression("NOW()"));
                        } else {
                                Comando = new qGen.Update(this.TablaDatos);
                                Comando.WhereClause = new qGen.Where(this.CampoId, this.Id);
                        }

                        Comando.ColumnValues.AddWithValue("id_ticket", this.Tarea.Id);
                        if (this.Persona == null)
                                Comando.ColumnValues.AddWithValue("id_tecnico", this.Persona.Id);
                        else
                                Comando.ColumnValues.AddWithValue("id_tecnico", null);
                        Comando.ColumnValues.AddWithValue("descripcion", this.Descripcion);
                        Comando.ColumnValues.AddWithValue("estado", this.Estado);
                        Comando.ColumnValues.AddWithValue("minutos_tecnico", this.Minutos);
                        Comando.ColumnValues.AddWithValue("privado", this.Privado ? 1 : 0);

                        this.AgregarTags(Comando);

                        this.Connection.ExecuteNonQuery(Comando);

                        return base.Guardar();
                }
        }
}
