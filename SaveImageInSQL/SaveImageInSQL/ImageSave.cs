using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SaveImageInSQL
{
    public partial class ImageSave : Form
    {
        //------------------------------------------------------------------------------------------------------------------------------------//

        public static string Connectionstring;
        SqlConnection DBConnection;
        byte[] ImageData;

        public ImageSave()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private void ImageSave_Load(object sender, EventArgs e)
        {
            Connectionstring = ConfigurationManager.ConnectionStrings["SQLCon"].ToString();
            DBConnection = new SqlConnection(Connectionstring);
            getuniqueID();
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                Ofd.Title = "Select a file";
                Ofd.InitialDirectory = @"C:\";
                Ofd.Filter = "Select Image|*.jpg;*.jpeg;*.png;";
                Ofd.FileName = "Select a file";
                DialogResult result = Ofd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    txtImage.Text = Ofd.FileName;
                    Image img = Image.FromFile(Ofd.FileName);                    
                    Image ResizeImg = ResizeImage(img, 221, 150);
                    ImageData = (byte[])(new ImageConverter()).ConvertTo(ResizeImg, typeof(byte[]));
                    FileStream fs = new FileStream(Ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    long Length = ImageData.Length;
                    int Ibyte = fs.Read(ImageData, 0, Convert.ToInt32(Length));
                    fs.Close();

                    #region Commented because save image in a fix size so first resize then save
                    //FileInfo fiImage = new FileInfo(Ofd.FileName);
                    //FileStream fs = new FileStream(Ofd.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    //long Length = fiImage.Length;
                    //ImageData = new byte[Convert.ToInt32(Length)];
                    //int Ibyte = fs.Read(ImageData, 0, Convert.ToInt32(Length));
                    //fs.Close();
                    #endregion Commented because save image in a fix size so first resize then save
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }         
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private string getuniqueID()
        {
            DBConnection.Open();
            int regcount = -1;
            string strEMPId = "";
            string str = "select (MAX(SUBSTRING(EmpId,4,3))) from ImageSave";
            SqlCommand cmd = new SqlCommand(str, DBConnection);
            SqlDataReader dr = null;
            try
            {
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (!(dr.GetValue(0).ToString() == ""))
                        {
                            regcount = Convert.ToInt32(dr.GetString(0));
                            int regcountinc = regcount + 1;
                            if (regcountinc <= 9)
                            {
                                strEMPId = "EMP00" + regcountinc;
                            }
                            else
                            {
                                strEMPId = "EMP0" + regcountinc;
                            }
                        }
                        else
                        {
                            strEMPId = "EMP001";
                        }
                    }
                }
                else
                {
                    strEMPId = "STUREG001";
                }
                txtEmpID.Text = strEMPId;
                txtEmpID.ReadOnly = true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DBConnection.Close();
            }
            return strEMPId;
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DBConnection.Open();
                string Query = "Insert into ImageSave(EmpId, Name, ContactNo, Picture) values (@EmpId, @Name, @ContactNo, @Picture)";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = DBConnection;
                cmd.CommandText = Query;
                cmd.Parameters.AddWithValue("@EmpId", txtEmpID.Text);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                cmd.Parameters.AddWithValue("@Picture", ImageData);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBConnection.Close();
                ClearForm();
            }
            
        }

        //------------------------------------------------------------------------------------------------------------------------------------//

        private void ClearForm()
        {
            txtImage.Clear();
            txtContactNo.Clear();
            txtName.Clear();
            getuniqueID();
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadImage obj = new LoadImage();
            obj.ShowDialog();
        }

        //------------------------------------------------------------------------------------------------------------------------------------//
    }
}
