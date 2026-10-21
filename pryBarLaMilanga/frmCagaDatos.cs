using System.Globalization;

namespace pryAguirreLaboratorio1;

public sealed class frmCagaDatos : Form
{
    private readonly string[] mozos = { "Julio", "Esteban", "Javier", "Gonzalo", "Alberto" };
    private readonly string[] categorias = { "Comidas", "Bebidas sin alcohol", "Bebidas con alcohol", "Postres" };
    private readonly float[,] ventas = new float[5, 4];

    private DataGridView grillaVentas = null!;
    private Button btnValidar = null!;
    private Button btnMozoDelDia = null!;
    private Button btnTotales = null!;
    private Button btnCerrar = null!;
    private Label lblMozoDelDia = null!;
    private Label lblImporteMozo = null!;
    private Label lblTotalGeneral = null!;
    private Label lblTotalComidas = null!;
    private Label lblTotalBebidasSinAlcohol = null!;
    private Label lblTotalBebidasConAlcohol = null!;
    private Label lblTotalPostres = null!;

    public frmCagaDatos()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Bar La Milanga | Registro de ventas";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(940, 650);
        MinimumSize = new Size(940, 650);
        Font = new Font("Segoe UI", 10F);
        AutoScaleMode = AutoScaleMode.Font;

        var titulo = new Label
        {
            Text = "Bar La Milanga - Ventas diarias",
            Dock = DockStyle.Top,
            Height = 55,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold)
        };

        grillaVentas = new DataGridView
        {
            Name = "grillaVentas",
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = false,
            RowHeadersVisible = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            EditMode = DataGridViewEditMode.EditOnEnter,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        };
        grillaVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Comidas", HeaderText = "Comidas", Width = 150 });
        grillaVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "BebidasSinAlcohol", HeaderText = "Bebidas sin alcohol", Width = 180 });
        grillaVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "BebidasConAlcohol", HeaderText = "Bebidas con alcohol", Width = 180 });
        grillaVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Postres", HeaderText = "Postres", Width = 150 });
        grillaVentas.Columns.Insert(0, new DataGridViewTextBoxColumn { Name = "Mozo", HeaderText = "Mozo", ReadOnly = true, Width = 130 });
        foreach (var mozo in mozos) grillaVentas.Rows.Add(mozo);

        var grupoCarga = new GroupBox { Text = "Carga de ventas", Dock = DockStyle.Fill, Padding = new Padding(10) };
        grupoCarga.Controls.Add(grillaVentas);

        btnValidar = CrearBoton("Validar Datos", BtnValidar_Click);
        btnMozoDelDia = CrearBoton("Mozo del día", BtnMozoDelDia_Click);
        btnTotales = CrearBoton("Totales", BtnTotales_Click);
        btnCerrar = CrearBoton("Cerrar", (_, _) => Close());
        btnMozoDelDia.Enabled = false;
        btnTotales.Enabled = false;

        var grupoMozo = new GroupBox { Text = "Mozo del día", Dock = DockStyle.Fill, Padding = new Padding(10) };
        lblMozoDelDia = CrearResultado();
        lblImporteMozo = CrearResultado();
        grupoMozo.Controls.Add(CrearPanelResultados(
            ("Mozo:", lblMozoDelDia),
            ("Importe:", lblImporteMozo)));

        var grupoTotales = new GroupBox { Text = "Totales", Dock = DockStyle.Fill, Padding = new Padding(10) };
        lblTotalGeneral = CrearResultado();
        lblTotalComidas = CrearResultado();
        lblTotalBebidasSinAlcohol = CrearResultado();
        lblTotalBebidasConAlcohol = CrearResultado();
        lblTotalPostres = CrearResultado();
        grupoTotales.Controls.Add(CrearPanelResultados(
            ("Total general:", lblTotalGeneral),
            ("Comidas:", lblTotalComidas),
            ("Bebidas sin alcohol:", lblTotalBebidasSinAlcohol),
            ("Bebidas con alcohol:", lblTotalBebidasConAlcohol),
            ("Postres:", lblTotalPostres)));

        var resultados = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(0, 10, 0, 0) };
        resultados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        resultados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        resultados.Controls.Add(grupoMozo, 0, 0);
        resultados.Controls.Add(grupoTotales, 1, 0);

        var botones = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 55, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) };
        botones.Controls.Add(btnCerrar);
        botones.Controls.Add(btnTotales);
        botones.Controls.Add(btnMozoDelDia);
        botones.Controls.Add(btnValidar);

        var contenido = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1, Padding = new Padding(15) };
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        contenido.Controls.Add(titulo, 0, 0);
        contenido.Controls.Add(grupoCarga, 0, 1);
        contenido.Controls.Add(resultados, 0, 2);
        Controls.Add(contenido);
        Controls.Add(botones);
    }

    private static Button CrearBoton(string texto, EventHandler evento)
    {
        var boton = new Button { Text = texto, AutoSize = true, MinimumSize = new Size(125, 32) };
        boton.Click += evento;
        return boton;
    }

    private static Label CrearResultado() => new() { Text = "-", AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };

    private static TableLayoutPanel CrearPanelResultados(params (string Texto, Label Resultado)[] filas)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = filas.Length };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        for (var fila = 0; fila < filas.Length; fila++)
        {
            panel.Controls.Add(new Label { Text = filas[fila].Texto, AutoSize = true, Anchor = AnchorStyles.Left }, 0, fila);
            panel.Controls.Add(filas[fila].Resultado, 1, fila);
        }
        return panel;
    }

    private void BtnValidar_Click(object? sender, EventArgs e)
    {
        btnMozoDelDia.Enabled = false;
        btnTotales.Enabled = false;

        for (var fila = 0; fila < mozos.Length; fila++)
        {
            for (var columna = 0; columna < categorias.Length; columna++)
            {
                var celda = grillaVentas.Rows[fila].Cells[columna + 1];
                if (!TryParseImporte(celda.Value?.ToString(), out var importe) || importe < 0)
                {
                    MessageBox.Show($"El importe de {categorias[columna]} para {mozos[fila]} debe ser un número mayor o igual a cero.", "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    grillaVentas.CurrentCell = celda;
                    grillaVentas.Focus();
                    return;
                }
                ventas[fila, columna] = importe;
            }
        }

        btnMozoDelDia.Enabled = true;
        btnTotales.Enabled = true;
        MessageBox.Show("Los datos fueron validados correctamente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnMozoDelDia_Click(object? sender, EventArgs e)
    {
        var totales = ObtenerTotalesPorMozo();
        var mayor = totales.Max();
        var empatados = mozos.Where((_, indice) => totales[indice] == mayor).ToArray();

        if (empatados.Length > 1)
        {
            MessageBox.Show("Hay un empate entre los mozos con mayor venta. Se abrirá la ruleta para definir el resultado.", "Empate", MessageBoxButtons.OK, MessageBoxIcon.Information);
            using var ruleta = new frmRuleta(empatados);
            ruleta.ShowDialog(this);
            lblMozoDelDia.Text = ruleta.MozoSeleccionado ?? "Sin selección";
        }
        else
        {
            lblMozoDelDia.Text = empatados[0];
        }

        lblImporteMozo.Text = FormatearImporte(mayor);
    }

    private void BtnTotales_Click(object? sender, EventArgs e)
    {
        var totalGeneral = 0f;
        var totalesCategoria = new float[categorias.Length];
        for (var fila = 0; fila < mozos.Length; fila++)
        {
            for (var columna = 0; columna < categorias.Length; columna++)
            {
                totalGeneral += ventas[fila, columna];
                totalesCategoria[columna] += ventas[fila, columna];
            }
        }

        lblTotalGeneral.Text = FormatearImporte(totalGeneral);
        lblTotalComidas.Text = FormatearImporte(totalesCategoria[0]);
        lblTotalBebidasSinAlcohol.Text = FormatearImporte(totalesCategoria[1]);
        lblTotalBebidasConAlcohol.Text = FormatearImporte(totalesCategoria[2]);
        lblTotalPostres.Text = FormatearImporte(totalesCategoria[3]);
    }

    private float[] ObtenerTotalesPorMozo()
    {
        var totales = new float[mozos.Length];
        for (var fila = 0; fila < mozos.Length; fila++)
            for (var columna = 0; columna < categorias.Length; columna++)
                totales[fila] += ventas[fila, columna];
        return totales;
    }

    private static bool TryParseImporte(string? texto, out float importe)
    {
        return float.TryParse(texto, NumberStyles.Float, CultureInfo.CurrentCulture, out importe) ||
               float.TryParse(texto, NumberStyles.Float, CultureInfo.InvariantCulture, out importe);
    }

    private static string FormatearImporte(float importe) => importe.ToString("C2", CultureInfo.CurrentCulture);
}
