using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCatera
{
    public partial class FormHome: Form
    {
        public FormHome()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Mostrar el formulario de carga
            using (FormLoading splash = new FormLoading())
            {
                splash.Show();
                splash.Refresh();

                // Simula la carga de recursos pesados (puedes reemplazarlo con inicialización real)
                System.Threading.Thread.Sleep(2000);

                splash.Close();
            }

            // Abrir el formulario de consulta de cartera
            FormConsultarCartera consultarCarteraForm = new FormConsultarCartera();
            consultarCarteraForm.Show();
        }

        private void btnActualizarClientes_Click(object sender, EventArgs e)
        {
            // Mostrar el formulario de carga
            using (FormLoading splash = new FormLoading())
            {
                splash.Show();
                splash.Refresh();

                // Simula la carga de recursos pesados (puedes reemplazarlo con inicialización real)
                System.Threading.Thread.Sleep(2000);

                splash.Close();
            }

            // Abrir el formulario de actualización de clientes
            FormActualizarClientes actualizarClientesForm = new FormActualizarClientes();
            actualizarClientesForm.Show();
        }

        private void FormHome_Load(object sender, EventArgs e)
        {

        }
    }
    public partial class FormLoading : Form
    {
        public FormLoading()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // Sin bordes
            this.StartPosition = FormStartPosition.CenterScreen; // Centrado
            this.ControlBox = false; // Sin botón de cerrar
            this.Text = "Cargando...";
            Label label = new Label
            {
                Text = "Espere, cargando...",
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(label);
        }
    }
}
