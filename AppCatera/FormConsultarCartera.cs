using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace AppCatera
{
    public partial class FormConsultarCartera : Form
    {
        private string connectionString = "Server=SRVMED02\\SRVMED02;Database=Factu01;Integrated Security=True;TrustServerCertificate=True;";
        private string selectedAnoMes = "";
        private string selectedSector = "";
        private string selectedCoordinador = "";

        public FormConsultarCartera()
        {
            InitializeComponent();
            SetDefaultAnoMes();
            LoadSectors();
            LoadCoordinadores();
            ConfigureDataGridView();
            LoadAccionesValidas(); // Cargar las acciones activas al iniciar
            


            // Eventos para formateo y validaciones en el DataGridView
            dataGridViewResults.CellFormatting += dataGridViewResults_CellFormatting;
            dataGridViewResults.CellEnter += dataGridViewResults_CellEnter;
            dataGridViewResults.CellLeave += dataGridViewResults_CellLeave;
            comboBoxCoordinador.SelectedIndexChanged += comboBoxCoordinador_SelectedIndexChanged;
            dataGridViewResults.CellValidating += dataGridViewResults_CellValidating;

        }


        private void txtAnoMes_TextChanged(object sender, EventArgs e)
        {
            string ano = textBoxAno.Text.Trim();
            string mes = textBoxMes.Text.Trim();
            selectedAnoMes = (ano.Length == 4 && mes.Length == 2) ? ano + mes : "";
        }

        private void comboBoxSectors_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSector = comboBoxSectors.SelectedItem?.ToString() ?? "";
        }
        private void LoadCoordinadores()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CodCartera, NombreCartera FROM DimCoorCartera ORDER BY NombreCartera";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBoxCoordinador.Items.Clear();
                    comboBoxCoordinador.Items.Add("Todos");

                    while (reader.Read())
                    {
                        comboBoxCoordinador.Items.Add(reader["CodCartera"].ToString() + " - " + reader["NombreCartera"].ToString());
                    }
                    comboBoxCoordinador.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los Coordinadores: " + ex.Message);
                }
            }
        }
        private void comboBoxCoordinador_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Extraer el código del Coordinador seleccionado
            selectedCoordinador = comboBoxCoordinador.SelectedItem?.ToString().Split(' ')[0] ?? "";

            // Llamar la función para actualizar los sectores según el Coordinador
            LoadSectors(selectedCoordinador);
        }



        private void LoadSectors(string codCartera = "")
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Consulta SQL que filtra sectores por el Coordinador si se seleccionó uno
                    string query = @"
                SELECT DISTINCT dimSectorEconomico.Descripcion 
                FROM dimSectorEconomico
                INNER JOIN DimCoorCartera 
                ON DimCoorCartera.CodCartera = dimSectorEconomico.CODCARTERA
                WHERE (@CodCartera = '' OR DimCoorCartera.CodCartera = @CodCartera)
                ORDER BY dimSectorEconomico.Descripcion";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CodCartera", codCartera);

                    SqlDataReader reader = cmd.ExecuteReader();

                    comboBoxSectors.Items.Clear();
                    comboBoxSectors.Items.Add("Todos"); // Agregar la opción "Todos"

                    while (reader.Read())
                    {
                        comboBoxSectors.Items.Add(reader["Descripcion"].ToString());
                    }
                    comboBoxSectors.SelectedIndex = 0; // Seleccionar el primer valor por defecto
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los sectores: " + ex.Message);
                }
            }
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedAnoMes))
            {
                MessageBox.Show("Debe ingresar un Año y Mes en formato YYYY MM.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string filtroCliente = textBoxBuscarCliente.Text.Trim();

                    // 🔹 Mensaje de depuración
                    //MessageBox.Show($"Cargando datos para: AñoMes = {selectedAnoMes}, FiltroCliente = {filtroCliente}");

                    string query = @"
                SELECT 
                    dimCiudad.NombreCiudad AS Ciudad, 
                    Clientes.Nit AS NIT,
                    Clientes.Razon_Social AS Nombre,  
                    CxC.Numero AS Factura,    
                    CONVERT(CHAR(10), CxC.Fecha, 103) AS Fecha,  
                    CONVERT(CHAR(10), CxC.Vencimiento, 103) AS Vence,
                    Clientes.Plazo_Credito as 'Plazo',  
                    DATEDIFF(DAY, GETDATE(), CxC.Vencimiento) AS Dias,
                                            ROUND(CASE WHEN DATEDIFF(DAY, CxC.Vencimiento, GETDATE()) BETWEEN 1 AND 30  
                                                THEN (CxC.Valor + CxC.Saldo_Ini_Mes + CxC.Debitos - CxC.Creditos) 
                                                ELSE 0  
                                            END, 0) AS Cartera30,

                                            ROUND(CASE WHEN DATEDIFF(DAY, CxC.Vencimiento, GETDATE()) BETWEEN 31 AND 60  
                                                THEN (CxC.Valor + CxC.Saldo_Ini_Mes + CxC.Debitos - CxC.Creditos) 
                                                ELSE 0  
                                            END, 0) AS Cartera60,

                                            ROUND(CASE WHEN DATEDIFF(DAY, CxC.Vencimiento, GETDATE()) BETWEEN 61 AND 90  
                                                THEN (CxC.Valor + CxC.Saldo_Ini_Mes + CxC.Debitos - CxC.Creditos) 
                                                ELSE 0  
                                            END, 0) AS Cartera90,

                                            ROUND(CASE WHEN DATEDIFF(DAY, CxC.Vencimiento, GETDATE()) > 90  
                                                THEN (CxC.Valor + CxC.Saldo_Ini_Mes + CxC.Debitos - CxC.Creditos) 
                                                ELSE 0  
                                            END, 0) AS MasDe91,

                                            ROUND(CASE WHEN DATEDIFF(DAY, CxC.Vencimiento, GETDATE()) <= 0  
                                                THEN (CxC.Valor + CxC.Saldo_Ini_Mes + CxC.Debitos - CxC.Creditos) 
                                                ELSE 0  
                                            END, 0) AS PorVencer,

                                            ROUND((CxC.Valor + CxC.Saldo_Ini_Mes + CxC.Debitos - CxC.Creditos), 1) AS Saldo,

                                            -- Datos de Comité
                                            CONVERT(CHAR(10), ISNULL(GestionCartera.fecha_compromiso, GETDATE()), 103) AS FechaCompromiso,
                                            GestionCartera.accion_comite AS AccionComite,
                                            GestionCartera.observacion AS Accion

                                        FROM CxC_2000 CxC  
                                            LEFT JOIN GestionCartera ON CxC.Numero = GestionCartera.factura
                                            INNER JOIN Clientes ON Clientes.Nit = CxC.Nit  
                                            INNER JOIN Zonas ON CxC.Zona = Zonas.Zona  
                                            LEFT JOIN dimCiudad ON Zonas.CodCiudad = dimCiudad.CodCiudad  
                                            LEFT JOIN dimSectorEconomico ON Zonas.Sector = dimSectorEconomico.Sector 
                                            INNER JOIN DimCoorCartera ON DimCoorCartera.CodCartera = DimSectorEconomico.CODCARTERA
                                            WHERE CxC.Anulado = 0  
                                            AND (CxC.Valor + CxC.Saldo_Ini_Mes + CxC.Debitos - CxC.Creditos) <> 0  
                                            AND CxC.mes = @SelectedAnoMes
                                            AND (@Coordinador = '' OR DimCoorCartera.CodCartera = @Coordinador)
                                            AND (@Sector = '' OR dimSectorEconomico.Descripcion = @Sector)
                                            AND (@FiltroCliente = '' OR Clientes.Razon_Social LIKE '%' + @FiltroCliente + '%' OR Clientes.Nit LIKE '%' + @FiltroCliente + '%')
                                        ORDER BY dimCiudad.NombreCiudad, Clientes.Razon_Social, CxC.Vencimiento ASC;";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@SelectedAnoMes", SqlDbType.VarChar).Value = selectedAnoMes;
                        cmd.Parameters.Add("@Coordinador", SqlDbType.VarChar).Value = selectedCoordinador;
                        cmd.Parameters.Add("@Sector", SqlDbType.VarChar).Value = selectedSector;
                        cmd.Parameters.Add("@FiltroCliente", SqlDbType.VarChar).Value = filtroCliente;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // 🔹 Mensaje de depuración para ver si hay datos
                        //MessageBox.Show($"Filas obtenidas: {dt.Rows.Count}");

                        // Asignar los datos al DataGridView
                        dataGridViewResults.DataSource = null;  // Evitar problemas de refresco
                        dataGridViewResults.DataSource = dt;
                        SetColumnWidths();
                        dataGridViewResults.ColumnHeadersVisible = true;
                        dataGridViewResults.Refresh();
                        dataGridViewResults.Update();
                        //dataGridViewResults.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); // Esta columna en particular Pone Automatico y anula el tamaño custom de columna
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureGridColumns()
        {
            if (!dataGridViewResults.Columns.Contains("FechaCompromiso"))
            {
                dataGridViewResults.Columns.Add("FechaCompromiso", "Fecha Compromiso");
            }

            if (!dataGridViewResults.Columns.Contains("AccionComite"))
            {
                DataGridViewTextBoxColumn accionComiteCol = new DataGridViewTextBoxColumn
                {
                    Name = "AccionComite",
                    HeaderText = "Acción Comité"
                };
                dataGridViewResults.Columns.Add(accionComiteCol);
            }

            if (!dataGridViewResults.Columns.Contains("Accion"))
            {
                dataGridViewResults.Columns.Add("Accion", "Acción");
            }
        }


        // Métodos para formateo y validación
        private void dataGridViewResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Lista de columnas que deben mostrar solo 2 decimales
            string[] columnasNumericas = { "Cartera30", "Cartera60", "Cartera90", "MasDe91", "PorVencer", "Saldo" };

            if (columnasNumericas.Contains(dataGridViewResults.Columns[e.ColumnIndex].Name))
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal valor))
                {
                    e.Value = valor.ToString("N0"); // Redondeo sin decimales
                    e.FormattingApplied = true;
                }
            }

            // Formatear fecha de compromiso
            if (dataGridViewResults.Columns[e.ColumnIndex].Name == "FechaCompromiso" && (e.Value == null || e.Value.ToString() == "dd/MM/yyyy"))
            {
                e.Value = "dd/MM/yyyy";
                e.CellStyle.ForeColor = Color.Gray;
            }

            // Formatear acción
            if (dataGridViewResults.Columns[e.ColumnIndex].Name == "Accion" && (e.Value == null || e.Value.ToString() == "Ingrese una observación"))
            {
                e.Value = "Ingrese una observación";
                e.CellStyle.ForeColor = Color.Gray;
            }
        }


        private void dataGridViewResults_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewResults.Columns[e.ColumnIndex].Name == "FechaCompromiso" ||
                dataGridViewResults.Columns[e.ColumnIndex].Name == "Accion")
            {
                if (dataGridViewResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "dd/MM/yyyy" ||
                    dataGridViewResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Ingrese una observación")
                {
                    dataGridViewResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    dataGridViewResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                }
            }
        }

        private void dataGridViewResults_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewResults.Columns[e.ColumnIndex].Name == "FechaCompromiso" &&
                string.IsNullOrWhiteSpace(dataGridViewResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString()))
            {
                dataGridViewResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "dd/MM/yyyy";
                dataGridViewResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Gray;
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewResults.Rows.Count > 0)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "CSV Files|*.csv";
                        saveFileDialog.Title = "Guardar como CSV";
                        saveFileDialog.FileName = "Cartera.csv";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName))
                            {
                                // Escribir encabezados
                                for (int i = 0; i < dataGridViewResults.Columns.Count; i++)
                                {
                                    writer.Write(dataGridViewResults.Columns[i].HeaderText);
                                    if (i < dataGridViewResults.Columns.Count - 1)
                                        writer.Write(",");
                                }
                                writer.WriteLine();

                                // Escribir filas
                                foreach (DataGridViewRow row in dataGridViewResults.Rows)
                                {
                                    for (int i = 0; i < dataGridViewResults.Columns.Count; i++)
                                    {
                                        writer.Write(row.Cells[i].Value?.ToString());
                                        if (i < dataGridViewResults.Columns.Count - 1)
                                            writer.Write(",");
                                    }
                                    writer.WriteLine();
                                }
                            }

                            MessageBox.Show("Datos exportados correctamente a CSV.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No hay datos para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSendData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    foreach (DataGridViewRow row in dataGridViewResults.Rows)
                    {
                        // Extraer los valores de la fila
                        string factura = row.Cells["Factura"].Value?.ToString()?.Trim();
                        string fechaCompromisoStr = row.Cells["FechaCompromiso"].Value?.ToString()?.Trim();
                        string accionComite = row.Cells["AccionComite"].Value?.ToString()?.Trim();
                        string observacion = row.Cells["Accion"].Value?.ToString()?.Trim();
                        string valorStr = row.Cells["Saldo"].Value?.ToString()?.Replace(",", "").Trim(); // Elimina comas de miles
                        decimal valorDecimal = 0;

                        if (!decimal.TryParse(valorStr, out valorDecimal))
                        {
                            MessageBox.Show($"No se pudo convertir el saldo para la factura {factura}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue; // Evita errores si el valor no es válido
                        }


                        // Validar que haya datos en acción y en la factura
                        if (string.IsNullOrWhiteSpace(factura) || string.IsNullOrWhiteSpace(accionComite))
                        {
                            continue; // Si no hay datos válidos, saltar esta fila
                        }

                        // Convertir Fecha de Compromiso (si existe)
                        DateTime? fechaCompromiso = null;
                        if (!string.IsNullOrWhiteSpace(fechaCompromisoStr) && fechaCompromisoStr != "dd/MM/yyyy")
                        {
                            if (DateTime.TryParse(fechaCompromisoStr, out DateTime tempFecha))
                            {
                                fechaCompromiso = tempFecha;
                            }
                            else
                            {
                                MessageBox.Show($"Error en la conversión de Fecha Compromiso para la factura {factura}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                continue;
                            }
                        }

                        // Verificar si la factura ya existe en la base de datos
                        string checkQuery = "SELECT COUNT(*) FROM GestionCartera WHERE factura = @Factura";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Factura", factura);
                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (count > 0)
                            {
                                // 🔁 Si la factura EXISTE: se actualiza
                                string updateQuery = @"
                            UPDATE GestionCartera
                            SET fecha_compromiso = @FechaCompromiso,
                                accion_comite = @AccionComite,
                                observacion = @Observacion
                            WHERE factura = @Factura";

                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@Factura", factura);
                                    updateCmd.Parameters.AddWithValue("@FechaCompromiso", fechaCompromiso.HasValue ? (object)fechaCompromiso.Value : DBNull.Value);
                                    updateCmd.Parameters.AddWithValue("@AccionComite", accionComite);
                                    updateCmd.Parameters.AddWithValue("@Observacion", observacion ?? string.Empty); // evita NULL

                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // 🆕 Si la factura NO existe: se inserta
                                string insertQuery = @"
                                                        INSERT INTO GestionCartera (factura, fecha_compromiso, accion_comite, observacion, fecha, valor)
                                                        VALUES (@Factura, @FechaCompromiso, @AccionComite, @Observacion, @FechaRegistro, @valor)";

                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@Factura", factura);
                                    insertCmd.Parameters.AddWithValue("@FechaCompromiso", fechaCompromiso.HasValue ? (object)fechaCompromiso.Value : DBNull.Value);
                                    insertCmd.Parameters.AddWithValue("@AccionComite", accionComite);
                                    insertCmd.Parameters.AddWithValue("@Observacion", observacion ?? string.Empty); // evita NULL
                                    insertCmd.Parameters.AddWithValue("@FechaRegistro", DateTime.Now); // fecha del sistema
                                    insertCmd.Parameters.AddWithValue("@Valor", valorDecimal);


                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                    }

                    MessageBox.Show("Datos guardados correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void ConfigureDataGridView()
        {
            // Asegurar que los encabezados sean visibles
            dataGridViewResults.ColumnHeadersVisible = true;
            dataGridViewResults.RowHeadersVisible = false;

            // Habilitar el ajuste manual de columnas con el mouse
            dataGridViewResults.AllowUserToResizeColumns = true;  // ✅ Permitir redimensionar columnas
            dataGridViewResults.AllowUserToResizeRows = false;  // ❌ No permitir redimensionar filas

            // 🔹 Deshabilitar el ajuste automático de columnas para permitir el redimensionamiento manual
            dataGridViewResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridViewResults.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // Ajustar tamaño de columnas y encabezados
            //dataGridViewResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; // Esta columna en particular Pone Automatico y anula el tamaño custom de columna
            dataGridViewResults.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridViewResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewResults.ColumnHeadersHeight = 100;  // Aumentar la altura de los encabezados

            // Aplicar estilo a los encabezados
            dataGridViewResults.EnableHeadersVisualStyles = false;
            dataGridViewResults.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            dataGridViewResults.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            dataGridViewResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewResults.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;

            // Aplicar alineación al contenido de las columnas
            foreach (DataGridViewColumn col in dataGridViewResults.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;  // Centrar los datos
                col.Width = 50;
                dataGridViewResults.ColumnHeadersDefaultCellStyle.Padding = new Padding(5); // Espaciado interno
                dataGridViewResults.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single; // Borde simple
                col.MinimumWidth = 60;
                col.Resizable = DataGridViewTriState.True;
            }
            dataGridViewResults.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64); // Fondo gris oscuro
            dataGridViewResults.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(100, 100, 100); // Fondo al seleccionar
            dataGridViewResults.GridColor = Color.LightGray; // Color de las líneas de cuadrícula
            dataGridViewResults.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal; // Líneas horizontales

            // Alternar colores de filas para mejorar la lectura
            dataGridViewResults.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dataGridViewResults.DefaultCellStyle.BackColor = Color.White;
            dataGridViewResults.DefaultCellStyle.ForeColor = Color.Black;
            dataGridViewResults.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dataGridViewResults.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Bordes y estilos generales
            dataGridViewResults.BorderStyle = BorderStyle.Fixed3D;
        }

        private void SetColumnWidths()
        {
            if (dataGridViewResults.Columns.Count > 0)
            {
                foreach (DataGridViewColumn column in dataGridViewResults.Columns)
                {
                    switch (column.Name)
                    {
                        case "Ciudad":
                            column.Width = 80;
                            break;
                        case "NIT":
                            column.Width = 80;
                            break;
                        case "Nombre":
                            column.Width = 120;
                            break;
                        case "Factura":
                            column.Width = 80;
                            break;
                        case "Fecha":
                        case "Vence":
                        case "FechaCompromiso":
                            column.Width = 80;
                            break;
                        case "Plazo":
                        case "Dias":
                            column.Width = 50;
                            break;
                        case "Cartera30":
                        case "Cartera60":
                        case "Cartera90":
                        case "MasDe91":
                        case "PorVencer":
                        case "Saldo":
                            column.Width = 80;
                            break;
                        case "AccionComite":
                            column.Width = 50;
                            break;
                        case "Accion":
                            column.Width = 100;
                            break;
                        default:
                            column.Width = 80; // Ancho por defecto para columnas que no están en la lista
                            break;
                    }
                    column.MinimumWidth = 60; // Mínimo para que siempre sean visibles
                    column.Resizable = DataGridViewTriState.True; // El usuario puede ampliar o reducir
                }
            }
        }


        private List<string> accionesValidas = new List<string>(); // Lista de acciones permitidas

        private void LoadAccionesValidas()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_accion FROM AccionComite WHERE Estado = 1"; // Ajusta el nombre de la tabla y la condición según tu BD
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    accionesValidas.Clear(); // Limpiar antes de volver a cargar

                    while (reader.Read())
                    {
                        accionesValidas.Add(reader["Id_accion"].ToString().Trim()); // Guardar códigos de acciones activas
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar acciones válidas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void dataGridViewResults_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridViewResults.Columns[e.ColumnIndex].Name == "AccionComite") // Validar solo esta columna
            {
                string nuevoValor = e.FormattedValue.ToString().Trim();

                if (!string.IsNullOrWhiteSpace(nuevoValor) && !accionesValidas.Contains(nuevoValor))
                {
                    MessageBox.Show($"El código '{nuevoValor}' no es una acción válida.\nPor favor ingrese un valor permitido.", "Valor Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Evita que el usuario abandone la celda hasta que ingrese un valor válido
                }
            }
        }

        private void textBoxBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            string filtro = textBoxBuscarCliente.Text.Trim().ToLower();

            if (dataGridViewResults.DataSource is DataTable dt)
            {
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    dt.DefaultView.RowFilter = ""; // 🔹 Sin filtro si está vacío
                }
                else
                {
                    dt.DefaultView.RowFilter = $"Nombre LIKE '%{filtro}%' OR Convert(NIT, 'System.String') LIKE '%{filtro}%'";
                }
            }
        }
        private void SetDefaultAnoMes()
        {
            DateTime fechaActual = DateTime.Now;
            textBoxAno.Text = fechaActual.Year.ToString();
            textBoxMes.Text = fechaActual.Month.ToString("D2"); // D2 => Formato de 2 dígitos
        }


    }
}
