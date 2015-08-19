﻿using CSharpGL.Maths;
using CSharpGL.Objects;
using CSharpGL.Objects.Cameras;
using CSharpGL.Objects.SceneElements;
using CSharpGL.Objects.Shaders;
using CSharpGL.Objects.UI.SimpleUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpGL.Winforms.Demo
{
    public partial class FormSimplePointSprite : Form
    {
        SimpleUIAxis uiLeftBottomAxis;
        SimpleUIAxis uiLeftTopAxis;
        SimpleUIAxis uiRightBottomAxis;
        SimpleUIAxis uiRightTopAxis;

        SimplePointSpriteElement pointSpriteElement;

        ScientificCamera camera;

        SatelliteRotator satelliteRoration;

        public FormSimplePointSprite(float fontSize, bool foreshortening, FragShaderType type)
        {
            InitializeComponent();

            if (CameraDictionary.Instance.ContainsKey(this.GetType().Name))
            {
                this.camera = CameraDictionary.Instance[this.GetType().Name];
            }
            else
            {
                this.camera = new ScientificCamera(CameraTypes.Ortho, this.glCanvas1.Width, this.glCanvas1.Height);
                CameraDictionary.Instance.Add(this.GetType().Name, this.camera);
            }

            satelliteRoration = new SatelliteRotator(camera);

            Padding padding = new System.Windows.Forms.Padding(40, 40, 40, 40);
            Size size = new Size(100, 100);
            //Size size = new Size(5, 5);
            IUILayoutParam param;
            param = new IUILayoutParam(AnchorStyles.Left | AnchorStyles.Bottom, padding, size);
            uiLeftBottomAxis = new SimpleUIAxis(param);
            param = new IUILayoutParam(AnchorStyles.Left | AnchorStyles.Top, padding, size);
            uiLeftTopAxis = new SimpleUIAxis(param);
            param = new IUILayoutParam(AnchorStyles.Right | AnchorStyles.Bottom, padding, size);
            uiRightBottomAxis = new SimpleUIAxis(param);
            param = new IUILayoutParam(AnchorStyles.Right | AnchorStyles.Top, padding, size);
            uiRightTopAxis = new SimpleUIAxis(param);

            uiLeftBottomAxis.Initialize();
            uiLeftTopAxis.Initialize();
            uiRightBottomAxis.Initialize();
            uiRightTopAxis.Initialize();

            uiLeftBottomAxis.BeforeRendering += SimpleUIAxis_BeforeRendering;
            uiLeftTopAxis.BeforeRendering += SimpleUIAxis_BeforeRendering;
            uiRightBottomAxis.BeforeRendering += SimpleUIAxis_BeforeRendering;
            uiRightTopAxis.BeforeRendering += SimpleUIAxis_BeforeRendering;

            uiLeftBottomAxis.AfterRendering += SimpleUIAxis_AfterRendering;
            uiLeftTopAxis.AfterRendering += SimpleUIAxis_AfterRendering;
            uiRightBottomAxis.AfterRendering += SimpleUIAxis_AfterRendering;
            uiRightTopAxis.AfterRendering += SimpleUIAxis_AfterRendering;

            pointSpriteElement = new SimplePointSpriteElement(fontSize, foreshortening, type);
            pointSpriteElement.Initialize();
            pointSpriteElement.BeforeRendering += pointSpriteElement_BeforeRendering;
            pointSpriteElement.AfterRendering += pointSpriteElement_AfterRendering;

            this.glCanvas1.MouseWheel += glCanvas1_MouseWheel;
            this.glCanvas1.KeyPress += glCanvas1_KeyPress;
            this.glCanvas1.MouseDown += glCanvas1_MouseDown;
            this.glCanvas1.MouseMove += glCanvas1_MouseMove;
            this.glCanvas1.MouseUp += glCanvas1_MouseUp;
            this.glCanvas1.OpenGLDraw += glCanvas1_OpenGLDraw;
            this.glCanvas1.Resize += glCanvas1_Resize;
        }

        void SimpleUIRect_AfterRendering(object sender, Objects.RenderEventArgs e)
        {
            SimpleUIRect element = sender as SimpleUIRect;

            element.shaderProgram.Unbind();
        }

        void SimpleUIRect_BeforeRendering(object sender, Objects.RenderEventArgs e)
        {
            SimpleUIRect element = sender as SimpleUIRect;

            mat4 projectionMatrix, viewMatrix, modelMatrix;

            element.GetMatrix(out projectionMatrix, out viewMatrix, out modelMatrix, this.camera);

            ShaderProgram shaderProgram = element.shaderProgram;

            shaderProgram.Bind();

            shaderProgram.SetUniformMatrix4(SimpleUIRect.strprojectionMatrix, projectionMatrix.to_array());
            shaderProgram.SetUniformMatrix4(SimpleUIRect.strviewMatrix, viewMatrix.to_array());
            shaderProgram.SetUniformMatrix4(SimpleUIRect.strmodelMatrix, modelMatrix.to_array());
        }

        void SimpleUIAxis_AfterRendering(object sender, Objects.RenderEventArgs e)
        {
            SimpleUIAxis element = sender as SimpleUIAxis;

            element.axisElement.shaderProgram.Unbind();
        }

        void SimpleUIAxis_BeforeRendering(object sender, Objects.RenderEventArgs e)
        {
            SimpleUIAxis element = sender as SimpleUIAxis;

            mat4 projectionMatrix, viewMatrix, modelMatrix;

            element.GetMatrix(out projectionMatrix, out viewMatrix, out modelMatrix, this.camera);

            IMVP axisElement = element.axisElement;

            axisElement.UpdateMVP(projectionMatrix * viewMatrix * modelMatrix);
        }

        void pointSpriteElement_AfterRendering(object sender, Objects.RenderEventArgs e)
        {
            IMVP element = sender as IMVP;

            element.UnbindShaderProgram();
        }

        void pointSpriteElement_BeforeRendering(object sender, Objects.RenderEventArgs e)
        {
            mat4 projectionMatrix = camera.GetProjectionMat4();

            mat4 viewMatrix = camera.GetViewMat4();

            mat4 modelMatrix = mat4.identity();

            mat4 mvp = projectionMatrix * viewMatrix * modelMatrix;

            IMVP element = sender as IMVP;

            element.UpdateMVP(mvp);
        }

        private void glCanvas1_MouseWheel(object sender, MouseEventArgs e)
        {
            this.camera.MouseWheel(e.Delta);
        }

        private void FormTranslateOnScreen_Load(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("{0}",
                "Use 'c' to switch camera types between perspective and ortho"));
        }

        private void glCanvas1_OpenGLDraw(object sender, RenderEventArgs e)
        {
            PrintCameraInfo();

            GL.ClearColor(0x87 / 255.0f, 0xce / 255.0f, 0xeb / 255.0f, 0xff / 255.0f);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);

            pointSpriteElement.Render(Objects.RenderModes.Render);

            uiLeftBottomAxis.Render(Objects.RenderModes.Render);
            uiLeftTopAxis.Render(Objects.RenderModes.Render);
            uiRightBottomAxis.Render(Objects.RenderModes.Render);
            uiRightTopAxis.Render(Objects.RenderModes.Render);
        }

        private void glCanvas1_Resize(object sender, EventArgs e)
        {
            if (this.camera != null)
            {
                this.camera.Resize(this.glCanvas1.Width, this.glCanvas1.Height);
            }
        }


        private void glCanvas1_MouseDown(object sender, MouseEventArgs e)
        {
            satelliteRoration.SetBounds(this.glCanvas1.Width, this.glCanvas1.Height);
            satelliteRoration.MouseDown(e.X, e.Y);
        }

        private void glCanvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (satelliteRoration.mouseDownFlag)
            {
                satelliteRoration.MouseMove(e.X, e.Y);
            }
        }

        private void glCanvas1_MouseUp(object sender, MouseEventArgs e)
        {
            satelliteRoration.MouseUp(e.X, e.Y);
        }

        private void PrintCameraInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("position:{0}", this.camera.Position));
            builder.Append(string.Format(" target:{0}", this.camera.Target));
            builder.Append(string.Format(" up:{0}", this.camera.UpVector));
            builder.Append(string.Format(" camera type: {0}", this.camera.CameraType));

            this.txtInfo.Text = builder.ToString();
        }

        private void glCanvas1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'c')
            {
                switch (this.camera.CameraType)
                {
                    case CameraTypes.Perspecitive:
                        this.camera.CameraType = CameraTypes.Ortho;
                        break;
                    case CameraTypes.Ortho:
                        this.camera.CameraType = CameraTypes.Perspecitive;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}