/*
 * Создано в SharpDevelop.
 * Пользователь: Lagutov_NA
 * Дата: 13.10.2015
 * Время: 9:40
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace TestNew
{
	/// <summary>
	/// Description of Form2.
	/// </summary>
	public partial class Form2 : Form
	{
		public Form2()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void Button1Click(object sender, EventArgs e)
		{
		MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=nikls;Password=mitsumi;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				
				
				//textBox1.Text=dateTimePicker1.Value.ToShortDateString();
				//textBox1.Text=dateTimePicker1.Value.ToShortTimeString();
				connection.Open();
				string sql = "UPDATE `"+ConfigurationManager.AppSettings["dbase"]+"`.`journal` SET `date`='"+dateTimePicker1.Value.ToString("yyyy/MM/dd")+"', `question`='"+comboBox1.Text+"',`array`='"+textBox1.Text+"',`phone`='"+textBox2.Text+"',`who`='"+comboBox2.Text+"',`prim`='"+prim.Text+"' where `id`='"+textBox3.Text+"'";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				Form2.ActiveForm.Close();
				MessageBox.Show("Данные внесены");
		}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		void Form2Load(object sender, EventArgs e)
		{
	MySqlConnection mycon = new MySqlConnection("datasource="+ConfigurationManager.AppSettings["Srv"]+";username=nikls;password=mitsumi;database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8");
//create a mysql DataAdapter
string selectstring="SELECT `name` from `users` order by name ASC"; //station is a string
MySqlDataAdapter myadp = new MySqlDataAdapter(selectstring, mycon);
//create a dataset
DataSet myds = new DataSet();
//now fill and bind the DataGrid
myadp.Fill(myds, "box");
this.comboBox2.DataSource = myds.Tables["box"].DefaultView;
this.comboBox2.DisplayMember = "name";
this.comboBox2.ValueMember = "name";

MySqlConnection mycon2 = new MySqlConnection("datasource="+ConfigurationManager.AppSettings["Srv"]+";username=nikls;password=mitsumi;database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8");
//create a mysql DataAdapter
string selectstring2="SELECT `name` from `answers`"; //station is a string
MySqlDataAdapter myadp2 = new MySqlDataAdapter(selectstring2, mycon2);
//create a dataset
DataSet myds2 = new DataSet();
//now fill and bind the DataGrid
myadp2.Fill(myds2, "box3");
this.comboBox1.DataSource = myds2.Tables["box3"].DefaultView;
this.comboBox1.DisplayMember = "name";
this.comboBox1.ValueMember = "name";  
		}
	}
}
