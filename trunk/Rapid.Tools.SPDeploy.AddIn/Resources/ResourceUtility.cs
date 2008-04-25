using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace Rapid.Tools.SPDeploy.AddIn.Resources
{
    public class ResourceUtility
    {
        private const string ICON = "icon";
        public static void SetFileNodeIcon(TreeNode n, bool checkedOut)
        {
            if (n.TreeView.ImageList == null)
                n.TreeView.ImageList = new ImageList();

            string fileExt = n.Text.Split('.')[1].ToLower();
            string key = string.Concat(fileExt, ICON);
            if (checkedOut)
                key += "checkedout";
            if (!n.TreeView.ImageList.Images.ContainsKey(key))
            {
                foreach (string st in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                {
                    if (!st.EndsWith(".GIF")) continue;
                    string iname = st.Remove(st.LastIndexOf("."));
                    iname = iname.Substring(iname.LastIndexOf(".") + 1);
                    if (!iname.StartsWith("IC")) continue;
                    iname = iname.Substring(2);
                    if (iname.ToLower() == fileExt)
                    {
                        Image icon = Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(st));
                        n.TreeView.ImageList.Images.Add(fileExt + "icon", icon);
                        n.TreeView.ImageList.Images.Add(fileExt + "iconcheckedout", CreateCheckoutImage(icon));
                        break;
                    }
                }
                if (!n.TreeView.ImageList.Images.ContainsKey(key))
                {


                    key = "genericicon";
                    if (checkedOut) key += "checkedout";
                    if (!n.TreeView.ImageList.Images.ContainsKey("genericicon"))
                        n.TreeView.ImageList.Images.Add("genericicon", Resources.Images.Files.ICGEN);
                    if (!n.TreeView.ImageList.Images.ContainsKey("genericiconcheckedout"))
                        n.TreeView.ImageList.Images.Add("genericiconcheckedout", Resources.Images.Files.ICGEN);
                }

            }

            n.SelectedImageKey = n.ImageKey = key;


        }      

        public static Bitmap CreateCheckoutImage(Image original)
        {
            Bitmap i = new Bitmap(original);

            Bitmap b = new Bitmap(i.Width + 2, i.Height + 2);

            for (int x = 0; x < i.Width; x++)
            {
                for (int z = 0; z < i.Height; z++)
                {
                    b.SetPixel(x, z, i.GetPixel(x, z));
                }
            }

            Bitmap i2 = new Bitmap(Resources.Images.Files.checkoutoverlay);

            for (int x = 1; x <= i2.Width; x++)
            {
                for (int z = 1; z <= i2.Height; z++)
                {
                    b.SetPixel(b.Width - x, b.Height - z, i2.GetPixel(i2.Width - x, i2.Height - z));
                }
            }
            return b;
        }
    }
}
