using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mesadepartes2026
{
    public partial class Form1 : Form
    {
        private readonly List<Expediente> expedientes = new List<Expediente>();

        private readonly string archivo = Path.Combine(
            Application.StartupPath, "expedientes.txt");

        public Form1()
        {
            InitializeComponent();

            textBox2.MaxLength = 8;

            ConfigurarTabla();
            ConectarBotones();
            CargarExpedientes();
            MostrarExpedientes(expedientes);
        }

        private void ConfigurarTabla()
        {
            // Las columnas se crean aquí para evitar que falten.
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Columns.Add("Codigo", "Código");
            dataGridView1.Columns.Add("Ciudadano", "Ciudadano");
            dataGridView1.Columns.Add("DNI", "DNI");
            dataGridView1.Columns.Add("Tramite", "Trámite");
            dataGridView1.Columns.Add("Descripcion", "Descripción");
            dataGridView1.Columns.Add("Fecha", "Fecha de registro");
        }

        private void ConectarBotones()
        {
            // Evitar eventos duplicados.
            button1.Click -= btnRegistrar_Click;
            button1.Click += btnRegistrar_Click;

            button2.Click -= button2_Click;
            button2.Click += button2_Click;

            button3.Click -= button3_Click;
            button3.Click += button3_Click;

            button4.Click -= button4_Click;
            button4.Click += button4_Click;

            button5.Click -= button5_Click;
            button5.Click += button5_Click;

            button6.Click -= button6_Click;
            button6.Click += button6_Click;
        }

        private void RegistrarExpediente()
        {
            string ciudadano = textBox1.Text.Trim();
            string dni = textBox2.Text.Trim();
            string tramite = textBox3.Text.Trim();
            string descripcion = textBox4.Text.Trim();

            if (string.IsNullOrWhiteSpace(ciudadano) ||
                string.IsNullOrWhiteSpace(dni) ||
                string.IsNullOrWhiteSpace(tramite) ||
                string.IsNullOrWhiteSpace(descripcion))
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }

            if (dni.Length != 8 || !dni.All(char.IsDigit))
            {
                MessageBox.Show("El DNI debe tener exactamente 8 dígitos.");
                return;
            }

            if (new[] { ciudadano, dni, tramite, descripcion }
                .Any(dato => dato.Contains("|")))
            {
                MessageBox.Show("No utilice el símbolo | en los datos.");
                return;
            }

            int mayor = 0;

            foreach (Expediente exp in expedientes)
            {
                int numero;

                if (exp.Codigo.StartsWith("EXP-") &&
                    int.TryParse(exp.Codigo.Substring(4), out numero))
                {
                    if (numero > mayor)
                        mayor = numero;
                }
            }

            if (mayor == int.MaxValue)
            {
                MessageBox.Show("No se pueden generar más códigos.");
                return;
            }

            Expediente nuevo = new Expediente
            {
                Codigo = "EXP-" + (mayor + 1).ToString("D4"),
                Ciudadano = ciudadano,
                DNI = dni,
                Tramite = tramite,
                Descripcion = descripcion,
                Fecha = DateTime.Now
            };

            expedientes.Add(nuevo);

            if (!GuardarExpedientes())
            {
                expedientes.Remove(nuevo);
                return;
            }

            MostrarExpedientes(expedientes);

            MessageBox.Show(
                "Expediente registrado correctamente.\nCódigo: " +
                nuevo.Codigo);

            LimpiarCampos();
        }

        private void MostrarExpedientes(List<Expediente> lista)
        {
            dataGridView1.Rows.Clear();

            foreach (Expediente exp in lista)
            {
                dataGridView1.Rows.Add(
                    exp.Codigo,
                    exp.Ciudadano,
                    exp.DNI,
                    exp.Tramite,
                    exp.Descripcion,
                    exp.Fecha.ToString("dd/MM/yyyy HH:mm"));
            }
        }

        private void BuscarExpediente()
        {
            string codigo = textBox5.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigo))
            {
                MessageBox.Show("Escriba el código del expediente.");
                textBox5.Focus();
                return;
            }

            Expediente encontrado = expedientes.Find(
                exp => string.Equals(
                    exp.Codigo,
                    codigo,
                    StringComparison.OrdinalIgnoreCase));

            if (encontrado == null)
            {
                dataGridView1.Rows.Clear();
                MessageBox.Show("No se encontró el expediente.");
                return;
            }

            MostrarExpedientes(new List<Expediente> { encontrado });
        }

        private void LimpiarCampos()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox1.Focus();
        }

        private void EliminarTodosExpedientes()
        {
            if (expedientes.Count == 0)
            {
                MessageBox.Show("No hay expedientes para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar todos los expedientes?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            List<Expediente> copia = new List<Expediente>(expedientes);
            expedientes.Clear();

            if (GuardarExpedientes())
            {
                MostrarExpedientes(expedientes);
                LimpiarCampos();
                textBox5.Clear();

                MessageBox.Show("Todos los expedientes fueron eliminados.");
            }
            else
            {
                expedientes.AddRange(copia);
                MostrarExpedientes(expedientes);
            }
        }

        private bool GuardarExpedientes()
        {
            try
            {
                List<string> lineas = new List<string>();

                foreach (Expediente exp in expedientes)
                {
                    lineas.Add(string.Join("|",
                        exp.Codigo,
                        exp.Ciudadano,
                        exp.DNI,
                        exp.Tramite,
                        exp.Descripcion,
                        exp.Fecha.ToString(
                            "yyyy-MM-dd HH:mm",
                            CultureInfo.InvariantCulture)));
                }

                File.WriteAllLines(archivo, lineas, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar los expedientes:\n" + ex.Message);
                return false;
            }
        }

        private void CargarExpedientes()
        {
            if (!File.Exists(archivo))
                return;

            try
            {
                foreach (string linea in File.ReadAllLines(archivo, Encoding.UTF8))
                {
                    string[] datos = linea.Split('|');

                    if (datos.Length != 6 ||
                        datos[2].Length != 8 ||
                        !datos[2].All(char.IsDigit))
                        continue;

                    DateTime fecha;

                    if (!DateTime.TryParse(
                        datos[5],
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out fecha) &&
                        !DateTime.TryParse(datos[5], out fecha))
                        continue;

                    if (expedientes.Exists(exp => exp.Codigo == datos[0]))
                        continue;

                    expedientes.Add(new Expediente
                    {
                        Codigo = datos[0],
                        Ciudadano = datos[1],
                        DNI = datos[2],
                        Tramite = datos[3],
                        Descripcion = datos[4],
                        Fecha = fecha
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los expedientes:\n" + ex.Message);
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            RegistrarExpediente();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EliminarTodosExpedientes();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BuscarExpediente();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MostrarExpedientes(expedientes);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            BuscarExpediente();
        }

        private void label1_Click(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private void dataGridView1_CellContentClick(
            object sender, DataGridViewCellEventArgs e)
        { }
    }

    public class Expediente
    {
        public string Codigo { get; set; }
        public string Ciudadano { get; set; }
        public string DNI { get; set; }
        public string Tramite { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }
}