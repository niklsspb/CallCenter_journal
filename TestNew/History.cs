/*
 * Сделано в SharpDevelop.
 * Пользователь: Lagutov_NA
 * Дата: 11.05.2016
 * Время: 11:25
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using sd=System.Data;
using MySql.Data.MySqlClient;

namespace TestNew
{
	/// <summary>
	/// Description of History.
	/// </summary>
	public partial class History : Form
	{
		public History()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//Fill();
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
				
		void TextBox1TextChanged(object sender, EventArgs e)
		{
			if (textBox1.Text.Length==7)
			{
			string sp=textBox1.Text.Remove(3);
			string spq=textBox1.Text.Remove(0,5);
			string spw=textBox1.Text.Remove(0,3);
			spw=spw.Remove(2);
			textBox2.Text=String.Concat(sp,"-",spw,"-",spq);
			}
			if (textBox1.Text.Length==10)
			{
			string sp=textBox1.Text.Remove(3);
			string spq=textBox1.Text.Remove(0,6);
			string spw=textBox1.Text.Remove(0,3);
			//string spz=textBox1.Text.Remove(0,7);
			spw=spw.Remove(3);
			string spz=spq.Remove(2);
			string spz2=spq.Remove(0,2);
			//spz=spz.Remove(6,2);
			textBox2.Text=String.Concat(sp,"-",spw,"-",spz,"-",spz2);
			}
			if (textBox1.Text.Length==9)
			{
			string sp=textBox1.Text.Remove(3);
			string spq=textBox1.Text.Remove(0,4);
			string spq3=spq.Remove(2);
			string spq2=spq.Remove(0,3);
			textBox2.Text=String.Concat(sp,spq3,spq2);
			}
			if (textBox1.Text.Length==13)
			{
			string sp=textBox1.Text.Remove(3);
			string spq=textBox1.Text.Remove(0,4);
			string spq2=spq.Remove(3);
			string spq3=spq.Remove(0,4);
			string spq4=spq3.Remove(2);
			string spq5=spq3.Remove(0,3);
			textBox2.Text=String.Concat(sp,spq2,spq4,spq5);
			
			}
			
			string cs = @"server="+ConfigurationManager.AppSettings["Srv"]+";userid=nikls;password=mitsumi;database="+ConfigurationManager.AppSettings["dbase"]+";convert zero datetime=True;charset=utf8";
			MySqlConnection conn = new MySqlConnection(cs);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            
            conn.Open();
            MySqlDataAdapter MyDA = new MySqlDataAdapter();
            MyDA.SelectCommand = new MySqlCommand("SELECT `id`, `date` AS 'Дата', `question` AS 'Вопрос', `array` AS 'Звонил(а)',`phone` AS 'Телефон',`who` AS 'Принял',`prim` AS 'Примечания' FROM `journal` where `phone`='"+textBox1.Text+"' or `phone`='"+textBox2.Text+"'  and `array`<>'"+""+"' and date between (SELECT DATE_SUB(CURDATE(),Interval 6 month)) and CURDATE() ORDER BY `id` DESC LIMIT 100", conn);
            sd.DataTable table = new sd.DataTable();
            
            MyDA.Fill(table);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = table;
            dataGridView1.DataSource = bSource; 
            conn.Close();
		}
		void DataGridView1CellClick(object sender, DataGridViewCellEventArgs e)
		{
		MainForm main = this.Owner as MainForm;
			if(main != null)
			{
			main.test.Text=dataGridView1.CurrentRow.Cells[3].Value.ToString();
			}
		}
	}
}
