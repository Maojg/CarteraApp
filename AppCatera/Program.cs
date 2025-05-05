using System;
using System.Threading;
using System.Windows.Forms;

namespace AppCatera
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Mostrar el formulario de carga
            using (FormLoading splash = new FormLoading())
            {
                splash.Show();
                splash.Refresh();

                // Simula la carga de recursos pesados (puedes reemplazarlo con inicialización real)
                System.Threading.Thread.Sleep(3000);

                splash.Close();
            }

            // Iniciar el formulario principal
            Application.Run(new FormHome());
        }
    }
}
