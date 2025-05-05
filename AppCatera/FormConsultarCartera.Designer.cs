using System;
using System.Data;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;

namespace AppCatera
{
    partial class FormConsultarCartera
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label labelAno;
        private System.Windows.Forms.Label labelMes;
        private System.Windows.Forms.Label labelSector;
        private System.Windows.Forms.Label labelCoordinador;
        private System.Windows.Forms.TextBox textBoxAno;
        private System.Windows.Forms.TextBox textBoxMes;
        private System.Windows.Forms.ComboBox comboBoxSectors;
        private System.Windows.Forms.ComboBox comboBoxCoordinador;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnSendData;
        private System.Windows.Forms.DataGridView dataGridViewResults;
        private System.Windows.Forms.Label labelBuscarCliente;
        private System.Windows.Forms.TextBox textBoxBuscarCliente;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // 🔹 Crear un TableLayoutPanel para organizar los elementos
            this.tableLayoutPanel = new TableLayoutPanel();
            this.tableLayoutPanel.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel.Size = new System.Drawing.Size(1200, 100);
            this.tableLayoutPanel.ColumnCount = 11;
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.Dock = DockStyle.Top;
            this.tableLayoutPanel.Padding = new Padding(10);           // Espaciado interno
            this.tableLayoutPanel.Margin = new Padding(10);            // Espaciado externo
            this.tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            this.tableLayoutPanel.BackColor = Color.WhiteSmoke;        // Fondo sutil
            this.tableLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.AddRows; // Evita desbordes extraños


            // Definir columnas y filas del TableLayoutPanel
            for (int i = 0; i < 11; i++)
            {
                this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // 🔹 Configurar fuente personalizada para etiquetas
            Font labelFont = new Font("Arial", 11, FontStyle.Bold); // Fuente Arial, tamaño 11, en negrita

            // 🔹 Etiqueta "Año"
            this.labelAno = new System.Windows.Forms.Label();
            this.labelAno.Text = "Año:";
            this.labelAno.AutoSize = true;
            this.labelAno.Font = labelFont; // Aplicar nueva fuente
            this.tableLayoutPanel.Controls.Add(this.labelAno, 0, 0);

            // 🔹 Campo para Año
            this.textBoxAno = new System.Windows.Forms.TextBox();
            this.textBoxAno.Size = new System.Drawing.Size(60, 25);
            this.textBoxAno.TextChanged += new System.EventHandler(this.txtAnoMes_TextChanged);
            this.tableLayoutPanel.Controls.Add(this.textBoxAno, 1, 0);
            SetCueBanner(this.textBoxAno, "YYYY");

            // 🔹 Etiqueta "Mes"
            this.labelMes = new System.Windows.Forms.Label();
            this.labelMes.Text = "Mes:";
            this.labelMes.AutoSize = true;
            this.labelMes.Font = labelFont;
            this.tableLayoutPanel.Controls.Add(this.labelMes, 2, 0);

            // 🔹 Campo para Mes
            this.textBoxMes = new System.Windows.Forms.TextBox();
            this.textBoxMes.Size = new System.Drawing.Size(40, 25);
            this.textBoxMes.TextChanged += new System.EventHandler(this.txtAnoMes_TextChanged);
            this.tableLayoutPanel.Controls.Add(this.textBoxMes, 3, 0);
            SetCueBanner(this.textBoxMes, "MM");

            // 🔹 Etiqueta "Coordinador"
            this.labelCoordinador = new System.Windows.Forms.Label();
            this.labelCoordinador.Text = "Coordinador:";
            this.labelCoordinador.AutoSize = true;
            this.labelCoordinador.Font = labelFont;
            this.tableLayoutPanel.Controls.Add(this.labelCoordinador, 4, 0);

            // 🔹 ComboBox para seleccionar Coordinador
            this.comboBoxCoordinador = new System.Windows.Forms.ComboBox();
            this.comboBoxCoordinador.Size = new System.Drawing.Size(200, 25);
            this.comboBoxCoordinador.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCoordinador.SelectedIndexChanged += new System.EventHandler(this.comboBoxCoordinador_SelectedIndexChanged);
            this.tableLayoutPanel.Controls.Add(this.comboBoxCoordinador, 5, 0);

            // 🔹 Etiqueta "Sector"
            this.labelSector = new System.Windows.Forms.Label();
            this.labelSector.Text = "Sector:";
            this.labelSector.AutoSize = true;
            this.labelSector.Font = labelFont;
            this.tableLayoutPanel.Controls.Add(this.labelSector, 6, 0);

            // 🔹 ComboBox para seleccionar sector
            this.comboBoxSectors = new System.Windows.Forms.ComboBox();
            this.comboBoxSectors.Size = new System.Drawing.Size(200, 25);
            this.comboBoxSectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSectors.SelectedIndexChanged += new System.EventHandler(this.comboBoxSectors_SelectedIndexChanged);
            this.tableLayoutPanel.Controls.Add(this.comboBoxSectors, 7, 0);

            // 🔹 Botón "Cargar"
            this.btnLoadData = new System.Windows.Forms.Button();
            this.btnLoadData.Text = "Cargar";
            this.btnLoadData.Size = new System.Drawing.Size(80, 25);
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            this.tableLayoutPanel.Controls.Add(this.btnLoadData, 8, 0);

            // 🔹 Botón "Exportar"
            this.btnExport = new System.Windows.Forms.Button();
            this.btnExport.Text = "Exportar";
            this.btnExport.Size = new System.Drawing.Size(80, 25);
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            this.tableLayoutPanel.Controls.Add(this.btnExport, 9, 0);

            // 🔹 Botón "Enviar"
            this.btnSendData = new System.Windows.Forms.Button();
            this.btnSendData.Text = "Enviar";
            this.btnSendData.Size = new System.Drawing.Size(80, 25);
            this.btnSendData.Click += new System.EventHandler(this.btnSendData_Click);
            this.tableLayoutPanel.Controls.Add(this.btnSendData, 10, 0);

            // 🔹 Etiqueta "Buscar Cliente" en la segunda fila
            this.labelBuscarCliente = new System.Windows.Forms.Label();
            this.labelBuscarCliente.Text = "Buscar Cliente:";
            this.labelBuscarCliente.AutoSize = true;
            this.labelBuscarCliente.Font = labelFont;
            this.tableLayoutPanel.Controls.Add(this.labelBuscarCliente, 0, 1); // Segunda fila


            // 🔹 Campo de búsqueda de cliente en la segunda fila
            this.textBoxBuscarCliente = new System.Windows.Forms.TextBox();
            this.textBoxBuscarCliente.Size = new System.Drawing.Size(200, 20);
            this.textBoxBuscarCliente.TextChanged += new System.EventHandler(this.textBoxBuscarCliente_TextChanged);
            this.tableLayoutPanel.Controls.Add(this.textBoxBuscarCliente, 1, 1); // Segunda fila

            // 🔹 DataGridView - Tabla de resultados
            this.dataGridViewResults = new System.Windows.Forms.DataGridView();
            this.dataGridViewResults.Dock = DockStyle.Fill;
            this.dataGridViewResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this.dataGridViewResults.AllowUserToAddRows = false;
            this.dataGridViewResults.AllowUserToDeleteRows = false;
            this.dataGridViewResults.BackgroundColor = Color.White;
            this.dataGridViewResults.ColumnHeadersVisible = true;
            this.dataGridViewResults.RowHeadersVisible = false;
            this.dataGridViewResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewResults.ColumnHeadersHeight = 40;

            // 🔹 Agregar los controles al formulario
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.dataGridViewResults);

            // 🔹 Configuración del formulario principal (Form1)
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Consulta de Cartera";

            this.ResumeLayout(false);
            this.PerformLayout();
        }


        private void SetCueBanner(TextBox textBox, string text)
        {
            const int EM_SETCUEBANNER = 0x1501;
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, text);
        }
    }
}
