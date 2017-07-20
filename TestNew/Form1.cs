/*
 * Создано в SharpDevelop.
 * Пользователь: Lagutov_NA
 * Дата: 14.09.2015
 * Время: 13:39
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TestNew
{
	/// <summary>
	/// Description of Form1.
	/// </summary>
	public partial class Form1 : Form
	{
		public Form1()
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
			
		}
		void Form1Load(object sender, EventArgs e)
		{
	MainForm fmain =new MainForm();
			string cs = @"server="+ConfigurationManager.AppSettings["Srv"]+";userid=USER;password=PASSWORD;database=NAMEDATABASE;convert zero datetime=True;charset=utf8";
	        MySqlConnection conn = new MySqlConnection(cs);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            // 
            conn.Open();
            MySqlDataAdapter MyDA = new MySqlDataAdapter();
            MyDA.SelectCommand = new MySqlCommand("SELECT *  FROM `journal` WHERE `date` = '"+fmain.dateTimePicker3.Value.ToString("yyyy/MM/dd")+"' and `who`='"+fmain.comboBox4.Text+"' ORDER BY id DESC limit 1", conn);
            DataTable table = new DataTable();
            MyDA.Fill(table);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = table;
            dataGridView1.DataSource = bSource; 
            conn.Close();
		}
	}
}
