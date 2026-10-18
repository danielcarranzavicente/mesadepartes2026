using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace mesadepartes2026
{
    public partial class Form1 : Form
    {
        private readonly List<Expediente> expedientes = new List<Expediente>();
        private readonly string archivo = Path.Combine(Application.StartupPath, "expedientes.txt");

        public Form1()
        {
            InitializeComponent();
            textBox2.MaxLength = 8;

            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = dataGridView1.AllowUserToDeleteRows = dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.Columns.Add("Codigo", "Código");
            dataGridView1.Columns.Add("Ciudadano", "Ciudadano");
            dataGridView1.Columns.Add("DNI", "DNI");
            dataGridView1.Columns.Add("Tramite", "Trámite");
            dataGridView1.Columns.Add("Descripcion", "Descripción");
            dataGridView1.Columns.Add("Fecha", "Fecha de registro");

            if (File.Exists(archivo))
            {
                string[] lineas = File.ReadAllLines(archivo, Encoding.UTF8);
                for (int i = 0; i < lineas.Length; i++)
                {
                    string[] d = lineas[i].Split('|');
                    if (d.Length == 6)
                    {
                        expedientes.Add(new Expediente { Codigo = d[0], Ciudadano = d[1], DNI = d[2], Tramite = d[3], Descripcion = d[4], Fecha = Convert.ToDateTime(d[5]) });
                    }
                }
            }
            MostrarTodo(expedientes);
        }

        private void MostrarTodo(List<Expediente> lista)
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < lista.Count; i++)
                dataGridView1.Rows.Add(lista[i].Codigo, lista[i].Ciudadano, lista[i].DNI, lista[i].Tramite, lista[i].Descripcion, lista[i].Fecha.ToString("dd/MM/yyyy HH:mm"));
        }

        private void Guardar()
        {
            List<string> lineas = new List<string>();
            for (int i = 0; i < expedientes.Count; i++)
                lineas.Add(expedientes[i].Codigo + "|" + expedientes[i].Ciudadano + "|" + expedientes[i].DNI + "|" + expedientes[i].Tramite + "|" + expedientes[i].Descripcion + "|" + expedientes[i].Fecha.ToString("yyyy-MM-dd HH:mm"));
            File.WriteAllLines(archivo, lineas, Encoding.UTF8);
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("Complete todos los campos.");
                return;
            }
            if (textBox2.Text.Length != 8)
            {
                MessageBox.Show("El DNI debe tener exactamente 8 dígitos.");
                return;
            }

            int mayor = 0;
            for (int i = 0; i < expedientes.Count; i++)
            {
                int num = Convert.ToInt32(expedientes[i].Codigo.Substring(4));
                if (num > mayor) mayor = num;
            }

            int nuevoNumero = mayor + 1;
            string cod = "EXP-" + nuevoNumero.ToString("D4");

            expedientes.Add(new Expediente { Codigo = cod, Ciudadano = textBox1.Text, DNI = textBox2.Text, Tramite = textBox3.Text, Descripcion = textBox4.Text, Fecha = DateTime.Now });
            Guardar();
            MostrarTodo(expedientes);
            MessageBox.Show("Registrado correctamente. Código: " + cod);
            textBox1.Clear(); textBox2.Clear(); textBox3.Clear(); textBox4.Clear(); textBox1.Focus();
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox1.Focus();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (expedientes.Count == 0)
            {
                MessageBox.Show("No hay expedientes para eliminar.");
                return;
            }
            if (MessageBox.Show("¿Eliminar todo?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                expedientes.Clear();
                Guardar();
                MostrarTodo(expedientes);
                textBox5.Clear();
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string codigoBuscar = textBox5.Text.Trim();
            if (codigoBuscar == "")
            {
                MessageBox.Show("Escriba el código del expediente.");
                return;
            }

            bool encontrado = false;
            for (int i = 0; i < expedientes.Count; i++)
            {
                if (expedientes[i].Codigo.ToLower() == codigoBuscar.ToLower())
                {
                    List<Expediente> resultado = new List<Expediente>();
                    resultado.Add(expedientes[i]);
                    MostrarTodo(resultado);
                    encontrado = true;
                    break;
                }
            }

            if (encontrado == false)
            {
                MessageBox.Show("No se encontró el expediente.");
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            MostrarTodo(expedientes);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < expedientes.Count - 1; i++)
            {
                for (int j = i + 1; j < expedientes.Count; j++)
                {
                    if (expedientes[i].Codigo.CompareTo(expedientes[j].Codigo) > 0)
                    {
                        Expediente temporal = expedientes[i];
                        expedientes[i] = expedientes[j];
                        expedientes[j] = temporal;
                    }
                }
            }
            MostrarTodo(expedientes);
        }
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