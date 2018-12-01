using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DataWindowDemo_frm
{
    public partial class Form1 : Form
    {
        string t_name = "";
        string t_group = "";
        string t_file = "";
        int isAdd = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private SqlConnection conn = new System.Data.SqlClient.SqlConnection();
        //打开数据库
        public void openDataBase()
        {
            try
            {
                conn.ConnectionString = "Data Source=.;initial catalog=Config;User ID =sa;password=123456;";
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                Sybase.DataWindow.AdoTransaction SQLCA = new Sybase.DataWindow.AdoTransaction(conn, "");
                SQLCA.DbParameter = "TimeOut=1000000,CommandTimeOut=2000000";
                SQLCA.BindConnection();
                this.dataWindowExt_menu.SetTransaction(SQLCA);
            }
            catch (Exception eeee)
            {
            }
        }

        //获取数据
        private void buttonRetriveData_Click(object sender, EventArgs e)
        {

            openDataBase();
            updateDb();
            try
            {
                dataWindowExt_menu.Retrieve(new Object[] { });
            }
            catch (Exception ee)
            {
                // Logger.Instance.Error(ee.Message);
            }
            try
            {
                conn.Close();
            }
            catch (Exception eee)
            {
                //Logger.Instance.Error(eee.Message);
            }
            // treeView1.Nodes.Clear();
            //tree_load();
            conn.Close();

        }

        //保存数据
        private void buttonSave_Click(object sender, EventArgs e)
        {
            openDataBase();
            //if (tNode == null)
                dataWindowExt_menu.SetFilter("");
            dataWindowExt_menu.Filter();
            dataWindowExt_menu.AcceptText();
            if ((dataWindowExt_menu.ModifiedCount + dataWindowExt_menu.DeletedCount) == 0)
            {
                MessageBox.Show("没有数据需要保存!");
                dataWindowExt_menu.Focus();
                return;
            }
            try
            {
                for (int i = 1; i <= dataWindowExt_menu.RowCount; i++)
                {
                    int flag = 0;
                    for (int j = i + 1; j <= dataWindowExt_menu.RowCount; j++)
                    {
                        string str_g = dataWindowExt_menu.GetItemString(i, 1);
                        string str_g1 = dataWindowExt_menu.GetItemString(j, 1);
                        string str = dataWindowExt_menu.GetItemString(i, 4);
                        string str1 = dataWindowExt_menu.GetItemString(j, 4);
                        string strg = dataWindowExt_menu.GetItemString(i, 2);
                        string strg1 = dataWindowExt_menu.GetItemString(j, 2);

                        if (str_g == str_g1 && strg == strg1 && str == str1)
                        {
                            MessageBox.Show("存在重复数据！！");
                            dataWindowExt_menu.SetFilter("filepath = '"+t_file+"'and groupno = '" + t_group + "' and name = '"+ t_name+"' ");
                            dataWindowExt_menu.Filter();
                            flag = 1;
                            break;
                        }
                    }
                    if (i == dataWindowExt_menu.RowCount)
                    {
                        dataWindowExt_menu.UpdateData(true, true);
                        MessageBox.Show("保存成功");
                        dataWindowExt_menu.Retrieve(new Object[] { });
                        tojson();
                        treeView1.Nodes.Clear();
                        tree_load();
                        if (t_group == "" && t_name != "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "' and name = '" + t_name + "' ");
                        if (t_group == "" && t_name == "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'");
                        if (t_group == "" && t_name != "" && t_file == "")
                            dataWindowExt_menu.SetFilter("name = '" + t_name + "'");
                        if (t_group != "" && t_name == "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'and groupno = '" + t_group + "' ");
                        if (t_group == "" && t_name == "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "' ");
                        if (t_group != "" && t_name == "" && t_file == "")
                            dataWindowExt_menu.SetFilter("groupno = '" + t_group + "' ");
                        if (t_group != "" && t_name != "" && t_file == "")
                            dataWindowExt_menu.SetFilter("groupno = '" + t_group + "' and name = '" + t_name + "' ");
                        if (t_group != "" && t_name == "" && t_file == "")
                            dataWindowExt_menu.SetFilter(" groupno = '" + t_group + "' ");
                        if (t_group == "" && t_name != "" && t_file == "")
                            dataWindowExt_menu.SetFilter(" name = '" + t_name + "' ");
                        if (t_group != "" && t_name != "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'and groupno = '" + t_group + "' and name = '" + t_name + "' ");                       
                        dataWindowExt_menu.Filter();
                        break;
                    }
                    if (flag == 1)
                        break;
                }
            }
            catch (Exception ee)
            {
                //    // Logger.Instance.Error(ee.Message);
            }
            try
            {
                conn.Close();
            }
            catch (Exception eee)
            {
                //Logger.Instance.Error(eee.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)           //总目录
        {
            openDataBase();
            dataWindowExt_menu.SetFilter("");
            dataWindowExt_menu.Filter();
            dataWindowExt_menu.Retrieve(new Object[] { });
            this.dataWindowExt_menu.Visible = true;
            dataWindowExt_menu.Show();
            t_file ="" ;
            t_group ="";
            t_name = "";
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)   //选择节点
        {
            int level = treeView1.SelectedNode.Level;
            if (level == 0)
            {
                string str = treeView1.SelectedNode.Text;
                openDataBase();
                dataWindowExt_menu.SetFilter("filepath = '" + str + "'");
                dataWindowExt_menu.Filter();
                dataWindowExt_menu.Retrieve(new Object[] { });
                dataWindowExt_menu.Show();
                t_file = str;
            }
            else if (level == 1)
            {
                string str = treeView1.SelectedNode.Text;
                string strp = treeView1.SelectedNode.Parent.Text;
                openDataBase();
                dataWindowExt_menu.SetFilter("filepath='"+strp+"'and groupno = '" + str + "'");
                dataWindowExt_menu.Filter();
                dataWindowExt_menu.Retrieve(new Object[] { });
                dataWindowExt_menu.Show();
                t_group = str;
                t_file = strp;
            }
            else
            {
                string str = treeView1.SelectedNode.Text;
                string strp = treeView1.SelectedNode.Parent.Text;
                string strpp = treeView1.SelectedNode.Parent.Parent.Text;
                openDataBase();
                dataWindowExt_menu.SetFilter("name = '" + str + "' and groupno = '" + strp + "'and filepath = '" + strpp + "'");
                dataWindowExt_menu.Filter();
                dataWindowExt_menu.Retrieve(new Object[] { });
                dataWindowExt_menu.Show();
                t_name = str;
                t_group = strp;
                t_file = strpp;
            }
        }

        private void button_search_Click(object sender, EventArgs e)    //搜索
        {
            string s_name = this.textBox_name.Text;
            string s_file = this.textBox_file.Text;
            string s_group = this.textBox_group.Text;
            openDataBase();
            if (s_group == "" && s_name != "" && s_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + s_file + "' and name = '" + s_name + "' ");
            if (s_group == "" && s_name == "" && s_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + s_file + "'");
            if (s_group == "" && s_name != "" && s_file == "")
                dataWindowExt_menu.SetFilter("name = '" + s_name + "'");
            if (s_group != "" && s_name == "" && s_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + s_file + "'and groupno = '" + s_group + "' ");
            if (s_group == "" && s_name == "" && s_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + s_file + "' ");
            if (s_group != "" && s_name == "" && s_file == "")
                dataWindowExt_menu.SetFilter("groupno = '" + s_group + "' ");
            if (s_group != "" && s_name != "" && s_file == "")
                dataWindowExt_menu.SetFilter("groupno = '" + s_group + "' and name = '" + s_name + "' ");
            if (s_group != "" && s_name == "" && s_file == "")
                dataWindowExt_menu.SetFilter(" groupno = '" + s_group + "' ");
            if (s_group == "" && s_name != "" && s_file == "")
                dataWindowExt_menu.SetFilter(" name = '" + s_name + "' ");
            if (s_group != "" && s_name != "" && s_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + s_file + "'and groupno = '" + s_group + "' and name = '" + s_name + "' ");
            dataWindowExt_menu.Filter();
            dataWindowExt_menu.Retrieve(new Object[] { });
            int rowCount = dataWindowExt_menu.RowCount;
            if (rowCount == 0)
                MessageBox.Show("没有此配置项");
            else
            {   t_file="";
                t_group="";
                t_name = "";
            }
        }
        public void tree_load()           //treeview加载
        {
            string pNode, cNode, ccNode;
            string pNode1 = "", cNode1 = "";
            int j = 1;
            int k = 1;
            for (int i = 1; i <= dataWindowExt_menu.RowCount; i++)
            {
                pNode = dataWindowExt_menu.GetItemString(i, 1);
                cNode = dataWindowExt_menu.GetItemString(i, 2);
                ccNode = dataWindowExt_menu.GetItemString(i, 3);
                if (pNode + "1" == pNode1)
                {
                    if (cNode + "1" == cNode1)
                    {
                        treeView1.Nodes[j - 1].Nodes[k - 1].Nodes.Add(ccNode);
                    }
                    else
                    {
                        j = j - 1;
                        treeView1.Nodes[j].Nodes.Add(cNode);  //添加子节点
                        treeView1.Nodes[j].Nodes[k].Nodes.Add(ccNode);
                        j++;
                        k++;
                        cNode1 = cNode + "1";
                        continue;
                    }
                }
                else if (i == 1)
                {
                    pNode1 = pNode + "1";
                    treeView1.Nodes.Add(pNode);   //添加根节点  
                    cNode1 = cNode + "1";
                    treeView1.Nodes[j - 1].Nodes.Add(cNode);   //添加子节点
                    treeView1.Nodes[j - 1].Nodes[k - 1].Nodes.Add(ccNode);
                }
                else
                {
                    j++;
                    k = 1;
                    pNode1 = pNode + "1";
                    cNode1 = cNode1 + "1";
                    treeView1.Nodes.Add(pNode);   //添加根节点               
                    treeView1.Nodes[j - 1].Nodes.Add(cNode);   //添加子节点
                    treeView1.Nodes[j - 1].Nodes[k - 1].Nodes.Add(ccNode);
                    cNode1 = cNode + "1";
                }
            }
            treeView1.Nodes[0].Expand();
            treeView1.Nodes[0].Nodes[0].Expand();
        }
        private void Form1_Load(object sender, EventArgs e)     
        {
            dataWindowExt_menu.SetFilter("");
            dataWindowExt_menu.Filter();
            openDataBase();
            updateDb();
            this.dataWindowExt_menu.Retrieve(new Object[] { });
            tree_load();
        }

        public void tojson()                     // 数据库到json
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.ConnectionString = "Data Source=.;initial catalog=Config;User ID =sa;password=123456;";
                conn.Open();
            }
            string sql = "select Id,FilePath,Groupno,ItemName,ItemValue,Name,Description,Example,IsActive,IsDeleted,ParentId,Creator,CreationTime,Modifier,ModificationTime from appsetting";
            SqlCommand com = new SqlCommand(sql, conn);//定义查询命令                        
            //List<Config_group> groups = new List<Config_group>();
            //using (SqlDataReader reader = com.ExecuteReader())
            //{
            //    string path = "";
            //    int flag = 0;
            //    string con_file_path;
            //    //List<ParentId> keys = new List<ParentId>();               
            //    while (reader.Read())  //如果能读到数据，一行一行地读
            //    {
            //        Config_group group = new Config_group()
            //        {
            //            id = int.Parse(reader["Id"].ToString()),
            //            filePath = reader["FilePath"].ToString(),
            //            groupno = reader["Groupno"].ToString(),
            //            itemValue = reader["ItemValue"].ToString(),
            //            itemName = reader["ItemName"].ToString(),
            //            name = reader["Name"].ToString(),
            //            description = reader["Description"].ToString(),
            //            example = reader["Example"].ToString(),
            //            isActive = int.Parse(reader["IsActive"].ToString()),
            //            isDeleted = int.Parse(reader["IsDeleted"].ToString()),
            //            parentId = reader["ParentId"].ToString(),
            //            creator = reader["Creator"].ToString(),
            //            creationTime = Convert.ToDateTime(reader["CreationTime"] == DBNull.Value ? SqlDateTime.MinValue.ToString() : reader["CreationTime"].ToString()),
            //            modifier = reader["Modifier"].ToString(),
            //            modificationTime = Convert.ToDateTime(reader["ModificationTime"] == DBNull.Value ? SqlDateTime.MinValue.ToString() : reader["ModificationTime"].ToString())
            //        };
            //        groups.Add(group);
            //        var jsonString = JsonConvert.SerializeObject(group);

            //        con_file_path = @"E:\VS\" + group.filePath + ".json";
            //        if (!File.Exists(con_file_path))
            //        {
            //            File.Create(con_file_path);
            //        }
            //        if (path != group.filePath)
            //        {
            //            flag = 1;
            //        }
            //        if (flag == 1)
            //        {
            //            FileStream stream = File.Open(con_file_path, FileMode.OpenOrCreate, FileAccess.Write);
            //            stream.SetLength(0);
            //            stream.Close();
            //            flag = 0;
            //        }
            //        using (StreamWriter writer = new StreamWriter(con_file_path, true))
            //        {
            //            writer.WriteLine(jsonString);
            //            writer.Flush();
            //        }
            //        path = group.filePath;
            //    }
            //    reader.Close();
            //} 

            // com.CommandText = "select Groupno,ItemName,ItemValue from appsetting where ItemName = @name";
            // com.Parameters.Add(new SqlParameter("@ItemName", SqlDbType.NChar, 50));
            string path = "";
            for (int j = 1; j <= dataWindowExt_menu.RowCount; j++)
            {
                dataWindowExt_menu.AcceptText();
                string file_name = dataWindowExt_menu.GetItemString(j, 1);
                string file_path = @"D:\vs workspace\" + file_name + ".json";
                if (path == file_name)
                {
                    continue;
                }
                if (file_name == "DwConditionRenderSettings")
                {
                    path = "DwConditionRenderSettings";
                    string[] a = { "RenderMode", "DatasourceSql","ItemText","MainDatawindowObject","PrintDatawindowObject","ConditionValue","AdditionalButtonSettings","ColumnReflectSettings","ColumnUpdateSettings",
                         "NursingRecordSyncScript","SplitRowsWithOnPrinting","NursingNoteHideGroupColumns","NursingNoteFocusColumnAfterInsert","NursingNoteAutoHeightWith","NursingNoteDatawindowZoom","NursingNotePrintDatawindowZoom",
                        "WordSize","IsCaSign","EachKey","CAKey","CaSignType","PicturePath","NursingFrequency","DefNursingFrequency","InsertRowMode","IsAddInOutSummaryManually","GeneralPopupEditableColumns","GeneralPopupUnitEditableColumns",
                        "InOutSummaryTimeRange","InStaticSql","OutStaticSql","InOutSumAccountSql","InSumMapping","OutSumMapping","SpecialInOutNameMapping","InDeptTime","ItemText","MainDatawindowObject","PrintDatawindowObject","ConditionValue","AdditionalButtonSettings","ColumnReflectSettings","ColumnUpdateSettings",
                         "NursingRecordSyncScript","SplitRowsWithOnPrinting","NursingNoteHideGroupColumns","NursingNoteFocusColumnAfterInsert","NursingNoteAutoHeightWith","NursingNoteDatawindowZoom","NursingNotePrintDatawindowZoom",
                         "WordSize","IsCaSign","EachKey","CAKey","CaSignType","PicturePath","NursingFrequency","DefNursingFrequency","InsertRowMode","IsAddInOutSummaryManually","GeneralPopupEditableColumns","GeneralPopupUnitEditableColumns",
                         "InOutSummaryTimeRange","InStaticSql","OutStaticSql","InOutSumAccountSql","InSumMapping","OutSumMapping","SpecialInOutNameMapping","InDeptTime"};
                    string name, value, groupno;
                    int f = 0;
                    DwConditionRenderSettings item = new DwConditionRenderSettings();
                    Items items = new Items();
                    Type type1 = items.GetType();
                    Items items1 = new Items();
                    Type type2 = items1.GetType();
                    SqlDataReader rd = com.ExecuteReader();
                    for (int i = 0; i < a.Length; i++)
                    {
                        //com.Parameters["@ItemName"].Value = a[i];
                        while (rd.Read())
                        {
                            name = rd["ItemName"].ToString();
                            value = rd["ItemValue"].ToString();
                            groupno = rd["Groupno"].ToString();
                            if (name == "RenderMode")
                            {
                                item.RenderMode = int.Parse(value);
                                break;
                            }
                            if (name == "DatasourceSql")
                            {
                                item.DatasourceSql = value;
                                break;
                            }
                            if (groupno == "危重病人")
                            {
                                foreach (var t in type1.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "NursingNotePrintDatawindowZoom" || name == "WordSize")
                                        {
                                            t.SetValue(items, int.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else if (name == "IsCaSign" || name == "EachKey" || name == "CAKey" || name == "IsAddInOutSummaryManually")
                                        {
                                            t.SetValue(items, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "一般病人")
                            {
                                foreach (var t in type2.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "NursingNotePrintDatawindowZoom" || name == "WordSize")
                                        {
                                            //if (value!="")
                                            //{
                                            t.SetValue(items1, Convert.ToInt32(value), null);
                                            f = 1;
                                            break;
                                            // }
                                            //else
                                            //{
                                            //    t.SetValue(items1, value, null);
                                            //    f = 1;
                                            //    break;
                                            //}
                                        }
                                        else if (name == "IsCaSign" || name == "EachKey" || name == "CAKey" || name == "IsAddInOutSummaryManually")
                                        {
                                            t.SetValue(items1, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items1, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                                
                            }
                            if (f == 1)
                            {
                                f = 0;
                                break;
                            }
                        }
                    }
                    rd.Close();
                    List<Items> itemss = new List<Items>();
                    itemss.Add(items);
                    itemss.Add(items1);
                    item.items = itemss;
                    var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };  //过滤null空值
                    var jString = JsonConvert.SerializeObject(item, Formatting.Indented, jsonSetting);
                    if (!File.Exists(file_path))
                    {
                        File.Create(file_path);
                    }
                    FileStream s = File.Open(file_path, FileMode.OpenOrCreate, FileAccess.Write);
                    s.SetLength(0);
                    s.Close();
                    using (StreamWriter writer = new StreamWriter(file_path))
                    {
                        writer.Write(jString);
                        writer.Flush();
                    }
                }
                else if (file_name == "ColumnReflectSettings")
                {
                    path = "ColumnReflectSettings";
                    string[] a = { "Column", "Assembly", "Type", "Method", "QueryString", "SyncDataWithResult", "ExecuteCondition", "Column", "Assembly", "Type", "Method", "QueryString", "SyncDataWithResult", "ExecuteCondition" };
                    string name, value, groupno;
                    int f = 0;

                    ColumnReflectSettings items = new ColumnReflectSettings();
                    Type type1 = items.GetType();
                    ColumnReflectSettings items1 = new ColumnReflectSettings();
                    Type type2 = items1.GetType();
                    SqlDataReader rd = com.ExecuteReader();
                    for (int i = 0; i < a.Length; i++)
                    {
                        //com.Parameters["@ItemName"].Value = a[i];
                        while (rd.Read())
                        {
                            name = rd["ItemName"].ToString();
                            value = rd["ItemValue"].ToString();
                            groupno = rd["Groupno"].ToString();

                            if (groupno == "Items")
                            {
                                foreach (var t in type1.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        t.SetValue(items, value, null);
                                        f = 1;
                                        break;
                                    }
                                }
                            }
                            if (groupno == "Items1")
                            {
                                foreach (var t in type2.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        t.SetValue(items1, value, null);
                                        f = 1;
                                        break;
                                    }
                                }
                               
                            }
                            if (f == 1)
                            {
                                f = 0;
                                break;
                            }
                        }
                    }
                    rd.Close();
                    List<ColumnReflectSettings> item = new List<ColumnReflectSettings>();
                    item.Add(items);
                    item.Add(items1);
                    var jString = JsonConvert.SerializeObject(item);
                    if (!File.Exists(file_path))
                    {
                        File.Create(file_path);
                    }
                    FileStream s = File.Open(file_path, FileMode.OpenOrCreate, FileAccess.Write);
                    s.SetLength(0);
                    s.Close();
                    using (StreamWriter writer = new StreamWriter(file_path))
                    {
                        writer.Write(jString);
                        writer.Flush();
                    }
                }
                else if (file_name == "AdditionalButtonSettings")
                {
                    path = "AdditionalButtonSettings";
                    string[] a = { "Caption", "Assembly", "Type", "Method", "Parameters", "Caption", "Assembly", "Type", "Method", "Parameters", 
                                     "Caption", "Assembly", "Type", "Method", "Parameters", "Caption", "Assembly", "Type", "Method", "Parameters" };
                    string name, value, groupno;
                    int f = 0;
                    AdditionalButtonSettings items = new AdditionalButtonSettings();
                    Type type1 = items.GetType();
                    AdditionalButtonSettings items1 = new AdditionalButtonSettings();
                    Type type2 = items1.GetType();
                    AdditionalButtonSettings items2 = new AdditionalButtonSettings();
                    Type type3 = items1.GetType();
                    AdditionalButtonSettings items3 = new AdditionalButtonSettings();
                    Type type4 = items1.GetType();
                    SqlDataReader rd = com.ExecuteReader();
                    for (int i = 0; i < a.Length; i++)
                    {
                        //com.Parameters["@ItemName"].Value = a[i];
                        while (rd.Read())
                        {
                            name = rd["ItemName"].ToString();
                            value = rd["ItemValue"].ToString();
                            groupno = rd["Groupno"].ToString();

                            if (groupno == "导入生命体征")
                            {
                                foreach (var t in type1.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        t.SetValue(items, value, null);
                                        f = 1;
                                        break;
                                    }
                                }
                            }
                            if (groupno == "绑定")
                            {
                                foreach (var t in type2.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        t.SetValue(items1, value, null);
                                        f = 1;
                                        break;
                                    }
                                }
                            }
                            if (groupno == "解绑")
                            {
                                foreach (var t in type3.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        t.SetValue(items2, value, null);
                                        f = 1;
                                        break;
                                    }
                                }
                            }
                            if (groupno == "设备维护")
                            {
                                foreach (var t in type4.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        t.SetValue(items3, value, null);
                                        f = 1;
                                        break;
                                    }
                                }
                               
                            }
                            if (f == 1)
                            {
                                f = 0;
                                break;
                            }
                        }
                    }
                    rd.Close();
                    List<AdditionalButtonSettings> item = new List<AdditionalButtonSettings>();
                    item.Add(items);
                    item.Add(items1);
                    item.Add(items2);
                    item.Add(items3);
                    var jString = JsonConvert.SerializeObject(item);
                    if (!File.Exists(file_path))
                    {
                        File.Create(file_path);
                    }
                    FileStream s = File.Open(file_path, FileMode.OpenOrCreate, FileAccess.Write);
                    s.SetLength(0);
                    s.Close();
                    using (StreamWriter writer = new StreamWriter(file_path))
                    {
                        writer.Write(jString);
                        writer.Flush();
                    }
                }
                else if (file_name == "ColumnUpdateSettings")
                {
                    path = "ColumnUpdateSettings";
                    string[] a = { "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",//9
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",//18
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql",
                                 "Column","InsertCondition","InsertSql","DwColumnInitedWithInsertResult","UpdateCondition","UpdateSql","DeleteCondition","DeleteSql","DwColumnInitedWithNullAfterDelete","AllowDeleteWhenNursingDateChanged","Description","UpdateNursingDateSql"
                                 };
                    string name, value, groupno;
                    int f = 0;
                    ColumnUpdateSettings items = new ColumnUpdateSettings();
                    Type type1 = items.GetType();
                    ColumnUpdateSettings items1 = new ColumnUpdateSettings();
                    Type type2 = items1.GetType();
                    ColumnUpdateSettings items2 = new ColumnUpdateSettings();
                    Type type3 = items2.GetType();
                    ColumnUpdateSettings items3 = new ColumnUpdateSettings();
                    Type type4 = items3.GetType();
                    ColumnUpdateSettings items4 = new ColumnUpdateSettings();
                    Type type5 = items4.GetType();
                    ColumnUpdateSettings items5 = new ColumnUpdateSettings();
                    Type type6 = items5.GetType();
                    ColumnUpdateSettings items6 = new ColumnUpdateSettings();
                    Type type7 = items6.GetType();
                    ColumnUpdateSettings items7 = new ColumnUpdateSettings();
                    Type type8 = items7.GetType();
                    ColumnUpdateSettings items8 = new ColumnUpdateSettings();
                    Type type9 = items8.GetType();
                    ColumnUpdateSettings items9 = new ColumnUpdateSettings();
                    Type type10 = items9.GetType();
                    ColumnUpdateSettings items10 = new ColumnUpdateSettings();
                    Type type11 = items10.GetType();
                    ColumnUpdateSettings items11 = new ColumnUpdateSettings();
                    Type type12 = items11.GetType();
                    ColumnUpdateSettings items12 = new ColumnUpdateSettings();
                    Type type13 = items12.GetType();
                    ColumnUpdateSettings items13 = new ColumnUpdateSettings();
                    Type type14 = items13.GetType();
                    ColumnUpdateSettings items14 = new ColumnUpdateSettings();
                    Type type15 = items14.GetType();
                    ColumnUpdateSettings items15 = new ColumnUpdateSettings();
                    Type type16 = items15.GetType();
                    ColumnUpdateSettings items16 = new ColumnUpdateSettings();
                    Type type17 = items16.GetType();
                    ColumnUpdateSettings items17 = new ColumnUpdateSettings();
                    Type type18 = items17.GetType();
                    ColumnUpdateSettings items18 = new ColumnUpdateSettings();
                    Type type19 = items18.GetType();
                    ColumnUpdateSettings items19 = new ColumnUpdateSettings();
                    Type type20 = items19.GetType();
                    ColumnUpdateSettings items20 = new ColumnUpdateSettings();
                    Type type21 = items20.GetType();
                    ColumnUpdateSettings items21 = new ColumnUpdateSettings();
                    Type type22 = items21.GetType();
                    ColumnUpdateSettings items22 = new ColumnUpdateSettings();
                    Type type23 = items22.GetType();
                    ColumnUpdateSettings items23 = new ColumnUpdateSettings();
                    Type type24 = items23.GetType();
                    ColumnUpdateSettings items24 = new ColumnUpdateSettings();
                    Type type25 = items24.GetType();
                    ColumnUpdateSettings items25 = new ColumnUpdateSettings();
                    Type type26 = items25.GetType();
                    ColumnUpdateSettings items26 = new ColumnUpdateSettings();
                    Type type27 = items26.GetType();
                    ColumnUpdateSettings items27 = new ColumnUpdateSettings();
                    Type type28 = items27.GetType();
                    ColumnUpdateSettings items28 = new ColumnUpdateSettings();
                    Type type29 = items28.GetType();
                    SqlDataReader rd = com.ExecuteReader();
                    for (int i = 0; i < a.Length; i++)
                    {
                        //com.Parameters["@ItemName"].Value = a[i];
                        while (rd.Read())
                        {
                            name = rd["ItemName"].ToString();
                            value = rd["ItemValue"].ToString();
                            groupno = rd["Groupno"].ToString();

                            if (groupno == "体温")
                            {
                                foreach (var t in type1.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "脉搏")
                            {
                                foreach (var t in type2.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items1, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items1, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "呼吸")
                            {
                                foreach (var t in type3.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items2, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items2, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "收缩压")
                            {
                                foreach (var t in type4.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items3, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items3, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "扩展压")
                            {
                                foreach (var t in type5.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items4, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items4, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "氧饱和度")
                            {
                                foreach (var t in type6.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items5, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items5, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "神志")
                            {
                                foreach (var t in type7.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items6, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items6, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "瞳孔左")
                            {
                                foreach (var t in type8.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items7, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items7, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "瞳孔右")
                            {
                                foreach (var t in type9.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items8, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items8, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "对光反应左")
                            {
                                foreach (var t in type10.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items9, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items9, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "对光反应右")
                            {
                                foreach (var t in type11.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items10, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items10, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "吸氧")
                            {
                                foreach (var t in type12.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items11, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items11, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "吸痰")
                            {
                                foreach (var t in type13.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items12, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items12, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "雾化吸入")
                            {
                                foreach (var t in type14.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items13, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items13, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "皮肤情况")
                            {
                                foreach (var t in type15.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items14, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items14, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "输液")
                            {
                                foreach (var t in type16.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items15, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items15, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "输液量")
                            {
                                foreach (var t in type17.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items16, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items16, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "出量")
                            {
                                foreach (var t in type18.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items17, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items17, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "出量值")
                            {
                                foreach (var t in type19.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items18, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items18, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "卧位")
                            {
                                foreach (var t in type20.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items19, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items19, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "饮食和水")
                            {
                                foreach (var t in type21.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items20, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items20, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "饮食水量值")
                            {
                                foreach (var t in type22.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items21, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items21, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "管路护理")
                            {
                                foreach (var t in type23.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items22, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items22, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "生活护理")
                            {
                                foreach (var t in type24.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items23, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items23, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "小结时间")
                            {
                                foreach (var t in type25.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items24, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items24, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "小结入量")
                            {
                                foreach (var t in type26.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items25, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items25, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "小结出量")
                            {
                                foreach (var t in type27.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items26, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items26, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "说明性小结")
                            {
                                foreach (var t in type28.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items27, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items27, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (groupno == "病情记录")
                            {
                                foreach (var t in type29.GetProperties())
                                {
                                    if (name.Equals(a[i]) && t.Name.Equals(name))
                                    {
                                        if (name == "AllowDeleteWhenNursingDateChanged")
                                        {
                                            t.SetValue(items28, bool.Parse(value), null);
                                            f = 1;
                                            break;
                                        }
                                        else
                                        {
                                            t.SetValue(items28, value, null);
                                            f = 1;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (f == 1)
                            {
                                f = 0;
                                break;
                            }
                        }
                    }
                    rd.Close();
                    List<ColumnUpdateSettings> item = new List<ColumnUpdateSettings>();
                    item.Add(items); item.Add(items1); item.Add(items2); item.Add(items3); item.Add(items4);
                    item.Add(items5); item.Add(items6); item.Add(items7); item.Add(items8); item.Add(items9); item.Add(items10);
                    item.Add(items11); item.Add(items12); item.Add(items13); item.Add(items14); item.Add(items15); item.Add(items16);
                    item.Add(items17); item.Add(items18); item.Add(items19); item.Add(items20); item.Add(items21); item.Add(items22);
                    item.Add(items23); item.Add(items24); item.Add(items25); item.Add(items26); item.Add(items27); item.Add(items28);
                    var jString = JsonConvert.SerializeObject(item);
                    if (!File.Exists(file_path))
                    {
                        File.Create(file_path);
                    }
                    FileStream s = File.Open(file_path, FileMode.OpenOrCreate, FileAccess.Write);
                    s.SetLength(0);
                    s.Close();
                    using (StreamWriter writer = new StreamWriter(file_path))
                    {
                        writer.Write(jString);
                        writer.Flush();
                    }
                }
            }
        }

        public void updateDb()         //json到数据库
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.ConnectionString = "Data Source=.;initial catalog=Config;User ID =sa;password=123456;";
                conn.Open();
            }

            string con_file_path;
            con_file_path = @"D:\vs workspace\DwConditionRenderSettings.json";
            using (StreamReader sr = new StreamReader(con_file_path))
            {
                string str = sr.ReadToEnd();
                str = str.Replace("\n", "");
                DwConditionRenderSettings dwobj = (DwConditionRenderSettings)JsonConvert.DeserializeObject(str, typeof(DwConditionRenderSettings));

                string drop = "truncate table Table_copy";
                SqlCommand com = new SqlCommand(drop, conn);
                com.ExecuteNonQuery();
                com.CommandText = "insert into Table_copy select Id,Groupno,ItemName,ItemValue from appsetting where appsetting.FilePath= 'DwConditionRenderSettings'";
                com.ExecuteNonQuery();
                com.CommandText = "update Table_copy set ItemValue = @ItemValue where ItemName = @ItemName";
                com.Parameters.Add(new SqlParameter("@ItemName", SqlDbType.NChar, 50));
                com.Parameters.Add(new SqlParameter("@ItemValue", SqlDbType.NChar, 50));
                com.Parameters["@ItemName"].Value = "RenderMode";
                com.Parameters["@ItemValue"].Value = dwobj.RenderMode;
                com.ExecuteNonQuery();
                com.Parameters["@ItemName"].Value = "DatasourceSql";
                com.Parameters["@ItemValue"].Value = dwobj.DatasourceSql;
                com.ExecuteNonQuery();

                com.CommandText = "update Table_copy set ItemValue = @ItemValue where ItemName = @ItemName and Groupno = @Groupno";
                com.Parameters.Add(new SqlParameter("@Groupno", SqlDbType.NChar, 50));
                List<Items> items0 = new List<Items>();
                items0.Add(dwobj.items[0]);
                List<Items> items1 = new List<Items>();
                items1.Add(dwobj.items[1]);

                Type tp = items0.GetType();
                PropertyInfo tp_item = tp.GetProperty("Item");
                object ob = tp_item.GetValue(items0, new object[] { 0 });
                Type tpob = ob.GetType();
                foreach (PropertyInfo p in tpob.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "危重病人";
                    com.Parameters["@ItemValue"].Value = p.GetValue(ob, null);
                    com.Parameters["@ItemName"].Value = p.Name;
                    com.ExecuteNonQuery();
                }

                Type t = items1.GetType();
                PropertyInfo t_item = t.GetProperty("Item");
                object ob1 = t_item.GetValue(items1, new object[] { 0 });
                Type tob = ob1.GetType();
                foreach (PropertyInfo p in tob.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "一般病人";
                    com.Parameters["@ItemValue"].Value = p.GetValue(ob1, null);
                    com.Parameters["@ItemName"].Value = p.Name;
                    com.ExecuteNonQuery();
                }

                com.CommandText = "update appsetting set appsetting.ItemName = Table_copy.ItemName,appsetting.ItemValue = Table_copy.ItemValue from appsetting,Table_copy where appsetting.Id=Table_copy.Id";
                com.ExecuteNonQuery();
            }

            con_file_path = @"D:\vs workspace\AdditionalButtonSettings.json";
            using (StreamReader sr = new StreamReader(con_file_path))
            {
                string str = sr.ReadToEnd();
                str = str.Replace("\n", "");
                List<AdditionalButtonSettings> dwobj = (List<AdditionalButtonSettings>)JsonConvert.DeserializeObject(str, typeof(List<AdditionalButtonSettings>));

                string drop = "truncate table Table_copy";
                SqlCommand com = new SqlCommand(drop, conn);
                com.ExecuteNonQuery();
                com.CommandText = "insert into Table_copy select Id,Groupno,ItemName,ItemValue from appsetting where appsetting.FilePath= 'AdditionalButtonSettings'";
                com.ExecuteNonQuery();
                com.Parameters.Add(new SqlParameter("@ItemName", SqlDbType.NChar, 50));
                com.Parameters.Add(new SqlParameter("@ItemValue", SqlDbType.NChar, 50));
                com.CommandText = "update Table_copy set ItemValue = @ItemValue where ItemName = @ItemName and Groupno = @Groupno";
                com.Parameters.Add(new SqlParameter("@Groupno", SqlDbType.NChar, 50));

                Type tp = dwobj[0].GetType();
                foreach (PropertyInfo t in tp.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "导入生命体征";
                    com.Parameters["@ItemValue"].Value = t.GetValue(dwobj[0], null);
                    com.Parameters["@ItemName"].Value = t.Name;
                    com.ExecuteNonQuery();
                }

                Type tp1 = dwobj[1].GetType();
                foreach (PropertyInfo t1 in tp1.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "绑定";
                    com.Parameters["@ItemValue"].Value = t1.GetValue(dwobj[1], null);
                    com.Parameters["@ItemName"].Value = t1.Name;
                    com.ExecuteNonQuery();
                }

                Type tp2 = dwobj[2].GetType();
                foreach (PropertyInfo t2 in tp2.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "解绑";
                    com.Parameters["@ItemValue"].Value = t2.GetValue(dwobj[2], null);
                    com.Parameters["@ItemName"].Value = t2.Name;
                    com.ExecuteNonQuery();
                }

                Type tp3 = dwobj[3].GetType();
                foreach (PropertyInfo t3 in tp3.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "设备维护";
                    com.Parameters["@ItemValue"].Value = t3.GetValue(dwobj[3], null);
                    com.Parameters["@ItemName"].Value = t3.Name;
                    com.ExecuteNonQuery();
                }

                com.CommandText = "update appsetting set appsetting.ItemName = Table_copy.ItemName,appsetting.ItemValue = Table_copy.ItemValue from appsetting,Table_copy where appsetting.Id=Table_copy.Id";
                com.ExecuteNonQuery();
            }

            con_file_path = @"D:\vs workspace\ColumnReflectSettings.json";
            using (StreamReader sr = new StreamReader(con_file_path))
            {
                string str = sr.ReadToEnd();
                str = str.Replace("\n", "");
                List<ColumnReflectSettings> dwobj = (List<ColumnReflectSettings>)JsonConvert.DeserializeObject(str, typeof(List<ColumnReflectSettings>));

                string drop = "truncate table Table_copy";
                SqlCommand com = new SqlCommand(drop, conn);
                com.ExecuteNonQuery();
                com.CommandText = "insert into Table_copy select Id,Groupno,ItemName,ItemValue from appsetting where appsetting.FilePath= 'ColumnReflectSettings'";
                com.ExecuteNonQuery();
                com.Parameters.Add(new SqlParameter("@ItemName", SqlDbType.NChar, 50));
                com.Parameters.Add(new SqlParameter("@ItemValue", SqlDbType.NChar, 50));
                com.CommandText = "update Table_copy set ItemValue = @ItemValue where ItemName = @ItemName and Groupno = @Groupno";
                com.Parameters.Add(new SqlParameter("@Groupno", SqlDbType.NChar, 50));

                Type tp = dwobj[0].GetType();
                foreach (PropertyInfo t in tp.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "Items";
                    com.Parameters["@ItemValue"].Value = t.GetValue(dwobj[0], null);
                    com.Parameters["@ItemName"].Value = t.Name;
                    com.ExecuteNonQuery();
                }

                Type tp1 = dwobj[1].GetType();
                foreach (PropertyInfo t1 in tp1.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "Items1";
                    com.Parameters["@ItemValue"].Value = t1.GetValue(dwobj[1], null);
                    com.Parameters["@ItemName"].Value = t1.Name;
                    com.ExecuteNonQuery();
                }
               
                com.CommandText = "update appsetting set appsetting.ItemName = Table_copy.ItemName,appsetting.ItemValue = Table_copy.ItemValue from appsetting,Table_copy where appsetting.Id=Table_copy.Id";
                com.ExecuteNonQuery();
            }

            con_file_path = @"D:\vs workspace\ColumnUpdateSettings.json";
            using (StreamReader sr = new StreamReader(con_file_path))
            {
                string str = sr.ReadToEnd();
                str = str.Replace("\r", "");
                str = str.Replace("\n", "");
                List<ColumnUpdateSettings> dwobj = (List<ColumnUpdateSettings>)JsonConvert.DeserializeObject(str, typeof(List<ColumnUpdateSettings>));

                string drop = "truncate table Table_copy";
                SqlCommand com = new SqlCommand(drop, conn);
                com.ExecuteNonQuery();
                com.CommandText = "insert into Table_copy select Id,Groupno,ItemName,ItemValue from appsetting where appsetting.FilePath= 'ColumnUpdateSettings'";
                com.ExecuteNonQuery();
                com.Parameters.Add(new SqlParameter("@ItemName", SqlDbType.NChar, 50));
                com.Parameters.Add(new SqlParameter("@ItemValue", SqlDbType.NChar, 50));
                com.CommandText = "update Table_copy set ItemValue = @ItemValue where ItemName = @ItemName and Groupno = @Groupno";
                com.Parameters.Add(new SqlParameter("@Groupno", SqlDbType.NChar, 50));

                Type tp = dwobj[0].GetType();
                foreach (PropertyInfo t in tp.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "体温";
                    com.Parameters["@ItemValue"].Value = t.GetValue(dwobj[0], null);
                    com.Parameters["@ItemName"].Value = t.Name;
                    com.ExecuteNonQuery();
                }

                Type tp1 = dwobj[1].GetType();
                foreach (PropertyInfo t1 in tp1.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "脉搏";
                    com.Parameters["@ItemValue"].Value = t1.GetValue(dwobj[1], null);
                    com.Parameters["@ItemName"].Value = t1.Name;
                    com.ExecuteNonQuery();
                }
                Type tp2 = dwobj[2].GetType();
                foreach (PropertyInfo t2 in tp2.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "呼吸";
                    com.Parameters["@ItemValue"].Value = t2.GetValue(dwobj[2], null);
                    com.Parameters["@ItemName"].Value = t2.Name;
                    com.ExecuteNonQuery();
                }
                Type tp3 = dwobj[3].GetType();
                foreach (PropertyInfo t3 in tp3.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "收缩压";
                    com.Parameters["@ItemValue"].Value = t3.GetValue(dwobj[3], null);
                    com.Parameters["@ItemName"].Value = t3.Name;
                    com.ExecuteNonQuery();
                }
                Type tp4 = dwobj[4].GetType();
                foreach (PropertyInfo t4 in tp4.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "扩展压";
                    com.Parameters["@ItemValue"].Value = t4.GetValue(dwobj[4], null);
                    com.Parameters["@ItemName"].Value = t4.Name;
                    com.ExecuteNonQuery();
                }
                Type tp5 = dwobj[5].GetType();
                foreach (PropertyInfo t5 in tp5.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "氧饱和度";
                    com.Parameters["@ItemValue"].Value = t5.GetValue(dwobj[5], null);
                    com.Parameters["@ItemName"].Value = t5.Name;
                    com.ExecuteNonQuery();
                }
                Type tp6 = dwobj[6].GetType();
                foreach (PropertyInfo t6 in tp6.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "神志";
                    com.Parameters["@ItemValue"].Value = t6.GetValue(dwobj[6], null);
                    com.Parameters["@ItemName"].Value = t6.Name;
                    com.ExecuteNonQuery();
                }
                Type tp7 = dwobj[7].GetType();
                foreach (PropertyInfo t7 in tp7.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "瞳孔左";
                    com.Parameters["@ItemValue"].Value = t7.GetValue(dwobj[7], null);
                    com.Parameters["@ItemName"].Value = t7.Name;
                    com.ExecuteNonQuery();
                }
                Type tp8 = dwobj[8].GetType();
                foreach (PropertyInfo t8 in tp8.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "瞳孔右";
                    com.Parameters["@ItemValue"].Value = t8.GetValue(dwobj[8], null);
                    com.Parameters["@ItemName"].Value = t8.Name;
                    com.ExecuteNonQuery();
                }
                Type tp9 = dwobj[9].GetType();
                foreach (PropertyInfo t9 in tp9.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "对光反应左";
                    com.Parameters["@ItemValue"].Value = t9.GetValue(dwobj[9], null);
                    com.Parameters["@ItemName"].Value = t9.Name;
                    com.ExecuteNonQuery();
                }
                Type tp10 = dwobj[10].GetType();
                foreach (PropertyInfo t10 in tp10.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "对光反应右";
                    com.Parameters["@ItemValue"].Value = t10.GetValue(dwobj[10], null);
                    com.Parameters["@ItemName"].Value = t10.Name;
                    com.ExecuteNonQuery();
                }
                Type tp11 = dwobj[11].GetType();
                foreach (PropertyInfo t11 in tp11.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "吸氧";
                    com.Parameters["@ItemValue"].Value = t11.GetValue(dwobj[11], null);
                    com.Parameters["@ItemName"].Value = t11.Name;
                    com.ExecuteNonQuery();
                }
                Type tp12 = dwobj[12].GetType();
                foreach (PropertyInfo t12 in tp12.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "吸痰";
                    com.Parameters["@ItemValue"].Value = t12.GetValue(dwobj[12], null);
                    com.Parameters["@ItemName"].Value = t12.Name;
                    com.ExecuteNonQuery();
                }
                Type tp13 = dwobj[13].GetType();
                foreach (PropertyInfo t13 in tp13.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "雾化吸入";
                    com.Parameters["@ItemValue"].Value = t13.GetValue(dwobj[13], null);
                    com.Parameters["@ItemName"].Value = t13.Name;
                    com.ExecuteNonQuery();
                }
                Type tp14 = dwobj[14].GetType();
                foreach (PropertyInfo t14 in tp14.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "皮肤情况";
                    com.Parameters["@ItemValue"].Value = t14.GetValue(dwobj[14], null);
                    com.Parameters["@ItemName"].Value = t14.Name;
                    com.ExecuteNonQuery();
                }
                Type tp15 = dwobj[15].GetType();
                foreach (PropertyInfo t15 in tp15.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "输液";
                    com.Parameters["@ItemValue"].Value = t15.GetValue(dwobj[15], null);
                    com.Parameters["@ItemName"].Value = t15.Name;
                    com.ExecuteNonQuery();
                }
                Type tp16 = dwobj[16].GetType();
                foreach (PropertyInfo t16 in tp16.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "输液量";
                    com.Parameters["@ItemValue"].Value = t16.GetValue(dwobj[16], null);
                    com.Parameters["@ItemName"].Value = t16.Name;
                    com.ExecuteNonQuery();
                }
                Type tp17 = dwobj[17].GetType();
                foreach (PropertyInfo t17 in tp17.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "出量";
                    com.Parameters["@ItemValue"].Value = t17.GetValue(dwobj[17], null);
                    com.Parameters["@ItemName"].Value = t17.Name;
                    com.ExecuteNonQuery();
                }
                Type tp18 = dwobj[18].GetType();
                foreach (PropertyInfo t18 in tp18.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "出量值";
                    com.Parameters["@ItemValue"].Value = t18.GetValue(dwobj[18], null);
                    com.Parameters["@ItemName"].Value = t18.Name;
                    com.ExecuteNonQuery();
                }
                Type tp19 = dwobj[19].GetType();
                foreach (PropertyInfo t19 in tp19.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "卧位";
                    com.Parameters["@ItemValue"].Value = t19.GetValue(dwobj[19], null);
                    com.Parameters["@ItemName"].Value = t19.Name;
                    com.ExecuteNonQuery();
                }
                Type tp20 = dwobj[20].GetType();
                foreach (PropertyInfo t20 in tp20.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "饮食和水";
                    com.Parameters["@ItemValue"].Value = t20.GetValue(dwobj[20], null);
                    com.Parameters["@ItemName"].Value = t20.Name;
                    com.ExecuteNonQuery();
                }
                Type tp21 = dwobj[21].GetType();
                foreach (PropertyInfo t21 in tp21.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "饮食水量值";
                    com.Parameters["@ItemValue"].Value = t21.GetValue(dwobj[21], null);
                    com.Parameters["@ItemName"].Value = t21.Name;
                    com.ExecuteNonQuery();
                }
                Type tp22 = dwobj[22].GetType();
                foreach (PropertyInfo t22 in tp22.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "管路护理";
                    com.Parameters["@ItemValue"].Value = t22.GetValue(dwobj[22], null);
                    com.Parameters["@ItemName"].Value = t22.Name;
                    com.ExecuteNonQuery();
                }
                Type tp23 = dwobj[23].GetType();
                foreach (PropertyInfo t23 in tp23.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "生活护理";
                    com.Parameters["@ItemValue"].Value = t23.GetValue(dwobj[23], null);
                    com.Parameters["@ItemName"].Value = t23.Name;
                    com.ExecuteNonQuery();
                }
                Type tp24 = dwobj[24].GetType();
                foreach (PropertyInfo t24 in tp24.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "小结时间";
                    com.Parameters["@ItemValue"].Value = t24.GetValue(dwobj[24], null);
                    com.Parameters["@ItemName"].Value = t24.Name;
                    com.ExecuteNonQuery();
                }
                Type tp25 = dwobj[25].GetType();
                foreach (PropertyInfo t25 in tp25.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "小结入量";
                    com.Parameters["@ItemValue"].Value = t25.GetValue(dwobj[25], null);
                    com.Parameters["@ItemName"].Value = t25.Name;
                    com.ExecuteNonQuery();
                }
                Type tp26 = dwobj[26].GetType();
                foreach (PropertyInfo t26 in tp26.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "小结出量";
                    com.Parameters["@ItemValue"].Value = t26.GetValue(dwobj[26], null);
                    com.Parameters["@ItemName"].Value = t26.Name;
                    com.ExecuteNonQuery();
                }
                Type tp27 = dwobj[27].GetType();
                foreach (PropertyInfo t27 in tp27.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "说明性小结";
                    com.Parameters["@ItemValue"].Value = t27.GetValue(dwobj[27], null);
                    com.Parameters["@ItemName"].Value = t27.Name;
                    com.ExecuteNonQuery();
                }
                Type tp28 = dwobj[28].GetType();
                foreach (PropertyInfo t28 in tp28.GetProperties())
                {
                    com.Parameters["@Groupno"].Value = "病情记录";
                    com.Parameters["@ItemValue"].Value = t28.GetValue(dwobj[28], null);
                    com.Parameters["@ItemName"].Value = t28.Name;
                    com.ExecuteNonQuery();
                }

                com.CommandText = "update appsetting set appsetting.ItemName = Table_copy.ItemName,appsetting.ItemValue = Table_copy.ItemValue from appsetting,Table_copy where appsetting.Id=Table_copy.Id";
                com.ExecuteNonQuery();
            }
            

            //for (int i = 1; i <= dataWindowExt_menu.RowCount; i++)
            //{
            //    path = dataWindowExt_menu.GetItemString(i, 1);
            //    con_file_path = @"D:\vs worksapce\" + path + ".json";
            //    //con_file_path = @"D:\meeHealth\output\x86\Debug\config\新建文件夹" + path + ".json";
            //    if (filepath != con_file_path)
            //    {
            //        //不相等，打开，更新数据库
            //        using (StreamReader sr = new StreamReader(con_file_path))
            //        {
            //            string str;
            //            while ((str = sr.ReadLine()) != null)
            //            {
            //                var config = JsonConvert.DeserializeObject<Config_group>(str);
            //                Convert.ToDateTime(config.creationTime).ToString("yyyy-mm-dd");
            //                Convert.ToDateTime(config.modificationTime).ToString("yyyy-mm-dd");
            //                string sqlUpdate = "update appsetting set FilePath = '" + config.filePath + "',Groupno = '" + config.groupno + "',ItemName = '" + config.itemName + "',ItemValue = '" + config.itemValue + "',Name='" + config.name + "',Description = '" + config.description +
            //                  "',Example='" + config.example + "',IsActive='" + config.isActive + "',IsDeleted='" + config.isDeleted + "',ParentId='" + config.parentId + "',Creator='" + config.creator +
            //                    "',CreationTime='" + config.creationTime + "',Modifier='" + config.modifier + "',ModificationTime='" + config.modificationTime + "'where Id = '" + config.id + "'";                          
            //                SqlCommand cmdUp = new SqlCommand(sqlUpdate, conn);
            //                cmdUp.ExecuteNonQuery();
            //            }
            //        }
            //    }
            //    filepath = con_file_path;
            //}
        }

        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)    //右键删除选项
        {
            int curentRow = dataWindowExt_menu.CurrentRow;
            if (curentRow <= 0)
            {
                MessageBox.Show("没有可删除的数据！");
                return;
            }
            if (MessageBox.Show("确定删除？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                dataWindowExt_menu.Focus();
                return;
            }
            dataWindowExt_menu.DeleteRow(dataWindowExt_menu.CurrentRow);
            dataWindowExt_menu.AcceptText();
            openDataBase();
            dataWindowExt_menu.UpdateData(true, true);
            tojson();
            dataWindowExt_menu.SetFilter("");
            dataWindowExt_menu.Filter();
            treeView1.Nodes.Clear();
            tree_load();
            if (t_group == "" && t_name != "" && t_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + t_file + "' and name = '" + t_name + "' ");
            if (t_group == "" && t_name == "" && t_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'");
            if (t_group == "" && t_name != "" && t_file == "")
                dataWindowExt_menu.SetFilter("name = '" + t_name + "'");
            if (t_group != "" && t_name == "" && t_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'and groupno = '" + t_group + "' ");
            if (t_group == "" && t_name == "" && t_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + t_file + "' ");
            if (t_group != "" && t_name == "" && t_file == "")
                dataWindowExt_menu.SetFilter("groupno = '" + t_group + "' ");
            if (t_group != "" && t_name != "" && t_file == "")
                dataWindowExt_menu.SetFilter("groupno = '" + t_group + "' and name = '" + t_name + "' ");
            if (t_group != "" && t_name == "" && t_file == "")
                dataWindowExt_menu.SetFilter(" groupno = '" + t_group + "' ");
            if (t_group == "" && t_name != "" && t_file == "")
                dataWindowExt_menu.SetFilter(" name = '" + t_name + "' ");
            if (t_group != "" && t_name != "" && t_file != "")
                dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'and groupno = '" + t_group + "' and name = '" + t_name + "' ");
            conn.Close();
        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)    //右键添加
        {
            int currentRow = dataWindowExt_menu.CurrentRow;
            dataWindowExt_menu.InsertRow(dataWindowExt_menu.CurrentRow);
            isAdd = 1;
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)      //右键保存
        {
            openDataBase();
                dataWindowExt_menu.SetFilter("");
            dataWindowExt_menu.Filter();
            dataWindowExt_menu.AcceptText();
            if ((dataWindowExt_menu.ModifiedCount + dataWindowExt_menu.DeletedCount) == 0)
            {
                MessageBox.Show("没有数据需要保存!");
                dataWindowExt_menu.Focus();
                return;
            }
            try
            {
                for (int i = 1; i <= dataWindowExt_menu.RowCount; i++)
                {
                    int flag = 0;
                    for (int j = i + 1; j <= dataWindowExt_menu.RowCount; j++)
                    {
                        string str_g = dataWindowExt_menu.GetItemString(i, 1);
                        string str_g1 = dataWindowExt_menu.GetItemString(j, 1);
                        string str = dataWindowExt_menu.GetItemString(i, 4);
                        string str1 = dataWindowExt_menu.GetItemString(j, 4);
                        string strg = dataWindowExt_menu.GetItemString(i, 2);
                        string strg1 = dataWindowExt_menu.GetItemString(j, 2);

                        if (str_g == str_g1 && strg == strg1 && str == str1)
                        {
                            MessageBox.Show("存在重复数据！！");
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'and groupno = '" + t_group + "' and name = '" + t_name + "' ");
                            dataWindowExt_menu.Filter();
                            flag = 1;
                            break;
                        }
                    }
                    if (i == dataWindowExt_menu.RowCount)
                    {
                        dataWindowExt_menu.UpdateData(true, true);
                        MessageBox.Show("保存成功");
                        dataWindowExt_menu.Retrieve(new Object[] { });
                        tojson();
                        treeView1.Nodes.Clear();
                        tree_load();
                        if (t_group == "" && t_name != "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "' and name = '" + t_name + "' ");
                        if (t_group == "" && t_name == "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'");
                        if (t_group == "" && t_name != "" && t_file == "")
                            dataWindowExt_menu.SetFilter("name = '" + t_name + "'");
                        if (t_group != "" && t_name == "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'and groupno = '" + t_group + "' ");
                        if (t_group == "" && t_name == "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "' ");
                        if (t_group != "" && t_name == "" && t_file == "")
                            dataWindowExt_menu.SetFilter("groupno = '" + t_group + "' ");
                        if (t_group != "" && t_name != "" && t_file == "")
                            dataWindowExt_menu.SetFilter("groupno = '" + t_group + "' and name = '" + t_name + "' ");
                        if (t_group != "" && t_name == "" && t_file == "")
                            dataWindowExt_menu.SetFilter(" groupno = '" + t_group + "' ");
                        if (t_group == "" && t_name != "" && t_file == "")
                            dataWindowExt_menu.SetFilter(" name = '" + t_name + "' ");
                        if (t_group != "" && t_name != "" && t_file != "")
                            dataWindowExt_menu.SetFilter("filepath = '" + t_file + "'and groupno = '" + t_group + "' and name = '" + t_name + "' ");
                        dataWindowExt_menu.Filter();
                        break;
                    }
                    if (flag == 1)
                        break;
                }
            }
            catch (Exception ee)
            {
                //    // Logger.Instance.Error(ee.Message);
            }
            try
            {
                conn.Close();
            }
            catch (Exception eee)
            {
                //Logger.Instance.Error(eee.Message);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            openDataBase();
            updateDb();
            try
            {
                dataWindowExt_menu.Retrieve(new Object[] { });
            }
            catch (Exception ee)
            {
                // Logger.Instance.Error(ee.Message);
            }
            try
            {
                conn.Close();
            }
            catch (Exception eee)
            {
                //Logger.Instance.Error(eee.Message);
            }
            // treeView1.Nodes.Clear();
            //tree_load();
            conn.Close();
        }
    }
}
