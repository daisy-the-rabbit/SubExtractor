using System.ComponentModel;
using System.Data;

namespace DvdSubExtractor;

public partial class CommentBox : Form
{
    public CommentBox()
    {
        InitializeComponent();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public String Comment
    {
        get
        {
            return this.commentTextBox.Text;
        }
        set
        {
            this.commentTextBox.Text = value;
        }
    }
}

