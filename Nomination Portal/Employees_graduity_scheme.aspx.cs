﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using MySql.Data.MySqlClient;


namespace Nomination_Portal
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        string conn = @"Data Source=localhost; Database=nomination_portal; User ID=root; Password='shubham3597'";
        private int numOfRows = 1;
        int sum_nom_share = 0;
        int sno = 0;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                CheckMySqlConnection();
                MultiView1.SetActiveView(View1);
               
            }
            if (!Page.IsPostBack)

            {

                DataTable nom_dt = new DataTable();

                nom_dt.Columns.Add("Name");

                nom_dt.Columns.Add("Address");

                nom_dt.Columns.Add("Relationship");

                nom_dt.Columns.Add("Date of Birth");

                nom_dt.Columns.Add("Nominee Share");

                nom_dt.Columns.Add("Invalid Contengencies");

                ViewState["nom_dt"] = nom_dt;

                GenerateTable(numOfRows);
                MultiView1.SetActiveView(View1);

                TextBox1.Text = "" + Session["form"].ToString();

               


            }
        }

        private void CheckMySqlConnection()
        {
            using (MySqlConnection conn1 = new MySqlConnection(conn))
            {
                conn1.Open();
                Response.Write("Successfull");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (ViewState["RowsCount"] != null)

            {
                text_total_share.Text = "" + sum_nom_share;

                DataTable nom_dt = (DataTable)ViewState["nom_dt"];

                numOfRows = Convert.ToInt32(ViewState["RowsCount"].ToString());

                DataRow drow = nom_dt.NewRow();

                //drow["Status"] = "Added";

                drow["Name"] = text_name.Text;

                drow["Address"] = test_address.Text;

                drow["Relationship"] = relation_drop.SelectedValue.ToString();

                drow["Date of Birth"] = text_dob.Text;

                drow["Nominee Share"] = text_share.Text;

                drow["Invalid Contengencies"] = text_cont.Text;

                nom_dt.Rows.Add(drow);

                nom_dt.AcceptChanges();

                ViewState["nom_dt"] = nom_dt;

                GridViewNom.DataSource = nom_dt;

                GridViewNom.DataBind();
                // GenerateTable(numOfRows);



                int rowscount = GridViewNom.Rows.Count;

                for (int i = 0; i < rowscount; i++)
                {
                    string abc = nom_dt.Rows[i]["Nominee Share"].ToString();

                    int x = Convert.ToInt32(abc);

                    sum_nom_share = sum_nom_share + x;
                    if (sum_nom_share <= 100)
                    {
                        text_total_share.Text = "" + sum_nom_share;
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Nomination not added as you might have extended the total share limit (i.e. 100%)');</script>");
                        nom_dt.Rows[nom_dt.Rows.Count - 1].Delete();
                        nom_dt.AcceptChanges();
                        ViewState["nom_dt"] = nom_dt;

                        GridViewNom.DataSource = nom_dt;

                        GridViewNom.DataBind();

                        break;


                    }
                }


            }


        }
        protected void SetPreviousData(int rowsCount, int colsCount)
        {
            Table table = (Table)Page.FindControl("Table1");

            //if (table != null)
            /*{
                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < colsCount; j++)

                    {

                        //Extracting the Dynamic Controls from the Table

                        TextBox tb = (TextBox)table.Rows[i].Cells[j].FindControl("TextBoxRow_" + i + "Col_" + j);

                        //Use Request objects for getting the previous data of the dynamic textbox

                        tb.Text = Request.Form["TextBoxRow_" + i + "Col_" + j];

                    }

                }

            }*/

        }
        protected void GenerateTable(int rowsCount)

        {
            //Creat the Table and Add it to the Page

            //The number of Columns to be generated
            const int colsCount = 1;//You can changed the value of 3 based on you requirements
            // Now iterate through the table and add your controls
            for (int i = 0; i < rowsCount; i++)
            {
                TableRow tr = new TableRow();

                // for (int j = 0; j < colsCount; j++)
                // {
                //TableCell status = new TableCell();

                //tr.Cells.Add(status);

                TableCell name = new TableCell();
                name.Text = text_name.Text;
                //tr.Cells.Add(name);

                TableCell rel = new TableCell();
                rel.Text = relation_drop.SelectedValue.ToString();
                //tr.Cells.Add(rel);

                TableCell dob = new TableCell();
                dob.Text = text_dob.Text;
                //tr.Cells.Add(dob);

                TableCell nom = new TableCell();
                nom.Text = text_share.Text;
                //tr.Cells.Add(nom);

                TableCell cont = new TableCell();
                cont.Text = text_cont.Text;
                //tr.Cells.Add(cont);
                // }
                //Table2.Rows.Add(tr);
            }

            // And finally, add the TableRow to the Table

            //Set Previous Data on PostBacks

            SetPreviousData(rowsCount, colsCount);

            //Sore the current Rows Count in ViewState

            rowsCount++;

            ViewState["RowsCount"] = rowsCount;
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            String text_check = text_total_share.Text.Trim();
            int total_share_check = Convert.ToInt32(text_check);
            if (total_share_check == 100)
            {
                MySqlConnection conn1 = new MySqlConnection(conn);
                conn1.Open();
                MySqlCommand cmd = conn1.CreateCommand();
                cmd.CommandType = CommandType.Text;
                for (int i = 0; i < GridViewNom.Rows.Count; i++)
                {
                    cmd.CommandText = "insert into nom_nomination values(" + Convert.ToInt32(Session["ID"].ToString()) + ", '" + GridViewNom.Rows[i].Cells[0].Text + "', '" + GridViewNom.Rows[i].Cells[1].Text + "', '" + GridViewNom.Rows[i].Cells[2].Text + "', '" + (Convert.ToDateTime(GridViewNom.Rows[i].Cells[3].Text)).Year + "-" + (Convert.ToDateTime(GridViewNom.Rows[i].Cells[3].Text)).Month + "-" + (Convert.ToDateTime(GridViewNom.Rows[i].Cells[3].Text)).Day + "'," + Convert.ToInt32(GridViewNom.Rows[i].Cells[4].Text) + ", '" + GridViewNom.Rows[i].Cells[5].Text + "', " + (i + 1) +", " + 0 + ")";
                    cmd.ExecuteNonQuery();
                }
                
                
                //Response.Redirect("WebForm6.aspx?");

            }

            else {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Please complete your total share!');</script>");

            }
            
            MultiView1.SetActiveView(View4);
            
        }
        protected void Button1_Click1(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection conn1 = new MySqlConnection(conn);
                conn1.Open();
                MySqlCommand cmd = conn1.CreateCommand();
                MySqlCommand cmd1 = conn1.CreateCommand();

                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "insert into nom_alt_nomination values(" + Convert.ToInt32(Session["ID"].ToString()) + ", '" + "null" + "', '" + "null" + "', '" + "null" + "','" + "0000-00-00" + "' ," + 0 + ", '" + "null" + "', " + 1 + ", " + 0 + ")";
                cmd.ExecuteNonQuery();
                cmd1.CommandText = "select count(*) from nom_nomination where dob >= date_sub(now(), interval 18 year)";

                MySqlDataReader mdr = cmd1.ExecuteReader();
                mdr.Read();

                if (mdr.GetInt32("count(*)") > 0)
                {
                    Response.Redirect("Add Guardian.aspx?" + mdr.GetInt32("count(*)"));
                }

            }
            catch (HttpException h)
            {

                h.GetHtmlErrorMessage();


            }
            catch (MySqlException m)
            {
                m.GetBaseException();
            }
            
        }
        protected void Button_preview_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(View2);
        }

        protected void Button_final_save_Click(object sender, EventArgs e)
        {
            
        }

        protected void Button7_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(View2);

        }

        protected void Button8_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(Session["ID"].ToString());
            
            MultiView1.SetActiveView(View2);
        
                MySqlConnection conn1 = new MySqlConnection(conn);
                conn1.Open();
            MySqlCommand cmd = new MySqlCommand("update nom_nomination Set status = @status where TRNS_ID=@trns_id", conn1); 
cmd.Parameters.AddWithValue("@status", ""+1);
            cmd.Parameters.AddWithValue("@trns_id", id);
           
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Scripts", "<script>alert('Data Saved, Sucessfully!');</script>");


            
            
        }

        protected void Button9_Click(object sender, EventArgs e)
        {
            MultiView1.SetActiveView(View1);
        }
    }

}