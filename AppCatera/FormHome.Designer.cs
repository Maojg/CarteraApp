namespace AppCatera
{
    partial class FormHome
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();

            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height = 70;
            this.headerPanel.Controls.Add(this.titleLabel);

            // 
            // titleLabel
            // 
            this.titleLabel.Text = "Gestión de Cartera - Jaltech";
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(20, 20);

            // 
            // button1 (Consultar Cartera)
            // 
            this.button1.Text = "Consultar Cartera";
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.button1.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Size = new System.Drawing.Size(200, 50);
            this.button1.Location = new System.Drawing.Point(300, 150);
            this.button1.Click += new System.EventHandler(this.button1_Click);

            // 
            // button2 (Actualizar Clientes)
            // 
            this.button2.Text = "Actualizar Clientes";
            this.button2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.button2.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Size = new System.Drawing.Size(200, 50);
            this.button2.Location = new System.Drawing.Point(300, 230);
            this.button2.Click += new System.EventHandler(this.btnActualizarClientes_Click);

            // 
            // FormHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(244, 246, 247);
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Name = "FormHome";
            this.Text = "Panel Principal";
            this.Load += new System.EventHandler(this.FormHome_Load);
        }

        #endregion
    }
}
