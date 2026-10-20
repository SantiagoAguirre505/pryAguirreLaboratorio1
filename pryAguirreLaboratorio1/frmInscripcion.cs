using System.Globalization;
using System.Text;

namespace pryAguirreLaboratorio1;

public sealed class frmInscripcion : Form
{
    private const decimal PRECIO_MUSCULACION = 15000m;
    private const decimal PRECIO_FUNCIONAL = 18000m;
    private const decimal PRECIO_NATACION = 22000m;
    private const decimal PRECIO_CASILLERO = 3000m;
    private const decimal DESCUENTO_MENOR = 0.25m;
    private const decimal DESCUENTO_MAYOR = 0.30m;
    private const decimal DESCUENTO_ESTUDIANTE = 0.15m;
    private const decimal DESCUENTO_EFECTIVO = 0.10m;
    private const decimal RECARGO_TRES_CUOTAS = 0.10m;
    private const decimal RECARGO_SEIS_CUOTAS = 0.20m;
    private const int EDAD_MINIMA = 14;
    private const int MESES_MINIMOS = 1;
    private const int MESES_MAXIMOS = 12;
    private const int CAPACIDAD_SOCIOS = 100;
    private const char SEPARADOR_ARCHIVO = '|';

    private readonly CultureInfo culturaMoneda = CultureInfo.GetCultureInfo("es-AR");
    private readonly SOCIO[] socios = new SOCIO[CAPACIDAD_SOCIOS];
    private readonly string rutaArchivoSocios = Path.Combine(AppContext.BaseDirectory, "Datos", "socios.txt");
    private int cantidadSocios;

    private TextBox txtNombre = null!;
    private TextBox txtEdad = null!;
    private CheckBox chkEstudiante = null!;
    private ComboBox cboPlan = null!;
    private ComboBox cboTurno = null!;
    private TextBox txtMeses = null!;
    private CheckBox chkCasillero = null!;
    private RadioButton rbtEfectivo = null!;
    private RadioButton rbtTarjeta = null!;
    private ComboBox cboCuotas = null!;
    private Button btnCalcular = null!;
    private Button btnLimpiar = null!;
    private TextBox txtBusqueda = null!;
    private Button btnBuscar = null!;

    public frmInscripcion()
    {
        InitializeComponent();
        Load += (_, _) =>
        {
            CargarSocios();
            EstadoInicial();
        };
    }

    private void InitializeComponent()
    {
        Text = "Gimnasio Siglo | Gestión de socios";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimumSize = new Size(760, 700);
        ClientSize = new Size(760, 700);
        Font = new Font("Segoe UI", 10F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(245, 247, 250);

        var datos = new GroupBox { Text = "  Datos personales  ", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var datosLayout = CrearLayout(4);
        txtNombre = CrearTexto(30);
        txtEdad = CrearTexto(3);
        chkEstudiante = new CheckBox { Name = "chkEstudiante", Text = "Estudiante", AutoSize = true };
        datosLayout.Controls.Add(new Label { Text = "Nombre:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
        datosLayout.Controls.Add(txtNombre, 1, 0);
        datosLayout.Controls.Add(new Label { Text = "Edad:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
        datosLayout.Controls.Add(txtEdad, 1, 1);
        datosLayout.Controls.Add(chkEstudiante, 1, 2);
        datos.Controls.Add(datosLayout);

        var plan = new GroupBox { Text = "  Plan y asistencia  ", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var planLayout = CrearLayout(4);
        cboPlan = CrearCombo("cboPlan", "Musculación", "Funcional", "Natación");
        cboTurno = CrearCombo("cboTurno", "Mañana", "Tarde", "Noche");
        txtMeses = CrearTexto(2);
        chkCasillero = new CheckBox { Name = "chkCasillero", Text = "Casillero ($ 3.000/mes)", AutoSize = true };
        planLayout.Controls.Add(new Label { Text = "Plan:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
        planLayout.Controls.Add(cboPlan, 1, 0);
        planLayout.Controls.Add(new Label { Text = "Turno:", Anchor = AnchorStyles.Left, AutoSize = true }, 2, 0);
        planLayout.Controls.Add(cboTurno, 3, 0);
        planLayout.Controls.Add(new Label { Text = "Meses:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
        planLayout.Controls.Add(txtMeses, 1, 1);
        planLayout.Controls.Add(chkCasillero, 1, 2);
        plan.Controls.Add(planLayout);

        var pago = new GroupBox { Text = "  Forma de pago  ", Dock = DockStyle.Fill, Padding = new Padding(12) };
        var pagoLayout = CrearLayout(4);
        rbtEfectivo = new RadioButton { Name = "rbtEfectivo", Text = "Efectivo", AutoSize = true };
        rbtTarjeta = new RadioButton { Name = "rbtTarjeta", Text = "Tarjeta", AutoSize = true };
        cboCuotas = CrearCombo("cboCuotas", "1", "3", "6");
        pagoLayout.Controls.Add(rbtEfectivo, 0, 0);
        pagoLayout.Controls.Add(rbtTarjeta, 1, 0);
        pagoLayout.Controls.Add(new Label { Text = "Cuotas:", Anchor = AnchorStyles.Left, AutoSize = true }, 2, 0);
        pagoLayout.Controls.Add(cboCuotas, 3, 0);
        pago.Controls.Add(pagoLayout);

        var busqueda = new GroupBox { Text = "  Buscar socio guardado  ", Dock = DockStyle.Fill, Padding = new Padding(25) };
        var busquedaLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
        busquedaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        busquedaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        busquedaLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        txtBusqueda = new TextBox { Name = "txtBusqueda", Dock = DockStyle.Fill, MaxLength = 100 };
        btnBuscar = new Button { Name = "btnBuscar", Text = "&Buscar", AutoSize = true, Anchor = AnchorStyles.None };
        busquedaLayout.Controls.Add(new Label { Text = "Dato de referencia:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
        busquedaLayout.Controls.Add(txtBusqueda, 1, 0);
        busquedaLayout.Controls.Add(btnBuscar, 2, 0);
        busqueda.Controls.Add(busquedaLayout);

        btnCalcular = new Button { Name = "btnCalcular", Text = "  Calcular inscripción  ", AutoSize = true, Anchor = AnchorStyles.None };
        btnLimpiar = new Button { Name = "btnLimpiar", Text = "  Limpiar formulario  ", AutoSize = true, Anchor = AnchorStyles.None };
        var botones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 10, 0, 0) };
        botones.Controls.Add(btnLimpiar);
        botones.Controls.Add(btnCalcular);

        var encabezado = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(24, 48, 85), Padding = new Padding(25, 12, 15, 8) };
        encabezado.Controls.Add(new Label { Text = "GIMNASIO SIGLO", ForeColor = Color.White, Font = new Font("Segoe UI", 18F, FontStyle.Bold), Dock = DockStyle.Top, AutoSize = true });
        encabezado.Controls.Add(new Label { Text = "Administración de inscripciones y socios", ForeColor = Color.FromArgb(190, 220, 238), Font = new Font("Segoe UI", 10F), Dock = DockStyle.Top, AutoSize = true });

        var tabInscripcion = new TabPage("  Nueva inscripción  ") { BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(10) };
        var inscripcionLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
        inscripcionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        inscripcionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        inscripcionLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
        inscripcionLayout.Controls.Add(datos, 0, 0);
        inscripcionLayout.Controls.Add(plan, 0, 1);
        inscripcionLayout.Controls.Add(pago, 0, 2);
        tabInscripcion.Controls.Add(inscripcionLayout);

        var tabBusqueda = new TabPage("  Consultar socios  ") { BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(10) };
        tabBusqueda.Controls.Add(busqueda);
        var tabs = new TabControl { Dock = DockStyle.Fill, Name = "tabPrincipal" };
        tabs.TabPages.Add(tabInscripcion);
        tabs.TabPages.Add(tabBusqueda);

        var principal = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(15), ColumnCount = 1, RowCount = 3 };
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 72));
        principal.RowStyles.Add(new RowStyle(SizeType.Percent, 13));
        principal.Controls.Add(encabezado, 0, 0);
        principal.Controls.Add(tabs, 0, 1);
        principal.Controls.Add(botones, 0, 2);
        Controls.Add(principal);
        AcceptButton = btnCalcular;
        AplicarTema(this);

        txtNombre.TextChanged += (_, _) => { txtNombre.Text = txtNombre.Text.ToUpper(); txtNombre.SelectionStart = txtNombre.Text.Length; ActualizarEstadoBoton(); };
        txtEdad.TextChanged += (_, _) => ActualizarEstadoBoton();
        txtMeses.TextChanged += (_, _) => ActualizarEstadoBoton();
        rbtEfectivo.CheckedChanged += (_, _) => ActualizarEstadoPago();
        rbtTarjeta.CheckedChanged += (_, _) => ActualizarEstadoPago();
        btnCalcular.Click += BtnCalcular_Click;
        btnLimpiar.Click += (_, _) => EstadoInicial();
        btnBuscar.Click += BtnBuscar_Click;
        txtEdad.KeyPress += SoloDigitos_KeyPress;
        txtMeses.KeyPress += SoloDigitos_KeyPress;
    }

    private static void AplicarTema(Control contenedor)
    {
        var azul = Color.FromArgb(24, 48, 85);
        var turquesa = Color.FromArgb(0, 150, 170);

        foreach (Control control in contenedor.Controls)
        {
            if (control is GroupBox grupo)
            {
                grupo.ForeColor = azul;
                grupo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            }
            else if (control is Button boton)
            {
                boton.BackColor = boton.Name == "btnCalcular" ? turquesa : Color.FromArgb(225, 232, 240);
                boton.ForeColor = boton.Name == "btnCalcular" ? Color.White : azul;
                boton.FlatStyle = FlatStyle.Flat;
                boton.FlatAppearance.BorderSize = 0;
                boton.Padding = new Padding(12, 7, 12, 7);
                boton.Cursor = Cursors.Hand;
            }
            else if (control is TextBox texto)
            {
                texto.BackColor = Color.White;
                texto.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is ComboBox combo)
            {
                combo.BackColor = Color.White;
                combo.ForeColor = azul;
            }
            else if (control is TabControl pestañas)
            {
                pestañas.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            }

            if (control.HasChildren) AplicarTema(control);
        }
    }

    private static TableLayoutPanel CrearLayout(int columnas)
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = columnas, RowCount = 3, AutoSize = true };
        for (var i = 0; i < columnas; i++) layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columnas));
        for (var i = 0; i < 3; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
        return layout;
    }

    private static TextBox CrearTexto(int maxLength) => new() { MaxLength = maxLength, Dock = DockStyle.Fill };

    private static ComboBox CrearCombo(string name, params string[] items)
    {
        var combo = new ComboBox { Name = name, DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        combo.Items.AddRange(items);
        return combo;
    }

    private void EstadoInicial()
    {
        txtNombre.Clear();
        txtEdad.Clear();
        txtMeses.Text = "1";
        chkEstudiante.Checked = false;
        chkCasillero.Checked = false;
        cboPlan.SelectedIndex = 0;
        cboTurno.SelectedIndex = 0;
        rbtEfectivo.Checked = true;
        cboCuotas.SelectedIndex = -1;
        cboCuotas.Enabled = false;
        btnCalcular.Enabled = false;
        txtNombre.Focus();
    }

    private void CargarSocios()
    {
        cantidadSocios = 0;
        if (!File.Exists(rutaArchivoSocios)) return;

        foreach (var linea in File.ReadLines(rutaArchivoSocios, Encoding.UTF8))
        {
            var datos = linea.Split(SEPARADOR_ARCHIVO);
            if (datos.Length != 11 || cantidadSocios >= CAPACIDAD_SOCIOS) continue;
            if (!int.TryParse(datos[1], out var edad) || !int.TryParse(datos[5], out var meses) ||
                !decimal.TryParse(datos[7], NumberStyles.Number, CultureInfo.InvariantCulture, out var total) ||
                !decimal.TryParse(datos[8], NumberStyles.Number, CultureInfo.InvariantCulture, out var valorCuota)) continue;

            socios[cantidadSocios++] = new SOCIO
            {
                Nombre = datos[0], Edad = edad, Categoria = datos[2], Plan = datos[3], Horario = datos[4],
                Meses = meses, FormaPago = datos[6], Total = total, ValorCuota = valorCuota,
                Estudiante = bool.TryParse(datos[9], out var estudiante) && estudiante,
                Casillero = bool.TryParse(datos[10], out var casillero) && casillero
            };
        }
    }

    private void GuardarSocios()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(rutaArchivoSocios)!);
        using var archivo = new StreamWriter(rutaArchivoSocios, false, Encoding.UTF8);
        for (var i = 0; i < cantidadSocios; i++)
        {
            var socio = socios[i];
            archivo.WriteLine(string.Join(SEPARADOR_ARCHIVO, socio.Nombre, socio.Edad, socio.Categoria, socio.Plan,
                socio.Horario, socio.Meses, socio.FormaPago, socio.Total.ToString(CultureInfo.InvariantCulture),
                socio.ValorCuota.ToString(CultureInfo.InvariantCulture), socio.Estudiante, socio.Casillero));
        }
    }

    private bool AgregarSocio(SOCIO socio)
    {
        if (cantidadSocios >= CAPACIDAD_SOCIOS) return false;
        socios[cantidadSocios++] = socio;
        GuardarSocios();
        return true;
    }

    private void BtnBuscar_Click(object? sender, EventArgs e)
    {
        var referencia = txtBusqueda.Text.Trim();
        if (referencia.Length == 0)
        {
            MessageBox.Show("Ingresá un nombre, edad, plan, turno, meses o forma de pago.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtBusqueda.Focus();
            return;
        }

        var resultado = new StringBuilder();
        for (var i = 0; i < cantidadSocios; i++)
        {
            var socio = socios[i];
            var textoSocio = string.Join(" ", socio.Nombre, socio.Edad, socio.Categoria, socio.Plan, socio.Horario,
                socio.Meses, socio.FormaPago, socio.Total.ToString("C", culturaMoneda), socio.ValorCuota.ToString("C", culturaMoneda));
            if (textoSocio.Contains(referencia, StringComparison.OrdinalIgnoreCase))
            {
                resultado.AppendLine($"{socio.Nombre} | {socio.Plan} | {socio.FormaPago} | Total: {socio.Total.ToString("C", culturaMoneda)}");
            }
        }

        MessageBox.Show(resultado.Length == 0 ? "No se encontraron socios." : resultado.ToString(),
            "Resultado de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ActualizarEstadoPago()
    {
        cboCuotas.Enabled = rbtTarjeta.Checked;
        if (rbtTarjeta.Checked && cboCuotas.SelectedIndex < 0) cboCuotas.SelectedIndex = 0;
        if (rbtEfectivo.Checked) cboCuotas.SelectedIndex = -1;
    }

    private void ActualizarEstadoBoton()
    {
        btnCalcular.Enabled = !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                              !string.IsNullOrWhiteSpace(txtEdad.Text) &&
                              !string.IsNullOrWhiteSpace(txtMeses.Text);
    }

    private static void SoloDigitos_KeyPress(object? sender, KeyPressEventArgs e)
    {
        e.Handled = !char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back;
    }

    private void BtnCalcular_Click(object? sender, EventArgs e)
    {
        if (!int.TryParse(txtEdad.Text, out var edad) || edad < EDAD_MINIMA)
        {
            MessageBox.Show("El socio debe tener al menos 14 años.", "Edad no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(txtMeses.Text, out var meses) || meses < MESES_MINIMOS || meses > MESES_MAXIMOS)
        {
            MessageBox.Show("La cantidad de meses debe estar entre 1 y 12.", "Meses no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var plan = cboPlan.Text;
        var precioPlan = plan switch
        {
            "Musculación" => PRECIO_MUSCULACION,
            "Funcional" => PRECIO_FUNCIONAL,
            "Natación" => PRECIO_NATACION,
            _ => 0m
        };
        var horario = cboTurno.Text switch
        {
            "Mañana" => "7 a 12 h",
            "Tarde" => "14 a 18 h",
            "Noche" => "18 a 23 h",
            _ => string.Empty
        };

        var precioMensual = precioPlan;
        if (chkCasillero.Checked) precioMensual += PRECIO_CASILLERO;
        var subtotal = precioMensual * meses;
        decimal descuento;
        if (edad < 18)
        {
            descuento = DESCUENTO_MENOR;
        }
        else
        {
            if (edad >= 65) descuento = DESCUENTO_MAYOR;
            else if (chkEstudiante.Checked) descuento = DESCUENTO_ESTUDIANTE;
            else descuento = 0m;
        }

        var importeConDescuento = subtotal * (1m - descuento);
        var cuotas = rbtTarjeta.Checked ? int.Parse(cboCuotas.Text) : 1;
        decimal total;
        if (rbtEfectivo.Checked)
        {
            total = importeConDescuento * (1m - DESCUENTO_EFECTIVO);
        }
        else
        {
            var recargo = cuotas switch
            {
                1 => 0m,
                3 => RECARGO_TRES_CUOTAS,
                6 => RECARGO_SEIS_CUOTAS,
                _ => 0m
            };
            total = importeConDescuento * (1m + recargo);
        }

        var socio = new SOCIO
        {
            Nombre = txtNombre.Text,
            Edad = edad,
            Categoria = edad < 18 ? "Menor" : "Mayor",
            Plan = plan,
            Horario = horario,
            Meses = meses,
            FormaPago = rbtEfectivo.Checked ? "Efectivo" : $"Tarjeta en {cuotas} cuota(s)",
            Total = total,
            ValorCuota = rbtEfectivo.Checked ? total : total / cuotas,
            Estudiante = chkEstudiante.Checked,
            Casillero = chkCasillero.Checked
        };

        if (!AgregarSocio(socio))
        {
            MessageBox.Show("Se alcanzó el límite de 100 socios guardados.", "Capacidad completa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var detalle = $"Socio: {socio.Nombre}\nEdad: {socio.Edad} ({socio.Categoria})\nPlan: {socio.Plan}\nTurno: {socio.Horario}\nMeses: {socio.Meses}\nForma de pago: {socio.FormaPago}\nSubtotal: {subtotal.ToString("C", culturaMoneda)}\nDescuento aplicado: {(descuento * 100):0}%\nTotal: {socio.Total.ToString("C", culturaMoneda)}\nValor de cuota: {socio.ValorCuota.ToString("C", culturaMoneda)}";
        MessageBox.Show(detalle, "Inscripción calculada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        EstadoInicial();
    }

    private struct SOCIO
    {
        public string Nombre;
        public int Edad;
        public string Categoria;
        public string Plan;
        public string Horario;
        public int Meses;
        public string FormaPago;
        public decimal Total;
        public decimal ValorCuota;
        public bool Estudiante;
        public bool Casillero;
    }
}
