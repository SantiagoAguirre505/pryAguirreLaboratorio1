namespace pryAguirreLaboratorio1;

public sealed class frmRuleta : Form
{
    private readonly string[] candidatos;
    private readonly Random random = new();
    private readonly Label lblResultado;
    private readonly Button btnGirar;
    private readonly System.Windows.Forms.Timer timer;
    private int girosRestantes;

    public string? MozoSeleccionado { get; private set; }

    public frmRuleta(string[] candidatos)
    {
        this.candidatos = candidatos;
        Text = "Ruleta del empate";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(430, 240);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("Segoe UI", 10F);

        var titulo = new Label
        {
            Text = "La ruleta definirá el mozo del día",
            Dock = DockStyle.Top,
            Height = 55,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 13F, FontStyle.Bold)
        };

        lblResultado = new Label
        {
            Text = "Presioná GIRAR",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 22F, FontStyle.Bold)
        };

        btnGirar = new Button
        {
            Text = "GIRAR",
            AutoSize = true,
            MinimumSize = new Size(120, 35),
            Anchor = AnchorStyles.None
        };
        btnGirar.Click += BtnGirar_Click;

        timer = new System.Windows.Forms.Timer { Interval = 100 };
        timer.Tick += Timer_Tick;

        var panelBoton = new Panel { Dock = DockStyle.Bottom, Height = 55 };
        panelBoton.Controls.Add(btnGirar);
        btnGirar.Location = new Point((panelBoton.Width - btnGirar.Width) / 2, 10);
        panelBoton.Resize += (_, _) => btnGirar.Left = (panelBoton.ClientSize.Width - btnGirar.Width) / 2;

        Controls.Add(lblResultado);
        Controls.Add(panelBoton);
        Controls.Add(titulo);
    }

    private void BtnGirar_Click(object? sender, EventArgs e)
    {
        if (candidatos.Length == 0) return;
        btnGirar.Enabled = false;
        girosRestantes = 20;
        timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        lblResultado.Text = candidatos[random.Next(candidatos.Length)];
        girosRestantes--;
        if (girosRestantes > 0) return;

        timer.Stop();
        MozoSeleccionado = lblResultado.Text;
        MessageBox.Show($"La ruleta eligió a {MozoSeleccionado}.", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        btnGirar.Enabled = true;
    }
}
