using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using System.Collections.Generic;

namespace [REPLACEPROJECTNAME].Web.UI.WebControls
{
    public class [REPLACEWEBPARTFILENAME] : System.Web.UI.WebControls.WebParts.WebPart, IWebEditable
    {
        private string textProperty;

        [WebBrowsable]
        [Personalizable]
        [WebDisplayName("Text Property")]
        [WebDescription("A Text Property")]
        [Category("[REPLACEPROJECTNAME] Properties")]
        public string TextProperty
        {
            get { return textProperty; }
            set { textProperty = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);           
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }       

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);            
        }

        public override WebPartVerbCollection Verbs
        {
            get
            {
                WebPartVerb verb = new WebPartVerb("mMinimize", delegate(object sender, WebPartEventArgs e)
                {
                    this.ChromeState = PartChromeState.Minimized;
                });
                verb.Text = "Minimize";
                return new WebPartVerbCollection(base.Verbs, new WebPartVerb[] { verb });
            }
        }

        #region IWebEditable Members

        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            List<EditorPart> editorParts = new List<EditorPart>();        
            [REPLACEWEBPARTFILENAME]EditorPart editorPart = new [REPLACEWEBPARTFILENAME]EditorPart();
            editorPart.ID = "editorPart1";
            editorParts.Add(editorPart);
            return new EditorPartCollection(base.CreateEditorParts(), editorParts);
        }

        object IWebEditable.WebBrowsableObject
        {
            get { return this; }
        }

        #endregion      

        public class [REPLACEWEBPARTFILENAME]EditorPart : EditorPart
        {
            TextBox propertyTextBox;

            public override bool ApplyChanges()
            {
                EnsureChildControls();
                [REPLACEWEBPARTFILENAME] parentControl = ([REPLACEWEBPARTFILENAME])WebPartToEdit;
                parentControl.TextProperty = propertyTextBox.Text;
                return true;
            }

            public override void SyncChanges()
            {
                EnsureChildControls();
                [REPLACEWEBPARTFILENAME] parentControl = ([REPLACEWEBPARTFILENAME])WebPartToEdit;
                propertyTextBox.Text = parentControl.TextProperty;
            }

            protected override void OnInit(EventArgs e)
            {
                base.OnInit(e);
                this.Title = "[REPLACEWEBPARTFILENAME] Properties";
                propertyTextBox = new TextBox();
                propertyTextBox.ID = "txtBox1";
                this.Controls.Add(propertyTextBox);
            }

            protected override void RenderContents(HtmlTextWriter writer)
            {
                string labelTitle = "Add a text property.";
                string labelText = "Text Property";

                writer.Write(string.Format("<DIV class=\"UserSectionHead\"><LABEL style=\"PADDING-TOP:5px\" title=\"{0}\">{1}</LABEL></DIV>", labelTitle, labelText));
                writer.Write("<DIV class=\"UserSectionBody\"><DIV class=\"UserControlGroup\">");
                propertyTextBox.RenderControl(writer);
                writer.Write("<NOBR></NOBR></DIV></DIV><DIV class=\"UserDottedLine\" style=\"WIDTH: 100%\"></DIV>");
            }          
        }   
    }
}