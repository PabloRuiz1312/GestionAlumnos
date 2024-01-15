using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GestionAlumnos
{
    public partial class Form1 : Form
    {
        OleDbDataAdapter oleAdapter;
        OleDbCommand oleCommand;
        OleDbCommandBuilder oleBuilder;
        DataSet dataSet;
        DataTable table;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'practicaDataSet1.Evaluaciones' Puede moverla o quitarla según sea necesario.
            this.evaluacionesTableAdapter1.Fill(this.practicaDataSet1.Evaluaciones);
            // TODO: esta línea de código carga datos en la tabla 'practicaDataSet.Evaluaciones' Puede moverla o quitarla según sea necesario.
            this.evaluacionesTableAdapter.Fill(this.practicaDataSet.Evaluaciones);
            // TODO: esta línea de código carga datos en la tabla 'alumnosDataSet.Alumnos' Puede moverla o quitarla según sea necesario.
            //this.alumnosTableAdapter1.Fill(this.alumnosDataSet.Alumnos);
            eliminarPaneles();
            cargarPanelMenu();
        }

        private void cargarPanelMenu()
        {
            panel1.Visible = true; 
        }

        private void cargarPanelInsAlumnos()
        {
            panel2.Visible = true;
        }

        private void cargarSelectAlumnos()
        {
            panel3.Visible = true;
            this.cargarAlumnos();
        }
        private void eliminarPaneles()
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            panel6.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            eliminarPaneles();
            cargarPanelMenu();
        }

        private void altaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eliminarPaneles();
            cargarPanelInsAlumnos();
        }

        private void listarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.alumnosTableAdapter1.Fill(this.alumnosDataSet.Alumnos);
            eliminarPaneles();
            cargarSelectAlumnos();
            botonGuardar.Enabled = false;
            botonBorrar.Enabled = true;
            botonModificar.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String textoNombre = campoNombre.Text;
            String textoApellido = campoApellido.Text;
            String textoDni = campoDni.Text;
            Boolean baja = campoBaja.Checked;
            textoNombre = textoNombre.Trim();
            textoApellido = textoApellido.Trim();
            textoDni = textoDni.Trim();
            if (textoNombre.Equals("") || textoApellido.Equals("") || textoDni.Equals(""))
            {
                logError.Text = "Error los campos no pueden estar vacios";
            }
            else
            {
                insertarAlumnos(textoNombre, textoApellido,textoDni,baja);
                campoNombre.Text = "";
                campoApellido.Text = "";
                campoDni.Text = "";
                campoBaja.Checked = false;
            }
        }

        private void insertarAlumnos(String textoNombre,String textoApellido,String textoDni,Boolean baja)
        {
            string connetionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=|DataDirectory|\\practica.accdb";
            string sentencia = "Insert into Alumnos (Nombre,Apellidos,NIF,Baja) values ('" + textoNombre + "'," + "'" + textoApellido + "'," + "'" + textoDni + "'," + baja + ")";
            OleDbConnection connection;
            OleDbCommand command;
            connection = new OleDbConnection(connetionString);
            try
            {
                connection.Open();
                command = new OleDbCommand(sentencia, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                MessageBox.Show("Alumno " + textoNombre + " " + textoApellido + " dado de alta");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! " + ex.ToString());
            }
        }

        private void cargarAlumnos()
        {
            String connetionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=|DataDirectory|\\practica.accdb";
            string sentencia = "select * FROM Alumnos";
            OleDbConnection connection;
            connection = new OleDbConnection(connetionString);
            try
            {
                connection.Open();
                this.oleCommand = new OleDbCommand(sentencia,connection);
                this.oleAdapter = new OleDbDataAdapter(this.oleCommand);
                this.oleBuilder = new OleDbCommandBuilder(this.oleAdapter);
                this.dataSet = new DataSet();
                this.oleAdapter.Fill(dataSet,"Alumnos");
                this.table = dataSet.Tables["Alumnos"];
                connection.Close();
                dataGridView1.DataSource = dataSet.Tables["Alumnos"];
                dataGridView1.ReadOnly = true;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Can not open the connection ! "+ ex.ToString());
            }
        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.alumnosTableAdapter1.Fill(this.alumnosDataSet.Alumnos);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void botonBorrar_Click(object sender, EventArgs e)
        {   
            try
            {
                Object item = dataGridView1.SelectedRows[0].Index;
                if (MessageBox.Show("¿Seguro que quieres borrar esta fila?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    oleAdapter.Update(table);
                    cargarAlumnos();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("No se ha seleccionado ningun item");
            }
        }

        private void botonModificar_Click(object sender, EventArgs e)
        {
            dataGridView1.ReadOnly = false;
            botonGuardar.Enabled = true;
            botonModificar.Enabled = false;
            botonBorrar.Enabled = false;
        }

        private void botonGuardar_Click(object sender, EventArgs e)
        {
            oleAdapter.Update(table);
            cargarAlumnos();
            dataGridView1.ReadOnly = true;
            botonModificar.Enabled = true;
            botonBorrar.Enabled = true;
            botonGuardar.Enabled = false;
        }

        private void altaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            eliminarPaneles();
            cargarPanelInsEvau();
        }

        private void cargarPanelInsEvau()
        {
            panel4.Visible = true;
        }

        private void botonEvauGuardar_Click(object sender, EventArgs e)
        {
            String descripcion = campoDescripcion.Text;
            descripcion = descripcion.Trim();
            if (descripcion.Equals(""))
            {
                logError2.Text = "La descripcion no puede estar vacia";
            }
            else if(descripcion.Length>255)
            {
                logError2.Text = "El maximo de caracteres permitidos son 255";
            }
            else
            {
                this.insertarEvaluacion(descripcion);
                campoDescripcion.Text = "";
            }

        }

        private void insertarEvaluacion(String descripcion)
        {
            string connetionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=|DataDirectory|\\practica.accdb";
            string sentencia = "Insert into Evaluaciones (Evaluacion) values ('" + descripcion + "')";
            OleDbConnection connection;
            OleDbCommand command;
            connection = new OleDbConnection(connetionString);
            try
            {
                connection.Open();
                command = new OleDbCommand(sentencia, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                MessageBox.Show("Evaluacion dada de alta");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! " + ex.ToString());
            }
        }

        private void cargarEvaluaciones()
        {
            String connetionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=|DataDirectory|\\practica.accdb";
            string sentencia = "select * FROM Evaluaciones";
            OleDbConnection connection;
            connection = new OleDbConnection(connetionString);
            try
            {
                connection.Open();
                this.oleCommand = new OleDbCommand(sentencia, connection);
                this.oleAdapter = new OleDbDataAdapter(this.oleCommand);
                this.oleBuilder = new OleDbCommandBuilder(this.oleAdapter);
                this.dataSet = new DataSet();
                this.oleAdapter.Fill(dataSet, "Evaluaciones");
                this.table = dataSet.Tables["Evaluaciones"];
                connection.Close();
                dataGridView2.DataSource = dataSet.Tables["Evaluaciones"];
                dataGridView2.ReadOnly = true;
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open the connection ! " + ex.ToString());
            }
        }

        private void listarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            eliminarPaneles();
            cargarSelectEvaluaciones();
        }

        private void cargarSelectEvaluaciones()
        {
           panel5.Visible = true;
            cargarEvaluaciones();
        }

        private void botonModificar2_Click(object sender, EventArgs e)
        {
            dataGridView2.ReadOnly = false;
            botonGuardar2.Enabled = true;
            botonModificar2.Enabled = false;
            botonEliminar2.Enabled = false;
        }

        private void botonEliminar2_Click(object sender, EventArgs e)
        {
            try
            {
                Object item = dataGridView1.SelectedRows[0].Index;
                if (MessageBox.Show("¿Seguro que quieres borrar esta fila?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dataGridView2.Rows.RemoveAt(dataGridView2.SelectedRows[0].Index);
                    oleAdapter.Update(table);
                    cargarEvaluaciones();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se ha seleccionado ningun item");
            }
        }

        private void botonGuardar2_Click(object sender, EventArgs e)
        {
            oleAdapter.Update(table);
            cargarEvaluaciones();
            dataGridView2.ReadOnly = true;
            botonModificar2.Enabled = true;
            botonEliminar2.Enabled = true;
            botonGuardar2.Enabled = false;
        }

        private void consultarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eliminarPaneles();
            cargarListaEvauAlumnos();
        }

        private void cargarListaEvauAlumnos()
        {
            panel6.Visible = true;
        }

        private void mostrarTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (mostrarTodos.Checked)
            {
                comboEvau.Enabled = false;
                listAlumnos.Enabled = false;
            }
            else
            {
                comboEvau.Enabled = true;
                listAlumnos.Enabled = true;
            }
        }

        private void cargarComboBox()
        {
            string connetionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=|DataDirectory|\\gestion.accdb";
            string sentencia = "select Id,Evaluacion from Evaluaciones";
            OleDbConnection connection;
            OleDbCommand command;
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            connection = new OleDbConnection(connetionString);
            try
            {
                connection.Open();
                command = new OleDbCommand(sentencia, connection);
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                adapter.Dispose();
                command.Dispose();
                connection.Close();
                comboEvau.DataSource = ds.Tables[0];
                comboEvau.ValueMember = "Id";
                comboEvau.DisplayMember = "Evaluacion";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la conexion " + ex);
            }
        }
        private void comboEvau_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void cargarListBox()
        {
            string connetionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=|DataDirectory|\\gestion.accdb";
            string sentencia = "select concat(Nombre,\" \",Apellidos),Id from Alumnos from Alumnos";
            OleDbConnection connection;
            OleDbCommand command;
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            connection = new OleDbConnection(connetionString);
            List<String> listaNombres =  new List<string>();
            //OleDbDataReader reader = new OleDbDataReader();
            try
            {
                connection.Open();
                command = new OleDbCommand(sentencia, connection);
                adapter.SelectCommand = command;
                adapter.Fill(ds);
                adapter.Dispose();
                command.Dispose();
                connection.Close();
                //listBox1.DataSource = ds.Tables[0];
                //listBox1.ValueMember = "idProveedor";
                //listBox1.DisplayMember = "descripcion";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! " + ex.ToString());
            }
        }
    }
}
