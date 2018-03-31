using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveImageInSQL
{
    public partial class LoadImage : Form
    {

        public static string Connectionstring;
        SqlConnection DBConnection;
        byte[] ImageData;

        //------------------------------------------------------------------------------------------------------------------------------------//

        public LoadImage()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private void LoadImage_Load(object sender, EventArgs e)
        {
            grpDetails.Visible = false;
            Connectionstring = ConfigurationManager.ConnectionStrings["SQLCon"].ToString();
            DBConnection = new SqlConnection(Connectionstring);
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                string EmpId = txtEmpID.Text;
                DBConnection.Open();
                string Query = "select * from ImageSave where EmpId = '" + EmpId + "'";
                SqlCommand cmd = new SqlCommand(Query, DBConnection);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        grpDetails.Visible = true;
                        byte[] Image1 = (byte[])(dr.GetValue(4));
                        string strfn = Convert.ToString(DateTime.Now.ToFileTime());
                        FileStream fs = new FileStream(strfn,
                                          FileMode.CreateNew, FileAccess.Write);
                        fs.Write(Image1, 0, Image1.Length);
                        fs.Flush();
                        fs.Close();
                        Image Pic = Image.FromFile(strfn);
                        pbImage.Image = ResizeImage(Pic, 221, 150);
                        pbImage.BackgroundImageLayout = ImageLayout.Stretch;
                        txtName.Text = Convert.ToString(dr.GetValue(2));
                        txtContactNo.Text = Convert.ToString(dr.GetValue(3));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBConnection.Close();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private Image ResizeImage(Image img, int iWidth, int iHeight)
        {
            Bitmap bmp = new Bitmap(iWidth, iHeight);
            Graphics graphic = Graphics.FromImage((Image)bmp);
            graphic.DrawImage(img, 0, 0, iWidth, iHeight);

            return (Image)bmp;
        }

        //------------------------------------------------------------------------------------------------------------------------------------//
    }
}
