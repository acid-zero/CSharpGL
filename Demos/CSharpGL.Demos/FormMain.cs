﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpGL.Demos
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnForm00GLCanvas_Click(object sender, EventArgs e)
        {
            (new Form00GLCanvas()).Show();
        }

        private void btnForm01ModernRenderer_Click(object sender, EventArgs e)
        {
            (new Form01ModernRenderer()).Show();
        }

        private void btnGeometryShader_Click(object sender, EventArgs e)
        {
            (new Form02GeometryShader()).Show();
        }
    }
}