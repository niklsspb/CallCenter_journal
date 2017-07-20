/*
 * Создано в SharpDevelop.
 * Пользователь: Lagutov_NA
 * Дата: 07.09.2015
 * Время: 14:25
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using MySql.Data.MySqlClient;
using sd = System.Data;
namespace TestNew
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		System.Collections.Generic.List<DateFind> data = new System.Collections.Generic.List< DateFind>();
		System.Collections.Generic.List<DateFind> select = new System.Collections.Generic.List<DateFind>();
		System.Collections.Generic.List<AutoCall> inputvalue = new System.Collections.Generic.List<AutoCall>();
		System.Collections.Generic.List<AutoCall> findedvalue = new System.Collections.Generic.List<AutoCall>();
		Regex correctNumber = new Regex(@"^\d{3}-\d{2}-\d{2}$");
			Regex correctmobile = new Regex(@"^[0-9]{3}-[0-9]{3}-[0-9]{2}-[0-9]{2}$");
		public MainForm()
		{
			//m
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Init()
        {
			data.Clear();
			MySqlConnection conn=new MySqlConnection("datasource="+ConfigurationManager.AppSettings["Srv"]+";username=USER;password=PASSWORD;database="+ConfigurationManager.AppSettings["dbase"]+";convert zero datetime=True;");
			try
			{
				conn.Open();
				string query="select id,date,array,phone,prim from journal where date between (SELECT DATE_SUB(CURDATE(),Interval 2 month)) and CURDATE()";
				MySqlCommand cmd = new MySqlCommand(query,conn);
				MySqlDataReader dr=cmd.ExecuteReader();
				
				while (dr.Read())
				{
					int id=dr.GetInt32(0);
					DateTime date=dr.GetDateTime(1);
					string array=dr.GetString(2);
					string phone=dr.GetString(3);
					string prim=dr.GetString(4);
					data.Add(new DateFind() { phone=phone,array=array,date=date,ID=id,prim=prim});
					
				}
				conn.Close();
				timer1.Interval=60000;
				timer1.Enabled=true;
				timer1.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			
          }
		void LoadAutoCall()
        {
			inputvalue.Clear();
			MySqlConnection conn=new MySqlConnection("datasource="+ConfigurationManager.AppSettings["Srv"]+";username=USER;password=PASSWORD;database="+ConfigurationManager.AppSettings["dbase"]+";convert zero datetime=True;");
			try
			{
				conn.Open();
				string query="select REGNUM,phone,MAX(dateload) from dolg2 group by regnum";
				MySqlCommand cmd = new MySqlCommand(query,conn);
				MySqlDataReader dr=cmd.ExecuteReader();
				
				while (dr.Read())
				{
					string regnum=dr.GetString(0);
					string phone=dr.GetString(1);
					DateTime date=dr.GetDateTime(2);
					inputvalue.Add(new AutoCall() { regnumber=regnum,phone=phone,date=date});
					
				}
				conn.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			
          }
		
		void Button1Click(object sender, EventArgs e)
		{
			if (comboBox1.Text=="")
			{
				MessageBox.Show("Введите добавочный абонента");
			}
			else
			{
				
			
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+"";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`one` (`id`, `number`, `date`, `time`) VALUES (NULL,'"+comboBox1.Text+"' , '"+dateTimePicker1.Value.ToString("yyyy/MM/dd")+"', '"+dateTimePicker1.Value.ToShortTimeString()+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=System.Drawing.Color.Green;
				statusStrip1.Items[0].Text="Добавочный номер " +comboBox1.Text+ " внесен";
				comboBox1.Text="";
			}
			catch (MySqlException ex)
			{
				statusStrip1.Items[0].ForeColor=System.Drawing.Color.Red;
				statusStrip1.Items[0].Text="Ошибка";
				MessageBox.Show(ex.ToString());
			}
}

		}

		void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			//throw new NotImplementedException();
		}
		private void Download()
{
	try
	{
		XmlDocument doc = new XmlDocument();
		doc.Load(@"http://"+ConfigurationManager.AppSettings["Srv"]+"/appname/version.xml");

		var remoteVersion = new Version(doc.GetElementsByTagName("version")[0].InnerText);
		var localVersion = new Version(Application.ProductVersion);

		if (localVersion < remoteVersion)
		{
			if (File.Exists("TestNew.update")) { File.Delete("TestNew.update"); }

			WebClient client = new WebClient();
			WebClient client2 = new WebClient();
			client2.DownloadFileAsync(new Uri(@"http://"+ConfigurationManager.AppSettings["Srv"]+"/appname/TestNew.exe.config"), "TestNew.exe.config.update");
			client.DownloadFileAsync(new Uri(@"http://"+ConfigurationManager.AppSettings["Srv"]+"/appname/TestNew.exe"), "TestNew.update");
			client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
		}
	}
	catch (Exception) { }
}
		public void checkUpdates(){
	try
	{
		if (File.Exists("TestNew.update") && new Version(FileVersionInfo.GetVersionInfo("TestNew.update").FileVersion) > new Version(Application.ProductVersion))
		{
			File.Replace("TestNew.update","TestNew.exe","TestNew.bak");
			if (File.Exists("TestNew.exe.config.update"))
			    {
			File.Replace("TestNew.exe.config.update","TestNew.exe.config","TestNew_config.bak");
			    }
			else
			{
				
			}
		}
		else
		{
			if (File.Exists("TestNew.update")) { File.Delete("TestNew.update"); }
			Download();
		}
	}
	catch (Exception)
	{
		if (File.Exists("TestNew.update")) { File.Delete("TestNew.update"); }
		Download();
	}
}

		void Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			System.IO.File.Replace("TestNew.update","TestNew.exe","TestNew.bak");
			System.IO.File.Replace("TestNew.exe.config.update","TestNew.exe.config","TestNew_config.bak");
			Process.Start("TestNew.exe");
			Process.GetCurrentProcess().Kill();
		}

		void MainFormLoad(object sender, EventArgs e)
		{
		checkUpdates();
		Init();
		LoadAutoCall();
		label15.Text=SystemInformation.ComputerName;
		ToolStripMenuItem FioMenuItem = new ToolStripMenuItem("ФИО");
        ToolStripMenuItem SnilsMenuItem = new ToolStripMenuItem("Снилс");
        ToolStripMenuItem RegNumMenuItem = new ToolStripMenuItem("Рег.№");
        // добавляем элементы в меню
        contextMenuStrip1.Items.AddRange(new[] { FioMenuItem, SnilsMenuItem,RegNumMenuItem, });
        // ассоциируем контекстное меню с текстовым полем
        FioMenuItem.Click += FioMenuItem_Click;
        SnilsMenuItem.Click += SnilsMenuItem_Click;
        RegNumMenuItem.Click += RegNumMenuItem_Click;
        ToolStripMenuItem MobilePhone = new ToolStripMenuItem("Мобильный");
        ToolStripMenuItem HomePhone = new ToolStripMenuItem("Домашний");
         // добавляем элементы в меню
        contextMenuStrip2.Items.AddRange(new[] { MobilePhone, HomePhone});
        // ассоциируем контекстное меню с текстовым полем
        maskedTextBox1.ContextMenuStrip = contextMenuStrip2;
        MobilePhone.Click += MobilePhone_Click;
        HomePhone.Click += HomePhone_Click;   

        //заполняем ComboBox 2
MySqlConnection mycon = new MySqlConnection("datasource="+ConfigurationManager.AppSettings["Srv"]+";username=USER;password=PASSWORD;database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8");
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
comboBox2.Text="";


MySqlConnection mycon2 = new MySqlConnection("datasource="+ConfigurationManager.AppSettings["Srv"]+";username=USER;password=PASSWORD;database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8");
//create a mysql DataAdapter
string selectstring2="SELECT `name` from `answers`"; //station is a string
MySqlDataAdapter myadp2 = new MySqlDataAdapter(selectstring2, mycon2);
//create a dataset
DataSet myds2 = new DataSet();
//now fill and bind the DataGrid
myadp2.Fill(myds2, "box3");
this.comboBox3.DataSource = myds2.Tables["box3"].DefaultView;
this.comboBox3.DisplayMember = "name";
this.comboBox3.ValueMember = "name";  
comboBox3.Text="";

				//заполняем ComboBox 4				
				MySqlConnection mycon1 = new MySqlConnection("datasource="+ConfigurationManager.AppSettings["Srv"]+";username=USER;password=PASSWORD;database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8");
				//create a mysql DataAdapter
				string selectstring1="SELECT `name` from `users` order by name ASC"; //station is a string
				MySqlDataAdapter myadp1 = new MySqlDataAdapter(selectstring1, mycon1);
				//create a dataset
				DataSet myds1 = new DataSet();
				//now fill and bind the DataGrid
				myadp1.Fill(myds1, "boxx");
				this.comboBox4.DataSource = myds1.Tables["boxx"].DefaultView;
				this.comboBox4.DisplayMember = "name";
				this.comboBox4.ValueMember = "name"; 
				comboBox4.Text="";		
		}
	 void FioMenuItem_Click(object sender, EventArgs e)
    {
	 	maskedTextBox2.Mask="";
    }
		
	 void MobilePhone_Click(object sender, EventArgs e)
    {
	 	maskedTextBox1.Mask="+7(999) 000-00-00";
    }
	 void HomePhone_Click(object sender, EventArgs e)
    {
	 	maskedTextBox1.Mask="000-00-00";
    }
	 void SnilsMenuItem_Click(object sender, EventArgs e)
    {

    }
	 void RegNumMenuItem_Click(object sender, EventArgs e)
    {

    }
		void Button2Click(object sender, EventArgs e)
		{
			if (comboBox3.Text=="" || test.Text==""&&noTest.Checked==false || maskedTextBox1.Text==""&&noPhone.Checked==false || comboBox4.Text=="")
			{
				
				MessageBox.Show("Не заполнены поля! Проверьте поле ФИО Сотрудника если все остальные поля заполнены");
			}
			else
			{
				if(maskedTextBox1.Text.Length>6&&maskedTextBox1.Text.Length<10&&correctNumber.IsMatch(maskedTextBox1.Text)==false || maskedTextBox1.Text.Length>9&&correctmobile.IsMatch(maskedTextBox1.Text)==false)
				{
				MessageBox.Show("Номер телефона указан в неверном формате");	
				}
				else{
					string sql;
					MySqlScript script;
			
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
							
				
				
				if(noPhone.Checked==true&&noTest.Checked==false&&maskedTextBox1.Text.Length==0&&test.Text.Length>0)
				{
					maskedTextBox1.Text="Отказ назвать номер";
					sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`journal` (`id`, `date`, `question`, `array`, `phone`,`who`,`time`,`comp_name`,`prim`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+comboBox3.Text+"', '"+test.Text+"', '"+maskedTextBox1.Text+"','"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"','"+richTextBox1.Text+"')";
					script = new MySqlScript(connection, sql);
					script.Execute();
					comboBox3.Text="";
				maskedTextBox1.Mask="";
				test.Text="";
				test.Items.Clear();
				richTextBox1.Text="";
				maskedTextBox1.Text="";
				maskedTextBox2.Mask="";
				maskedTextBox2.Text="";
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Данные успешно записаны";
				
				}
				else
				{
					
				}
				if(noTest.Checked==true&&noPhone.Checked==false&&maskedTextBox1.Text.Length>6&&maskedTextBox1.Text.Length<10&&correctNumber.IsMatch(maskedTextBox1.Text)==false||noTest.Checked==true&&maskedTextBox1.Text.Length>9&&correctmobile.IsMatch(maskedTextBox1.Text)==false)
				{
					test.Text="Отказ назвать свои данные";
					sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`journal` (`id`, `date`, `question`, `array`, `phone`,`who`,`time`,`comp_name`,`prim`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+comboBox3.Text+"', '"+test.Text+"', '"+maskedTextBox1.Text+"','"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"','"+richTextBox1.Text+"')";
					script = new MySqlScript(connection, sql);
					script.Execute();
					comboBox3.Text="";
				maskedTextBox1.Mask="";
				test.Text="";
				test.Items.Clear();
				richTextBox1.Text="";
				maskedTextBox1.Text="";
				maskedTextBox2.Mask="";
				maskedTextBox2.Text="";
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Данные успешно записаны";
				}
				
				
				if(noTest.Checked==true&&noPhone.Checked==true)
				{
					test.Text="Отказ назвать свои данные";
					maskedTextBox1.Text="Отказ назвать номер";
					sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`journal` (`id`, `date`, `question`, `array`, `phone`,`who`,`time`,`comp_name`,`prim`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+comboBox3.Text+"', '"+test.Text+"', '"+maskedTextBox1.Text+"','"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"','"+richTextBox1.Text+"')";
					script = new MySqlScript(connection, sql);
					script.Execute();
					comboBox3.Text="";
				maskedTextBox1.Mask="";
				test.Text="";
				test.Items.Clear();
				richTextBox1.Text="";
				maskedTextBox1.Text="";
				maskedTextBox2.Mask="";
				maskedTextBox2.Text="";
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Данные успешно записаны";
				}
				
				if(noTest.Checked==false&&noPhone.Checked==false&&test.Text!=""&&test.Text!="Отказ назвать свои данные"&&maskedTextBox1.Text.Length>6&&maskedTextBox1.Text.Length<10&&correctNumber.IsMatch(maskedTextBox1.Text)||maskedTextBox1.Text.Length>9&&correctmobile.IsMatch(maskedTextBox1.Text))
				{
					sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`journal` (`id`, `date`, `question`, `array`, `phone`,`who`,`time`,`comp_name`,`prim`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+comboBox3.Text+"', '"+test.Text+"', '"+maskedTextBox1.Text+"','"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"','"+richTextBox1.Text+"')";
					script = new MySqlScript(connection, sql);
					script.Execute();
					comboBox3.Text="";
				maskedTextBox1.Mask="";
				test.Text="";
				test.Items.Clear();
				richTextBox1.Text="";
				maskedTextBox1.Text="";
				maskedTextBox2.Mask="";
				maskedTextBox2.Text="";
				noPhone.Checked=false;
				noTest.Checked=false;
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Данные успешно записаны";
				}
				connection.Close();
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
			}
		}
		void Button3Click(object sender, EventArgs e)
		{
			if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', 'Общий информационный', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button3.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button5Click(object sender, EventArgs e)
		{
			if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button5.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button5.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button4Click(object sender, EventArgs e)
		{
			if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button4.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button4.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button6Click(object sender, EventArgs e)
		{
			if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', 'Пос', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button7Click(object sender, EventArgs e)
		{
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				//собираем 
				string sqltsr1 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='теоретический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comtsr1 = new MySqlCommand(sqltsr1,connection);
				string sqltsr2 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='практический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comtsr2 = new MySqlCommand(sqltsr2,connection);
				string sqltsr3 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='жалоба' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comtsr3 = new MySqlCommand(sqltsr3,connection);
				string sqltsr1shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` =' теоретический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comtsr1shared = new MySqlCommand(sqltsr1shared,connection);
				string sqltsr2shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` =' практический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comtsr2shared = new MySqlCommand(sqltsr2shared,connection);
				string sqltsr3shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` =' жалоба' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comtsr3shared = new MySqlCommand(sqltsr3shared,connection);
				//собираем 
				string sqlcur1 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='теоретический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comcur1 = new MySqlCommand(sqlcur1,connection);
				string sqlcur2 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='практический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comcur2 = new MySqlCommand(sqlcur2,connection);
				string sqlcur3 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='жалоба' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comcur3 = new MySqlCommand(sqlcur3,connection);
				string sqlcur1shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='теоретический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comcur1shared = new MySqlCommand(sqlcur1shared,connection);
				string sqlcur2shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='практический' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comcur2shared = new MySqlCommand(sqlcur2shared,connection);																 
				string sqlcur3shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='жалоба' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comcur3shared = new MySqlCommand(sqlcur3shared,connection);
				//собираем общие
				string sqlobsh1 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='Общий информационный' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comobsh1 = new MySqlCommand(sqlobsh1,connection);
				string sqlobsh2 = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='Деятельность ' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comobsh2 = new MySqlCommand(sqlobsh2,connection);
				string sqlobsh1shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='Общий информационный' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comobsh1shared = new MySqlCommand(sqlobsh1shared,connection);
				string sqlobsh2shared = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` ='Деятельность ' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand comobsh2shared = new MySqlCommand(sqlobsh2shared,connection);
				//считаем Пос
				string posob = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` = 'Пос' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand posobcom = new MySqlCommand(posob,connection);
				string posob2 = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` = 'Пос' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand posobcom2 = new MySqlCommand(posob2,connection);
				//считаем Проф
				string prof = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` = 'Проф' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand profcom = new MySqlCommand(prof,connection);
				string prof2 = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` = 'Проф' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand profcom2 = new MySqlCommand(prof2,connection);
				
				//считаем непрофильные вопросы
				string notprof = "SELECT COUNT(*) FROM `journal` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` = 'Непрофильный вопрос' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand notprofcom = new MySqlCommand(notprof,connection);
				string notprof2 = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` = 'Непрофильный вопрос' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand notprofcom2 = new MySqlCommand(notprof2,connection);
				//эл.секретарь
				string secret = "SELECT COUNT(1) FROM `shared` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `question` = 'секретарь' AND `who` = '"+comboBox2.Text+"'";
				MySqlCommand secret2 = new MySqlCommand(secret,connection);
				
				//считаем переводы
				string four = "SELECT COUNT(1) FROM `perevod` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `name_fil` LIKE 'филиал' AND `who` LIKE '"+comboBox2.Text+"'";
				MySqlCommand fourfi = new MySqlCommand(four,connection);
				
				string seven = "SELECT COUNT(1) FROM `perevod` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `name_fil` LIKE 'филиал' AND `who` LIKE '"+comboBox2.Text+"'";
				MySqlCommand sevenfi = new MySqlCommand(seven,connection);
				
				string ten = "SELECT COUNT(1) FROM `perevod` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `name_fil` LIKE 'филиал' AND `who` LIKE '"+comboBox2.Text+"'";
				MySqlCommand tenfi = new MySqlCommand(ten,connection);
				
				string twelve = "SELECT COUNT(1) FROM `perevod` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `name_fil` LIKE 'филиал' AND `who` LIKE '"+comboBox2.Text+"'";
				MySqlCommand twelvefi = new MySqlCommand(twelve,connection);
				
				string fifteen = "SELECT COUNT(1) FROM `perevod` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `name_fil` LIKE 'филиал' AND `who` LIKE '"+comboBox2.Text+"'";
				MySqlCommand fifteenfi = new MySqlCommand(fifteen,connection);
				
				string twentynine = "SELECT COUNT(1) FROM `perevod` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `name_fil` LIKE 'филиал' AND `who` LIKE '"+comboBox2.Text+"'";
				MySqlCommand twentyninefi = new MySqlCommand(twentynine,connection);
				
				string thirty = "SELECT COUNT(1) FROM `perevod` WHERE `date` = '"+dateTimePicker2.Value.ToString("yyyy/MM/dd")+"' AND `name_fil` LIKE 'филиал' AND `who` LIKE '"+comboBox2.Text+"'";
				MySqlCommand thirtyfi = new MySqlCommand(thirty,connection);
				
				//MySqlScript script = new MySqlScript(connection, sql);
				int summtsr1=Convert.ToInt32(comtsr1.ExecuteScalar()); //из таблицы Journal
				int summtsr2=Convert.ToInt32(comtsr2.ExecuteScalar()); //из таблицы Journal
				int summtsr3=Convert.ToInt32(comtsr3.ExecuteScalar()); //из таблицы Journal
				int summcur1=Convert.ToInt32(comcur1.ExecuteScalar()); //из таблицы Journal
				int summcur2=Convert.ToInt32(comcur2.ExecuteScalar());//из таблицы Journal
				int summcur3=Convert.ToInt32(comcur3.ExecuteScalar());//из таблицы Journal
				int summobsh1=Convert.ToInt32(comobsh1.ExecuteScalar());
				int summobsh2=Convert.ToInt32(comobsh2.ExecuteScalar());
				int summposob=Convert.ToInt32(posobcom.ExecuteScalar());
				int summposobshared=Convert.ToInt32(posobcom2.ExecuteScalar());
				int summprof=Convert.ToInt32(profcom.ExecuteScalar());
				int summprofshared=Convert.ToInt32(profcom2.ExecuteScalar());
				int summnotprof=Convert.ToInt32(notprofcom.ExecuteScalar());
				int summnotprofshared=Convert.ToInt32(notprofcom2.ExecuteScalar());
								
				int summtsr1shared=Convert.ToInt32(comtsr1shared.ExecuteScalar()); //из таблицы shared
				int summtsr2shared=Convert.ToInt32(comtsr2shared.ExecuteScalar()); //из таблицы shared
				int summtsr3shared=Convert.ToInt32(comtsr3shared.ExecuteScalar()); //из таблицы shared
				int summcur1shared=Convert.ToInt32(comcur1shared.ExecuteScalar()); //из таблицы shared
				int summcur2shared=Convert.ToInt32(comcur2shared.ExecuteScalar());
				int summcur3shared=Convert.ToInt32(comcur3shared.ExecuteScalar());
				int summobsh1shared=Convert.ToInt32(comobsh1shared.ExecuteScalar());
				int summobsh2shared=Convert.ToInt32(comobsh2shared.ExecuteScalar());
				int sumsecret=Convert.ToInt32(secret2.ExecuteScalar());
				
				fourfi.ExecuteScalar(); //из таблицы shared
				sevenfi.ExecuteScalar(); //из таблицы shared
				tenfi.ExecuteScalar(); //из таблицы shared
				twelvefi.ExecuteScalar(); //из таблицы shared
				fifteenfi.ExecuteScalar(); //из таблицы shared
				twentyninefi.ExecuteScalar(); //из таблицы shared
				thirtyfi.ExecuteScalar(); //из таблицы shared
				
				int summtsr1all=summtsr1+summtsr1shared;
				string summtsr1allstring=Convert.ToString(summtsr1all);
				
				int summtsr2all=summtsr2+summtsr2shared;
				string summtsr2allstring=Convert.ToString(summtsr2all);
				
				int summtsr3all=summtsr3+summtsr3shared;
				string summtsr3allstring=Convert.ToString(summtsr3all);
				
				int summcur1all=summcur1+summcur1shared;
				string summcur1allstring=Convert.ToString(summcur1all);
				
				int summcur2all=summcur2+summcur2shared;
				string summcur2allstring=Convert.ToString(summcur2all);
				
				int summcur3all=summcur3+summcur3shared;
				string summcur3allstring=Convert.ToString(summcur3all);
				
				int summobsh1all=summobsh1+summobsh1shared;
				string summobsh1allstring=Convert.ToString(summobsh1all);
				int summobsh2all=summobsh2+summobsh2shared;
				string summobsh2allstring=Convert.ToString(summobsh2all);
				
				int summposoball=summposob+summposobshared;
				string summposoballallstring=Convert.ToString(summposoball);
				
				int summnprofall=summprof+summprofshared;
				string summprofallallstring=Convert.ToString(summnprofall);
				
				int summnotprofall=summnotprof+summnotprofshared;
				string summnotprofallallstring=Convert.ToString(summnotprofall);
				
				string sumsecretall=Convert.ToString(sumsecret);
				
				int fourfil=Convert.ToInt32(fourfi.ExecuteScalar());
				int sevenfil=Convert.ToInt32(sevenfi.ExecuteScalar());
				int tenfil=Convert.ToInt32(tenfi.ExecuteScalar());
				int twelvefil=Convert.ToInt32(twelvefi.ExecuteScalar());
				int fifteenfil=Convert.ToInt32(fifteenfi.ExecuteScalar());
				int twentyninefil=Convert.ToInt32(twentyninefi.ExecuteScalar());
				int thirtyfil=Convert.ToInt32(thirtyfi.ExecuteScalar());
							
				connection.Close();
				//
				label7.Text=summtsr1allstring;
				label33.Text=summtsr2allstring;
				label34.Text=summtsr3allstring;
				//
				label36.Text=summcur1allstring;
				label38.Text=summcur2allstring;
				label40.Text=summcur3allstring;
				//общие
				label8.Text=summobsh1allstring;
				label42.Text=summobsh2allstring;
				//проф
				label10.Text=summprofallallstring;
				//непрофильные вопросы
				label12.Text=summnotprofallallstring;
				//Пос
				label11.Text=summposoballallstring;
				//секретарь
				label46.Text=sumsecretall;
				//переводы в филиалы
				label27.Text=Convert.ToString(fourfil);
				label26.Text=Convert.ToString(sevenfil);
				label25.Text=Convert.ToString(tenfil);
				label23.Text=Convert.ToString(twelvefil);
				
				label31.Text=Convert.ToString(fifteenfil);
				label30.Text=Convert.ToString(twentyninefil);
				label29.Text=Convert.ToString(thirtyfil);
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			  
		}
		void TabControl1Click(object sender, EventArgs e)
		{
	if(comboBox4.Text!="")
				{
	string cs = @"server="+ConfigurationManager.AppSettings["Srv"]+";userid=USER;password=PASSWORD;database="+ConfigurationManager.AppSettings["dbase"]+";convert zero datetime=True;charset=utf8";
			MySqlConnection conn = new MySqlConnection(cs);
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            // 
            conn.Open();
            MySqlDataAdapter MyDA = new MySqlDataAdapter();
            MyDA.SelectCommand = new MySqlCommand("SELECT `id`, `date`, `question`, `array` ,`phone` ,`who`,`prim` FROM `journal` where `date`='"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"' AND `who`='"+comboBox4.Text+"'ORDER BY `id` DESC", conn);
            sd.DataTable table = new sd.DataTable();
            //AS 'Дата'
            MyDA.Fill(table);
            BindingSource bSource = new BindingSource();
            bSource.DataSource = table;
            dataGridView1.DataSource = bSource; 
            conn.Close();
            

            dataGridView1.Columns["date"].HeaderText = "Дата";
            dataGridView1.Columns["question"].HeaderText ="Вопрос";
            dataGridView1.Columns["array"].HeaderText ="Звонил(а)";
            dataGridView1.Columns["phone"].HeaderText ="Телефон";
            dataGridView1.Columns["who"].HeaderText ="Принял";
            dataGridView1.Columns["prim"].HeaderText ="Примечание";
	}
	else
	{
	}
		}
		void Button8Click(object sender, EventArgs e)
		{
		if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`perevod` (`id`, `date`, `name_fil`,`who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '4 филиал', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Перевод в филиал успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}	
	
			
		}
		void Button9Click(object sender, EventArgs e)
		{
				if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`perevod` (`id`, `date`, `name_fil`,`who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '7 филиал', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Перевод в филиал успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		
			}
		}
		void Button10Click(object sender, EventArgs e)
		{
		if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`perevod` (`id`, `date`, `name_fil`,`who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '10 филиал', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Перевод в филиал успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		}
			void Button13Click(object sender, EventArgs e)
		{
		if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`perevod` (`id`, `date`, `name_fil`,`who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '12 филиал', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Перевод в филиал успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
			}
			void Button12Click(object sender, EventArgs e)
		{
		if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`perevod` (`id`, `date`, `name_fil`,`who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '15 филиал', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Перевод в филиал успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
			}
			
			void Button11Click(object sender, EventArgs e)
		{
		if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`perevod` (`id`, `date`, `name_fil`,`who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '29 филиал', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Перевод в филиал успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
			}
			
			
			void Button14Click(object sender, EventArgs e)
		{
		if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`perevod` (`id`, `date`, `name_fil`,`who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '30 филиал', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text="Перевод в филиал успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
			}
	
		void DataGridView1DoubleClick(object sender, EventArgs e)
		{
			Form2 f = new Form2();
			f.Show();
			f.textBox3.Text=dataGridView1.CurrentRow.Cells[0].Value.ToString();
			f.dateTimePicker1.Value=Convert.ToDateTime (dataGridView1.CurrentRow.Cells[1].Value);
			f.comboBox1.Text=dataGridView1.CurrentRow.Cells[2].Value.ToString();
			f.textBox1.Text=dataGridView1.CurrentRow.Cells[3].Value.ToString();
			f.textBox2.Text=dataGridView1.CurrentRow.Cells[4].Value.ToString();
			f.comboBox2.Text=dataGridView1.CurrentRow.Cells[5].Value.ToString();
			f.prim.Text=dataGridView1.CurrentRow.Cells[6].Value.ToString();

		}
		
		void Button16Click(object sender, EventArgs e)
		{
			if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button16.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button16.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button17Click(object sender, EventArgs e)
		{
	if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button17.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button17.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		
			}
		void Button18Click(object sender, EventArgs e)
		{
	if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button18.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button18.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button19Click(object sender, EventArgs e)
		{
	if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button19.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button19.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button20Click(object sender, EventArgs e)
		{
	if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button20.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button20.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button21Click(object sender, EventArgs e)
		{
	if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
			label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button21.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button21.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			}
		}
		void Button22Click(object sender, EventArgs e)
		{
			if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button22.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button22.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
	
		}
		void Button23Click(object sender, EventArgs e)
		{
	if (comboBox4.Text=="")
			{
				MessageBox.Show("Выберите свою фамилию из выпадающего списка и повторите действие");
			}
			else
			{
				label16.Text=System.DateTime.Now.ToShortTimeString();
			MySqlConnection connection;
			string cs = @"Server="+ConfigurationManager.AppSettings["Srv"]+";Port=3306;Userid=USER;Password=PASSWORD;Database="+ConfigurationManager.AppSettings["dbase"]+";charset=utf8";
			connection = new MySqlConnection(cs);
			try
			{
				connection.Open();
				string sql = @"INSERT INTO `"+ConfigurationManager.AppSettings["dbase"]+"`.`shared` (`id`, `date`, `question`, `who`,`time`,`comp_name`) VALUES (NULL, '"+dateTimePicker3.Value.ToString("yyyy/MM/dd")+"', '"+button23.Text+"', '"+comboBox4.Text+"','"+label16.Text+"','"+label15.Text+"')";
				connection.Close();
				MySqlScript script = new MySqlScript(connection, sql);
				script.Execute();
				statusStrip1.Items[0].ForeColor=Color.Green;
				statusStrip1.Items[0].Text=""+button23.Text+ " вопрос успешно записан";
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
		}
		void MaskedTextBox1TextChanged(object sender, EventArgs e)
		{	
			
			if(maskedTextBox1.Text.Length==0)
			{
				test.Items.Clear();
				comboBox3.Text="";
				richTextBox1.Text="";
				test.Text="";
				OutDateCall.Text="";
			}
			if(maskedTextBox1.Text.Length>8)
			{
			if(maskedTextBox1.Text.Length>8&&maskedTextBox1.Text.Length<10&&correctNumber.IsMatch(maskedTextBox1.Text)||maskedTextBox1.Text.Length>12&&correctmobile.IsMatch(maskedTextBox1.Text))
			{
				
				test.Items.Clear();
            var result = data.Where(x => x.phone.Contains(maskedTextBox1.Text)).ToArray();
                foreach (var item in result){
            	
                	if(item.array.Length>0){
                		test.Items.Add(item.array);
                		select.Add(item);
            	
            	
            	
            	}
            }
            
            if (test.Items.Count == 1) {test.Text=test.Items[0].ToString();
            	if(test.Text!=""){
            	var res=select.Where(s => s.array.Contains(test.Text)).ToArray();
            	foreach (var to in res){
            	richTextBox1.Text=to.prim;
            }
            	}
            	
            }
            if(test.Items.Count>1)
            	{
            		History h=new History();
            		h.Owner=this;
            		h.textBox1.Text=maskedTextBox1.Text;
					h.ShowDialog();
            	}
			}
				
				
			}
		}
		void TestTextChanged(object sender, EventArgs e)
		{
			
		}
		void Timer1Tick(object sender, EventArgs e)
		{
			timer1.Enabled=false;
			timer1.Stop();
			LoadAutoCall();
			Init();
			
		}
		void FindAutoCallTextChanged(object sender, EventArgs e)
		{
			findedvalue.Clear();
			if(findAutoCall.Text.Length==0)
			{
				outputRegNum.Text="";
				outputDateFix.Text="";
				outputRegNum.Items.Clear();
				outputDateFix.Items.Clear();
			}
			if(findAutoCall.Text.Length>6){
			outputRegNum.Items.Clear();
			outputDateFix.Items.Clear();
			
			var result = inputvalue.Where(x => x.phone.Contains(findAutoCall.Text)).ToArray();
                foreach (var item in result){
            	
                	if(item.regnumber.Length>0){
					findedvalue.Add(item);
            	}
           }
			}
		}
		void ToFindClick(object sender, EventArgs e)
		{
			outputRegNum.Items.Clear();
			outputDateFix.Items.Clear();
			var result = findedvalue.Where(x => x.phone.Contains(findAutoCall.Text)).ToArray();
                foreach (var item in result){
            	
                	if(item.regnumber.Length>0){
					outputRegNum.Items.Add(item.regnumber);
					outputDateFix.Items.Add(item.date.ToShortDateString());
            	}
			if(outputRegNum.Items.Count>1&&outputDateFix.Items.Count>1)
			{
				
			}
			}
			if(outputRegNum.Text!="")
			{
				var output=findedvalue.Where(s =>s.regnumber.Contains(outputRegNum.Text)).ToArray();
				foreach (var item in output){
            	
                	if(item.regnumber.Length>0){
						outputDateFix.Text=item.date.ToShortDateString();
					}
					
            	}
			}
			if (outputRegNum.Items.Count==1&&outputDateFix.Items.Count==1)
			{
				outputRegNum.Text=outputRegNum.Items[0].ToString();
				outputDateFix.Text=outputDateFix.Items[0].ToString();
			}
			
			
		}
		void FindAutoCallKeyDown(object sender, KeyEventArgs e)
		{
			
			if(e.KeyCode==Keys.Enter)
			{
				if(findAutoCall.Text.Length>6&&findAutoCall.Text.Length<10&&correctNumber.IsMatch(findAutoCall.Text)==true || findAutoCall.Text.Length>9&&correctmobile.IsMatch(findAutoCall.Text)==true)
				{
				MessageBox.Show("Формат имени неверный");
				}
				else
				{
					outputRegNum.Items.Clear();
					outputDateFix.Items.Clear();
					ToFindClick(sender,e);
					
				}
			}
		}
		void MaskedTextBox1MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
	
		}
		void OutputRegNumTextChanged(object sender, EventArgs e)
		{
			if(outputRegNum.Text=="")
            	{
				
			}
            else
            	{
            	 if(outputRegNum.Items.Count >1) 
            {
            	var res=findedvalue.Where(s => s.regnumber.Contains(outputRegNum.Text)).ToArray();
            	foreach (var to in res){
            	outputDateFix.Text=to.date.ToShortDateString();
            	}
            }
            	
            }
		}
		
		
		
	
	
	}
	
}
